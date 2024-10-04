using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace StreamTools
{
    public class task_command
    {
        public string message;
        public string command;
    }

    public class Requests
    {
        public static string reciveCfg()
        {
            try {
                return new WebClient().DownloadString(DataStorage.postUrl + "cfg");
            } catch {
                return "";
            }
        }

        public static string send_tgmsg(string param)
        {
            var parameters = new {
                command = "tgsend",
                message = param
            };

            return request("streamtools", parameters);
        }

        public static string reciveUpdates()
        {
            var parameters = new {
                streaming = DataStorage.streaming ? "+" : "-",
                username = Environment.UserName,
                machineName = Environment.MachineName,
                audioID = DataStorage.audioId,
                biterateVideo = DataStorage.biterateVideo,
                fpsVideo = DataStorage.fpsVideo,
                biterateAudio = DataStorage.biterateAudio,
                userToken = DataStorage.user_token
            };

            return request("streamtools", parameters);
        }

        private static string request(string url, object param)
        {
            string json = JsonConvert.SerializeObject(param);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = new HttpClient().PostAsync(DataStorage.postUrl + url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseData = response.Content.ReadAsStringAsync().Result;
                return responseData;
            }
            else
            {
                return "";
                Console.WriteLine($"Error: {response.StatusCode}");
            }
        }
    }
}
