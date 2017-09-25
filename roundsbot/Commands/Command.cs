﻿using System;
using System.Collections.Generic;
using System.Text;
using DSharpPlus.Entities;
using roundsbot.Modules;

namespace roundsbot.Commands
{
    abstract class Command
    {
        public abstract string GetName();
        public abstract string GetDescriptionText();
        public abstract string GetHelpText();
        public abstract void Trigger(DiscordMessage message, CommandHostModule host, params string[] args);

        protected void React(DiscordMessage message, CommandHostModule host, string emoji)
        {
            message.CreateReactionAsync(DiscordEmoji.FromName(host.DiscordClient, Emojies.OK));
        }

        protected async void SendMessage(DiscordChannel channel, string text)
        {
            await channel.SendMessageAsync(text);
        }
    }
}
