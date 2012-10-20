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
    // A fascade for options
    public class Options
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Options).FullName);
        private static Object lockObject = new Object();
        private static volatile Options instance = null;

        private GeneralOptions generalOptions;
        private AWSOptions awsOptions;
        // whether this is a result of hardcoded default filed stuff
        private bool defaulted;

        public delegate void OptionsChangedHandler(object sender, OptionsChangedEventArgs e);
        public event OptionsChangedHandler OptionsChanged;
        public class OptionsChangedEventArgs : EventArgs
        {
        }

        public GeneralOptions GeneralOptions
        {
            get
            {
                return generalOptions;
            }
        }

        public AWSOptions AWSOptions
        {
            get
            {
                return awsOptions;
            }
        }

        public bool Defaulted
        {
            get
            {
                return defaulted;
            }
        }

        public static Options Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = loadFromConfig();
                        }
                    }
                }

                return instance;
            }
        }

        private Options(GeneralOptions generalOpts, AWSOptions awsOpts, bool defaulted)
        {
            this.generalOptions = generalOpts;
            this.awsOptions = awsOpts;
            this.defaulted = defaulted;
        }


        // Serializes the object into json configuration
        public void writeToConfig()
        {
            lock (lockObject)
            {
                logger.Info("Writing configuration");
                // yeah we might end up with partially written files
                // but shall we care?
                generalOptions.write();
                awsOptions.write();
                defaulted = false;
                OptionsChanged(this, new OptionsChangedEventArgs());
            }
        }

        public Options Clone()
        {
            lock (lockObject)
            {
                GeneralOptions generalOpts = generalOptions.Clone();
                AWSOptions awsOpts = awsOptions.Clone();
                return new Options(generalOptions, awsOptions, defaulted);
            }
        }

        // Factory method to read from config
        private static Options loadFromConfig()
        {
            bool defaulted = false;
            lock (lockObject)
            {
                logger.Info("Loading configuration");
                GeneralOptions generalOpts = null;
                AWSOptions awsOpts = null;
                try
                {
                    generalOpts = GeneralOptions.load();
                }
                catch (FileNotFoundException )
                {
                    logger.Info("Defaulting general options to hardcoded values");
                    generalOpts = new GeneralOptions();
                    generalOpts.SQLLitePath = "data/data.db";
                    generalOpts.BackupToS3 = false;
                    generalOpts.FullResyncOnStart = false;
                    generalOpts.MaxUploads = 10;
                    generalOpts.MultipartEnabled = false;
                    generalOpts.MultipartThresholdBytes = 10000000;
                    generalOpts.RelaxedResyncOnStart = true;
                    generalOpts.SyncOnStart = false;
                    defaulted = true;
                }

                try
                {
                    awsOpts = AWSOptions.load();
                }
                catch (FileNotFoundException )
                {
                    logger.Info("Defaulting AWS options to hardcoded values");
                    awsOpts = new AWSOptions();
                    awsOpts.AWSAccessKey = String.Empty;
                    awsOpts.AWSSecretKey = String.Empty;
                    awsOpts.GlacierVault = String.Empty;
                    awsOpts.S3Bucket = String.Empty;
                    defaulted = true; 
                }

                return new Options(generalOpts, awsOpts, defaulted);
            }
        }
    }
}
