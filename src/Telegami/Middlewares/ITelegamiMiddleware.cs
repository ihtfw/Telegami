namespace Telegami.Middlewares;

public interface ITelegamiMiddleware
{
    Task InvokeAsync(IMessageContext ctx, MessageContextDelegate next);
}