﻿using System.Collections;

namespace Telegami.Controls.Buttons;

public class TelegamiButtonRow : IEnumerable<TelegamiButton>
{
    private List<TelegamiButton> _buttons = new();

    public TelegamiButtonRow()
    {
    }

    public TelegamiButtonRow(IEnumerable<TelegamiButton> buttons)
    {
        _buttons = buttons.ToList();
    }

    public TelegamiButtonRow(params TelegamiButton[] buttons)
    {
        _buttons = buttons.ToList();
    }
    
    public TelegamiButton this[int index] => _buttons[index];

    public int Count => _buttons.Count;

    public void Add(string text, string value, string? url = null)
    {
        Add(new TelegamiButton(text, value, url));
    }

    public void Add(TelegamiButton button)
    {
        _buttons.Add(button);
    }

    public void AddRange(TelegamiButton button)
    {
        _buttons.Add(button);
    }

    public void Insert(int index, TelegamiButton button)
    {
        _buttons.Insert(index, button);
    }

    public bool Matches(string text, bool useText = true)
    {
        foreach (var b in _buttons)
        {
            if (useText && b.Text.Trim().Equals(text, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            if (!useText && b.Value.Equals(text, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///     Returns the button inside of the row which matches.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="useText"></param>
    /// <returns></returns>
    public TelegamiButton? GetButtonMatch(string text, bool useText = true)
    {
        foreach (var b in _buttons)
        {
            if (useText && b.Text.Trim().Equals(text, StringComparison.InvariantCultureIgnoreCase))
            {
                return b;
            }

            if (!useText && b.Value.Equals(text, StringComparison.InvariantCultureIgnoreCase))
            {
                return b;
            }
        }

        return null;
    }

    public static implicit operator TelegamiButtonRow(List<TelegamiButton> buttons)
    {
        return new TelegamiButtonRow { _buttons = buttons };
    }

    public IEnumerator<TelegamiButton> GetEnumerator()
    {
        return _buttons.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}