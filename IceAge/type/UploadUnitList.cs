using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace IceAge.type
{
    class UploadUnitList: ObservableCollection<UploadUnit>
    {
        public void addAll(ICollection<UploadUnit> uploadUnits)
        {
            foreach (UploadUnit unit in uploadUnits)
            {
                Add(unit);
            }
        }
    }
}
