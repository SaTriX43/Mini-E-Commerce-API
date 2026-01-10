using Microsoft.IdentityModel.Logging;
using Serilog.Context;
using System.Security.Claims;

namespace Mini_E_Commerce_API.Middleware
{
    public class LoggerIntercerptorRequestMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggerIntercerptorRequestMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            
            var requestPath = context.Request.Path;
            var method = context.Request.Method;
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var rol = context.User.FindFirst(ClaimTypes.Role)?.Value;

            int? userId = null;

            if (!string.IsNullOrWhiteSpace(userIdClaim) && int.TryParse(userIdClaim, out var parsedUserId))
            {
                userId = parsedUserId;
            }

            using (LogContext.PushProperty("RequestPath", requestPath))
            using (LogContext.PushProperty("Method", method))
            using (LogContext.PushProperty("UserIdClaim", userId))
            using (LogContext.PushProperty("Rol", rol))
            {
                await _next(context);
            }


        }
    }
}
