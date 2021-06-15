using System;
using System.Collections.Generic;
using System.Text;
using Squid.Xml;
using System.ComponentModel;

namespace Squid
{
    /// <summary>
    /// A dictionary of ControlState/Style pairs
    /// </summary>
    public class StyleCollection : Dictionary<ControlState, Style>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StyleCollection"/> class.
        /// </summary>
        public StyleCollection() : base(new EqualityComparer()) { }

        internal class EqualityComparer : IEqualityComparer<ControlState>
        {
            public bool Equals(ControlState x, ControlState y)
            {
                return (int)x == (int)y;
            }

            public int GetHashCode(ControlState x)
            {
                return (int)x;
            }
        }
    }

    /// <summary>
    /// Describes the visual appearance of a control
    /// </summary>
    public sealed class Style
    {
        /// <summary>
        /// user data
        /// </summary>
        [Hidden, XmlIgnore]
        public object Tag;// { get; set; }

        /// <summary>
        /// color for any text to be drawn (argb)
        /// </summary>
        [IntColor]
        [Category("Text")]
        public int TextColor;// { get; set; }

        /// <summary>
        /// name of the font to use for text
        /// </summary>
        [Font]
        [Category("Text")]
        public string Font;// { get; set; }

        /// <summary>
        /// text padding (distance to control borders)
        /// </summary>
        [Category("Text")]
        public Margin TextPadding { get; set; }

        /// <summary>
        /// text alignment
        /// </summary>
        [Category("Text")]
        public Alignment TextAlign;// { get; set; }

        /// <summary>
        /// color to tint the texture (argb)
        /// </summary>
        [IntColor]
        [Category("Graphics")]
        public int Tint;// { get; set; }

        /// <summary>
        /// background color (argb)
        /// </summary>
        [IntColor]
        [Category("Graphics")]
        public int BackColor;// { get; set; }

        /// <summary>
        /// opacity (0-1)
        /// </summary>
        [ValueRange(0, 1)]
        [Category("Graphics")]
        public float Opacity;// { get; set; }

        /// <summary>
        /// name of the texture to draw
        /// </summary>
        [Texture]
        [Category("Graphics")]
        public string Texture;// { get; set; }

        /// <summary>
        /// source rectangle of the texture expressed in pixels
        /// </summary>
        [Category("Graphics")]
        public Rectangle TextureRect;// { get; set; }

        /// <summary>
        /// describes the 9sclice texture regions expressed as margin
        /// </summary>
        [Category("Graphics")]
        public Margin Grid;// { get; set; }

        /// <summary>
        /// enables/disables the 9sclice grid
        /// </summary>
        [Category("Graphics")]
        public TextureMode Tiling;// { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Style"/> class.
        /// </summary>
        public Style()
        {
            Font = "default";
            Tint = -1;
            Opacity = 1;
            BackColor = 0;
            TextColor = -1;
            TextPadding = new Margin(0);
            TextAlign = Alignment.MiddleLeft;
            Texture = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Style"/> class.
        /// </summary>
        /// <param name="style">The style.</param>
        public Style(Style style)
        {
            Font = style.Font;
            Tint = style.Tint;
            Opacity = style.Opacity;
            BackColor = style.BackColor;
            TextColor = style.TextColor;
            TextPadding = style.TextPadding;
            TextAlign = style.TextAlign;
            Texture = style.Texture;
            TextureRect = style.TextureRect;
            Tiling = style.Tiling;
            Grid = style.Grid;
        }

        /// <summary>
        /// Copies this instance.
        /// </summary>
        /// <returns>Style.</returns>
        public Style Copy()
        {
            return (Style)MemberwiseClone();
        }

        /// <summary>
        /// Pastes the specified style.
        /// </summary>
        /// <param name="style">The style.</param>
        public void Paste(Style style)
        {
            Font = style.Font;
            Tint = style.Tint;
            Opacity = style.Opacity;
            BackColor = style.BackColor;
            TextColor = style.TextColor;
            TextPadding = style.TextPadding;
            TextAlign = style.TextAlign;
            Texture = style.Texture;
            TextureRect = style.TextureRect;
            Tiling = style.Tiling;
            Grid = style.Grid;
        }

        public bool IsTextureDifferent(Style other)
        {
            return Texture != other.Texture
                    || !TextureRect.Equals(other.TextureRect)
                    || Tiling != other.Tiling
                    || Tint != other.Tint 
                    || BackColor != other.BackColor;
        }

        public bool IsFontDifferent(Style other)
        {
            return Font != other.Font || TextPadding != other.TextPadding || TextColor != other.TextColor || TextAlign != other.TextAlign;
        }

    }
}
