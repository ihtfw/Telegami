using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegami.Extensions
{
    public static class TelegramClientEx
    {
        /// <summary>Use this method to send long(more then 4096 chars, will split automatically) text messages. Will automatically Retry if a limit has been exceeded.</summary>
        /// <param name="client">An instance of <see cref="ITelegramBotClient"/></param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="text">Text of the message to be sent, 1-4096 characters after entities parsing</param>
        /// <param name="parseMode">Mode for parsing entities in the message text. See <a href="https://core.telegram.org/bots/api#formatting-options">formatting options</a> for more details.</param>
        /// <param name="replyParameters">Description of the message to reply to</param>
        /// <param name="replyMarkup">Additional interface options. An object for an <a href="https://core.telegram.org/bots/features#inline-keyboards">inline keyboard</a>, <a href="https://core.telegram.org/bots/features#keyboards">custom reply keyboard</a>, instructions to remove a reply keyboard or to force a reply from the user</param>
        /// <param name="linkPreviewOptions">Link preview generation options for the message</param>
        /// <param name="messageThreadId">Unique identifier for the target message thread (topic) of the forum; for forum supergroups only</param>
        /// <param name="entities">A list of special entities that appear in message text, which can be specified instead of <paramref name="parseMode"/></param>
        /// <param name="disableNotification">Sends the message <a href="https://telegram.org/blog/channels-2-0#silent-messages">silently</a>. Users will receive a notification with no sound.</param>
        /// <param name="protectContent">Protects the contents of the sent message from forwarding and saving</param>
        /// <param name="messageEffectId">Unique identifier of the message effect to be added to the message; for private chats only</param>
        /// <param name="businessConnectionId">Unique identifier of the business connection on behalf of which the message will be sent</param>
        /// <param name="allowPaidBroadcast">Pass <see langword="true"/> to allow up to 1000 messages per second, ignoring <a href="https://core.telegram.org/bots/faq#how-can-i-message-all-of-my-bot-39s-subscribers-at-once">broadcasting limits</a> for a fee of 0.1 Telegram Stars per message. The relevant Stars will be withdrawn from the bot's balance</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
        /// <returns>The sent <see cref="Message"/> is returned.</returns>
        public static async Task<Message[]> SendLongMessage(this ITelegramBotClient client,
            ChatId chatId,
            string text,
            ParseMode parseMode = default,
            ReplyParameters? replyParameters = null,
            ReplyMarkup? replyMarkup = null,
            LinkPreviewOptions? linkPreviewOptions = null,
            int? messageThreadId = null,
            IEnumerable<MessageEntity>? entities = null,
            bool disableNotification = false,
            bool protectContent = false,
            string? messageEffectId = null,
            string? businessConnectionId = null,
            bool allowPaidBroadcast = false,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(text))
            {
                return Array.Empty<Message>();
            }

            var count = (int)Math.Ceiling(text.Length / 4096.0);
            if (count == 1)
            {
                // no need to Chuck
                var message = await client.SendMessage(chatId, text, parseMode, replyParameters, replyMarkup,
                    linkPreviewOptions, messageThreadId, entities, disableNotification, protectContent, messageEffectId,
                    businessConnectionId, allowPaidBroadcast, cancellationToken);
                return [message];
            }

            var result = new Message[count];

            var index = 0;
            foreach (var chunk in text.Chunk(4096))
            {
                var messageChunk = new string(chunk);

                var message = await client.SendMessage(chatId, messageChunk, parseMode, replyParameters, replyMarkup,
                    linkPreviewOptions, messageThreadId, entities, disableNotification, protectContent, messageEffectId,
                    businessConnectionId, allowPaidBroadcast, cancellationToken);

                result[index] = message;

                // set to null, so we send them only with the first message
                entities = null;
                index++;
            }

            return result;
        }

        /// <summary> 
        /// This will call a function on the TelegramClient and automatically Retry if a limit has been exceeded.
        /// If RetryAfter exception then rethrow
        /// If exceeds maxNumberOfRetries then rethrow
        /// </summary>
        [Obsolete("Auto retry is already implemented in Telegram.Bot client library, so we don't need to repeat it here", true)]
        public static async Task Call(this ITelegramBotClient client, Func<ITelegramBotClient, Task> call, int maxNumberOfRetries = 5)
        {
            Exception? lastException = null;
            var numberOfTries = 0;
            while (numberOfTries < maxNumberOfRetries)
            {
                try
                {
                    await call(client);
                    return;
                }
                catch (ApiRequestException ex)
                {
                    lastException = ex;
                    if (ex.ErrorCode != 429)
                    {
                        throw;
                    }

                    if (ex.Parameters != null && ex.Parameters.RetryAfter != null)
                    {
                        await Task.Delay(ex.Parameters.RetryAfter.Value * 1000);
                    }

                    numberOfTries++;
                }
            }

            if (lastException != null)
            {
                throw lastException;
            }

            throw new ApplicationException($"Max number of retries ({maxNumberOfRetries}) exceeded.");
        }

        /// <summary>
        /// This will call a function on the TelegramClient and automatically Retry if a limit has been exceeded.
        /// If RetryAfter exception then rethrow
        /// If exceeds maxNumberOfRetries then rethrow
        /// </summary>
        [Obsolete("Auto retry is already implemented in Telegram.Bot client library, so we don't need to repeat it here", true)]
        public static async Task<TReturn> Call<TReturn>(this ITelegramBotClient client, Func<ITelegramBotClient, Task<TReturn>> call, int maxNumberOfRetries = 5)
        {
            Exception? lastException = null;
            var numberOfTries = 0;
            while (numberOfTries < maxNumberOfRetries)
            {
                try
                {
                    return await call(client);
                }
                catch (ApiRequestException ex)
                {
                    if (ex.ErrorCode != 429)
                    {
                        throw;
                    }

                    lastException = ex;
                    if (ex.Parameters != null && ex.Parameters.RetryAfter != null)
                    {
                        await Task.Delay(ex.Parameters.RetryAfter.Value * 1000);
                    }

                    numberOfTries++;
                }
            }

            if (lastException != null)
            {
                throw lastException;
            }

            throw new ApplicationException($"Max number of retries ({maxNumberOfRetries}) exceeded.");
        }
    }
}
