using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Squid
{
    /// <summary>
    /// A multiline text input. Text does not scroll. (use a Panel).
    /// </summary>
    [Toolbox]
    public class TextArea : Control, IText
    {
        private float BlinkTime;
        private int DoBlink;
        private bool HasFocus;
        private string SavedText;
        private List<TextLine> Lines = new List<TextLine>();
        private bool IsDirty;
        private Point TextSize;
        private string ActiveHref;
        private Point LastSize;
        private string _text = string.Empty;
        // private int Ln;
        // private int Col;
        private int Caret = 0;

        private bool IsSelection { get { return _selectStart != _selectEnd; } }
        private int _selectStart = 0;
        private int _selectEnd = 0;

        /// <summary>
        /// Raised when [text changed].
        /// </summary>
        public event VoidEvent TextChanged;

        /// <summary>
        /// Raised when [text commit].
        /// </summary>
        public event EventHandler TextCommit;

        /// <summary>
        /// Raised when [text cancel].
        /// </summary>
        public event EventHandler TextCancel;

        /// <summary>
        /// Gets or sets a value indicating whether [read only].
        /// </summary>
        /// <value><c>true</c> if [read only]; otherwise, <c>false</c>.</value>
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [text wrap].
        /// </summary>
        public bool TextWrap { get; set; }

        /// <summary>
        /// Gets or sets the leading.
        /// </summary>
        /// <value>The leading.</value>
        [DefaultValue(0)]
        public int Leading { get; set; }

        /// <summary>
        /// Gets or sets the color of the link.
        /// </summary>
        /// <value>The color of the link.</value>
        [IntColor, DefaultValue(-1)]
        public int LinkColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        /// <value>The color of the text.</value>
        [IntColor, DefaultValue(-1)]
        public int TextColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the blink.
        /// </summary>
        /// <value>The color of the blink.</value>
        [IntColor, DefaultValue(-1)]
        public int BlinkColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use text color].
        /// </summary>
        /// <value><c>true</c> if [use text color]; otherwise, <c>false</c>.</value>
        [Category("Skin"), DefaultValue(false)]
        public bool UseTextColor { get; set; }

        /// <summary>
        /// Gets or sets the text align.
        /// </summary>
        /// <value>The text align.</value>
        public Alignment TextAlign { get; set; }

        /// <summary>
        /// Gets or sets the blink interval.
        /// </summary>
        /// <value>The blink interval.</value>
        [DefaultValue(500f)]
        public float BlinkInterval { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        [Multiline]
        public string Text
        {
            get { return _text.Replace("\n","\r\n"); }
            set
            {
                if (_text == value) return;

                _text = value.Replace("\r\n", "\n");
                //_text = value;

                Caret = _text.Length;
                IsDirty = true;

                _selectStart = 0;
                _selectEnd = 0;

                if (TextChanged != null)
                    TextChanged(this);
            }
        }

        private void SetText(string text)
        {
            //_text = text.Replace("\r\n", "\n");
            _text = text;

            IsDirty = true;
            if (TextChanged != null)
                TextChanged(this);
        }

        /// <summary>
        /// Gets the selection start.
        /// </summary>
        /// <value>The selection start.</value>
        [Xml.XmlIgnore]
        public int SelectionStart
        {
            get { return Math.Min(_selectStart, _selectEnd); }
        }

        /// <summary>
        /// Gets the selection end.
        /// </summary>
        /// <value>The selection end.</value>
        [Xml.XmlIgnore]
        public int SelectionEnd
        {
            get { return Math.Max(_selectStart, _selectEnd); }
        }

        /// <summary>
        /// Gets the cursor.
        /// </summary>
        /// <returns>System.Int32.</returns>
        public int GetCursor()
        {
            return Caret;
        }

        /// <summary>
        /// Sets the cursor.
        /// </summary>
        /// <param name="index">The index.</param>
        public void SetCursor(int index)
        {
            Caret = Math.Min(_text.Length, Math.Max(0, index));
            _selectStart = _selectEnd = Caret;
        }

        /// <summary>
        /// Selects the specified start.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public void Select(int start, int end)
        {
            _selectStart = Math.Min(_text.Length, Math.Max(0, start));
            _selectEnd = Math.Min(_text.Length, Math.Max(0, end));
            Caret = _selectEnd;
        }

        /// <summary>
        /// Gets the selection.
        /// </summary>
        /// <value>The selection.</value>
        [Xml.XmlIgnore]
        public string Selection
        {
            get
            {
                if (IsSelection)
                {
                    if (_text == null || _text.Length == 0) return string.Empty;

                    int start = SelectionStart;
                    int end = SelectionEnd;

                    return _text.Substring(start, end - start);
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the color of the selection.
        /// </summary>
        /// <value>The color of the selection.</value>
        [IntColor, DefaultValue(-1)]
        public int SelectionColor { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextArea"/> class.
        /// </summary>
        public TextArea()
        {
            LinkColor = -1;
            TextColor = -1;
            BlinkColor = -1;
            BlinkInterval = 500;
            SelectionColor = ColorInt.ARGB(.5f, 1, 1, 1);

            Style = "textbox";
            Cursor = CursorNames.Select;
            Size = new Point(100, 60);
            TextAlign = Alignment.TopLeft;
            AllowFocus = true;

            MouseDown += TextBox_MouseDown;
            MousePress += TextBox_MousePress;
            MouseDoubleClick += TextBox_MouseDoubleClick;
            LostFocus += TextBox_LostFocus;
            GotFocus += TextBox_GotFocus;
        }

        protected override void OnUpdate()
        {
            BlinkTime += Gui.TimeElapsed;

            if (BlinkTime > BlinkInterval)
            {
                DoBlink = 1 - DoBlink;
                BlinkTime = 0;
            }

            base.OnUpdate();
        }

        void TextBox_GotFocus(Control sender)
        {
            SavedText = _text;
            HasFocus = true;
        }

        void TextBox_LostFocus(Control sender)
        {
            HasFocus = false;

            if (TextCommit != null)
                TextCommit(this, null);
        }

        void TextBox_MouseDoubleClick(Control sender, MouseEventArgs args)
        {
        }

        void TextBox_MousePress(Control sender, MouseEventArgs args)
        {
            Point m = Gui.MousePosition;
            int total = 0;
            Rectangle rect;

            foreach (TextLine line in Lines)
            {
                for (int i = 0; i < line.Elements.Count; i++)
                {
                    bool firstElement = i == 0;
                    bool lastElement = i == line.Elements.Count - 1;

                    rect = line.Elements[i].Rectangle;

                    if (firstElement)
                        rect.Left = Location.x;

                    if (lastElement)
                        rect.Right = Location.x + Size.x;

                    if (lastElement && line.Elements[i].Linebreak)
                    {
                        rect.Top -= line.Elements[i].Size.y;
                        rect.Bottom -= line.Elements[i].Size.y;
                    }

                    if (rect.Contains(m))
                    {
                        Point p = new Point(line.Elements[i].Rectangle.Left, line.Elements[i].Rectangle.Top);
                        Point mb = m - p;
                        int font = Gui.Renderer.GetFont(line.Elements[i].Font);
                        int off = 0;
                        int c = 0;

                        while (c < line.Elements[i].Text.Length)
                        {
                            off = Gui.Renderer.GetTextSize(line.Elements[i].Text.Substring(0, c), font).x;

                            if (off > mb.x)
                                break;

                            c++;
                        }

                        _selectEnd = total + c;
                        return;
                    }

                    if (line.Elements[i].Linebreak)
                        total++;
                    else
                        total += line.Elements[i].Text.Length;
                }
            }
        }

        void TextBox_MouseDown(Control sender, MouseEventArgs args)
        {
            Point m = Gui.MousePosition;
            int total = 0;
            Rectangle rect;

            foreach (TextLine line in Lines)
            {
                for (int i = 0; i < line.Elements.Count; i++)
                {
                    bool firstElement = i == 0;
                    bool lastElement = i == line.Elements.Count - 1;

                    rect = line.Elements[i].Rectangle;

                    if (firstElement)
                        rect.Left = Location.x;

                    if (lastElement)
                        rect.Right = Location.x + Size.x;

                    if (lastElement && line.Elements[i].Linebreak)
                    {
                        rect.Top -= line.Elements[i].Size.y;
                        rect.Bottom -= line.Elements[i].Size.y;
                    }

                    if (rect.Contains(m))
                    {
                        Point p = new Point(line.Elements[i].Rectangle.Left, line.Elements[i].Rectangle.Top);
                        Point mb = m - p;
                        int font = Gui.Renderer.GetFont(line.Elements[i].Font);
                        int off = 0;
                        int c = 0;

                        while (c < line.Elements[i].Text.Length)
                        {
                            off = Gui.Renderer.GetTextSize(line.Elements[i].Text.Substring(0, c), font).x;

                            if (off > mb.x)
                                break;

                            c++;
                        }

                        _selectStart = _selectEnd = Caret = total + c;
                        return;
                    }

                    if (line.Elements[i].Linebreak)
                        total++;
                    else
                        total += line.Elements[i].Text.Length;
                }
            }
        }

        private int FindIndexLeft(int start, string text)
        {
            if (start <= 0) return start;

            while (true)
            {
                start--;

                if (char.IsWhiteSpace(text, start) || char.IsPunctuation(text, start) || start <= 0)
                    return start;
            }
        }

        private int FindIndexRight(int start, string text)
        {
            if (start >= text.Length - 1) return start;

            while (true)
            {
                start++;

                if (char.IsWhiteSpace(text, start) || char.IsPunctuation(text, start) || start >= text.Length - 1)
                    return start + 1;
            }
        }

        private void HandleLeftArrow()
        {
            //if (Caret > 0)
            //    Caret--;

            if (Gui.CtrlPressed)
            {
                string masked = _text;

                if (Gui.ShiftPressed)
                {
                    if (_selectStart == Caret)
                    {
                        Caret = _selectStart = FindIndexLeft(_selectStart, masked);
                    }
                    else if (_selectEnd == Caret)
                    {
                        int index = FindIndexLeft(_selectEnd, masked);

                        if (index < _selectStart)
                        {
                            _selectEnd = _selectStart;
                            _selectStart = index;
                            Caret = index;
                        }
                        else
                            Caret = _selectEnd = index;
                    }
                }
                else
                {
                    _selectStart = _selectEnd = Caret = FindIndexLeft(Caret, masked);
                }
            }
            else
            {
                if (Caret > 0)
                {
                    if (Gui.ShiftPressed)
                    {
                        if (_selectStart == Caret)
                            _selectStart--;
                        else if (_selectEnd == Caret)
                            _selectEnd--;
                        Caret--;
                    }
                    else
                    {
                        if (IsSelection)
                        {
                            _selectStart = _selectEnd = Caret;
                        }
                        else
                        {
                            Caret--;
                            _selectStart = _selectEnd = Caret;
                        }
                    }
                }
                else if (!Gui.ShiftPressed)
                {
                    _selectStart = _selectEnd = Caret;
                }
            }
        }

        private void HandleRightArrow()
        {
            //if (Caret < _text.Length)
            //    Caret++;

            if (Gui.CtrlPressed)
            {
                if (Gui.ShiftPressed)
                {
                    if (_selectEnd == Caret)
                    {
                        int index = FindIndexRight(_selectEnd, _text);

                        Caret = _selectEnd = index;
                    }
                    else if (_selectStart == Caret)
                    {
                        int index = FindIndexRight(_selectStart, _text);
                        if (index > _selectEnd)
                        {
                            _selectStart = _selectEnd;
                            _selectEnd = index;
                            Caret = index;
                        }
                        else
                            Caret = _selectStart = index;
                    }
                }
                else
                {
                    _selectStart = _selectEnd = Caret = FindIndexRight(Caret, _text);
                }
            }
            else
            {
                if (Caret < _text.Length)
                {
                    if (Gui.ShiftPressed)
                    {
                        if (_selectEnd == Caret)
                            _selectEnd++;
                        else if (_selectStart == Caret)
                            _selectStart++;

                        Caret++;
                    }
                    else
                    {
                        if (IsSelection)
                        {
                            _selectStart = _selectEnd = Caret;
                        }
                        else
                        {
                            Caret++;
                            _selectStart = _selectEnd = Caret;
                        }
                    }
                }
                else if (!Gui.ShiftPressed)
                {
                    _selectStart = _selectEnd = Caret;
                }
            }
        }

        private void HandleUpArrow()
        {
            int textlength = 0;
            int lastline = 0;

            if (Gui.ShiftPressed)
            {
                if (!IsSelection)
                    _selectStart = Caret;
            }
            else
            {
                _selectStart = _selectEnd = 0;
            }

            for (int i = 0; i < Lines.Count; i++)
            {
                TextLine line = Lines[i];

                if (textlength + line.CharLength > Caret)
                {
                    int left = Caret - textlength;

                    if (i > 0 && left > -1)
                    {
                        int lastlen = Lines[i - 1].CharLength;
                        Caret = textlength + Math.Min(lastlen - 1, left) - lastlen;

                        if (Gui.ShiftPressed)
                            _selectEnd = Caret;

                        break;
                    }
                }
                else if (textlength + line.CharLength == Caret)
                {
                    int left = 0;

                    if (i > 0 && left > -1)
                    {
                        int lastlen = Lines[i - 1].CharLength;

                        if (i == Lines.Count - 1)
                            Caret = lastline + Math.Min(lastlen - 1, line.CharLength);
                        else
                            Caret = textlength + Math.Min(lastlen - 1, left);

                        if (Gui.ShiftPressed)
                            _selectEnd = Caret;

                        break;
                    }
                }

                lastline = textlength;
                textlength += line.CharLength;
            }
        }

        private void HandleDownArrow()
        {
            int total = 0;

            if (Gui.ShiftPressed)
            {
                if (!IsSelection)
                    _selectStart = Caret;
            }
            else
            {
                _selectStart = _selectEnd = 0;
            }

            for (int i = 0; i < Lines.Count; i++)
            {
                TextLine line = Lines[i];

                if (total + line.CharLength > Caret)
                {
                    int left = Caret - total;

                    if (Lines.Count > i + 1)
                    {
                        int nextlen = Lines[i + 1].CharLength;

                        if (total + line.CharLength == Caret + 1 && i + 1 != Lines.Count - 1)
                        {
                            if (left == 0 && line.CharLength == 1)
                                Caret = total + line.CharLength + Math.Min(nextlen, left);
                            else
                                Caret = total + line.CharLength - 1 + Math.Min(nextlen, left);
                        }
                        else
                            Caret = total + line.CharLength + Math.Min(nextlen, left);

                        if (Gui.ShiftPressed)
                            _selectEnd = Caret;

                        break;
                    }
                }

                total += line.CharLength;
            }
        }

        protected override void OnKeyDown(KeyEventArgs args)
        {
            Desktop root = Desktop;
            if (root == null) return;

            BlinkTime = 0; DoBlink = 1;

            if (args.Key == Keys.HOME && !ReadOnly) // home
            {
                if (Gui.ShiftPressed)
                {
                    if (_selectStart == Caret)
                    {
                        Caret = _selectStart = 0;
                    }
                    else if (_selectEnd == Caret)
                    {
                        if (0 < _selectStart)
                        {
                            _selectEnd = _selectStart;
                            _selectStart = 0;
                            Caret = 0;
                        }
                        else
                            Caret = _selectEnd = 0;
                    }
                }
                else
                {
                    Caret = 0;
                    _selectStart = _selectEnd = Caret;
                }
            }
            else if (args.Key == Keys.END && !ReadOnly) // end
            {
                if (Gui.ShiftPressed)
                {
                    if (_selectEnd == Caret)
                    {
                        Caret = _selectEnd = _text.Length;
                    }
                    else if (_selectStart == Caret)
                    {
                        if (_text.Length > _selectEnd)
                        {
                            _selectStart = _selectEnd;
                            _selectEnd = _text.Length;
                            Caret = _text.Length;
                        }
                        else
                            Caret = _selectStart = _text.Length;
                    }
                }
                else
                {
                    Caret = _text.Length;
                    _selectStart = _selectEnd = Caret;
                }
            }
            else if (args.Key == Keys.RIGHTARROW && !ReadOnly) // right arrow
            {
                HandleRightArrow();
            }
            else if (args.Key == Keys.LEFTARROW && !ReadOnly) // left arrow
            {
                HandleLeftArrow();
            }
            else if (args.Key == Keys.UPARROW && !ReadOnly) // up arrow
            {
                HandleUpArrow();
            }
            else if (args.Key == Keys.DOWNARROW && !ReadOnly) // down arrow
            {
                HandleDownArrow();
            }
            else if (args.Key == Keys.BACKSPACE && !ReadOnly) // backspace
            {
                //if (Caret > 0)
                //{
                //    HandleLeftArrow();

                //    if (_text.Length > Caret)
                //        SetText(_text.Remove(Caret, 1));
                //}

                if (IsSelection)
                {
                    int start = SelectionStart;
                    int end = SelectionEnd;

                    SetText(_text.Remove(start, end - start));
                    Caret = start;
                }
                else
                {
                    if (Caret > 0)
                    {
                        SetText(_text.Remove(Caret - 1, 1));
                        if (Caret > 0) Caret--;
                    }
                }

                _selectStart = _selectEnd = Caret;
            }
            else if (args.Key == Keys.DELETE && !ReadOnly) // delete
            {
                //if (_text.Length > Caret)
                //    SetText(_text.Remove(Caret, 1));

                if (IsSelection)
                {
                    int start = SelectionStart;
                    int end = SelectionEnd;

                    SetText(_text.Remove(start, end - start));
                    Caret = start;
                }
                else
                {
                    if (_text.Length > Caret)
                        SetText(_text.Remove(Caret, 1));
                }

                _selectStart = _selectEnd = Caret;
            }
            else if (args.Key == Keys.RETURN || args.Key == Keys.NUMPADENTER) // return/enter
            {
                SetText(_text.Insert(Caret, "\n"));
                Caret++;
            }
            else if (args.Key == Keys.ESCAPE)
            {
                LostFocus -= TextBox_LostFocus;

                SetText(SavedText);
                Caret = 0;

                root.FocusedControl = null;

                LostFocus += TextBox_LostFocus;

                if (TextCancel != null)
                    TextCancel(this, null);
            }
            else
            {
                if (Gui.CtrlPressed && !Gui.AltPressed)
                {
                    if (args.Key == Keys.A) // select all
                    {
                        _selectStart = 0;
                        _selectEnd = _text.Length;
                        Caret = _text.Length;
                    }
                    else if (args.Key == Keys.C) // copy
                    {
                        if (IsSelection)
                            Gui.SetClipboard(Selection.Replace("\n", "\r\n"));

                    }
                    else if (args.Key == Keys.X) // copy
                    {
                        if (IsSelection)
                        {
                            Gui.SetClipboard(Selection);

                            int start = SelectionStart;
                            int end = SelectionEnd;

                            SetText(_text.Remove(start, end - start));
                            //Caret = start;
                            _selectStart = _selectEnd = Caret = start;
                        }
                    }
                    else if (args.Key == Keys.V && !ReadOnly) // pasteb
                    {
                        string paste = Gui.GetClipboard().Replace("\r\n","\n");
                        if (!string.IsNullOrEmpty(paste))
                        {
                            if (IsSelection)
                            {
                                int start = SelectionStart;
                                int end = SelectionEnd;

                                SetText(_text.Remove(start, end - start));
                                Caret = start;
                            }

                            SetText(_text.Insert(Caret, paste.ToString()));

                            if (Caret < _text.Length)
                                Caret += paste.Length;

                            _selectStart = _selectEnd = Caret;
                        }
                    }
                }
                else
                {
                    if (args.Key != Keys.TAB)
                    {
                        if (args.Char.HasValue)
                        {
                            bool valid = true;
                            char c = args.Char.Value;

                            if (valid)
                            {
                                if (IsSelection)
                                {
                                    int start = SelectionStart;
                                    int end = SelectionEnd;

                                    SetText(_text.Remove(start, end - start));
                                    Caret = start;
                                }

                                SetText(_text.Insert(Caret, c.ToString()));
                                if (Caret < _text.Length)
                                    Caret++;

                                _selectStart = _selectEnd = Caret;
                            }
                        }
                    }
                }
            }
        }

        private void UpdateText(Style style)
        {
            Lines.Clear();

            if (string.IsNullOrEmpty(_text)) return;

            TextElement def = new TextElement();
            def.Font = style.Font;

            List<TextElement> elements = BBCode.Parse(_text, style, false);
            List<TextElement> textElements = new List<TextElement>();

            Point pos = new Point();
            Point tsize = new Point();

            int lineHeight = 0;
            List<TextElement> thisLine = new List<TextElement>();

            TextSize = Point.Zero;

            if (TextWrap)
            {
                #region TextWrap = true
                foreach (TextElement element in elements)
                {
                    int font = Gui.Renderer.GetFont(element.Font);

                    if (element.Linebreak)
                    {
                        pos.x = 0;
                        pos.y += lineHeight + Leading;

                        element.Position = pos;
                        element.Size = Gui.Renderer.GetTextSize(" ", font);

                        foreach (TextElement el in thisLine)
                        {
                            if (!el.Linebreak)
                                el.Position.y += lineHeight - el.Size.y;
                        }

                        thisLine.Clear();
                        lineHeight = element.Size.y;
                        textElements.Add(element);
                    }
                    else
                    {
                        #region wrap

                        string[] words = element.Text.Split(' ');

                        List<TextElement> sub = new List<TextElement>();

                        TextElement e = new TextElement(element);
                        e.Text = string.Empty;
                        e.Position = pos;
                        sub.Add(e);

                        int i = 0;

                        foreach (string word in words)
                        {
                            string temp = word;
                            if (i > 0) temp = " " + word;

                            tsize = Gui.Renderer.GetTextSize(temp, font);
                            lineHeight = Math.Max(lineHeight, tsize.y);
                            int limit = Size.x - (style.TextPadding.Left + style.TextPadding.Right);

                            i++;

                            // the word fits, add to current element
                            if (pos.x + tsize.x < limit)
                            {
                                e.Text += temp;
                                e.Size = Gui.Renderer.GetTextSize(e.Text, font);
                                pos.x += tsize.x;
                            }
                            else
                            {
                                // the whole word is larger than the text area
                                if (tsize.x > limit)
                                {

                                }
                                // the whole word fits into text area
                                else
                                {
                                    pos.x = 0;
                                    pos.y += lineHeight + Leading;
                                    thisLine.Add(e);

                                    foreach (TextElement el in thisLine)
                                    {
                                        if (!el.Linebreak)
                                            el.Position.y += lineHeight - el.Size.y;
                                    }

                                    thisLine.Clear();
                                    lineHeight = 0;

                                    TextElement linebreak = new TextElement(e);
                                    linebreak.Text = string.Empty;
                                    linebreak.Linebreak = true;
                                    linebreak.Position = pos;
                                    sub.Add(linebreak);

                                    e = new TextElement(element);
                                    e.Text = word;
                                    e.Position = pos;
                                    e.Size = Gui.Renderer.GetTextSize(e.Text, font);
                                    sub.Add(e);

                                    lineHeight = Math.Max(lineHeight, e.Size.y);

                                    pos.x += tsize.x;
                                }
                            }
                        }

                        thisLine.AddRange(sub);
                        textElements.AddRange(sub);
                        #endregion
                    }
                }

                foreach (TextElement el in thisLine)
                {
                    if (!el.Linebreak)
                        el.Position.y += lineHeight - el.Size.y;
                }
                #endregion
            }
            else
            {
                foreach (TextElement element in elements)
                {
                    int font = Gui.Renderer.GetFont(element.Font);

                    if (element.Linebreak)
                    {
                        pos.x = 0;
                        pos.y += lineHeight + Leading;

                        element.Position = pos;
                        element.Size = Gui.Renderer.GetTextSize(" ", font);

                        foreach (TextElement el in thisLine)
                        {
                            if (!el.Linebreak)
                                el.Position.y += lineHeight - el.Size.y;
                        }

                        thisLine.Clear();
                        lineHeight = element.Size.y;

                        textElements.Add(element);
                    }
                    else
                    {
                        //if (!string.IsNullOrEmpty(element.Text))
                        //{
                        tsize = Gui.Renderer.GetTextSize(string.IsNullOrEmpty(element.Text) ? " " : element.Text, font);
                        lineHeight = Math.Max(lineHeight, tsize.y);

                        element.Position = pos;
                        element.Size = tsize;

                        textElements.Add(element);

                        pos.x += tsize.x;

                        thisLine.Add(element);
                        //}
                    }
                }

                foreach (TextElement el in thisLine)
                {
                    if (!el.Linebreak)
                        el.Position.y += lineHeight - el.Size.y;
                }

            }

            TextLine line = new TextLine();

            foreach (TextElement element in textElements)
            {
                if (element.Linebreak)
                {
                    line.CharLength += 1;
                    line.Elements.Add(element);
                    Lines.Add(line);
                    line = new TextLine();
                }
                else
                {
                    line.Width += element.Size.x;
                    line.CharLength += element.Text.Length;
                    line.Elements.Add(element);
                }

                TextSize.x = Math.Max(TextSize.x, line.Width);
                TextSize.y = Math.Max(TextSize.y, element.Position.y + element.Size.y);
            }

            Lines.Add(line);

            TextSize += new Point(style.TextPadding.Left + style.TextPadding.Right, style.TextPadding.Top + style.TextPadding.Bottom);

            LastSize = Size;
            IsDirty = false;

            //GetLineAndCol();
        }


        //protected override void Bind(object instance)
        //{
        //    if (string.IsNullOrEmpty(Aspect)) return;
        //    if (instance == null) return;

        //    Type type = instance.GetType();
        //    object value = null;

        //    System.Reflection.PropertyInfo property = type.GetProperty(Aspect);
        //    if (property != null)
        //    {
        //        value = property.GetValue(instance, null);
        //        if (value != null) Text = value.ToString();
        //    }
        //    else
        //    {
        //        System.Reflection.FieldInfo field = type.GetField(Aspect);
        //        if (field != null)
        //        {
        //            value = field.GetValue(instance);
        //            if (value != null) Text = value.ToString();
        //        }
        //    }
        //}

        protected override void OnStateChanged()
        {
            Style style = Desktop.GetStyle(Style).Styles[State];
            UpdateText(style);
        }

        protected override void OnAutoSize()
        {
            if (IsDirty)
            {
                Style style = Desktop.GetStyle(Style).Styles[State];
                UpdateText(style);
            }

            if (AutoSize == Squid.AutoSize.Vertical)
                Size = new Point(Size.x, TextSize.y);
            else if (AutoSize == Squid.AutoSize.Horizontal)
                Size = new Point(TextSize.x, Size.y);
            else if (AutoSize == Squid.AutoSize.HorizontalVertical)
                Size = TextSize;
        }

        protected override void OnLateUpdate()
        {
            if (!IsDirty)
                IsDirty = LastSize.x != Size.x || LastSize.y != Size.y;

            if (IsDirty)
            {
                Style style = Desktop.GetStyle(Style).Styles[State];
                UpdateText(style);
            }

            if (Desktop.HotControl == this)
            {
                Point m = Gui.MousePosition;
                ActiveHref = null;

                foreach (TextLine line in Lines)
                {
                    foreach (TextElement element in line.Elements)
                    {
                        if (!element.IsLink) continue;

                        if (element.Rectangle.Contains(m))
                        {
                            ActiveHref = element.Href;
                            break;
                        }
                    }
                }
            }
        }

        protected override void DrawText(Style style, float opacity)
        {
            if (IsDirty) UpdateText(style);

            int font;
            int total = 0;
            int numLine = 0;
            Point p1, p2, size;
            Alignment align = TextAlign != Alignment.Inherit ? TextAlign : style.TextAlign;
            bool drawCaret = HasFocus && DoBlink > 0;

            if (Lines.Count == 0)
            {
                if (drawCaret)
                {
                    p1 = Location;
                    font = Gui.Renderer.GetFont(style.Font);

                    if (align == Alignment.TopLeft || align == Alignment.TopCenter || align == Alignment.TopRight)
                        p1.y += style.TextPadding.Top;

                    if (align == Alignment.BottomLeft || align == Alignment.BottomCenter || align == Alignment.BottomRight)
                        p1.y += Size.y - TextSize.y;

                    if (align == Alignment.MiddleLeft || align == Alignment.MiddleCenter || align == Alignment.MiddleRight)
                        p1.y += (Size.y - (TextSize.y - (style.TextPadding.Top + style.TextPadding.Bottom))) / 2;

                    if (align == Alignment.TopLeft || align == Alignment.MiddleLeft || align == Alignment.BottomLeft)
                        p1.x += style.TextPadding.Left;

                    if (align == Alignment.TopRight || align == Alignment.MiddleRight || align == Alignment.BottomRight)
                        p1.x += Size.x - style.TextPadding.Right;

                    if (align == Alignment.TopCenter || align == Alignment.MiddleCenter || align == Alignment.BottomCenter)
                        p1.x += Size.x / 2;

                    Point subsize = Gui.Renderer.GetTextSize(" ", font);
                    Gui.Renderer.DrawBox(p1.x, p1.y, 2, subsize.y, ColorInt.FromArgb(opacity, BlinkColor));

                }

                return;
            }

            foreach (TextLine line in Lines)
            {
                int perline = 0;

                foreach (TextElement element in line.Elements)
                {
                    //if (element.Linebreak)
                    //    continue;

                    font = Gui.Renderer.GetFont(element.Font);

                    if (element.Linebreak)
                        total++;
                    else
                        total += element.Text.Length;

                    size = element.Size;
                    p1 = Location;

                    if (align == Alignment.TopLeft || align == Alignment.TopCenter || align == Alignment.TopRight)
                        p1.y += style.TextPadding.Top;

                    if (align == Alignment.BottomLeft || align == Alignment.BottomCenter || align == Alignment.BottomRight)
                        p1.y += Size.y - TextSize.y;

                    if (align == Alignment.MiddleLeft || align == Alignment.MiddleCenter || align == Alignment.MiddleRight)
                        p1.y += (Size.y - (TextSize.y - (style.TextPadding.Top + style.TextPadding.Bottom))) / 2;

                    if (align == Alignment.TopLeft || align == Alignment.MiddleLeft || align == Alignment.BottomLeft)
                        p1.x += style.TextPadding.Left;

                    if (align == Alignment.TopRight || align == Alignment.MiddleRight || align == Alignment.BottomRight)
                        p1.x += Size.x - line.Width - style.TextPadding.Right;

                    if (align == Alignment.TopCenter || align == Alignment.MiddleCenter || align == Alignment.BottomCenter)
                        p1.x += (Size.x - line.Width) / 2;

                    p2 = element.Position + p1;

                    element.Rectangle = new Rectangle(p2, size);

                    if (element.IsLink && element.Href == ActiveHref)
                        Gui.Renderer.DrawBox(p2.x, p2.y, size.x - 1, size.y, ColorInt.FromArgb(opacity, LinkColor));

                    if (drawCaret && total >= Caret)
                    {
                        drawCaret = false;

                        if (string.IsNullOrEmpty(element.Text))
                        {
                            Point subsize = Gui.Renderer.GetTextSize(" ", font);
                            Gui.Renderer.DrawBox(p2.x, p2.y, 2, subsize.y, ColorInt.FromArgb(opacity, BlinkColor));
                        }
                        else
                        {
                            string substr = element.Text.Substring(0, element.Text.Length - (total - Caret));

                            if (string.IsNullOrEmpty(substr))
                            {
                                Point subsize = Gui.Renderer.GetTextSize(" ", font);
                                Gui.Renderer.DrawBox(p2.x, p2.y, 2, subsize.y, ColorInt.FromArgb(opacity, BlinkColor));
                            }
                            else
                            {
                                Point subsize = Gui.Renderer.GetTextSize(substr, font);
                                Gui.Renderer.DrawBox(p2.x + subsize.x, p2.y, 2, subsize.y, ColorInt.FromArgb(opacity, BlinkColor));
                            }
                        }
                    }

                    if (UseTextColor)
                        Gui.Renderer.DrawText(element.Text, p2.x, p2.y, font, ColorInt.FromArgb(opacity, TextColor));
                    else
                        Gui.Renderer.DrawText(element.Text, p2.x, p2.y, font, ColorInt.FromArgb(opacity, element.Color.HasValue ? (int)element.Color : style.TextColor));

                    //    Gui.Renderer.DrawBox(element.Rectangle.Left, element.Rectangle.Top, element.Rectangle.Width, element.Rectangle.Height, -1);

                    if (IsSelection && total >= SelectionStart && perline < SelectionEnd && !element.Linebreak)
                    {
                        int start = SelectionStart;
                        int end = SelectionEnd;
                        int color = ColorInt.FromArgb(0.5f, -1);

                        //int origin = perline - element.Text.Length;
                        //start = Math.Max(0, origin - start);

                        //if (string.IsNullOrEmpty(element.Text))
                        //{
                        //    Point subsize = Gui.Renderer.GetTextSize(" ", font);
                        //    Gui.Renderer.DrawBox(p2.x, p2.y, subsize.x, subsize.y, ColorInt.FromArgb(opacity, BlinkColor));
                        //}
                        //else
                        //{
                        int begin = element.Text.Length - (total - start);
                        if (begin < 0) begin = 0;

                        int len = element.Text.Length - begin - (total - end);
                        if (len < 0) len = 0;
                        if (len > element.Text.Length) len = element.Text.Length;
                        if (begin + len > element.Text.Length) len = element.Text.Length - begin;

                        string strOffset = element.Text.Substring(0, begin);
                        string strSelected = element.Text.Substring(begin, len);

                        Point offset = Gui.Renderer.GetTextSize(strOffset, font);
                        Point selection = Gui.Renderer.GetTextSize(strSelected, font);
                        Gui.Renderer.DrawBox(p2.x + offset.x, p2.y, selection.x, selection.y, ColorInt.FromArgb(opacity, SelectionColor));
                        //}
                    }

                    if (!string.IsNullOrEmpty(element.Text))
                        perline += element.Text.Length;
                    else
                        perline++;

                }

                numLine++;
            }
        }
    }
}
