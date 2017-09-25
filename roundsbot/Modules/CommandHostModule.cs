using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using roundsbot.Commands;


namespace roundsbot.Modules
{
    class CommandHostModule : ModuleBase
    {
        public List<Command> Commands { get; private set; }

        public DiscordClient DiscordClient { get; private set; }
        public Configuration Configuration { get; private set; }

        public CommandHostModule()
        {
            Commands = new List<Command>();
        }

        public override void Init(DiscordClient discordClient, Configuration config)
        {
            DiscordClient = discordClient;
            Configuration = config;
            DiscordClient.MessageCreated += MessageCreated;
        }

        private async Task MessageCreated(DSharpPlus.EventArgs.MessageCreateEventArgs e)
        {
            if (Configuration.ChannelId == 0)
            {
                Configuration.ChannelId = e.Channel.Id;
            }
            if (e.Channel.Id != Configuration.ChannelId)
            {
                return;
            }
            if (e.Author.IsBot)
            {
                return;
            }
            RoundData.Activity = true;
            if (!e.Message.MentionedUsers.Contains(DiscordClient.CurrentUser))
            {
                return;
            }
            
            string messageContent = e.Message.Content.Replace(DiscordClient.CurrentUser.Mention, "");
            var messages = new string[] { messageContent };
            if (messageContent.Contains(';'))
            {
                messages = messageContent.Split(';');
            }
            else if (messageContent.Contains(';')) //GREEK QUESTION MARK
            {
                NotifyUsers("You tried the greek question mark. Surely you've read the help page which clarifies "
                    +"that the delimiter isn't a greek question mark? Well, *the help information lied.* I fully support that character." +
                    Environment.NewLine + "I guess you *can* have your cake and eat it to. " + Emojies.CAKE + " " + Emojies.FORK_KNIFE);
                messages = messageContent.Split(';');
            }
            foreach (var msg in messages)
            {
                var parsed = ParseInput(msg.Trim());
                HandleMessage(parsed, e.Message);
            }
        }

        private void HandleMessage((string name, string[] args) parsed, DiscordMessage message)
        {
            if (string.IsNullOrWhiteSpace(parsed.name))
            {
                return;
            }
            ExecCommand(parsed.name, message, parsed.args);
        }

        public async void NotifyUsers(string message, bool mention = false)
        {
            if (mention)
            {
                StringBuilder messageBuilder = new StringBuilder();

                messageBuilder.Append($"{Emojies.WARNING} Attention");
                for (var i = 0; i < Configuration.SubscribedUsers.Count; i++)
                {
                    var userId = Configuration.SubscribedUsers[i];
                    var user = await DiscordClient.GetUserAsync(userId);
                    if (user.Presence.Status == UserStatus.Online || user.Presence.Status == UserStatus.Idle)
                    {
                        if (i == Configuration.SubscribedUsers.Count - 1 && Configuration.SubscribedUsers.Count > 1)
                        {
                            messageBuilder.Append(" and");
                        }
                        else if (i != 0 && Configuration.SubscribedUsers.Count > 1)
                        {
                            messageBuilder.Append(",");
                        }
                        messageBuilder.Append(' ').Append(user.Mention);
                    }
                }

                messageBuilder.Append("! ").Append(message);
                await DiscordClient.SendMessageAsync(await DiscordClient.GetChannelAsync(Configuration.ChannelId), messageBuilder.ToString());
            }
            else
            {
                await DiscordClient.SendMessageAsync(await DiscordClient.GetChannelAsync(Configuration.ChannelId), message);
            }
        }

        public void ExecCommand(string commandName, DiscordMessage message, params string[] args)
        {
            var command = Commands.FirstOrDefault(c => c.GetName() == commandName);
            if (command == null)
            {
                return;
            }
            try
            {
                command.Trigger(message, this, args);
            }
            catch (Exception e)
            {
                NotifyUsers(e.Message, false);
            }
        }

        private static (string name, string[] args) ParseInput(string input)
        {
            List<string> parameters = new List<string>();
            StringBuilder curParam = new StringBuilder();
            bool quote = false;
            for (int i = 0; i < input.Length; i++)
            {
                char curChar = input[i];
                char nextChar = '\0';
                if (i + 1 < input.Length)
                {
                    nextChar = input[i + 1];
                }

                if (curChar == '\\' && nextChar == '\"')
                {
                    curParam.Append('\"');
                    i++;
                    continue;
                }
                if (curChar == '\"')
                {
                    quote = !quote;
                }
                if (curChar == ' ' && !quote)
                {
                    parameters.Add(curParam.ToString());
                    curParam.Clear();
                    continue;
                }

                curParam.Append(curChar);


                if (nextChar == '\0')
                {
                    parameters.Add(curParam.ToString());
                    curParam.Clear();
                }
            }

            string name = "";
            if (parameters.Count > 0)
            {
                name = parameters[0];
                parameters.RemoveAt(0);
            }

            return (name, parameters.ToArray());
        }
    }
}