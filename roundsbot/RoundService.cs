using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace roundsbot
{
    class RoundService
    {
        public static RoundService Instance { get; private set; }

        public Discord Discord { get; private set; }

        private CancellationTokenSource _cancelSource;

        public RoundService(Discord discord)
        {
            Instance = this;
            Discord = discord;
        }
        
        private DateTime FindNextStartTime()
        {
            var curTime = DateTime.Now;
            curTime = curTime.AddSeconds(-curTime.Second);
            int startMinute = -1;

            for (int i = 0; i < 60; i += Discord.DiscordConfig.RoundLength + Discord.DiscordConfig.BreakLength)
            {
                if (curTime.Minute <= i)
                {
                    startMinute = i;
                    break;
                }
            }
            if (startMinute == -1)
            {
                startMinute = 60;
            }
            return curTime.AddMinutes(startMinute - curTime.Minute);
        }

        //We must implement a sleep function that short-circuits when a cancel is requested.
        private bool Sleep(long milliseconds)
        {
            const int MAX_IDLETIME = 5500;

            int idleTime = int.MaxValue;
            if (milliseconds <= MAX_IDLETIME)
            {
                idleTime = (int)milliseconds;
            }

            int factor = 1;
            while (idleTime > MAX_IDLETIME || idleTime < 0)
            {
                factor++;
                unchecked
                {
                    idleTime = (int)(milliseconds / factor);
                }
            }


            long sleptTime = 0;
            while (sleptTime < milliseconds)
            {
                if (_cancelSource == null || _cancelSource.IsCancellationRequested)
                {
                    return false;
                }
                Thread.Sleep(idleTime);
                sleptTime += idleTime;
            }
            return true;
        }
    }
}
