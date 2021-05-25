using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace CurTerm
{
    class Currencies
    {
        //Symbol : Country name.
        private Dictionary<string, string> currencies;

        public Currencies()
        {
            string jsonString = System.IO.File.ReadAllText("currencies.json");

            currencies = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
        }

        public bool isCurrency(string currency)
        {
            return currencies.ContainsKey(currency);
        }

        public void ListCurrencies()
        {
            Console.WriteLine("\nSupported Currencies: ");

            foreach (var item in currencies)
                Console.WriteLine(item.Key + " : " + item.Value);

            Console.WriteLine("\n");
        }

    }

}
