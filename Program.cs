using AuthApi.Controllers;
using AuthApi.Enums;
using AuthApi.Extensions;
using AuthApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


builder.Services
            .AddSwaggerExplorer()
            .InjectDbContext(builder.Configuration)
            .AddAppConfig(builder.Configuration)
            .AddIdentityHandlersAndStores()
             .ConfigureIdentityOptions()
             .AddIdentityAuth(builder.Configuration);


var app = builder.Build();

app.ConfigureSwaggerExplorer();

app.ConfigCORS();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var apiGroup = app.MapGroup("/api");

// Identity endpoints under /api
apiGroup.MapIdentityApi<AppUser>();


apiGroup
    .MapIdentityUserEndPoints()
    .MapAccountEndPoints();

// Books under /api/books
apiGroup
    .MapGroup("/books")
    .MapBookEndPoints();



app.Run();

public class BookCreateModel
{
    public string Name { get; set; }
    public string ISBN { get; set; }
    public DateOnly DatePublished { get; set; }
    public string Author { get; set; }

    public string Genre { get; set; } // capture raw string from client
}

public class LoginModel
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class UserRegistrationModel
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
}
