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
                if (model.SelectedProblems != null)
                {
                    foreach (var problem in model.SelectedProblems)
                    {
                        CompetetionsProblem cp = new CompetetionsProblem();
                        cp.CompetetionId = comp.ID;
                        cp.ProblemId = problem;
                        entityModel.CompetetionsProblems.AddObject(cp);
                    }
                    entityModel.SaveChanges();
                }
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
                entityModel.CompetetionsProblems.Where(x => x.CompetetionId == id).ToList()
                    .ForEach(entityModel.CompetetionsProblems.DeleteObject);
                if (model.SelectedProblems != null)
                {
                    foreach (var problem in model.SelectedProblems)
                    {
                        CompetetionsProblem cp = new CompetetionsProblem();
                        cp.CompetetionId = comp.ID;
                        cp.ProblemId = problem;
                        entityModel.CompetetionsProblems.AddObject(cp);
                    }
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

                entityModel.AddToProblems(problem);
                entityModel.SaveChanges();
                model.ID = problem.ID;

				BinaryFormatter bf = new BinaryFormatter();
				MemoryStream ms = new MemoryStream();
				bf.Serialize(ms, model);
				problem.Data = ms.ToArray(); 

				entityModel.SaveChanges();
			}
		}

        public static void DeleteProblem(int id)
        {
            using (TopCoderPrototypeEntities entityModel = new TopCoderPrototypeEntities())
            {
                var problems = entityModel.Problems.Where(x => x.ID == id).ToList();
                foreach (var problem in problems)
                {
                    entityModel.Problems.DeleteObject(problem);
                }
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

        public static List<ProblemModel> GetTasks(int compID)
        {
            using (TopCoderPrototypeEntities entityModel = new TopCoderPrototypeEntities())
            {
                return entityModel.CompetetionsProblems
                    .Where(x => x.CompetetionId == compID)
                    .Select(x => new ProblemModel() { Title = x.Problem.Title, ID = x.Problem.ID })
                    .ToList();
            }
        }

		public static ProblemModel GetTask(int id)
		{
			using (TopCoderPrototypeEntities entityModel = new TopCoderPrototypeEntities())
			{
                Problem pr = entityModel.Problems.Where(x => x.ID == id).FirstOrDefault();
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream(pr.Data);
                ProblemModel model = null;
				if (pr.ProblemType == (int)ProblemType.SimpleTestQuestion)
				{
					model = (SimpleTestProblemModel)formatter.Deserialize(stream); 
				}
                else if (pr.ProblemType == (int)ProblemType.ComplexTextQuestion)
                {
                    model = (ComplexTestProblemModel)formatter.Deserialize(stream);
                }
                else
                {
                    return null;
                }
                model.ID = pr.ID;
                return model;
			}

			return null;
		}


		internal static void StartCompetitionForUser(string username, int competitionId)
		{
			using (TopCoderPrototypeEntities entityModel = new TopCoderPrototypeEntities())
			{
				CompetitionsUser cuser = entityModel.CompetitionsUsers.
					Where(x => x.User.Username == username && x.CompetitionId == competitionId).FirstOrDefault();
				if (cuser != null && cuser.Start == null)
				{
					cuser.Start = DateTime.Now;
					entityModel.SaveChanges();
				} 
			}
		}

		internal static InsideCompetitionModel GetInsideCompetitionModel(string username, int competitionId)
		{
			using (TopCoderPrototypeEntities entityModel = new TopCoderPrototypeEntities())
			{
				CompetitionsUser cuser = entityModel.CompetitionsUsers.
					Where(x => x.User.Username == username && x.CompetitionId == competitionId).FirstOrDefault();
				if (cuser != null && cuser.Start.HasValue)
				{
					InsideCompetitionModel model = new InsideCompetitionModel();
					model.TimeLeft = TimeSpan.FromMinutes(cuser.Competition.Duration) - (DateTime.Now - cuser.Start.Value);
					return model;
				}
			}

			return null;
		}
	}
}