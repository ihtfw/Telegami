namespace Telegami.Middlewares;

public interface ITelegamiMiddleware
{
    Task InvokeAsync(IMessageContext context, MessageContextDelegate next);
}