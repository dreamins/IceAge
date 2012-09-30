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
    public class AWSOptions : JSONConfig<AWSOptions>
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(AWSOptions).FullName);
        private const string CONFIG_FILE = "config/aws.json";

        [DataMember]
        public string AWSAccessKey { get; set; }
        [DataMember]
        public string AWSSecretKey { get; set; }
        [DataMember]
        public string S3Bucket { get; set; }
        [DataMember]
        public string GlacierVault { get; set; }

        internal void write()
        {
            logger.Info("Writing AWS options to " + CONFIG_FILE);
            base.write(CONFIG_FILE);
        }

        internal static AWSOptions load() 
        {
            logger.Info("Loading AWS options from " + CONFIG_FILE);
            return JSONConfig<AWSOptions>.load(CONFIG_FILE);
        }

        internal AWSOptions Clone()
        {
            MemoryStream stream = new MemoryStream();
            base.write(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return JSONConfig<AWSOptions>.load(stream);
        }

    }
}
