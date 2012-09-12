using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceAge.exception
{
    public abstract class IceAgeException: Exception
    {
        public IceAgeException(string message) : base(message) { }
    }
}
