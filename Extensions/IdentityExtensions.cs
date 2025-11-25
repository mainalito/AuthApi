using AuthApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace AuthApi.Extensions
{
    public static class IdentityExtensions
    {
        public static IServiceCollection
            AddIdentityHandlersAndStores(this IServiceCollection services)
        {
            services
                .AddIdentityApiEndpoints<AppUser>()
                .AddEntityFrameworkStores<AppDbContext>(); // Replace 'YourDbContext' with your actual DbContext

            return services;
        }
        public static IServiceCollection ConfigureIdentityOptions(this IServiceCollection services)
        {
            services.Configure<IdentityOptions>(options =>
            {
                options.User.RequireUniqueEmail = true;
            });
            return services;

        }
        public static IServiceCollection AddIdentityAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme =
                x.DefaultChallengeScheme =
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = false;
                options.TokenValidationParameters = new()
                {

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["AppSettings:JWTSecret"]!)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };

            });
            return services;

        }

    }
}
