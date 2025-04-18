using Telegami;
using Telegami.Middlewares;

namespace TelegamiDemoBot.Middlewares;

public class LoggerMiddleware : ITelegamiMiddleware
{
    public async Task InvokeAsync(MessageContext ctx, MessageContextDelegate next)
    {
        System.Console.WriteLine($"[{ctx.Message.Chat.Id}] from {ctx.From} {ctx.Message.Text}");
        await next(ctx);
    }
}