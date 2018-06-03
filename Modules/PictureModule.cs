using System.IO;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Commands;
using bot.Services;

namespace bot.Modules
{
    // Modules must be public and inherit from an IModuleBase
    public class PictureModule : ModuleBase<SocketCommandContext>
	{
		// Dependency Injection will fill this value in for us
		public PictureService PictureService { get; set; }
		public DiscordSocketClient Discord { get; set; }

	    [Command("смешнявку")]
		[Alias("meme", "мемас", "картинку")]
		public async Task PictureAsync()
		{
			var stream = await PictureService.GetPictureAsync();
			stream.Seek(0, SeekOrigin.Begin);
			await Context.Channel.SendFileAsync(stream, "meme.png");
		}
    }
}