using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);
IConfiguration _config = builder.Configuration;

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme).
    AddMicrosoftIdentityWebApp(_config.GetSection("AzureAd"));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();


builder.Services.AddHttpClient("ApiProtegida", opt => {
    opt.BaseAddress = new Uri(_config.GetValue<string>("ApiExternas:ApiOxxo"));
});

builder.Services.AddHttpClient("ApiAurrera", opt => {
    opt.BaseAddress = new Uri(_config.GetValue<string>("ApiExternas:ApiAurrera"));
});


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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
