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
                host.NotifyUsers("Rounds are already running. If you think something terrible has happened, mention @MooCow#9699 and he'll check.");
            }
            else
            {
                RoundData.CancelTokenSource = new CancellationTokenSource();
            }

            if (args.Length >= 1 && args.Length <= 2)
            {
                if (int.TryParse(args[0], out var length))
                {
                    host.Configuration.RoundLength = length;
                }
                else
                {
                    host.NotifyUsers($"I can't grasp the notion of '{args[0]}' being a numerical value representing the number of minutes each round will take.");
                    return;
                }
            }
            if (args.Length == 2)
            {
                if (int.TryParse(args[1], out var length))
                {
                    host.Configuration.BreakLength = length;
                }
                else
                {
                    host.NotifyUsers($"I can't grasp the notion of '{args[1]}' being a numerical value representing the number of minutes each break will take.");
                    return;
                }
            }
            if (args.Length > 2)
            {
                host.NotifyUsers("I don't understand those arguments.");
                return;
            }

            RoundData.Activity = false;
            host.NotifyUsers(
                $"Rounds will run for {host.Configuration.RoundLength} minute(s) with {host.Configuration.BreakLength} minute breaks.");
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
            Sleep((long)startTime.Subtract(DateTime.Now).TotalMilliseconds);

            while (RoundData.CancelTokenSource != null && !RoundData.CancelTokenSource.IsCancellationRequested)
            {
                //Copy these values to be locally scoped for the event that
                //somebody changes something while we're already sleeping.

                var roundLength = host.Configuration.RoundLength;
                //var countdownStart = host.Configuration.CountdownStart;
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
                    if (!Sleep(30000))
                    {
                        return;
                    }
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
                    if (!Sleep(30000))
                    {
                        return;
                    }
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
                    await host.DiscordClient.UpdateStatusAsync(new Game("Nothing :("));
                    host.NotifyUsers("**Ending rounds due to inactivity.**", false);
                    break;
                }

                RoundData.Activity = false;
                roundCounter++;
            }
            if (RoundData.CancelTokenSource != null && !RoundData.CancelTokenSource.IsCancellationRequested)
            {
                await host.DiscordClient.UpdateStatusAsync(new Game("Nothing :("));
                Task.Run((Action)RoundData.End);
            }
        }

        private static DateTime FindNextStartTime(Configuration config)
        {
            var curTime = DateTime.Now;
            curTime = curTime.AddSeconds(-curTime.Second);
            int startMinute = -1;

            for (int i = 0; i < 60; i += config.RoundLength + config.BreakLength)
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

        private static bool Sleep(long milliseconds)
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
                if (RoundData.CancelTokenSource == null || RoundData.CancelTokenSource.IsCancellationRequested)
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
