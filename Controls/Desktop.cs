using System.Collections.Generic;

namespace Squid
{
    /// <summary>
    /// Delegate CursorChangedEvent
    /// </summary>
    /// <param name="cursor">The cursor.</param>
    public delegate void CursorChangedEvent(Cursor cursor);

    /// <summary>
    ///The root Control
    /// </summary>
    public class Desktop : Control, IControlContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Desktop"/> class.
        /// </summary>
        public Desktop()
        {
            Skin = new Skin();
            CursorSet = new CursorCollection();
            TooltipControl = new SimpleTooltip();
            DragDropSnap = 0;
        }

        /// <summary>
        /// Raised when [cursor changed].
        /// </summary>
        public event CursorChangedEvent CursorChanged;

        /// <summary>
        /// Enum PickMode
        /// </summary>
        public enum PickMode
        {
            /// <summary>
            /// The control
            /// </summary>
            Control,

            /// <summary>
            /// The container
            /// </summary>
            Container
        }

        /// <summary>
        /// Gets or sets the controls.
        /// </summary>
        /// <value>The controls.</value>
        public ControlCollection Controls { get; set; }

        /// <summary>
        /// Gets or sets the current cursor.
        /// </summary>
        /// <value>The current cursor.</value>
        [Xml.XmlIgnore]
        public string CurrentCursor
        {
            get
            {
                return _cursor;
            }
            set
            {
                if (value == _cursor) return;
                _cursor = value;

                if (CursorChanged != null)
                {
                    Cursor cursor = GetCursor();
                    CursorChanged(cursor);
                }

                // Cursor cursor = GetCursor();

                // if (cursor == null)
                //    UnityEngine.Cursor.SetCursor(null, UnityEngine.Vector2.zero, UnityEngine.CursorMode.Auto);
                // else
                // {
                //    UnityEngine.Texture2D tex = ((UnityRenderer)Gui.Renderer).FindTexture(cursor.Texture);
                //    UnityEngine.Cursor.SetCursor(tex, new UnityEngine.Vector2(cursor.HotSpot.x, cursor.HotSpot.y), UnityEngine.CursorMode.Auto);
                // }

                // if (CursorChanged != null)
                //    CursorChanged(cursor);
            }
        }

        [Xml.XmlIgnore]
        public CursorCollection CursorSet { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [design mode].
        /// </summary>
        /// <value><c>true</c> if [design mode]; otherwise, <c>false</c>.</value>
        [Xml.XmlIgnore]
        public bool DesignMode { get; set; }

        /// <summary>
        /// Gets or sets the drag drop snap.
        /// </summary>
        /// <value>The drag drop snap.</value>
        public int DragDropSnap { get; set; }

        /// <summary>
        /// Gets the drop target control.
        /// </summary>
        /// <value>The drop target control.</value>
        public Control DropTarget { get { return dropTarget; } }

        /// <summary>
        /// Gets the focused control.
        /// </summary>
        /// <value>The focused control.</value>
        [Xml.XmlIgnore]
        public Control FocusedControl
        {
            get
            {
                return _focused;
            }
            internal set
            {
                if (_focused == value) return;
                if (_focused != null) _focused.OnLostFocus();
                _focused = value;
                if (_focused != null) _focused.OnGotFocus();
            }
        }

        /// <summary>
        /// Gets the hot control.
        /// </summary>
        /// <value>The hot control.</value>
        public Control HotControl { get; internal set; }

        [IntColor]
        public int ModalColor { get; set; }

        /// <summary>
        /// Gets the pressed control.
        /// </summary>
        /// <value>The pressed control.</value>
        public Control PressedControl { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show cursor].
        /// </summary>
        /// <value><c>true</c> if [show cursor]; otherwise, <c>false</c>.</value>
        public bool ShowCursor { get; set; }

        [Xml.XmlIgnore]
        public Skin Skin { get; set; }

        /// <summary>
        /// Gets or sets the tooltip control.
        /// </summary>
        /// <value>The tooltip control.</value>
        [Xml.XmlIgnore]
        public Tooltip TooltipControl { get; set; }

        /// <summary>
        /// Closes the dropdowns.
        /// </summary>
        public void CloseDropdowns()
        {
            foreach (Control control in Dropdowns)
                control.Parent = null;

            Dropdowns.Clear();
        }

        /// <summary>
        /// Draws this instance.
        /// </summary>
        public new void Draw()
        {
            Gui.Renderer.Scissor(ClipRect.Left, ClipRect.Top, ClipRect.Width, ClipRect.Height);
            Gui.Renderer.StartBatch();

            base.Draw();

            if (ShowCursor)
                DrawCursor(Gui.MousePosition.x, Gui.MousePosition.y);

            Gui.Renderer.EndBatch(true);
        }

        /// <summary>
        /// Draws the cursor.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void DrawCursor(int x, int y)
        {
            Cursor cursor = GetCursor();

            if (cursor != null)
                cursor.Draw(x, y);
        }

        /// <summary>
        /// Gets the cursor.
        /// </summary>
        /// <returns>Cursor.</returns>
        public Cursor GetCursor()
        {
            if (_cursor == null)
                _cursor = string.Empty;

            if (CursorSet.ContainsKey(_cursor))
                return CursorSet[_cursor];
            else if (CursorSet.ContainsKey("default"))
                return CursorSet["default"];

            return null;
        }

        /// <summary>
        /// returns the style with the given name
        /// </summary>
        /// <param name="name">the name to search for</param>
        /// <returns>matching ControlStyle</returns>
        public ControlStyle GetStyle(string name)
        {
            if (string.IsNullOrEmpty(name))
                return DefaultStyle;

            //if (AdditionalStyles.ContainsKey(name))
            //    return AdditionalStyles[name];

            if (Skin == null)
                return DefaultStyle;

            if (Skin.ContainsKey(name))
                return Skin[name];

            return DefaultStyle;
        }

        /// <summary>
        /// Gets the window at.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>Window.</returns>
        public Window GetWindowAt(int x, int y)
        {
            for (int i = Controls.Count - 1; i >= 0; i--)
            {
                Window w = Controls[i] as Window;
                if (w == null) continue;

                if (w.Enabled && w.Visible && w.Hit(Gui.MousePosition.x, Gui.MousePosition.y))
                    return w;
            }

            return null;
        }

        /// <summary>
        /// Picks the control.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="mode">The mode.</param>
        /// <returns>Control.</returns>
        public Control PickControl(int x, int y, PickMode mode)
        {
            if (mode == PickMode.Control) return PickFirst(x, y);
            if (mode == PickMode.Container) return PickDeep(x, y);
            return null;
        }

        /// <summary>
        /// Sets the hot.
        /// </summary>
        /// <param name="control">The control.</param>
        public void SetHot(Control control) { HotControl = control; }

        /// <summary>
        /// Sets the tooltip.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SetTooltip(Control context)
        {
            if (DesignMode) return;
            if (TooltipControl == null) return;

            if (context != currentContext)
            {
                currentContext = context;
                Elements.Add(TooltipControl);
                TooltipControl.SetContext(context);
            }

            if (TooltipControl.Parent != null)
            {
                if (!TooltipControl.Visible)
                    Elements.Remove(TooltipControl);
                else// if (TooltipControl.AutoLayout)
                    TooltipControl.LayoutTooltip();
            }
            else if (TooltipControl.Visible)
            {
                Elements.Add(TooltipControl);
            }
        }

        /// <summary>
        /// Shows the dropdown.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="stack">if set to <c>true</c> [stack].</param>
        public void ShowDropdown(Control control, bool stack)
        {
            if (DesignMode) return;
            if (control == null) return;

            if (!stack)
                CloseDropdowns();

            if (Dropdowns.Contains(control)) return;

            if (Dropdowns.Count > 0)
            {
                int index = 0;
                bool found = false;

                for (int i = 0; i < Dropdowns.Count; i++)
                {
                    if (Dropdowns[i].Owner == control.Owner)
                    {
                        index = i;
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    for (int i = Dropdowns.Count - 1; i >= index; i--)
                    {
                        Dropdowns[i].Parent = null;
                        Dropdowns.RemoveAt(i);
                    }
                }
            }

            control.Parent = this;
            Dropdowns.Add(control);
        }

        /// <summary>
        /// Tabs the next.
        /// </summary>
        public void TabNext()
        {
            Tab(1);
        }

        /// <summary>
        /// Tabs the previous.
        /// </summary>
        public void TabPrevious()
        {
            Tab(-1);
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        public new void Update()
        {
            if (!DesignMode)
            {
                ProcessDragDrop();

                if (Gui.GetButton(0) == ButtonState.Up)
                    EndDragDrop();
            }

            int pressed = -1;
            int down = -1;

            for (int i = 0; i < Gui.Buttons.Length; i++)
            {
                if (Gui.GetButton(i) == ButtonState.Press)
                    pressed = i;

                if (Gui.GetButton(i) == ButtonState.Down)
                    down = i;
            }

            if (pressed == -1)
            {
                if (TooltipControl != null) TooltipControl.Visible = false;
                hot = GetControlAt(Gui.MousePosition.x, Gui.MousePosition.y);
                if (TooltipControl != null) TooltipControl.Visible = true;

                if (!DesignMode && hot != null && ModalQueue.Count > 0)
                {
                    Control check = ModalQueue[ModalQueue.Count - 1];
                    bool found = check == hot || hot.IsChildOf(check);

                    if (!found && Dropdowns.Count > 0)
                    {
                        for (int i = Dropdowns.Count - 1; i >= 0; i--)
                        {
                            if (Dropdowns[i].Contains(hot))
                            {
                                found = true;
                                break;
                            }
                        }
                    }

                    if (!found && hot.Owner != null)
                        found = hot.Owner.IsChildOf(check);

                    if (!found)
                        hot = this;
                }

                if (hot != HotControl)
                {
                    if (HotControl != null)
                        HotControl.OnMouseLeave();

                    if (hot != null)
                    {
                        CurrentCursor = hot.Cursor;
                        hot.OnMouseEnter();
                    }

                    HotControl = hot;
                }
            }
            else if (pressed > 1)
            {
                hot = null;
            }

            for (int i = 0; i < Gui.Buttons.Length; i++)
            {
                if (Gui.GetButton(i) == ButtonState.Up)
                {
                    if (MouseDownControl != null)
                    {
                        MouseDownControl.OnMouseUp(i);
                        break;
                    }
                }
            }

            if (!DesignMode && down > -1)
            {
                if (ModalQueue.Count == 0)
                {
                    Window w = GetWindowAt(Gui.MousePosition.x, Gui.MousePosition.y);
                    if (w != null && w != window && w.Dock == DockStyle.None)
                    {
                        w.BringToFront();
                        w.Focus();
                        window = w;
                    }
                }

                if (hot != null)
                {
                    if (hot.AllowFocus)
                        FocusedControl = hot;
                    else if (!hot.PreventFocusChange)
                        FocusedControl = null;
                }
                else
                    FocusedControl = null;

                //if(OnClick != null)
                //    OnClick(hot);

                if (Dropdowns.Count > 0)
                {
                    if (hot == null)
                        CloseDropdowns();
                    else
                    {
                        for (int i = Dropdowns.Count - 1; i >= 0; i--)
                        {
                            if (hot != Dropdowns[i])
                            {
                                if (!Dropdowns[i].Contains(hot))
                                {
                                    Dropdowns[i].Parent = null;
                                    Dropdowns.RemoveAt(i);
                                }
                                else break;
                            }
                            else break;
                        }
                    }
                }
            }

            if (!DesignMode)
            {
                if (hot != null)
                    hot.DoEvents();

                DoKeyEvents();

                if (FocusedControl != null)
                    FocusedControl.DoKeyEvents();

                if (IsDragging)
                {
                    SetTooltip(dropTarget);
                }
                else
                {
                    SetTooltip((down > -1 || pressed > -1) ? null : hot);
                }
            }

            PerformUpdate();
            PerformLayout();
            PerformLateUpdate();

            foreach (KeyData data in Gui.KeyBuffer)
            {
                if (data.Pressed && data.Key == Keys.TAB)
                {
                    if (Gui.ShiftPressed)
                        TabPrevious();
                    else
                        TabNext();
                }
            }
        }

        internal Control MouseDownControl;

        internal bool CheckModalLock(Control control)
        {
            if (ModalQueue.Count > 0)
            {
                Control check = ModalQueue[ModalQueue.Count - 1];
                bool found = check == control || control.IsChildOf(check);

                return !found;
            }

            return false;
        }

        internal void DoDragDrop(Control sender, Control data)
        {
            if (IsDragging) return;
            if (data == null) return;

            IsDragging = true;

            DragDropArgs = new DragDropEventArgs();
            DragDropArgs.DraggedControl = data;
            DragDropArgs.Source = sender;

            DragDropOffset = data.Location - Gui.MousePosition;

            DragData = data;
            DragDropSender = sender;
            Controls.Add(DragData);
        }

        internal void RegisterModal(Window control)
        {
            ModalQueue.Add(control);
        }

        internal void Tab(int dir)
        {
            int index = 0;

            if (FocusedControl != null)
                index = FocusedControl.TabIndex;

            index += dir;

            if (index <= 0)
                index = FindHighestTabIndex(0);

            if (index > 0)
            {
                Control result = FindTabIndex(index);

                if (result == null)
                    result = FindTabIndex(1);

                if (result != null)
                    result.Focus();
            }
        }

        internal void UnregisterModal(Window control)
        {
            ModalQueue.Remove(control);
        }

        /// <summary>
        /// Draws the style.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="opacity">The opacity.</param>
        protected override void DrawStyle(Style style, float opacity)
        {
        }

        /// <summary>
        /// The _cursor
        /// </summary>
        private string _cursor;

        private Control _focused;
        private Control currentContext;
        private ControlStyle DefaultStyle = new ControlStyle();
        private Control DragData;
        private DragDropEventArgs DragDropArgs;
        private Point DragDropOffset;
        private Control DragDropSender;
        private List<Control> Dropdowns = new List<Control>();
        private Control dropTarget;
        private Control hot;
        private bool IsDragging;
        private bool isDropInvalid;
        private List<Window> ModalQueue = new List<Window>();
        private Window window;

        private void EndDragDrop()
        {
            if (!IsDragging) return;
            IsDragging = false;

            if (isDropInvalid)
            {
                isDropInvalid = false;

                if (dropTarget != null)
                {
                    DragDropArgs.Cancel = false;
                    dropTarget.OnDragLeave(DragDropArgs);
                }

                return;
            }

            DragDropArgs.Cancel = false;

            if (dropTarget != null)
                dropTarget.OnDragDrop(DragDropArgs);
            else
                OnDragDrop(DragDropArgs);
        }

        private void ProcessDragDrop()
        {
            if (DragData == null) return;

            if (IsDragging)
            {
                DragData.Position = DragDropOffset + Gui.MousePosition;
                DragData.Position = Snap(DragData.Position);

                DragData.Visible = false;
                Control drop = GetDropTarget(DragDropSender);
                DragData.Visible = true;

                if (drop != dropTarget)
                {
                    if (dropTarget != null)
                    {
                        DragDropArgs.Cancel = false;
                        dropTarget.OnDragLeave(DragDropArgs);
                    }

                    dropTarget = drop;

                    if (dropTarget != null)
                    {
                        DragDropArgs.Cancel = false;
                        dropTarget.OnDragEnter(DragDropArgs);
                        if (DragDropArgs.Cancel) isDropInvalid = true;
                    }
                }

                if (dropTarget != null)
                {
                    DragDropArgs.Cancel = false;
                    dropTarget.OnDragResponse(DragDropArgs);
                    if (DragDropArgs.Cancel) isDropInvalid = true;
                }
            }
            else
            {
                Controls.Remove(DragData);
                DragData = null;
                dropTarget = null;
            }
        }

        private Point Snap(Point p)
        {
            if (DragDropSnap > 0)
            {
                int x = (int)System.Math.Floor((float)p.x / DragDropSnap) * DragDropSnap;
                int y = (int)System.Math.Floor((float)p.y / DragDropSnap) * DragDropSnap;

                p = new Point(x, y);
            }

            return p;
        }
    }
}