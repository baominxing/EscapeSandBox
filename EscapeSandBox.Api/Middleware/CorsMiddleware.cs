using Microsoft.AspNetCore.Cors.Infrastructure;

namespace EscapeSandBox.Api.Middleware
{
    public class CorsMiddleware
    {
        private readonly RequestDelegate next;

        public CorsMiddleware(RequestDelegate next)
        {
            this.next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.ContainsKey(CorsConstants.Origin))
            {
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.Headers.Add("Access-Control-Allow-Methods", "*");
                context.Response.Headers.Add("Access-Control-Allow-Headers", "*");
                context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");

                if (context.Request.Method.Equals("OPTIONS"))
                {
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    return;
                }
            }

            await next(context);
        }
    }
}
