using System;
using System.Threading.Tasks;
using DSharpPlus;
using roundsbot.Modules;

namespace roundsbot
{
    class Program
    {

        private static DiscordClient _discordClient;
        private static Configuration _config;
        /*
        private static CancellationTokenSource _cancelTokenSource;
        private static CancellationToken _cancelToken;
        private static Task _runTask;


        private static int _timeoutTimer;
        private static bool _activity;

        private static string[] _foodEmojies = new[]
        {
            ":green_apple:",
            ":apple:",
            ":tangerine:",
            ":bread:",
            ":cheese:",
            ":pizza:",
            ":icecream:",
            ":cake:",
            ":cookie:",
            ":coffee:",
        };
        */

        static void Main(string[] args)
        {
            RoundData.TimeoutTimer = 0;
            RoundData.Activity = false;
            _config = new Configuration();


            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();

            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
        }

        private static async Task MainAsync(string[] args)
        {
            _discordClient = new DiscordClient(new DiscordConfiguration()
            {
                Token = System.IO.File.ReadAllText("bottoken.txt"),
                TokenType = TokenType.Bot
            });
            //_discordClient.MessageCreated += MessageCreated;
            LoadModules();


            await _discordClient.ConnectAsync();
            await Task.Delay(-1);
        }

        private static void LoadModules()
        {
            var commandModule = new CommandHostModule();
            commandModule.Init(_discordClient, _config);
            LoadCommands(commandModule);
        }

        private static void LoadCommands(CommandHostModule hostModule)
        {
            hostModule.Commands.Add(new Commands.StartCommand());
            hostModule.Commands.Add(new Commands.EndCommand());
            hostModule.Commands.Add(new Commands.RoundLengthCommand());
            hostModule.Commands.Add(new Commands.BreakLengthCommand());
            hostModule.Commands.Add(new Commands.TimeoutCommand());
            hostModule.Commands.Add(new Commands.SubscribeCommand());
            hostModule.Commands.Add(new Commands.UnsubscribeCommand());
            hostModule.Commands.Add(new Commands.HelpCommand());
        }

        /*
        private static async Task MessageCreated(DSharpPlus.EventArgs.MessageCreateEventArgs e)
        {
            string message = e.Message.Content.Replace(_discordClient.CurrentUser.Mention, "").Trim();
            var commandData = ParseInput(message);
            if (string.IsNullOrWhiteSpace(commandData.name))
            {
                //TODO: Tell the user that either my code broke or they're on the lower end of the IQ spectrum.
                return;
            }
            if (commandData.name == "start")
            {
                if (_runTask != null)
                {
                    //TODO: Tell the user that rounds are underway.
                }
                else
                {
                    _cancelTokenSource = new CancellationTokenSource();
                    _cancelToken = _cancelTokenSource.Token;
                    _runTask = StartCommand(commandData.args);
                }
            }
            if (commandData.name == "end")
            {
                if (_cancelTokenSource != null)
                {
                    EndCommand(commandData.args);
                }
            }
            if (commandData.name == "roundlength")
            {
                if (commandData.args.Length > 0)
                {
                    if (int.TryParse(commandData.args[0], out var length))
                    {
                        _config.RoundLength = length;
                        if (_runTask != null)
                        {
                            NotifyUsers("Round length will reset once the next round starts.", false);
                        }
                    }
                    else
                    {
                        NotifyUsers("Invalid value. *(Hint: A valid number might work)*", false);
                    }
                }
                else
                {
                    NotifyUsers($"The current round length is {_config.RoundLength} minutes.", false);
                }
            }

            if (commandData.name == "breaklength")
            {
                if (commandData.args.Length > 0)
                {
                    if (int.TryParse(commandData.args[0], out var length))
                    {
                        _config.BreakLength = length;
                        if (_runTask != null)
                        {
                            NotifyUsers("Break length will reset once the next round starts.", false);
                        }
                    }
                    else
                    {
                        NotifyUsers("Invalid value. *(Hint: A valid number might work)*", false);
                    }
                }
                else
                {
                    NotifyUsers($"The current break length is {_config.BreakLength} minutes.", false);
                }
            }

            if (commandData.name == "timeout")
            {
                if (commandData.args.Length > 0)
                {
                    if (int.TryParse(commandData.args[0], out var count))
                    {
                        _config.TimeoutCount = count;
                    }
                    else
                    {
                        NotifyUsers("Invalid value. *(Hint: A valid number might work)*", false);
                    }
                }
                else
                {
                    NotifyUsers($"The current round timeout is {_config.TimeoutCount} rounds.", false);
                }
            }

            if (commandData.name == "subscribe")
            {
                if (_config.SubscribedUsers.Contains(e.Message.Author.Id))
                {
                    NotifyUsers(
                        $"Excuse me sir/ma'am, but my records show that you have already subscribed. {EMOJIE_SUBSCRIBE}{Environment.NewLine}*(Hint: Try `unsubscribe` to unsubscribe)*",
                        false);
                }
                else
                {
                    _config.SubscribedUsers.Add(e.Message.Author.Id);
                }
            }

            if (commandData.name == "unsubscribe")
            {
                if (!_config.SubscribedUsers.Contains(e.Message.Author.Id))
                {
                    NotifyUsers($"Excuse me sir/ma'am, but my records show that you are already not subscribed. {EMOJIE_SUBSCRIBE}{Environment.NewLine}*(Hint: Try `subscribe` to subscribe)*", false);
                }
                else
                {
                    _config.SubscribedUsers.Remove(e.Message.Author.Id);
                }
            }
        }

        private static Task StartCommand(params string[] args)
        {
            return Task.Run(() =>
            {
                RunRound();
            }, _cancelToken);
        }

        private static void EndCommand(params string[] args)
        {
            _cancelTokenSource.Cancel();
            _runTask.Wait(5000);
            _cancelTokenSource.Token.ThrowIfCancellationRequested();
            _runTask.Dispose();
            _cancelTokenSource.Dispose();

            _runTask = null;
            _cancelTokenSource = null;

            _activity = false;
            _timeoutTimer = 0;
        }

        private static void RunRound()
        {
            int roundCounter = 1;

            var startTime = FindNextStartTime();
            NotifyUsers($"Round {roundCounter} is starting at XX:{startTime.Minute:00}!", true);
            Thread.Sleep(startTime.Subtract(DateTime.Now));

            while (!_cancelToken.IsCancellationRequested)
            {
                //Copy these values to be locally scoped for the event that
                //somebody changes something while we're already sleeping.

                var roundLength = _config.RoundLength;
                var countdownStart = _config.CountdownStart;
                var breakLength = _config.BreakLength;

                NotifyUsers($"{EMOJIE_TIMER} Round {roundCounter} is starting! {EMOJIE_TIMER}", true);

                var endTime = startTime.AddMinutes(roundLength);
                Thread.Sleep(endTime.Subtract(startTime).Subtract(TimeSpan.FromSeconds(countdownStart)));

                Countdown(_config.CountdownStart);
                Thread.Sleep(_config.CountdownStart * 1000);

                var breakEndTime = endTime.AddMinutes(breakLength);

                var foodEmojie = _foodEmojies[new Random().Next(_foodEmojies.Length)];

                NotifyUsers($"{foodEmojie} Round over! Break until XX:{breakEndTime.Minute:00}! {foodEmojie}", true);

                Thread.Sleep(breakEndTime.Subtract(endTime));
                if (_activity)
                {
                    _timeoutTimer = 0;
                }
                else
                {
                    _timeoutTimer++;
                }

                if (_timeoutTimer >= _config.TimeoutCount)
                {
                    NotifyUsers("Ending rounds due to inactivity.", false);
                    break;
                }

                _activity = false;
                roundCounter++;
            }
            if (!_cancelToken.IsCancellationRequested)
            {
                EndCommand("");
            }
        }

        private static async void NotifyUsers(string message, bool mention)
        {
            if (mention)
            {
                StringBuilder messageBuilder = new StringBuilder();

                messageBuilder.Append("Attention ");
                for (var i = 0; i < _config.SubscribedUsers.Count; i++)
                {
                    var userId = _config.SubscribedUsers[i];
                    var user = await _discordClient.GetUserAsync(userId);
                    if (user.Presence.Status == UserStatus.Online || user.Presence.Status == UserStatus.Idle)
                    {
                        if (i == _config.SubscribedUsers.Count - 1)
                        {
                            messageBuilder.Append(" and ");
                        }
                        else if (i != 0)
                        {
                            messageBuilder.Append(", ");
                        }
                        messageBuilder.Append(user.Mention);
                    }
                }

                messageBuilder.Append(": ").Append(message);
                await _discordClient.SendMessageAsync(await _discordClient.GetChannelAsync(_config.ChannelId), messageBuilder.ToString());
            }
            else
            {
                await _discordClient.SendMessageAsync(await _discordClient.GetChannelAsync(_config.ChannelId), message);
            }
        }

        private static void Countdown(int time)
        {
            
        }

        private static DateTime FindNextStartTime()
        {
            var curTime = DateTime.Now;
            int startMinute = 0;

            for (int i = 0; i < 60; i += _config.RoundLength + _config.BreakLength)
            {
                if (curTime.Minute < i)
                {
                    startMinute = i;
                    break;
                }
            }
            return curTime.AddMinutes(startMinute - curTime.Minute);
        }

        private static (string name, string[] args) ParseInput(string input)
        {
            List<string> parameters = new List<string>();
            StringBuilder curParam = new StringBuilder();
            bool quote = false;
            for (int i = 0; i < input.Length; i++)
            {
                char curChar = input[i];
                char nextChar = '\0';
                if (i + 1 < input.Length)
                {
                    nextChar = input[i + 1];
                }

                if (curChar == '\\' && nextChar == '\"')
                {
                    curParam.Append('\"');
                    i++;
                    continue;
                }
                if (curChar == '\"')
                {
                    quote = !quote;
                }
                if (curChar == ' ' && !quote)
                {
                    parameters.Add(curParam.ToString());
                    curParam.Clear();
                    continue;
                }
                
                curParam.Append(curChar);


                if (nextChar == '\0')
                {
                    parameters.Add(curParam.ToString());
                    curParam.Clear();
                }
            }

            string name = "";
            if (parameters.Count > 0)
            {
                name = parameters[0];
                parameters.RemoveAt(0);
            }

            return (name, parameters.ToArray());
        }
        */
    }
}
