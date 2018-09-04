using System;
using System.Collections.Generic;
using System.Text;

namespace roundsbot.Commands.Admin
{
    class HelpCommand : AdminCommand
    {
        public override string Name => "adhelp";

        public override string Usage => "adhelp [command]";

        public override string Description =>
            "When used **without** the 'command' argument, displays available commands. " +
            "When used **with** the 'command' argument, displays specific information about the specified command.";

        public override void Execute(Discord discord, params string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
