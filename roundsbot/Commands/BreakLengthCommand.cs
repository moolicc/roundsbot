using System;
using System.Collections.Generic;
using System.Text;
using DSharpPlus.Entities;
using roundsbot.Modules;

namespace roundsbot.Commands
{
    class BreakLengthCommand : Command
    {
        public override string GetName()
        {
            return "breaklength";
        }

        public override string GetDescriptionText()
        {
            return "Gets/sets the amount of time (in minutes) a break between rounds lasts for.";
        }

        public override string GetHelpText()
        {
            return "\"breaklength\" to get the current value or \"breaklength [integer]\" to set the value.";
        }

        public override void Trigger(DiscordMessage message, CommandHostModule host, params string[] args)
        {
            if (args.Length > 0)
            {
                if (int.TryParse(args[0], out var length))
                {
                    host.Configuration.BreakLength = length;
                    React(message, host, Emojies.OK);

                    if (RoundData.RunTask != null)
                    {
                        host.NotifyUsers("Break length will reset once the next round starts.", false);
                    }
                }
                else
                {
                    SendMessage(message.Channel, "Invalid value. *(Hint: A valid number might work)*");
                }
            }
            else
            {
                SendMessage(message.Channel, $"The current break length is {host.Configuration.BreakLength} minutes.");
            }
        }
    }
}
