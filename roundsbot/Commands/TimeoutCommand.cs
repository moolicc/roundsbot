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
                if (int.TryParse(args[0], out var count))
                {
                    host.Configuration.TimeoutCount = count;
                }
                else
                {
                    host.NotifyUsers("Invalid value. *(Hint: A valid number might work)*", false);
                }
            }
            else
            {
                host.NotifyUsers($"The current round timeout is {host.Configuration.TimeoutCount} rounds.", false);
            }
        }
    }
}
