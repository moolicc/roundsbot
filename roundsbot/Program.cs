using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Net.WebSocket;
using Newtonsoft.Json;

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

            var roundService = new RoundService(_discord);

            _discord.AddCommand(new Commands.UptimeCommand());
            _discord.AddCommand(new Commands.BreakLengthCommand());
            _discord.AddCommand(new Commands.HelpCommand());
            _discord.AddCommand(new Commands.RoundLengthCommand());
            _discord.AddCommand(new Commands.StartCommand());
            _discord.AddCommand(new Commands.StopCommand());
            _discord.AddCommand(new Commands.TimeoutCommand());

            Repl();
        }

        private static void Repl()
        {
            _exiting = false;
            while (!_exiting)
            {
                var input = Console.ReadLine().Trim().ToLower();
                string[] split = null;
                if (input.Contains(' '))
                {
                    split = input.Split(' ');
                }
                else
                {
                    split = new string[1] { input };
                }
                Evaluate(split);
            }
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
        }
    }
}
