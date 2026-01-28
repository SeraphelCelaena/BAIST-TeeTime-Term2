using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<CookiePolicyOptions>(options =>
{
	options.CheckConsentNeeded = context => true;
	options.MinimumSameSitePolicy = SameSiteMode.None;
	options.Secure = CookieSecurePolicy.Always;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
	cookieOptions =>
	{
		cookieOptions.LoginPath = "/Login";
		cookieOptions.LogoutPath = "/Logout";
		cookieOptions.SlidingExpiration = true;
		cookieOptions.Cookie.Name = "TeeTimeWebAppAuthCookie";
		cookieOptions.ExpireTimeSpan = TimeSpan.FromDays(7);
		cookieOptions.Cookie.HttpOnly = true;
	}
);

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
