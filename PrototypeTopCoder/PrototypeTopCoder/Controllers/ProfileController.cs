using System;
using System.Linq;
using System.Web.Mvc;
using PrototypeTopCoder.Models;

namespace PrototypeTopCoder.Controllers
{
	public class ProfileController : Controller
	{
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if (Session["username"] == null)
			{
				filterContext.Result = new RedirectResult("/Account/Logon?returnUrl=" + Request.Url.LocalPath, false);
				return;
			}

			base.OnActionExecuting(filterContext);
		}

		[HttpGet]
		public ActionResult OwnProfile()
		{
			return View(new ProfileModel() {
				Username = Session["username"] as string});
		}
	}
}
