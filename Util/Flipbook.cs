using System;
using System.Collections.Generic;
using System.Text;

namespace Squid
{
    /// <summary>
    /// Helper class to manage a sprite sheet animation.
    /// </summary>
    public sealed class Flipbook
    {
        public int Rows = 1;
        public int Columns = 1;
        public float Speed = 60;

        private float Timer;
        private int col = 0;
        private int row = 0;

        private Rectangle rect = new Rectangle();

        public void Draw(int texture, int x, int y, int width, int height, int color)
        {
            if (texture < 0) return;

            Timer += Gui.TimeElapsed;
            
            if (Timer >= Speed)
            {
                Timer = 0;
                Advance();
            }

            Point size = Gui.Renderer.GetTextureSize(texture);
            
            int w = (int)((float)size.x / (float)Columns);
            int h = (int)((float)size.y / (float)Rows);

            rect.Left = w * col;
            rect.Right = (w * (col + 1));

            rect.Top = h * row;
            rect.Bottom = (h * (row + 1));

            Gui.Renderer.DrawTexture(texture, x, y, width, height, rect, color);
        }

        private void Advance()
        {
            if (col < Columns)
            {
                col++;

                if (col >= Columns)
                {
                    col = 0;

                    if (row < Rows)
                    {
                        row++;

                        if (row >= Rows)
                            row = 0;
                    }
                }
            }
        }
    }
}
