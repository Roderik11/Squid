using System;
using System.Collections.Generic;
using System.Text;

namespace Squid
{
    /// <summary>
    /// A DropDownList
    /// </summary>
    [Toolbox]
    public class DropDownList : Control
    {
        private ListBoxItem _selectedItem;
        private bool WasOpenOnce;

        /// <summary>
        /// Raised when [selected item changed].
        /// </summary>
        public event SelectedItemChangedEventHandler SelectedItemChanged;

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
        /// Gets the label.
        /// </summary>
        /// <value>The label.</value>
        public Label Label { get; private set; }
        /// <summary>
        /// Gets the button.
        /// </summary>
        /// <value>The button.</value>
        public Button Button { get; private set; }
        /// <summary>
        /// Gets the listbox.
        /// </summary>
        /// <value>The listbox.</value>
        public ListBox Listbox { get; private set; }
        /// <summary>
        /// Gets the dropdown.
        /// </summary>
        /// <value>The dropdown.</value>
        public Window Dropdown { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is open.
        /// </summary>
        /// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
        public bool IsOpen { get; private set; }

        /// <summary>
        /// Gets or sets the size of the dropdown.
        /// </summary>
        /// <value>The size of the dropdown.</value>
        public Point DropdownSize { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [dropdown auto size].
        /// </summary>
        /// <value><c>true</c> if [dropdown auto size]; otherwise, <c>false</c>.</value>
        public bool DropdownAutoSize { get; set; }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        public ActiveList<ListBoxItem> Items
        {
            get { return Listbox.Items; }
            private set { }
        }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        public ListBoxItem SelectedItem
        {
            get { return Listbox.SelectedItem; }
            set { Listbox.SelectedItem = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DropDownList"/> class.
        /// </summary>
        public DropDownList()
        {
            DropdownAutoSize = true;
            DropdownSize = new Point(100, 100);
            Style = "dropdown";

            Button = new Button();
            Button.Size = new Point(30, 30);
            Button.Dock = DockStyle.Right;
            Button.MouseClick += Button_MouseClick;
            Button.Style = "dropdownButton";
            Elements.Add(Button);

            Label = new Label();
            Label.Dock = DockStyle.Fill;
            Label.MouseClick += Button_MouseClick;
            Label.Style = "dropdownLabel";
            Elements.Add(Label);

            Dropdown = new Window();
            Dropdown.Resizable = true;
            Dropdown.Scissor = false;
            Dropdown.Style = "frame";

            Listbox = new ListBox();
            Listbox.Dock = DockStyle.Fill;
            Listbox.SelectedItemChanged += Listbox_SelectedItemChanged;
            Listbox.Items.ItemAdded += Items_ItemAdded;
            Listbox.Items.ItemRemoved += Items_ItemRemoved;
            Listbox.Multiselect = false;
            Dropdown.Controls.Add(Listbox);
        }

        void Items_ItemRemoved(object sender, ListEventArgs<ListBoxItem> e)
        {
            if (e.Item.Selected)
            {
                Label.Text = string.Empty;
            
                if (SelectedItemChanged != null)
                    SelectedItemChanged(this, e.Item);
            }
        }

        void Items_ItemAdded(object sender, ListEventArgs<ListBoxItem> e)
        {
            if (e.Item.Selected)
            {
                Label.Text = e.Item.Text;

                if (SelectedItemChanged != null)
                    SelectedItemChanged(this, e.Item);
            }
        }

        void Listbox_SelectedItemChanged(Control sender, ListBoxItem value)
        {
            Label.Text = value != null ? value.Text : string.Empty;

            if (SelectedItemChanged != null)
                SelectedItemChanged(this, value);

            Close();
        }

        public void Open()
        {
            if (OnOpening != null)
            {
                SquidEventArgs args = new SquidEventArgs();
                OnOpening(this, args);
                if (args.Cancel) return;
            }

            if (!WasOpenOnce)
            {
                if (DropdownAutoSize)
                    Dropdown.Size = new Point(Size.x, DropdownSize.y);
                else
                    Dropdown.Size = new Point(DropdownSize.x, DropdownSize.y);

                WasOpenOnce = true;
            }

            Dropdown.Position = Location + new Point(0, Size.y);

            Desktop.ShowDropdown(Dropdown, false);
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
