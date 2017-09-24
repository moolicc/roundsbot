using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;

namespace roundsbot.Modules
{
    class ModuleBase
    {
        public virtual void Init(DiscordClient discordClient, Configuration config)
        {
        }
    }
}
