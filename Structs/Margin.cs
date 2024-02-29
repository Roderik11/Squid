using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Collections;
using System.ComponentModel.Design.Serialization;

namespace Squid
{
    /// <summary>
    /// Struct Margin
    /// </summary>
    public struct Margin
    {
        public int Top;
        public int Left;
        public int Right;
        public int Bottom;

        public static readonly Margin Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="Margin"/> struct.
        /// </summary>
        /// <param name="all">All.</param>
        public Margin(int all)
        {
            Top = Left = Right = Bottom = all;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Margin"/> struct.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="right">The right.</param>
        /// <param name="bottom">The bottom.</param>
        public Margin(int left, int top, int right, int bottom)
        {
            Top = top;
            Left = left;
            Right = right;
            Bottom = bottom;
        }

        public void Scale(float scale)
        {
            Top = (int)(Top * scale);
            Left = (int)(Left * scale);
            Right = (int)(Right * scale);
            Bottom = (int)(Bottom * scale);
        }
             
        /// <summary>
        /// Gets or sets all.
        /// </summary>
        /// <value>All.</value>
        [RefreshProperties(RefreshProperties.All)]
        public int All
        {
            get
            {
                return Top;
            }
            set
            {
                Top = Left = Right = Bottom = value;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object other)
        {
            return (other is Margin) && ((Margin)other) == this;
        }

        /// <summary>
        /// Implements the ==.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Margin p1, Margin p2)
        {
            return (((p1.Left == p2.Left) && (p1.Top == p2.Top)) && (p1.Right == p2.Right)) && (p1.Bottom == p2.Bottom);
        }

        /// <summary>
        /// Implements the !=.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Margin p1, Margin p2)
        {
            return !(p1 == p2);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("{0}; {1}; {2}; {3}", Left, Top, Right, Bottom);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        static Margin()
        {
            Empty = new Margin(0);
        }
    }


}
