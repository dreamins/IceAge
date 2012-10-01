using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using IceAge.type;

namespace IceAge.factory
{
    class UploadUnitFactory
    {
        public static UploadUnit createUploadUnit(string fileName)
        {
            FileInfo info = new FileInfo(fileName);
            return new UploadUnit(info.Name, info.FullName, info.Length);
        }
    }
}
