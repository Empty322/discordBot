using Discord;

namespace bot.Models
{
	public class User
	{
		public string user;
		public int reputation;

		public User(string user, int reputation)
		{
			this.user = user;
			this.reputation = reputation;
		}
	}
}
