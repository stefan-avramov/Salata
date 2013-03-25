using System;
using System.Linq;
using System.Web.Mvc;
using PrototypeTopCoder.Models;

namespace PrototypeTopCoder.Controllers
{
	public class ProfileController : Controller
	{
		[HttpGet]
		public ActionResult OwnProfile()
		{
			return View(new ProfileModel() {
				Username = Session["username"] as string});
		}
	}
}
