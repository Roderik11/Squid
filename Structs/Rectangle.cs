using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Collections;

namespace Squid
{
    /// <summary>
    /// Struct Rectangle
    /// </summary>
    public struct Rectangle
    {
        public int Left;
        public int Right;
        public int Top;
        public int Bottom;

        public Point Size => new Point(Width, Height);
        public Point Position => new Point(Left, Top);

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>The width.</value>
        public int Width
        {
            get { return Right - Left; }
        }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>The height.</value>
        public int Height
        {
            get { return Bottom - Top; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public Rectangle(int x, int y, int width, int height)
        {
            Left = x;
            Top = y;
            Right = x + width;
            Bottom = y + height;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> struct.
        /// </summary>
        /// <param name="pos">The pos.</param>
        /// <param name="size">The size.</param>
        public Rectangle(Point pos, Point size)
        {
            Left = pos.x;
            Top = pos.y;

            Right = pos.x + size.x;
            Bottom = pos.y + size.y;
        }

        public void From(ref Point pos, ref Point size)
        {
            Left = pos.x;
            Top = pos.y;

            Right = pos.x + size.x;
            Bottom = pos.y + size.y;
        }

        public void From(int x, int y, int w, int h)
        {
            Left = x;
            Top = y;

            Right = x + w;
            Bottom = y + h;
        }

        /// <summary>
        /// Intersects the specified rect.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public bool Intersects(Rectangle rect)
        {
            return (rect.Left < (Left + Width)) && (Left < (rect.Left + rect.Width)) && (rect.Top < (Top + Height)) && (Top < (rect.Top + rect.Height));
        }

        /// <summary>
        /// Intersects the specified rect.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public bool Contains(Rectangle rect)
        {
            return rect.Left >= Left && Right >= rect.Right && rect.Top >= Top && Bottom >= rect.Bottom;
        }

        /// <summary>
        /// Clips the specified rect.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <returns>Rectangle.</returns>
        public Rectangle Clip(Rectangle rect)
        {
            Rectangle result = new Rectangle();
            result.Left = Math.Max(Left, rect.Left);
            result.Top = Math.Max(Top, rect.Top);

            result.Right = Math.Min(Right, rect.Right);
            result.Bottom = Math.Min(Bottom, rect.Bottom);

            return result;
        }

        public void ClipBy(ref Rectangle rect)
        {
            Left = Math.Max(Left, rect.Left);
            Top = Math.Max(Top, rect.Top);

            Right = Math.Min(Right, rect.Right);
            Bottom = Math.Min(Bottom, rect.Bottom);
        }

        /// <summary>
        /// Determines whether [contains] [the specified point].
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns><c>true</c> if [contains] [the specified point]; otherwise, <c>false</c>.</returns>
        public bool Contains(Point point)
        {
            return point.x > Left && point.x < Right && point.y > Top && point.y < Bottom;
        }

        /// <summary>
        /// Determines whether this instance is empty.
        /// </summary>
        /// <returns><c>true</c> if this instance is empty; otherwise, <c>false</c>.</returns>
        public bool IsEmpty => Left == Right && Top == Bottom;

        public bool IsZeroSize => Width < 1 || Height < 1;
    }
}
