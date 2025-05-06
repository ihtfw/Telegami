using System.Collections;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegami.Controls.Buttons
{
    public class TelegamiButtons : IEnumerable<TelegamiButtonRow>
    {
        public string Id { get; }

        private readonly List<TelegamiButtonRow> _rows = new();

        public TelegamiButtons(string id = "")
        {
            Id = id;
        }

        public TelegamiButtons(IEnumerable<TelegamiButtonRow> rows) : this("", rows)
        {

        }

        public TelegamiButtons(string id, IEnumerable<TelegamiButtonRow> rows)
        {
            Id = id;
            _rows = rows.ToList();
        }

        /// <summary>
        ///     Contains the number of rows.
        /// </summary>
        public int Rows => _rows.Count;

        /// <summary>
        ///     Contains the highest number of columns in a row.
        /// </summary>
        public int Cols
        {
            get { return _rows.Select(a => a.Count).OrderByDescending(a => a).FirstOrDefault(); }
        }
        
        public TelegamiButtonRow this[int row] => _rows[row];

        public int Count
        {
            get
            {
                if (_rows.Count == 0)
                {
                    return 0;
                }

                return _rows.Sum(x => x.Count);
            }
        }

        public TelegamiButtons Add(params (string text, string value)[] rowButtons)
        {
            if (rowButtons.Length == 0)
            {
                return this;
            }

            var row = new TelegamiButtonRow();
            foreach (var (text, value) in rowButtons)
            {
                row.Add(new TelegamiButton(text, value));
            }
            _rows.Add(row);
            return this;
        }

        public TelegamiButtons Add(params (string text, string value, string? url)[] rowButtons)
        {
            if (rowButtons.Length == 0)
            {
                return this;
            }

            var row = new TelegamiButtonRow();
            foreach (var (text, value, url) in rowButtons)
            {
                row.Add(new TelegamiButton(text, value, url));
            }
            _rows.Add(row);
            return this;
        }

        public TelegamiButtons Add(string text, string value, string? url = null)
        {
            return Add(new TelegamiButtonRow(new TelegamiButton(text, value, url)));
        }
        public TelegamiButtons Add(TelegamiButton button)
        {
            _rows.Add(new TelegamiButtonRow(button));

            return this;
        }

        public TelegamiButtons Add(TelegamiButtonRow row)
        {
            _rows.Add(row);

            return this;
        }

        public TelegamiButtons AddRange(IEnumerable<TelegamiButtonRow> rows)
        {
            _rows.AddRange(rows);

            return this;
        }

        public void Insert(int index, TelegamiButtonRow row)
        {
            _rows.Insert(index, row);
        }

        /// <summary>
        /// Buttons that are shown in chat with message
        /// </summary>
        /// <returns></returns>
        public InlineKeyboardButton[][] ToInlineButtons()
        {
            var array = _rows
                .Select(a => a.Select(b => b.ToInlineButton(this)).ToArray())
                .ToArray();

            return array;
        }

        /// <summary>
        /// Buttons that are global for chat, so be careful with them
        /// </summary>
        /// <returns></returns>
        public KeyboardButton[][] ToReplyButtons()
        {
            var ikb = _rows
                .Select(a => a.Select(b => b.ToKeyboardButton(this)).ToArray())
                .ToArray();

            return ikb;
        }

        public IEnumerator<TelegamiButtonRow> GetEnumerator()
        {
            return _rows.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
