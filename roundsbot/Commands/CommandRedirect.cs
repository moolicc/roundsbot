using System;
using System.Collections.Generic;
using System.Text;
using DSharpPlus.Entities;
using roundsbot.Modules;

namespace roundsbot.Commands
{
    class CommandRedirect : Command
    {
        public string TargetCommand { get; private set; }
        public string RedirectCommand { get; private set; }

        public CommandRedirect(string target, string redirect)
        {
            TargetCommand = target;
            RedirectCommand = redirect;
        }

        public override string GetName()
        {
            return RedirectCommand;
        }

        public override string GetDescriptionText()
        {
            return $"See: {TargetCommand}";
        }

        public override string GetHelpText()
        {
            return $"See: {TargetCommand}";
        }

        public override void Trigger(DiscordMessage message, CommandHostModule host, params string[] args)
        {
            host.ExecCommand(TargetCommand, message, args);
        }
    }
}
