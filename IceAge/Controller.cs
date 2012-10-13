using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IceAge.type;
using IceAge.factory;
using log4net;

namespace IceAge
{
    class Controller
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Controller).FullName);

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
        }

        internal void addPath(string path)
        {
            Uploads.addAll(UploadUnitListFactory.createUploadUnitsFromPath(path));
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
    }
}

