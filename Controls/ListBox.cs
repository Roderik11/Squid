using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Squid.Xml;

namespace Squid
{
    /// <summary>
    /// A collection of ListBoxItems
    /// </summary>
    public class ListBoxItemCollection : ActiveList<ListBoxItem> { }

    /// <summary>
    /// Delegate SelectedItemChangedEventHandler
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="value">The value.</param>
    public delegate void SelectedItemChangedEventHandler(Control sender, ListBoxItem value);
    /// <summary>
    /// Delegate SelectedItemsChangedEventHandler
    /// </summary>
    /// <param name="sender">The sender.</param>
    public delegate void SelectedItemsChangedEventHandler(Control sender);

    /// <summary>
    /// A ListBox
    /// </summary>
    [Toolbox]
    public class ListBox : Control
    {
        private bool skipEvents;
        private Frame itemContainer;
        private ListBoxItem _selectedItem;
        private readonly ActiveList<ListBoxItem> _selected = new ActiveList<ListBoxItem>();

        /// <summary>
        /// Raised when [selected item changed].
        /// </summary>
        public event SelectedItemChangedEventHandler SelectedItemChanged;

        /// <summary>
        /// Raised when [selected items changed].
        /// </summary>
        public event SelectedItemsChangedEventHandler SelectedItemsChanged;

        /// <summary>
        /// Gets the scrollbar.
        /// </summary>
        /// <value>The scrollbar.</value>
        public ScrollBar Scrollbar { get; private set; }

        /// <summary>
        /// Gets the clip frame.
        /// </summary>
        /// <value>The clip frame.</value>
        public Frame ClipFrame { get; private set; }

        public Frame ItemContainer { get { return itemContainer; } }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        public ListBoxItemCollection Items { get; set; }

        /// <summary>
        /// Gets or sets the scroll.
        /// </summary>
        /// <value>The scroll.</value>
        [Hidden]
        public float Scroll { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ListBox"/> is multiselect.
        /// </summary>
        /// <value><c>true</c> if multiselect; otherwise, <c>false</c>.</value>
        public bool Multiselect { get; set; }

        /// <summary>
        /// Gets or sets the max selected.
        /// </summary>
        /// <value>The max selected.</value>
        public int MaxSelected { get; set; }

        /// <summary>
        /// Gets or sets wether to scroll with the mouse wheel
        /// </summary>
        public bool MouseScroll { get; set; }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        [XmlIgnore]
        public ListBoxItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (value == _selectedItem) return;

                skipEvents = true;

                if (_selectedItem != null)
                    _selectedItem.Selected = false;

                skipEvents = false;

                if (Multiselect)
                    _selected.Clear();

                _selectedItem = value;

                skipEvents = true;

                if (_selectedItem != null)
                    _selectedItem.Selected = true;

                skipEvents = false;

                SelectedItemChanged?.Invoke(this, _selectedItem);
            }
        }

        /// <summary>
        /// Gets the selected items.
        /// </summary>
        /// <value>The selected items.</value>
        [XmlIgnore]
        public ActiveList<ListBoxItem> SelectedItems
        {
            get { return _selected; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListBox"/> class.
        /// </summary>
        public ListBox()
        {
            MouseScroll = true;
            MaxSelected = 0;
            Scroll = 1;
            Size = new Point(100, 100);
            Style = "listbox";

            Items = new ListBoxItemCollection();
            Items.BeforeItemAdded += Items_BeforeItemAdded;
            Items.BeforeItemsCleared += Items_BeforeItemsCleared;
            Items.ItemAdded += Items_ItemAdded;
            Items.ItemRemoved += Items_ItemRemoved;
            Items.ItemsSorted += Items_ItemsSorted;

            Scrollbar = new ScrollBar();
            Scrollbar.Dock = DockStyle.Right;
            Scrollbar.Size = new Point(25, 25);
            Elements.Add(Scrollbar);

            ClipFrame = new Frame();
            ClipFrame.Dock = DockStyle.Fill;
            ClipFrame.Scissor = true;
            Elements.Add(ClipFrame);

            itemContainer = new Frame();
            itemContainer.AutoSize = AutoSize.Vertical;
            ClipFrame.Controls.Add(itemContainer);

            _selected.BeforeItemAdded += _selected_BeforeItemAdded;
            _selected.ItemAdded += _selected_ItemAdded;
            _selected.ItemRemoved += _selected_ItemRemoved;
            _selected.BeforeItemsCleared += _selected_BeforeItemsCleared;

            MouseWheel += ListBox_MouseWheel;
        }

        void ListBox_MouseWheel(Control sender, MouseEventArgs args)
        {
            if (MouseScroll)
            {
                Scrollbar.Scroll(Gui.MouseScroll);
                args.Cancel = true;
            }
        }

        void _selected_BeforeItemsCleared(object sender, EventArgs e)
        {
            skipEvents = true;

            foreach (ListBoxItem item in _selected)
                item.Selected = false;

            if (SelectedItemsChanged != null)
                SelectedItemsChanged(this);

            skipEvents = false;
        }

        void _selected_BeforeItemAdded(object sender, ListEventArgs<ListBoxItem> e)
        {
            if (!Items.Contains(e.Item)) e.Cancel = true;
        }

        void _selected_ItemRemoved(object sender, ListEventArgs<ListBoxItem> e)
        {
            if (e.Item == null) return;

            skipEvents = true;
            e.Item.Selected = false;
            skipEvents = false;

            if (SelectedItemsChanged != null)
                SelectedItemsChanged(this);
        }

        void _selected_ItemAdded(object sender, ListEventArgs<ListBoxItem> e)
        {
            if (e.Item == null) return;

            skipEvents = true;
            e.Item.Selected = true;
            skipEvents = false;

            if (SelectedItemsChanged != null)
                SelectedItemsChanged(this);
        }

        protected override void OnUpdate()
        {
            // force the width to be that of its parent
            itemContainer.Size = new Point(ClipFrame.Size.x, itemContainer.Size.y);

            // move the label up/down using the scrollbar value
            if (itemContainer.Size.y <= ClipFrame.Size.y) // no need to scroll
            {
                Scrollbar.Visible = false; // hide scrollbar
                itemContainer.Position = new Point(0, 0); // set fixed position
            }
            else
            {
                Scrollbar.Scale = Math.Min(1, (float)Size.y / (float)itemContainer.Size.y);
                Scrollbar.Visible = true; // show scrollbar
                itemContainer.Position = new Point(0, (int)((ClipFrame.Size.y - itemContainer.Size.y) * Scrollbar.EasedValue));
            }

            if (Scrollbar.ShowAlways)
                Scrollbar.Visible = true;
        }

        void Items_BeforeItemAdded(object sender, ListEventArgs<ListBoxItem> e)
        {
            if (Items.Contains(e.Item)) e.Cancel = true;
        }

        void Items_ItemsSorted(object sender, EventArgs e)
        {
            itemContainer.Controls.Clear();

            foreach (ListBoxItem item in Items)
                itemContainer.Controls.Add(item);
        }

        void Items_ItemRemoved(object sender, ListEventArgs<ListBoxItem> e)
        {
            itemContainer.Controls.Clear();

            if (e.Item.Selected)
            {
                if (Multiselect)
                    _selected.Remove(e.Item);
                else
                    SelectedItem = null;
            }

            e.Item.Selected = false;
            e.Item.MouseClick -= item_MouseClick;
            e.Item.SelectedChanged -= Item_SelectedChanged;

            foreach (ListBoxItem item in Items)
                itemContainer.Controls.Add(item);
        }

        void Items_ItemAdded(object sender, ListEventArgs<ListBoxItem> e)
        {
            itemContainer.Controls.Clear();

            if (e.Item.Selected)
            {
                if (Multiselect)
                    _selected.Add(e.Item);
                else
                    SelectedItem = e.Item;
            }

            e.Item.MouseClick += item_MouseClick;
            e.Item.SelectedChanged += Item_SelectedChanged;

            foreach (ListBoxItem item in Items)
                itemContainer.Controls.Add(item);
        }

        void Item_SelectedChanged(Control sender)
        {
            ListBoxItem item = sender as ListBoxItem;

            if (skipEvents) return;

            if (item.Selected)
            {
                if (Multiselect)
                    _selected.Add(item);
                else
                    SelectedItem = item;
            }
            else
            {
                if (Multiselect)
                    _selected.Remove(item);
                else
                    SelectedItem = null;
            }
        }

        void Items_BeforeItemsCleared(object sender, EventArgs e)
        {
            skipEvents = true;
            foreach (ListBoxItem item in Items)
            {
                item.MouseClick -= item_MouseClick;
                item.SelectedChanged -= Item_SelectedChanged;
                item.Selected = false;
            }
            skipEvents = false;

            itemContainer.Controls.Clear();
        }

        void item_MouseClick(Control sender, MouseEventArgs args)
        {
            if (args.Button > 0) return;

            ListBoxItem item = sender as ListBoxItem;

            if (Multiselect)
            {
                if (MaxSelected > 0)
                {
                    if (!item.Selected)
                    {
                        if (_selected.Count < MaxSelected)
                            item.Selected = true;
                    }
                    else
                        item.Selected = false;
                }
                else
                {
                    item.Selected = !item.Selected;
                }
            }
            else
            {
                SelectedItem = item;
            }
        }
    }

    /// <summary>
    /// A ListBoxItem. Inherit this to create custom items.
    /// </summary>
    public class ListBoxItem : Button, ISelectable
    {
        private bool _selected;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value { get; set; }

        /// <summary>
        /// Raised when [selected changed].
        /// </summary>
        public event VoidEvent SelectedChanged;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Label" /> is selected.
        /// </summary>
        /// <value><c>true</c> if selected; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        public bool Selected
        {
            get { return _selected; }
            set
            {
                if (value == _selected) return;

                _selected = value;

                SelectedChanged?.Invoke(this);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListBoxItem"/> class.
        /// </summary>
        public ListBoxItem()
        {
            Size = new Point(100, 20);
            Dock = DockStyle.Top;
        }
    }
}
