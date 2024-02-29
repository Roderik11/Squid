using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Squid
{
    /// <summary>
    /// Delegate SelectedNodeChangedEventHandler
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="value">The value.</param>
    public delegate void SelectedNodeChangedEventHandler(Control sender, TreeNode value);

    /// <summary>
    /// A TreeView
    /// </summary>
    [Toolbox]
    public class TreeView : Control
    {
        private TreeNode _selectedNode;

        /// <summary>
        /// Raised when [selected node changed].
        /// </summary>
        public event SelectedNodeChangedEventHandler SelectedNodeChanged;

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

        public Frame ItemContainer { get; private set; }

        /// <summary>
        /// Gets the nodes.
        /// </summary>
        /// <value>The nodes.</value>
        public ActiveList<TreeNode> Nodes { get; private set; }

        /// <summary>
        /// Gets or sets the selected node.
        /// </summary>
        /// <value>The selected node.</value>
        public TreeNode SelectedNode
        {
            get { return _selectedNode; }
            set
            {
                if (value == _selectedNode) return;

                if (_selectedNode != null) 
                    _selectedNode.Selected = false;

                _selectedNode = value;

                if (_selectedNode != null)
                {
                    _selectedNode.Selected = true;
                    _selectedNode.Focus();
                }

                SelectedNodeChanged?.Invoke(this, _selectedNode);
            }
        }

        /// <summary>
        /// Gets or sets the indent.
        /// </summary>
        /// <value>The indent.</value>
        [DefaultValue(0)]
        public int Indent { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeView"/> class.
        /// </summary>
        public TreeView()
        {
            Size = new Point(100, 100);
            Style = "treeview";

            Nodes = new ActiveList<TreeNode>();
            Nodes.ItemAdded += Nodes_ItemAdded;
            Nodes.ItemRemoved += Nodes_ItemRemoved;
            Nodes.BeforeItemsCleared += Nodes_BeforeItemsCleared;

            Scrollbar = new ScrollBar();
            Scrollbar.Dock = DockStyle.Right;
            Scrollbar.Size = new Point(25, 25);
            Scrollbar.Orientation = Orientation.Vertical;
            Elements.Add(Scrollbar);

            ClipFrame = new Frame();
            ClipFrame.Dock = DockStyle.Fill;
            ClipFrame.Scissor = true;
            Elements.Add(ClipFrame);

            ItemContainer = new Frame();
            ItemContainer.AutoSize = AutoSize.Vertical;
            ItemContainer.Dock = DockStyle.FillX;
            ItemContainer.Parent = ClipFrame;

            MouseWheel += TreeView_MouseWheel;
        }

        public void NextNode()
        {
            var selected = SelectedNode;
            var index = ItemContainer.Controls.IndexOf(selected);
            var next = Math.Min(index + 1, ItemContainer.Controls.Count - 1);
            SelectedNode = ItemContainer.Controls[next] as TreeNode;
        }

        public void PreviousNode()
        {
            var selected = SelectedNode;
            var index = ItemContainer.Controls.IndexOf(selected);
            var prev = Math.Max(index - 1, 0);
            SelectedNode = ItemContainer.Controls[prev] as TreeNode;
        }

        public TreeNode FindNode(object value, TreeNode parent = null)
        {
            var nodes = parent != null ? parent.Nodes : Nodes;

            foreach (var node in nodes)
            {
                if(node.Value == value)return node;

                var found = FindNode(value, node);
                if(found != null) return found;
            }

            return null;
        }

        public void ExpandTo(TreeNode node)
        {
            while(node.ParentNode != null)
            {
                node.ParentNode.Expanded = true;
                node = node.ParentNode;
            }
        }

        public void ScrollTo(TreeNode node)
        {
            if (node == null) return;
            var pos = (float)node.Position.y / (ItemContainer.Size.y - ClipFrame.Size.y);
            pos -= (float)(node.Size.y * 3) / (ItemContainer.Size.y - ClipFrame.Size.y);
            Scrollbar.SetValue(pos);
        }

        void TreeView_MouseWheel(Control sender, MouseEventArgs args)
        {
            Scrollbar.Scroll(Gui.MouseScroll);
            args.Cancel = true;
        }

        protected override void OnUpdate()
        {
            bool visible = Scrollbar.ShowAlways || ItemContainer.Size.y > ClipFrame.Size.y;
            Scrollbar.Visible = visible;
            Scrollbar.Scale = visible ? Math.Min(1, (float)Size.y / ItemContainer.Size.y) : 1;
            ItemContainer.Position = visible ? new Point(0, (int)((ClipFrame.Size.y - ItemContainer.Size.y) * Scrollbar.EasedValue)) : Point.Zero;
        }

        void Nodes_BeforeItemsCleared(object sender, EventArgs e)
        {
            foreach (TreeNode node in Nodes)
                Nodes_ItemRemoved(sender, new ListEventArgs<TreeNode>(node));
        }

        void Nodes_ItemRemoved(object sender, ListEventArgs<TreeNode> e)
        {
            e.Item.ExpandedChanged -=Item_ExpandedChanged;
            e.Item.SelectedChanged -=Item_SelectedChanged;
            e.Item.treeview = null;

            ItemContainer.Controls.Remove(e.Item);

            foreach (TreeNode child in e.Item.Nodes)
                Nodes_ItemRemoved(sender, new ListEventArgs<TreeNode>(child));
        }

        void Item_SelectedChanged(Control sender)
        {
            if (!(sender is TreeNode node)) return;

            if (node.Selected)
                SelectedNode = node;
            else if (node == SelectedNode)
                SelectedNode = null;
        }

        void Item_ExpandedChanged(Control sender)
        {
            TreeNode node = sender as TreeNode;
           
            if (!node.Expanded)
            {
                List<TreeNode> nodes = FindExpandedNodes(node);
                foreach (TreeNode child in nodes)
                {
                    child.ExpandedChanged -= Item_ExpandedChanged;
                    child.SelectedChanged -= Item_SelectedChanged;
                    child.treeview = null;

                    ItemContainer.Controls.Remove(child);
                }
            }
            else
            {
                int i = ItemContainer.Controls.IndexOf(node) + 1;
                List<TreeNode> nodes = FindExpandedNodes(node);
                foreach (TreeNode child in nodes)
                {
                    child.ExpandedChanged += Item_ExpandedChanged;
                    child.SelectedChanged += Item_SelectedChanged;
                    child.treeview = this;

                    ItemContainer.Controls.Insert(i, child);
                    i++;
                }
            }
        }

        void Nodes_ItemAdded(object sender, ListEventArgs<TreeNode> e)
        {
            e.Item.NodeDepth = 0;
            e.Item.ExpandedChanged += Item_ExpandedChanged;
            e.Item.SelectedChanged += Item_SelectedChanged;
            e.Item.treeview = this;

            ItemContainer.Controls.Add(e.Item);
        }

        internal void RemoveNode(TreeNode node)
        {
            ItemContainer.Controls.Remove(node);

            List<TreeNode> nodes = FindExpandedNodes(node);
            foreach (TreeNode child in nodes)
            {
                child.ExpandedChanged -= Item_ExpandedChanged;
                child.SelectedChanged -= Item_SelectedChanged;
                ItemContainer.Controls.Remove(child);
            }
        }

        private List<TreeNode> FindExpandedNodes(TreeNode parent)
        {
            List<TreeNode> list = new List<TreeNode>();

            foreach (TreeNode node in parent.Nodes)
            {
                list.Add(node);

                if (node.Expanded)
                    list.AddRange(FindExpandedNodes(node));
            }

            return list;
        }
    }

    /// <summary>
    /// A collection of TreeNodes
    /// </summary>
    public class TreeNodeCollection : ActiveList<TreeNode> { }

    /// <summary>
    /// A TreeNode. Inherit this to create custom nodes.
    /// </summary>
    public class TreeNode : Control, ISelectable
    {
        private bool _selected;
        private bool _expanded;
        private bool _suspendEvents;
        private int _depth;
        /// <summary>
        /// Raised when [on selected changed].
        /// </summary>
        public event VoidEvent SelectedChanged;

        public TreeView treeview { get; internal set; }

        /// <summary>
        /// Raised when [on exppanded changed].
        /// </summary>
        public event VoidEvent ExpandedChanged;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TreeNode"/> is selected.
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
        /// Gets or sets a value indicating whether this <see cref="TreeNode"/> is expanded.
        /// </summary>
        /// <value><c>true</c> if selected; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        public bool Expanded
        {
            get { return _expanded; }
            set
            {
                if (value == _expanded) return;
                if (Nodes.Count == 0) return;

                _expanded = value;

                if (!_suspendEvents)
                    ExpandedChanged?.Invoke(this);
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value { get; set; }

        /// <summary>
        /// Gets the node depth.
        /// </summary>
        /// <value>The node depth.</value>
        public int NodeDepth
        {
            get => _depth;
            set { _depth = value; OnDepthChanged(); }

        }

        /// <summary>
        /// Gets or sets the nodes.
        /// </summary>
        /// <value>The nodes.</value>
        public TreeNodeCollection Nodes { get; set; }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public TreeNode ParentNode { get; private set; }

        public TreeNode()
        {
            Nodes = new TreeNodeCollection();
            Nodes.ItemAdded += Nodes_ItemAdded;
            Nodes.ItemRemoved += Nodes_ItemRemoved;
            Nodes.BeforeItemsCleared += Nodes_BeforeItemsCleared;

            Size = new Point(100, 20);
            Dock = DockStyle.Top;
        }

        protected virtual void OnDepthChanged() { }

        //protected override void OnUpdate()
        //{
        //    base.OnUpdate();

        //    if (treeview != null && treeview.Indent != 0)
        //    {
        //        Margin m = Margin;
        //        Margin = new Margin(treeview.Indent * NodeDepth, m.Top, m.Right, m.Bottom);
        //    }
        //}

        void Nodes_BeforeItemsCleared(object sender, EventArgs e)
        {
            foreach (TreeNode node in Nodes)
            {
                node.ParentNode = null;

                treeview?.RemoveNode(node);
            }
        }

        void Nodes_ItemRemoved(object sender, ListEventArgs<TreeNode> e)
        {
            treeview?.RemoveNode(e.Item);

            e.Item.ParentNode = null;
        }

        void Nodes_ItemAdded(object sender, ListEventArgs<TreeNode> e)
        {
            e.Item.NodeDepth = NodeDepth + 1;
            e.Item.ParentNode = this;

            if (treeview != null && Expanded)
            {
                _suspendEvents = true;
                Expanded = false;
                Expanded = true;
                _suspendEvents = false;
            }
        }

        public void Remove()
        {
            if (ParentNode != null)
                ParentNode.Nodes.Remove(this);
            else if (treeview != null)
            {
                treeview.Nodes.Remove(this);
            }
        }
    }

    /// <summary>
    /// A TreeNode using a DropDownButton and a Button to expand.
    /// </summary>
    public class TreeNodeDropDown : TreeNode
    {
        /// <summary>
        /// Gets the button.
        /// </summary>
        /// <value>The button.</value>
        public Button Button { get; private set; }
        /// <summary>
        /// Gets the drop down button.
        /// </summary>
        /// <value>The drop down button.</value>
        public DropDownButton DropDownButton { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNodeDropDown"/> class.
        /// </summary>
        public TreeNodeDropDown()
        {
            Button = new Button();
            Button.Size = new Point(20, 20);
            Button.Margin = new Margin(6);
            Button.Dock = DockStyle.Left;
            Button.MouseClick += Button_MouseClick;
            Elements.Add(Button);

            DropDownButton = new DropDownButton();
            DropDownButton.Size = new Point(20, 20);
            DropDownButton.Dock = DockStyle.Fill;
            Elements.Add(DropDownButton);
        }

        /// <summary>
        /// Button_s the mouse click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        void Button_MouseClick(Control sender, MouseEventArgs args)
        {
            if (args.Button > 0) return;

            Expanded = !Expanded;
        }
    }

    /// <summary>
    /// A TreeNode using a Label and a Button to expand
    /// </summary>
    public class TreeNodeLabel : TreeNode
    {
        /// <summary>
        /// Gets the button.
        /// </summary>
        /// <value>The button.</value>
        public Button Button { get; private set; }
        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <value>The label.</value>
        public Label Label { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNodeLabel"/> class.
        /// </summary>
        public TreeNodeLabel()
        {
            Button = new Button();
            Button.Size = new Point(20, 20);
            Button.Margin = new Margin(6);
            Button.Dock = DockStyle.Left;
            Button.MouseClick += Button_MouseClick;
            Elements.Add(Button);

            Label = new Button();
            Label.Size = new Point(20, 20);
            Label.Dock = DockStyle.Fill;
            Label.MouseClick += Label_MouseClick;
            Label.NoEvents = true;
            Elements.Add(Label);

            MouseClick += Label_MouseClick;
        }

        protected override void OnStateChanged()
        {
            Label.State = State;
        }

        void Label_MouseClick(Control sender, MouseEventArgs args)
        {
            if (args.Button > 0) return;

            Selected = true;
        }

        void Button_MouseClick(Control sender, MouseEventArgs args)
        {
            if (args.Button > 0) return;

            Expanded = !Expanded;
        }
    }
}
