using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace roundsbot
{
    static class RoundData
    {
        public static bool Activity { get; set; }
        public static int TimeoutTimer { get; set; }
        public static CancellationTokenSource CancelTokenSource;
        public static Task RunTask;


        public static void End()
        {
            if (CancelTokenSource == null)
            {
                return;
            }
            CancelTokenSource.Cancel();
            RunTask.Wait(6000);
            RunTask.Dispose();
            CancelTokenSource.Dispose();

            RunTask = null;
            CancelTokenSource = null;

            Activity = false;
            TimeoutTimer = 0;
        }
    }
}
