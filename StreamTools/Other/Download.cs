using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace StreamTools
{
    public class Download
    {
        public static void download(string url, string file) {
            if(!File.Exists(file))
                new WebClient().DownloadFile(url, file);
        }
    }
}
