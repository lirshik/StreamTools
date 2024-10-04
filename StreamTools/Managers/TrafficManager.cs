using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamTools.Other
{
    public class TrafficManager
    {
        public static void setTraffic(string path, bool value) {
            string exe_name = path.Split('\\')[path.Split('\\').Length - 1];

            string rule_in = $"{exe_name} In";
            string rule_out = $"{exe_name} Out";

            ruleRemove(rule_in);
            ruleRemove(rule_out);

            if (value) {
                ruleBlockAdd(rule_in, path, "in");
                ruleBlockAdd(rule_out, path, "out");
            }
        }

        private static void ruleBlockAdd(string ruleName, string path, string type) {
            if(!ruleExists(ruleName))
                executeCommand($"netsh advfirewall firewall add rule name=\"{ruleName}\" dir={type} action=allow program=\"{path}\" enable=yes");
        }

        private static void ruleRemove(string ruleName) {
            if (ruleExists(ruleName))
                executeCommand($"netsh advfirewall firewall delete rule name=\"{ruleName}\"");
        }

        private static bool ruleExists(string ruleName) {
            string command = $"netsh advfirewall firewall show rule name=\"{ruleName}\"";
            ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe", "/c " + command)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(processStartInfo))
            {
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                return process.ExitCode == 0;
            }
        }

        private static void executeCommand(string command)
        {
            try {
                ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
                processInfo.RedirectStandardOutput = true;
                processInfo.UseShellExecute = false;
                processInfo.CreateNoWindow = true;

                Process.Start(processInfo);
            } catch { }
        }
    }
}
