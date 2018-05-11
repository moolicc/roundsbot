using System;
using System.Collections.Generic;
using System.Linq;
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

        private DiscordChannel _channel;
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

            await DiscordClient.ConnectAsync();

            if (DiscordConfig.ChannelId != 0)
            {
                _channel = await DiscordClient.GetChannelAsync(DiscordConfig.ChannelId);
            }
        }

        private Task MessageCreated(MessageCreateEventArgs e)
        {
            if(e.MentionedUsers.All(d => d.Id != DiscordClient.CurrentUser.Id))
            {
                return Task.CompletedTask;
            }
            if (DiscordConfig.ChannelId == 0)
            {
                DiscordConfig.ChannelId = e.Channel.Id;
                _channel = DiscordClient.GetChannelAsync(DiscordConfig.ChannelId).Result;
            }
            else if (e.Channel.Id != DiscordConfig.ChannelId)
            {
                return Task.CompletedTask;
            }

            _lastMessage = e.Message;
            HandleCommand(e.Message.Content.Replace(DiscordClient.CurrentUser.Mention, ""));

            return Task.CompletedTask;
        }

        public void AddReaction(string reaction)
        {
            _lastMessage.CreateReactionAsync(DiscordEmoji.FromName(DiscordClient, reaction));
        }

        public void SendInvalidCommand(CommandBase command)
        {
            SendMessage($"{command.Name}: **Invalid Command Usage!**{Environment.NewLine}`{command.Usage}`");
        }
        
        public void SendCommandMessage(CommandBase command, string message)
        {
            SendMessage($"**{command.Name}**:{Environment.NewLine}{message}");
        }

        public void SendMessage(DiscordEmbed discordEmbed)
        {
            if (_channel == null)
            {
                return;
            }
            _channel.SendMessageAsync(embed: discordEmbed);
        }

        public void SendMessage(string text)
        {
            if (_channel == null)
            {
                return;
            }
            DiscordClient.SendMessageAsync(_channel, text);
        }

        private void HandleCommand(string text)
        {
            var parsed = ParseCommand(text);
            if (!Commands.ContainsKey(parsed.name))
            {
                SendMessage($"Unknown command *{parsed.name}*");
                return;
            }
            Commands[parsed.name].Execute(this, parsed.args);
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
