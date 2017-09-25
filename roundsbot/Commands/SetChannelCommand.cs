using System;
using System.Collections.Generic;
using System.Text;
using DSharpPlus.Entities;
using roundsbot.Modules;

namespace roundsbot.Commands
{
    class SetChannelCommand : Command
    {
        public override string GetName()
        {
            return "setchannel";
        }

        public override string GetDescriptionText()
        {
            return "Sets the active \"rounds\" channel.";
        }

        public override string GetHelpText()
        {
            return "Sets the active \"rounds\" channel.";
        }

        public override void Trigger(DiscordMessage message, CommandHostModule host, params string[] args)
        {
            host.Configuration.ChannelId = message.ChannelId;
            host.NotifyUsers("Active channel set.");
        }
    }
}
