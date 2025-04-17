namespace TelegamiDemoBot.OrderDemo;

internal class PizzaOrderState
{
    public Dictionary<string, int> Basket { get; set; } = new();

    public void Add(PizzaItem item, int count)
    {
        if (Basket.TryGetValue(item.Key, out var currentCount))
        {
            Basket[item.Key] = currentCount + count;
        }
        else
        {
            Basket[item.Key] = count;
        }
    }
}