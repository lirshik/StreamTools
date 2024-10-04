using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace StreamTools
{
    public class AFKService
    {
        public AFKService() { }

        private static WaveInEvent waveSource;
        private static float[] buffer;
        private static DateTime? silenceStartTime;
        private static DateTime? soundDetectedTime;

        private const int SAMPLE_RATE = 44100;
        private const int BUFFER_SIZE = 2048;

        public float AFK_DB = 0.01f;

        public int SILENCE_DURATION_SECONDS = 180;
        public int SOUND_DETECTED_DURATION_SECONDS = 5;

        public void Start() {
            if(waveSource != null)
                waveSource.StartRecording();
        }

        public void Stop()
        {
            if (waveSource != null)
                waveSource.StopRecording();
        }

        public void ChangeAudioID(int id)
        {
            waveSource.DeviceNumber = id;
        }

        public void Initialize()
        {
            waveSource = new WaveInEvent {
                DeviceNumber = DataStorage.audioId,
                WaveFormat = new WaveFormat(SAMPLE_RATE, 1) // mono
            };
            waveSource.DataAvailable += OnDataAvailable;
        }

        private bool IsSilent(float[] audioBuffer)
        {
            return audioBuffer.Average(Math.Abs) < AFK_DB;
        }

        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            buffer = new float[e.Buffer.Length / 2];
            Buffer.BlockCopy(e.Buffer, 0, buffer, 0, e.Buffer.Length);

            if (IsSilent(buffer)) {
                if (!silenceStartTime.HasValue) {
                    silenceStartTime = DateTime.Now;
                }

                else if ((DateTime.Now - silenceStartTime.Value).TotalSeconds >= SILENCE_DURATION_SECONDS) {
                    Console.WriteLine("[AFKService] AFK started (3min no activity)");
                    DataStorage.isAFK = true;
                }
            } else {
                if (silenceStartTime.HasValue) {
                    silenceStartTime = null;
                    soundDetectedTime = DateTime.Now;
                }

                if (soundDetectedTime.HasValue && (DateTime.Now - soundDetectedTime.Value).TotalSeconds >= SOUND_DETECTED_DURATION_SECONDS) {
                    Console.WriteLine("[AFKService] AFK stopped");
                    DataStorage.isAFK = false;
                    soundDetectedTime = null;

                    Discord.sendInTry("[AFKService] Activity!");
                }
            }
        }
    }
}
