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
    [TypeConverter(typeof(MarginConverter))]
    public struct Margin
    {
        private bool _all;
        private int _top;
        private int _left;
        private int _right;
        private int _bottom;
        public static readonly Margin Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="Margin"/> struct.
        /// </summary>
        /// <param name="all">All.</param>
        public Margin(int all)
        {
            _all = true;
            _top = _left = _right = _bottom = all;
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
            _top = top;
            _left = left;
            _right = right;
            _bottom = bottom;
            _all = ((_top == _left) && (_top == _right)) && (_top == _bottom);
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
                if (!_all)
                    return -1;

                return _top;
            }
            set
            {
                if (!_all || (_top != value))
                {
                    _all = true;
                    _top = _left = _right = _bottom = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the bottom.
        /// </summary>
        /// <value>The bottom.</value>
        [RefreshProperties(RefreshProperties.All)]
        public int Bottom
        {
            get
            {
                if (_all)
                    return _top;

                return _bottom;
            }
            set
            {
                if (_all || (_bottom != value))
                {
                    _all = false;
                    _bottom = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the left.
        /// </summary>
        /// <value>The left.</value>
        [RefreshProperties(RefreshProperties.All)]
        public int Left
        {
            get
            {
                if (_all)
                    return _top;

                return _left;
            }
            set
            {
                if (_all || (_left != value))
                {
                    _all = false;
                    _left = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the right.
        /// </summary>
        /// <value>The right.</value>
        [RefreshProperties(RefreshProperties.All)]
        public int Right
        {
            get
            {
                if (_all)
                    return _top;

                return _right;
            }
            set
            {
                if (_all || (_right != value))
                {
                    _all = false;
                    _right = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the top.
        /// </summary>
        /// <value>The top.</value>
        [RefreshProperties(RefreshProperties.All)]
        public int Top
        {
            get
            {
                return _top;
            }
            set
            {
                if (_all || (_top != value))
                {
                    _all = false;
                    _top = value;
                }
            }
        }

        /// <summary>
        /// Gets the horizontal.
        /// </summary>
        /// <value>The horizontal.</value>
        [Browsable(false)]
        public int Horizontal
        {
            get
            {
                return (Left + Right);
            }
        }

        /// <summary>
        /// Gets the vertical.
        /// </summary>
        /// <value>The vertical.</value>
        [Browsable(false)]
        public int Vertical
        {
            get
            {
                return (Top + Bottom);
            }
        }
        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <value>The size.</value>
        [Browsable(false)]
        public Point Size
        {
            get
            {
                return new Point(Horizontal, Vertical);
            }
        }

        /// <summary>
        /// Adds the specified p1.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <returns>Margin.</returns>
        public static Margin Add(Margin p1, Margin p2)
        {
            return p1 + p2;
        }

        /// <summary>
        /// Subtracts the specified p1.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <returns>Margin.</returns>
        public static Margin Subtract(Margin p1, Margin p2)
        {
            return p1 - p2;
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
        /// Implements the +.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <returns>The result of the operator.</returns>
        public static Margin operator +(Margin p1, Margin p2)
        {
            return new Margin(p1.Left + p2.Left, p1.Top + p2.Top, p1.Right + p2.Right, p1.Bottom + p2.Bottom);
        }

        /// <summary>
        /// Implements the -.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <returns>The result of the operator.</returns>
        public static Margin operator -(Margin p1, Margin p2)
        {
            return new Margin(p1.Left - p2.Left, p1.Top - p2.Top, p1.Right - p2.Right, p1.Bottom - p2.Bottom);
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

        internal bool ShouldSerializeAll()
        {
            return _all;
        }

        static Margin()
        {
            Empty = new Margin(0);
        }
    }

    /// <summary>
    /// Converts from String to Margin and vice versa.
    /// </summary>
    public class MarginConverter : TypeConverter
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

            return new Margin(numArray[0], numArray[1], numArray[2], numArray[3]);
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
            if (value is Margin)
            {
                if (destinationType == typeof(string))
                {
                    Margin padding = (Margin)value;
                    if (culture == null)
                        culture = CultureInfo.CurrentCulture;

                    string separator = "; ";
                    TypeConverter converter = TypeDescriptor.GetConverter(typeof(int));
                    string[] strArray = new string[4];

                    int num = 0;
                    strArray[num++] = converter.ConvertToString(context, culture, padding.Left);
                    strArray[num++] = converter.ConvertToString(context, culture, padding.Top);
                    strArray[num++] = converter.ConvertToString(context, culture, padding.Right);
                    strArray[num++] = converter.ConvertToString(context, culture, padding.Bottom);

                    return string.Join(separator, strArray);
                }

                if (destinationType == typeof(InstanceDescriptor))
                {
                    Margin padding2 = (Margin)value;
                    if (padding2.ShouldSerializeAll())
                        return new InstanceDescriptor(typeof(Margin).GetConstructor(new Type[] { typeof(int) }), new object[] { padding2.All });

                    return new InstanceDescriptor(typeof(Margin).GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) }), new object[] { padding2.Left, padding2.Top, padding2.Right, padding2.Bottom });
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

            Margin padding = (Margin)context.PropertyDescriptor.GetValue(context.Instance);

            int all = (int)propertyValues["All"];

            if (padding.All != all)
                return new Margin(all);

            return new Margin((int)propertyValues["Left"], (int)propertyValues["Top"], (int)propertyValues["Right"], (int)propertyValues["Bottom"]);
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
            return TypeDescriptor.GetProperties(typeof(Margin), attributes).Sort(new string[] { "All", "Left", "Top", "Right", "Bottom" });
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
