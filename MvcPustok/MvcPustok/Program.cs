using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MvcPustok.Data;
using MvcPustok.Models;
using MvcPustok.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(option => {
	option.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddIdentity<AppUser, IdentityRole>(opt => {
	opt.Password.RequireNonAlphanumeric = false;
	opt.Password.RequireUppercase = false;
	opt.Password.RequiredLength = 8;
	opt.User.RequireUniqueEmail = true;
}).AddDefaultTokenProviders().AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddControllersWithViews()
		.AddNewtonsoftJson(options =>
		options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

builder.Services.AddScoped<LayoutService>();

builder.Services.AddHttpContextAccessor();

builder.Services.ConfigureApplicationCookie(opt => {
	opt.Events.OnRedirectToLogin = opt.Events.OnRedirectToAccessDenied = context => {
		if (context.Request.Path.Value.ToLower().StartsWith("/manage")) {
			var uri = new Uri(context.RedirectUri);
			context.Response.Redirect("/manage/auth/login" + uri.Query);
		}
		else {
			var uri = new Uri(context.RedirectUri);
			context.Response.Redirect("/auth/login" + uri.Query);
		}
		return Task.CompletedTask;
	};
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllerRoute(
					 name: "areas",
					 pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
				 );
app.MapControllerRoute(
		name: "default",
		pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

