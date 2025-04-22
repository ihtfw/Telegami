using Telegami.Middlewares;

namespace Telegami.Example.Advanced.Middlewares;

public class LoggerMiddleware : ITelegamiMiddleware
{
    public async Task InvokeAsync(MessageContext ctx, MessageContextDelegate next)
    {
        if (ctx.Update.CallbackQuery != null)
        {
            var jsonCallback = Utils.ToJson(ctx.Update.CallbackQuery);
            Console.WriteLine(jsonCallback);
        } 

        var json = Utils.ToJson(ctx.Message);
        Console.WriteLine(json);
        await next(ctx);
    }
}