using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("conn")
    ));

QuestPDF.Settings.License = LicenseType.Community;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLower() ?? string.Empty;

    var isPublicRoute =
        path.StartsWith("/account/login") ||
        path.StartsWith("/account/register") ||
        path.StartsWith("/account/forgotpassword") ||
        path.StartsWith("/account/resetpassword");

    if (!isPublicRoute)
    {
        var userId = context.Session.GetInt32("UserId");
        if (userId == null)
        {
            context.Response.Redirect("/Account/Login");
            return;
        }
    }

    if (path == "/" || path == "/account/login")
    {
        var userId = context.Session.GetInt32("UserId");
        if (userId != null)
        {
            var role = context.Session.GetString("UserRole")?.ToLower();
            if (role == "user")
            {
                context.Response.Redirect("/User/Dashboard");
                return;
            }

            context.Response.Redirect("/Product/Index");
            return;
        }
    }

    await next();
});

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();

builder.Services.AddSession();

app.UseSession();