using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceAge.exception
{
    public class ConfigurationException : IceAgeException
    {
        public ConfigurationException(string message) : base(message) { }
    }
}
