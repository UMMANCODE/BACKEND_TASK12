using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcPustok.Data;
using MvcPustok.Models;
using MvcPustok.ViewModels;

namespace MvcPustok.Controllers {
	public class AuthController : Controller {
		private readonly AppDbContext _context;
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;

		public AuthController(AppDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) {
			_context = context;
			_userManager = userManager;
			_signInManager = signInManager;
		}

		public IActionResult Logout() {
			_signInManager.SignOutAsync();
			return RedirectToAction("index", "home");
		}

		public IActionResult Login() {
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(MemberLoginViewModel loginVM, string returnUrl) {
			AppUser member = await _userManager.FindByNameAsync(loginVM.UserName);

			if (member == null) {
				ModelState.AddModelError("", "UserName or Password incorrect");
				return View();
			}

			var result = await _signInManager.PasswordSignInAsync(member, loginVM.Password, false, false);

			if (!result.Succeeded) {
				ModelState.AddModelError("", "UserName or Password incorrect");
				return View();
			}

			var fullNameClaim = new Claim("Custom:FullName", member.FullName);
			var emailClaim = new Claim("Custom:Email", member.Email);

			var fullNameParts = member.FullName.Split(" ");
			var firstNameClaim = new Claim("Custom:FirstName", fullNameParts.Length > 0 ? fullNameParts[0] : "");
			var lastNameClaim = new Claim("Custom:LastName", fullNameParts.Length > 1 ? fullNameParts[1] : "");

			var claims = new List<Claim> { fullNameClaim, emailClaim, firstNameClaim, lastNameClaim };

			var addClaimsResult = await _userManager.AddClaimsAsync(member, claims);
			if (!addClaimsResult.Succeeded) {
				ModelState.AddModelError("", "Claim could not be added");
				return View();
			}

			if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) {
				return Redirect(returnUrl);
			}
			else {
				return RedirectToAction("profile", "member");
			}
		}

		public IActionResult Register() {
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(MemberRegisterViewModel registerVM) {
			if (!ModelState.IsValid) return View();

			AppUser member = new() {
				UserName = registerVM.UserName,
				FullName = registerVM.FullName,
				Email = registerVM.Email
			};

			var result = await _userManager.CreateAsync(member, registerVM.Password);

			if (!result.Succeeded) {
				foreach (var error in result.Errors) {
					ModelState.AddModelError("", error.Description);
				}
				return View();
			}

			await _signInManager.SignInAsync(member, true);

			return RedirectToAction("profile", "member");
		}
	}
}
