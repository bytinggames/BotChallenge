using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JuliHelper
{
    class G
    {

        public static readonly char I = Path.DirectorySeparatorChar; // slash for paths
        public static readonly string exeDir = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
    }
}
