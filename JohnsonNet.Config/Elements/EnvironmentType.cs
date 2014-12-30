using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace JohnsonNet.Config
{
    public enum EnvironmentType : byte
    {
        Live,
        PreProduction,
        Test,
        Local,
        Unknown
    }
}
