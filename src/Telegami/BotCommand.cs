namespace Telegami
{
    public class BotCommand
    {
        public string Command { get; }
        public string BotName { get; }
        public string Arguments { get; }

        private BotCommand(string command, string botName, string arguments)
        {
            Command = command;
            BotName = botName;
            Arguments = arguments;
        }

        public static bool TryParse(string? text, out BotCommand botCommand)
        {
            botCommand = null!;
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            if (!text.StartsWith("/"))
            {
                return false;
            }

            var parts = text.Split(new[] { ' ' }, 2);
            var command = parts[0].TrimStart('/').ToLowerInvariant();
            var arguments = parts.Length > 1 ? parts[1] : string.Empty;
            if (string.IsNullOrWhiteSpace(command))
            {
                return false;
            }

            var commandParts = command.Split(new[] { '@' }, 2);
            var commandName = commandParts[0];
            var botName = commandParts.Length > 1 ? commandParts[1] : string.Empty;

            if (string.IsNullOrWhiteSpace(commandName))
            {
                return false;
            }

            botCommand = new BotCommand(commandName, botName, arguments);
            return true;
        }
    }
}
