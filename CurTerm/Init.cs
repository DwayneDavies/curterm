using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Text;

namespace CurTerm
{

    static class Init
    {

        public static void checkFiles()
        {
            string[] fileList = { "currencies.json", "help.txt", "helpArguments.json" };

            foreach (string fileName in fileList)
                if (!File.Exists(fileName))
                    downloadFiles(fileName);

        }

        private static void downloadFiles(string fileName)
        {
            WebClient client = new WebClient();

            client.DownloadFile("https://raw.githubusercontent.com/DwayneDavies/CurTerm/blob/master/CurTerm/bin/Release/netcoreapp3.1/publish/" + fileName, fileName);
        }

    }

}
