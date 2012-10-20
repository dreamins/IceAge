using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceAge.exception
{
    class BugInCodeException : IceAgeException
    {
        public BugInCodeException(string message) : base("BugInCodeException: Impossible happened : " + message) { }
    }
}
