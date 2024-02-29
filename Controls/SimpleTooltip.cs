using System;
using System.Collections.Generic;
using System.Text;

namespace Squid
{
    /// <summary>
    /// Simple text tooltip that fades in and out
    /// </summary>
    public class SimpleTooltip : Tooltip
    {
        private Control _context;
        private int FadeDirection = 1; // used to fade in&out
        private float DelayTimer; // timer to keep track of delay

        public bool AutoLayout = false;

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <value>The label.</value>
        public Label Label { get; private set; }

        /// <summary>
        /// Gets or sets the duration of the fade.
        /// </summary>
        /// <value>The duration of the fade.</value>
        public float FadeDuration { get; set; }

        /// <summary>
        /// Gets or sets the delay.
        /// </summary>
        /// <value>The delay.</value>
        public float Delay { get; set; }

        protected Control Context => _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleTooltip"/> class.
        /// </summary>
        public SimpleTooltip()
        {
            Delay = 500f; // 0.5 seconds delay
            FadeDuration = 500f; // 0.5 seconds fade
            Opacity = 0; // start transparent
            //AutoSize = AutoSize.HorizontalVertical;

            // lets just use a Label to display the tooltip as simple text
            Label = new Label();
            Label.BBCodeEnabled = true;
            Label.AutoSize = AutoSize.HorizontalVertical;
            Label.Style = "tooltip";
            Controls.Add(Label);
        }

        private Control realContext;

        /// <summary>
        /// Sets the context.
        // Gets called when the tooltip context is updated.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void SetContext(Control context)
        {
            // if there's no tooltip context
            if (context == null || string.IsNullOrEmpty(context.Tooltip))
            {
                // fade out
                FadeDirection = -1;
                realContext = null;
            }
            else
            {
                _context = context;
                realContext = context;

                // grab the tooltip text
                Label.Text = context.Tooltip;
                Label.Position = new Point(8, 8);

                // make the control visible
                Visible = true;
                PerformLayout();
                PerformUpdate();

                Size = Label.Size + new Point(16 + Padding.Left + Padding.Right, 16 + Padding.Top + Padding.Bottom);
                
                // fade in
                FadeDirection = 1;
            }
        }

        public override void LayoutTooltip()
        {
            if (AutoLayout)
                base.LayoutTooltip();
            else
                AlignTooltip();
        }

        void AlignTooltip()
        {
            if (_context == null) return;
            AlignTo(_context, _context.TooltipAlign);
        }

        public void AlignTo(Control context, Alignment align)
        {
            if (context == null) return;

            var p = TryAlign(context, align);

            Rectangle oos = OutOfScreen(p, Size);

            if (oos.Left != 0 || oos.Right != 0 || oos.Top != 0 || oos.Bottom != 0)
            {
                if (align == Alignment.MiddleLeft)
                {
                    if (oos.Top > 0)
                        p.y += oos.Top;
                    else if (oos.Bottom > 0)
                        p.y -= oos.Bottom;
                    else
                        p = TryAlign(context, Alignment.MiddleRight);
                }
                else if (align == Alignment.MiddleRight)
                {
                    if (oos.Top > 0)
                        p.y += oos.Top;
                    else if (oos.Bottom > 0)
                        p.y -= oos.Bottom;
                    else
                        p = TryAlign(context, Alignment.MiddleLeft);
                }
                else if (align == Alignment.TopCenter)
                {
                    if (oos.Left > 0)
                        p.x += oos.Left;
                    else if (oos.Right > 0)
                        p.x -= oos.Right;
                    else
                        p = TryAlign(context, Alignment.BottomCenter);
                }
                else if (align == Alignment.BottomCenter)
                {
                    if (oos.Left > 0)
                        p.x += oos.Left;
                    else if (oos.Right > 0)
                        p.x -= oos.Right;
                    else
                        p = TryAlign(context, Alignment.TopCenter);
                }
            }

            Position = p;
            PerformLayout();
            PerformUpdate();
        }

        protected Rectangle OutOfScreen(Point pos, Point size)
        {
            Rectangle result = new Rectangle(0, 0, 0, 0);

            if (pos.x < 0)
                result.Left = -pos.x;

            if (pos.y < 0)
                result.Top = -pos.y;

            if (pos.x + size.x > Desktop.Size.x)
                result.Right = (pos.x + size.x) - Desktop.Size.x;

            if (pos.y + size.y > Desktop.Size.y)
                result.Bottom = (pos.y + size.y) - Desktop.Size.y;

            return result;
        }

        protected Point TryAlign(Control context, Alignment align)
        {
            Point loc = context.Location;
            Point csize = context.Size;
            Point p = loc;

            switch (align)
            {
                case Alignment.TopCenter:
                    p = new Point(loc.x + csize.x / 2 - Size.x / 2, loc.y - Size.y);
                    break;
                case Alignment.BottomCenter:
                    p = new Point(loc.x + csize.x / 2 - Size.x / 2, loc.y + csize.y);
                    break;
                case Alignment.MiddleLeft:
                    p = new Point(loc.x - Size.x, loc.y + csize.y / 2 - Size.y / 2);
                    break;
                case Alignment.MiddleRight:
                    p = new Point(loc.x + csize.x, loc.y + csize.y / 2 - Size.y / 2);
                    break;
                case Alignment.BottomLeft:
                    p = loc + new Point(0, csize.y);
                    break;
                case Alignment.TopRight:
                    p = loc + new Point(csize.x, 0);
                    break;
                case Alignment.TopLeft:
                    p = loc - new Point(csize.x, 0);
                    break;
            }

            return p;
        }

        protected override void OnUpdate()
        {
            if(realContext != null)
            {
                if(Gui.MouseMovement.IsEmpty)
                {
                    if (DelayTimer < Delay)
                        DelayTimer += Gui.TimeElapsed;
                }
                else if(DelayTimer < Delay)
                {
                    DelayTimer = 0;
                }

                if (DelayTimer >= Delay)
                    Opacity += (Gui.TimeElapsed / FadeDuration);
                else
                    Opacity -= (Gui.TimeElapsed / FadeDuration);
            }
            else
            {
                DelayTimer = 0;
                Opacity -= (Gui.TimeElapsed / FadeDuration);
            }

            Opacity = Opacity < 0 ? 0 : (Opacity > 1 ? 1 : Opacity);

            // make the control invisible when completely faded out
            if (FadeDirection < 0 && Opacity == 0)
            {
                Visible = false;
                DelayTimer = 0;
            }
        }
    }
}
