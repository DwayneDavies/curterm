using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;

namespace CurTerm
{

    class ParameterEngine
    {
        private string[] argList;
        string[] infoArgs;
        private Dictionary<string, MethodInfo> supportedArgs;
        private Dictionary<string, string> options;
        // Used by main to check if it should not do other stuff.
        public bool PrematureTerm = false;
        private static Currencies currencies;

        public ParameterEngine(string[] argList, string [] args, char divider, Dictionary<string, string> options, string[] infoArgs = null)
        {
            // Don't forget to replace "/" and "--" charactes in our args with "-"! 
            string[] arguments = string.Join(" ", args).Replace('/', '-').Replace("--", "-").Split(divider);
            // Our supported arguments so far. Others are ignored.
            this.argList = argList;
            this.infoArgs = infoArgs;
            // Stores options, including some default ones.
            this.options = options;
            // The supported functions all have their own named methods.
            supportedArgs = new Dictionary<string, MethodInfo>();

            foreach (string arg in argList)
                supportedArgs[arg] = this.GetType().GetTypeInfo().GetDeclaredMethod(arg);

            // Add option, but only if it has a named function that handles that that option does. If it is help, call that instead.
            for (int i = 1; i < arguments.Length; i++)
                if (arguments[i].StartsWith("help"))
                    help(arguments[i]);
                else
                    addOption(arguments[i]);

            // If only "help" and "list" options are given, lets not do other stuff.
            if (isInfoOnly(arguments))
                PrematureTerm = true;

        }

        public static ParameterEngine FactoryMethod(string[] args, char divider = '-')
        {
            string[] argList = new string[] { "amount", "from", "list", "to" };
            Dictionary<string, string> options = new Dictionary<string, string>();

            // Set defaults, though they may be overwritten later.
            options["from"] = "NZD";
            options["to"] = "USD";
            options["amount"] = "1";

            currencies = new Currencies();
            string[] infoArgs = new string[] { "help", "list" };

            return new ParameterEngine(argList, args, divider, options, infoArgs);
        }

        private void help(string argument)
        {
            string[] split = argument.Split(" ");
       
            if ( (split.Length < 2 ) )
                if (File.Exists("help.txt"))
                    Console.WriteLine(File.ReadAllText("help.txt") + "\n");
                else
                    Console.WriteLine("Help file has gone missing or is otherwise inaccessible!");
            else
                getHelpArgument(split[1]);

        }

        private void addOption(string potentialOption)
        {
            string toSplit = potentialOption;

            // If option has no parameter, this will stop undesired behaviour later.
            if (toSplit.IndexOf(' ') < 0)
                toSplit += " ";

            string[] str = toSplit.Split(' ');

            if (argsContains(argList, str[0]))
                if ((bool)supportedArgs[str[0]].Invoke(this, new object[] { str[1] }))
                    options[str[0]] = str[1];

        }

        private bool argsContains(string[] args, string toFind)
        {
            return (Array.FindIndex(args, x => x == toFind) > -1);
        }

        private void getHelpArgument(string argument)
        {
            string jsonString = System.IO.File.ReadAllText("helpArguments.json");
            Dictionary<string, string> helpArguments = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);

            if (helpArguments.ContainsKey(argument))
                Console.WriteLine("\nHelp for the " + argument + " argument: \n" + helpArguments[argument] + "\n");
            else
                Console.WriteLine("We do not have any help information for the " + argument + " argument\n");

        }

        private bool isInfoOnly(string[] args)
        {

            if (infoArgs == null)
                return false;

            var items = argList.Except(infoArgs);

            foreach (var item in args)
                if (items.Contains(item.Split(" ")[0]))
                    return false;

            return true;
        }

        public string this[string key]
        {
            get { return options[key]; }
        }

        private bool amount(string a)
        {
            bool isValid = float.TryParse(a, out _);

            if (!isValid)
                Console.WriteLine("That is not a valid amount!");

            return isValid;
        }

        private bool from(string currency)
        {
            bool isValid = currencies.isCurrency(currency.ToUpper());

            if (!isValid)
                Console.WriteLine("Your from currency is not a valid currency!");

            return isValid;
        }

        private bool list(string sourceFile = "")
        {
            currencies.ListCurrencies();

            return true;
        }

        private bool to(string currency)
        {
            bool isValid = currencies.isCurrency(currency.ToUpper());

            if (!isValid)
                Console.WriteLine("Your to currency is not a valid currency!");

            return isValid;
        }

    }

}
