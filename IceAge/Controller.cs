using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IceAge.type;
using IceAge.factory;
using log4net;
using IceAge.dao;

namespace IceAge
{
    class Controller
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Controller).FullName);
        private DAOController daoController;

        public UploadUnitList Uploads { get; private set; }
        public Estimations Estimations { get; private set; }

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
        #endregion

        public Controller(){
            // TODO: load from db if need sync
            this.Uploads = new UploadUnitList();
            Options.Instance.OptionsChanged += this.reloadOptionsStuff;
            reloadOptionsStuff(Options.Instance, null);
        }

        internal void addPath(string path)
        {
            ICollection<UploadUnit> units = UploadUnitListFactory.createUploadUnitsFromPath(path);
            ICollection<UploadUnit> unitsToUpload = new List<UploadUnit>();
            foreach (UploadUnit unit in units)
            {
                // is it in db?
                UploadUnit newUnit = daoController.UploadUnitDAO.getOrSaveUploadUnit(unit);
                unitsToUpload.Add(newUnit);
            }
            // later if user clicks upload and forget, will just upload (but still save in sqllite), no matter if it is in sync or no
            // if user clicks sync we will pay attention to checksums and unit sync status
            Uploads.addAll(unitsToUpload);
            
            reestimate();
        }

        internal void clearPaths()
        {
            Uploads.Clear();
            reestimate();
        }

        private void reestimate()
        {
            Estimations estimates = new Estimations();
            estimates.estimate(Uploads);
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
    }
}

