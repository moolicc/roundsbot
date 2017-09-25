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

        public Version Version { get; private set; }
        public string[] Authors { get; private set; }
        public DateTime ReleaseDate { get; private set; }

        public Configuration()
        {
            ChannelId = 0;
            RoundLength = 25;
            BreakLength = 5;
            CountdownStart = 180;
            TimeoutCount = 3;
            SubscribedUsers = new List<ulong>();
            LaunchTime = DateTime.Now;

            LoadAbout();
        }

        private void LoadAbout()
        {
            var data = System.IO.File.ReadAllLines("about.txt");
            Version = new Version(data[0]);
            Authors = new string[data.Length - 2];
            ReleaseDate = new DateTime(2017, 9, 25, 23, 25, 0, DateTimeKind.Utc);
        }
    }
}
