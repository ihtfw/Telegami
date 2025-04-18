using System.Text;

namespace TelegamiDemoBot.OrderPizza;

internal class PizzaMenu
{
    public List<PizzaItem> Items { get; } = new()
    {
        new PizzaItem("margherita", "Margherita", 10),
        new PizzaItem("pepperoni", "Pepperoni", 12),
        new PizzaItem("vegetarian", "Vegetarian", 11),
        new PizzaItem("bbqchicken", "BBQ Chicken", 13)
    };

    public string ItemsAsCommands()
    {
        var sb = new StringBuilder();
        foreach (var item in Items)
        {
            var price = item.Price;
            var name = item.Name;
            var key = item.Key;
            sb.AppendLine($"/{key} - {name} - ${price}");
        }

        return sb.ToString();
    }

    public PizzaItem Get(string key)
    {
        return Items.First(x => x.Key == key);
    }
}