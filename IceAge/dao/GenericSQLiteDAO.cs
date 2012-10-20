using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data;

using log4net;

namespace IceAge.dao
{
    class GenericSQLiteDAO
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(GenericSQLiteDAO).FullName);
        private String connectionString;

        public GenericSQLiteDAO(String fileName) {
            connectionString = String.Format("Data Source={0}", fileName);
        }

        public void executeNonQuery(String query) {
            TransactionalConnectionWrapper connection = null;
            try
            {
                connection = getConnection();
                SQLiteCommand command = new SQLiteCommand(connection.Connection);
                command.CommandText = query;
                int rowsUpdated = command.ExecuteNonQuery();
                connection.commit();
            }
            finally
            {
                connection.Close();
            }
        }

        protected void closeConnection(TransactionalConnectionWrapper connection)
        {
            if (connection != null)
            {
                connection.Close();
            }
        }

        protected void closeConnection(SQLiteConnection connection)
        {
            if (connection != null)
            {
                connection.Close();
            }
        }

        protected TransactionalConnectionWrapper getConnection() {
            SQLiteConnection connection = new SQLiteConnection(connectionString);

            connection.Open();
            return new TransactionalConnectionWrapper(connection);
        }

        protected SQLiteConnection getNonTransactionalConnection()
        {
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            connection.Open();
            return connection;
        }

        protected class TransactionalConnectionWrapper {
            public SQLiteConnection Connection{get; private set;}
            public SQLiteTransaction Transaction{get; private set;}
            private bool committed = false;

            public TransactionalConnectionWrapper(SQLiteConnection conn)
            {
                this.Connection = conn;
                this.Transaction = conn.BeginTransaction();
            }

            public void commit()
            {
                logger.Debug("Committing transaction");
                this.Transaction.Commit();
                committed = true;
            }

            public void Close()
            {
                if (!committed)
                {
                    logger.Debug("Rolling back transaction");
                    this.Transaction.Rollback();
                }
                this.Connection.Close();
            }
        }
    }
}
