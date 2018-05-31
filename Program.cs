using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using bot.Services;

namespace bot
{
	public enum AnswerResult
	{
		Guessed = 0,
		WrongAnswer = 1,
		CorrectAnswer = 2
	}

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
			var services = ConfigureServices();
			
			client = services.GetRequiredService<DiscordSocketClient>();
			client.Ready += ReadyAsync;
			client.Log += LogAsync;
			await client.LoginAsync(TokenType.Bot, Configuration["token"]);
			await client.StartAsync();
			await services.GetRequiredService<CommandHandlingService>().InitializeAsync();
			await Task.Delay(-1);
		}

		private Task LogAsync(LogMessage log)
		{
			Console.WriteLine(log.ToString());
			return Task.CompletedTask;
		}

		private Task ReadyAsync() {
			Console.WriteLine($"{client.CurrentUser} is connected!");
			return Task.CompletedTask;
		}
		
		private IServiceProvider ConfigureServices()
		{
			return new ServiceCollection()
				.AddSingleton<DiscordSocketClient>()
				.AddSingleton<CommandService>()
				.AddSingleton<CommandHandlingService>()
				.AddSingleton<HttpClient>()
				.AddSingleton<ZagadkaService>()
				.AddSingleton<PictureService>()
				.AddSingleton<ReputationService>()
				.BuildServiceProvider();
		}
    }
}
