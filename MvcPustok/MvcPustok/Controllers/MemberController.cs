using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MvcPustok.Controllers {
	public class MemberController : Controller {
		public IActionResult Profile() {
			return View();
		}
	}
}
