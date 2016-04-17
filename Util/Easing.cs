using System;
using System.Collections.Generic;
using System.Text;

namespace Squid
{
    /// <summary>
    /// Utility class used to ease values
    /// </summary>
    public static class Easing
    {
        public static float EaseIn(float t, float b, float c, float d)
        {
            return c * (t /= d) * t + b;
        }

        public static float EaseOut(float t, float b, float c, float d)
        {
            return -c * (t /= d) * (t - 2) + b;
        }

        public static float EaseInOut(float t, float b, float c, float d)
        {
            if ((t /= d * 0.5f) < 1)
                return c * 0.5f * t * t + b;

            return -c * 0.5f * ((--t) * (t - 2) - 1) + b;
        }

        public static Point EaseInOut(float t, Point b, Point c, float d)
        {
            if ((t /= d / 2) < 1) return c / 2 * t * t + b;
            return (c * -1) / 2 * ((--t) * (t - 2) - 1) + b;
        }
    }
}
