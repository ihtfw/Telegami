namespace Telegami.Example.Advanced.OrderPizza;

internal class PizzaOrderState
{
    public Dictionary<string, int> Basket { get; set; } = new();

    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? DeliveryAddress { get; set; }
    public bool IsOrderCompleted { get; set; }

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