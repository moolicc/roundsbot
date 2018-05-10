using System;
using System.Collections.Generic;
using System.Text;

namespace roundsbot
{
    class Configuration
    {
        public ulong ChannelId { get; set; }

        public int RoundLength { get; set; }
        public int BreakLength { get; set; }

        public int CountdownStart { get; set; }
        public int TimeoutCount { get; set; }
        
        public DateTime LaunchTime { get; private set; }

        public Configuration()
        {
            ChannelId = 0;
            RoundLength = 25;
            BreakLength = 5;
            CountdownStart = 180;
            TimeoutCount = 3;
            LaunchTime = DateTime.Now;
        }
    }
}
