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
        private static DateTime EPOCH = new DateTime(1970, 1, 1, 0, 0, 0);
        public static UploadUnit createUploadUnit(string fileName)
        {
            FileInfo info = new FileInfo(fileName);
            return new UploadUnit(info.Name, info.FullName, info.Length, toUnixTimestamp(info.LastWriteTimeUtc));
        }

        private static long toUnixTimestamp(DateTime timestamp)
        {
            return (long)((timestamp - EPOCH).TotalSeconds);
        }
    }
}
