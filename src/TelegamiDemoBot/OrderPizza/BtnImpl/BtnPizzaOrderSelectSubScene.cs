using System.Text.RegularExpressions;
using Telegami;
using Telegami.Controls.Buttons;
using Telegami.Scenes;
using Telegami.Sessions;

namespace TelegamiDemoBot.OrderPizza.BtnImpl;

internal class BtnPizzaOrderSelectSubScene : Scene
{
    public const string SceneName = "BtnPizzaOrderSelectSubScene";

    public BtnPizzaOrderSelectSubScene() : base(SceneName)
    {
        Enter(AskPizza);

        var pizzaMatch = new Regex("^pizza_(?<key>.+)$");
        this.CallbackQuery(pizzaMatch, async (MessageContext ctx, PizzaMenu menu) =>
        {
            var match = pizzaMatch.Match(ctx.Update.CallbackQuery!.Data!);
            var key = match.Groups["key"].Value;
            var pizzaItem = menu.Get(key);

            await ctx.SendAsync($"Add {pizzaItem.Name} to basket!");

            var state = ctx.Session.Get<PizzaOrderState>() ?? new PizzaOrderState();
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
            btns.AddButtonRow(pizzaItem.Name, "pizza_" + pizzaItem.Key);
        }
        btns.AddButtonRow("Back", "back");

        await ctx.SendAsync(msg, replyMarkup: btns.ToInlineButtonArray());
    }
}