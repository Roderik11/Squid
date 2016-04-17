using System;
using System.Collections.Generic;
using System.Text;
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
        private Frame ItemContainer;
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
                    _selectedNode.Selected = true;

                if (SelectedNodeChanged != null)
                    SelectedNodeChanged(this, _selectedNode);
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
            ItemContainer.Parent = ClipFrame;

            MouseWheel += TreeView_MouseWheel;
        }

        void TreeView_MouseWheel(Control sender, MouseEventArgs args)
        {
            Scrollbar.Scroll(Gui.MouseScroll);
            args.Cancel = true;
        }

        protected override void OnUpdate()
        {
            // force the width to be that of its parent
            ItemContainer.Size = new Point(ClipFrame.Size.x, ItemContainer.Size.y);

            // move the label up/down using the scrollbar value
            if (ItemContainer.Size.y < ClipFrame.Size.y) // no need to scroll
            {
                Scrollbar.Visible = false; // hide scrollbar
                ItemContainer.Position = new Point(0, 0); // set fixed position
            }
            else
            {
                Scrollbar.Scale = Math.Min(1, (float)Size.y / (float)ItemContainer.Size.y);
                Scrollbar.Visible = true; // show scrollbar
                ItemContainer.Position = new Point(0, (int)((ClipFrame.Size.y - ItemContainer.Size.y) * Scrollbar.EasedValue));
            }

            if (Scrollbar.ShowAlways)
                Scrollbar.Visible = true;
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
            TreeNode node = sender as TreeNode;
            if (node == null) return;

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

        void item_OnSelect(object sender, EventArgs e)
        {
            TreeNode node = sender as TreeNode;

            if (SelectedNode != null) SelectedNode.Selected = false;
            SelectedNode = node;
            if (SelectedNode != null) SelectedNode.Selected = true;

            if (SelectedNodeChanged != null)
                SelectedNodeChanged(this, SelectedNode);
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
        internal TreeView treeview;
        private bool _selected;
        private bool _expanded;
        private bool _suspendEvents;

        /// <summary>
        /// Raised when [on selected changed].
        /// </summary>
        public event VoidEvent SelectedChanged;

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
                if (SelectedChanged != null)
                    SelectedChanged(this);
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
                _expanded = value;

                if (!_suspendEvents)
                {
                    if (ExpandedChanged != null)
                        ExpandedChanged(this);
                }
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
        public int NodeDepth { get; internal set; }

        /// <summary>
        /// Gets or sets the nodes.
        /// </summary>
        /// <value>The nodes.</value>
        public TreeNodeCollection Nodes { get; set; }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public TreeNode Parent { get; private set; }

        public TreeNode()
        {
            Nodes = new TreeNodeCollection();
            Nodes.ItemAdded += Nodes_ItemAdded;
            Nodes.ItemRemoved += Nodes_ItemRemoved;
            Nodes.BeforeItemsCleared += Nodes_BeforeItemsCleared;

            Size = new Point(100, 20);
            Dock = DockStyle.Top;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (treeview != null && treeview.Indent != 0)
            {
                Margin m = Margin;
                Margin = new Margin(treeview.Indent * NodeDepth, m.Top, m.Right, m.Bottom);
            }
        }

        void Nodes_BeforeItemsCleared(object sender, EventArgs e)
        {
            foreach (TreeNode node in Nodes)
            {
                node.Parent = null;

                if (treeview != null)
                    treeview.RemoveNode(node);
            }
        }

        void Nodes_ItemRemoved(object sender, ListEventArgs<TreeNode> e)
        {
            if (treeview != null)
                treeview.RemoveNode(e.Item);

            e.Item.Parent = null;
        }

        void Nodes_ItemAdded(object sender, ListEventArgs<TreeNode> e)
        {
            e.Item.NodeDepth = NodeDepth + 1;
            e.Item.Parent = this;

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
            if (Parent != null)
                Parent.Nodes.Remove(this);
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

        //protected override void OnStateChanged()
        //{
        //    Label.State = State;
        //}

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
