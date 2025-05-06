using Telegami.Controls.Buttons;
using Telegami.Scenes;

namespace Telegami.Controls.DatePicker;

/// <summary>
/// Not production ready, proof of concept.
/// </summary>
[Obsolete("Warning! Proof of concept. Can be removed or changed in any way")]
public class DatePickerWizardScene : WizardScene
{
    private const int YearBatch = 16;

    public DatePickerWizardScene(params Delegate[] stages) : base(stages)
    {
        Add(AskYear, nameof(AskYear));
        Add(GetYear, nameof(GetYear));

        Add(AskMonth, nameof(AskMonth));
        Add(GetMonth, nameof(GetMonth));

        Add(AskDay, nameof(AskDay));
        Add(GetDay, nameof(GetDay));
    }

    public int MinYear { get; set; } = 1900;
    public int MaxYear { get; set; } = DateTime.Now.Year + 100;

    public string Message { get; set; } = "";

    public bool AllowCancellation { get; set; } = true;

    public async Task AskYear(MessageContext ctx, WizardContext<DatePickerWizardSceneState> wiz)
    {
        if (wiz.State.Year != null)
        {
            wiz.Set(nameof(AskMonth));
            wiz.ForceExecute();
            return;
        }

        var now = DateTime.Now;
        var index = wiz.State.YearOffset;
        var endYear = now.Year + index;
        if (endYear > MaxYear)
        {
            endYear = MaxYear;
        }
        else if (endYear < MinYear)
        {
            endYear = MinYear;
        }

        var btns = new TelegamiButtons();

        /*
             * four rows of years i.e.
             * 2011 2012 2013 2014
             * 2015 2016 2017 2018
             * 2019 2020 2021 2022
             * 2023 2024 2025 2026
             * ⬅️ ❌ ➡️
             */
        var years = Enumerable.Range(endYear - YearBatch, YearBatch).Where(y => y > MinYear).ToList();
        if (years.Count == 0)
        {
            years = Enumerable.Range(MinYear, YearBatch).ToList();
        }

        foreach (var yearsRows in years.Chunk(4))
        {
            var row = new TelegamiButtonRow();
            foreach (var year in yearsRows)
            {
                row.Add(year.ToString(), $"year_{year}");
            }
            btns.Add(row);
        }
            
        if (AllowCancellation)
        {
            btns.Add(("⬅️", "prev"), ("❌", "cancel"), ("➡️", "next"));
        }
        else
        {
            btns.Add(("⬅️", "prev"), ("➡️", "next"));
        }

        await ctx.SendAsync(WithMessage("Select year"), replyMarkup: btns.ToInlineButtons());
        wiz.Next();
    }

    private string WithMessage(string subMessage)
    {
        if (string.IsNullOrEmpty(Message))
        {
            return subMessage;
        }

        return $"{Message}\n{subMessage}";
    }

    public async Task GetYear(MessageContext ctx, WizardContext<DatePickerWizardSceneState> wiz)
    {
        if (!ctx.IsCallbackQuery)
        {
            // let's allow user to enter a year by text, i.e. 2025
            var msg = ctx.Message.Text;
            if (int.TryParse(msg, out var year) && year >= MinYear && year <= MaxYear)
            {
                wiz.State.Year = year;
                wiz.Set(nameof(AskMonth));
                wiz.ForceExecute();
                return;
            }

            wiz.Prev();
            wiz.ForceExecute();
            return;
        }

        switch (ctx.CallbackData)
        {
            case "cancel":
                await ctx.LeaveSceneAsync();
                return;
            case "prev":
                wiz.State.YearOffset -= YearBatch;
                wiz.Prev();
                wiz.ForceExecute();
                return;
            case "next":
                wiz.State.YearOffset += YearBatch;
                wiz.Prev();
                wiz.ForceExecute();
                return;
        }

        if (ctx.CallbackData!.StartsWith("year_"))
        {
            var year = int.Parse(ctx.CallbackData.Substring(5));
            wiz.State.Year = year;
            wiz.Set(nameof(AskMonth));
            wiz.ForceExecute();
            return;
        }

        await ctx.SendAsync("Something went wrong on Year. Cancelling date selection.");
        await ctx.LeaveSceneAsync();
    }

    public async Task AskMonth(MessageContext ctx, WizardContext<DatePickerWizardSceneState> wiz)
    {
        if (wiz.State.Month != null)
        {
            wiz.Set(nameof(AskDay));
            wiz.ForceExecute();
            return;
        }

        var btns = new TelegamiButtons()
        {
            {("January", "january"), ("February", "february"), ("March", "march")},
            {("April", "april"), ("May", "may"), ("June", "june")},
            {("July", "july"), ("August", "august"), ("September", "september")},
            {("October", "october"), ("November", "november"), ("December", "december")}
        };

        await ctx.SendAsync(WithMessage("Select month"), replyMarkup: btns.ToInlineButtons());
        wiz.Next();
    }

    public int MonthToNumber(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return -1;
        }

        switch (input.ToLowerInvariant().Trim())
        {
            case "january":
            case "jan":
                return 1;
            case "february":
            case "feb":
                return 2;
            case "march":
            case "mar":
                return 3;
            case "april":
            case "apr":
                return 4;
            case "may":
                return 5;
            case "june":
            case "jun":
                return 6;
            case "july":
            case "jul":
                return 7;
            case "august":
            case "aug":
                return 8;
            case "september":
            case "sep":
                return 9;
            case "october":
            case "oct":
                return 10;
            case "november":
            case "nov":
                return 11;
            case "december":
            case "dec":
                return 12;
            default:
                return -1;
        }
    }

    public async Task GetMonth(MessageContext ctx, WizardContext<DatePickerWizardSceneState> wiz)
    {
        if (!ctx.IsCallbackQuery)
        {
            var userMonth = MonthToNumber(ctx.Message.Text);
            if (userMonth > 0)
            {
                wiz.State.Month = userMonth;
                wiz.Set(nameof(AskDay));
                wiz.ForceExecute();
                return;
            }

            wiz.Prev();
            wiz.ForceExecute();
            return;
        }

        switch (ctx.CallbackData)
        {
            case "back":
                wiz.Set(nameof(AskYear));
                wiz.ForceExecute();
                return;
            case "cancel":
                await ctx.LeaveSceneAsync();
                return;
        }

        var month = MonthToNumber(ctx.CallbackData);
        if (month > 0)
        {
            wiz.State.Month = month;
            wiz.Set(nameof(AskDay));
            wiz.ForceExecute();
            return;
        }

        await ctx.SendAsync("Something went wrong on Month. Cancelling date selection.");
        await ctx.LeaveSceneAsync();
    }

    public async Task AskDay(MessageContext ctx, WizardContext<DatePickerWizardSceneState> wiz)
    {
        if (wiz.State.Day != null)
        {
            await ctx.LeaveSceneAsync();
            return;
        }

        var daysInMonth = DateTime.DaysInMonth(wiz.State.Year!.Value, wiz.State.Month!.Value);

        var btns = new TelegamiButtons();
        foreach (var daysRow in Enumerable.Range(1, daysInMonth).Chunk(5))
        {
            var row = new TelegamiButtonRow();
            foreach (var day in daysRow)
            {
                row.Add(day.ToString(), $"day_{day}");
            }
            btns.Add(row);
        }

        await ctx.SendAsync(WithMessage("Select day"), replyMarkup: btns.ToInlineButtons());
        wiz.Next();
    }

    public async Task GetDay(MessageContext ctx, WizardContext<DatePickerWizardSceneState> wiz)
    {
        if (!ctx.IsCallbackQuery)
        {
            var daysInMonth = DateTime.DaysInMonth(wiz.State.Year!.Value, wiz.State.Month!.Value);

            // let's allow user to enter a year by text, i.e. 2025
            var msg = ctx.Message.Text;
            if (int.TryParse(msg, out var day) && day >= 1 && day <= daysInMonth)
            {
                wiz.State.Day = day;
                await ctx.LeaveSceneAsync();
                return;
            }

            wiz.Prev();
            wiz.ForceExecute();
            return;
        }

        switch (ctx.CallbackData)
        {
            case "back":
                wiz.Set(nameof(AskMonth));
                wiz.ForceExecute();
                return;
            case "cancel":
                await ctx.LeaveSceneAsync();
                return;
        }

        if (ctx.CallbackData!.StartsWith("day_"))
        {
            var day = int.Parse(ctx.CallbackData.Substring(4));
            wiz.State.Day = day;
            await ctx.LeaveSceneAsync();
            return;
        }

        await ctx.SendAsync("Something went wrong on Day. Cancelling date selection.");
        await ctx.LeaveSceneAsync();
    }
}