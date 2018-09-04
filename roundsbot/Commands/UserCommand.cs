using System;
using System.Collections.Generic;
using System.Text;

namespace roundsbot.Commands
{
    abstract class UserCommand : CommandBase
    {
        public override bool Hidden => false;
    }
}
