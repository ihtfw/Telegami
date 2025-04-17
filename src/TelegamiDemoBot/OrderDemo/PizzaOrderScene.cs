using System.Text;
using Telegami;
using Telegami.Scenes;
using Telegami.Sessions;

namespace TelegamiDemoBot.OrderDemo
{
    internal class PizzaOrderScene : Scene, IHaveSubScenes
    {
        public const string SceneName = "order_pizza_scene";

        public PizzaOrderScene() : base(SceneName)
        {
            var menu = new PizzaMenu();

            Enter(async ctx =>
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

            ReEnter(async ctx =>
            {
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
            
            this.Command("select", async ctx =>
            {
                await ctx.EnterSceneAsync(PizzaOrderSelectSubScene.SceneName);
            });

            this.Command("basket", ctx =>
            {
                var state = ctx.Session.Get<PizzaOrderState>() ?? new PizzaOrderState();
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
                var state = ctx.Session.Get<PizzaOrderState>() ?? new PizzaOrderState();
                if (state.Basket.Count == 0)
                {
                    await ctx.SendAsync("Please select pizza!");
                    return;
                }
                
                await ctx.SendAsync("Your order is cooking now!");
                await ctx.LeaveSceneAsync();
            });

            this.Command("cancel", async ctx =>
            {
                await ctx.LeaveSceneAsync();
            });
        }

        public IEnumerable<IScene> SubScenes()
        {
            yield return new PizzaOrderSelectSubScene();
        }
    }
}
