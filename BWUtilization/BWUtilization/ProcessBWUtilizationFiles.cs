using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ManiacProject.Libs;

namespace BWUtilization
{
    internal class ProcessBWUtilizationFiles
    {
        private List<string> inputData = new List<string>();
        private Dictionary<string,List<Dictionary<string,string>>> trunkWiseData = new Dictionary<string, List<Dictionary<string, string>>>(); 
        private List<Dictionary<string,string>> outputReportData = new List<Dictionary<string, string>>(); 
        public void ReadInputDataFiles()
        {

            ReadInputFiles();
            List<Dictionary<string, string>> sortedData = SortInputDataIntoDictionary();
            SetTrunkWiseData(sortedData);
            GenerateOutputReportData();
            IOFileOperation.CreateExelFile(outputReportData, Directory.GetCurrentDirectory(), "Report");


        }

        private void GenerateOutputReportData()
        {
            foreach (KeyValuePair<string, List<Dictionary<string, string>>> keyValuePair in trunkWiseData)
            {
                string maxInbound = GetMaxInboundValue(keyValuePair.Value);
                string maxOutbound = GetMaxOutboundValue(keyValuePair.Value);
                Dictionary<string,string> aDictionary = new Dictionary<string, string>();
                aDictionary.Add("Node",keyValuePair.Key);
                aDictionary.Add("Max of inbounds Mbits/s", maxInbound);
                aDictionary.Add("Max of outbounds Mbits/s", maxOutbound);
                outputReportData.Add(aDictionary);
             }
        }

        private string GetMaxOutboundValue(List<Dictionary<string, string>> aTrunkData)
        {
            double maxOnbound = 0;
            foreach (Dictionary<string, string> dictionary in aTrunkData)
            {
                if (Convert.ToDouble(dictionary["Outbound Rate(bit/s)"]) > maxOnbound)
                {
                    maxOnbound = Convert.ToDouble(dictionary["Outbound Rate(bit/s)"]);
                }
            }

            return maxOnbound.ToString();
        }

        private string GetMaxInboundValue(List<Dictionary<string, string>> aTrunkData)
        {
            double maxInbound = 0;
            foreach (Dictionary<string, string> dictionary in aTrunkData)
            {
                if (Convert.ToDouble(dictionary["Inbound Rate(bit/s)"]) > maxInbound)
                {
                    maxInbound = Convert.ToDouble(dictionary["Inbound Rate(bit/s)"]);
                }
            }

            return maxInbound.ToString();
        }

        private void SetTrunkWiseData(List<Dictionary<string, string>> sortedData)
        {
            foreach (Dictionary<string, string> dictionary in sortedData)
            {
                dictionary.Add("Original Inbound", dictionary["Inbound Rate(bit/s)"]);
                dictionary.Add("Original Outbound", dictionary["Outbound Rate(bit/s)"]);


                if (dictionary["Inbound Rate(bit/s)"].Contains("G"))
                {
                    dictionary["Inbound Rate(bit/s)"] = (Convert.ToDouble(dictionary["Inbound Rate(bit/s)"].Replace("G", "")) * 1000).ToString();
                }
                else if (dictionary["Inbound Rate(bit/s)"].Contains("M"))
                {
                    dictionary["Inbound Rate(bit/s)"] = Convert.ToDouble(dictionary["Inbound Rate(bit/s)"].Replace("M", "")).ToString();
                }
                else if (dictionary["Inbound Rate(bit/s)"].Contains("K"))
                {
                    dictionary["Inbound Rate(bit/s)"] = (Convert.ToDouble(dictionary["Inbound Rate(bit/s)"].Replace("K", "")) / 1000).ToString();
                }
                else if(dictionary["Inbound Rate(bit/s)"].Contains("NA"))
                {
                    dictionary["Inbound Rate(bit/s)"] = 0.ToString();
                }




                if (dictionary["Outbound Rate(bit/s)"].Contains("G"))
                {
                    dictionary["Outbound Rate(bit/s)"] = (Convert.ToDouble(dictionary["Outbound Rate(bit/s)"].Replace("G", "")) * 1000).ToString();
                }
                else if (dictionary["Outbound Rate(bit/s)"].Contains("M"))
                {
                    dictionary["Outbound Rate(bit/s)"] = Convert.ToDouble(dictionary["Outbound Rate(bit/s)"].Replace("M", "")).ToString();
                }
                else if (dictionary["Outbound Rate(bit/s)"].Contains("K"))
                {
                    dictionary["Outbound Rate(bit/s)"] =
                        (Convert.ToDouble(dictionary["Outbound Rate(bit/s)"].Replace("K", ""))/1000).ToString();
                }
                else if (dictionary["Outbound Rate(bit/s)"].Contains("NA"))
                {
                    dictionary["Outbound Rate(bit/s)"] = 0.ToString();
                }





                if (trunkWiseData.ContainsKey(dictionary["Resource Name"]))
                {
                    trunkWiseData[dictionary["Resource Name"]].Add(dictionary);
                }
                else
                {
                    
                    List<Dictionary<string,string>> aList = new List<Dictionary<string, string>>();
                    aList.Add(dictionary);
                    trunkWiseData.Add(dictionary["Resource Name"],aList);
                }
            }
        }

        private List<Dictionary<string, string>> SortInputDataIntoDictionary()
        {
            List<Dictionary<string, string>> sortedData = new List<Dictionary<string, string>>();
            List<string> columns = new List<string>();
            foreach (string stringData in inputData)
            {
                if (!IsColumnHeader(stringData, columns, out columns))
                {

                    Dictionary<string, string> aDictionary = new Dictionary<string, string>();
                    string[] splittedData = stringData.Split(',');
                    int index = 0;

                    foreach (string column in columns)
                    {
                        aDictionary.Add(column.Trim().Replace('\"', ' ').Trim(), splittedData[index]);
                        index++;
                    }
                    sortedData.Add(aDictionary);
                }
            }

            return sortedData;
        }

        private void ReadInputFiles()
        {
            foreach (string inputDataFile in FileLocations.InputDataFiles)
            {
                using (StreamReader sr = new StreamReader(inputDataFile))
                {
                    string temp = string.Empty;
                    while ((temp = sr.ReadLine()) != null)
                    {
                        if (temp.Contains(','))
                        {
                            inputData.Add(temp);
                        }
                    }
                }
            }
        }

        private bool IsColumnHeader(string aLineData, List<string> oldColumns, out List<string> columns)
        {
            columns = new List<string>();

            if (aLineData.ToLower().Contains("resource name"))
            {
                columns = aLineData.Split(',').ToList();
                return true;
            }

            columns = oldColumns;
            return false;
        }

    }
}
