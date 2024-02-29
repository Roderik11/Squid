using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Squid
{
    /// <summary>
    /// A control that show a texture
    /// </summary>
    [Toolbox]
    public class ImageControl : Control
    {
        private string _texture;

        /// <summary>
        /// Gets or sets the texture.
        /// </summary>
        /// <value>The texture.</value>
        [Texture, Category("Image")]
        public string Texture
        {
            get { return _texture; }
            set { _texture = value; TextureRect = new Rectangle(0, 0, 0, 0); }
        }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        [IntColor, Category("Image")]
        public int Color { get; set; }

        /// <summary>
        /// Gets or sets the texture rect.
        /// </summary>
        /// <value>The texture rect.</value>
        [Category("Image")]
        public Rectangle TextureRect { get; set; }

        /// <summary>
        /// Gets or sets the texture tiling
        /// </summary>
        [Category("Image")]
        public TextureMode Tiling { get; set; }

        /// <summary>
        /// Gets or sets the slice9 grid
        /// </summary>
        [Category("Image")]
        public Margin Grid { get; set; }

        public bool ExcludeFromAtlas = false;

        [Category("Image")]
        public Margin Inset { get; set; }

        public bool ColorByTint { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageControl"/> class.
        /// </summary>
        public ImageControl()
        {
            Color = -1;
            Inset = new Margin();
        }

        protected override void DrawStyle(Style style, float opacity)
        {
            base.DrawStyle(style, opacity);

            if (string.IsNullOrEmpty(Texture)) return;
            int texture = Gui.Renderer.GetTexture(Texture);
            if (texture < 0) return;

            if(ColorByTint)
                Color = style.Tint;

            int color = Color;

            if(Tint != -1)
                color = ColorInt.Blend(Tint, color);

            color = ColorInt.FromArgb(opacity, color);

            if (TextureRect.IsEmpty)
            {
                Point texsize = Gui.Renderer.GetTextureSize(texture);
                TextureRect = new Rectangle(Point.Zero, texsize);
            }

            //bool atlas = SpriteBatch.AutoAtlas;

            //if (ExcludeFromAtlas)
            //    SpriteBatch.AutoAtlas = false;

            if (Tiling == TextureMode.Grid || Tiling == TextureMode.GridRepeat)
            {
                SliceTexture(texture, Tiling, TextureRect, Grid, color);
            }
            else if (Tiling == TextureMode.Stretch)
            {
                Gui.Renderer.DrawTexture(texture, Location.x + Inset.Left, Location.y + Inset.Top, Size.x - (Inset.Left + Inset.Right), Size.y - (Inset.Top + Inset.Bottom), TextureRect, color);
            }
            else if (Tiling == TextureMode.Center)
            {
                Point center = Location + Size / 2;
                Point rectsize = new Point(TextureRect.Width, TextureRect.Height);
                Point pos = center - rectsize / 2;

                Gui.Renderer.DrawTexture(texture, pos.x, pos.y, rectsize.x, rectsize.y, TextureRect, color);
            }
            else if (Tiling == TextureMode.StretchAspect)
            {
                Point center = Location + Size / 2;
                Point rectsize = new Point(TextureRect.Width, TextureRect.Height);


                float ratio = (float)rectsize.x / rectsize.y;

                float h = Size.y;
                float w = h * ratio;

                if (w > Size.x)
                {
                    w = Size.x;
                    h = w / ratio;
                }

                rectsize = new Point((int)w, (int)h);
                Point pos = center - rectsize / 2;

                Gui.Renderer.DrawTexture(texture, pos.x, pos.y, rectsize.x, rectsize.y, TextureRect, color);
            }
            else
            {
                RepeatTexture(texture, Location, TextureRect, Tiling, color);
            }

            //if (ExcludeFromAtlas)
            //    SpriteBatch.AutoAtlas = atlas;
        }
    }
}
