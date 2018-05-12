using System;
using System.Collections.Generic;
using System.Text;

namespace roundsbot
{
    class RoundData
    {
        public DateTime StartedTime { get; private set; }
        public DateTime EndedTime { get; private set; }

        public ulong StartedUser { get; private set; }
        public ulong EndedUser { get; private set; }
        public ulong[] Participants { get; private set; }
        
        public int CompletedRounds { get; private set; }
        public bool TimedOut { get; private set; }

        public int RoundLength { get; private set; }
        public int BreakLength { get; private set; }

        public RoundData()
        {

        }
        
        public void Start(ulong user, int roundLength, int breakLength)
        {
            RoundLength = roundLength;
            BreakLength = breakLength;
            StartedUser = user;
            StartedTime = DateTime.UtcNow;
        }
        
        public void Timeout()
        {
            End(0);
            TimedOut = true;
        }

        public void End(ulong user)
        {
            EndedUser = user;
            EndedTime = DateTime.UtcNow;
        }
        
        public void RoundComplete()
        {
            CompletedRounds++;
        }

    }
}
