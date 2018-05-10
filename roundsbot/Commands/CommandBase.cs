using System;
using System.Collections.Generic;
using System.Text;

namespace roundsbot.Commands
{
    abstract class CommandBase
    {
        public abstract void Execute(Discord discord, params string[] args);
    }
}
