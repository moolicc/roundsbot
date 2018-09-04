using System;
using System.Collections.Generic;
using System.Text;

namespace roundsbot.Commands
{
    abstract class AdminCommand : CommandBase
    {
        public override bool Hidden => true;
    }
}
