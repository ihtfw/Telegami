namespace Telegami.Middlewares;

public interface ITelegamiMiddleware
{
    Task InvokeAsync(MessageContext ctx, MessageContextDelegate next);
}