var builder = WebApplication.CreateBuilder(args);
IConfiguration _config = builder.Configuration;

// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
