using System;
using System.Collections.Generic;
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

		public void ChangeRep(IUser iuser, int rep)
		{
			User user = users.Find(u => u.user == iuser.ToString());
			if(user == null)
				users.Add(new User(iuser.ToString(), rep > 0 ? rep : 0));
			else
			{
				user.reputation += rep;
				user.reputation = user.reputation <= 0 ? 0 : user.reputation;
			}
			File.WriteAllText(Directory.GetCurrentDirectory() + "/reps.json", JsonConvert.SerializeObject(users));
		}

		public int GetRepByUser(IUser iuser)
		{
			int rep = 0;
			User user = users.Find(u => u.user == iuser.ToString());
			if(user == null)
			{
				return rep;
			}
			return user.reputation;
		}
	}
}
