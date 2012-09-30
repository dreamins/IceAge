﻿using System;
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
        private GeneralOptions generalOptions;
        private AWSOptions awsOptions;
        private static Object lockObject = new Object();

        private Options(GeneralOptions generalOpts, AWSOptions awsOpts)
        {
            this.generalOptions = generalOpts;
            this.awsOptions = awsOpts;
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
            }
        }

        // Factory method to read from config
        public static Options loadFromConfig(bool withDefaults = false)
        {
            lock (lockObject)
            {
                logger.Info("Loading configuration");
                GeneralOptions generalOpts = null;
                AWSOptions awsOpts = null;
                try
                {
                    generalOpts = GeneralOptions.load();
                }
                catch (FileNotFoundException ex)
                {
                    if (withDefaults)
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
                    }
                    else
                    {
                        logger.Info("No defaults. Rethrowing.");
                        throw ex;
                    }
                }

                try {
                    awsOpts = AWSOptions.load();
                } catch (FileNotFoundException ex) {
                    if (withDefaults)
                    {
                        logger.Info("Defaulting AWS options to hardcoded values");
                        awsOpts = new AWSOptions();
                        awsOpts.AWSAccessKey = String.Empty;
                        awsOpts.AWSSecretKey = String.Empty;
                        awsOpts.GlacierVault = String.Empty;
                        awsOpts.S3Bucket = String.Empty;
                    }
                    else
                    {
                        logger.Info("No defaults. Rethrowing.");
                        throw ex;
                    }
                }

                return new Options(generalOpts, awsOpts);
            }
        }

        public Options Clone()
        {
            lock (lockObject)
            {
                GeneralOptions generalOpts = generalOptions.Clone();
                AWSOptions awsOpts = awsOptions.Clone();
                return new Options(generalOptions, awsOptions);
            }
        }

        public static bool isReady()
        {
            try
            {
                // kinda hacky
                lock (lockObject)
                {
                    GeneralOptions generalOpts = GeneralOptions.load();
                    AWSOptions awsOpts = AWSOptions.load();
                }
            }
            catch (FileNotFoundException)
            {
                logger.Warn("File not found not ready");
                return false;
            }

            return true;
        }
    }
}
