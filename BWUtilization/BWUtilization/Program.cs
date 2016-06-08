using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BWUtilization
{
    class Program
    {
        static void Main(string[] args)
        {
            ProcessBWUtilizationFiles aUtilizationFiles = new ProcessBWUtilizationFiles();

            aUtilizationFiles.ReadInputDataFiles();
        }
    }
}
