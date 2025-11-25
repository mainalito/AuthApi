
using Microsoft.AspNetCore.Authorization;

namespace AuthApi.Controllers
{
    public static class AccountEndPoints
    {
        public static IEndpointRouteBuilder MapAccountEndPoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/UserProfile", GetUserProfile);
            return app;

        }

        [Authorize]
        private static string GetUserProfile()
        {
            return "User Profile Data";
        }
    }
}
