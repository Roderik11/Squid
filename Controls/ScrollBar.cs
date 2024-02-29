using System;
using System.Collections.Generic;
using System.Text;

namespace Squid
{
    /// <summary>
    /// A ScrollBar.Can be used vertically and horizontally.
    /// </summary>
    [Toolbox]
    public class ScrollBar : Control
    {
        private float _wheelScroll;

        /// <summary>
        /// Raised when [value changed].
        /// </summary>
        public event VoidEvent ValueChanged;

        /// <summary>
        /// Gets the button up.
        /// </summary>
        /// <value>The button up.</value>
        public Button ButtonUp { get; private set; }

        /// <summary>
        /// Gets the button down.
        /// </summary>
        /// <value>The button down.</value>
        public Button ButtonDown { get; private set; }

        /// <summary>
        /// Gets the slider.
        /// </summary>
        /// <value>The slider.</value>
        public Slider Slider { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show always].
        /// </summary>
        /// <value><c>true</c> if [show always]; otherwise, <c>false</c>.</value>
        public bool ShowAlways { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ScrollBar"/> is ease.
        /// </summary>
        /// <value><c>true</c> if ease; otherwise, <c>false</c>.</value>
        public bool Ease
        {
            get => Slider.Ease; 
            set => Slider.Ease = value;
        }

        /// <summary>
        /// Gets the eased value.
        /// </summary>
        /// <value>The eased value.</value>
        public float EasedValue => Slider.EasedValue;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public float Value
        {
            get => Slider.Value; 
            set => Slider.Value = value;
        }

        /// <summary>
        /// Gets or sets the steps.
        /// </summary>
        /// <value>The steps.</value>
        public float Steps
        {
            get => Slider.Steps;
            set => Slider.Steps = value;
        }

        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        /// <value>The scale.</value>
        public float Scale
        {
            get => Slider.Scale; 
            set => Slider.Scale = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [auto scale].
        /// </summary>
        /// <value><c>true</c> if [auto scale]; otherwise, <c>false</c>.</value>
        public bool AutoScale
        {
            get => Slider.AutoScale; 
            set => Slider.AutoScale = value;
        }

        /// <summary>
        /// Gets or sets the mouse scroll speed.
        /// </summary>
        /// <value>The mouse scroll speed.</value>
        public float MouseScrollSpeed
        {
            get => _wheelScroll;
            set => _wheelScroll = value < 0 ? 0 : (value > 1 ? 1 : value);
        }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>The orientation.</value>
        public Orientation Orientation
        {
            get => Slider.Orientation;
            set
            {
                Slider.Orientation = value;

                if (Slider.Orientation == Squid.Orientation.Horizontal)
                {
                    ButtonUp.Dock = DockStyle.Left;
                    ButtonDown.Dock = DockStyle.Right;
                }
                else
                {
                    ButtonUp.Dock = DockStyle.Top;
                    ButtonDown.Dock = DockStyle.Bottom;
                }
            }
        }

        public void SetValue(float value)
        {
            Slider.SetValue(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollBar"/> class.
        /// </summary>
        public ScrollBar()
        {
            MouseScrollSpeed = 0.1f;

            Style = "scroll";

            ButtonUp = new Button();
            ButtonUp.Size = new Point(25, 26);
            ButtonUp.MouseDown += Btn1_MousePress;
            ButtonUp.Style = "scrollUp";
            Elements.Add(ButtonUp);

            ButtonDown = new Button();
            ButtonDown.Size = new Point(25, 26);
            ButtonDown.MouseDown += Btn2_MousePress;
            ButtonDown.Style = "scrollDown";
            Elements.Add(ButtonDown);

            Slider = new Slider();
            Slider.Dock = DockStyle.Fill;
            Slider.Button.Size = new Point(20, 30);
            Slider.AutoScale = true;
            Slider.Minimum = 0;
            Slider.Maximum = 1;
            Slider.ValueChanged += Slider_OnValueChanged;
            Slider.Style = "scrollSlider";
            Slider.Button.Style = "scrollSliderButton";
            Slider.MinHandleSize = 32;
            Elements.Add(Slider);

            Orientation = Orientation.Vertical;
        }

        void Slider_OnValueChanged(Control sender)
        {
            ValueChanged?.Invoke(this);
        }

        void Btn2_MousePress(Control sender, MouseEventArgs args)
        {
            if (args.Button > 0) return;

            if (Steps > 1)
                Slider.Value += 1f / Slider.Steps;
            else
            {
                if (Orientation == Squid.Orientation.Horizontal)
                    Slider.Value += 1f / Slider.Size.x;
                else
                    Slider.Value += 1f / Slider.Size.y;
            }
        }

        void Btn1_MousePress(Control sender, MouseEventArgs args)
        {
            if (args.Button > 0) return;

            if (Steps > 1)
                Slider.Value -= 1f / Slider.Steps;
            else
            {
                if (Orientation == Squid.Orientation.Horizontal)
                    Slider.Value -= 1f / Slider.Size.x;
                else
                    Slider.Value -= 1f / Slider.Size.y;
            }
        }

        /// <summary>
        /// Scrolls in the specified direction.
        /// </summary>
        /// <param name="direction">The direction.</param>
        public void Scroll(int direction)
        {
            if (direction > 0)
                Value += MouseScrollSpeed;
            else
                Value -= MouseScrollSpeed;
        }
    }
}
