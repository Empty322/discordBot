using System;
﻿using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using bot.Services;

namespace bot.Modules
{
	// Modules must be public and inherit from an IModuleBase
	public class PublicModule : ModuleBase<SocketCommandContext>
	{
		// Dependency Injection will fill this value in for us
		public PictureService PictureService { get; set; }
		public ZagadkaService ZagadkaService { get; set; }
		public ReputationService ReputationService { get; set; }

		[Command("загадку")]
		[Alias("загадочку мне", "загадочка", "загадочку", "загадка", "загадку мне")]
		public async Task Zagadka()
		{
			await ReplyAsync(ZagadkaService.GetZagadku());
		}

		[Command("ответ")]
        [Alias("изи", "ответик")]
		public async Task Answer(params string[] answerWords)
		{
			IUser user = Context.User;
			string ans = "";
			for (int i = 0; answerWords.Length > i; i++)
				ans += answerWords[i] + ' ';
			ans = ans.Substring(0, ans.Length - 1);
			Console.WriteLine("'" + ans + "'");
			switch (ZagadkaService.CheckAnswer(ans))
			{
				case AnswerResult.Guessed:
					break;
				case AnswerResult.WrongAnswer:
					ReputationService.ChangeRep(Context.User, -3);
					await ReplyAsync($"Neverno, {user.Username}.");
					break;
				case AnswerResult.CorrectAnswer:
					ReputationService.ChangeRep(Context.User, 10);
					await ReplyAsync($"Верно, {user.Username}.");
					break;
			}
		}

        [Command("смешнявку")]
        [Alias("meme", "мемас", "картинку")]
        public async Task PictureAsync()
        {
            var stream = await PictureService.GetPictureAsync();
            stream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(stream, "meme.png");
        }

		[Command("rank")]
		[Alias("ранг", "rep")]
		public async Task Rank(params string[] users)
		{
			string user = "";
			if (users.Length == 0)
			{
				user = Context.User.ToString();
			}
			else
			{
				user = users[0];
			}
			int rep = ReputationService.GetRepByUser(user);
			if(rep == -100)
			{
				await ReplyAsync("Izvini, takoy so mnoy eshe ne igralsya");
			}
			else
			{
				await ReplyAsync($"Ранг пользователя {user} cocтавляет {rep}.");
			}
		}

		[Command("ping")]
		[Alias("pong", "hello")]
		public Task PingAsync()
			=> ReplyAsync("pong!");

		[Command("cat")]
		public async Task CatAsync()
		{
			// Get a stream containing an image of a cat
			var stream = await PictureService.GetCatPictureAsync();
			// Streams must be seeked to their beginning before being uploaded!
			stream.Seek(0, SeekOrigin.Begin);
			await Context.Channel.SendFileAsync(stream, "cat.png");
		}

		// Get info on a user, or the user who invoked the command if one is not specified
		[Command("userinfo")]
		public async Task UserInfoAsync(IUser user = null)
		{
			user = user ?? Context.User;

			await ReplyAsync(user.ToString());
		}

		// Ban a user
		[Command("ban")]
		[RequireContext(ContextType.Guild)]
		// make sure the user invoking the command can ban
		[RequireUserPermission(GuildPermission.BanMembers)]
		// make sure the bot itself can ban
		[RequireBotPermission(GuildPermission.BanMembers)]
		public async Task BanUserAsync(IGuildUser user, [Remainder] string reason = null)
		{
			await user.Guild.AddBanAsync(user, reason: reason);
			await ReplyAsync("ok!");
		}

		// [Remainder] takes the rest of the command's arguments as one argument, rather than splitting every space
		[Command("echo")]
		public Task EchoAsync([Remainder] string text)
			// Insert a ZWSP before the text to prevent triggering other bots!
			=> ReplyAsync('\u200B' + text);

		// 'params' will parse space-separated elements into a list
		[Command("list")]
		public Task ListAsync(params string[] objects)
			=> ReplyAsync("You listed: " + string.Join("; ", objects));
	}
}
