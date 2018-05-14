using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using ScathachBot.Core.UserAccounts;
using Discord.WebSocket;

namespace ScathachBot.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
        [Command("warn")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task WarnUser(IGuildUser user)
        {
            var userAccount = UserAccounts.GetAccount((SocketUser)user);
            userAccount.NumberOfWarnings++;
            UserAccounts.SaveAccounts();

            if (userAccount.NumberOfWarnings >= 3)
            {
                await Context.Channel.SendMessageAsync(user + " has been banned!");
                await user.Guild.AddBanAsync(user, 7);
            }
            else if (userAccount.NumberOfWarnings == 1)
            {
                await Context.Channel.SendMessageAsync("You currently have 1 warning!");
            }
            else if (userAccount.NumberOfWarnings == 2)
            {
                await Context.Channel.SendMessageAsync("You currently have 2 warnings!");
            }

        }

        [Command("addXP")]
        [RequireOwner]
        public async Task AddXP(uint xp)
        {
            var account = UserAccounts.GetAccount(Context.User);
            account.XP += xp;
            UserAccounts.SaveAccounts();
            await Context.Channel.SendMessageAsync($"You gained {xp} XP.");
        }

        [Command("stats")]
        public async Task Stats([Remainder]string arg = "")
        {
            SocketUser target = null;
            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();
            target = mentionedUser ?? Context.User;

            var account = UserAccounts.GetAccount(target);
            await Context.Channel.SendMessageAsync($"{target.Username} have {account.XP} XP and {account.Points} Points.");
        }

        [Command("embed")]
        public async Task Echo([Remainder]string message)
        {
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithTitle("Message by " + Context.User.Username);
            Embed.WithDescription(message);
            Embed.WithColor(new Color(0, 0, 255));

            await Context.Channel.SendMessageAsync("", false, Embed.Build());

        }

        [Command("pick")]
        public async Task PickOne([Remainder]string message)
        {
            string[] options = message.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            Random r = new Random();
            string selection = options[r.Next(0, options.Length)];

            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithTitle("Choice for " + Context.User.Username);
            Embed.WithDescription(selection);
            Embed.WithColor(new Color(255, 0, 255));
            Embed.WithThumbnailUrl("https://k50.kn3.net/taringa/4/9/F/F/5/8/Eze_Hina23/97A.jpg");
            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }
        [Command("kick")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task KickUser(IGuildUser user, string reason = "")
        {
            await user.KickAsync(reason);
            await Context.Channel.SendMessageAsync(user + " has been kicked!");
        }
        [Command("ban")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task BanUser(IGuildUser user, string reason = "")
        {
            await user.Guild.AddBanAsync(user, 7, reason);
            await Context.Channel.SendMessageAsync(user + " has now been banned!");
        }
        [Command("unban")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task Unban([Remainder]string user)
        {
            var bans = await Context.Guild.GetBansAsync();

            var theUser = bans.FirstOrDefault(x => x.User.ToString().ToLowerInvariant() == user.ToLowerInvariant());

            await Context.Guild.RemoveBanAsync(theUser.User).ConfigureAwait(false);
        }
        [Command("data")]
        public async Task GetData()
        {
            await Context.Channel.SendMessageAsync("Data Has " + DataStorage.GetPairsCount() + " pairs.");
            DataStorage.AddPairToStorage("Count" + DataStorage.GetPairsCount(), "TheCount" + DataStorage.GetPairsCount());
        }
    }
}