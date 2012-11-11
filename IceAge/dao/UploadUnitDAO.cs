using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IceAge.type;
using System.Data.SQLite;
using System.Data;
using IceAge.exception;
using log4net;

namespace IceAge.dao
{
    // TODO: Refactor into smth more generic or use NHibernate?
    class UploadUnitDAO: GenericSQLiteDAO
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(UploadUnitDAO).FullName);

        public UploadUnitDAO(String filename): base(filename)
        {
        }

        // Need for opimistic locking?
        public static String getCreateStatement() {
            String create = "CREATE TABLE UPLOAD_ITEM" +
                "( ID INTEGER PRIMARY KEY, " +
                "  FILENAME TEXT, " +
                "  FILEPATH TEXT, " +
                "  SIZE INTEGER, " +
                "  MOD_TIMESTAMP INTEGER, " +
                "  UPLOADED_TIMESTAMP INTEGER," +
                "  CHECKSUM TEXT, " +
                " UNIQUE (FILEPATH)) ;";

            String createIndices =  "CREATE INDEX filepath_idx ON UPLOAD_ITEM(FILEPATH);" +
                                    "CREATE INDEX checksum_idx ON UPLOAD_ITEM(CHECKSUM);";
            return create + "\n" + createIndices;
        }

        public UploadUnit update(UploadUnit unit)
        {
            if (unit.Id == -1)
            {
                throw new BugInCodeException("Object tried to be updated is not obtained from db");
            }

            String query = "UPDATE UPLOAD_ITEM SET " + 
                           " FILENAME = @FILENAME, " +
                           " FILEPATH = @FILEPATH " + 
                           " SIZE = @SIZE" + 
                           " MOD_TIMESTAMP = @MOD_TIMESTAMP " +
                           " UPLOADED_TIMESTAMP = @UPLOADED_TIMESTAMP " + 
                           " CHECKSUM = @CHECKSUM)";
            TransactionalConnectionWrapper connection = getConnection();
            SQLiteCommand command = new SQLiteCommand(connection.Connection);
            command.CommandType = CommandType.Text;
            command.CommandText = query;
            command.Parameters.Add(new SQLiteParameter("@FILENAME", unit.FileName));
            command.Parameters.Add(new SQLiteParameter("@FILEPATH", unit.FullName));
            command.Parameters.Add(new SQLiteParameter("@SIZE", unit.Size.ToString()));
            command.Parameters.Add(new SQLiteParameter("@MOD_TIMESTAMP", unit.Timestamp.ToString()));
            command.Parameters.Add(new SQLiteParameter("@UPLOADED_TIMESTAMP", unit.Timestamp.ToString()));
            command.Parameters.Add(new SQLiteParameter("@CHECKSUM", unit.Checksum));
            try
            {
                command.ExecuteNonQuery();
            }
            finally
            {
                closeConnection(connection);
            }
            return getOrSaveUploadUnit(unit);
        }

        public UploadUnit getOrSaveUploadUnit(UploadUnit unit)
        {
            DataTable dt = getDataForUnit(unit);
            if (dt.Rows.Count == 0) {
                TransactionalConnectionWrapper connection = null;
                try
                {
                    connection = getConnection();
                    saveUnit(unit, connection);
                    unit.Id = getLastId(connection);
                    connection.commit();
                }
                finally
                {
                    closeConnection(connection);
                }
                return unit;
            } else {
                return toUploadUnit(dt.Rows[0]);
            }
        }

        private int getLastId(TransactionalConnectionWrapper connection)
        {
            SQLiteCommand command = new SQLiteCommand(connection.Connection);
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT last_insert_rowid()";
            logger.Debug("Executing: <" + command.CommandText + ">");
            return int.Parse(command.ExecuteScalar().ToString());
        }

        private void saveUnit(UploadUnit unit, TransactionalConnectionWrapper connection)
        {
            String query = "INSERT INTO UPLOAD_ITEM (FILENAME, FILEPATH, SIZE, MOD_TIMESTAMP, UPLOADED_TIMESTAMP, CHECKSUM)";
            query += " VALUES (@FILENAME, @FILEPATH, @SIZE, @MOD_TIMESTAMP, @UPLOADED_TIMESTAMP, @CHECKSUM)";
            SQLiteCommand command = new SQLiteCommand(connection.Connection);
            command.CommandType = CommandType.Text;
            command.CommandText = query;
            command.Parameters.Add(new SQLiteParameter("@FILENAME", unit.FileName));
            command.Parameters.Add(new SQLiteParameter("@FILEPATH", unit.FullName));
            command.Parameters.Add(new SQLiteParameter("@SIZE", unit.Size.ToString()));
            command.Parameters.Add(new SQLiteParameter("@MOD_TIMESTAMP", unit.Timestamp.ToString()));
            command.Parameters.Add(new SQLiteParameter("@UPLOADED_TIMESTAMP", (-1).ToString()));
            command.Parameters.Add(new SQLiteParameter("@CHECKSUM", unit.Checksum));
            logger.Debug("Executing: <" + command.CommandText + ">");
            command.ExecuteNonQuery();
        }

        private UploadUnit toUploadUnit(DataRow dr)
        {
            UploadUnit uploadUnit = new UploadUnit(dr["FILENAME"].ToString(),
                                  dr["FILEPATH"].ToString(),
                                  long.Parse(dr["SIZE"].ToString()),
                                  long.Parse(dr["MOD_TIMESTAMP"].ToString()));
            uploadUnit.Checksum = dr["CHECKSUM"].ToString();
            uploadUnit.InSync = dr["UPLOADED_TIMESTAMP"] == null;
            uploadUnit.Id = int.Parse(dr["ID"].ToString());
            return uploadUnit;
        }



        private DataTable getDataForUnit(UploadUnit unit) {
            DataTable dt = new DataTable();
            SQLiteConnection connection = null;
            SQLiteDataReader reader = null;
            try
            {
                connection = getNonTransactionalConnection();
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT * FROM UPLOAD_ITEM WHERE FILEPATH = @filePath";
                command.Parameters.Add(new SQLiteParameter("@filePath", unit.FullName));
                logger.Debug("Executing: <" + command.CommandText + ">");
                reader = command.ExecuteReader();
                dt.Load(reader);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                closeConnection(connection);
            }
            return dt;
        }

        private DataTable getDataById(int id)
        {
            DataTable dt = new DataTable();
            SQLiteConnection connection = null;
            SQLiteDataReader reader = null;
            try
            {
                connection = getNonTransactionalConnection();
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT * FROM UPLOAD_ITEM WHERE ID = @id";
                logger.Debug("Executing: <" + command.CommandText + ">");
                command.Parameters.Add(new SQLiteParameter("@filePath", id));
                reader = command.ExecuteReader();
                dt.Load(reader);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                closeConnection(connection);
            }
            return dt;
        }

    }
}
