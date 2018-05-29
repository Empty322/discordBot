using System;
using System.Collections.Generic;
using System.Text;

namespace bot.Services
{
    public class ZagadkaService
    {
		private KeyValuePair<string, string>[] zagadki;
		private DateTime last;
		private TimeSpan interval;
		private string answer;
		private bool guessed;

		public ZagadkaService()
		{
			zagadki[0] = new KeyValuePair<string, string>("загадка типа", "ответ");

			interval = new TimeSpan(0, 0, 10);
			last = DateTime.Now;
			guessed = false;
		}

		public string GetZagadku()
		{
			if (DateTime.Now - last > interval)
			{
				Random rnd = new Random();
				int i = rnd.Next(zagadki.Length - 1);
				answer = zagadki[i].Value;
				return zagadki[i].Key;
			}
			return "время не прошло";
		}

		public string CheckAnswer(string ans)
		{
			if(guessed)
				return "Загадка уже отгадана";
			if(ans != answer)
				return "Неправильный ответ";
			last = DateTime.Now;
			guessed = true;
			return "Верно";
		}
    }
}
