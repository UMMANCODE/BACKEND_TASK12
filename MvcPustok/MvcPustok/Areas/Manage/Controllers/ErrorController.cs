using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MvcPustok.Areas.Manage.Controllers {
	[Area("manage")]
	[Authorize]
	public class ErrorController : Controller {
		public IActionResult NotFound() {
			return View();
		}
	}
}

