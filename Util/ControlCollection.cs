using System;
using System.Collections.Generic;
using System.Text;

namespace Squid
{
    /// <summary>
    /// A collection of controls. This class requires a parent Control.
    /// </summary>
    public class ControlCollection : ActiveList<Control>
    {
        internal bool IsLocked;

        private bool dirtyAdd;
        private bool dirtyRemove;

        private List<Control> toAdd = new List<Control>();

        internal void Cleanup()
        {
            if (dirtyAdd)
            {
                foreach (Control c in toAdd)
                {
                    base.Add(c);

                    c._isElement = false;
                    c.IsRemoved = false;
                    c.ParentControl = Parent;
                }

                toAdd.Clear();
                dirtyAdd = false;
            }

            if (dirtyRemove)
            {
                int count = Count;
                for (int i = count - 1; i >= 0; i--)
                {
                    if (this[i].IsRemoved)
                    {
                        Control child = this[i];
                        this.RemoveAt(i);
                        child.Parent = null;
                    }
                }

                dirtyRemove = false;
            }
        }

        public Control Parent { get; private set; }

        public ControlCollection(Control parent)
        {
            Parent = parent;

            BeforeItemAdded += Items_BeforeItemAdded;
            BeforeItemRemoved += Items_BeforeItemRemoved;
            BeforeItemsCleared += Items_BeforeItemsCleared;
        }

        void Items_BeforeItemsCleared(object sender, EventArgs e)
        {
            foreach (Control control in this)
                control.ParentControl = null;
        }

        void Items_BeforeItemRemoved(object sender, ListEventArgs<Control> e)
        {
            // item is Null or item is our parent
            if (e.Item == null || e.Item == Parent)
            {
                e.Cancel = true;
                return;
            }

            // we dont own this item
            if (e.Item._isElement || e.Item.Parent != Parent)
            {
                e.Cancel = true;
                return;
            }

            e.Item.IsRemoved = true;
            dirtyRemove = true;

            if (IsLocked)
                e.Cancel = true;
            else
                e.Item.ParentControl = null;
        }

        void Items_BeforeItemAdded(object sender, ListEventArgs<Control> e)
        {
            // item is Null or item is our parent
            if (e.Item == null || e.Item == Parent)
            {
                e.Cancel = true;
                return;
            }

            // we already own this item
            if (e.Item.Parent == Parent)
            {
                e.Cancel = true;
                return;
            }

            if (e.Item.Container != null)
            {
                if (!e.Item.Container.Controls.Remove(e.Item))
                {
                    e.Cancel = true;
                    return;
                }
            }

            e.Item._isElement = false;

            if (IsLocked)
            {
                e.Cancel = true;
                toAdd.Add(e.Item);
                dirtyAdd = true;
                return;
            }

            e.Item.IsRemoved = false;
            e.Item.ParentControl = Parent;
        }
    }

    /// <summary>
    /// A collection of elements. This class requires a parent Control.
    /// </summary>
    public class ElementCollection : ActiveList<Control>
    {
        internal bool IsLocked;

        private bool dirtyAdd;
        private bool dirtyRemove;

        private List<Control> toAdd = new List<Control>();

        internal void Cleanup()
        {
            if (dirtyAdd)
            {
                foreach (Control c in toAdd)
                {
                    base.Add(c);

                    c._isElement = true;
                    c.IsRemoved = false;
                    c.ParentControl = Parent;
                }

                toAdd.Clear();
                dirtyAdd = false;
            }

            if (dirtyRemove)
            {
                int count = Count;
                for (int i = count - 1; i >= 0; i--)
                {
                    if (this[i].IsRemoved)
                    {
                        Control child = this[i];
                        this.RemoveAt(i);
                        child.ParentControl = null;
                    }
                }

                dirtyRemove = false;
            }
        }

        public Control Parent { get; private set; }

        public ElementCollection(Control parent)
        {
            Parent = parent;

            BeforeItemAdded += Items_BeforeItemAdded;
            BeforeItemRemoved += Items_BeforeItemRemoved;
            BeforeItemsCleared += Items_BeforeItemsCleared;
        }

        void Items_BeforeItemsCleared(object sender, EventArgs e)
        {
            foreach (Control control in this)
                control.ParentControl = null;
        }

        void Items_BeforeItemRemoved(object sender, ListEventArgs<Control> e)
        {
            // item is Null or item is our parent
            if (e.Item == null || e.Item == Parent)
            {
                e.Cancel = true;
                return;
            }

            // we dont own this item
            if (e.Item.Parent != Parent)
            {
                e.Cancel = true;
                return;
            }

            e.Item.IsRemoved = true;
            dirtyRemove = true;

            if (IsLocked)
                e.Cancel = true;
            else
                e.Item.ParentControl = null;
        }

        void Items_BeforeItemAdded(object sender, ListEventArgs<Control> e)
        {
            // item is Null or item is our parent
            if (e.Item == null || e.Item == Parent)
            {
                e.Cancel = true;
                return;
            }

            // we already own this item
            if (e.Item.Parent == Parent)
            {
                e.Cancel = true;
                return;
            }

            if (e.Item.Parent != null)
            {
                e.Cancel = true;
                return;
            }

            e.Item._isElement = true;

            if (IsLocked)
            {
                e.Cancel = true;
                toAdd.Add(e.Item);
                dirtyAdd = true;
                return;
            }

            e.Item.IsRemoved = false;
            e.Item.ParentControl = Parent;
        }
    }

}
