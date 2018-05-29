using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace bot.Services
{
    class CommandHandlingService
    {
		private readonly CommandService commands;
		private readonly DiscordSocketClient discord;
		private readonly IServiceProvider services;

		public CommandHandlingService(IServiceProvider services)
		{
			commands = services.GetRequiredService<CommandService>();
			discord = services.GetRequiredService<DiscordSocketClient>();
			this.services = services;
			discord.MessageReceived += MessageReceivedAsync;
		}

		public async Task InitializeAsync()
		{
			await commands.AddModulesAsync(Assembly.GetEntryAssembly());
		}

		public async Task MessageReceivedAsync(SocketMessage rawMessage)
		{
			Console.WriteLine(rawMessage.ToString());
			// Ignore system messages, or messages from other bots
			if(!(rawMessage is SocketUserMessage message)) return;
			Console.WriteLine("SocketUserMessage");
			if(message.Source != MessageSource.User) return;
			Console.WriteLine("Source");			

			// This value holds the offset where the prefix ends
			var argPos = 0;
			if(!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(discord.CurrentUser, ref argPos))) return;
			Console.WriteLine("Prefix");

			var context = new SocketCommandContext(discord, message);
			var result = await commands.ExecuteAsync(context, argPos, services);

			if(result.Error.HasValue && result.Error.Value != CommandError.UnknownCommand) // it's bad practice to send 'unknown command' errors
				await context.Channel.SendMessageAsync(result.ToString());
		}
	}
}
