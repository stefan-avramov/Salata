using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PrototypeTopCoder.Models;

namespace PrototypeTopCoder.Controllers
{
	public class CompetitionController : Controller
	{
		//
		// GET: /Competition/

		public ActionResult Index(int id)
		{
			InsideCompetitionModel model = DataHelper.GetInsideCompetitionModel(Session["username"] as string, id);
			if (model == null)
			{
				return RedirectToAction("Index", "Home");
			}

			return View(model);
		}

		[HttpGet]
		public JsonResult SubmitSimpleQuestion(int id, int answer)
		{
			if (DataHelper.SubmitSimpleQuestion(Session["username"] as string, id, answer))
				return new JsonResult() { JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet,  Data = "OK" };
			else
				return new JsonResult() { JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet, Data = "Failed" };
		}

		[HttpGet]
		public JsonResult SubmitComplexQuestion(int id, string answer)
		{
			if (DataHelper.SubmitComplexQuestion(Session["username"] as string, id, answer))
				return new JsonResult() { JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet, Data = "OK" };
			else
				return new JsonResult() { JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet, Data = "Failed" };
		}
	}
}
