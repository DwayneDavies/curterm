using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.ComTypes;

namespace CurTerm
{

    class Program
    {

        static void Main(string[] args)
        {

            if (new Ping().Send("www.google.com.mx").Status != IPStatus.Success)
            {
                Console.WriteLine("You do not seem to be connected to the internet!");

                return;
            }

            Init.checkFiles();

            ParameterEngine options = ParameterEngine.FactoryMethod(args);

            // We do not want to do anything else if we only have "help" and/or "list" commands given.
            if (options.PrematureTerm)
                return;
 
            double rate;
            string from = options["from"].ToUpper(), to = options["to"].ToUpper();
            double amount = Convert.ToDouble(options["amount"]);
          
            if (from == to)
                from = "USD";
          
            string rateString = (from + "_" + to);

            using (WebClient wc = new WebClient())
            {
                string downloadString = ("https://free.currconv.com/api/v7/convert?q=" + rateString + "&compact=ultra&apiKey=99994e0b5fbe6d00990a");
                string jsonString = wc.DownloadString(downloadString);
                JObject jsonObject = JObject.Parse(jsonString);

                rate = (double) jsonObject[rateString];
            }

            Console.WriteLine(amount.ToString("N0") +  " " + from +  " = " + (Math.Round(rate, 2) * amount).ToString("N0") + " " + to);
        }

    }
}
