using Discord;
using Discord.WebSocket;
using Discord.Commands;
using bot.Services;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Threading;
using System.Diagnostics;

namespace bot.Services
{
    public class DownloadService 
    {
        private readonly string downloadPath;

        public DownloadService() {
            downloadPath = Directory.GetCurrentDirectory() + "\\music";
        }

        public async Task<string> Download(string url) {
            if (url.ToLower().Contains("youtube.com"))
                return await DownloadFromYouTube(url);
            else
                throw new Exception("Url not supported");
        }

        private async Task<string> DownloadFromYouTube(string url) {
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
            new Thread(() => {
                string file;
                int count = 0;
                do {
                    file = downloadPath + "\\song" + ++count + ".mp3";
                } while (File.Exists(file));
                Process youtubedl;
                ProcessStartInfo youtubedlDownload = new ProcessStartInfo() {
                    FileName = "youtube-dl",
                    Arguments = $"-x --audio-format mp3 --prefer-ffmpeg -o \"{file.Replace("mp3", "%(ext)s").Replace(" ", "")}\" {url}",
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    /*UseShellExecute = false*/     //Linux?
                };
                youtubedl = Process.Start(youtubedlDownload);
                youtubedl.WaitForExit();

                if (File.Exists(file))
                    tcs.SetResult(file);
                else
                    tcs.SetResult(null);
            }).Start();

            string result = await tcs.Task;
            if (result == null)
                throw new Exception("youtube-dl.exe failed to download!");

            result = result.Replace("\n", "").Replace(Environment.NewLine, "");
            return result;
        }

		public void DeleteFile(string file)
		{
			File.Delete(file);
		}
	}
}