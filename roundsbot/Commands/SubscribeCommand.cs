using System;
using System.Collections.Generic;
using System.Text;
using DSharpPlus.Entities;
using roundsbot.Modules;

namespace roundsbot.Commands
{
    class SubscribeCommand : Command
    {
        public override string GetName()
        {
            return "subscribe";
        }

        public override string GetDescriptionText()
        {
            return "Marks you as one who is interested in being notified when this bot feels like its necessary to notify you (At the start/end of a round. And only if you're \"Online\" or \"Idle\".).";
        }

        public override string GetHelpText()
        {
            return "I don't really know what more you should know about this. :thinking:";
        }

        public override void Trigger(DiscordMessage message, CommandHostModule host, params string[] args)
        {
            if (host.Configuration.SubscribedUsers.Contains(message.Author.Id))
            {
                SendMessage(message.Channel, $"Excuse me sir/ma'am, but my records show that you have already subscribed. {Emojies.SUBSCRIBE}{Environment.NewLine}*(Hint: Try `unsubscribe` to unsubscribe)*");
            }
            else
            {
                host.Configuration.SubscribedUsers.Add(message.Author.Id);
                React(message, host, Emojies.OK);
            }
        }
    }
}
