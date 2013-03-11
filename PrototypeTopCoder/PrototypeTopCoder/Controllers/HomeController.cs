using System;
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

			return View(new HomeModel()
			{
				Competitions = DataHelper.GetAllCompetitions(),
				EnrolledCompetitions = DataHelper.GetEnrolledCompetitionsIds(Session["username"] as string)
			});
		}

		public ActionResult About()
		{
			return View();
		}
	}
}
