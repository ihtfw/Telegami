namespace Telegami.Commands
{
    public interface ITelegamiCommandHandler
    {
        Task HandleAsync(MessageContext ctx);
    }
}
