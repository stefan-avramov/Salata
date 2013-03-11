using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PrototypeTopCoder.Models;

namespace PrototypeTopCoder.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			ViewBag.Message = "Upcoming competitions";

			var enrolledCompetitions = DataHelper.GetEnrolledCompetitionsIds(Session["username"] as string);

			var model = new HomeModel()
				{
					Competitions = DataHelper.GetAllCompetitions(),
					EnrolledCompetitions = enrolledCompetitions != null ? new HashSet<int>(enrolledCompetitions) : new HashSet<int>()
				};
			return View(model);
		}

		[HttpGet]
		public ActionResult EnrollForCompetition(int id)
		{
			DataHelper.EnrollUserForCompetition(Session["username"] as string, id);
			return RedirectToAction("Index");
		}

		public ActionResult About()
		{
			return View();
		}
	}
}
