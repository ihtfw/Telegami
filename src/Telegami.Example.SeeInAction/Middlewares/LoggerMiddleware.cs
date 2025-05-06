using Telegami.Middlewares;

namespace Telegami.Example.SeeInAction.Middlewares;

internal class LoggerMiddleware : ITelegamiMiddleware
{
    public async Task InvokeAsync(MessageContext ctx, MessageContextDelegate next)
    {
        Console.WriteLine($"[{ctx.Message.Chat.Id}] {ctx.Message.Text}");
        await next(ctx);
    }
}