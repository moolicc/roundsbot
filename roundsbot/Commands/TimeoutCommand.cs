using System;
using System.Collections.Generic;
using System.Text;

namespace roundsbot.Commands
{
    class TimeoutCommand : CommandBase
    {
        public override string Name => "timeout";
        public override string Usage => "timeout <value>";

        public override string Description => "Sets the number of rounds that must go by, without having any user " +
                                              "messages sent, before canceling any on-going rounds.";

        public override void Execute(Discord discord, params string[] args)
        {
            if (args.Length != 1)
            {
                discord.SendInvalidCommand(this);
                return;
            }

            if (!int.TryParse(args[0], out var value))
            {
                discord.SendInvalidCommand(this);
                return;
            }

            discord.DiscordConfig.TimeoutCount = value;
            discord.AddReaction(Emojies.OK);
        }
    }
}
