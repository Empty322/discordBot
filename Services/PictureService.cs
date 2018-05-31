using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using bot.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

namespace bot.Services
{
	public class PictureService
	{
		private readonly HttpClient http;
        private readonly List<Picture> pictures;
        Random rnd;

		public PictureService(HttpClient http)
		{
            rnd = new Random();
			this.http = http;
            pictures = JsonConvert.DeserializeObject<List<Picture>>(File.ReadAllText(Directory.GetCurrentDirectory() + "/pictures.json"));
        }

        public async Task<Stream> GetPictureAsync()
        {
            int i = rnd.Next(pictures.Count - 1);
            var res = await http.GetAsync(pictures[i].url);
            return await res.Content.ReadAsStreamAsync();
        }
	}
}