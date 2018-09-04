using System.Text;
using DSharpPlus.Entities;

namespace roundsbot.Commands.User
{
    class HelpCommand : UserCommand
    {
        public override string Name => "help";
        public override string Usage => "help [command]";

        public override string Description =>
            "When used **without** the 'command' argument, displays available commands. " +
            "When used **with** the 'command' argument, displays specific information about the specified command.";

        public override void Execute(Discord discord, params string[] parameters)
        {
            if (parameters.Length == 0)
            {
                DiscordEmbedBuilder builder = new DiscordEmbedBuilder();
                builder.Title = "Available Commands";
                builder.Description = "Note: Chain commands together with a semicolon.";

                foreach (var hostCommand in discord.Commands)
                {
                    builder.AddField(hostCommand.Value.Usage, hostCommand.Value.Description);
                }
                
                builder.WithFooter("RoundsBot by MooCow • Version: 1.1.0.0 • September 4, 2018");
                discord.SendMessage(builder.Build());
            }
            else if (parameters.Length == 1)
            {
                StringBuilder builder = new StringBuilder();
                if (!discord.Commands.TryGetValue(parameters[0], out var command))
                {
                    discord.SendCommandMessage(this, "This is awkward. It seems I have **no idea** what you're asking for help with :no_mouth:");
                    return;
                }
                builder.Append("**").Append(command.Name).Append("**");
                builder.AppendLine();
                builder.Append(command.Description);
                discord.SendMessage(builder.ToString());
            }
            else
            {
                discord.SendInvalidCommand(this);
            }
        }
    }
}
