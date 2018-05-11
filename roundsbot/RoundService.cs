using System;
using System.Collections.Generic;
using System.Text;

namespace roundsbot
{
    class RoundService
    {
        public static RoundService Instance { get; private set; }

        public Discord Discord { get; private set; }

        public RoundService(Discord discord)
        {
            Instance = this;
            Discord = discord;
        }
        
    }
}
