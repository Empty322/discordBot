using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using bot.Services;
using System;

namespace bot.Modules
{
    // Modules must be public and inherit from an IModuleBase
    public class AudioModule : ModuleBase<SocketCommandContext>
	{
		// Dependency Injection will fill this value in for us
		public DiscordSocketClient Discord { get; set; }
		public AudioService AudioService { get; set; }
        public DownloadService DownloadService { get; set; }

		[Command("join", RunMode = RunMode.Async)]
		public async Task JoinChannel(IVoiceChannel channel = null)
		{
		    channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
		    if (channel == null) 
		    { 
		    	await ReplyAsync("User must be in a voice channel."); 
		    	return; 
			}
			await AudioService.ConnectAsync(channel);
		}

		[Command("disconnect")]
		public async Task DisconnectChannel()
		{
		    if (AudioService.Connected) 
		    { 
		    	await AudioService.DisconnectAsync();
		    	return;
			}
		    await ReplyAsync("Bot must be in a voice channel."); 
		}		

		[Command("play", RunMode = RunMode.Async)]
		public async Task PlayMusic()
        {
			if (AudioService.Connected){
				await AudioService.PlayAsync("song1.mp3");
				return;
			}
			await ReplyAsync("Bot must be in a voice channel.");
		}

        [Command("playyt", RunMode = RunMode.Async)]
        public async Task PlayMusicFromYouTube(string url) 
        {
            string file = await DownloadService.Download(url);
            await AudioService.PlayAsync(file);
			DownloadService.DeleteFile(file);
        }

        [Command("stop")]
		public Task StopMusic(){
			AudioService.Stop();
            return Task.CompletedTask;
		}
	}
}
