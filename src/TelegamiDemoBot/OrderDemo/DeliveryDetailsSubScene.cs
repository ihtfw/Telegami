using Telegami;
using Telegami.Scenes;

namespace TelegamiDemoBot.OrderDemo;

internal class DeliveryDetailsSubScene : WizardScene
{
    public const string SceneName = "DeliveryDetailsSubScene";

    public DeliveryDetailsSubScene() : base(SceneName)
    {
        Add(AskNameStage);
        Add(SetNameStage);
        Add(SetPhoneStage);
        Add(SetDeliveryAddressStage);

        this.Command("back", async (ctx) => await ctx.LeaveSceneAsync());
    }

    private async Task AskNameStage(MessageContext ctx, WizardContext<PizzaOrderState> wiz)
    {
        await ctx.SendAsync("What's your name? (Use /back to return to order)");
        wiz.Next();
    }

    private async Task SetNameStage(MessageContext ctx, WizardContext<PizzaOrderState> wiz)
    {
        if (string.IsNullOrEmpty(ctx.Message.Text))
        {
            await ctx.SendAsync("Incorrect message, please send text as name");
            return;
        }

        wiz.State.Name = ctx.Message.Text;
        await ctx.SendAsync("What's your phone number? (Use /back to return to order)");
        wiz.Next();
    }

    private async Task SetPhoneStage(MessageContext ctx, WizardContext<PizzaOrderState> wiz)
    {
        if (string.IsNullOrEmpty(ctx.Message.Text))
        {
            await ctx.SendAsync("Incorrect message, please send text as phone number");
            return;
        }

        wiz.State.Phone = ctx.Message.Text;
        await ctx.SendAsync("What's your delivery address? (Use /back to return to order)");
        wiz.Next();
    }

    private async Task SetDeliveryAddressStage(MessageContext ctx, WizardContext<PizzaOrderState> wiz)
    {
        if (string.IsNullOrEmpty(ctx.Message.Text))
        {
            await ctx.SendAsync("Incorrect message, please send text as delivery address");
            return;
        }

        wiz.State.DeliveryAddress = ctx.Message.Text;
        wiz.State.IsOrderCompleted = true;

        // we have to call this manually to update the state, cause we are leaving before stage is completed
        // so it won't be updated automatically until stage is completed
        wiz.StateChanged();

        await ctx.LeaveSceneAsync();
    }
}