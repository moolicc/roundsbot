using System;
using System.Collections.Generic;
using System.Text;
using DSharpPlus.Entities;
using roundsbot.Modules;

namespace roundsbot.Commands
{
    class UptimeCommand : Command
    {
        public override string GetName()
        {
            return "uptime";
        }

        public override string GetDescriptionText()
        {
            return "Displays the total time the bot has been online.";
        }

        public override string GetHelpText()
        {
            return
                "Displays the total time the bot has been online. You can optionally specify a formatting argument."
                + Environment.NewLine +"The default format is \"{0:%d}\"." 
                + Environment.NewLine + "See: https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-timespan-format-strings.";
        }

        public override void Trigger(DiscordMessage message, CommandHostModule host, params string[] args)
        {
            TimeSpan uptime = DateTime.Now.Subtract(host.Configuration.LaunchTime);
            string formatString = "{0:%d} day(s)";
            if (args.Length == 1)
            {
                formatString = args[0];
            }
            else if (args.Length != 0)
            {
                host.NotifyUsers("You over-estimate this command's abilities.");
                return;
            }

            host.NotifyUsers(string.Format(formatString, uptime));
            
        }
    }

}
