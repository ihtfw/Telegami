using Telegami.Controls.Buttons;
using Telegami.Extensions;
using Telegami.MessageHandlers;
using Telegami.Scenes;
using Telegami.Sessions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Telegami.Example.Advanced.ImageCarousel
{
    public class ImageCarouselState
    {
        public int Index { get; set; }

        public int? ImageMessageId { get; set; }
    }

    public class ImageCarouselScene : Scene
    {
        public const string SceneName = "ImageCarouselScene";

        public ImageCarouselScene() : base(SceneName)
        {
            Enter(Render);

            CallbackQuery("prev", HandlePrev, MessageHandlerOptions.PreventRemoveMarkupOptions);
            CallbackQuery("done", HandleDone);
            CallbackQuery("next", HandleNext, MessageHandlerOptions.PreventRemoveMarkupOptions);
        }

        public IReadOnlyList<string> Images { get; set; } = Array.Empty<string>();
        
        public Task HandlePrev(MessageContext ctx)
        {
            var state = ctx.Session.Get<ImageCarouselState>();
            state.Index--;
            if (state.Index < 0)
            {
                state.Index = Images.Count - 1;
            }
            ctx.Session.Set(state);

            return Render(ctx);
        }

        public Task HandleNext(MessageContext ctx)
        {
            var state = ctx.Session.Get<ImageCarouselState>();
            state.Index++;
            if (state.Index >= Images.Count)
            {
                state.Index = 0;
            }
            ctx.Session.Set(state);

            return Render(ctx);
        }

        public async Task HandleDone(MessageContext ctx)
        {
            await ctx.LeaveSceneAsync();
        }

        public async Task Render(MessageContext ctx)
        {
            var state = ctx.Session.Get<ImageCarouselState>();
            var imageFilePath = Images[state.Index];

            await using var fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            var inputFile = InputFile.FromStream(fileStream);

            var btns = new TelegamiButtons
            {
                {("⬅️", "prev"), ("✅", "done"), ("➡️", "next") }
            };

            var inlineButtons = btns.ToInlineButtons();

            if (state.ImageMessageId != null)
            {
                var media = new InputMediaPhoto(inputFile);
                await ctx.Bot.Client.EditMessageMedia(ctx.Chat.Id, state.ImageMessageId.Value, media, replyMarkup: inlineButtons);
            }
            else
            {
                var message = await ctx.Bot.Client.SendPhoto(ctx.Chat.Id, inputFile, messageThreadId: ctx.Message.ResolveMessageThreadId(), replyMarkup: inlineButtons);
                state.ImageMessageId = message.Id;

                ctx.Session.Set(state);
            }
        }
    }
}
