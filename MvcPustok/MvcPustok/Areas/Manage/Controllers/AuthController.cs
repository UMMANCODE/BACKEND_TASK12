using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcPustok.Areas.Manage.ViewModels;
using MvcPustok.Data;
using MvcPustok.Models;

namespace MvcPustok.Areas.Manage.Controllers {
	[Area("Manage")]
	public class AuthController : Controller {
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly AppDbContext _context;

		public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, AppDbContext context) {
			_userManager = userManager;
			_signInManager = signInManager;
			_context = context;
		}
		[Authorize(Roles = "SuperAdmin")]
		public async Task<IActionResult> CreateAdmin() {
			AppUser admin = new() {
				UserName = "admin",
				FullName = "Admin",
			};
			var result = await _userManager.CreateAsync(admin, "Admin123");
			return Json(result);
		}

		public IActionResult Login() {
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(AdminLoginViewModel loginVM, string returnUrl) {
			AppUser admin = await _userManager.FindByNameAsync(loginVM.UserName);

			if (admin == null) {
				ModelState.AddModelError("", "UserName or Password incorrect");
				return View();
			}


			var result = await _signInManager.PasswordSignInAsync(admin, loginVM.Password, false, false);

			if (!result.Succeeded) {
				ModelState.AddModelError("", "UserName or Password incorrect");
				return View();
			}

			return returnUrl != null ? Redirect(returnUrl) : RedirectToAction("index", "dashboard");
		}
	}
}
