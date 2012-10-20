using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using log4net;

namespace IceAge.dao
{
    class DAOController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(DAOController).FullName);

        public UploadUnitDAO UploadUnitDAO { get; private set; }

        public DAOController(String filename) {
            if(!File.Exists(filename)) {
                boostrap(filename);
            }

            UploadUnitDAO = new UploadUnitDAO(Path.GetFullPath(filename));
        }

        public void boostrap(String filename) {
            
            logger.Info("Boostrapping DB");
            String boostrapQuery = UploadUnitDAO.getCreateStatement();
            logger.Info("Executing <" + boostrapQuery + ">");
            String directoryName = Path.GetDirectoryName(filename);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            GenericSQLiteDAO dao = new GenericSQLiteDAO(Path.GetFullPath(filename));
            dao.executeNonQuery(boostrapQuery);
        }
    }
}
