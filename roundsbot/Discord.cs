﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Net.WebSocket;
using roundsbot.Commands;

namespace roundsbot
{
    class Discord
    {
        public Dictionary<string, CommandBase> Commands { get; private set; }
        public DiscordClient DiscordClient { get; private set; }
        public Configuration DiscordConfig { get; private set; }
        public DiscordChannel Channel { get; private set; }
        public DateTime StartTime { get; private set; }

        public event Action<MessageReactionAddEventArgs> OnReactionAdded;
        public event Action<MessageReactionRemoveEventArgs> OnReactionRemoved;

        private DiscordMessage _lastBotMessage;
        private DiscordMessage _lastMessage;


        public Discord()
        {
            Commands = new Dictionary<string, CommandBase>();
        }

        public void AddCommand(CommandBase command)
        {
            Commands.Add(command.Name, command);
        }

        public async void Connect(Configuration config)
        {
            DiscordConfig = config;
            var discordConfig = new DiscordConfiguration
            {   
                Token = config.Token,
                TokenType = TokenType.Bot,

                AutoReconnect = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            };
            DiscordClient = new DiscordClient(discordConfig);
            DiscordClient.SetWebSocketClient<WebSocket4NetCoreClient>();
            DiscordClient.MessageCreated += MessageCreated;
            DiscordClient.MessageReactionAdded += ReactionAdded;
            DiscordClient.MessageReactionRemoved += ReactionRemoved;

            await DiscordClient.ConnectAsync();

            if (DiscordConfig.ChannelId != 0)
            {
                Channel = await DiscordClient.GetChannelAsync(DiscordConfig.ChannelId);
            }

            StartTime = DateTime.Now;
        }

        public async void Close()
        {
            await DiscordClient.DisconnectAsync();
        }

        public async void SetConfig(Configuration config)
        {
            Channel = await DiscordClient.GetChannelAsync(DiscordConfig.ChannelId);
        }


        private Task ReactionAdded(MessageReactionAddEventArgs e)
        {
            if (e.User.IsCurrent)
            {
                return Task.CompletedTask;
            }

            if (e.Message.Id == _lastBotMessage.Id)
            {
                Console.WriteLine("[{0}] {1} added reaction {2}", e.User.Id, e.User.Username, e.Emoji.GetDiscordName());
                OnReactionAdded?.Invoke(e);
            }
            return Task.CompletedTask;
        }

        private Task ReactionRemoved(MessageReactionRemoveEventArgs e)
        {
            if (e.User.IsCurrent)
            {
                return Task.CompletedTask;
            }
            if (e.Message.Id == _lastBotMessage.Id)
            {
                Console.WriteLine("[{0}] {1} removed reaction {2}", e.User.Id, e.User.Username, e.Emoji.GetDiscordName());
                OnReactionRemoved?.Invoke(e);
            }
            return Task.CompletedTask;
        }

        private Task MessageCreated(MessageCreateEventArgs e)
        {
            if (e.Author.IsCurrent)
            {
                _lastBotMessage = e.Message;
                return Task.CompletedTask;
            }
            if(e.MentionedUsers.All(d => d.Id != DiscordClient.CurrentUser.Id))
            {
                return Task.CompletedTask;
            }
            if (DiscordConfig.ChannelId == 0)
            {
                DiscordConfig.ChannelId = e.Channel.Id;
                Channel = DiscordClient.GetChannelAsync(DiscordConfig.ChannelId).Result;
            }
            else if (e.Channel.Id != DiscordConfig.ChannelId)
            {
                return Task.CompletedTask;
            }

            RoundService.Instance.Activity = true;
            _lastMessage = e.Message;

            var commandWithoutMention = e.Message.Content.Replace(DiscordClient.CurrentUser.Mention, "");
            Console.WriteLine("[{0}] {1}: {2}", e.Author.Id, e.Author.Username, commandWithoutMention);
            var commands = commandWithoutMention.Split(';', StringSplitOptions.RemoveEmptyEntries);
            HandleCommand(commands);

            return Task.CompletedTask;
        }

        public void SetStatus(string status)
        {
            DiscordClient.UpdateStatusAsync(new DiscordGame(status));
        }

        public void AddReaction(string reaction)
        {
            _lastMessage.CreateReactionAsync(DiscordEmoji.FromName(DiscordClient, reaction)).Wait();
        }

        public void AddBotReaction(string reaction)
        {
            _lastBotMessage.CreateReactionAsync(DiscordEmoji.FromName(DiscordClient, reaction)).Wait();
        }

        public void SendInvalidCommand(CommandBase command)
        {
            SendMessage($"**{command.Name}**: *Invalid Command Usage!*{Environment.NewLine}`{command.Usage}`");
        }
        
        public void SendCommandMessage(CommandBase command, string message)
        {
            SendMessage($"**{command.Name}**:{Environment.NewLine}{message}");
        }

        public void SendMessage(DiscordEmbed discordEmbed)
        {
            if (Channel == null)
            {
                return;
            }
            Channel.SendMessageAsync(embed: discordEmbed).Wait();
        }

        public void SendMessage(string text)
        {
            if (Channel == null)
            {
                return;
            }
            Console.WriteLine("Roundsbot: {0}", text);
            DiscordClient.SendMessageAsync(Channel, text).Wait();
        }

        public void PumpCommand(string command)
        {
            var parsed = ParseCommand(command);
            if (!Commands.ContainsKey(parsed.name))
            {
                SendMessage($"Unknown command *{parsed.name}*");
                return;
            }
            Commands[parsed.name].Execute(this, parsed.args);
        }

        private void HandleCommand(string[] commandTexts)
        {
            foreach (var text in commandTexts)
            {
                PumpCommand(text.Trim());
            }
        }
        
        private (string name, string[] args) ParseCommand(string text)
        {
            string[] split = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string name = split[0];

            string[] args = new string[split.Length - 1];
            Array.Copy(split, 1, args, 0, args.Length);

            return (name, args);
        }
    }
}
