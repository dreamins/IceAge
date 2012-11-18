using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading;
using Amazon.Glacier;
using Amazon.Glacier.Transfer;
using Amazon.Runtime;

using IceAge.type;

using log4net;

namespace IceAge
{
    // Uploads stuff to Glacier, only "sync" for now
    // encapsulates what kind of upload it is, it might be async too in future
    // but not now
    class UploadWorker
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(UploadWorker).FullName);
        public UploadUnit Unit { get; private set; }
        private long uploadStart;
        // need to "join" in future but good for now
        public bool Done { get; private set; }


        public delegate void UploadDoneHandler(object sender, UploadDoneEventArgs e);
        public event UploadDoneHandler UploadDone;
        public class UploadDoneEventArgs : EventArgs {
            public UploadUnit Unit {get; private set;}
            public UploadDoneEventArgs(UploadUnit unit)
            {
                this.Unit = unit;
            }
        }

        public UploadWorker(UploadUnit unit)
        {
            this.Unit = unit;
        }

        public void doIt() {
            //spawining zillion of threads.. bad bad bad, fix it !
            Thread thread = new Thread(new ThreadStart(uploadIt));
            thread.Start();
        }

        private void uploadIt()
        {
            logger.Info("Working on " + Unit.FullName);

            String checksum = null;
            // was uploaded before
            if (Unit.InSync || Unit.UploadTimestamp != -1)
            {
                logger.Info("Upload timestamp is valid checking modification time of file " + Unit.FullName);
                if (Unit.FileTimestamp == Unit.UploadedModTimestamp)
                {
                    logger.Info("File not modified");
                    Unit.InSync = true;
                    Unit.Progress = 100;
                    Unit.Speed = "lightspeed";
                    Done = true; UploadDone(this, new UploadDoneEventArgs(Unit));
                    return;
                }

                logger.Info("Calculating checksum of " + Unit.FullName);
                checksum = Unit.calculateChecksum();
                logger.Info("Checksum is [" + checksum + "]" + Unit.FullName);
                // do not upload id it's checksum matches
                if (Unit.Checksum != null && Unit.Checksum.Equals(checksum))
                {
                    logger.Info("Unit checksum match, not uploading" + Unit.FullName);
                    Unit.InSync = true;
                    Unit.Progress = 100;
                    Unit.Speed = "lightspeed";
                    Done = true; UploadDone(this, new UploadDoneEventArgs(Unit));
                    return;
                }
            }

            if (checksum == null)
            {
                checksum = Unit.calculateChecksum();
            }


            try
            {
                // TODO: accessor!!!!
                ArchiveTransferManager manager = new ArchiveTransferManager(
                    Options.Instance.AWSOptions.AWSCredentials,
                    Options.Instance.AWSOptions.AWSRegionEndpoint);
                Unit.Checksum = checksum;
                UploadOptions options = new UploadOptions();
                options.StreamTransferProgress = progress;
                uploadStart = utils.DateUtils.toUnixTimestamp(DateTime.UtcNow);
                manager.Upload(Options.Instance.AWSOptions.GlacierVault, Unit.FileName, Unit.FullName, options);
                logger.Info("Unit uploaded [" + Unit.FullName + "]");
                Unit.InSync = true;
                Unit.Checksum = checksum;
                Unit.UploadedModTimestamp = Unit.FileTimestamp;
                Unit.UploadTimestamp = utils.DateUtils.toUnixTimestamp(DateTime.UtcNow);
                Done = true; UploadDone(this, new UploadDoneEventArgs(Unit));
                return;
            }
            catch (Exception e)
            {
                logger.Error("Upload failed for item " + Unit.FullName, e);
            }

            Unit.InSync = false;
            Done = true; UploadDone(this, new UploadDoneEventArgs(Unit));
        }

        private void progress(object sender, StreamTransferProgressArgs args)
        {
            if (Unit.Progress != args.PercentDone)
            {
                Unit.Progress = args.PercentDone;
                long elapsed = utils.DateUtils.toUnixTimestamp(DateTime.UtcNow) - uploadStart;
                logger.Debug("Callback - transferred " + args.TransferredBytes + " percentage " + args.PercentDone + " time passed :" + elapsed);
                if (elapsed > 0)
                {
                    int KBps = (int)(args.TransferredBytes / elapsed) / 1000;
                    Unit.Speed = KBps + " KB/sec";
                }
            }
        }
    }
}
