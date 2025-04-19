using Telegami;
using Telegami.Scenes;
using Telegami.Sessions;

namespace TelegamiDemoBot.OrderPizza.TextImpl;

internal class TextPizzaOrderSelectSubScene : Scene
{
    public const string SceneName = "TextPizzaOrderSelectSubScene";

    public TextPizzaOrderSelectSubScene(PizzaMenu menu) : base(SceneName)
    {
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
                var state = ctx.Session.Get<PizzaOrderState>() ?? new PizzaOrderState();

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