﻿using System.Text;
using Telegami.Scenes;
using Telegami.Sessions;
using Telegram.Bot.Types.Enums;

namespace Telegami.Example.Advanced.OrderPizza.TextImpl
{
    [SubScene(typeof(TextPizzaOrderSelectSubScene))]
    [SubScene(typeof(TextDeliveryDetailsSubScene))]
    internal class TextPizzaOrderScene : Scene
    {
        public TextPizzaOrderScene()
        {
            this.Enter(async ctx =>
            {
                var msg = """
                          Let's order pizza!
                          /select - to select a pizza
                          /basket - to view the basket
                          /confirm - to confirm the order
                          /cancel - to cancel the order
                          """;

                await ctx.SendAsync(msg);
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

                var msg = """
                          Continue to order!
                          /select - to select a pizza
                          /basket - to view the basket
                          /confirm - to confirm the order
                          /cancel - to cancel the order
                          """;
                
                await ctx.SendAsync(msg);
            });

            this.Leave(async ctx =>
            {
                await ctx.DeleteSceneMessagesAsync();

                var msg = """
                          Thank you for using our pizza ordering service!
                          Have a great day!
                          """;
                await ctx.SendAsync(msg);
            });

            this.Command("select", async ctx =>
            {
                await ctx.EnterSceneAsync<TextPizzaOrderSelectSubScene>();
                await ctx.DeleteUserMessageAsync();
            });

            Command("basket", (MessageContext ctx, PizzaMenu menu) =>
            {
                var state = ctx.Session.Get<PizzaOrderState>();
                if (state.Basket.Count == 0)
                {
                    return ctx.SendAsync("Your basket is empty.");
                }

                var sb = new StringBuilder();
                sb.AppendLine("Your basket:");
                foreach (var item in state.Basket)
                {
                    var pizzaItem = menu.Get(item.Key);
                    sb.AppendLine($"{pizzaItem.Name} - {item.Value} pcs");
                }
                sb.AppendLine($"Total: ${state.Basket.Sum(i => menu.Get(i.Key).Price * i.Value)}");
                return ctx.SendAsync(sb.ToString());
            });

            this.Command("confirm", async ctx =>
            {
                var state = ctx.Session.Get<PizzaOrderState>();
                if (state.Basket.Count == 0)
                {
                    await ctx.SendAsync("Please select pizza!");
                    return;
                }

                await ctx.EnterSceneAsync<TextDeliveryDetailsSubScene>();
            });

            this.Command("cancel", async ctx =>
            {
                await ctx.LeaveSceneAsync();
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
    }
}
