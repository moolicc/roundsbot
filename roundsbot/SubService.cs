using System;
using System.Collections.Generic;
using System.Text;
using DSharpPlus.Entities;

namespace roundsbot
{
    class SubService
    {
        public static SubService Instance { get; set; }

        public Discord Discord { get; private set; }
        private List<DiscordUser> _users;

        public static void AddReactions(Discord discord)
        {
            discord.AddBotReaction(Emojies.SUB);
            discord.AddBotReaction(Emojies.UNSUB);
        }

        public SubService(Discord discord)
        {
            Instance = this;
            _users = new List<DiscordUser>();
            Discord = discord;
            discord.OnReactionAdded += ReactionAdded;
            discord.OnReactionRemoved += ReactionRemoved;
        }

        //Format:
        //{0} - emoji
        //{1} - users
        public void SendMessage(string format)
        {
            var userNames = "";
            lock (_users)
            {
                if (_users.Count == 0)
                {
                    return;
                }
                for (int i = 0; i < _users.Count; i++)
                {
                    var user = _users[i];
                    if (i == 0)
                    {
                        userNames = user.Mention;
                    }
                    else if (i == _users.Count - 1)
                    {
                        userNames += ", and " + user.Mention;
                    }
                    else
                    {
                        userNames += ", " + user.Mention;
                    }
                }
            }
            var message = string.Format(format, Emojies.WARNING, userNames);
            Discord.SendMessage(message);
        }

        public void AddUserToSubs(DiscordUser user)
        {
            lock (_users)
            {
                if (!_users.Contains(user))
                {
                    _users.Add(user);
                }
            }
        }

        public void RemoveUserFromSubs(DiscordUser user)
        {
            lock (_users)
            {
                if (_users.Contains(user))
                {
                    _users.Remove(user);
                }
            }
        }

        private void ReactionAdded(DSharpPlus.EventArgs.MessageReactionAddEventArgs obj)
        {
            if (obj.Emoji.GetDiscordName() == Emojies.SUB)
            {
                AddUserToSubs(obj.User);
            }
            else if (obj.Emoji.GetDiscordName() == Emojies.UNSUB)
            {
                RemoveUserFromSubs(obj.User);
            }
        }
        
        private void ReactionRemoved(DSharpPlus.EventArgs.MessageReactionRemoveEventArgs obj)
        {
            if (obj.Emoji.GetDiscordName() == Emojies.SUB)
            {
                RemoveUserFromSubs(obj.User);
            }
            else if (obj.Emoji.GetDiscordName() == Emojies.UNSUB)
            {
                AddUserToSubs(obj.User);
            }
        }
    }
}
