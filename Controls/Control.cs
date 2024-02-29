﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.ComponentModel;
using Squid.Xml;
using System.Text.RegularExpressions;
using System.Runtime.DesignerServices;
using System.Net.Mime;
using System.Runtime.Remoting.Contexts;

namespace Squid
{
    /// <summary>
    /// Delegate VoidEvent
    /// </summary>
    /// <param name="sender">The sender.</param>
    public delegate void VoidEvent(Control sender);
    /// <summary>
    /// Delegate KeyEvent
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
    public delegate void KeyEvent(Control sender, KeyEventArgs args);
    /// <summary>
    /// Delegate MouseEvent
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
    public delegate void MouseEvent(Control sender, MouseEventArgs args);
    /// <summary>
    /// Delegate DragDropEvent
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="DragDropEventArgs"/> instance containing the event data.</param>
    public delegate void DragDropEvent(Control sender, DragDropEventArgs e);
    /// <summary>
    /// Delegate EventWithArgs
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The <see cref="SquidEventArgs"/> instance containing the event data.</param>
    public delegate void EventWithArgs(Control sender, SquidEventArgs args);

    /// <summary>
    /// The base class of all Controls
    /// </summary>
    public partial class Control
    {
        /// <summary>
        /// Raised when [update].
        /// </summary>
        public event VoidEvent Update;

        /// <exclude />
        public event VoidEvent Layout;

        /// <summary>
        /// Raised when [late update].
        /// </summary>
        public event VoidEvent LateUpdate;

        /// <summary>
        /// Raised when [size changed].
        /// </summary>
        public event VoidEvent SizeChanged;

        /// <summary>
        /// Raised when [position changed].
        /// </summary>
        public event VoidEvent PositionChanged;

        /// <summary>
        /// Raised when [mouse enter].
        /// </summary>
        public event VoidEvent MouseEnter;

        /// <summary>
        /// Raised when [mouse leave].
        /// </summary>
        public event VoidEvent MouseLeave;

        /// <summary>
        /// Raised when [got focus].
        /// </summary>
        public event VoidEvent GotFocus;

        /// <summary>
        /// Raised when [lost focus].
        /// </summary>
        public event VoidEvent LostFocus;

        /// <summary>
        /// Raised when [key down].
        /// </summary>
        public event KeyEvent KeyDown;

        /// <summary>
        /// Raised when [key up].
        /// </summary>
        public event KeyEvent KeyUp;

        /// <summary>
        /// Raised when [mouse drag].
        /// </summary>
        public event MouseEvent MouseDrag;

        /// <summary>
        /// Raised when [mouse up].
        /// </summary>
        public event MouseEvent MouseUp;

        /// <summary>
        /// Raised when [mouse down].
        /// </summary>
        public event MouseEvent MouseDown;

        /// <summary>
        /// Raised when [mouse press].
        /// </summary>
        public event MouseEvent MousePress;

        /// <summary>
        /// Raised when [mouse click].
        /// </summary>
        public event MouseEvent MouseClick;

        /// <summary>
        /// Raised when [mouse double click].
        /// </summary>
        public event MouseEvent MouseDoubleClick;

        /// <summary>
        /// Raised when [mouse wheel].
        /// </summary>
        public event MouseEvent MouseWheel;

        /// <summary>
        /// Raised when [drag drop].
        /// </summary>
        public event DragDropEvent DragDrop;

        /// <summary>
        /// Raised during Drag&Drop, when this control becomes the active DropTarget.
        /// AllowDrop must be True for this event to fire.
        /// </summary>
        public event DragDropEvent DragEnter;

        /// <summary>
        /// Raised during Drag&Drop, when the active DropTarget changes from this control to a different one.
        /// AllowDrop must be True for this event to fire.
        /// </summary>
        public event DragDropEvent DragLeave;

        /// <summary>
        /// Raised during Drag&Drop, while this control is the active DropTargt.
        /// AllowDrop must be True for this event to fire.
        /// </summary>
        public event DragDropEvent DragResponse;

        /// <summary>
        /// Raised when control is dropped.
        /// </summary>
        public event DragDropEvent Drop;
    }

    public partial class Control
    {
        internal bool _isElement;
        internal Rectangle Bounds;
        internal Rectangle DockAreaC;
        internal Rectangle DockAreaE;
        internal bool IsRemoved;
        internal Control Owner;

        internal Control ParentControl
        {
            get { return _parent; }
            set
            {
                if (_parent == value) return;
                _parent = value;
                SetBounds();
            }
        }

        private bool TextureFade;
        private bool FontFade;

        private float FadeIn;
        private float FadeOut = 1;
        private bool _isMouseDrag;
        private bool _enabled = true;
        private bool _parentEnabled = true;
        private DateTime TimeClicked;
        private bool IsDoubleClick;
        private Margin _padding;
        private Margin _margin;
        private Point _size;
        private Point _position;
        private Control _parent;
        private ControlState _state;
        private ControlState _oldState;
        private AnchorStyles _anchor = AnchorStyles.Top | AnchorStyles.Left;
        private Point _floatingPosition;
        private Point _floatingSize;
        private DockStyle _dock;

        private static readonly Stack<Rectangle> ScissorStack = new Stack<Rectangle>();
        private static Rectangle currentScissorRect;

        /// <summary>
        /// The area
        /// </summary>
        protected Rectangle ClipRect;

        public Rectangle Rectangle => ClipRect;

        /// <summary>
        /// The elements
        /// </summary>
        protected ElementCollection Elements;

        /// <summary>
        /// Returns all child elements
        /// </summary>
        /// <returns></returns>
        public ElementCollection GetElements()
        {
            return Elements;
        }

        private bool _useTranslation = false;

        /// <summary>
        /// Gets or sets a value indicating whether [use translation].
        /// </summary>
        /// <value><c>true</c> if [use translation]; otherwise, <c>false</c>.</value>
        public bool UseTranslation
        {
            get { return _useTranslation; }
            set
            {
                if (value == _useTranslation) return;
                bool from = _useTranslation;
                bool to = value;
                TranslationChanged(from, to);
                _useTranslation = value;
            }
        }

        internal void UpdateTranslation()
        {
            if (UseTranslation)
            {
                TranslationChanged(true, false);
                TranslationChanged(false, true);
            }

            for (int i = 0; i < Elements.Count; i++)
                Elements[i].UpdateTranslation();

            if (IsContainer)
            {
                ControlCollection controls = LocalContainer.Controls;

                for (int i = 0; i < controls.Count; i++)
                    controls[i].UpdateTranslation();
            }
        }

        protected virtual void TranslationChanged(bool from, bool to)
        {
            if (string.IsNullOrEmpty(_originalTooltip)) return;

            if (from == false && to == true)
                _tooltip = TranslateText(_originalTooltip);
            else if (from == true && to == false)
                _tooltip = _originalTooltip;
        }

        protected string TranslateText(string text)
        {
            string szPattern = @"(\{tk:(.*?)\})+";
            List<string> tokens = new List<string>();

            foreach (Match match in Regex.Matches(text, szPattern))
                tokens.Add(match.Value);

            string key;
            string translated;

            string result = text;

            foreach (string token in tokens)
            {
                key = token.Substring(4, token.Length - 5);
                translated = Gui.Translate(key);
                result = result.Replace(token, translated);
            }

            if (tokens.Count == 0)
                result = Gui.Translate(text);

            return result;
        }

        /// <summary>
        /// Returns true if the control is a child element
        /// </summary>
        public bool IsElement { get { return _isElement; } }

        /// <summary>
        /// Gets/Sets the color used to Tint the used Style
        /// </summary>
        [IntColor]
        [Category("Design")]
        [DefaultValue(-1)]
        public int Tint;// { get; set; }

        /// <summary>
        /// Name of the control
        /// </summary>
        [DefaultValue("")]
        [Category("Base")]
        public string Name;// { get; set; }

        /// <summary>
        /// Opacity of the control
        /// This is multiplied with any style opacity and hierarchical opacity
        /// </summary>
        [ValueRange(0, 1)]
        [DefaultValue(1.0f)]
        [Category("Design")]
        public float Opacity;// { get; set; }

        /// <summary>
        /// Gets/Sets the autosize behavior
        /// </summary>
        [DefaultValue(AutoSize.None)]
        [Category("Layout")]
        public AutoSize AutoSize;// { get; set; }

        /// <summary>
        /// Gets/Sets whether or not the control processes DragDrop events
        /// </summary>
        [DefaultValue(false)]
        [Category("Behavior")]
        public bool AllowDrop;// { get; set; }

        /// <summary>
        /// Gets/Sets whether or not the control is able to acquire focus
        /// </summary>
        [DefaultValue(false)]
        [Category("Behavior")]
        public bool AllowFocus;//  { get; set; }

        /// <summary>
        /// Gets/Sets whether or not the control prevents the focus to change
        /// </summary>
        [DefaultValue(false)]
        [Category("Behavior")]
        public bool PreventFocusChange;//  { get; set; }

        /// <summary>
        /// Gets/Sets whether or not hardware scissor test is used
        /// </summary>
        [DefaultValue(false)]
        [Category("Design")]
        public bool Scissor;//  { get; set; }

        /// <summary>
        /// Gets/Sets the docking behavior
        /// </summary>
        [DefaultValue(DockStyle.None)]
        [Category("Layout")]
        public DockStyle Dock
        {
            get { return _dock; }
            set
            {
                if (value == DockStyle.None)
                {
                    _size = _floatingSize;
                    _position = _floatingPosition;
                }
                else if (_dock == DockStyle.None)
                {
                    _floatingSize = _size;
                    _floatingPosition = _position;
                }

                _dock = value;
            }
        }

        private static readonly ControlStyle DefaultStyle = new ControlStyle();

        protected ControlStyle LocalStyle = DefaultStyle;
        private string _style;

        /// <summary>
        /// Name of the ControlStyle 
        /// </summary>
        [Style]
        [DefaultValue("")]
        [Category("Design")]
        public string Style
        {
            get => _style;
            set
            {
                if (_style == value) return;
                _style = value;

                LocalStyle = DefaultStyle;

                if (Desktop != null)
                    LocalStyle = Desktop.GetStyle(_style);
            }
        }

        /// <summary>
        /// Gets/Sets whether or not the control is interactive and processes any events
        /// </summary>
        [Category("Behavior")]
        public bool NoEvents;// { get; set; }

        /// <summary>
        /// user defined data object
        /// </summary>
        [XmlIgnore, Hidden]
        public object Tag;// { get; set; }

        /// <summary>
        /// user defined data object
        /// </summary>
        [XmlIgnore, Hidden]
        public object UserData;// { get; set; }

        /// <summary>
        /// internal data object
        /// </summary>
        [XmlIgnore, Hidden]
        internal object InternalTag;// { get; set; }

        /// <summary>
        /// tab index
        /// </summary>
        [DefaultValue(0)]
        [Category("Behavior")]
        public int TabIndex;// { get; set; }

        /// <summary>
        /// Gets/Sets whether or not the control is visible
        /// </summary>
        [DefaultValue(true)]
        [Category("Base")]
        public bool Visible = true;// { get { return _visible; } set { _visible = value; } }

        /// <summary>
        /// Returns the parent of the control as IControlContainer
        /// </summary>
        public IControlContainer Container { get { return _parent as IControlContainer; } }

        /// <summary>
        /// Name of the cursor to use 
        /// </summary>
        [Cursor]
        [DefaultValue("")]
        [Category("Behavior")]
        public string Cursor;// { get; set; }

        private string _tooltip;
        private string _originalTooltip;

        /// <summary>
        /// Tooltip text 
        /// </summary>
        [DefaultValue("")]
        [Multiline]
        [Category("Base")]
        public string Tooltip
        {
            get { return _tooltip; }
            set
            {
                _originalTooltip = value;

                if (UseTranslation)
                    _tooltip = TranslateText(_originalTooltip);
                else
                    _tooltip = value;
            }
        }

        [DefaultValue(Alignment.TopCenter)]
        public Alignment TooltipAlign;

        /// <summary>
        /// Returns the z-index
        /// </summary>
        public int ZIndex
        {
            get
            {
                if (Container != null)
                    return Container.Controls.Count - (Container.Controls.IndexOf(this) + 1);
                else return 0;
            }
        }

        /// <summary>
        /// Gets/Sets whether or not the control, and all its children, is enabled
        /// </summary>
        [DefaultValue(true)]
        [Category("Base")]
        public bool Enabled
        {
            get
            {
                if (!_parentEnabled)
                    return false;

                return _enabled;
            }
            set
            {
                if (_enabled == value)
                    return;

                _enabled = value;

                if (IsContainer)
                {
                    foreach (Control control in LocalContainer.Controls)
                        control.SetEnabled(value);
                }

                foreach (Control control in Elements)
                    control.SetEnabled(value);
            }
        }

        /// <summary>
        /// Gets/Sets the parent
        /// </summary>
        [XmlIgnore, Hidden]
        public Control Parent
        {
            get { return _parent; }
            set
            {
                if (value == this) return;
                if (value == _parent) return;

                if (value != null)
                {
                    if (value.IsChildOf(this))
                        return;
                }

                Container?.Controls.Remove(this);

                if (value is IControlContainer container)
                    container.Controls.Add(this);
            }
        }

        /// <summary>
        /// Gets/Sets the minimum size.
        /// This is only used during Control.Resize
        /// </summary>
        [DefaultValue(typeof(Point), "0; 0")]
        [Category("Layout")]
        public Point MinSize;

        /// <summary>
        /// Gets/Sets the maximum size.
        /// This is only used during Control.Resize
        /// </summary>
        [DefaultValue(typeof(Point), "0; 0")]
        [Category("Layout")]
        public Point MaxSize;

        /// <summary>
        /// Gets/Sets the size
        /// </summary>
        [DefaultValue(typeof(Point), "0; 0")]
        [Category("Layout")]
        public Point Size
        {
            get { return _size; }
            set
            {
                if (_size.x == value.x && _size.y == value.y) return;
                _size = value;

                SizeChanged?.Invoke(this);

                SetBounds();
            }
        }

        /// <summary>
        /// Gets/Sets the position (relative to parent)
        /// </summary>
        [DefaultValue(typeof(Point), "0; 0")]
        [Category("Layout")]
        public Point Position
        {
            get { return _position; }
            set
            {
                if (_position.x == value.x && _position.y == value.y) return;
                _position = value;
                newlocation = true;

                PositionChanged?.Invoke(this);

                SetBounds();
            }
        }

        /// <summary>
        /// Gets/Sets the anchoring behavior
        /// </summary>
        [DefaultValue(AnchorStyles.Top | AnchorStyles.Left)]
        [Category("Layout")]
        public AnchorStyles Anchor
        {
            get { return _anchor; }
            set
            {
                if (_anchor == value) return;
                _anchor = value;
                SetBounds();
            }
        }

        /// <summary>
        /// Defines the space around a control that keeps other controls at a specified distance from the control's borders.
        /// </summary>
        [DefaultValue(typeof(Margin), "0; 0; 0; 0")]
        [Category("Layout")]
        public Margin Margin
        {
            get { return _margin; }
            set { _margin = value; }
        }

        /// <summary>
        /// Defines the space inside of a control that keeps child controls at a specified distance from the control's borders.
        /// </summary>
        [DefaultValue(typeof(Margin), "0; 0; 0; 0")]
        [Category("Layout")]
        public Margin Padding
        {
            get { return _padding; }
            set { _padding = value; }
        }

        private ControlState externalState = ControlState.Default;
       
        /// <summary>
        /// Gets/Sets the state
        /// </summary>
        [XmlIgnore, Hidden]
        public ControlState State
        {
            get { return _state; }
            set
            {
                if (_state == value) return;
                if (!selfStateChange) externalState = value;

                _oldState = _state;
                _state = value;

                Style last = LocalStyle.Styles[_oldState];
                Style next = LocalStyle.Styles[_state];

                TextureFade = last.IsTextureDifferent(next);
                FontFade = last.IsFontDifferent(next);

                OnStateChanged();
                if (selfStateChange)
                    StateChanged?.Invoke();

                (FadeOut, FadeIn) = (FadeIn, FadeOut);
            }
        }

        /// <summary>
        /// Gets/Sets the local fading speed
        /// </summary>
        [DefaultValue(0.0f)]
        [Category("Design")]
        public float FadeSpeed;// { get; set; }

        private bool newlocation = false;
        private Point _location;

        /// <summary>
        /// Returns the screen position
        /// </summary>
        public Point Location
        {
            get
            {
                if (!newlocation)
                    return _location;

                newlocation = false;
                _location = _position * GetScale();

                if (_parent != null)
                    _location = _parent.Location + _position * GetScale();

                return _location;
            }
        }

        private Desktop _desktop;
       
        /// <summary>
        /// Returns the root control
        /// </summary>
        public Desktop Desktop => _desktop ?? this as Desktop;

        //public ControlAnimation Animation;
        public GuiActionList Actions;

        ///// <summary>
        ///// Name of a property or field to bind to this control
        ///// </summary>
        //public string Aspect { get; set; }

        /// <summary>
        /// Gets the unique auto id.
        /// </summary>
        /// <value>The auto id.</value>
        [XmlIgnore]
        public int AutoId { get; private set; }

        private static int AutoIncrement;
        public bool IsContainer { get; private set; }
        private readonly IControlContainer LocalContainer;
        private float FinalOpacity;

        /// <summary>
        /// Control ctor
        /// </summary>
        public Control()
        {
            Name = "";
            Tooltip = "";
            Cursor = "";

            TooltipAlign = Alignment.TopCenter;
            Size = new Point(100, 30);
            AutoId = AutoIncrement++;

            Tint = -1;
            Opacity = 1;
            Anchor = AnchorStyles.Top | AnchorStyles.Left;
            Elements = new ElementCollection(this);
            Actions = new GuiActionList(this);
            //Animation = new ControlAnimation(this);

            LocalContainer = this as IControlContainer;

            if (LocalContainer != null)
            {
                IsContainer = true;
                LocalContainer.Controls = new ControlCollection(this);
                LocalContainer.Controls.ItemAdded += Controls_ItemAdded;
            }

            Elements.ItemAdded += Elements_ItemAdded;
        }

        private void Elements_ItemAdded(object sender, ListEventArgs<Control> e)
        {
            e.Item.SetDesktop(Desktop);
        }

        private void Controls_ItemAdded(object sender, ListEventArgs<Control> e)
        {
            e.Item.SetDesktop(Desktop);
        }

        void SetDesktop(Desktop desktop)
        {
            _desktop = desktop;

            foreach (var child in Elements)
                child.SetDesktop(desktop);

            if(IsContainer)
            {
                foreach (var child in LocalContainer.Controls)
                    child.SetDesktop(desktop);
            }

            LocalStyle = DefaultStyle;

            if (Desktop != null)
                LocalStyle = Desktop.GetStyle(_style);
        }

        //protected virtual void Bind(object value)
        //{
        //    if (IsContainer)
        //    {
        //        foreach (Control child in ((IControlContainer)this).Controls)
        //            child.Bind(value);
        //    }
        //}

        /// <summary>
        /// Makes the control the focused control
        /// </summary>
        public void Focus()
        {
            if (!AllowFocus) return;
            if (Desktop == null) return;

            Desktop.FocusedControl = this;
        }

        /// <summary>
        /// Return true if the point collides with the control
        /// </summary>
        /// <param name="x">x screen coord</param>
        /// <param name="y">y screen coord</param>
        /// <returns></returns>
        public bool Hit(int x, int y)
        {
            return x >= ClipRect.Left && x <= ClipRect.Right && y >= ClipRect.Top && y <= ClipRect.Bottom;
        }

        /// <summary>
        /// Returns the first control found below the given screen point
        /// Returns elements and controls
        /// </summary>
        /// <param name="x">x screen coord</param>
        /// <param name="y">y screen coord</param>
        /// <returns></returns>
        public Control GetControlAt(int x, int y)
        {
            return GetControlAt(x, y, true);
        }

        /// <summary>
        /// Returns the first control matching the given name.
        /// This method searches recursively
        /// </summary>
        /// <param name="name">Name of the control to find</param>
        /// <returns>Matching control</returns>
        public Control GetControl(string name)
        {
            if (Name == name) return this;

            Control result = null;

            if (IsContainer)
            {
                foreach (Control child in LocalContainer.Controls)
                {
                    result = child.GetControl(name);
                    if (result != null)
                        return result;
                }
            }

            foreach (Control child in Elements)
            {
                result = child.GetControl(name);
                if (result != null)
                    return result;
            }

            return result;
        }

        /// <summary>
        /// Returns direct children of the given type
        /// </summary>
        /// <returns>Matching controls</returns>
        public List<T> GetControls<T>() where T : Control
        {
            List<T> result = new List<T>();

            if (IsContainer)
            {
                foreach (Control child in LocalContainer.Controls)
                {
                    if (child is T)
                        result.Add(child as T);
                }
            }

            foreach (Control child in Elements)
            {
                if (child is T)
                    result.Add(child as T);
            }

            return result;
        }

        /// <summary>
        /// Returns the first control found below the given screen point
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
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
        /// <param name="data">The control to be displayed as dragged</param>
        public void DoDragDrop(Control data)
        {
            if (Desktop == null) return;

            Desktop.DoDragDrop(this, data);
        }

        /// <summary>
        /// Returns true if the given control is a child  of the control
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public bool IsChildOf(Control control)
        {
            if (control.Elements.Contains(this)) return true;

            foreach (Control child in control.Elements)
            {
                if (IsChildOf(child))
                    return true;
            }

            if (!control.IsContainer) return false;
            if (control.LocalContainer.Controls.Contains(this)) return true;

            foreach (Control child in control.LocalContainer.Controls)
            {
                if (IsChildOf(child))
                    return true;
            }

            return false;
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
        /// <param name="control"></param>
        /// <returns></returns>
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
        /// Sends the control to the front of its container
        /// </summary>
        public void BringToFront()
        {
            if (_parent == null) return;
            if(_isElement)
            {
                var tempparent = _parent;
                tempparent.Elements.Remove(this);
                tempparent.Elements.Add(this);
            }
            else
            {
                if (Container == null) return;
                int index = Container.Controls.IndexOf(this);
                if (index == Container.Controls.Count - 1) return;

                Control parent = _parent;
                Parent = null;
                Parent = parent;
            }
        }

        /// <summary>
        /// Sends the control to the back of its container
        /// </summary>
        public void BringToBack()
        {
            if (_parent == null) return;

            if (_isElement)
            {
                var tempparent = _parent;
                tempparent.Elements.Remove(this);
                tempparent.Elements.Insert(0, this);
            }
            else
            {
                if (Container == null) return;
                int index = Container.Controls.IndexOf(this);
                if (index == 0) return;

                IControlContainer container = Container;
                Parent = null;
                container.Controls.Insert(0, this);
            }
        }

        /// <summary>
        /// Resizes to.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="anchor">The anchor.</param>
        /// <returns>Point.</returns>
        public Point ResizeTo(Point size, AnchorStyles anchor)
        {
            Point oldSize = _size;
            Point p = size - oldSize;

            switch (anchor)
            {
                case AnchorStyles.Bottom:
                    ResizeBottom(p.y);
                    break;
                case AnchorStyles.Right:
                    ResizeRight(p.x);
                    break;
                case AnchorStyles.Top:
                    ResizeTop(-p.y);
                    break;
                case AnchorStyles.Left:
                    ResizeLeft(-p.x);
                    break;
                case AnchorStyles.Bottom | AnchorStyles.Right:
                    ResizeRight(p.x);
                    ResizeBottom(p.y);
                    break;
                case AnchorStyles.Bottom | AnchorStyles.Left:
                    ResizeLeft(-p.x);
                    ResizeBottom(p.y);
                    break;
                case AnchorStyles.Top | AnchorStyles.Right:
                    ResizeRight(p.x);
                    ResizeTop(-p.y);
                    break;
                case AnchorStyles.Top | AnchorStyles.Left:
                    ResizeLeft(-p.x);
                    ResizeTop(-p.y);
                    break;
                case AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom:
                    ResizeLeft(-p.x / 2);
                    ResizeTop(-p.y / 2);
                    ResizeRight(p.x / 2);
                    ResizeBottom(p.y / 2);
                    break;
            }

            SetBounds();

            if (!Actions.IsUpdating)
            {
                if (Dock != DockStyle.None)
                {
                    _parent?.PerformLayout();
                }
                else
                    PerformLayout();
            }

            return _size - oldSize;
        }

        /// <summary>
        /// Processes the events.
        /// </summary>
        public void ProcessEvents() { DoEvents(); }

        private int localLanguage;

        internal void PerformUpdate()
        {
            if (!Visible) return;

            if (FadeSpeed > 0 || Gui.GlobalFadeSpeed > 0)
            {
                float speed = FadeSpeed > 0 ? FadeSpeed : Gui.GlobalFadeSpeed;
                float delta = Gui.TimeElapsed / speed;

                FadeOut -= delta; FadeIn += delta;

                FadeIn = FadeIn < 0 ? 0 : (FadeIn > 1 ? 1 : FadeIn);
                FadeOut = FadeOut < 0 ? 0 : (FadeOut > 1 ? 1 : FadeOut);
            }

            FinalOpacity = GetOpacity(LocalStyle.Styles[_state].Opacity);

            int elementCount = Elements.Count;
            int controlCount = 0;

            if (IsContainer)
            {
                controlCount = LocalContainer.Controls.Count;
                LocalContainer.Controls.IsLocked = true;
            }

            Elements.IsLocked = true;

            Actions.Update(Gui.TimeElapsed);

            if (!IsRemoved)
            {
                if (localLanguage != Gui.Language)
                {
                    UpdateTranslation();
                    localLanguage = Gui.Language;
                }

                OnUpdate();

                for (int i = 0; i < elementCount; i++)
                    Elements[i].PerformUpdate();

                for (int i = 0; i < controlCount; i++)
                    LocalContainer.Controls[i].PerformUpdate();
            }

            Elements.IsLocked = false;
            Elements.Cleanup();

            if (IsContainer)
            {
                LocalContainer.Controls.IsLocked = false;
                LocalContainer.Controls.Cleanup();
            }

            if (!IsRemoved)
                DetermineState();
        }

        public Point GetContentSize()
        {
            Point auto = new Point(Padding.Left + Padding.Right, Padding.Top + Padding.Bottom);
            int count = Elements.Count;
            Point childSize;
            Margin childMargin;

            Control child;
            for (int i = 0; i < count; i++)
            {
                child = Elements[i];

                if (AutoSize != AutoSize.None && !child.IsRemoved && child.Visible)
                {
                    childSize = child._size;
                    childMargin = child._margin;

                    auto.x += childSize.x + childMargin.Right + child.Margin.Left;
                    auto.y += childSize.y + childMargin.Bottom + child.Margin.Top;
                }
            }

            if (IsContainer)
            {
                auto = new Point(Padding.Left + Padding.Right, Padding.Top + Padding.Bottom);
                ControlCollection controls = LocalContainer.Controls;

                count = controls.Count;

                for (int i = 0; i < count; i++)
                {
                    child = controls[i];

                    if (AutoSize != AutoSize.None && !child.IsRemoved && child.Visible)
                    {
                        childSize = child._size;
                        childMargin = child._margin;

                        auto.x += childSize.x + childMargin.Right + child.Margin.Left;
                        auto.y += childSize.y + childMargin.Bottom + child.Margin.Top;
                    }
                }
            }

            return auto;
        }

        /// <summary>
        /// Performs the layout.
        /// </summary>
        public void PerformLayout()
        {
            if (!Visible) return;

            Point pOld = _position;
            Point sOld = _size;

            newlocation = true;

            PerformLayoutAndClip();
            OnLayout();

            Point auto = Point.Zero;
            Control child = null;
            int count = Elements.Count;
            Point childSize, childPosition;
            Margin childMargin;

            for (int i = 0; i < count; i++)
            {
                child = Elements[i];
                child.PerformLayout();

                if (AutoSize == AutoSize.None) continue;
                if (!child.Visible) continue;
                if (child.IsRemoved) continue;

                childPosition = child._position;
                childSize = child._size;
                childMargin = child._margin;

                auto.x = Math.Max(auto.x, childPosition.x + childSize.x + childMargin.Right);
                auto.y = Math.Max(auto.y, childPosition.y + childSize.y + childMargin.Bottom);
            }

            if (IsContainer)
            {
                auto = Point.Zero;
                ControlCollection controls = LocalContainer.Controls;

                child = null;
                count = controls.Count;

                for (int i = 0; i < count; i++)
                {
                    child = controls[i];
                    child.PerformLayout();

                    if (AutoSize == AutoSize.None) continue;
                    if (!child.Visible) continue;
                    if (child.IsRemoved) continue;

                    childPosition = child._position;
                    childSize = child._size;
                    childMargin = child._margin;

                    auto.x = Math.Max(auto.x, childPosition.x + childSize.x + childMargin.Right);
                    auto.y = Math.Max(auto.y, childPosition.y + childSize.y + childMargin.Bottom);

                    auto.x += _padding.Left + _padding.Right;
                    auto.y += _padding.Top + _padding.Bottom;
                }
            }

            if (AutoSize != AutoSize.None)
            {
                if (MinSize.x > 0 && auto.x < MinSize.x) auto.x = MinSize.x;
                if (MinSize.y > 0 && auto.y < MinSize.y) auto.y = MinSize.y;

                if (AutoSize == AutoSize.Vertical)
                {
                    _size.y = auto.y;
                }
                else if (AutoSize == AutoSize.Horizontal)
                {
                    _size.x = auto.x;
                }
                else
                {
                    _size.x = auto.x;
                    _size.y = auto.y;
                }

                OnAutoSize();
            }

            if (_size.x != sOld.x || _size.y != sOld.y)
                SizeChanged?.Invoke(this);


            if (_position.x != pOld.x || _position.y != pOld.y)
                PositionChanged?.Invoke(this);
        }

        /// <summary>
        /// Gets the opacity.
        /// </summary>
        /// <param name="opacity">The opacity.</param>
        /// <returns>System.Single.</returns>
        protected float GetOpacity(float opacity)
        {
            if (_parent != null)
                return _parent.FinalOpacity * opacity * Opacity;

            return opacity * Opacity;
        }

        /// <summary>
        /// Aligns the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="align">The align.</param>
        /// <param name="padding">The padding.</param>
        /// <param name="font">The font.</param>
        /// <returns>Point.</returns>
        protected Point AlignText(string text, Alignment align, Margin padding, int font, float scale = 1)
        {
            Point tsize = Gui.Renderer.GetTextSize(text, font);
            padding.Scale(scale);

            switch (align)
            {
                case Alignment.BottomCenter:
                    return Location + new Point((_size.x - tsize.x) / 2, _size.y - tsize.y - padding.Bottom);
                case Alignment.BottomLeft:
                    return Location + new Point(padding.Left, _size.y - tsize.y - padding.Bottom);
                case Alignment.BottomRight:
                    return Location + new Point(_size.x - tsize.x - padding.Right, _size.y - tsize.y - padding.Bottom);
                case Alignment.MiddleCenter:
                    return Location + new Point((_size.x - tsize.x) / 2, (_size.y - tsize.y) / 2);
                case Alignment.MiddleLeft:
                    return Location + new Point(padding.Left, (int)Math.Floor((float)(_size.y - tsize.y) / 2));
                case Alignment.MiddleRight:
                    return Location + new Point(_size.x - tsize.x - padding.Right, (_size.y - tsize.y) / 2);
                case Alignment.TopCenter:
                    return Location + new Point((_size.x - tsize.x) / 2, padding.Top);
                case Alignment.TopLeft:
                    return Location + new Point(padding.Left, padding.Top);
                case Alignment.TopRight:
                    return Location + new Point(_size.x - tsize.x - padding.Right, padding.Top);
                default:
                    return Location;
            }
        }

        protected Point AlignText(string text, Alignment align, Point size, Point location, Margin padding, int font, float scale = 1)
        {
            Point tsize = Gui.Renderer.GetTextSize(text, font);
            padding.Scale(scale);

            switch (align)
            {
                case Alignment.BottomCenter:
                    return location + new Point((size.x - tsize.x) / 2, size.y - tsize.y - padding.Bottom);
                case Alignment.BottomLeft:
                    return location + new Point(padding.Left, size.y - tsize.y - padding.Bottom);
                case Alignment.BottomRight:
                    return location + new Point(size.x - tsize.x - padding.Right, size.y - tsize.y - padding.Bottom);
                case Alignment.MiddleCenter:
                    return location + new Point((size.x - tsize.x) / 2, (size.y - tsize.y) / 2);
                case Alignment.MiddleLeft:
                    return location + new Point(padding.Left, (int)Math.Floor((float)(size.y - tsize.y) / 2));
                case Alignment.MiddleRight:
                    return location + new Point(size.x - tsize.x - padding.Right, (size.y - tsize.y) / 2);
                case Alignment.TopCenter:
                    return location + new Point((size.x - tsize.x) / 2, padding.Top);
                case Alignment.TopLeft:
                    return location + new Point(padding.Left, padding.Top);
                case Alignment.TopRight:
                    return location + new Point(size.x - tsize.x - padding.Right, padding.Top);
                default:
                    return location;
            }
        }

        /// <summary>
        /// Aligns the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="align">The align.</param>
        /// <param name="padding">The padding.</param>
        /// <param name="font">The font.</param>
        /// <param name="tsize">The tsize.</param>
        /// <returns>Point.</returns>
        protected Point AlignText(string text, Alignment align, Margin padding, int font, out Point tsize)
        {
            tsize = Gui.Renderer.GetTextSize(text, font);

            switch (align)
            {
                case Alignment.BottomCenter:
                    return Location + new Point((_size.x - tsize.x) / 2, _size.y - tsize.y - padding.Bottom);
                case Alignment.BottomLeft:
                    return Location + new Point(padding.Left, _size.y - tsize.y - padding.Bottom);
                case Alignment.BottomRight:
                    return Location + new Point(_size.x - tsize.x - padding.Right, _size.y - tsize.y - padding.Bottom);
                case Alignment.MiddleCenter:
                    return Location + new Point((_size.x - tsize.x) / 2, (_size.y - tsize.y) / 2);
                case Alignment.MiddleLeft:
                    return Location + new Point(padding.Left, (_size.y - tsize.y) / 2);
                case Alignment.MiddleRight:
                    return Location + new Point(_size.x - tsize.x - padding.Right, (_size.y - tsize.y) / 2);
                case Alignment.TopCenter:
                    return Location + new Point((_size.x - tsize.x) / 2, padding.Top);
                case Alignment.TopLeft:
                    return Location + new Point(padding.Left, padding.Top);
                case Alignment.TopRight:
                    return Location + new Point(_size.x - tsize.x - padding.Right, padding.Top);
                default:
                    return Location;
            }
        }

        /// <summary>
        /// Called every frame.
        /// Override this to do per-frame operations
        /// </summary>
        protected virtual void OnUpdate()
        {
            Update?.Invoke(this);
        }

        /// <summary>
        /// Called every frame.
        /// Override this to do per-frame operations
        /// </summary>
        protected virtual void OnLayout()
        {
            Layout?.Invoke(this);
        }

        /// <summary>
        /// Called late every frame.
        /// Override this in special circumstances to do per-frame operations
        /// </summary>
        protected virtual void OnLateUpdate()
        {
            LateUpdate?.Invoke(this);
        }

        /// <summary>
        /// Override this method to handle any keyevents passed to the control
        /// </summary>
        protected virtual void OnKeyDown(KeyEventArgs args) { }

        /// <summary>
        /// Override this method to handle any keyevents passed to the control
        /// </summary>
        protected virtual void OnKeyUp(KeyEventArgs args) { }

        /// <summary>
        /// Override this to implement your own AutoSize behavior
        /// </summary>
        protected virtual void OnAutoSize() { }

        /// <summary>
        /// Override this to draw text.
        /// </summary>
        /// <param name="style">The style to be used</param>
        /// <param name="opacity">The opacity to be used</param>
        protected virtual void DrawText(Style style, float opacity) { }

        /// <summary>
        /// Override this to do additional drawing before the control has been drawn
        /// </summary>
        protected virtual void DrawBefore() { }

        protected virtual void DrawBeforeChildren() { }

        /// <summary>
        /// Override this to do additional drawing after the control has been drawn
        /// </summary>
        protected virtual void DrawCustom() { }

        /// <summary>
        /// Override this to handle state changes.
        /// This is useful to sync states between controls.
        /// </summary>
        protected virtual void OnStateChanged() { }

        public event Action StateChanged;

        public float UIScale = 1;
             
        public float GetScale()
        {
            if (_parent == null)
                return UIScale;

            return _parent.UIScale * _parent.GetScale();
        }

        /// <summary>
        /// Override this to do additional drawing
        /// Call the base method to draw the control as usual
        /// </summary>
        /// <param name="style"></param>
        /// <param name="opacity"></param>
        protected virtual void DrawStyle(Style style, float opacity)
        {
            if (opacity == 0) return;

            int blend = Tint != -1 ? ColorInt.Blend(Tint, style.BackColor) : style.BackColor;

            var scale = GetScale();

            Point loc = Location;
            Point size = _size;

            //loc.Scale(scale);
            size.Scale(scale);

            if (blend != 0)
                Gui.Renderer.DrawBox(loc.x, loc.y, size.x, size.y, ColorInt.FromArgb(opacity, blend));

            if (string.IsNullOrEmpty(style.Texture)) return;
            int texture = Gui.Renderer.GetTexture(style.Texture);
            if (texture < 0) return;

            if (style.TextureRect.IsEmpty)
            {
                Point texsize = Gui.Renderer.GetTextureSize(texture);
                style.TextureRect = new Rectangle(Point.Zero, texsize);
            }

            blend = Tint != -1 ? ColorInt.Blend(Tint, style.Tint) : style.Tint;
            int color = ColorInt.FromArgb(opacity, blend);

            switch (style.Tiling)
            {
                case TextureMode.Grid:
                case TextureMode.GridRepeat:
                    SliceTexture(texture, style.Tiling, style.TextureRect, style.Grid, color);
                    break;
                case TextureMode.Stretch:
                    Gui.Renderer.DrawTexture(texture, loc.x, loc.y, size.x, size.y, style.TextureRect, color);
                    break;
                case TextureMode.Center:
                    Point rectsize = new Point(style.TextureRect.Width, style.TextureRect.Height) * scale;
                    Point pos = loc + size / 2 - rectsize / 2;
                    Gui.Renderer.DrawTexture(texture, pos.x, pos.y, rectsize.x, rectsize.y, style.TextureRect, color);
                    break;
                default:
                    RepeatTexture(texture, loc, style.TextureRect, style.Tiling, color);
                    break;

            }
        }

        /// <summary>
        /// Sets the scissor rectangle
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        protected void SetScissor(int x, int y, int width, int height)
        {
            if (ScissorStack.Count == 0)
                currentScissorRect = Desktop.ClipRect;

            ScissorStack.Push(currentScissorRect);

            currentScissorRect.From(x, y, width, height);

            Gui.Renderer.EndBatch(false);
            Gui.Renderer.Scissor(x, y, width, height);
            Gui.Renderer.StartBatch();
        }

        /// <summary>
        /// Resets the scissor test to whatever is was before the last SetScissor call
        /// </summary>
        protected void ResetScissor()
        {
            Rectangle r = Desktop.ClipRect;

            if (ScissorStack.Count > 0)
                r = ScissorStack.Pop();

            currentScissorRect = r;

            Gui.Renderer.EndBatch(false);
            Gui.Renderer.Scissor(r.Left, r.Top, r.Width, r.Height);
            Gui.Renderer.StartBatch();
        }

        /// <summary>
        /// Clips the specified rect.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <returns>Rectangle.</returns>
        protected Rectangle Clip(Rectangle rect)
        {
            if (_parent != null)
                return _parent.ClipRect.Clip(rect);

            return rect;
        }


        private void ResizeLeft(int value)
        {
            Point p = _position;
            Point s = _size;

            s.x -= value;

            if (MinSize.x >= 0 && MaxSize.x > 0)
                s.x = Math.Min(s.x, MaxSize.x);

            s.x = Math.Max(s.x, MinSize.x);

            p.x += (_size.x - s.x);

            if (p.x < 0)
            {
                s.x += p.x;
                p.x = 0;
            }

            _position = p;
            _size = s;
        }

        private void ResizeRight(int value)
        {
            Point s = _size;

            s.x += value;

            if (MinSize.x >= 0 && MaxSize.x > 0)
                s.x = Math.Min(s.x, MaxSize.x);

            s.x = Math.Max(s.x, MinSize.x);

            if (_parent != null && _parent.AutoSize != AutoSize.Horizontal && _parent.AutoSize != AutoSize.HorizontalVertical)
            {
                if (_position.x + s.x > _parent.Size.x)
                    s.x = _parent.Size.x - _position.x;
            }

            _size = s;
        }

        private void ResizeBottom(int value)
        {
            Point s = _size;

            s.y += value;

            if (MinSize.y >= 0 && MaxSize.y > 0)
                s.y = Math.Min(s.y, MaxSize.y);

            s.y = Math.Max(s.y, MinSize.y);

            if (_parent != null && _parent.AutoSize != AutoSize.Vertical && _parent.AutoSize != AutoSize.HorizontalVertical)
            {
                if (_position.y + s.y > _parent.Size.y)
                    s.y = _parent.Size.y - _position.y;
            }

            _size = s;
        }

        private void ResizeTop(int value)
        {
            Point p = _position;
            Point s = _size;

            s.y -= value;

            if (MinSize.y >= 0 && MaxSize.y > 0)
                s.y = Math.Min(s.y, MaxSize.y);

            s.y = Math.Max(s.y, MinSize.y);

            p.y += (_size.y - s.y);

            if (p.y < 0)
            {
                s.y += p.y;
                p.y = 0;
            }

            _position = p;
            _size = s;
        }

        private void SetBounds()
        {
            if (_parent == null) return;

            Bounds.Left = _position.x;
            Bounds.Top = _position.y;
            Bounds.Right = _parent._size.x - (_position.x + _size.x);
            Bounds.Bottom = _parent._size.y - (_position.y + _size.y);
        }

        protected void PerformLayoutAndClip()
        {
            Point savedSize = _size;

            if (Dock != DockStyle.None)
                LayoutDock();
            else
                LayoutAnchor();

            SetDockRegions();

            var scale = GetScale();
            var loc = Location;
            var size = _size;

            //loc.Scale(scale);
            size.Scale(scale);

            ClipRect.From(ref loc, ref size);

            if (_parent != null)
                ClipRect.ClipBy(ref _parent.ClipRect);

            if (_size.x != savedSize.x || _size.y != savedSize.y)
                SizeChanged?.Invoke(this);
        }

        private void SetDockRegions()
        {
            DockAreaC.Top = _padding.Top;
            DockAreaC.Left = _padding.Left;
            DockAreaC.Right = _size.x - _padding.Right;
            DockAreaC.Bottom = _size.y - _padding.Bottom;

            DockAreaE.Top = 0;
            DockAreaE.Left = 0;
            DockAreaE.Right = _size.x;
            DockAreaE.Bottom = _size.y;
        }

        private ref Rectangle GetDockArea()
        {
            if (_isElement) return ref _parent.DockAreaE;
            return ref _parent.DockAreaC;
        }

        private void LayoutDock()
        {
            ref Rectangle rect = ref GetDockArea();

            int bottom, left, right, top;

            switch(Dock)
            {
                case DockStyle.Bottom:
                    bottom = rect.Bottom - _margin.Bottom;
                    left = rect.Left + _margin.Left;
                    right = rect.Right - _margin.Right;

                    _position.x = left;
                    _position.y = bottom - _size.y;// -_margin.Top;

                    _size.x = right - left;
                    rect.Bottom = _position.y - _margin.Top;
                    break;

                case DockStyle.Fill:
                    bottom = rect.Bottom - _margin.Bottom;
                    left = rect.Left + _margin.Left;
                    right = rect.Right - _margin.Right;
                    top = rect.Top + _margin.Top;

                    _position.x = left;
                    _position.y = top;

                    _size.x = right - left;
                    _size.y = bottom - top;
                    break;

                case DockStyle.Left:
                    left = rect.Left + _margin.Left;
                    top = rect.Top + _margin.Top;
                    bottom = rect.Bottom - _margin.Bottom;

                    _position.x = left;
                    _position.y = top;

                    _size.y = bottom - top;
                    rect.Left = left + _size.x + _margin.Right;
                    break;

                case DockStyle.Right:
                    right = rect.Right - _margin.Right;
                    top = rect.Top + _margin.Top;
                    bottom = rect.Bottom - _margin.Bottom;

                    _position.x = right - _size.x;
                    _position.y = top;

                    _size.y = bottom - top;
                    rect.Right = _position.x - _margin.Left;

                    break;

                case DockStyle.Top:

                    top = rect.Top + _margin.Top;
                    left = rect.Left + _margin.Left;
                    right = rect.Right - _margin.Right;

                    _position.x = left;
                    _position.y = top;

                    _size.x = right - left;
                    rect.Top = top + _size.y + _margin.Bottom;
                    break;

                case DockStyle.CenterY:
                    left = rect.Left + _margin.Left;
                    right = rect.Right - _margin.Right;

                    _position.x = left;

                    _size.x = right - left;
                    _position.y = (_parent._size.y - _size.y) / 2;
                    break;

                case DockStyle.CenterX:
                    top = rect.Top + _margin.Top;
                    bottom = rect.Bottom - _margin.Bottom;

                    _position.y = top;

                    _size.y = bottom - top;
                    _position.x = (_parent._size.x - _size.x) / 2;
                    break;

                case DockStyle.Center:
                    _position = (_parent.Size - _size) / 2;

                    break;

                case DockStyle.FillY:
                    bottom = rect.Bottom - _margin.Bottom;
                    top = rect.Top + _margin.Top;

                    _position.y = top;
                    _size.y = bottom - top;
                    break;

                case DockStyle.FillX:
                    left = rect.Left + _margin.Left;
                    right = rect.Right - _margin.Right;

                    _position.x = left;
                    _size.x = right - left;
                    break;
            }

            SetBounds();
        }

        private void LayoutAnchor()
        {
            switch (_anchor)
            {
                case AnchorStyles.Top:
                    _position.x = _parent._size.x - Bounds.Right - _size.x;
                    break;
                case AnchorStyles.Top | AnchorStyles.Left:
                    // position unchanged
                    break;
                case AnchorStyles.Top | AnchorStyles.Right:
                    _position.x = _parent._size.x - Bounds.Right - _size.x;
                    break;
                case AnchorStyles.Top | AnchorStyles.Bottom:
                    _size.y = (_parent._size.y - Bounds.Bottom) - _position.y;
                    break;
                case AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left:
                    _size.y = (_parent._size.y - Bounds.Bottom) - _position.y;
                    break;
                case AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right:
                    _size.y = (_parent.Size.y - Bounds.Bottom) - _position.y;
                    _position.x = _parent._size.x - Bounds.Right - _size.x;
                    break;
                case AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right:
                    _size.x = (_parent._size.x - Bounds.Right) - _position.x;
                    break;
                case AnchorStyles.Left | AnchorStyles.Right:
                    _size.x = (_parent._size.x - Bounds.Right) - _position.x;
                    break;
                case AnchorStyles.Bottom | AnchorStyles.Left:
                    _position.y = _parent._size.y - Bounds.Bottom - _size.y;
                    break;
                case AnchorStyles.Bottom | AnchorStyles.Right:
                    _position.x = _parent._size.x - Bounds.Right - _size.x;
                    _position.y = _parent._size.y - Bounds.Bottom - _size.y;
                    break;
                case AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right:
                    _size.x = (_parent._size.x - Bounds.Right) - _position.x;
                    _position.y = _parent._size.y - Bounds.Bottom - _size.y;
                    break;
                case AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right:
                    _size.x = (_parent._size.x - Bounds.Right) - _position.x;
                    _size.y = (_parent._size.y - Bounds.Bottom) - _position.y;
                    break;
            }
        }

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

        private void DrawChildren()
        {
            if (IsContainer)
            {
                for (int i = 0; i < LocalContainer.Controls.Count; i++)
                    LocalContainer.Controls[i].Draw();
            }
        }

        private void DrawElements()
        {
            for (int i = 0; i < Elements.Count; i++)
                Elements[i].Draw();
        }

        protected void RepeatTexture(int texture, Point loc, Rectangle rect, TextureMode mode, int color)
        {
            var scale = GetScale();
            Point texsize = Gui.Renderer.GetTextureSize(texture);
            Point size = _size * scale;
            
            int width = rect.Width != 0 ? rect.Width : texsize.x;
            int height = rect.Height != 0 ? rect.Height : texsize.y;

            int countx = (int)Math.Ceiling((float)size.x / width);
            int county = (int)Math.Ceiling((float)size.y / height);

            if (mode == TextureMode.RepeatX)
            {
                county = 1;
                height = size.y;
            }
            else if (mode == TextureMode.RepeatY)
            {
                countx = 1;
                width = size.x;
            }

            for (int j = 0; j < county; j++)
            {
                for (int i = 0; i < countx; i++)
                {
                    Rectangle newrect = rect;

                    int deltax = (width + width * i) - size.x;
                    int deltay = (height + height * j) - size.y;

                    int clippedx = width;
                    int clippedy = height;

                    if (deltax > 0)
                    {
                        newrect.Right = rect.Right - deltax;
                        clippedx = width - deltax;
                    }

                    if (deltay > 0)
                    {
                        newrect.Bottom = rect.Bottom - deltay;
                        clippedy = height - deltay;
                    }

                    Gui.Renderer.DrawTexture(texture, loc.x + width * i, loc.y + height * j, clippedx, clippedy, newrect, color);
                }
            }
        }

        protected void SliceTexture(int texture, TextureMode mode, Rectangle rect, Margin grid, int color)
        {
            bool repeat = mode == TextureMode.GridRepeat;
            var scale = GetScale();

            Point location = Location;// * scale;
            Point size = _size * scale;

            grid.Right = (int)(grid.Right * scale);
            grid.Left = (int)(grid.Left * scale);
            grid.Top = (int)(grid.Top * scale);
            grid.Bottom = (int)(grid.Bottom * scale);

            Rectangle outside = new Rectangle(location, size);
            Rectangle inside = new Rectangle(location + new Point(grid.Left, grid.Top), size - new Point(grid.Left + grid.Right, grid.Top + grid.Bottom));

            Rectangle slice = new Rectangle();

            int x1 = rect.Left + grid.Left;
            int y1 = rect.Top + grid.Top;
            int x2 = rect.Right - grid.Right;
            int y2 = rect.Bottom - grid.Bottom;

            if (grid.Top > 0 && grid.Left > 0)
            {
                //// draw top left
                slice.Left = rect.Left;
                slice.Top = rect.Top;
                slice.Right = x1;
                slice.Bottom = y1;

                Gui.Renderer.DrawTexture(texture, outside.Left, outside.Top, grid.Left, grid.Top, slice, color);
            }

            if (grid.Top > 0 && grid.Right > 0)
            {
                //// draw top right
                slice.Left = x2;
                slice.Top = rect.Top;
                slice.Right = rect.Right;
                slice.Bottom = y1;

                Gui.Renderer.DrawTexture(texture, inside.Right, outside.Top, grid.Right, grid.Top, slice, color);
            }

            if (grid.Bottom > 0 && grid.Left > 0)
            {
                //// draw bottom left
                slice.Left = rect.Left;
                slice.Top = y2;
                slice.Right = x1;
                slice.Bottom = rect.Bottom;

                Gui.Renderer.DrawTexture(texture, outside.Left, inside.Bottom, grid.Left, grid.Bottom, slice, color);
            }

            if (grid.Bottom > 0 && grid.Right > 0)
            {
                //// draw bottom right
                slice.Left = x2;
                slice.Top = y2;
                slice.Right = rect.Right;
                slice.Bottom = rect.Bottom;

                Gui.Renderer.DrawTexture(texture, inside.Right, inside.Bottom, grid.Right, grid.Bottom, slice, color);
            }

            if (grid.Left > 0)
            {
                // draw left
                slice.Left = rect.Left;
                slice.Top = y1;
                slice.Right = x1;
                slice.Bottom = y2;

                if (!repeat)
                {
                    Gui.Renderer.DrawTexture(texture, outside.Left, inside.Top, grid.Left, inside.Height, slice, color);
                }
                else
                {
                    int sliceSize = rect.Height - (grid.Top + grid.Bottom);
                    int count = (int)Math.Ceiling((float)inside.Height / sliceSize);

                    if (inside.Height < sliceSize)
                    {
                        slice.Bottom = y2 - (sliceSize - inside.Height);
                        sliceSize = inside.Height;
                    }

                    int h = 0;
                    for (int i = 0; i < count; i++)
                    {
                        h += sliceSize;

                        if (h > inside.Height)
                        {
                            int delta = h - inside.Height;

                            slice.Bottom = y2 - delta;
                            int clipped = sliceSize - delta;

                            Gui.Renderer.DrawTexture(texture, outside.Left, inside.Top + sliceSize * i, grid.Left, clipped, slice, color);
                        }
                        else
                        {
                            Gui.Renderer.DrawTexture(texture, outside.Left, inside.Top + sliceSize * i, grid.Left, sliceSize, slice, color);
                        }
                    }
                }
            }

            if (grid.Top > 0)
            {
                // draw top
                slice.Left = x1;
                slice.Top = rect.Top;
                slice.Right = x2;
                slice.Bottom = y1;

                if (!repeat)
                {
                    Gui.Renderer.DrawTexture(texture, inside.Left, outside.Top, inside.Width, grid.Top, slice, color);
                }
                else
                {
                    int sliceSize = rect.Width - (grid.Left + grid.Right);
                    int count = (int)Math.Ceiling((float)inside.Width / sliceSize);

                    if (inside.Width < sliceSize)
                    {
                        slice.Right = x2 - (sliceSize - inside.Width);
                        sliceSize = inside.Width;
                    }

                    int w = 0;
                    for (int i = 0; i < count; i++)
                    {
                        w += sliceSize;

                        if (w > inside.Width)
                        {
                            int delta = w - inside.Width;

                            slice.Right = x2 - delta;
                            int clipped = sliceSize - delta;

                            Gui.Renderer.DrawTexture(texture, inside.Left + sliceSize * i, outside.Top, clipped, grid.Top, slice, color);
                        }
                        else
                        {
                            Gui.Renderer.DrawTexture(texture, inside.Left + sliceSize * i, outside.Top, sliceSize, grid.Top, slice, color);
                        }
                    }
                }
            }

            if (grid.Right > 0)
            {
                // draw right
                slice.Left = x2;
                slice.Top = y1;
                slice.Right = rect.Right;
                slice.Bottom = y2;

                if (!repeat)
                {
                    Gui.Renderer.DrawTexture(texture, inside.Right, inside.Top, grid.Right, inside.Height, slice, color);
                }
                else
                {
                    int sliceSize = rect.Height - (grid.Top + grid.Bottom);
                    int count = (int)Math.Ceiling((float)inside.Height / sliceSize);

                    if (inside.Height < sliceSize)
                    {
                        slice.Bottom = y2 - (sliceSize - inside.Height);
                        sliceSize = inside.Height;
                    }

                    int h = 0;
                    for (int i = 0; i < count; i++)
                    {
                        h += sliceSize;

                        if (h > inside.Height)
                        {
                            int delta = h - inside.Height;

                            slice.Bottom = y2 - delta;
                            int clipped = sliceSize - delta;

                            Gui.Renderer.DrawTexture(texture, inside.Right, inside.Top + sliceSize * i, grid.Right, clipped, slice, color);
                        }
                        else
                        {
                            Gui.Renderer.DrawTexture(texture, inside.Right, inside.Top + sliceSize * i, grid.Right, sliceSize, slice, color);
                        }
                    }
                }
            }

            if (grid.Bottom > 0)
            {
                // draw bottom
                slice.Left = x1;
                slice.Top = y2;
                slice.Right = x2;
                slice.Bottom = rect.Bottom;

                if (!repeat)
                {
                    Gui.Renderer.DrawTexture(texture, inside.Left, inside.Bottom, inside.Width, grid.Bottom, slice, color);
                }
                else
                {
                    int sliceSize = rect.Width - (grid.Left + grid.Right);
                    int count = (int)Math.Ceiling((float)inside.Width / sliceSize);

                    if (inside.Width < sliceSize)
                    {
                        slice.Right = x2 - (sliceSize - inside.Width);
                        sliceSize = inside.Width;
                    }

                    int w = 0;
                    for (int i = 0; i < count; i++)
                    {
                        w += sliceSize;

                        if (w > inside.Width)
                        {
                            int delta = w - inside.Width;

                            slice.Right = x2 - delta;
                            int clipped = sliceSize - delta;

                            Gui.Renderer.DrawTexture(texture, inside.Left + sliceSize * i, inside.Bottom, clipped, grid.Bottom, slice, color);
                        }
                        else
                        {
                            Gui.Renderer.DrawTexture(texture, inside.Left + sliceSize * i, inside.Bottom, sliceSize, grid.Bottom, slice, color);
                        }
                    }
                }
            }

            // draw center
            slice.Left = x1;
            slice.Top = y1;
            slice.Right = x2;
            slice.Bottom = y2;

            Gui.Renderer.DrawTexture(texture, inside.Left, inside.Top, inside.Width, inside.Height, slice, color);
        }

        private void SetEnabled(bool value)
        {
            if (_parentEnabled == value)
                return;

            _parentEnabled = value;

            if (IsContainer)
            {
                foreach (Control control in LocalContainer.Controls)
                    control.SetEnabled(value);
            }

            foreach (Control control in Elements)
                control.SetEnabled(value);
        }


        // ---------- internal ----------------

        internal void PerformLateUpdate()
        {
            OnLateUpdate();

            for (int i = 0; i < Elements.Count; i++)
                Elements[i].PerformLateUpdate();

            if (IsContainer)
            {
                for (int i = 0; i < LocalContainer.Controls.Count; i++)
                    LocalContainer.Controls[i].PerformLateUpdate();
            }
        }

        internal void Draw()
        {
            if (!Visible) return;

            if (_size.x <= 0 || _size.y <= 0)
                return;

            if (ClipRect.IsZeroSize)
                return;

            DrawBefore();

            if (Scissor || Gui.AlwaysScissor)
                SetScissor(Math.Max(0, ClipRect.Left), Math.Max(0, ClipRect.Top), ClipRect.Width, ClipRect.Height);

            if (FadeSpeed > 0 || Gui.GlobalFadeSpeed > 0)
            {
                Style next = LocalStyle.Styles[_state];
                float opacity = GetOpacity(next.Opacity);

                if (_oldState != _state && FadeIn < 1 && (TextureFade || FontFade))
                {
                    Style last = LocalStyle.Styles[_oldState];

                    float a1 = GetOpacity(last.Opacity) * FadeOut;
                    float a2 = GetOpacity(next.Opacity) * FadeIn;

                    if (TextureFade)
                    {
                        if (FadeOut > 0) DrawStyle(last, a1);
                        DrawStyle(next, a2);
                    }
                    else
                        DrawStyle(next, opacity);

                    if (FontFade)
                    {
                        if (FadeOut > 0) DrawText(last, a1);
                        DrawText(next, a2);
                    }
                    else
                        DrawText(next, opacity);
                }
                else
                {
                    DrawStyle(next, opacity);
                    DrawText(next, opacity);
                }
            }
            else
            {
                Style style = LocalStyle.Styles[_state];
                float opacity = GetOpacity(style.Opacity);

                DrawStyle(style, opacity);
                DrawText(style, opacity);
            }

            DrawBeforeChildren();
            DrawChildren();
            DrawElements();
            DrawCustom();

            if (Scissor || Gui.AlwaysScissor)
                ResetScissor();
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

        /// <summary>
        /// Raises the <see cref="E:DragEnter" /> event.
        /// </summary>
        /// <param name="e">The <see cref="DragDropEventArgs"/> instance containing the event data.</param>
        internal void OnDragEnter(DragDropEventArgs e)
        {
            if (e.Cancel) return;
            DragEnter?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:DragLeave" /> event.
        /// </summary>
        /// <param name="e">The <see cref="DragDropEventArgs"/> instance containing the event data.</param>
        internal void OnDragLeave(DragDropEventArgs e)
        {
            if (e.Cancel) return;
            DragLeave?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:DragResponse" /> event.
        /// </summary>
        /// <param name="e">The <see cref="DragDropEventArgs"/> instance containing the event data.</param>
        internal void OnDragResponse(DragDropEventArgs e)
        {
            if (e.Cancel) return;
            DragResponse?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:DragDrop" /> event.
        /// </summary>
        /// <param name="e">The <see cref="DragDropEventArgs"/> instance containing the event data.</param>
        internal void OnDragDrop(DragDropEventArgs e)
        {
            DragDrop?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:Dropped" /> event.
        /// </summary>
        /// <param name="e">The <see cref="DragDropEventArgs"/> instance containing the event data.</param>
        internal void OnDrop(DragDropEventArgs e)
        {
            Drop?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:MouseDoubleClick" /> event.
        /// </summary>
        /// <param name="button">The button.</param>
        internal void OnMouseDoubleClick(int button)
        {
            MouseDoubleClick?.Invoke(this, new MouseEventArgs { Button = button });
        }

        /// <summary>
        /// Raised the MouseDrag event
        /// </summary>
        /// <param name="button">The button.</param>
        internal void OnMouseDrag(int button)
        {
            MouseDrag?.Invoke(this, new MouseEventArgs { Button = button });
        }

        /// <summary>
        /// Raised the MousePress event
        /// </summary>
        /// <param name="button">The button.</param>
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
        /// <param name="button">The button.</param>
        internal void OnMouseClick(int button)
        {
            MouseClick?.Invoke(this, new MouseEventArgs { Button = button });
        }

        /// <summary>
        /// Raised the MouseRelease event
        /// </summary>
        /// <param name="button"></param>
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
        /// <param name="button"></param>
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
        /// <param name="button"></param>
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


        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Control control)) return false;
            return control.AutoId == AutoId;
        }
    }
}
