using System;

namespace Squid
{
    public partial class Control
    {
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
    }
}
