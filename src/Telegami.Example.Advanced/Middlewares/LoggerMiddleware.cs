using Telegami.Middlewares;

namespace Telegami.Example.Advanced.Middlewares;

public class LoggerMiddleware : ITelegamiMiddleware
{
    public async Task InvokeAsync(MessageContext ctx, MessageContextDelegate next)
    {
        if (ctx.Update.CallbackQuery != null)
        {
            var jsonCallback = Telegami.Utils.ToJsonDebug(ctx.Update.CallbackQuery);
            Console.WriteLine(jsonCallback);
        } 

        var json = Telegami.Utils.ToJsonDebug(ctx.Message);
        Console.WriteLine(json);
        await next(ctx);
    }
}