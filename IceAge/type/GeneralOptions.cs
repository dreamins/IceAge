using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.IO;
using log4net;


using IceAge.exception;

namespace IceAge.type
{
    // various options needed for app to run
    // not a simple type though
    [DataContract]
    public class GeneralOptions : JSONConfig<GeneralOptions>
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(GeneralOptions).FullName);
        private const string CONFIG_FILE = "config/config.json";

        // location of SQLLite db file
        [DataMember]
        private string sqlLitePath;

        // backup SQLLite db file to S3 (and load it if it doesn't exist or is of a newer version)
        // as it is needed to know whether uploaded file is newer than the one on disk
        // is very recommended. After all S3 has a free tier!
        [DataMember]
        private bool backupToS3;

        // Check if all monitored directories are in sync or no
        [DataMember]
        private bool syncOnStart;

        // use relaxed resync - compare file size, name and timestamp
        // true by default, as it is fast
        [DataMember]
        private bool relaxedResyncOnStart;

        // use hardcore resync with md5 hash sums and timestamps and names
        // in addition to relaxed resync will compare hashes 
        [DataMember]
        private bool fullResyncOnStart;

        // Maximum number of simultaneous uploads
        [DataMember]
        private int maxUploads;

        // Multipart upload enabled
        [DataMember]
        private bool multipartEnabled;

        // Multipart upload threshold in bytes
        [DataMember]
        private uint multipartThresholdBytes;

        public string SQLLitePath
        {
            get
            {
                return sqlLitePath;
            }

            set
            {
                sqlLitePath = value;
            }
        }

        internal void write()
        {
            logger.Info("Writing general options from " + CONFIG_FILE);
            base.write(CONFIG_FILE);
        }

        internal static GeneralOptions load()
        {
            logger.Info("Loading general options from " + CONFIG_FILE);
            return JSONConfig<GeneralOptions>.load(CONFIG_FILE);
        }

        internal GeneralOptions Clone()
        {
            MemoryStream stream = new MemoryStream();
            base.write(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return JSONConfig<GeneralOptions>.load(stream);
        }

    }
}
