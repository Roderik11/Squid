using System;
using System.Collections.Generic;
using System.Text;

namespace Squid
{
    /// <summary>
    /// A SplitContainer. Can be used horizontally and vertically.
    /// This is a Frame|Button|Frame combination.
    /// The Button resizes Frame1.
    /// </summary>
    [Toolbox]
    public class SplitContainer : Control 
    {
        private Orientation _orientation;
        private Point ClickedPos;
        private Point OldSize;

        /// <summary>
        /// Gets the split frame1.
        /// </summary>
        /// <value>The split frame1.</value>
        public Frame SplitFrame1 { get; private set; }

        /// <summary>
        /// Gets the split frame2.
        /// </summary>
        /// <value>The split frame2.</value>
        public Frame SplitFrame2 { get; private set; }

        /// <summary>
        /// Gets the split button.
        /// </summary>
        /// <value>The split button.</value>
        public Button SplitButton { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether [retain aspect].
        /// </summary>
        /// <value><c>true</c> if [retain aspect]; otherwise, <c>false</c>.</value>
        public bool RetainAspect { get; set; }

        /// <summary>
        /// Gets or sets the aspect ratio.
        /// </summary>
        /// <value>The aspect ratio.</value>
        public float AspectRatio { get; set; }

         /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>The orientation.</value>
        public Orientation Orientation
        {
            get { return _orientation; }
            set
            {
                if (value == _orientation) return;
                _orientation = value;
                ChangeOrientation();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SplitContainer"/> class.
        /// </summary>
        public SplitContainer()
        {
            Orientation = Orientation.Horizontal;
            Size = new Point(100, 100);

            SplitFrame1 = new Frame();
            SplitFrame1.Size = new Point();
            SplitFrame1.Dock = DockStyle.Left;
            SplitFrame1.Size = new Point(40, 40);
            Elements.Add(SplitFrame1);

            SplitButton = new Button();
            SplitButton.Dock = DockStyle.Left;
            SplitButton.Size = new Point(10, 10);
            SplitButton.MousePress += SplitButton_MousePress;
            SplitButton.MouseDown += SplitButton_MouseDown;
            SplitButton.Cursor = Cursors.VSplit;
            Elements.Add(SplitButton);

            SplitFrame2 = new Frame();
            SplitFrame2.Size = new Point();
            SplitFrame2.Dock = DockStyle.Fill;
            SplitFrame2.Size = new Point(50, 50);
            Elements.Add(SplitFrame2);

            RetainAspect = true;
            AspectRatio = (float)SplitFrame1.Size.x / (float)Size.x;
        }

        private void ChangeOrientation()
        {
            if (Orientation == Orientation.Horizontal)
            {
                SplitFrame1.Dock = DockStyle.Left;
                SplitButton.Size = new Point(SplitButton.Size.y, 10);
                SplitButton.Dock = DockStyle.Left;
                SplitButton.Cursor = Cursors.VSplit;
            }
            else
            {
                SplitFrame1.Dock = DockStyle.Top;
                SplitButton.Size = new Point(10, SplitButton.Size.x);
                SplitButton.Dock = DockStyle.Top;
                SplitButton.Cursor = Cursors.HSplit;
            }
        }

        void SplitButton_MouseDown(Control sender, MouseEventArgs args)
        {
            if (args.Button > 0) return;

            ClickedPos = Gui.MousePosition;
            OldSize = SplitFrame1.Size;
        }

        void SplitButton_MousePress(Control sender, MouseEventArgs args)
        {
            if (args.Button > 0) return;

            Point p = Gui.MousePosition - ClickedPos;

            if (Orientation == Orientation.Horizontal)
            {
                SplitFrame1.ResizeTo(OldSize + p, AnchorStyles.Right);
                AspectRatio = (float)SplitFrame1.Size.x / (float)Size.x;
            }
            else
            {
                SplitFrame1.ResizeTo(OldSize + p, AnchorStyles.Bottom);
                AspectRatio = (float)SplitFrame1.Size.y / (float)Size.y;
            }
        }

        protected override void OnLateUpdate()
        {
            if (RetainAspect)
            {
                if (Desktop.PressedControl != SplitButton)
                {
                    if (Orientation == Orientation.Horizontal)
                        SplitFrame1.Size = new Point((int)(AspectRatio * Size.x), SplitFrame1.Size.y);
                    else
                        SplitFrame1.Size = new Point(SplitFrame1.Size.x, (int)(AspectRatio * Size.y));
                }
            }
        }
    }
}
