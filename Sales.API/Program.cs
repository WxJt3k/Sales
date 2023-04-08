using Microsoft.EntityFrameworkCore;
using Sales.API.Data;
using System.Text.Json.Serialization;
using Sales.API.Services;
using Microsoft.AspNetCore.Identity;
using Sales.API.Helpers;
using Sales.Shared.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
.AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(x => x.UseSqlServer("Name = DB"));
builder.Services.AddTransient<SeedDb>();
builder.Services.AddScoped<IApiService, ApiService>();
builder.Services.AddScoped<IUserHelper, UserHelper>();

builder.Services.AddIdentity<User, IdentityRole>(x =>
{
    x.User.RequireUniqueEmail = true;
    x.Password.RequireDigit = false;
    x.Password.RequiredUniqueChars = 0;
    x.Password.RequireLowercase = false;
    x.Password.RequireNonAlphanumeric = false;
    x.Password.RequireUppercase = false;
})
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();
SeedData(app);

void SeedData (WebApplication app)
{
    IServiceScopeFactory? scopedFactory = app.Services.GetService<IServiceScopeFactory?>();

    using (IServiceScope? scope = scopedFactory!.CreateScope())
    {
        SeedDb? service = scope.ServiceProvider.GetService<SeedDb?>();
        service!.SeedAsync().Wait();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseCors(x =>
x.AllowAnyMethod()
.AllowAnyHeader()
.SetIsOriginAllowed(origin => true)
.AllowCredentials());

app.Run();
