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

        private Options options;
        public List<UploadUnit> Uploads { get; private set; }

        public Controller(Options options){
            this.options = options;
            // TODO: load from db if need sync
            this.Uploads = new List<UploadUnit>();
        }

        internal void addPath(string path)
        {
            Uploads.AddRange(UploadUnitListFactory.createUploadUnitsFromPath(path));
        }
    }
}
