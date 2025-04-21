namespace Telegami.MessageHandlers;

internal sealed class CommandMessageHandler : BaseMessageHandler
{
    public string Command { get; }

    public CommandMessageHandler(string command, Delegate handler, MessageHandlerOptions? options = null) : base(handler, options)
    {
        Command = command;
    }

    public override bool CanHandle(MessageContext ctx)
    {
        if (!ctx.IsCommand)
        {
            return false;
        }

        // if bot name is provided for command and it is not equal to the bot name in the context we should ignore it
        if (!string.IsNullOrEmpty(ctx.BotCommand!.BotName)
            && !string.IsNullOrEmpty(ctx.BotUser.Username)
            && !ctx.BotCommand.BotName.Equals(ctx.BotUser.Username, StringComparison.InvariantCultureIgnoreCase))
        {
            return false;
        }

        return Command.Equals(ctx.BotCommand!.Command, StringComparison.InvariantCultureIgnoreCase);
    }
}