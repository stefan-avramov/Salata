using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PrototypeTopCoder.Models;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace PrototypeTopCoder
{
	public static class DataHelper
	{
        public enum UserType
        {
            NotExisting,
            User,
            Admin
        }

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

        public static UserType UserExists(string username, string password)
		{
			using (TopCoderPrototypeEntities model = new TopCoderPrototypeEntities())
			{
                UserType res = UserType.NotExisting;

				User user = model.Users.Where(x => x.Username == username && x.Password == password).FirstOrDefault();
                if (user != null)
                {
                    if (user.Type == 1)
                    {
                        res = UserType.User;
                    }
                    else if(user.Type == 2)
                    {
                        res = UserType.Admin;
                    }
                }

                return res;
			}
		}

        public static void AddNewCompetition(Models.CompetitionModel model)
        {
            Competition comp = new Competition();
            comp.CategoryId = model.CategoryId;
            comp.Start = model.Start;
            comp.End = model.End;
            comp.Duration = model.Duration;
			comp.Name = model.Name;
			comp.Description = model.Description;

            using (TopCoderPrototypeEntities entityModel = new TopCoderPrototypeEntities())
            {
                entityModel.AddToCompetitions(comp);
                entityModel.SaveChanges();
            }
        }

        public static List<Models.CompetitionModel> GetAllCompetitions()
        {
            using (TopCoderPrototypeEntities entityModel = new TopCoderPrototypeEntities())
            {
				return entityModel.Competitions.Select(x => new CompetitionModel() { EntityModel = x }).ToList();
            }
        }

        internal static CompetitionModel GetCompetition(int id)
        {
            using (TopCoderPrototypeEntities entityModel = new TopCoderPrototypeEntities())
            {
                Competition comp = entityModel.Competitions.Where(x => x.ID == id).FirstOrDefault();
                if (comp != null)
                {
                    return new CompetitionModel(comp);
                }

                return null;
            }
        }

        internal static void EditCompetition(int id, CompetitionModel model)
        {
			using (TopCoderPrototypeEntities entityModel = new TopCoderPrototypeEntities())
			{
				Competition comp = entityModel.Competitions.Where(x => x.ID == id).FirstOrDefault();
				if (comp != null)
				{
					comp.CategoryId = model.CategoryId;
					comp.Start = model.Start;
					comp.End = model.End;
					comp.Duration = model.Duration;
					comp.Name = model.Name;
					comp.Description = model.Description;
				}
				entityModel.SaveChanges();
			}
        }

		public static dynamic GetCategories()
		{
			using (TopCoderPrototypeEntities entityModel = new TopCoderPrototypeEntities())
			{
				return entityModel.Categories.ToList();
			}
		}

		public static void AddNewProblem(ProblemModel model, int? id)
		{
			using (TopCoderPrototypeEntities entityModel = new TopCoderPrototypeEntities())
			{
				Problem problem = new Problem();
				problem.Title = model.Title;

				BinaryFormatter bf = new BinaryFormatter();
				MemoryStream ms = new MemoryStream();
				bf.Serialize(ms, model);
				problem.Data =  ms.ToArray();

				entityModel.AddToProblems(problem);
				entityModel.SaveChanges();
			}
		}
	}
}