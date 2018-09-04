using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Net.WebSocket;
using Newtonsoft.Json;
using roundsbot.Commands.User;

namespace roundsbot
{
    class Program
    {
        private static Discord _discord;
        private static bool _exiting;

        static void Main(string[] args)
        {
            if (!File.Exists("conf.json"))
            {
                var conf = new Configuration();
                File.WriteAllText("conf.json", JsonConvert.SerializeObject(conf));

                Console.WriteLine("Please edit the conf.json file and add the bot's token. Then restart the bot.");
                Console.ReadLine();
                return;
            }

            var config = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText("conf.json"));
            _discord = new Discord();
            _discord.Connect(config);

            //Instantiate this so that the static 'Instance' field is set.
            var roundService = new RoundService(_discord);
            var subService = new SubService(_discord);

            _discord.AddCommand(new UptimeCommand());
            _discord.AddCommand(new BreakLengthCommand());
            _discord.AddCommand(new HelpCommand());
            _discord.AddCommand(new RoundLengthCommand());
            _discord.AddCommand(new StartCommand());
            _discord.AddCommand(new StopCommand());
            _discord.AddCommand(new TimeoutCommand());

            Repl();
        }

        private static void Repl()
        {
            _exiting = false;
            while (!_exiting)
            {
                var input = Console.ReadLine().Trim().ToLower();
                var split = SplitCommand(input);
                Evaluate(split);
            }
        }

        private static string[] SplitCommand(string input)
        {
            List<string> commands = new List<string>();
            bool inQuote = false;
            string curCommand = "";
            for (int i = 0; i < input.Length; i++)
            {
                var curChar = input[i];
                if (curChar == '"')
                {
                    inQuote = !inQuote;
                    continue;
                }

                if (curChar == '\\')
                {
                    if (input.Length > i + 1)
                    {
                        if (input[i + 1] == '"')
                        {
                            curCommand += "\"";
                            i++;
                            continue;
                        }
                    }
                    curCommand += "\\";
                    continue;
                }

                if (curChar == ' ' && !inQuote)
                {
                    commands.Add(curCommand);
                    curCommand = "";
                    continue;
                }

                curCommand += curChar;
            }
            commands.Add(curCommand);

            return commands.ToArray();
        }
        
        private static void Evaluate(string[] input)
        {
            if (input[0] == "exit")
            {
                _exiting = true;
            }
            else if (input[0] == "save")
            {
                File.WriteAllText("conf.json", JsonConvert.SerializeObject(_discord.DiscordConfig));
            }
            else if (input[0] == "cmd")
            {
                EvaluateDiscordCommand(input);
            }
        }

        private static void EvaluateDiscordCommand(string[] commands)
        {
            for (int i = 1; i < commands.Length; i++)
            {
                _discord.PumpCommand(commands[i]);
            }
        }
    }
}
