using System;
using System.Collections.Generic;
using System.Text;

namespace roundsbot
{
    static class Emojies
    {
        public const string TIMER = ":stopwatch:";
        public const string SUBSCRIBE = ":notepad_spiral:";
        public const string WARNING = ":warning:";
        public const string OK = ":ok_hand:";
        public const string PENSIVE = ":pensive:";
        public const string NO_MOUTH = ":no_mouth:";
        public const string CAKE = ":cake:";
        public const string FORK_KNIFE = ":fork_and_knife:";

        public static string[] FoodEmojies = new[]
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
            ":doughnut:",
        };

        private static Random _random;

        public static string GetRandomFoodEmojie()
        {
            if (_random == null)
            {
                _random = new Random();
            }
            return FoodEmojies[_random.Next(FoodEmojies.Length)];
        }
    }
}
