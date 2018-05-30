using System;
using System.Collections.Generic;
using System.Text;
using bot.Models;
using Discord;
using Newtonsoft.Json;
using System.IO;

namespace bot.Services
{
    public class ReputationService
    {
		private List<User> users;

		public ReputationService()
		{
			if(!File.Exists(Directory.GetCurrentDirectory() + "/reps.json"))
				users = new List<User>();
			else
				users = JsonConvert.DeserializeObject<List<User>>(File.ReadAllText(Directory.GetCurrentDirectory() + "/reps.json"));
		}

		public void ChangeRep(IUser user, int rep)
		{
			if (users.Find(u => u.user == user.ToString()) == null)
			{
				users.Add(new User(user.ToString(), rep));
			}
			File.WriteAllText(Directory.GetCurrentDirectory() + "/reps.json", JsonConvert.SerializeObject(users));
		}
	}
}
