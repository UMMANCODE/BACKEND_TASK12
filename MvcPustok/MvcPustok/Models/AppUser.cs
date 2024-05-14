using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MvcPustok.Models {
	public class AppUser : IdentityUser {
		public string FullName { get; set; }
	}
}
