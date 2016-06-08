using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BWUtilization
{
    class FileLocations
    {
        private static string dataFileLocaton = Directory.GetCurrentDirectory() + "\\InputFiles";
        public static string DataFileLocation {get { return dataFileLocaton; }}
        private static List<string> inputFiles = Directory.GetFiles(dataFileLocaton).ToList();
        public static List<string> InputDataFiles {get { return inputFiles; }}
        private static string outputFile = Directory.GetCurrentDirectory();

        public static string OutputFileName
        {
            get
            {
                int index= 0;
                while (File.Exists(outputFile + index +"_Output.xlsx"))
                {
                    index++;
                }

                return  index + "_Output.xlsx";
            }
        }
    }
}
