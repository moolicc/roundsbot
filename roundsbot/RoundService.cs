using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace roundsbot
{
    class RoundService
    {
        public static RoundService Instance { get; private set; }

        public Discord Discord { get; private set; }
        public bool IsRunning => _roundTask != null && (!_roundTask.IsCompleted || !_roundTask.IsCanceled);
        public bool Activity { get; set; }

        private Task _roundTask;
        private CancellationTokenSource _cancelSource;
        private int _timeoutCounter;
        

        public RoundService(Discord discord)
        {
            Instance = this;
            Discord = discord;
        }

        public void StartRounds()
        {
            if (IsRunning)
            {
                Discord.SendMessage("Rounds are already underway.");
                return;
            }
            
            _cancelSource = new CancellationTokenSource();
            _roundTask = Task.Factory.StartNew(Run, _cancelSource.Token);
        }

        public void StopRounds()
        {
            if (!IsRunning)
            {
                Discord.SendMessage("Rounds are not running right now.");
                return;
            }

            _cancelSource.Cancel();
            _roundTask.Wait(60000);
            _roundTask.Dispose();
            _cancelSource.Dispose();

            Activity = false;
            _timeoutCounter = 0;
        }

        private void Run()
        {
            int roundCounter = 1;
            var startTime = FindNextStartTime(Discord.DiscordConfig.RoundLength, Discord.DiscordConfig.BreakLength);

            Discord.SendMessage($"Round {roundCounter} is starting at XX:{startTime.Minute:00}!");
            SubService.AddReactions(Discord);

            Sleep((long)startTime.Subtract(DateTime.Now).TotalMilliseconds);
            
            var roundLength = Discord.DiscordConfig.RoundLength;
            var breakLength = Discord.DiscordConfig.BreakLength;
            while (!_cancelSource.IsCancellationRequested)
            {
                var endTime = startTime.AddMinutes(roundLength);

                SubService.Instance.SendMessage("{0} Attention: *{1}* {0}");
                Discord.SendMessage($"Round {roundCounter} is starting! {Emojies.TIMER}" +
                                    $"{Environment.NewLine}*Break at XX:{endTime.Minute:00}.*");
                SubService.AddReactions(Discord);

                if (!SleepAndStatus(ref startTime, endTime, "Rounds for {0} more minute(s)"))
                {
                    break;
                }


                var breakEndTime = endTime.AddMinutes(breakLength);
                var foodEmojie = Emojies.GetRandomFoodEmojie();

                SubService.Instance.SendMessage("{0} Attention: *{1}* {0}");
                Discord.SendMessage($"{foodEmojie} Round over! Break until **XX:{breakEndTime.Minute:00}**! {foodEmojie}");
                SubService.AddReactions(Discord);

                if (!SleepAndStatus(ref startTime, breakEndTime, "Break for {0} more minute(s)"))
                {
                    break;
                }

                if (Activity)
                {
                    _timeoutCounter = 0;
                }
                else
                {
                    _timeoutCounter++;
                }

                if (_timeoutCounter >= Discord.DiscordConfig.TimeoutCount)
                {
                    SubService.Instance.SendMessage("{0} Attention: *{1}* {0}");
                    Discord.SendMessage("**Ending rounds due to inactivity.**");
                    break;
                }

                Activity = false;
                roundCounter++;
            }
            
            Discord.SetStatus("Nothing");
            _cancelSource.Cancel();
        }

        private DateTime FindNextStartTime(int roundLength, int breakLength)
        {
            var curTime = DateTime.Now;
            curTime = curTime.AddSeconds(-curTime.Second);
            int startMinute = -1;

            for (int i = 0; i < 60; i += roundLength + breakLength)
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

        private bool SleepAndStatus(ref DateTime start, DateTime end, string format)
        {
            while (true)
            {
                if (start.Hour == end.Hour && start.Minute >= end.Minute)
                {
                    break;
                }

                Discord.SetStatus(string.Format(format, end.Subtract(start).Minutes));
                
                if (!Sleep(30000))
                {
                    return false;
                }
                start = start.AddMinutes(0.5D);
            }

            return true;
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
