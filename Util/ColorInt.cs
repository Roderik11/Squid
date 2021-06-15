using System;
using System.Collections.Generic;
using System.Text;

namespace Squid
{
    /// <summary>
    /// Utility class used to convert colors
    /// </summary>
    public static class ColorInt
    {
        /// <summary>Blends the specified colors together.</summary>
        /// <param name="color1">Color to blend onto the background color.</param>
        /// <param name="color2">Color to blend the other color onto.</param>
        /// <returns>The blended colors.</returns>
        public static int Blend(int color1, int color2)
        {
            int red1, green1, blue1, alpha1;
            int red2, green2, blue2, alpha2;

            FromArgb(color1, out alpha1, out red1, out green1, out blue1);
            FromArgb(color2, out alpha2, out red2, out green2, out blue2);

            double amount = (double)alpha1 / 255d;

            byte r = (byte)((red1 * amount) + red2 * (1 - amount));
            byte g = (byte)((green1 * amount) + green2 * (1 - amount));
            byte b = (byte)((blue1 * amount) + blue2 * (1 - amount));
            byte a = (byte)alpha2;// (byte)((alpha1 * amount) + alpha2 * (1 - amount));

            return ARGB((byte)a, r, g, b);
        }

        /// <summary>
        /// Overlays the specified colors.
        /// </summary>
        /// <param name="color1">The color1.</param>
        /// <param name="color2">The color2.</param>
        /// <returns>System.Int32.</returns>
        public static int Overlay(int color1, int color2)
        {
            int red1, green1, blue1, alpha1;
            int red2, green2, blue2, alpha2;

            FromArgb(color1, out alpha1, out red1, out green1, out blue1);
            FromArgb(color2, out alpha2, out red2, out green2, out blue2);

            byte r = red1 > 127 ? (byte)(2 * red1 * red2 / 255) : (byte)(255 - 2 * (255 - red1) * (255 - red2));
            byte g = red1 > 127 ? (byte)(2 * green1 * green2 / 255) : (byte)(255 - 2 * (255 - green1) * (255 - green2));
            byte b = red1 > 127 ? (byte)(2 * blue1 * blue2 / 255) : (byte)(255 - 2 * (255 - blue1) * (255 - blue2));
            byte a = red1 > 127 ? (byte)(2 * alpha1 * alpha2 / 255) : (byte)(255 - 2 * (255 - alpha1) * (255 - alpha2));

            return ARGB(a, r, g, b);
        }

        /// <summary>
        /// Multiplies the specified colors.
        /// </summary>
        /// <param name="color1">The color1.</param>
        /// <param name="color2">The color2.</param>
        /// <returns>System.Int32.</returns>
        public static int Multiply(int color1, int color2)
        {
            int red1, green1, blue1, alpha1;
            int red2, green2, blue2, alpha2;

            FromArgb(color1, out alpha1, out red1, out green1, out blue1);
            FromArgb(color2, out alpha2, out red2, out green2, out blue2);

            byte r = (byte)(red1 * red2 / 255);
            byte g = (byte)(green1 * green2 / 255);
            byte b = (byte)(blue1 * blue2 / 255);
            byte a = (byte)(alpha1 * alpha2 / 255);

            return ARGB(a, r, g, b);
        }

        /// <summary>
        /// Multiplies the specified colors.
        /// </summary>
        /// <param name="color1">The color1.</param>
        /// <param name="color2">The color2.</param>
        /// <returns>System.Int32.</returns>
        public static int Screen(int color1, int color2)
        {
            int red1, green1, blue1, alpha1;
            int red2, green2, blue2, alpha2;

            FromArgb(color1, out alpha1, out red1, out green1, out blue1);
            FromArgb(color2, out alpha2, out red2, out green2, out blue2);

            byte r = (byte)(255 - (255 - red1) * (255 - red2));
            byte g = (byte)(255 - (255 - green1) * (255 - green2));
            byte b = (byte)(255 - (255 - blue1) * (255 - blue2));
            byte a = (byte)(255 - (255 - alpha1) * (255 - alpha2));

            return ARGB(a, r, g, b);
        }

        /// <summary>
        /// Returns the components of the specified ARGB color
        /// </summary>
        /// <param name="argb">The argb.</param>
        /// <param name="red">The red.</param>
        /// <param name="green">The green.</param>
        /// <param name="blue">The blue.</param>
        /// <param name="alpha">The alpha.</param>
        public static void FromArgb(int argb, out int a, out int r, out int g, out int b)
        {
            a = (int)((argb >> 0x18) & 0xffL);
            r = (int)((argb >> 0x10) & 0xffL);
            g = (int)((argb >> 8) & 0xffL);
            b = (int)(argb & 0xffL);
        }

        /// <summary>
        /// Returns the specified ARGB color with the specified opacity.
        /// </summary>
        /// <param name="opacity">The opacity.</param>
        /// <param name="argb">The ARGB.</param>
        /// <returns>System.Int32.</returns>
        public static int FromArgb(float opacity, int argb)
        {
            int alpha = (int)((argb >> 0x18) & 0xffL);
            int red = (int)((argb >> 0x10) & 0xffL);
            int green = (int)((argb >> 8) & 0xffL);
            int blue = (int)(argb & 0xffL);

            int newalpha = (int)((float)alpha * opacity);

            return ARGB(newalpha, red, green, blue);
        }

        /// <summary>
        /// Returns the color expressed as integer
        /// </summary>
        /// <param name="r">red 0-1</param>
        /// <param name="g">green 0-1</param>
        /// <param name="b">blue 0-1</param>
        /// <param name="a">alpha 0-1</param>
        /// <returns></returns>
        [Obsolete("Please use ARGB instead. This method returns ARGB.")]
        public static int RGBA(float r, float g, float b, float a)
        {
            return ARGB(a, r, g, b);
        }

        /// <summary>
        /// Returns the color expressed as integer
        /// </summary>
        /// <param name="a">alpha</param>
        /// <param name="r">red</param>
        /// <param name="g">green</param>
        /// <param name="b">blue</param>
        /// <returns></returns>
        public static int ARGB(int a, int r, int g, int b)
        {
            return ((a & 0xff) << 24) | ((r & 0xff) << 16) | ((g & 0xff) << 8) | b;
        }

        /// <summary>
        /// Returns the color expressed as integer
        /// </summary>
        /// <param name="r">red</param>
        /// <param name="g">green</param>
        /// <param name="b">blue</param>
        /// <returns></returns>
        public static int RGB(int r, int g, int b)
        {
            return (0xff << 24) | ((r & 0xff) << 16) | ((g & 0xff) << 8) | b;
        }

        /// <summary>
        /// Returns the color expressed as integer
        /// </summary>
        /// <param name="a">alpha</param>
        /// <param name="r">red</param>
        /// <param name="g">green</param>
        /// <param name="b">blue</param>
        /// <returns></returns>
        public static int ARGB(float a, float r, float g, float b)
        {
            return ARGB((int)(a * 0xff), (int)(b * 0xff), (int)(g * 0xff), (int)(r * 0xff));
        }
    }
}
