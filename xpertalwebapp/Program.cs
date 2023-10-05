using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using static System.Formats.Asn1.AsnWriter;

var builder = WebApplication.CreateBuilder(args);
IConfiguration _config = builder.Configuration;

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(_config.GetSection("AzureAd")).
    EnableTokenAcquisitionToCallDownstreamApi(_config.GetSection("AzureAd:Scopes").Get<IEnumerable<string>>()).
    AddInMemoryTokenCaches();

//builder.Services.AddAuthorization(options =>
//{
//    options.FallbackPolicy = options.DefaultPolicy;
//});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();




builder.Services.AddHttpClient("ApiProtegida", httpClient =>
{
    httpClient.BaseAddress = new Uri(_config.GetValue<string>("ApiAd:urlapi"));
    httpClient.DefaultRequestHeaders.Add(
        "x-header-app", "webdemo");
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
