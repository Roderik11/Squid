using System;
using System.Collections.Generic;
using System.Text;

namespace Squid
{
    /// <summary>
    /// A DropDownButton
    /// </summary>
    public class DropDownButton : Button
    {
        /// <summary>
        /// Gets the dropdown.
        /// </summary>
        /// <value>The dropdown.</value>
        public Window Dropdown { get; private set; }
        /// <summary>
        /// Gets or sets the align.
        /// </summary>
        /// <value>The align.</value>
        public Alignment Align { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [hot drop].
        /// </summary>
        /// <value><c>true</c> if [hot drop]; otherwise, <c>false</c>.</value>
        public bool HotDrop { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is open.
        /// </summary>
        /// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
        public bool IsOpen { get; private set; }

        /// <summary>
        /// Raised when [on closed].
        /// </summary>
        public event EventWithArgs OnClosed;
        /// <summary>
        /// Raised when [on opened].
        /// </summary>
        public event EventWithArgs OnOpened;
        /// <summary>
        /// Raised when [on opening].
        /// </summary>
        public event EventWithArgs OnOpening;
        /// <summary>
        /// Raised when [on closing].
        /// </summary>
        public event EventWithArgs OnClosing;

        /// <summary>
        /// Initializes a new instance of the <see cref="DropDownButton"/> class.
        /// </summary>
        public DropDownButton()
        {
            Dropdown = new Window();
            Dropdown.Size = new Point(100, 200);
            Dropdown.Resizable = true;
            Dropdown.Scissor = false;

            Align = Alignment.BottomLeft;

            MouseClick += Button_MouseClick;
            MouseDown += Button_MouseDown;
            MouseEnter += Button_MouseEnter;
        }

        /// <summary>
        /// Opens this instance.
        /// </summary>
        public void Open()
        {
            if (OnOpening != null)
            {
                SquidEventArgs args = new SquidEventArgs();
                OnOpening(this, args);
                if (args.Cancel) return;
            }

            if (HotDrop && Dropdown.Controls.Count == 0) return;

            Dropdown.Owner = Parent;

            switch (Align)
            {
                case Alignment.BottomLeft:
                    Dropdown.Position = Location + new Point(0, Size.y);
                    break;
                case Alignment.TopRight:
                    Dropdown.Position = Location + new Point(Size.x, 0);
                    break;
                case Alignment.TopLeft:
                    Dropdown.Position = Location - new Point(Dropdown.Size.x, 0);
                    break;
            }

            Desktop.ShowDropdown(Dropdown, true);
            IsOpen = true;

            if (OnOpened != null)
                OnOpened(this, null);
        }

        public void Close()
        {
            if (Desktop == null) return;

            if (OnClosing != null)
            {
                SquidEventArgs args = new SquidEventArgs();
                OnClosing(this, args);
                if (args.Cancel) return;
            }

            Desktop.CloseDropdowns();
            IsOpen = false;

            if (OnClosed != null)
                OnClosed(this, null);
        }

        public override bool Contains(Control control)
        {
            if (control.IsChildOf(this))
                return true;

            return control.IsChildOf(Dropdown);
        }

        void Button_MouseEnter(Control sender)
        {
            if (HotDrop) Open();
        }

        void Button_MouseDown(Control sender, MouseEventArgs args)
        {
            if (Dropdown.Parent == null) IsOpen = false;
        }

        void Button_MouseClick(Control sender, MouseEventArgs args)
        {
            if (args.Button > 0) return;

            if (IsOpen)
                Close();
            else
                Open();
        }
    }
}
