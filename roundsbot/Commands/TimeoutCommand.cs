using System;
using System.Collections.Generic;
using System.Text;
using DSharpPlus.Entities;
using roundsbot.Modules;

namespace roundsbot.Commands
{
    class TimeoutCommand : Command
    {
        public override string GetName()
        {
            return "timeout";
        }

        public override string GetDescriptionText()
        {
            return "Gets/sets the number of rounds that may run without any user activity before automatically ending.";
        }

        public override string GetHelpText()
        {
            return "\"timeout\" to get the current value or \"timeout [integer]\" to set the value.";
        }

        public override void Trigger(DiscordMessage message, CommandHostModule host, params string[] args)
        {
            if (args.Length > 0)
            {
                if (int.TryParse(args[0], out var count))
                {
                    host.Configuration.TimeoutCount = count;
                    React(message, host, Emojies.OK);
                }
                else
                {
                    SendMessage(message.Channel, "Invalid value. *(Hint: A valid number might work)*");
                }
            }
            else
            {
                SendMessage(message.Channel, $"The current round timeout is {host.Configuration.TimeoutCount} rounds.");
            }
        }
    }
}
