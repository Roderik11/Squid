using System;
using System.Collections.Generic;
using System.Text;
using Squid;
using System.Collections;
using System.Reflection;

namespace Squid
{
    /// <summary>
    /// A multi-column ListView.
    /// </summary>
    [Toolbox]
    public class ListView : Control
    {
        #region classes

        /// <summary>
        /// Delegate CellFormatter
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="FormatCellEventArgs"/> instance containing the event data.</param>
        /// <returns>Control.</returns>
        public delegate Control CellFormatter(object sender, FormatCellEventArgs e);

        /// <summary>
        /// Delegate HeaderFormatter
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="FormatHeaderEventArgs"/> instance containing the event data.</param>
        /// <returns>Control.</returns>
        public delegate Control HeaderFormatter(object sender, FormatHeaderEventArgs e);

        /// <summary>
        /// Class FormatCellEventArgs
        /// </summary>
        public class FormatCellEventArgs : EventArgs
        {
            public object Model { get; private set; }
            public Column Column { get; private set; }

            public FormatCellEventArgs(object row, Column column)
            {
                Model = row;
                Column = column;
            }
        }

        /// <summary>
        /// Class FormatHeaderEventArgs
        /// </summary>
        public class FormatHeaderEventArgs : EventArgs
        {
            public Column Column { get; private set; }

            public FormatHeaderEventArgs(Column column)
            {
                Column = column;
            }
        }

        /// <summary>
        /// A Column in the ListView
        /// </summary>
        public class Column
        {
            internal Control CustomHeader;

            private int _width;
            private int _minWidth;
            private int _maxWidth;
            private Point _clickedPos;
            private Point _oldSize;

            /// <summary>
            /// Gets or sets the text.
            /// </summary>
            /// <value>The text.</value>
            public string Text { get; set; }
            /// <summary>
            /// Gets or sets the aspect.
            /// </summary>
            /// <value>The aspect.</value>
            public string Aspect { get; set; }

            /// <summary>
            /// Gets the frame.
            /// </summary>
            /// <value>The frame.</value>
            public Frame Frame { get; private set; }
            /// <summary>
            /// Gets the frame handle.
            /// </summary>
            /// <value>The frame handle.</value>
            public Button FrameHandle { get; private set; }

            /// <summary>
            /// Gets the header.
            /// </summary>
            /// <value>The header.</value>
            public Frame Header { get; private set; }
            /// <summary>
            /// Gets the header handle.
            /// </summary>
            /// <value>The header handle.</value>
            public Button HeaderHandle { get; private set; }

            /// <summary>
            /// The tag
            /// </summary>
            public object Tag;

            /// <summary>
            /// Gets or sets the width.
            /// </summary>
            /// <value>The width.</value>
            public int Width
            {
                get { return _width; }
                set
                {
                    if (_width == value) return;
                    _width = value;
                    if (Header != null) Header.Size = new Point(_width, Header.Size.y);
                    if (Frame != null) Frame.Size = new Point(_width, Frame.Size.y);
                }
            }

            /// <summary>
            /// Gets or sets the minimum width.
            /// </summary>
            /// <value>The width.</value>
            public int MinWidth
            {
                get { return _minWidth; }
                set
                {
                    if (_minWidth == value) return;
                    _minWidth = value;

                    if (Header != null) Header.MinSize = new Point(_minWidth, Header.MinSize.y);
                    if (Frame != null) Frame.MinSize = new Point(_minWidth, Frame.MinSize.y);
                }
            }

            /// <summary>
            /// Gets or sets the maximum width.
            /// </summary>
            /// <value>The width.</value>
            public int MaxWidth
            {
                get { return _maxWidth; }
                set
                {
                    if (_maxWidth == value) return;
                    _maxWidth = value;

                    if (Header != null) Header.MaxSize = new Point(_maxWidth, Header.MaxSize.y);
                    if (Frame != null) Frame.MaxSize = new Point(_maxWidth, Frame.MaxSize.y);
                }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Column"/> class.
            /// </summary>
            public Column()
            {
                Frame = new Frame { Dock = DockStyle.Left, AutoSize = AutoSize.Vertical, Scissor = true };
                Header = new Frame { Dock = DockStyle.Left, Scissor = true };

                FrameHandle = new Button { Size = new Point(4, 4), Dock = DockStyle.Right, Cursor = CursorNames.VSplit };
                FrameHandle.MouseDown += Handle_MouseDown;
                FrameHandle.MousePress += Handle_MousePress;

                HeaderHandle = new Button { Size = new Point(4, 4), Dock = DockStyle.Right, Cursor = CursorNames.VSplit };
                HeaderHandle.MouseDown += Handle_MouseDown;
                HeaderHandle.MousePress += Handle_MousePress;

                MinWidth = 0;
                MaxWidth = 0;
                Width = 100;

                Reset();
            }

            internal void Reset()
            {
                Header.Controls.Clear();
                Header.Controls.Add(HeaderHandle);

                Frame.Controls.Clear();
                Frame.Controls.Add(FrameHandle);
            }

            internal void Clear()
            {
                Frame.Controls.Clear();
                Frame.Controls.Add(FrameHandle);
            }

            void Handle_MouseDown(Control sender, MouseEventArgs args)
            {
                if (args.Button > 0) return;

                _clickedPos = Gui.MousePosition;
                _oldSize = sender.Parent.Size;
            }

            void Handle_MousePress(Control sender, MouseEventArgs args)
            {
                if (args.Button > 0) return;

                Point p = Gui.MousePosition - _clickedPos;
                sender.Parent.ResizeTo(_oldSize + p, AnchorStyles.Right);
                Width = sender.Parent.Size.x;
            }
        }

        /// <summary>
        /// Class Row
        /// </summary>
        private class Row
        {
            /// <summary>
            /// The row object
            /// </summary>
            public object RowObject;
            /// <summary>
            /// The cells
            /// </summary>
            public List<Control> Cells = new List<Control>();
        }

        #endregion

        private Frame _headers;
        private List<object> _objects;
        private List<Row> Rows = new List<Row>();

        /// <summary>
        /// The cell formatter delegate
        /// </summary>
        public CellFormatter CreateCell;
        
        /// <summary>
        /// The header formatter delegate
        /// </summary>
        public HeaderFormatter CreateHeader;

        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <value>The header.</value>
        public Frame Header { get; private set; }

        /// <summary>
        /// Gets the panel.
        /// </summary>
        /// <value>The panel.</value>
        public Panel Panel { get; private set; }

        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>The columns.</value>
        public ActiveList<Column> Columns { get; private set; }

        private ControlCollection Headers { get { return _headers.Controls; } }
        private ControlCollection Frames { get { return Panel.Content.Controls; } }

        /// <summary>
        /// Gets or sets a value indicating whether [stretch last column].
        /// </summary>
        /// <value><c>true</c> if [stretch last column]; otherwise, <c>false</c>.</value>
        public bool StretchLastColumn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [full row select].
        /// </summary>
        /// <value><c>true</c> if [full row select]; otherwise, <c>false</c>.</value>
        public bool FullRowSelect { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListView"/> class.
        /// </summary>
        public ListView()
        {
            _objects = new List<object>();

            Header = new Frame { Size = new Point(32, 32), Dock = DockStyle.Top };
            Elements.Add(Header);

            _headers = new Frame();
            _headers.AutoSize = AutoSize.Horizontal;
            _headers.Size = new Point(32, 32);
            Header.Controls.Add(_headers);

            Panel = new Panel { Dock = DockStyle.Fill };
            Panel.Content.AutoSize = Squid.AutoSize.Vertical;
            Panel.Content.Update += Content_OnUpdate;

            Panel.VScroll.ButtonUp.Visible = false;
            Panel.VScroll.ButtonDown.Visible = false;
            Panel.VScroll.Size = new Point(13, 12);
            Panel.VScroll.Slider.Style = "vscrollTrack";
            Panel.VScroll.Slider.Button.Style = "vscrollButton";
            Panel.VScroll.Dock = DockStyle.Right;
            Panel.VScroll.Margin = new Margin(4, 0, 0, 0);

            Panel.HScroll.ButtonUp.Visible = false;
            Panel.HScroll.ButtonDown.Visible = false;
            Panel.HScroll.Size = new Point(13, 12);
            Panel.HScroll.Slider.Style = "vscrollTrack";
            Panel.HScroll.Slider.Button.Style = "vscrollButton";
            Panel.HScroll.Margin = new Margin(0, 4, 0, 0);
            Elements.Add(Panel);

            Columns = new ActiveList<Column>();
            Columns.ItemAdded += Columns_ItemAdded;
            Columns.ItemRemoved += Columns_ItemRemoved;
            Columns.ItemsCleared += Columns_ItemsCleared;
            Columns.ItemsSorted += Columns_ItemsSorted;

            StretchLastColumn = true;
        }

        void Content_OnUpdate(Control sender)
        {
            _headers.Position = new Point(Panel.Content.Position.x, 0);
            _headers.PerformLayout();
        }

        protected override void OnUpdate()
        {
            _headers.Size = new Point(_headers.Size.x, Header.Size.y);

            // disable horiz. scrollbar if last column is stretched
            Panel.Content.AutoSize = StretchLastColumn ? AutoSize.Vertical : AutoSize.HorizontalVertical;

            if (Columns.Count > 0)
            {
                if (StretchLastColumn)
                {
                    Columns[Columns.Count - 1].Frame.Dock = DockStyle.Fill;
                    Columns[Columns.Count - 1].Width = Columns[Columns.Count - 1].Frame.Size.x;
                }
                else
                    Columns[Columns.Count - 1].Frame.Dock = DockStyle.Left;
            }

            base.OnUpdate();
        }

        void Columns_ItemsSorted(object sender, EventArgs e)
        {
            // we clear frames/headers
            Headers.Clear();
            Frames.Clear();

            // and add them again in the new order
            foreach (Column column in Columns)
            {
                Headers.Add(column.Header);
                Frames.Add(column.Frame);
            }
        }

        void Columns_ItemsCleared(object sender, EventArgs e)
        {
            // clear frames&headers
            Frames.Clear();
            Headers.Clear();

            Rows.Clear();
        }

        void Columns_ItemRemoved(object sender, ListEventArgs<ListView.Column> e)
        {
            e.Item.Clear();

            // remove frame&header
            Frames.Remove(e.Item.Frame);
            Headers.Remove(e.Item.Header);
        }

        void Columns_ItemAdded(object sender, ListEventArgs<ListView.Column> e)
        {
            // lets put columns in the right order
            Columns_ItemsSorted(sender, null);

            // add cells for current column
            foreach (Row row in Rows)
                AddCell(row, e.Item);
        }

        private void BuildRows()
        {
            Rows.Clear();

            // add cells
            for (int i = 0; i < _objects.Count; i++)
            {
                Row row = new Row();
                row.RowObject = _objects[i];
                Rows.Add(row);

                foreach (Column column in Columns)
                    row.Cells.Add(column.Frame.Controls[i + 1]);
            }
        }

        private void AddCell(Row row, Column column)
        {
            Control cell = null;

            if (CreateCell != null)
                cell = CreateCell(this, new FormatCellEventArgs(row.RowObject, column)); // custom item

            if (cell == null)
                cell = new Label { Size = new Point(28, 28), Dock = DockStyle.Top, Text = GetAspectValue(row.RowObject, column) }; // default text item

            row.Cells.Add(cell);

            cell.Tag = row;
            cell.MouseClick += cell_MouseClick;
            cell.Update += cell_OnUpdate;

            // add cell to column
            column.Frame.Controls.Add(cell);
        }

        void cell_MouseClick(Control sender, MouseEventArgs args)
        {
            Row row = (Row)sender.Tag;

            if (sender is ISelectable)
            {
                ISelectable check = sender as ISelectable;
                check.Selected = !check.Selected;
            }

            foreach (Control cell in row.Cells)
            {
                if (!sender.Equals(cell) && cell is ISelectable)
                {
                    ISelectable check = cell as ISelectable;
                    check.Selected = !check.Selected;
                }
            }
        }

        void cell_OnUpdate(Control sender)
        {
            if (!FullRowSelect) return;

            Row row = (Row)sender.Tag;

            if (sender.Equals(Desktop.HotControl) || sender.Equals(Desktop.DropTarget))
            {
                foreach (Control cell in row.Cells)
                {
                    if (!sender.Equals(cell))
                        cell.State = sender.State;
                }
            }
        }

        /// <summary>
        /// Sets the objects.
        /// </summary>
        /// <param name="objects">The objects.</param>
        public void SetObjects(IEnumerable objects)
        {
            Rows.Clear();

            Panel.HScroll.SetValue(0);
            Panel.VScroll.SetValue(0);
            Panel.Content.Size = new Point(0, 0);
            Panel.Content.Position = new Point(0, 0);

            // clear frames&headers
            Columns_ItemsSorted(this, null);

            // clear columns
            foreach (Column column in Columns)
            {
                column.Clear();

                if (column.CustomHeader == null)
                {
                    Control header = null;

                    if (CreateHeader != null)
                        header = CreateHeader(this, new FormatHeaderEventArgs(column));

                    if (header == null)
                        header = new Button { Size = new Point(28, 28), Dock = DockStyle.Fill, Text = column.Text }; // default header

                    column.CustomHeader = header;

                    // add header button
                    column.Header.Controls.Add(header);
                }
            }

            // copy the data model
            _objects = new List<object>();

            if (objects == null)
                return;

            foreach (object ob in objects)
                _objects.Add(ob);

            // add cells
            for (int i = 0; i < _objects.Count; i++)
            {
                Row row = new Row();
                row.RowObject = _objects[i];
                Rows.Add(row);

                foreach (Column column in Columns)
                    AddCell(row, column);
            }
        }

        /// <summary>
        /// Sorts all objects using the specified comparer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="comparer">The comparer.</param>
        public void Sort<T>(Comparison<T> comparer)
        {
            foreach (Column column in Columns)
                column.Clear();

            Rows.Sort((a, b) => comparer.Invoke((T)a.RowObject, (T)b.RowObject));

            foreach (Row row in Rows)
            {
                for (int i = 0; i < Columns.Count; i++)
                    Columns[i].Frame.Controls.Add(row.Cells[i]);
            }
        }

        /// <summary>
        /// Gets the aspect value.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="column">The column.</param>
        /// <returns>System.String.</returns>
        public string GetAspectValue(object item, Column column)
        {
            if (string.IsNullOrEmpty(column.Aspect))
                return "no aspect";

            Type type = item.GetType();
            System.Reflection.MemberInfo[] infos = type.GetMember(column.Aspect);
            if (infos.Length == 0) return "not found";
            System.Reflection.MemberInfo member = infos[0];

            object value = null;

            if (member is FieldInfo)
            {
                value = ((FieldInfo)member).GetValue(item);
                return string.Format("{0}", value);
            }
            else if (member is PropertyInfo)
            {
                value = ((PropertyInfo)member).GetValue(item, null);
                return string.Format("{0}", value);
            }
            else if (member is MethodInfo)
            {
                value = ((MethodInfo)member).Invoke(item, null);
                return string.Format("{0}", value);
            }
            else
            {
                return "not a member";
            }
        }
    }
}
