using System.Text.RegularExpressions;
using Telegami.Controls.Buttons;
using Telegami.Scenes;
using Telegami.Sessions;

namespace Telegami.Example.Advanced.OrderPizza.BtnImpl;

internal class BtnPizzaOrderSelectSubScene : Scene
{
    public BtnPizzaOrderSelectSubScene()
    {
        Enter(AskPizza);

        var pizzaMatch = new Regex("^pizza_(?<key>.+)$");
        CallbackQuery(pizzaMatch, async (MessageContext ctx, PizzaMenu menu) =>
        {
            var match = pizzaMatch.Match(ctx.Update.CallbackQuery!.Data!);
            var key = match.Groups["key"].Value;
            var pizzaItem = menu.Get(key);

            await ctx.SendAsync($"Add {pizzaItem.Name} to basket!");

            var state = ctx.Session.Get<PizzaOrderState>();
            state.Add(pizzaItem, 1);
            ctx.Session.Set(state);

            await AskPizza(ctx, menu);
        });

        this.CallbackQuery("back", async ctx =>
        {
            await ctx.LeaveSceneAsync();
        });
    }

    private async Task AskPizza(MessageContext ctx, PizzaMenu menu)
    {
        var msg = "What pizza do you want?";

        var btns = new TelegamiButtons();
        foreach (var pizzaItem in menu.Items)
        {
            btns.Add(pizzaItem.Name, "pizza_" + pizzaItem.Key);
        }
        btns.Add("Back", "back");

        var keyboardButtons = btns.ToInlineButtons();
        await ctx.SendAsync(msg, replyMarkup: keyboardButtons);
    }
}