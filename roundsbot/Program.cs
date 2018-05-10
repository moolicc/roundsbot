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
            var discord = new Discord();
            discord.Connect(config);

            //TODO: Create REPL (and a command to save the config)
            while (Console.ReadLine() != "exit")
            {
                
            }
            
            File.WriteAllText("conf.json", JsonConvert.SerializeObject(discord.DiscordConfig));
        }
    }
}
