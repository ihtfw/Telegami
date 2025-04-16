using Telegami.Middlewares;

namespace Telegami.Demo.Console.Middlewares
{
    internal class GlobalErrorHandlerMiddleware : ITelegamiMiddleware
    {
        public async Task InvokeAsync(IMessageContext context, MessageContextDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                await context.ReplyAsync("error: " + e.Message);
            }
        }
    }

    internal class LoggerMiddleware : ITelegamiMiddleware
    {
        public async Task InvokeAsync(IMessageContext context, MessageContextDelegate next)
        {
            System.Console.WriteLine($"[{context.Message.Chat.Id}] {context.Message.Text}");
            await next(context);
        }
    }
}
