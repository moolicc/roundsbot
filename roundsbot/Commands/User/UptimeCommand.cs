using System;

namespace roundsbot.Commands.User
{
    class UptimeCommand : UserCommand
    {
        public override string Name => "uptime";
        public override string Usage => "uptime";
        public override string Description => "Displays the amount of time the bot has been online.";
        

        public UptimeCommand()
        {
        }

        public override void Execute(Discord discord, params string[] args)
        {
            var difference = TimeSpan.FromTicks(DateTime.UtcNow.Ticks - discord.StartTime.Ticks);
            discord.SendCommandMessage(this, $"The bot has been online since: {discord.StartTime} (UTC).{Environment.NewLine} "+
                                             $"The bot has been online for a total of {difference.TotalDays:0.00} days.");
        }
    }
}
