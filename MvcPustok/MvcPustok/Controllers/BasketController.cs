using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcPustok.Data;
using MvcPustok.ViewModels;
using Newtonsoft.Json;

namespace MvcPustok.Controllers {
	public class BasketController : Controller {
		private readonly AppDbContext _context;

		public BasketController(AppDbContext context) {
			_context = context;
		}

		public async Task<IActionResult> Wishlist() {
			List<BasketCookiesViewModel> basketCookiesViewModels = new();

			if (HttpContext.Request.Cookies["Books"] != null) {
				basketCookiesViewModels = JsonConvert.DeserializeObject<List<BasketCookiesViewModel>>(HttpContext.Request.Cookies["Books"]);
			}
			else {
				basketCookiesViewModels = new();
			}
			List<BasketViewModel> basketViewModels = new();

			foreach (var item in basketCookiesViewModels) {
				var book = await _context.Books.Include(x => x.BookImages).FirstOrDefaultAsync(x => x.Id == item.BookId);
				basketViewModels.Add(new BasketViewModel {
					Title = book.Name,
					Status = book.StockStatus,
					Price = (double)book.SalePrice,
					Img = book.BookImages.Where(x => x.Status == true).FirstOrDefault().Name,
					Count = item.Count,
					TotalPrice = (int)book.SalePrice * item.Count,
				});

			}
			return View(basketViewModels);
		}
	}
}