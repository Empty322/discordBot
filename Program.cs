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
			await client.LoginAsync(TokenType.Bot, Configuration["token"]);
			await client.StartAsync();
			Console.WriteLine("Logged in as");
			//Console.WriteLine(client.CurrentUser.Username);
			//Console.WriteLine(client.ShardId);
			await Task.Delay(-1);
			Console.WriteLine("Stopped");
		}
    }
}
