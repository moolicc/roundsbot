using System;
using System.Collections.Generic;
using System.Text;
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
            throw new NotImplementedException();
        }

        public override string GetHelpText()
        {
            throw new NotImplementedException();
        }

        public override void Trigger(DiscordMessage message, CommandHostModule host, params string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
