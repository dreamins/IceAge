﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceAge.accessor
{
    abstract class Accessor<T>
    {
        public abstract T get();
    }
}
