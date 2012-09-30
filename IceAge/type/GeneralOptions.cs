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
        public string SQLLitePath{get;set;}

        // backup SQLLite db file to S3 (and load it if it doesn't exist or is of a newer version)
        // as it is needed to know whether uploaded file is newer than the one on disk
        // is very recommended. After all S3 has a free tier!
        [DataMember]
        public bool BackupToS3 { get; set; }

        // Check if all monitored directories are in sync or no
        [DataMember]
        public bool SyncOnStart { get; set; }

        // use relaxed resync - compare file size, name and timestamp
        // true by default, as it is fast
        [DataMember]
        public bool RelaxedResyncOnStart { get; set; }

        // use hardcore resync with md5 hash sums and timestamps and names
        // in addition to relaxed resync will compare hashes 
        [DataMember]
        public bool FullResyncOnStart { get; set; }

        // Maximum number of simultaneous uploads
        [DataMember]
        public uint MaxUploads { get; set; }

        // Multipart upload enabled
        [DataMember]
        public bool MultipartEnabled { get; set; }

        // Multipart upload threshold in bytes
        [DataMember]
        public uint MultipartThresholdBytes { get; set; }

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
