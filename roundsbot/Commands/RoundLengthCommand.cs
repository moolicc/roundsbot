using System;
using System.Collections.Generic;
using System.Text;
using DSharpPlus.Entities;
using roundsbot.Modules;

namespace roundsbot.Commands
{
    class RoundLengthCommand : Command
    {
        public override string GetName()
        {
            return "roundlength";
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
            if (args.Length > 0)
            {
                if (int.TryParse(args[0], out var length))
                {
                    host.Configuration.RoundLength = length;
                    if (RoundData.RunTask != null)
                    {
                        host.NotifyUsers("Round length will reset once the next round starts.", false);
                    }
                }
                else
                {
                    host.NotifyUsers("Invalid value. *(Hint: A valid number might work)*", false);
                }
            }
            else
            {
                host.NotifyUsers($"The current round length is {host.Configuration.RoundLength} minutes.", false);
            }
        }
    }
}
