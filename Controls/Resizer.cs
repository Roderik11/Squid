using System;
using System.Collections.Generic;
using System.Text;

namespace Squid
{
    /// <summary>
    /// Delegate ResizeHandler
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="delta">The delta.</param>
    /// <param name="moved">The moved.</param>
    public delegate void ResizeHandler(Control sender, Point delta, Point moved);

    /// <summary>
    /// This control provides handles to resize its parent.
    /// </summary>
    [Hidden]
    public sealed class Resizer : Control
    {
        public Control Left { get; private set; }
        public Control Right { get; private set; }
     
        public Control Top { get; private set; }
        public Control TopLeft { get; private set; }
        public Control TopRight { get; private set; }

        public Control Bottom { get; private set; }
        public Control BottomLeft { get; private set; }
        public Control BottomRight { get; private set; }

        private Margin _grip = new Margin(8);
        private Point ClickedPos;
        private Point OldSize;

        public event MouseEvent GripDown;
        public event MouseEvent GripUp;
        public event ResizeHandler Resized;

        public Margin GripSize
        {
            get { return _grip; }
            set
            {
                if (_grip == value) return;
                _grip = value;
                Adjust();
            }
        }

        public Resizer()
        {
            NoEvents = true;
            Dock = DockStyle.Fill;

            Left = new Control();
            Left.Size = new Point(2, 2);
            Left.Dock = DockStyle.Left;
            Left.MouseDown += Grip_OnDown;
            Left.MousePress += Grip_OnPress;
            Left.MouseUp += Grip_OnUp;
            Left.Tag = AnchorStyles.Left;
            Left.Cursor = Cursors.SizeWE;
            Elements.Add(Left);

            Top = new Control();
            Top.Size = new Point(2, 2);
            Top.Dock = DockStyle.Top;
            Top.MouseDown += Grip_OnDown;
            Top.MousePress += Grip_OnPress;
            Top.MouseUp += Grip_OnUp;
            Top.Tag = AnchorStyles.Top;
            Top.Cursor = Cursors.SizeNS;
            Elements.Add(Top);

            Right = new Control();
            Right.Size = new Point(2, 2);
            Right.Dock = DockStyle.Right;
            Right.MouseDown += Grip_OnDown;
            Right.MousePress += Grip_OnPress;
            Right.MouseUp += Grip_OnUp;
            Right.Tag = AnchorStyles.Right;
            Right.Cursor = Cursors.SizeWE;
            Elements.Add(Right);

            Bottom = new Control();
            Bottom.Size = new Point(2, 2);
            Bottom.Dock = DockStyle.Bottom;
            Bottom.MouseDown += Grip_OnDown;
            Bottom.MousePress += Grip_OnPress;
            Bottom.MouseUp += Grip_OnUp;
            Bottom.Tag = AnchorStyles.Bottom;
            Bottom.Cursor = Cursors.SizeNS;
            Elements.Add(Bottom);

            TopLeft = new Control();
            TopLeft.Size = new Point(4, 4);
            TopLeft.Position = new Point(0, 0);
            TopLeft.MouseDown += Grip_OnDown;
            TopLeft.MousePress += Grip_OnPress;
            TopLeft.MouseUp += Grip_OnUp;
            TopLeft.Tag = AnchorStyles.Top | AnchorStyles.Left;
            TopLeft.Cursor = Cursors.SizeNWSE;
            Elements.Add(TopLeft);

            TopRight = new Control();
            TopRight.Size = new Point(4, 4);
            TopRight.Position = new Point(Size.x - 4, 0);
            TopRight.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            TopRight.MouseDown += Grip_OnDown;
            TopRight.MousePress += Grip_OnPress;
            TopRight.MouseUp += Grip_OnUp;
            TopRight.Tag = AnchorStyles.Top | AnchorStyles.Right;
            TopRight.Cursor = Cursors.SizeNESW;
            Elements.Add(TopRight);

            BottomLeft = new Control();
            BottomLeft.Size = new Point(4, 4);
            BottomLeft.Position = new Point(0, Size.y - 4);
            BottomLeft.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            BottomLeft.MouseDown += Grip_OnDown;
            BottomLeft.MousePress += Grip_OnPress;
            BottomLeft.MouseUp += Grip_OnUp;
            BottomLeft.Tag = AnchorStyles.Bottom | AnchorStyles.Left;
            BottomLeft.Cursor = Cursors.SizeNESW;
            Elements.Add(BottomLeft);

            BottomRight = new Control();
            BottomRight.Size = new Point(8, 8);
            BottomRight.Position = new Point(Size.x - 8, Size.y - 8);
            BottomRight.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BottomRight.MouseDown += Grip_OnDown;
            BottomRight.MousePress += Grip_OnPress;
            BottomRight.MouseUp += Grip_OnUp;
            BottomRight.Tag = AnchorStyles.Bottom | AnchorStyles.Right;
            BottomRight.Cursor = Cursors.SizeNWSE;
            Elements.Add(BottomRight);

            Adjust();
        }

        private void Adjust()
        {
            Left.Size = new Point(_grip.Left, _grip.Left);
            Top.Size = new Point(_grip.Top, _grip.Top);
            Right.Size = new Point(_grip.Right, _grip.Right);
            Bottom.Size = new Point(_grip.Bottom, _grip.Bottom);

            TopLeft.Size = new Point(_grip.Left, _grip.Top);
            TopLeft.Position = new Point(0, 0);

            TopRight.Size = new Point(_grip.Right, _grip.Top);
            TopRight.Position = new Point(Size.x - _grip.Right, 0);

            BottomLeft.Size = new Point(_grip.Left, _grip.Bottom);
            BottomLeft.Position = new Point(0, Size.y - _grip.Bottom);

            BottomRight.Size = new Point(_grip.Right, _grip.Bottom);
            BottomRight.Position = new Point(Size.x - _grip.Right, Size.y - _grip.Bottom);

            Left.Visible = !Left.Size.IsEmpty;
            Top.Visible = !Top.Size.IsEmpty;
            Right.Visible = !Right.Size.IsEmpty;
            Bottom.Visible = !Bottom.Size.IsEmpty;

            TopLeft.Visible = Top.Visible && Left.Visible;
            TopRight.Visible = Top.Visible && Right.Visible;
            BottomLeft.Visible = Bottom.Visible && Left.Visible;
            BottomRight.Visible = Bottom.Visible && Right.Visible;
        }

        void Grip_OnDown(Control sender, MouseEventArgs args)
        {
            if (args.Button > 0) return;

            ClickedPos = Gui.MousePosition / GetScale();
            OldSize = Parent.Size;

            GripDown?.Invoke(sender, args);
        }

        void Grip_OnUp(Control sender, MouseEventArgs args)
        {
            if (args.Button > 0) return;

            GripUp?.Invoke(sender, args);
        }

        void Grip_OnPress(Control sender, MouseEventArgs args)
        {
            if (args.Button > 0) return;

            Point mousePos = Gui.MousePosition / GetScale();
            Point p = mousePos - ClickedPos;

            Point position = Parent.Position;
            Point size = Parent.Size;

            AnchorStyles anchor = (AnchorStyles) sender.Tag;

            if ((anchor & AnchorStyles.Left) == AnchorStyles.Left)
                p.x = ClickedPos.x - mousePos.x;
            
            if ((anchor & AnchorStyles.Top) == AnchorStyles.Top)
                p.y = ClickedPos.y - mousePos.y;

            Parent.ResizeTo(OldSize + p, anchor);

            Resized?.Invoke(this, Parent.Size - size, Parent.Position - position);
        }
    }
}
