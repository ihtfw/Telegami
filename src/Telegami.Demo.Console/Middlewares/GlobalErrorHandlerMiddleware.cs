using Telegami.Middlewares;

namespace Telegami.Demo.Console.Middlewares
{
    internal class GlobalErrorHandlerMiddleware : ITelegamiMiddleware
    {
        public async Task InvokeAsync(IMessageContext ctx, MessageContextDelegate next)
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

    internal class LoggerMiddleware : ITelegamiMiddleware
    {
        public async Task InvokeAsync(IMessageContext ctx, MessageContextDelegate next)
        {
            System.Console.WriteLine($"[{ctx.Message.Chat.Id}] {ctx.Message.Text}");
            await next(ctx);
        }
    }
}
