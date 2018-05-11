using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace roundsbot.Commands
{
    class UptimeCommand : CommandBase
    {
        public override string Name => "uptime";
        public override string Usage => "uptime";
        public override string Description => "Displays the amount of time the bot has been online.";

        public DateTime StartTime { get; private set; }


        public UptimeCommand()
        {
            StartTime = DateTime.UtcNow;
        }

        public override void Execute(Discord discord, params string[] args)
        {
            var difference = TimeSpan.FromTicks(DateTime.UtcNow.Ticks - StartTime.Ticks);
            discord.SendCommandMessage(this, $"The bot has been online since: {StartTime} (UTC).{Environment.NewLine} "+
                                             $"The bot has been online for a total of {difference.TotalDays:0.00} days.");
        }
    }
}
