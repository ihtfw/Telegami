// namespace Telegami;
//
// public interface ITelegramBuilderStage
// {
//     /// <summary>
//     ///     Sets the bot token which will be used by the telegram bot client.
//     /// </summary>
//     /// <param name="token"></param>
//     /// <returns></returns>
//     TelegamiBotBuilder WithToken(string token);
// }
//
//
// public class TelegamiBotBuilder : ITelegramBuilderStage
// {
//     private string? _token;
//
//     private TelegamiBotBuilder()
//     {
//
//     }
//
//     public static ITelegramBuilderStage Create()
//     {
//         return new TelegamiBotBuilder();
//     }
//
//
//     public TelegamiBotBuilder WithToken(string token)
//     {
//         _token = token;
//         return this;
//     }
//
//
// }