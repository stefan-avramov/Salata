using System;
using System.Linq;
using System.Web.Mvc;
using PrototypeTopCoder.Models;

namespace PrototypeTopCoder.Controllers
{
	public class AdminController : Controller
	{
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if (!Object.Equals(Session["isAdmin"], true))
			{
				filterContext.Result = new RedirectResult("/Account/Logon?returnUrl=" + Request.Url.LocalPath, false);
				return;
			}

			base.OnActionExecuting(filterContext);
		}

		public ActionResult Index()
		{
			return View(DataHelper.GetAllCompetitions());
		}

		public ActionResult CreateCompetition()
		{
			ViewBag.Categories = DataHelper.GetCategories();
			return View(new CompetitionModel());
		}

		[HttpPost]
		public ActionResult CreateCompetition(CompetitionModel model)
		{
			DataHelper.AddNewCompetition(model);
			return RedirectToAction("Index");
		}

		public ActionResult EditCompetition(int id)
		{
			ViewBag.Categories = DataHelper.GetCategories();
			return View(DataHelper.GetCompetition(id));
		}

		[HttpPost]
		public ActionResult EditCompetition(CompetitionModel model, int id)
		{
			DataHelper.EditCompetition(id, model);
			return RedirectToAction("Index");
		}

        public ActionResult DeleteCompetition(int id)
        {
            DataHelper.DeleteCompetition(id);
            return RedirectToAction("Index");
        }

		public ActionResult CreateProblem(int id, int type)
		{
			if (type == (int)ProblemType.SimpleTestQuestion)
			{
				return View("CreateSimpleTestProblem");
			}
			else if (type == (int)ProblemType.ComplexTextQuestion)
			{
				return View("CreateComplexTestProblem");
			}

			return RedirectToAction("Index");
		}

		[HttpPost]
		public ActionResult CreateSimpleTestProblem(SimpleTestProblemModel model, int? id)
		{
			DataHelper.AddNewProblem(model, id, ProblemType.SimpleTestQuestion);
			return RedirectToAction("Index");
		}

		[HttpPost]
		public ActionResult CreateComplexTestProblem(ComplexTestProblemModel model, int? id)
		{
			DataHelper.AddNewProblem(model, id, ProblemType.ComplexTextQuestion);
			return RedirectToAction("Index");
		}

        public ActionResult DeleteTask(int id)
        {
            DataHelper.DeleteProblem(id);
            return RedirectToAction("ViewTasks");
        }

		public ActionResult ViewTasks()
		{
			return View(DataHelper.GetAllTasks());
		}

		public ActionResult ViewTask(int id)
		{
			ProblemModel model = DataHelper.GetTask(id);
			return View(model);
		}
	}
}
