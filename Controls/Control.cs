using System;
using System.Collections.Generic;
using System.ComponentModel;
using Squid.Xml;
using System.Text.RegularExpressions;

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
        public event Action OnParentChanged;

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

                OnParentChanged?.Invoke();
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

    }
}
