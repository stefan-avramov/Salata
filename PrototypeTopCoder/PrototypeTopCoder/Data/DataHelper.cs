using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrototypeTopCoder
{
	public static class DataHelper
	{
		public static void RegisterUser(string username, string password, string email)
		{
			using (TopCoderPrototypeEntities model = new TopCoderPrototypeEntities())
			{
				User user = new User();
				user.Username = username;
				user.Password = password;
				user.Email = email;
				user.Type = 1;
				model.Users.AddObject(user);
				model.SaveChanges();
			}
		}

		public static bool UserExists(string username, string password)
		{
			using (TopCoderPrototypeEntities model = new TopCoderPrototypeEntities())
			{
				return model.Users.Where(x => x.Username == username && x.Password == password).Count() == 1;
			}
		}
	}
}