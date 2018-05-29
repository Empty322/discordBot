using System;
using System.Collections.Generic;
using bot.Models;
using System.Numerics;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace bot.Services
{
    public class ZagadkaService
    {
		private Zagadka[] zagadki;
		private DateTime last;
		private TimeSpan interval;
		private string answer;
		private bool guessed;

		public ZagadkaService()
		{
			JsonConvert.DeserializeObject<Zagadka[]>(File.ReadAllText(Directory.GetCurrentDirectory() + "/zagadki.json"));

			interval = new TimeSpan(0, 0, 10);
			last = DateTime.Now;
			guessed = false;
		}

		public string GetZagadku()
		{
			if (DateTime.Now - last > interval)
			{
				guessed = false;
				Random rnd = new Random();
				int i = rnd.Next(zagadki.Length - 1);
				answer = zagadki[i].otvet;
				return zagadki[i].zagadka;
			}
			return "время не прошло";
		}

		public AnswerResult CheckAnswer(string ans)
		{
			if(guessed)
				return AnswerResult.Guessed;
			if(ans != answer)
				return AnswerResult.WrongAnswer;
			last = DateTime.Now;
			guessed = true;
			return AnswerResult.CurrectAnswer;
		}
    }
}
