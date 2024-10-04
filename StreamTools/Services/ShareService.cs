using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace StreamTools
{
    public class ShareService
    {
        public static Process ffmpegProcess = null;

        public ShareService() { }

        public Process getProcess() { return ffmpegProcess; }

        string ffmpegArgs;

        public void Initialize()
        {
            string inputAudio = DataStorage.inputAudioId == -1 ? "" : $"-f dshow -i audio=\"{DataStorage.cameraService.ReciveHeadphones()}\" ";
            string text = $"{DateTime.Now.Hour} {DateTime.Now.Minute} {DateTime.Now.Second}";
            ffmpegArgs = $"-f dshow -i video=\"{DataStorage.cameraService.ReciveCamera()}\" -f dshow -i audio=\"{DataStorage.cameraService.ReciveMicrophone()}\" " +
                         $"-c:v libx264 -b:v {DataStorage.biterateVideo}k -preset veryfast -r {DataStorage.fpsVideo} " +
                         $"-af \"volume={DataStorage.audioVolume}dB\" " +
                         $"{inputAudio}" +
                         $"-c:a aac -b:a {DataStorage.biterateAudio}k -f flv {DataStorage.stream_url}";
        }

        public void StartStreaming()
        {
            // -af \"volume={DataStorage.audioVolume}dB\"
            //$"-vf \"drawtext=text='{text}':fontcolor=white:fontsize=17:x=10:y=10\" " +
            //$"-analyzeduration 0 -tune zerolatency " +

            var processStartInfo = new ProcessStartInfo {
                FileName = "C:\\Windows\\ffmpeg\\bin\\ffmpeg.exe",
                Arguments = ffmpegArgs,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            ffmpegProcess = new Process { StartInfo = processStartInfo };
            ffmpegProcess.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
            ffmpegProcess.ErrorDataReceived += (sender, e) => Console.WriteLine(e.Data);

            ffmpegProcess.Start();
            ffmpegProcess.BeginOutputReadLine();
            ffmpegProcess.BeginErrorReadLine();

            DataStorage.streaming = true;
            Discord.sendInTry($"Stream started");
        }

        public void StopStreaming()
        {
            try {
                DataStorage.streaming = false;
                ffmpegProcess.Kill();
                ffmpegProcess = null;
                Discord.sendInTry($"Stream ended");
            } catch {
                Discord.sendInTry($"Failed to stop stream");
            }
        }
    }
}
