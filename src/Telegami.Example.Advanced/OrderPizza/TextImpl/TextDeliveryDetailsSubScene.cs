using Telegami.Controls.DatePicker;
using Telegami.Scenes;
using Telegami.Sessions;

namespace Telegami.Example.Advanced.OrderPizza.TextImpl;

#pragma warning disable CS0618 // Type or member is obsolete
internal class DeliveryDatePickerWizardScene : DatePickerWizardScene
{
    public DeliveryDatePickerWizardScene()
    {
        Message = "Let's select delivery time";
        this.Leave(OnLeave);
    }

    private Task OnLeave(MessageContext ctx)
    {
        var pickerState = ctx.Session.GetOrDefault<DatePickerWizardSceneState>();
        if (pickerState == null || pickerState.Day == null || pickerState.Month == null || pickerState.Year == null)
            return Task.CompletedTask;

        ctx.Session.Update<PizzaOrderState>(s =>
        {
            s.DeliveryDate = new DateTime(pickerState.Year.Value, pickerState.Month.Value, pickerState.Day.Value);
        });

        return Task.CompletedTask;
    }
}
#pragma warning restore CS0618 // Type or member is obsolete

[SubScene(typeof(DeliveryDatePickerWizardScene))]
internal class TextDeliveryDetailsSubScene : WizardScene
{
    public TextDeliveryDetailsSubScene()
    {
        this.Leave(async ctx =>
        {
            await ctx.DeleteSceneBotMessagesAsync();
            await ctx.DeleteSceneUserMessagesAsync();
        });

        Add(DateStage);
        Add(AskNameStage);
        Add(SetNameStage);
        Add(SetPhoneStage);
        Add(SetDeliveryAddressStage);

        this.Command("back", async (ctx) => await ctx.LeaveSceneAsync());
    }

    private async Task DateStage(MessageContext ctx, WizardContext<PizzaOrderState> wiz)
    {
        if (wiz.State.DeliveryDate != null)
        {
            wiz.Next();
            wiz.ForceExecute();
            return;
        }

        await ctx.EnterSceneAsync<DeliveryDatePickerWizardScene>();
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