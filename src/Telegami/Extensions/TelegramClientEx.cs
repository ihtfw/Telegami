using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace Telegami.Extensions
{
    public static class TelegramClientEx
    {
        /// <summary>
        /// This will call a function on the TelegramClient and automatically Retry if a limit has been exceeded.
        /// If RetryAfter exception then rethrow
        /// If exceeds maxNumberOfRetries then rethrow
        /// </summary>
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
