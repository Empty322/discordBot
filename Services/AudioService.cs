using System.Threading.Tasks;
using System.Diagnostics;
using Discord;
using Discord.Audio;
using System.Threading;

namespace bot.Services
{
    public class AudioService
    {
    	private IAudioClient audioClient;
		private int bufferSize = 1920;
		private CancellationTokenSource cts;

    	public bool Connected { get; private set; }
		public bool Paused { get; private set; }
		public AudioService() {
			Connected = false;
			audioClient = null;
			Paused = true;
			cts = new CancellationTokenSource();
		}

    	private Process CreateStream(string path)
		{
		    var ffmpeg = new ProcessStartInfo
		    {
		        FileName = "ffmpeg",
		        Arguments = $"-i {path} -ac 2 -f s16le -ar 48000 pipe:1",
		        UseShellExecute = false,
		        RedirectStandardOutput = true,
		    };
		    return Process.Start(ffmpeg);
		}

		public async Task ConnectAsync(IVoiceChannel channel)
		{
			audioClient = await channel.ConnectAsync();
			if (audioClient != null)
				Connected = true;
		}

		public async Task DisconnectAsync() {
			if (Connected)
			{
				Stop();
				Connected = false;
				await audioClient.StopAsync();
			}
		}

		public async Task PlayAsync(string path)
		{
			if (!Paused) {
				Stop();
			}
            await Task.Delay(500);
			cts = new CancellationTokenSource();
		    var ffmpeg = CreateStream(path);
		    var output = ffmpeg.StandardOutput.BaseStream;
		    var discord = audioClient.CreatePCMStream(AudioApplication.Mixed);
		    await output.CopyToAsync(discord, bufferSize, cts.Token);
			ffmpeg.Close();
		    await discord.FlushAsync();
		}

		public void Stop() 
		{
			if (Connected)
			{
				if (cts.Token.CanBeCanceled)
					cts.Cancel();
				Paused = true;
			}
		}
    }
}