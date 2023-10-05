using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddControllers().AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

IConfiguration _config = builder.Configuration;


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(_config.GetSection("AzureAd"));


builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("permisoweather", p =>
    {
        p.RequireAssertion(c => c.User.HasClaim(o =>
                                {
                                    bool _result = false;
                                    if (o.Type == "http://schemas.microsoft.com/identity/claims/scope")
                                    {
                                        string[] _valores = o.Value.Split(' ');
                                        _result = _valores.Contains("miotropermiso");
                                    }
                                    return _result;
                                }));
    });
    o.AddPolicy("permisototal", p =>
    {
        p.RequireAssertion(c => c.User.HasClaim(o =>
        {
            bool _result = false;
            if (o.Type == "http://schemas.microsoft.com/identity/claims/scope")
            {
                string[] _valores = o.Value.Split(' ');
                _result = _valores.Contains("mipermiso") && _valores.Contains("minadapermiso");
            }
            return _result;
        }));
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
