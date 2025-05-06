using Telegami.Middlewares;

namespace Telegami.Example.SeeInAction.Middlewares
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
                Console.WriteLine(e);
                await ctx.ReplyAsync("error: " + e.Message);
            }
        }
    }
}
