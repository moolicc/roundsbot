using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using roundsbot.Modules;

namespace roundsbot.Commands
{
    class StartCommand : Command 
    {
        public override string GetName()
        {
            return "start";
        }

        public override string GetDescriptionText()
        {
            return "Starts running rounds.";
        }

        public override string GetHelpText()
        {
            return "Starts running rounds using the values represented by the commands \"roundlength\" and \"breaklength\" to specify the amount of time (in minutes) rounds/breaks run for.";
        }

        public override void Trigger(DiscordMessage message, CommandHostModule host, params string[] args)
        {
            if (RoundData.RunTask != null)
            {
                //TODO: Tell the user that rounds are underway.
            }
            else
            {
                RoundData.CancelTokenSource = new CancellationTokenSource();
            }

            RoundData.RunTask = Task.Run(() =>
            {
                RunRound(host);
            }, RoundData.CancelTokenSource.Token);
        }


        private async Task RunRound(CommandHostModule host)
        {
            int roundCounter = 1;

            var startTime = FindNextStartTime(host.Configuration);
            host.NotifyUsers($"Round {roundCounter} is starting at XX:{startTime.Minute:00}!", true);
            Thread.Sleep(startTime.Subtract(DateTime.Now));

            while (!RoundData.CancelTokenSource.IsCancellationRequested)
            {
                //Copy these values to be locally scoped for the event that
                //somebody changes something while we're already sleeping.

                var roundLength = host.Configuration.RoundLength;
                var countdownStart = host.Configuration.CountdownStart;
                var breakLength = host.Configuration.BreakLength;

                var endTime = startTime.AddMinutes(roundLength);
                host.NotifyUsers($"Round {roundCounter} is starting! {Emojies.TIMER}{Environment.NewLine}*Break at XX:{endTime.Minute:00}.*", true);
                
                while (true)
                {
                    if (startTime.Hour == endTime.Hour && startTime.Minute >= endTime.Minute)
                    {
                        break;
                    }
                    await host.DiscordClient.UpdateStatusAsync(
                        new Game($"Rounds for {endTime.Subtract(startTime).Minutes} more minute(s)"));
                    Thread.Sleep(30000);
                    startTime = startTime.AddMinutes(0.5D);
                }
                //Thread.Sleep(endTime.Subtract(startTime).Subtract(TimeSpan.FromSeconds(countdownStart)));

                //Countdown(host.Configuration.CountdownStart);
                //Thread.Sleep(host.Configuration.CountdownStart * 1000);


                var foodEmojie = Emojies.FoodEmojies[new Random().Next(Emojies.FoodEmojies.Length)];

                var breakEndTime = endTime.AddMinutes(breakLength);
                host.NotifyUsers($"{foodEmojie} Round over! Break until **XX:{breakEndTime.Minute:00}**! {foodEmojie}", true);

                //Thread.Sleep(breakEndTime.Subtract(endTime));
                while (true)
                {
                    if (startTime.Hour == breakEndTime.Hour && startTime.Minute >= breakEndTime.Minute)
                    {
                        break;
                    }
                    await host.DiscordClient.UpdateStatusAsync(
                        new Game($"Break for {breakEndTime.Subtract(startTime).Minutes} more minute(s)"));
                    Thread.Sleep(30000);
                    startTime = startTime.AddMinutes(0.5D);
                }

                if (RoundData.Activity)
                {
                    RoundData.TimeoutTimer = 0;
                }
                else
                {
                    RoundData.TimeoutTimer++;
                }

                if (RoundData.TimeoutTimer >= host.Configuration.TimeoutCount)
                {
                    host.NotifyUsers("**Ending rounds due to inactivity.**", false);
                    break;
                }

                RoundData.Activity = false;
                roundCounter++;
            }
            if (!RoundData.CancelTokenSource.IsCancellationRequested)
            {
                RoundData.End();
            }
        }
        
        private static void Countdown(int time)
        {

        }

        private static DateTime FindNextStartTime(Configuration config)
        {
            var curTime = DateTime.Now;
            int startMinute = 0;

            for (int i = 0; i < 60; i += config.RoundLength + config.BreakLength)
            {
                if (curTime.Minute < i)
                {
                    startMinute = i;
                    break;
                }
            }
            return curTime.AddMinutes(startMinute - curTime.Minute);
        }
    }
}
