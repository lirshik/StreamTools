using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamTools
{
    public class ConfigManager
    {
        public ConfigManager() { }

        public bool updateConfig()
        {
            try
            {
                string cfg = Requests.reciveCfg();
                if (cfg == "") {
                    Console.WriteLine("Failed to recive config!");
                    Discord.sendInTry("Failed to recive config!");
                    return false;
                } else {
                    try
                    {
                        DataStorage.cfgManager.loadConfig(cfg);
                    } catch {
                        Console.WriteLine("Failed to load config! (json parse error)");
                        Discord.sendInTry("Failed to load config! (json parse error)");
                        return false;
                    }
                    return true;
                }
            } catch {
                Console.WriteLine("Failed to recive config! (server error)");
                Discord.sendInTry("Failed to recive config! (server error)");
                return false;
            }
        }

        public void loadConfig(string cfg)
        {
            JObject jsonObject = JObject.Parse(cfg);

            string inputAudioDevice = jsonObject["inputAudioDevice"].ToString();
            string audioDevice = jsonObject["audioDevice"].ToString();
            string audioVolume = jsonObject["audioVolume"].ToString();
            string bitrateAudio = jsonObject["bitrateAudio"].ToString();
            string bitrateVideo = jsonObject["bitrateVideo"].ToString();
            string fpsVideo = jsonObject["fpsVideo"].ToString();

            DataStorage.inputAudioId = Int32.Parse(inputAudioDevice);
            DataStorage.audioId = Int32.Parse(audioDevice);
            DataStorage.audioVolume = Int32.Parse(audioVolume);
            DataStorage.biterateAudio = Int32.Parse(bitrateAudio);
            DataStorage.biterateVideo = Int32.Parse(bitrateVideo);
            DataStorage.fpsVideo = Int32.Parse(fpsVideo);

            if (DataStorage.afkService != null)
                DataStorage.afkService.ChangeAudioID(DataStorage.audioId);
        }
    }
}
