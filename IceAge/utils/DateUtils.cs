using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceAge.utils
{
    class DateUtils
    {
        public static readonly DateTime EPOCH_DATE = new DateTime(1970, 1, 1, 0, 0, 0);

        public static long toUnixTimestamp(DateTime timestamp)
        {
            return (long)((timestamp - EPOCH_DATE).TotalSeconds);
        }
    }
}
