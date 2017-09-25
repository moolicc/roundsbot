using System;
using System.Collections.Generic;
using System.Text;
using DSharpPlus.Entities;
using roundsbot.Modules;

namespace roundsbot.Commands
{
    class UnsubscribeCommand : Command
    {
        public override string GetName()
        {
            return "unsubscribe";
        }

        public override string GetDescriptionText()
        {
            return "Marks you as one who is most certainly **not** interested in being notified when this bot feels like its necessary to notify you (At the start/end of a round).";
        }

        public override string GetHelpText()
        {
            return "I hope you're kidding and don't really need an in-depth explanation of this command. :joy:";
        }

        public override void Trigger(DiscordMessage message, CommandHostModule host, params string[] args)
        {
            if (!host.Configuration.SubscribedUsers.Contains(message.Author.Id))
            {
                SendMessage(message.Channel, $"Excuse me sir/ma'am, but my records show that you are already not subscribed. {Emojies.SUBSCRIBE}{Environment.NewLine}*(Hint: Try `subscribe` to subscribe)*");
            }
            else
            {
                host.Configuration.SubscribedUsers.Remove(message.Author.Id);
                React(message, host, Emojies.OK);
            }
        }
    }
}
