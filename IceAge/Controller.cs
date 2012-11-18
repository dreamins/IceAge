using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using IceAge.type;
using IceAge.factory;
using log4net;
using IceAge.dao;

namespace IceAge
{
    /**
     * Controls all stuff execution and wiring between ui, DAO and uploads
     */
    class Controller
    {
        public UploadUnitList UploadUnitList { get; private set; }
        public Estimations Estimations { get; private set; }


        private static readonly ILog logger = LogManager.GetLogger(typeof(Controller).FullName);
        private DAOController daoController;
        private List<UploadWorker> UploadsInProgress = new List<UploadWorker>();

        #region Events
        public delegate void EstimationChangedHandler(object sender, EstimationChangedEventArgs e);
        public event EstimationChangedHandler EstimationChanged;
        public class EstimationChangedEventArgs: EventArgs {
            public Estimations Estimations { get; private set; }
            public EstimationChangedEventArgs(Estimations estimations)
            {
                this.Estimations = estimations;
            }
        }
        
        public delegate void UploadDoneHandler(object sender, UploadDoneEventArgs e);
        public event UploadDoneHandler UploadDone;
        public class UploadDoneEventArgs : EventArgs {}

        #endregion

        public Controller(){
            // TODO: load from db if need sync
            this.UploadUnitList = new UploadUnitList();
            Options.Instance.OptionsChanged += this.reloadOptionsStuff;
            reloadOptionsStuff(Options.Instance, null);
        }

        
        public void startSyncUpload() {
            if (UploadUnitList.Count == 0)
            {
                UploadDone(this, new UploadDoneEventArgs());
            }
            // launch uploaders
            foreach (UploadUnit unit in UploadUnitList)
            {
                // refactor to pass uploading strategy instead, or the underlying obj shall decide
                UploadWorker worker = new UploadWorker(unit);
                worker.UploadDone += new UploadWorker.UploadDoneHandler(worker_UploadDone);
                UploadsInProgress.Add(worker);
            }
            // separate so we have complete list before launching
            foreach (UploadWorker worker in UploadsInProgress)
            {
                worker.doIt();
            }
        }

        internal void addPath(string path)
        {
            ICollection<UploadUnit> units = UploadUnitListFactory.createUploadUnitsFromPath(path);
            ICollection<UploadUnit> unitsToUpload = new List<UploadUnit>();
            logger.Debug("Adding path [" + path + "]");
            foreach (UploadUnit unit in units)
            {
                if (UploadUnitList.Contains(unit))
                {
                    continue;
                }
                // is it in db?
                unitsToUpload.Add(daoController.UploadUnitDAO.getOrSaveUploadUnit(unit));
            }
            logger.Debug("Units added : [" + unitsToUpload.Count + "]");
            // later if user clicks upload and forget, will just upload (but still save in sqllite), no matter if it is in sync or no
            // if user clicks sync we will pay attention to checksums and unit sync status
            UploadUnitList.addAll(unitsToUpload);
            
            reestimate();
        }

        internal void clearPaths()
        {
            logger.Debug("Clear");
            UploadUnitList.Clear();
            reestimate();
        }

        private void reestimate()
        {
            logger.Debug("Estimating");
            Estimations estimates = new Estimations();
            estimates.estimate(UploadUnitList);
            EstimationChanged(this, new EstimationChangedEventArgs(estimates));
        }

        private void reloadOptionsStuff(object sender, Options.OptionsChangedEventArgs args)
        {
            try
            {
                daoController = new DAOController(Options.Instance.GeneralOptions.SQLLitePath);
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
                throw ex;
            }
        }

        private void worker_UploadDone(Object worker, UploadWorker.UploadDoneEventArgs args)
        {
            daoController.UploadUnitDAO.update(args.Unit);
            lock (UploadsInProgress)
            {
                UploadsInProgress.Remove((UploadWorker)worker);
            }
            // check if we are done-done
            // TODO: refactor this into upload manager
            bool done = false;
            lock (UploadsInProgress) {
                if (UploadsInProgress.Count == 0)
                {
                    done = true;
                }
            }

            if (done)
            {
                logger.Info("All uploads finished");
                // note this will be executed in Worker's thread
                UploadDone(this, new UploadDoneEventArgs());
            }
        }
    }
}

