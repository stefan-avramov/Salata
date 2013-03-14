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
		public static void EnrollUserForCompetition(string username, int competitionId)
		{
			using (TopCoderPrototypeEntities model = new TopCoderPrototypeEntities())
			{
				var user = GetUser(model, username);
				if (user != null)
				{
					CompetitionsUser competitionUser = model.CompetitionsUsers
						.Where(x => x.UserId == user.ID && x.CompetitionId == competitionId)
						.FirstOrDefault();

					// evade duplicates - mad evasion
					if (competitionUser == null)
					{
						user.CompetitionsUsers.Add(new CompetitionsUser() { UserId = user.ID, CompetitionId = competitionId });
						model.SaveChanges();
					}
				}
			}
		}
		
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

				User user = GetUser(model, username);
				if (user != null && user.Password == password)
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

		public static List<CompetitionModel> GetAllCompetitions()
		{
			using (TopCoderPrototypeEntities entityModel = new TopCoderPrototypeEntities())
			{
				return entityModel.Competitions.Select(x => new CompetitionModel() { EntityModel = x }).ToList();
			}
		}

		public static Dictionary<Models.CompetitionModel, UserCompetitionState> GetAllCompetitions(string username)
		{
			using (TopCoderPrototypeEntities entityModel = new TopCoderPrototypeEntities())
			{ 
				Dictionary<CompetitionModel, UserCompetitionState> result = new Dictionary<CompetitionModel, UserCompetitionState>();

				foreach(Competition competition in entityModel.Competitions)	
				{
					CompetitionModel model = new CompetitionModel() { EntityModel = competition };
					UserCompetitionState state = UserCompetitionState.NotEnrolled;
					CompetitionsUser cuser = entityModel.CompetitionsUsers.Where(x => x.User.Username == username && x.CompetitionId == competition.ID).FirstOrDefault();
					
					if (cuser == null)
					{
						state = UserCompetitionState.NotEnrolled;
					}
					else if (cuser.Start == null)
					{
						state = UserCompetitionState.NotStarted;
					}
					else if ((DateTime.Now - cuser.Start.Value) < TimeSpan.FromMinutes(competition.Duration))
					{
						state = UserCompetitionState.InProgress;
					}
					else
					{
						state = UserCompetitionState.Ended;
					}

					result.Add(model, state);
				}

				return result;
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

		public static void AddNewProblem(ProblemModel model, int? id, ProblemType type)
		{
			using (TopCoderPrototypeEntities entityModel = new TopCoderPrototypeEntities())
			{
				Problem problem = new Problem();
				problem.Title = model.Title;
				problem.ProblemType = (int)type;

				BinaryFormatter bf = new BinaryFormatter();
				MemoryStream ms = new MemoryStream();
				bf.Serialize(ms, model);
				problem.Data = ms.ToArray(); 

				entityModel.AddToProblems(problem);
				entityModel.SaveChanges();
			}
		}
 
		private static User GetUser(TopCoderPrototypeEntities model, string username)
		{
			if (username != null)
			{
				return model.Users.Where(x => x.Username == username).FirstOrDefault();
			}
			return null;
		}

		public static List<ProblemModel> GetAllTasks()
		{
			using (TopCoderPrototypeEntities entityModel = new TopCoderPrototypeEntities())
			{
				return entityModel.Problems.Select(x => new ProblemModel(){Title = x.Title, ID = x.ID}).ToList();
			}
		}

		public static ProblemModel GetTask(int id)
		{
			using (TopCoderPrototypeEntities entityModel = new TopCoderPrototypeEntities())
			{
				Problem pr = entityModel.Problems.Where(x => x.ID == id).FirstOrDefault();
				if (pr.ProblemType == (int)ProblemType.SimpleTestQuestion)
				{
					BinaryFormatter formatter = new BinaryFormatter();
					MemoryStream stream = new MemoryStream(pr.Data);
					return (SimpleTestProblemModel)formatter.Deserialize(stream); 
				}
				else if (pr.ProblemType == (int)ProblemType.ComplexTextQuestion)
				{
					BinaryFormatter formatter = new BinaryFormatter();
					MemoryStream stream = new MemoryStream(pr.Data);
					return (ComplexTestProblemModel)formatter.Deserialize(stream); 
				}
			}

			return null;
		}
		 
	}
}