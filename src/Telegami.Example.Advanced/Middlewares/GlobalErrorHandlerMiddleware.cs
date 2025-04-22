using Telegami.Middlewares;

namespace Telegami.Example.Advanced.Middlewares
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
                try
                {
                    await ctx.SendAsync("ERROR! " + e.Message);
                }
                catch
                {
                    // ignore
                }
            }
        }
    }
}
