using Telegami.Middlewares;

namespace Telegami.Demo.Console.Middlewares;

internal class LoggerMiddleware : ITelegamiMiddleware
{
    public async Task InvokeAsync(MessageContext ctx, MessageContextDelegate next)
    {
        System.Console.WriteLine($"[{ctx.Message.Chat.Id}] {ctx.Message.Text}");
        await next(ctx);
    }
}