using StreamTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

public class DataStorage
{
    public static string serverVerison = "";
    public static string Version = "03.10.2024 16:40";
    public static dynamic geoData;

    public static bool enableLogger = false;
    public static bool anwserToPacket = true;

    public static bool workOnlyWithRU = true;

    public static bool streaming = false;

    public static string postUrl = "http://"; // github.com/lirshik/StreamToolsWebServer

    public static string ip = "";

    public static Thread apiThread;
    public static Thread streamThread;
    public static Thread afkThread;

    public static bool isAFK = false;

    public static int inputAudioId = 0;
    public static int audioId = 0;
    public static int videoId = 0;

    public static int audioVolume = 30;
    public static int biterateVideo = 5280;
    public static int fpsVideo = 27;
    public static int biterateAudio = 128;

    public static string stream_url = "";
    public static string stream_url_unready;
    public static string user_token;

    public static Mutex mutex;

    public static ClientAPI client;
    public static ServerAPI server;

    public static TelegramService telegramService;

    public static AFKService afkService;

    public static ConfigManager cfgManager;

    public static CameraService cameraService;
    public static ShareService shareService;
}