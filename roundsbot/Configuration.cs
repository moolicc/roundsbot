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
        public List<ulong> SubscribedUsers { get; private set; }
        public DateTime LaunchTime { get; private set; }

        public Configuration()
        {
            LaunchTime = DateTime.Now;
            ChannelId = 0;
            RoundLength = 25;
            BreakLength = 5;
            CountdownStart = 180;
            TimeoutCount = 3;
            SubscribedUsers = new List<ulong>();
        }
    }
}
