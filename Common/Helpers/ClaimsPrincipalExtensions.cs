using Common.DataBase.Entities;
using Common.Environment;
using System;
using System.Security.Claims;

namespace Common.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool IsAuthenticated(this ClaimsPrincipal claims)
        {
            return claims.Identity.IsAuthenticated;
        }
        public static string Id(this ClaimsPrincipal claims)
        {
            return claims.FindFirstValue(Claims.Id);
        }

        public static string Email(this ClaimsPrincipal claims)
        {
            return claims.FindFirstValue(Claims.Email);
        }

        public static string DisplayName(this ClaimsPrincipal claims)
        {
            return claims.FindFirstValue(Claims.DisplayName);
        }

        public static Role Role(this ClaimsPrincipal claims)
        {
            return Enum.Parse<Role>(claims.FindFirstValue(Claims.Role));
        }
    }
}
