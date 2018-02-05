using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Net.WebSocket;
using roundsbot.Modules;

namespace roundsbot
{
    class Program
    {
        private const int RESTART_TIMER_SECONDS = 10;

        private static DiscordClient _discordClient;
        private static Configuration _config;

        static void Main(string[] args)
        {
                RoundData.TimeoutTimer = 0;
                RoundData.Activity = false;
                _config = new Configuration();
            
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
            
            Console.WriteLine($"Press 'q' to close within the next {RESTART_TIMER_SECONDS} seconds to close...");


            bool timedOut = true;
            Task timerTask = Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < RESTART_TIMER_SECONDS; i++)
                {
                    System.Threading.Thread.Sleep(1000);
                    Console.Write($"\rRestarting in {RESTART_TIMER_SECONDS - i} seconds...");
                }
            });
            while (!timerTask.IsCompleted)
            {
                if (Console.KeyAvailable)
                {
                    if (Console.ReadKey(true).Key == ConsoleKey.Q)
                    {
                        timedOut = false;
                        break;
                    }
                }
            }

            if (timerTask.IsCompleted && timedOut)
            {
                timerTask.Dispose();

                //Leave this try/catch here to bury a null-reference exception within discordclient.dispose() that occurs
                //when the client fails to connect.
                try
                {
                    _discordClient.Dispose();
                }
                catch
                {
                }


                RoundData.End();

                var proc = new Process();
                proc.StartInfo.FileName = "restart.bat";
                proc.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                proc.Start();
                proc.Dispose();
            }
            else
            {
                timerTask.Wait();
                timerTask.Dispose();
                //Leave this try/catch here to burry a null-reference exception within discordclient.dispose() that occurs
                //when the client fails to connect.
                try
                {
                    _discordClient.Dispose();
                }
                catch
                {
                }
            }
        }

        private static async Task MainAsync(string[] args)
        {
            var token = System.IO.File.ReadAllText("bottoken.txt");
            Console.WriteLine("Using token {0}", token);

            _discordClient = new DiscordClient(new DiscordConfiguration()
            {
                Token = token,
                TokenType = TokenType.Bot
            });
            _discordClient.SetWebSocketClient<WebSocket4NetCoreClient>();


            bool socketOpen = false;
            _discordClient.SocketClosed += eventArgs =>
            {
                Console.WriteLine("Socket closed! Code {0} ('{1}')", eventArgs.CloseCode, eventArgs.CloseMessage);
                socketOpen = false;
                return Task.Delay(1);
            };

            _discordClient.SocketErrored += eventArgs =>
            {
                Console.WriteLine("Socket error!\r\n{0}", eventArgs.Exception);
                socketOpen = false;
                return Task.Delay(1);
            };

            _discordClient.SocketOpened += () =>
            {
                Console.WriteLine("Socket opened.");
                socketOpen = true;

                //TODO: Load state
                return Task.Delay(1);
            };
            Console.WriteLine("Loading modules...");
            LoadModules();
            

            Task WaitForClose()
            {
                System.Threading.Thread.Sleep(10000);
                while (socketOpen)
                {
                    //Sleep for 5 minutes.
                    //System.Threading.Thread.Sleep(300000);
                    System.Threading.Thread.Sleep(1000);
                }

                //TODO: Save state
                return Task.CompletedTask;
            }

            Console.WriteLine("Connecting to discord...");
            try
            {
                await _discordClient.ConnectAsync();
                Console.WriteLine("Connected!");
                await WaitForClose();
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to connect!");
            }
        }

        private static void LoadModules()
        {
            var commandModule = new CommandHostModule();
            commandModule.Init(_discordClient, _config);
            LoadCommands(commandModule);
        }

        private static void LoadCommands(CommandHostModule hostModule)
        {
            Console.WriteLine("   Loading commands...");
            hostModule.Commands.Add(new Commands.StartCommand());
            hostModule.Commands.Add(new Commands.EndCommand());
            hostModule.Commands.Add(new Commands.CommandRedirect("end", "stop"));
            hostModule.Commands.Add(new Commands.RoundLengthCommand());
            hostModule.Commands.Add(new Commands.BreakLengthCommand());
            hostModule.Commands.Add(new Commands.TimeoutCommand());
            hostModule.Commands.Add(new Commands.SubscribeCommand());
            hostModule.Commands.Add(new Commands.UnsubscribeCommand());
            hostModule.Commands.Add(new Commands.HelpCommand());
            hostModule.Commands.Add(new Commands.UptimeCommand());
            hostModule.Commands.Add(new Commands.AboutCommand());
        }
    }
}
