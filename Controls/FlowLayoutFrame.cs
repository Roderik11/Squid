using System;
using System.Collections.Generic;
using System.Text;

namespace Squid
{
    /// <summary>
    /// A container that auto-layouts its children.
    /// Children must not be docked for this to work.
    /// </summary>
    [Toolbox]
    public class FlowLayoutFrame : Frame
    {
        private Point lastSize;

        /// <summary>
        /// Gets or sets the flow direction.
        /// </summary>
        /// <value>The flow direction.</value>
        public FlowDirection FlowDirection { get; set; }

        /// <summary>
        /// Gets or sets the H spacing.
        /// </summary>
        /// <value>The H spacing.</value>
        public int HSpacing { get; set; }

        /// <summary>
        /// Gets or sets the V spacing.
        /// </summary>
        /// <value>The V spacing.</value>
        public int VSpacing { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlowLayoutFrame"/> class.
        /// </summary>
        public FlowLayoutFrame()
        {
            Size = new Point(100, 100);

            FlowDirection = FlowDirection.LeftToRight;
            HSpacing = 4;
            VSpacing = 4;

            Controls.ItemAdded += Controls_ItemAdded;
            Controls.ItemRemoved += Controls_ItemRemoved;
            Controls.ItemsSorted += Controls_ItemsSorted;
        }

        void Controls_ItemsSorted(object sender, EventArgs e)
        {
            Refresh();
        }

        void Controls_ItemRemoved(object sender, ListEventArgs<Control> e)
        {
            Refresh();
        }

        void Controls_ItemAdded(object sender, ListEventArgs<Control> e)
        {
            Refresh();
        }

        private void LayoutLeftToRight()
        {
            int x = HSpacing;
            int y = VSpacing;
            int max = 0;
            int c = 0;

            foreach (Control control in Controls)
            {
                if (!control.Visible) continue;

                if (x + control.Size.x + HSpacing <= Size.x)
                {
                    control.Position = new Point(x, y);

                    x = x + control.Size.x + HSpacing;
                }
                else
                {
                    x = HSpacing;

                    if (c > 0)
                    {
                        y += max + VSpacing;
                        max = 0;
                        c = 0;
                    }

                    control.Position = new Point(x, y);

                    x = x + control.Size.x + HSpacing;
                }

                max = Math.Max(max, control.Size.y);

                c++;
            }
        }

        private void LayoutRightToLeft()
        {
            int x = Size.x;
            int y = HSpacing;
            int max = 0;
            int c = 0;

            foreach (Control control in Controls)
            {
                if (!control.Visible) continue;

                if (x - control.Size.x - HSpacing >= HSpacing)
                {
                    control.Position = new Point(x - control.Size.x - HSpacing, y);

                    x = x - control.Size.x - HSpacing;
                }
                else
                {
                    x = Size.x;

                    if (c > 0)
                    {
                        y += max + VSpacing;
                        max = 0;
                        c = 0;
                    }

                    control.Position = new Point(x, y);

                    x = x - control.Size.x - HSpacing;
                }

                max = Math.Max(max, control.Size.y);

                c++;
            }
        }

        private void LayoutTopToBottom()
        {
            int x = HSpacing;
            int y = HSpacing;
            int max = 0;
            int c = 0;

            foreach (Control control in Controls)
            {
                if (!control.Visible) continue;

                if (y + control.Size.y + VSpacing <= Size.y)
                {
                    control.Position = new Point(x, y);

                    y = y + control.Size.y + VSpacing;
                }
                else
                {
                    y = VSpacing;

                    if (c > 0)
                    {
                        x += max + HSpacing;
                        max = 0;
                        c = 0;
                    }

                    control.Position = new Point(x, y);

                    y = y + control.Size.y + VSpacing;
                }

                max = Math.Max(max, control.Size.x);

                c++;
            }
        }

        private void LayoutBottomToTop()
        {
            int x = HSpacing;
            int y = Size.y;
            int max = 0;
            int c = 0;

            foreach (Control control in Controls)
            {
                if (!control.Visible) continue;

                if (y - control.Size.y - VSpacing >= VSpacing)
                {
                    control.Position = new Point(x, y - control.Size.y - VSpacing);

                    y = y - control.Size.y - VSpacing;
                }
                else
                {
                    y = Size.y;

                    if (c > 0)
                    {
                        x += max + HSpacing;
                        max = 0;
                        c = 0;
                    }

                    control.Position = new Point(x, y);

                    y = y - control.Size.y - VSpacing;
                }

                max = Math.Max(max, control.Size.x);

                c++;
            }
        }

        protected override void OnUpdate()
        {
            if (!lastSize.Equals(Size))
            {
                Refresh();
                lastSize = Size;
            }
        }

        /// <summary>
        /// Forces the flow layout.
        /// </summary>
        public void ForceFlowLayout()
        {
            Refresh();
        }

        protected void Refresh()
        {
            switch (FlowDirection)
            {
                case FlowDirection.None:
                    return;
                case FlowDirection.LeftToRight:
                    LayoutLeftToRight();
                    break;
                case FlowDirection.TopToBottom:
                    LayoutTopToBottom();
                    break;
                case FlowDirection.RightToLeft:
                    LayoutRightToLeft();
                    break;
                case FlowDirection.BottomToTop:
                    LayoutBottomToTop();
                    break;
            }
        }
    }
}
