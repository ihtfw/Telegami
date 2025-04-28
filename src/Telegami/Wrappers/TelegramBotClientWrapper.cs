// using Telegram.Bot;
// using Telegram.Bot.Requests.Abstractions;
// using Telegram.Bot.Types;
//
// namespace Telegami.Wrappers
// {
//     internal class InternalTelegramBotClient : TelegramBotClient
//     {
//         public InternalTelegramBotClient(TelegramBotClientOptions options, HttpClient? httpClient = null, CancellationToken cancellationToken = default) : base(options, httpClient, cancellationToken)
//         {
//         }
//
//         public InternalTelegramBotClient(string token, HttpClient? httpClient = null, CancellationToken cancellationToken = default) : base(token, httpClient, cancellationToken)
//         {
//         }
//
//         public override async Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
//         {
//             var response = await base.SendRequest(request, cancellationToken);
//             if (response is Message message)
//             {
//                 OnMessageSend?.Invoke(this, message);
//             }
//
//             return response;
//         }
//
//         public event EventHandler<Message>? OnMessageSend;
//     }
// }
