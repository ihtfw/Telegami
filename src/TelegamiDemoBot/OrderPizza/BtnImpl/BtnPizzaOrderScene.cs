using System.Text;
using Telegami;
using Telegami.Controls.Buttons;
using Telegami.Scenes;
using Telegami.Sessions;
using TelegamiDemoBot.OrderPizza.TextImpl;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace TelegamiDemoBot.OrderPizza.BtnImpl
{
    internal class BtnPizzaOrderScene : Scene//, IHaveSubScenes
    {
        public const string SceneName = "BtnPizzaOrderScene";

        public BtnPizzaOrderScene() : base(SceneName)
        {
            var menu = new PizzaMenu();

            Enter(async ctx =>
            {
                var btns = new TelegamiButtons()
                    .AddButtonRow("Select pizza", "select")
                    .AddButtonRow("View basket", "basket")
                    .AddButtonRow("Confirm order", "confirm")
                    .AddButtonRow("Cancel order", "cancel")
                    ;

                var replyMarkup = btns.ToInlineButtonArray();
                await ctx.SendAsync("Let's order pizza!", replyMarkup:replyMarkup);
            });

            this.Command("cancel", async ctx => await ctx.LeaveSceneAsync());

            this.CallbackQuery("select", async ctx =>
            {
                await ctx.EnterSceneAsync(TextPizzaOrderSelectSubScene.SceneName);
            });

            this.CallbackQuery("basket", async ctx =>
            {
                var state = ctx.Session.Get<PizzaOrderState>() ?? new PizzaOrderState();
                if (state.Basket.Count == 0)
                {
                    await ctx.SendAsync("Your basket is empty.");
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
            });

            this.CallbackQuery("confirm", async ctx =>
            {
                var state = ctx.Session.Get<PizzaOrderState>() ?? new PizzaOrderState(); if (state.Basket.Count == 0)
                {
                    await ctx.SendAsync("Please select pizza!");
                    return;
                }

                await ctx.EnterSceneAsync(TextDeliveryDetailsSubScene.SceneName);
            });

            this.CallbackQuery("cancel", async ctx =>
            {
                await ctx.LeaveSceneAsync();
            });

            ReEnter(async ctx =>
            {
                var state = ctx.Session.Get<PizzaOrderState>() ?? new PizzaOrderState();
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

                var msg = """
                          Continue to order!
                          /select - to select a pizza
                          /basket - to view the basket
                          /confirm - to confirm the order
                          /cancel - to cancel the order
                          """;

                await ctx.SendAsync(msg);
            });

            Leave(async ctx =>
            {
                var msg = """
                          Thank you for using our pizza ordering service!
                          Have a great day!
                          """;
                await ctx.SendAsync(msg);
            });
            
            this.Command("state", async ctx =>
            {
                var state = ctx.Session.Get<PizzaOrderState>() ?? new PizzaOrderState(); 

                await ctx.SendAsync($"""
                                    ```json
                                    {Utils.ToJson(state)}
                                    ```
                                    """, ParseMode.MarkdownV2);
            });

//             this.On(async ctx =>
//             {
//                 await ctx.SendAsync($"""
//                                      ```json
//                                      {Utils.ToJson(ctx.Message)}
//                                      ```
//                                      """, ParseMode.MarkdownV2);
//             }, MessageHandlerOptions.PreventHandling);
        }

        // public IEnumerable<IScene> SubScenes()
        // {
        //     yield return new TextPizzaOrderSelectSubScene();
        //     yield return new TextDeliveryDetailsSubScene();
        // }
    }
}
