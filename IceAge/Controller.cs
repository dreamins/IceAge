using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IceAge.type;
using log4net;

namespace IceAge
{
    class Controller
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Controller).FullName);

        private Options options;

        public Controller(Options options){
            this.options = options;
        }
    }
}
