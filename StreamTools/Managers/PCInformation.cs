using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace StreamTools
{
    internal class PCInformation
    {
        private static List<string> getHardware(string WIN32_Class, string ClassItemField)
        {
            List<string> result = new List<string>();

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM " + WIN32_Class);

            try
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    result.Add(obj[ClassItemField].ToString().Trim());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }

        public static string cpu = String.Join(", ", getHardware("Win32_Processor", "Name"));
        public static string cpu_builder = String.Join(", ", getHardware("Win32_Processor", "Manufacturer"));
        public static string gpu = String.Join(", ", getHardware("Win32_VideoController", "Name"));
        public static string ram = String.Join(", ", getHardware("Win32_VideoController", "AdapterRAM"));
        public static int ram_usage()
        {
            int ram = 0;

            ManagementObjectSearcher ramMonitor = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize,FreePhysicalMemory FROM Win32_OperatingSystem");

            foreach (ManagementObject objram in ramMonitor.Get())
            {
                ulong totalRam = Convert.ToUInt64(objram["TotalVisibleMemorySize"]);
                ulong busyRam = totalRam - Convert.ToUInt64(objram["FreePhysicalMemory"]);
                ram += Convert.ToInt32(busyRam * 100 / totalRam); ;
            }

            return ram;
        }
        public static string hdd = String.Join(", ", getHardware("Win32_DiskDrive", "Caption"));
    }
}
