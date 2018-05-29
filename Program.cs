using System;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace bot
{
    class Program
    {
		public static IConfiguration Configuration { get; set; }
		private DiscordSocketClient client;

        static void Main(string[] args)
        {
			var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
			Configuration = builder.Build();
			new Program().StartAsync().GetAwaiter().GetResult();
        }

	private async Task StartAsync()
	{
		client = new DiscordSocketClient();
		client.Ready += ReadyAsync;
		client.MessageReceived += MessageReceivedAsync;
		await client.LoginAsync(TokenType.Bot, Configuration["token"]);
		await client.StartAsync();
		await Task.Delay(-1);
	}

	private Task ReadyAsync() {
		Console.WriteLine($"{client.CurrentUser} is connected!");
		return Task.CompletedTask;
	}
	
	private async Task MessageReceivedAsync(SocketMessage message)
	{
		if(message.Author.Id == client.CurrentUser.Id)
			return;
		if(message.Content == "!ping")
			await message.Channel.SendMessageAsync("pong!");
	}
    }
}
