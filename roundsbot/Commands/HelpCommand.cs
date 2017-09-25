using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using roundsbot.Modules;

namespace roundsbot.Commands
{
    class HelpCommand : Command
    {
        public override string GetName()
        {
            return "help";
        }

        public override string GetDescriptionText()
        {
            return
                "Recursively displays help information... Uh oh... Recursively displays help information... Uh oh... Recursively displays help information... Uh oh..." +
                Environment.NewLine + "Recursively displays help information...Uh oh.. Recursi"+
                Environment.NewLine + "(Seriously though, you can use \"help [command]\" to get more detailed information about the specified command.)";
        }

        public override string GetHelpText()
        {
            return "What is this supposed to say?";
        }

        public override void Trigger(DiscordMessage message, CommandHostModule host, params string[] args)
        {
            if (args.Length == 0)
            {
                DiscordEmbedBuilder builder = new DiscordEmbedBuilder();
                builder.Title = "Available Commands";
                builder.Description =
                    "Note that you can chain commands together. Separating each with a ';' (It's a semicolon. Not the greek question mark. Don't try the greek character.).";
                lock (host)
                {
                    foreach (var hostCommand in host.Commands)
                    {
                        builder.AddField(hostCommand.GetName(), hostCommand.GetDescriptionText());
                    }
                }
                SendList(message.Channel, builder.Build());
            }
            else if (args.Length == 1)
            {
                StringBuilder builder = new StringBuilder();
                var command = host.Commands.FirstOrDefault(c => c.GetName() == args[0]);
                if (command == null)
                {
                    host.NotifyUsers("This is awkward. It seems like I have **no idea** what you're asking for help with. " + Emojies.NO_MOUTH);
                    return;
                }
                builder.Append("**").Append(command.GetName()).Append("**");
                builder.AppendLine();
                builder.Append(command.GetHelpText());
                host.NotifyUsers(builder.ToString());
            }
            else
            {
                host.NotifyUsers("This is awkward. I have no idea what you meant by that. " + Emojies.PENSIVE);
            }
        }

        private async void SendList(DiscordChannel channel, DiscordEmbed embed)
        {
            await channel.SendMessageAsync(embed: embed);
        }
    }
}
