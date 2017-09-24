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
            throw new NotImplementedException();
        }

        public override string GetHelpText()
        {
            throw new NotImplementedException();
        }

        public override void Trigger(DiscordMessage message, CommandHostModule host, params string[] args)
        {
            if (host.Configuration.SubscribedUsers.Contains(message.Author.Id))
            {
                host.NotifyUsers(
                    $"Excuse me sir/ma'am, but my records show that you have already subscribed. {Emojies.SUBSCRIBE}{Environment.NewLine}*(Hint: Try `unsubscribe` to unsubscribe)*",
                    false);
            }
            else
            {
                host.Configuration.SubscribedUsers.Add(message.Author.Id);
                React(message, host, Emojies.OK);
            }
        }
    }
}
