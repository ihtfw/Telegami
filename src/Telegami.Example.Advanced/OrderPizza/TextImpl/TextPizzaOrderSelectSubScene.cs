﻿using Telegami.Scenes;
using Telegami.Sessions;

namespace Telegami.Example.Advanced.OrderPizza.TextImpl;

internal class TextPizzaOrderSelectSubScene : Scene
{
    public TextPizzaOrderSelectSubScene(PizzaMenu menu)
    {
        this.Leave(async ctx =>
        {
            await ctx.DeleteSceneBotMessagesAsync();
            await ctx.DeleteSceneUserMessagesAsync();
        });

        Enter(async (MessageContext ctx, PizzaMenu m) =>
        {
            var msg = $"""
                       Please select a pizza from the menu:
                       {m.ItemsAsCommands()}
                       /back - to go back to the main menu
                       """;
            await ctx.SendAsync(msg);
        });

        foreach (var pizzaItem in menu.Items)
        {
            this.Command(pizzaItem.Key, async ctx =>
            {
                var state = ctx.Session.Get<PizzaOrderState>();

                await ctx.SendAsync($"{pizzaItem.Name} Pizza was added to basket. Add more or use /back to return to main menu");

                state.Add(pizzaItem, 1);
                ctx.Session.Set(state);
            });
        }

        this.Command("back", async ctx =>
        {
            await ctx.LeaveSceneAsync();
        });
    }
}