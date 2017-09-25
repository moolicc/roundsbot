﻿using System;
using System.Threading.Tasks;
using DSharpPlus;
using roundsbot.Modules;

namespace roundsbot
{
    class Program
    {

        private static DiscordClient _discordClient;
        private static Configuration _config;

        static void Main(string[] args)
        {
            RoundData.TimeoutTimer = 0;
            RoundData.Activity = false;
            _config = new Configuration();


            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();

            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
        }

        private static async Task MainAsync(string[] args)
        {
            _discordClient = new DiscordClient(new DiscordConfiguration()
            {
                Token = System.IO.File.ReadAllText("bottoken.txt"),
                TokenType = TokenType.Bot
            });
            LoadModules();


            await _discordClient.ConnectAsync();
            await Task.Delay(-1);
        }

        private static void LoadModules()
        {
            var commandModule = new CommandHostModule();
            commandModule.Init(_discordClient, _config);
            LoadCommands(commandModule);
        }

        private static void LoadCommands(CommandHostModule hostModule)
        {
            hostModule.Commands.Add(new Commands.StartCommand());
            hostModule.Commands.Add(new Commands.EndCommand());
            hostModule.Commands.Add(new Commands.RoundLengthCommand());
            hostModule.Commands.Add(new Commands.BreakLengthCommand());
            hostModule.Commands.Add(new Commands.TimeoutCommand());
            hostModule.Commands.Add(new Commands.SubscribeCommand());
            hostModule.Commands.Add(new Commands.UnsubscribeCommand());
            hostModule.Commands.Add(new Commands.HelpCommand());
            hostModule.Commands.Add(new Commands.UptimeCommand());
        }
    }
}
