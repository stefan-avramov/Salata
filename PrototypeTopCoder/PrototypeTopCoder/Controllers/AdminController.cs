using System;
using System.Linq;
using System.Web.Mvc;
using PrototypeTopCoder.Models;

namespace PrototypeTopCoder.Controllers
{
	public class AdminController : Controller
	{
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
	}
}
