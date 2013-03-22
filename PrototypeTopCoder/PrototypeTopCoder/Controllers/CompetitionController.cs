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

    }
}
