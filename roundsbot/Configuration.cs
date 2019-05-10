using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace roundsbot
{
    class Configuration
    {
        public string Token { get; set; }
        public ulong ChannelId { get; set; }
        
        public int RoundLength { get; set; }
        public int BreakLength { get; set; }
        public int TimeoutCount { get; set; }

        public string VersionString { get; set; }

        public Configuration()
        {
            Token = "";
            ChannelId = 0;

            RoundLength = 25;
            BreakLength = 5;
            TimeoutCount = 5;

            VersionString = "Roundsbot by MooCow • Version 1.1.0.2 • May 10, 2019";
        }
    }
}
