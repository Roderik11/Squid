using System;

namespace Squid
{
    public partial class Control
    {
        /// <summary>
        /// Raises the <see cref="E:DragEnter" /> event.
        /// </summary>
        internal void OnDragEnter(DragDropEventArgs e)
        {
            if (e.Cancel) return;
            DragEnter?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:DragLeave" /> event.
        /// </summary>
        internal void OnDragLeave(DragDropEventArgs e)
        {
            if (e.Cancel) return;
            DragLeave?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:DragResponse" /> event.
        /// </summary>
        internal void OnDragResponse(DragDropEventArgs e)
        {
            if (e.Cancel) return;
            DragResponse?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:DragDrop" /> event.
        /// </summary>
        internal void OnDragDrop(DragDropEventArgs e)
        {
            DragDrop?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:Dropped" /> event.
        /// </summary>
        internal void OnDrop(DragDropEventArgs e)
        {
            Drop?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:MouseDoubleClick" /> event.
        /// </summary>
        internal void OnMouseDoubleClick(int button)
        {
            MouseDoubleClick?.Invoke(this, new MouseEventArgs { Button = button });
        }

        /// <summary>
        /// Raised the MouseDrag event
        /// </summary>
        internal void OnMouseDrag(int button)
        {
            MouseDrag?.Invoke(this, new MouseEventArgs { Button = button });
        }

        /// <summary>
        /// Raised the MousePress event
        /// </summary>
        internal void OnMousePress(int button)
        {
            if (Desktop == null) return;

            //if(button == 0)
            Desktop.PressedControl = this;
            Desktop.MouseDownControl = this;

            MousePress?.Invoke(this, new MouseEventArgs { Button = button });
        }

        /// <summary>
        /// Raised the MouseClick event
        /// </summary>
        internal void OnMouseClick(int button)
        {
            MouseClick?.Invoke(this, new MouseEventArgs { Button = button });
        }

        /// <summary>
        /// Raised the MouseRelease event
        /// </summary>
        internal void OnMouseRelease(int button)
        {
            if (Desktop == null) return;

            if (Desktop.MouseDownControl != this) return;

            OnMouseClick(button);

            if (IsDoubleClick)
            {
                IsDoubleClick = false;
                OnMouseDoubleClick(button);
            }
        }

        /// <summary>
        /// Raised the MouseDown event
        /// </summary>
        internal void OnMouseDown(int button)
        {
            if (Desktop == null) return;

            //if (button == 0)
            Desktop.PressedControl = this;
            Desktop.MouseDownControl = this;

            DateTime now = DateTime.Now;
            TimeSpan delta = now.Subtract(TimeClicked);
            TimeClicked = now;
            IsDoubleClick = delta.TotalMilliseconds < Gui.DoubleClickSpeed;

            var args = new MouseEventArgs { Button = button };
            MouseDown?.Invoke(this, args);
            Gui.OnMouseDown(this, args);
        }

        /// <summary>
        /// Raised the MouseEnter event
        /// </summary>
        internal void OnMouseEnter()
        {
            MouseEnter?.Invoke(this);
        }

        /// <summary>
        /// Raised the MouseLeave event
        /// </summary>
        internal void OnMouseLeave()
        {
            MouseLeave?.Invoke(this);
        }

        /// <summary>
        /// Raised the MouseUp event
        /// </summary>
        internal void OnMouseUp(int button)
        {
            var args = new MouseEventArgs { Button = button };
            MouseUp?.Invoke(this, args);
            Gui.OnMouseUp(this, args);
        }

        internal void OnMouseWheel()
        {
            if (MouseWheel != null)
            {
                MouseEventArgs args = new MouseEventArgs();
                MouseWheel(this, args);
                if (args.Cancel) return;
            }

            _parent?.OnMouseWheel();
        }

        /// <summary>
        /// Raised the GotFocus event
        /// </summary>
        internal void OnGotFocus()
        {
            GotFocus?.Invoke(this);
        }

        /// <summary>
        /// Raised the LostFocus event
        /// </summary>
        internal void OnLostFocus()
        {
            LostFocus?.Invoke(this);
        }
    }
}
