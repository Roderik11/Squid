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
    [TypeConverter(typeof(RectangleConverter))]
    public struct Rectangle
    {
        private int _left;
        private int _right;
        private int _top;
        private int _bottom;

        /// <summary>
        /// Gets or sets the left edge.
        /// </summary>
        /// <value>The left.</value>
        public int Left { get { return _left; } set { _left = value; } }

        /// <summary>
        /// Gets or sets the top edge.
        /// </summary>
        /// <value>The top.</value>
        public int Top { get { return _top; } set { _top = value; } }

        /// <summary>
        /// Gets or sets the right edge.
        /// </summary>
        /// <value>The right.</value>
        public int Right { get { return _right; } set { _right = value; } }

        /// <summary>
        /// Gets or sets the bottom edge.
        /// </summary>
        /// <value>The bottom.</value>
        public int Bottom { get { return _bottom; } set { _bottom = value; } }

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
            _left = x;
            _top = y;
            _right = x + width;
            _bottom = y + height;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> struct.
        /// </summary>
        /// <param name="pos">The pos.</param>
        /// <param name="size">The size.</param>
        public Rectangle(Point pos, Point size)
        {
            _left = pos.x;
            _top = pos.y;

            _right = pos.x + size.x;
            _bottom = pos.y + size.y;
        }

        /// <summary>
        /// Intersects the specified rect.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public bool Intersects(Rectangle rect)
        {
            return ((((rect.Left < (Left + Width)) && (Left < (rect.Left + rect.Width))) && (rect.Top < (Top + Height))) && (Top < (rect.Top + rect.Height)));
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
        public bool IsEmpty()
        {
            return Left == Right && Top == Bottom;
        }
    }

    /// <summary>
    /// Converts from String to Rectangle and vice versa.
    /// </summary>
    public class RectangleConverter : TypeConverter
    {
        // Methods
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type" /> that represents the type you want to convert from.</param>
        /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
        }

        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="T:System.Type" /> that represents the type you want to convert to.</param>
        /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return ((destinationType == typeof(InstanceDescriptor)) || base.CanConvertTo(context, destinationType));
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo" /> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert.</param>
        /// <returns>An <see cref="T:System.Object" /> that represents the converted value.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string str = value as string;
            if (str == null)
                return base.ConvertFrom(context, culture, value);

            try
            {
                str = str.Trim();

                if (str.Length == 0)
                    return null;

                if (culture == null)
                    culture = CultureInfo.CurrentCulture;

                string[] strArray = str.Split(new char[] { ';', ':' });
                int[] numArray = new int[strArray.Length];

                TypeConverter converter = TypeDescriptor.GetConverter(typeof(int));
                for (int i = 0; i < numArray.Length; i++)
                    numArray[i] = (int)converter.ConvertFromString(context, culture, strArray[i]);

                return new Rectangle(numArray[0], numArray[1], numArray[2], numArray[3]);
            }
            catch
            {
                return new Rectangle();
            }
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" />. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert.</param>
        /// <param name="destinationType">The <see cref="T:System.Type" /> to convert the <paramref name="value" /> parameter to.</param>
        /// <returns>An <see cref="T:System.Object" /> that represents the converted value.</returns>
        /// <exception cref="System.ArgumentNullException">destinationType</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            if (value is Rectangle)
            {
                Rectangle rect = (Rectangle)value;

                if (destinationType == typeof(string))
                {
                    if (culture == null)
                        culture = CultureInfo.CurrentCulture;

                    string separator = "; ";
                    TypeConverter converter = TypeDescriptor.GetConverter(typeof(int));
                    string[] strArray = new string[4];

                    int num = 0;
                    strArray[num++] = converter.ConvertToString(context, culture, rect.Left);
                    strArray[num++] = converter.ConvertToString(context, culture, rect.Top);
                    strArray[num++] = converter.ConvertToString(context, culture, rect.Width);
                    strArray[num++] = converter.ConvertToString(context, culture, rect.Height);

                    return string.Join(separator, strArray);
                }
                else if (destinationType == typeof(InstanceDescriptor))
                {
                    return new InstanceDescriptor(typeof(Margin).GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) }), new object[] { rect.Left, rect.Top, rect.Width, rect.Height });
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        /// Creates an instance of the type that this <see cref="T:System.ComponentModel.TypeConverter" /> is associated with, using the specified context, given a set of property values for the object.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="propertyValues">An <see cref="T:System.Collections.IDictionary" /> of new property values.</param>
        /// <returns>An <see cref="T:System.Object" /> representing the given <see cref="T:System.Collections.IDictionary" />, or null if the object cannot be created. This method always returns null.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// context
        /// or
        /// propertyValues
        /// </exception>
        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (propertyValues == null)
                throw new ArgumentNullException("propertyValues");

            Rectangle padding = (Rectangle)context.PropertyDescriptor.GetValue(context.Instance);

            return new Margin((int)propertyValues["Left"], (int)propertyValues["Top"], (int)propertyValues["Width"], (int)propertyValues["Height"]);
        }

        /// <summary>
        /// Returns whether changing a value on this object requires a call to <see cref="M:System.ComponentModel.TypeConverter.CreateInstance(System.Collections.IDictionary)" /> to create a new value, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <returns>true if changing a property on this object requires a call to <see cref="M:System.ComponentModel.TypeConverter.CreateInstance(System.Collections.IDictionary)" /> to create a new value; otherwise, false.</returns>
        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Returns a collection of properties for the type of array specified by the value parameter, using the specified context and attributes.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="value">An <see cref="T:System.Object" /> that specifies the type of array for which to get properties.</param>
        /// <param name="attributes">An array of type <see cref="T:System.Attribute" /> that is used as a filter.</param>
        /// <returns>A <see cref="T:System.ComponentModel.PropertyDescriptorCollection" /> with the properties that are exposed for this data type, or null if there are no properties.</returns>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(typeof(Rectangle), attributes).Sort(new string[] { "Left", "Top", "Width", "Height" });
        }

        /// <summary>
        /// Returns whether this object supports properties, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <returns>true if <see cref="M:System.ComponentModel.TypeConverter.GetProperties(System.Object)" /> should be called to find the properties of this object; otherwise, false.</returns>
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
