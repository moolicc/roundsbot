using System;
using System.Collections.Generic;
using System.Text;
using DSharpPlus.Entities;
using roundsbot.Modules;

namespace roundsbot.Commands
{
    class AboutCommand : Command
    {
        public override string GetName()
        {
            return "about";
        }

        public override string GetDescriptionText()
        {
            return "Displays credits/about information.";
        }

        public override string GetHelpText()
        {
            return "You win! :trophy:";
        }

        public override void Trigger(DiscordMessage message, CommandHostModule host, params string[] args)
        {
            //Bot icon source: https://github.com/iconic/open-iconic
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder();
            builder.WithTitle("About");
            //builder.WithDescription("Version " + host.Configuration.Version);
            builder.WithTimestamp(host.Configuration.ReleaseDate);

            builder.AddField("Version", host.Configuration.Version.ToString(), true);
            foreach (var contributor in host.Configuration.Authors)
            {
                builder.AddField("Contributor", contributor, true);
            }
            builder.WithFooter("Icon source: https://github.com/iconic/open-iconic",
                "https://github.com/Icecream-Burglar/roundsbot/blob/master/roundsbot/timer-8x.png");
            SendEmbed(message.Channel, builder.Build());
        }


        private async void SendEmbed(DiscordChannel channel, DiscordEmbed embed)
        {
            await channel.SendMessageAsync(embed: embed);
        }
    }
}
