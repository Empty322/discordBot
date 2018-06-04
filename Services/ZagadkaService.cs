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
		private List<Zagadka> zagadki;
		private DateTime last;
		private TimeSpan interval;
		private string answer;
		private bool guessed;
		private bool flag;
        private Random rnd;
        private TimeSpan intervalForSkip;

		public ZagadkaService()
		{
			zagadki = JsonConvert.DeserializeObject<List<Zagadka>>(File.ReadAllText(Directory.GetCurrentDirectory() + "/questions.json"));

			interval = new TimeSpan(0, 0, 30);
            intervalForSkip = new TimeSpan(0, 5, 0);
			last = DateTime.Now;
			guessed = false;
			flag = true;
            rnd = new Random();
        }

		public string GetZagadku()
		{
            if (!guessed && !flag)
            {
                return "Сначала отгадай загадку, потом проси новую";
            }
			if ((DateTime.Now - last > interval && guessed) || flag)
			{
				flag = false;
				guessed = false;
				int i = rnd.Next(zagadki.Count - 1);
				answer = zagadki[i].otvet;
				return zagadki[i].zagadka;
			}
			return "Подождите немного. Интервал между разгадкой и новой загадкой 30 секунд";
		}

        public string SkipZagadku()
        {
            if(DateTime.Now - last > intervalForSkip)
            {
                guessed = true;
                return "Никто не угадал. Ну и ладно, вот вам новая загадка:\n" + GetZagadku();
            }
            return "5 минут еще не прошло. Не торопись, подумай получше";
        }

		private bool CheckAbout(string ans)
		{
			double times = 0;
			for(int i = 0; (ans.Length > i) && (answer.Length) > i; ++i)
			{
				if(ans.ToLower()[i] != answer.ToLower()[i])
				{
					times+= 1;
				}
			}
			times += Math.Abs(ans.Length - answer.Length);
			double difference = times / (double)answer.Length;
			//Console.WriteLine(times);
			//Console.WriteLine(answer.Length);
			//Console.WriteLine(difference);
			if(difference > 0.2)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public AnswerResult CheckAnswer(string ans)
		{
			if(guessed)
				return AnswerResult.Guessed;
			if(!CheckAbout(ans))
				return AnswerResult.WrongAnswer;
			last = DateTime.Now;
			guessed = true;
			return AnswerResult.CorrectAnswer;
		}
    }
}
