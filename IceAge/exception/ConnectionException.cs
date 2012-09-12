using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceAge.exception
{
    public class ConnectionException : IceAgeException
    {
        public ConnectionException(string message) : base(message) { }
    }
}
