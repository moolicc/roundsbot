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
            return "Gets/sets the amount of time (in minutes) a round lasts for.";
        }

        public override string GetHelpText()
        {
            return "\"roundlength\" to get the current value or \"roundlength [integer]\" to set the value.";
        }

        public override void Trigger(DiscordMessage message, CommandHostModule host, params string[] args)
        {
            if (args.Length > 0)
            {
                if (int.TryParse(args[0], out var length))
                {
                    host.Configuration.RoundLength = length;
                    React(message, host, Emojies.OK);

                    if (RoundData.RunTask != null)
                    {
                        host.NotifyUsers("Round length will reset once the next round starts.", false);
                    }
                }
                else
                {
                    host.NotifyUsers("Invalid value. *(Hint: A valid number might work)*");
                }
            }
            else
            {
                host.NotifyUsers($"The current round length is {host.Configuration.RoundLength} minutes.");
            }
        }
    }
}
