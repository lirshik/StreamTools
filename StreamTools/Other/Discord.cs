using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace StreamTools
{
    internal class Discord
    {
        public static void sendInTry(string message)
        {
            try { send(message); } catch { }
        }

        public static void send(string message)
        {
            string webhook = ""; // webhook url

            string title = $"[ StreamTools (build: {DataStorage.Version}) -> Client: {Environment.UserName} ({Environment.MachineName}) ]";

            NameValueCollection discordValues = new NameValueCollection();
            discordValues.Add("username", title);
            discordValues.Add("content", $"Recived message from client ({Environment.UserName}):\n" + message);
            new WebClient().UploadValues(webhook, discordValues);
        }
    }
}
