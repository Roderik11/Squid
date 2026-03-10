using System;

namespace Squid
{
    public partial class Control
    {
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
        /// Override this to draw text.
        /// </summary>
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
        /// Override this to do additional drawing
        /// Call the base method to draw the control as usual
        /// </summary>
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
        protected Rectangle Clip(Rectangle rect)
        {
            if (_parent != null)
                return _parent.ClipRect.Clip(rect);

            return rect;
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
    }
}
