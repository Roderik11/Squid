using System;
using System.ComponentModel;

namespace Squid
{
    /// <summary>
    /// A Slider control. Also known as TrackBar.
    /// </summary>
    [Toolbox]
    public class Slider : Control
    {
        private float _value;
        private float _easeScroll;
        private Point Offset;

        /// <summary>
        /// Raised when [value changed].
        /// </summary>
        public event VoidEvent ValueChanged;

        /// <summary>
        /// Gets or sets the steps.
        /// </summary>
        /// <value>The steps.</value>
        public float Steps { get; set; }

        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        /// <value>The scale.</value>
        public float Scale { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [auto scale].
        /// </summary>
        /// <value><c>true</c> if [auto scale]; otherwise, <c>false</c>.</value>
        public bool AutoScale { get; set; }

        public int MinHandleSize { get; set; }

        /// <summary>
        /// Gets or sets the minimum.
        /// </summary>
        /// <value>The minimum.</value>
        public float Minimum { get; set; }

        /// <summary>
        /// Gets or sets the maximum.
        /// </summary>
        /// <value>The maximum.</value>
        public float Maximum { get; set; }

        /// <summary>
        /// Gets the button.
        /// </summary>
        /// <value>The button.</value>
        public Button Button { get; private set; }
        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>The orientation.</value>
        public Orientation Orientation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Slider"/> is ease.
        /// </summary>
        /// <value><c>true</c> if ease; otherwise, <c>false</c>.</value>
        [DefaultValue(true)]
        public bool Ease { get; set; }

        /// <summary>
        /// Gets the eased value.
        /// </summary>
        /// <value>The eased value.</value>
        public float EasedValue { get; private set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public float Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value == value) return;
                _value = value;
                _value = Math.Min(_value, Maximum);
                _value = Math.Max(_value, Minimum);

                ValueChanged?.Invoke(this);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Slider"/> class.
        /// </summary>
        public Slider()
        {
            Minimum = 0;
            Maximum = 100;
            Ease = true;

            Style = "slider";
            Scale = 1;
            Size = new Point(20, 100);
            Orientation = Orientation.Vertical;

            Button = new Button();
            Button.Size = new Point(20, 20);
            Button.MouseDown += Button_MouseDown;
            Button.Style = "sliderButton";
            Elements.Add(Button);

            MouseDown += Slider_MouseDown;
        }

        public void SetValue(float value)
        {
            Value = EasedValue = value;
            _easeScroll = value;
        }

        void Slider_MouseDown(Control sender, MouseEventArgs args)
        {
            if (args.Button > 0) return;

            Point position = Gui.MousePosition - Location - Button.Size / 2;

            if (Orientation == Orientation.Vertical)
                Value = Minimum + (Maximum - Minimum) * position.y / (Size.y - Button.Size.y);
            else
                Value = Minimum + (Maximum - Minimum) * position.x / (Size.x - Button.Size.x);

            Snap();
        }

        private void Snap()
        {
            if (Steps > 1)
            {
                float snap = 1f / Steps;
                Value = (float)Math.Ceiling(_value / snap) * snap;
            }
        }

        void Button_MouseDown(Control sender, MouseEventArgs args)
        {
            if (args.Button > 0) return;

            Offset = Gui.MousePosition - sender.Location;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            Desktop root = Desktop;
            if (root == null) return;

            if (AutoScale)
            {
                Scale = Math.Min(1, Math.Max(0, Scale));

                if (Orientation == Orientation.Vertical)
                {
                    int size = Size.y;// (int)(Size.y - Button.Margin.Top - Button.Margin.Bottom);
                    int actualSize = (int)((float)size * Scale);

                    if (MinHandleSize > 0 && actualSize < MinHandleSize)
                        Scale = (float)MinHandleSize / (float)size;
                    
                    Button.Size = new Point(Button.Size.x, (int)(size * Scale));
                }
                else
                {
                    int size = Size.x;// (int)(Size.y - Button.Margin.Top - Button.Margin.Bottom);
                    int actualSize = (int)((float)size * Scale);

                    if (MinHandleSize > 0 && actualSize < MinHandleSize)
                        Scale = MinHandleSize / size;

                    Button.Size = new Point((int)(size * Scale), Button.Size.y);
                }
            }

            Button.Dock = Orientation == Orientation.Vertical ? DockStyle.FillX : DockStyle.FillY;

            if (root.PressedControl == Button)
            {
                if (!(Scale >= 1 && AutoScale))
                {
                    Point position = Gui.MousePosition - Location;

                    if (Orientation == Orientation.Vertical)
                    {
                        position.x = Button.Position.x;
                        position.y -= Offset.y;
                        position.y = Math.Max(0, Math.Min(Size.y - Button.Size.y, position.y));

                        Button.Position = position;

                        Value = Minimum + (Maximum - Minimum) * position.y / (Size.y - Button.Size.y);
                    }
                    else
                    {
                        position.y = Button.Position.y;
                        position.x -= Offset.x;
                        position.x = Math.Max(0, Math.Min(Size.x - Button.Size.x, position.x));

                        Button.Position = position;

                        Value = Minimum + (Maximum - Minimum) * position.x / (Size.x - Button.Size.x);
                    }

                    Snap();

                    _easeScroll = _value;
                }
            }
            else
            {
                Snap();

                float m;
                if (Ease)
                {
                    _easeScroll += ((_value - _easeScroll) / 8f) * Math.Min(8, Gui.TimeElapsed * 0.1f);
                    m = (_easeScroll - Minimum) / (Maximum - Minimum);
                }
                else
                {
                    m = (_value - Minimum) / (Maximum - Minimum);
                }

                Point end;
                if (Orientation == Orientation.Vertical)
                {
                    int size = Size.y;// (int)(Size.y - Button.Margin.Top - Button.Margin.Bottom);
                    int y = (int)(m * (size - Button.Size.y));
                    end = new Point(Button.Position.x, y);

                    Button.Position = end;
                }
                else
                {
                    int x = (int)(m * (Size.x - Button.Size.x));
                    end = new Point(x, Button.Position.y);

                    Button.Position = end;
                }
            }

            if (Ease)
                EasedValue += ((_value - EasedValue) / 8f) * Math.Min(8, Gui.TimeElapsed * 0.1f);
            else
                EasedValue = _value;
        }
    }
}
