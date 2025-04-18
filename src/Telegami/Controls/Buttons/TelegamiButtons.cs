using Telegram.Bot.Types.ReplyMarkups;

namespace Telegami.Controls.Buttons
{
    public class TelegamiButtons
    {
        public string Id { get; }

        private readonly List<TelegamiButtonRow> _buttons = new();

        public TelegamiButtons(string id = "")
        {
            Id = id;
        }

        // public TelegamiButtons(ControlBase control)
        // {
        //     DependencyControl = control;
        // }

        public TelegamiButtons(IEnumerable<TelegamiButtonRow> rows) : this("", rows)
        {

        }

        public TelegamiButtons(string id, IEnumerable<TelegamiButtonRow> rows)
        {
            Id = id;
            _buttons = rows.ToList();
        }


        // public IReplyMarkup Markup { get; set; }

        // public ControlBase DependencyControl { get; set; }

        /// <summary>
        ///     Contains the number of rows.
        /// </summary>
        public int Rows => _buttons.Count;

        /// <summary>
        ///     Contains the highest number of columns in an row.
        /// </summary>
        public int Cols
        {
            get { return _buttons.Select(a => a.Count).OrderByDescending(a => a).FirstOrDefault(); }
        }


        public TelegamiButtonRow this[int row] => _buttons[row];

        public int Count
        {
            get
            {
                if (_buttons.Count == 0)
                {
                    return 0;
                }

                return _buttons.Select(a => a.ToArray()).ToList().Aggregate((a, b) => a.Union(b).ToArray()).Length;
            }
        }

        public TelegamiButtons AddButtonRow(string text, string value, string? url = null)
        {
            _buttons.Add(new List<TelegamiButton> { new(text, value, url) });

            return this;
        }

        //public void AddButtonRow(ButtonRow row)
        //{
        //    Buttons.Add(row.ToList());
        //}

        public TelegamiButtons AddButtonRow(TelegamiButtonRow row)
        {
            _buttons.Add(row);

            return this;
        }

        public TelegamiButtons AddButtonRow(params TelegamiButton[] row)
        {
            AddButtonRow(row.ToList());

            return this;
        }

        public TelegamiButtons AddButtonRows(IEnumerable<TelegamiButtonRow> rows)
        {
            _buttons.AddRange(rows);

            return this;
        }

        public void InsertButtonRow(int index, IEnumerable<TelegamiButton> row)
        {
            _buttons.Insert(index, row.ToList());
        }

        public void InsertButtonRow(int index, TelegamiButtonRow row)
        {
            _buttons.Insert(index, row);
        }

        public InlineKeyboardButton[][] ToInlineButtonArray()
        {
            var ikb = _buttons.Select(a => a.ToArray().Select(b => b.ToInlineButton(this)).ToArray()).ToArray();

            return ikb;
        }
    }
}
