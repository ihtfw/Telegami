using System.Text;
using Telegami.Controls.Buttons;
using Telegami.Example.Advanced.OrderPizza.TextImpl;
using Telegami.Scenes;
using Telegami.Sessions;
using Telegram.Bot.Types.Enums;

namespace Telegami.Example.Advanced.OrderPizza.BtnImpl
{
    [SubScene(BtnPizzaOrderSelectSubScene.SceneName, typeof(BtnPizzaOrderSelectSubScene))]
    [SubScene(TextDeliveryDetailsSubScene.SceneName, typeof(TextDeliveryDetailsSubScene))]
    internal class BtnPizzaOrderScene : Scene
    {
        public const string SceneName = "BtnPizzaOrderScene";

        public BtnPizzaOrderScene() : base(SceneName)
        {
            this.Enter(async ctx =>
            {
                var btns = new TelegamiButtons
                {
                    ("Select pizza", "select"),
                    ("View basket", "basket"),
                    { ("Confirm order", "confirm"), ("Cancel order", "cancel") }
                };

                var replyMarkup = btns.ToInlineButtons();
                await ctx.SendAsync("Let's order pizza!", replyMarkup: replyMarkup);
            });

            ReEnter(async (MessageContext ctx, PizzaMenu menu) =>
            {
                var state = ctx.Session.Get<PizzaOrderState>();
                if (state.IsOrderCompleted)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("Your basket:");
                    foreach (var item in state.Basket)
                    {
                        var pizzaItem = menu.Get(item.Key);
                        sb.AppendLine($"{pizzaItem.Name} - {item.Value} pcs");
                    }

                    await ctx.SendAsync(
                        $"{state.Name}, we are already preparing your order:\n{sb}\n\nYour order will be delivered to {state.DeliveryAddress}.");
                    await ctx.LeaveSceneAsync();
                    return;
                }

                await SendContinueToOrder(ctx);
            });

            this.Command("cancel", async ctx => await ctx.LeaveSceneAsync());

            this.CallbackQuery("select",
                async ctx => { await ctx.EnterSceneAsync(BtnPizzaOrderSelectSubScene.SceneName); });

            this.CallbackQuery("basket", async (MessageContext ctx, PizzaMenu menu) =>
            {
                var state = ctx.Session.Get<PizzaOrderState>();
                if (state.Basket.Count == 0)
                {
                    await ctx.SendAsync("Your basket is empty.");
                    await SendContinueToOrder(ctx);
                    return;
                }

                var sb = new StringBuilder();
                sb.AppendLine("Your basket:");
                foreach (var item in state.Basket)
                {
                    var pizzaItem = menu.Get(item.Key);
                    sb.AppendLine($"{pizzaItem.Name} - {item.Value} pcs");
                }

                sb.AppendLine($"Total: ${state.Basket.Sum(i => menu.Get(i.Key).Price * i.Value)}");
                await ctx.SendAsync(sb.ToString());

                await SendContinueToOrder(ctx);
            });

            this.CallbackQuery("confirm", async ctx =>
            {
                var state = ctx.Session.Get<PizzaOrderState>();
                if (state.Basket.Count == 0)
                {
                    await ctx.SendAsync("Please select pizza!");
                    await SendContinueToOrder(ctx);
                    return;
                }

                await ctx.EnterSceneAsync(TextDeliveryDetailsSubScene.SceneName);
            });

            this.CallbackQuery("cancel", async ctx => { await ctx.LeaveSceneAsync(); });

            this.Leave(async ctx =>
            {
                var msg = """
                          Thank you for using our pizza ordering service!
                          Have a great day!
                          """;
                await ctx.SendAsync(msg);
            });

            this.Command("state", async ctx =>
            {
                var state = ctx.Session.Get<PizzaOrderState>();

                await ctx.SendAsync($"""
                                     ```json
                                     {Telegami.Utils.ToJsonDebug(state)}
                                     ```
                                     """, ParseMode.MarkdownV2);
            });
        }

        private static async Task SendContinueToOrder(MessageContext ctx)
        {
            var btns = new TelegamiButtons
            {
                ("Select pizza", "select"),
                ("View basket", "basket"),
                { ("Confirm order", "confirm"), ("Cancel order", "cancel") }
            };

            var replyMarkup = btns.ToInlineButtons();
            await ctx.SendAsync("Continue to order.", replyMarkup: replyMarkup);
        }
    }
}