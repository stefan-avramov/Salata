﻿using System;
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
			 
			return View( DataHelper.GetAllCompetitions(Session["username"] as string) );
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
