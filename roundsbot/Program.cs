using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Net.WebSocket;
using Newtonsoft.Json;
using roundsbot.Commands;

namespace roundsbot
{
    class Program
    {
        private static Discord _discord;
        private static bool _exiting;

        static void Main(string[] args)
        {
            //TODO: We have some duplication here and in the terminal command processing code below.
            //TODO: We need to cleanup the terminal command processing code.
            if (!File.Exists("conf.json"))
            {
                var conf = new Configuration();
                File.WriteAllText("conf.json", JsonConvert.SerializeObject(conf));

                Console.WriteLine("Please edit the conf.json file and add the bot's token. Then restart the bot.");
                Console.ReadLine();
                return;
            }

            try
            {
                Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("[err]");
            }
        }

        private static void Run()
        {
            while (!HasInternet())
            {
            }

            var config = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText("conf.json"));
            _discord = new Discord();
            _discord.OnClosed += DiscordClosed;
            var result = _discord.Connect(config);
            result.Wait();

            //Instantiate this so that the static 'Instance' field is set.
            var roundService = new RoundService(_discord);
            var subService = new SubService();
            subService.Init(_discord);

            _discord.AddCommand(new UptimeCommand());
            _discord.AddCommand(new BreakLengthCommand());
            _discord.AddCommand(new HelpCommand());
            _discord.AddCommand(new RoundLengthCommand());
            _discord.AddCommand(new StartCommand());
            _discord.AddCommand(new StopCommand());
            _discord.AddCommand(new TimeoutCommand());

            Repl();
        }

        private static void DiscordClosed()
        {
            System.Threading.Thread.Sleep(30000);
            _discord.OnClosed -= DiscordClosed;
            _discord = null;
            Run();
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
            else if (input[0] == "saveconf")
            {
                File.WriteAllText("conf.json", JsonConvert.SerializeObject(_discord.DiscordConfig));
            }
            else if (input[0] == "loadconf")
            {
                if (RoundService.Instance != null && RoundService.Instance.IsRunning)
                {
                    _discord.SendMessage("Ending rounds to reload bot configuration...");
                    RoundService.Instance.StopRounds();
                }
                SubService.Instance.Close(); 
                var configContents = File.ReadAllText("conf.json");
                var config = JsonConvert.DeserializeObject<Configuration>(configContents);
                _discord.SetConfig(config);
                SubService.Instance.Init(_discord);
            }
            else if (input[0] == "close")
            {
                _discord.SendMessage("Host is forcing closure of the bot.");
                if (RoundService.Instance != null && RoundService.Instance.IsRunning)
                {
                    RoundService.Instance.StopRounds();
                }
                SubService.Instance.Close();
                _discord.Close();
                var configContents = File.ReadAllText("conf.json");
                var config = JsonConvert.DeserializeObject<Configuration>(configContents);
                _discord.SetConfig(config);
                SubService.Instance.Init(_discord);
            }
            else if (input[0] == "open")
            {
                var discordConfigContents = File.ReadAllText("conf.json");
                var discordConfig = JsonConvert.DeserializeObject<Configuration>(discordConfigContents);

                _discord = new Discord();
                _discord.SetConfig(discordConfig);
                _discord.Connect(discordConfig);

                var rounds = new RoundService(_discord);
                var sub = new SubService();
                SubService.Instance.Init(_discord);
            }
            else if (input[0] == "cmd")
            {
                EvaluateDiscordCommand(input);
            }
            else if (input[0] == "msg")
            {
                _discord.SendMessage(input[1]);
            }
            else if (input[0] == "channel")
            {
                Console.WriteLine(string.Format("[{0}] {1}", _discord.Channel.Id, _discord.Channel.Name));
            }
            else if (input[0] == "uptime")
            {
                var difference = TimeSpan.FromTicks(DateTime.UtcNow.Ticks - _discord.StartTime.Ticks);
                Console.WriteLine($"{_discord.StartTime} (UTC); {difference.TotalDays:0.00} days.");
            }
#if DEBUG
            else if (input[0] == "throw")
            {
                throw new Exception("Controlled exception");
            }
#endif
        }

        private static void EvaluateDiscordCommand(string[] commands)
        {
            for (int i = 1; i < commands.Length; i++)
            {
                _discord.PumpCommand(commands[i]);
            }
        }

        private static bool HasInternet()
        {
            Ping ping = new Ping();

            bool result = false;
            try
            {
                var reply = ping.Send("8.8.8.8", 5000);
                result = reply.Status == IPStatus.Success;
            }
            catch (Exception ex)
            {
            }

            return result;
        }
    }
}
