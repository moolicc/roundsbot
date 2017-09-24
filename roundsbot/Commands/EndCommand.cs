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
            throw new NotImplementedException();
        }

        public override string GetHelpText()
        {
            throw new NotImplementedException();
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
