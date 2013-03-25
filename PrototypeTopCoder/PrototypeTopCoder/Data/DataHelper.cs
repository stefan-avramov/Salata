using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PrototypeTopCoder.Models;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Data;

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

        internal static void DeleteCompetition(int id)
        {
            using (TopCoderPrototypeEntities entityModel = new TopCoderPrototypeEntities())
            {
                entityModel.CompetitionsUsers.Where(x => x.CompetitionId == id).ToList()
                    .ForEach(entityModel.CompetitionsUsers.DeleteObject);
                entityModel.Competitions.Where(x => x.ID == id).ToList().ForEach(entityModel.Competitions.DeleteObject);
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
                    var compTasks = entityModel.CompetetionsProblems.Where(x => x.CompetetionId == competitionId);
                    model.Problems = new List<ProblemModel>();
                    foreach (var task in compTasks)
                    {
                        model.Problems.Add(DataHelper.GetTask(task.ProblemId));
                    }
					return model;
				}
			}

			return null;
		}

		internal static bool SubmitSimpleQuestion(string username, int problemId, int answer)
		{
			using (TopCoderPrototypeEntities entityModel = new TopCoderPrototypeEntities())
			{
				CompetitionsUser cuser = entityModel.CompetitionsUsers.
					Where(x => x.User.Username == username && x.Competition.CompetetionsProblems.Any(y => y.ProblemId == problemId)).FirstOrDefault();
				if(cuser == null || !cuser.Start.HasValue || cuser.Start.Value.AddMinutes(cuser.Competition.Duration) < DateTime.Now)
				{
					return false;
				}

				SimpleProblemAnswer ans = new SimpleProblemAnswer();
				ans.Answer = answer;

				BinaryFormatter bf = new BinaryFormatter();
				MemoryStream ms = new MemoryStream();
				bf.Serialize(ms, ans);
				
				Submission submission = new Submission();
				submission.UserId = cuser.UserId;
				submission.ProblemId = problemId;
				submission.Submitted = DateTime.Now;
				submission.Answer = ms.ToArray();
				
				entityModel.AddToSubmissions(submission);
				entityModel.SaveChanges();
			}

			return true;
		}

		internal static bool SubmitComplexQuestion(string username, int problemId, string answer)
		{
			using (TopCoderPrototypeEntities entityModel = new TopCoderPrototypeEntities())
			{
				CompetitionsUser cuser = entityModel.CompetitionsUsers.
					Where(x => x.User.Username == username && x.Competition.CompetetionsProblems.Any(y => y.ProblemId == problemId)).FirstOrDefault();
				if (cuser == null || !cuser.Start.HasValue || cuser.Start.Value.AddMinutes(cuser.Competition.Duration) < DateTime.Now)
				{
					return false;
				}

				string[] tokens = answer.Split(',');
				List<int> ints = new List<int>();

				foreach (string token in tokens)
				{
					int res = -1;
					if (int.TryParse(token, out res))
					{
						ints.Add(res);
					} 
				}

				ComplexProblemAnswer ans = new ComplexProblemAnswer();
				ans.Answers = ints.ToArray();

				BinaryFormatter bf = new BinaryFormatter();
				MemoryStream ms = new MemoryStream();
				bf.Serialize(ms, ans);

				Submission submission = new Submission();
				submission.UserId = cuser.UserId;
				submission.ProblemId = problemId;
				submission.Submitted = DateTime.Now;
				submission.Answer = ms.ToArray();

				entityModel.AddToSubmissions(submission);
				entityModel.SaveChanges();
			}

			return true;
		}

		internal static DataTable GetCompetitionResults(int id)
		{
			using (TopCoderPrototypeEntities entityModel = new TopCoderPrototypeEntities())
			{
				Competition comp = entityModel.Competitions.Where(x => x.ID == id).First();

				foreach (CompetetionsProblem problem in comp.CompetetionsProblems)
				{
					foreach (Submission submit in entityModel.Submissions.Where(x => x.ProblemId == problem.ProblemId && !x.Score.HasValue))
					{
						ProblemModel problemModel = GetTask(problem.ProblemId);
						MemoryStream stream = new MemoryStream(submit.Answer);
						BinaryFormatter formatter = new BinaryFormatter();
						IProblemAnswer answer = (IProblemAnswer)formatter.Deserialize(stream);
						submit.Score = problemModel.Evaluate(answer);
					}
				}

				entityModel.SaveChanges();
			}

			using (TopCoderPrototypeEntities entityModel = new TopCoderPrototypeEntities())
			{
				Competition comp = entityModel.Competitions.Where(x => x.ID == id).First();
				DataTable table = new DataTable();
				table.Columns.Add("Username");
				foreach (CompetetionsProblem problem in comp.CompetetionsProblems)
				{
					table.Columns.Add(problem.Problem.Title);
				}
				
				table.Columns.Add("Total");

				foreach (CompetitionsUser user in comp.CompetitionsUsers)
				{
					DataRow row = table.NewRow();
					row["Username"] = user.User.Username;
					int totalScore = 0;
					foreach (CompetetionsProblem problem in comp.CompetetionsProblems)
					{ 
						Submission subm = entityModel.Submissions
							.Where(x => x.UserId == user.UserId && x.ProblemId == problem.ProblemId)
							.OrderByDescending(x => x.ID).FirstOrDefault();
						int score = subm != null && subm.Score.HasValue ? subm.Score.Value : 0;
						totalScore += score;
						row[problem.Problem.Title] = score;
					}

					row["Total"] = totalScore;
					table.Rows.Add(row);
				}

				return table;
			} 
		} 
	}
}