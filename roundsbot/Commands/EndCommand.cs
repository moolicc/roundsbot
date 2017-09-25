using System;
using System.Collections.Generic;
using System.Text;
using DSharpPlus.Entities;
using roundsbot.Modules;

namespace roundsbot.Commands
{
    class EndCommand : Command
    {
        public override string GetName()
        {
            return "end";
        }

        public override string GetDescriptionText()
        {
            return "Stops any rounds that are currently running.";
        }

        public override string GetHelpText()
        {
            return "What can I say? This command takes no arguments. It has a clear purpose in this life.";
        }

        public override void Trigger(DiscordMessage message, CommandHostModule host, params string[] args)
        {
            if (RoundData.CancelTokenSource != null)
            {
                RoundData.End();
                React(message, host, Emojies.OK);
            }
        }
    }
}
