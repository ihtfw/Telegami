using Telegami.Middlewares;

namespace Telegami.Demo.Console.Middlewares
{
    internal class GlobalErrorHandlerMiddleware : ITelegamiMiddleware
    {
        public async Task InvokeAsync(MessageContext ctx, MessageContextDelegate next)
        {
            try
            {
                await next(ctx);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                await ctx.ReplyAsync("error: " + e.Message);
            }
        }
    }
}
