using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

        //
        // GET: /Competitions/

        public ActionResult Index()
        {
            return View(DataHelper.GetAllCompetitions());
        }

        public ActionResult CreateCompetition()
        {
			ViewBag.Categories = DataHelper.GetCategories();
            return View();
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

		public ActionResult CreateProblem(int id, int type)
		{
			if (type == (int)ProblemType.SimpleTest)
			{
				return View("CreateSimpleTestProblem");
			}

			return RedirectToAction("Index");
		}

		[HttpPost]
		public ActionResult CreateSimpleTestProblem(SimpleTestProblemModel model, int? id)
		{
			DataHelper.AddNewProblem(model, id);
			return RedirectToAction("Index");
		}


    }
}
