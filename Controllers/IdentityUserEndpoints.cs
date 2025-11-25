using AuthApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace AuthApi.Controllers
{
    public static class IdentityUserEndpoints
    {
        public static IEndpointRouteBuilder MapIdentityUserEndPoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("signup", CreateUser);

            app.MapPost("signin", SignIn
                );


            return app;

        }


        private static async Task<IResult> CreateUser(UserManager<AppUser> userManager,
    [FromBody] UserRegistrationModel userRegistrationModel)
        {
            AppUser user = new AppUser
            {
                UserName = userRegistrationModel.Email,   // REQUIRED
                Email = userRegistrationModel.Email,
                FullName = userRegistrationModel.FullName
            };
            var result = await userManager.CreateAsync(user, userRegistrationModel.Password);
            if (result.Succeeded)
            {
                return Results.Ok("User registered successfully");
            }
            else
            {
                return Results.BadRequest(result.Errors);
            }
        }
        private static async Task<IResult> SignIn(UserManager<AppUser> userManager,
                [FromBody] LoginModel loginModel, IOptions<AppSettings> appSettings)
        {
            var user = await userManager.FindByEmailAsync(loginModel.Email);
            if (user != null && await userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                var signInKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(appSettings.Value.JWTSecret));
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
                    {
                        new Claim("UserID", user.Id.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddDays(10),
                    SigningCredentials = new SigningCredentials(signInKey, SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);

                return Results.Ok(new { token });
            }
            else
            {
                return Results.BadRequest("Invalid email or password");
            }

        }
    }
}
