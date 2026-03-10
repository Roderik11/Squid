using System;
using System.Collections.Generic;

namespace Squid
{
    public partial class Control
    {
        /// <summary>
        /// Return true if the point collides with the control
        /// </summary>
        public bool Hit(int x, int y)
        {
            return x >= ClipRect.Left && x <= ClipRect.Right && y >= ClipRect.Top && y <= ClipRect.Bottom;
        }

        /// <summary>
        /// Returns the first control found below the given screen point
        /// Returns elements and controls
        /// </summary>
        public Control GetControlAt(int x, int y)
        {
            return GetControlAt(x, y, true);
        }

        /// <summary>
        /// Returns the first control found below the given screen point
        /// </summary>
        public Control GetControlAt(int x, int y, bool elements)
        {
            if (!Enabled) return null;
            if (!Visible) return null;
            if (!Hit(x, y)) return null;

            Control found = NoEvents ? null : this;
            if (!elements && _isElement)
                found = null;

            if (IsContainer)
            {
                for (int i = LocalContainer.Controls.Count - 1; i >= 0; i--)
                {
                    Control child = LocalContainer.Controls[i].GetControlAt(x, y, elements);

                    if (child != null && child.Enabled && child.Visible && !child.NoEvents)
                    {
                        found = child;
                        break;
                    }
                }
            }

            for (int i = Elements.Count - 1; i >= 0; i--)
            {
                Control child = Elements[i].GetControlAt(x, y, elements);

                if (child != null && child.Enabled && child.Visible && !child.NoEvents)
                {
                    found = child;
                    break;
                }
            }

            return found;
        }

        /// <summary>
        /// Performs a drag drop operation
        /// </summary>
        public void DoDragDrop(Control data)
        {
            if (Desktop == null) return;
            Desktop.DoDragDrop(this, data);
        }

        public bool InRect(Point start, Point end)
        {
            Rectangle r1 = new Rectangle
            {
                Left = Math.Min(start.x, end.x),
                Top = Math.Min(start.y, end.y),
                Right = Math.Max(start.x, end.x),
                Bottom = Math.Max(start.y, end.y)
            };

            return !(ClipRect.Left > r1.Right
                     || ClipRect.Right < r1.Left
                     || ClipRect.Top > r1.Bottom
                     || ClipRect.Bottom < r1.Top
                    );
        }

        /// <summary>
        /// Override this in custom controls to handle special cases
        /// </summary>
        public virtual bool Contains(Control control)
        {
            if (control == null)
                return false;

            return control.IsChildOf(this);
        }

        /// <summary>
        /// Fires the MouseClick event
        /// </summary>
        public virtual void Click(int button)
        {
            if (Desktop.CheckModalLock(this))
                return;

            OnMouseDown(button);
            OnMouseUp(button);
            OnMouseRelease(button);
        }

        /// <summary>
        /// Processes the events.
        /// </summary>
        public void ProcessEvents() { DoEvents(); }

        private bool selfStateChange = false;

        private void DetermineState()
        {
            if (NoEvents) return;
            if (Desktop == null) return;
            if (externalState != ControlState.Default) return;

            // if (root.DesignMode) return;

            selfStateChange = true;

            if (this is ICheckable checkable && checkable.Checked)
            {
                if (!Enabled)
                    State = ControlState.CheckedDisabled;
                else if (Desktop.FocusedControl == this)
                    State = ControlState.CheckedFocused;
                else if (Desktop.PressedControl == this)
                    State = ControlState.CheckedPressed;
                else if (Desktop.HotControl == this)
                    State = ControlState.CheckedHot;
                else
                    State = ControlState.Checked;
            }
            else if (this is ISelectable selectable && selectable.Selected)
            {
                if (!Enabled)
                    State = ControlState.SelectedDisabled;
                else if (Desktop.FocusedControl == this)
                    State = ControlState.SelectedFocused;
                else if (Desktop.PressedControl == this)
                    State = ControlState.SelectedPressed;
                else if (Desktop.HotControl == this)
                    State = ControlState.SelectedHot;
                else
                    State = ControlState.Selected;
            }
            else if (!Enabled)
                State = ControlState.Disabled;
            else if (Desktop.FocusedControl == this)
                State = ControlState.Focused;
            else if (Desktop.PressedControl == this)
                State = ControlState.Pressed;
            else if (Desktop.HotControl == this)
                State = ControlState.Hot;
            else
                State = ControlState.Default;

            selfStateChange = false;
        }

        internal void DoKeyEvents()
        {
            if (NoEvents) return;

            foreach (KeyData data in Gui.KeyEvents)
            {
                KeyEventArgs args = new KeyEventArgs(data);

                if (data.Pressed)
                {
                    if (KeyDown != null)
                    {
                        KeyDown(this, args);

                        if (!args.Cancel)
                            OnKeyDown(args);
                    }
                    else
                        OnKeyDown(args);
                }

                if (data.Released)
                {
                    if (KeyUp != null)
                    {
                        KeyUp(this, args);

                        if (!args.Cancel)
                            OnKeyUp(args);
                    }
                    else
                        OnKeyUp(args);
                }
            }
        }

        private Point pointDown;

        internal void DoEvents()
        {
            if (NoEvents) return;
            if (Desktop == null) return;

            if (Gui.MouseScroll != 0)
                OnMouseWheel();

            var count = Gui.Buttons.Length;
            for (int i = 0; i < count; i++)
            {
                var state = Gui.GetButton(i);

                switch(state)
                {
                    case ButtonState.Down:
                        _isMouseDrag = false;
                        OnMouseDown(i);
                        pointDown = Gui.MousePosition;
                        return;

                    case ButtonState.Press:
                        OnMousePress(i);
                        var mp = pointDown - Gui.MousePosition;
                        bool isdrag = ((mp.x * mp.x) + (mp.y * mp.y)) > (Gui.DragThreshold * Gui.DragThreshold);

                        if (isdrag && !_isMouseDrag)
                        {
                            _isMouseDrag = true;
                            OnMouseDrag(i);
                        }
                        return;

                    case ButtonState.Up:
                        OnMouseRelease(i);
                        return;
                }
            }

            if (Desktop.MouseDownControl != null)
            {
                Desktop.PressedControl = null;
                Desktop.MouseDownControl = null;
            }
        }

        internal Control PickDeep(int x, int y)
        {
            if (!Visible) return null;
            if (!Hit(x, y)) return null;

            Control found = null;

            if (IsContainer)
            {
                found = this;

                for (int i = LocalContainer.Controls.Count - 1; i >= 0; i--)
                {
                    Control child = LocalContainer.Controls[i].PickDeep(x, y);

                    if (child != null && child.Visible)
                    {
                        found = child;
                        break;
                    }
                }
            }

            for (int i = Elements.Count - 1; i >= 0; i--)
            {
                Control child = Elements[i].PickDeep(x, y);

                if (child != null && child.Visible)
                {
                    found = child;
                    break;
                }
            }

            return found;
        }

        internal Control PickFirst(int x, int y)
        {
            if (!Visible) return null;
            if (!Hit(x, y)) return null;

            Control found = _isElement ? null : this;

            if (IsContainer)
            {
                for (int i = LocalContainer.Controls.Count - 1; i >= 0; i--)
                {
                    Control child = LocalContainer.Controls[i].PickFirst(x, y);

                    if (child != null && child.Visible && !child._isElement)
                    {
                        found = child;
                        break;
                    }
                }
            }

            for (int i = Elements.Count - 1; i >= 0; i--)
            {
                Control child = Elements[i].PickFirst(x, y);

                if (child != null && child.Visible && !child.IsContainer)
                {
                    found = child;
                    break;
                }
            }

            return found;
        }

        internal Control GetDropTarget(Control sender)
        {
            if (!Visible) return null;

            int x = Gui.MousePosition.x;
            int y = Gui.MousePosition.y;

            if (!Hit(x, y)) return null;

            Control found = Enabled && Visible && AllowDrop ? this : null;

            if (IsContainer)
            {
                for (int i = LocalContainer.Controls.Count - 1; i >= 0; i--)
                {
                    Control child = LocalContainer.Controls[i].GetDropTarget(sender);
                    if (child == null) continue;

                    if (sender != child && child.Enabled && child.Visible && !child.NoEvents && child.AllowDrop)
                    {
                        found = child;
                        break;
                    }
                }
            }

            for (int i = Elements.Count - 1; i >= 0; i--)
            {
                Control child = Elements[i].GetDropTarget(sender);
                if (child == null) continue;

                if (sender != child && child.Enabled && child.Visible && !child.NoEvents && child.AllowDrop)
                {
                    found = child;
                    break;
                }
            }

            return found;
        }

        internal Control FindTabIndex(int index)
        {
            Control control = null;
            IList<Control> controls = Elements;

            if (!Visible || Desktop.CheckModalLock(this))
                return null;

            if (!NoEvents && Enabled && TabIndex == index)
                control = this;

            for (int i = 0; i < controls.Count; i++)
            {
                Control child = controls[i].FindTabIndex(index);

                if (child != null && !child.NoEvents && child.Visible && child.Enabled && child.TabIndex == index)
                {
                    control = child;
                    break;
                }
            }

            if (IsContainer)
            {
                controls = LocalContainer.Controls;

                for (int i = 0; i < controls.Count; i++)
                {
                    Control child = controls[i].FindTabIndex(index);

                    if (child != null && !child.NoEvents && child.Visible && child.Enabled && child.TabIndex == index)
                    {
                        control = child;
                        break;
                    }
                }
            }

            return control;
        }

        internal int FindHighestTabIndex(int max)
        {
            int index = max;
            IList<Control> all = Elements;

            if (!Visible)
                return index;

            if (!NoEvents && Enabled && TabIndex == index)
                index = TabIndex;

            for (int i = 0; i < all.Count; i++)
            {
                int result = all[i].FindHighestTabIndex(index);

                if (result > index)
                    index = result;
            }

            if (IsContainer)
            {
                all = LocalContainer.Controls;

                for (int i = 0; i < all.Count; i++)
                {
                    int result = all[i].FindHighestTabIndex(index);

                    if (result > index)
                        index = result;
                }
            }

            return index;
        }
    }
}
