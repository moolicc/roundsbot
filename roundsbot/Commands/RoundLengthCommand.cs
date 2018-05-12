using System;
using System.Collections.Generic;
using System.Text;

namespace roundsbot.Commands
{
    class RoundLengthCommand : CommandBase
    {
        public override string Name => "roundlength";
        public override string Usage => "roundlength <value>";

        public override string Description => "Sets the amount of time, in minutes, that a round lasts for.";

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

            discord.DiscordConfig.RoundLength = value;
            discord.AddReaction(Emojies.OK);

            if (RoundService.Instance.IsRunning)
            {
                discord.SendCommandMessage(this, "Rounds must be restarted for that change to apply.");
            }
        }
    }
}
