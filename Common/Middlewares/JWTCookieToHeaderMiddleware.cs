using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using Common.Environment;

namespace Common.Middlewares
{
    public class JWTCookieToHeaderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtSettings _jwtSettings;
        public JWTCookieToHeaderMiddleware(RequestDelegate next, JwtSettings jwtSettings)
        {
            _next = next;
            _jwtSettings = jwtSettings;
        }

        public async Task Invoke(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"];
            if (string.IsNullOrWhiteSpace(authHeader.ToString()))
            {
                var name = _jwtSettings.CookieName;
                var cookie = context.Request.Cookies[name];

                if (cookie != null)
                    context.Request.Headers.Append("Authorization", "Bearer " + cookie);
            }                    

            await _next.Invoke(context);
        }
    }
}
