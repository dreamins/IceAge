using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IceAge.type;

using log4net;

namespace IceAge.factory
{
    class UploadUnitListFactory
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(UploadUnitFactory).FullName);

        public static ICollection<UploadUnit> createUploadUnitsFromPath(string filePath)
        {
            logger.Info("Adding new upload item " + filePath);
            FileAttributes attributes = File.GetAttributes(filePath);
            List<UploadUnit> ret = new List<UploadUnit>();

            if (!attributes.HasFlag(FileAttributes.Directory)) {
                logger.Debug("Plain file");
                ret.Add(UploadUnitFactory.createUploadUnit(filePath));
                return ret;
            }
            
            // add subdir walk
            return createUploadUnitsFromDirectory(new DirectoryInfo(filePath));
        }

        private static ICollection<UploadUnit> createUploadUnitsFromDirectory(System.IO.DirectoryInfo path)
        {
            logger.Info("Recursing into " + path.FullName);
            List<UploadUnit> ret = new List<UploadUnit>();
            FileInfo[] files = null;
            DirectoryInfo[] subDirs = null;

            try
            {
                files = path.GetFiles("*.*");
            }
            catch (UnauthorizedAccessException e)
            {
                logger.Error("Cannot read files from subdir", e);
                return ret;
            }
            
            foreach(FileInfo fileInfo in files) 
            {
                ret.Add(UploadUnitFactory.createUploadUnit(fileInfo.FullName));
            }

            subDirs = path.GetDirectories();

            foreach (System.IO.DirectoryInfo dirInfo in subDirs)
            {
                ICollection<UploadUnit> subList = createUploadUnitsFromDirectory(dirInfo);
                ret.AddRange(subList);
            }

            return ret;
        }
    }
}
