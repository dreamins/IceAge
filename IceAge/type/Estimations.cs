using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceAge.type
{
    class Estimations
    {
        public double MonthlyCost  {get; private set;}
        public double RequestsCost {get; private set;}
        private static readonly double STORAGE_GB_DOLLAR_MONTH = 0.01;
        private static readonly double COST_PER_REQUEST = 0.01 / 1000;
        private static readonly int AMAZON_GIGABYTE = 1024 * 1024 * 1024; // really?

        public void estimate(ICollection<UploadUnit> uploads) {
            long uploadSize = 0;
            foreach (UploadUnit unit in uploads)
            {
                uploadSize += unit.Size;
            }

            MonthlyCost = ((double)uploadSize / AMAZON_GIGABYTE) * STORAGE_GB_DOLLAR_MONTH;
            RequestsCost = uploads.Count * COST_PER_REQUEST;
        }
    }
}
