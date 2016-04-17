using System;
using System.Collections.Generic;
using System.Text;

namespace Squid
{
    /// <summary>
    /// Used to indicate a multiline string to external editors
    /// </summary>
    public class MultilineAttribute : Attribute { }

    /// <summary>
    /// Used to indicate whether this object/member should be visible in external editors
    /// </summary>
    public class ToolboxAttribute : Attribute
    {
        public bool Visible { get; private set; }
        public string Category { get; private set; }
        public ToolboxAttribute() { Visible = true; }
        public ToolboxAttribute(bool visible) { Visible = visible; }
        public ToolboxAttribute(string category) { Visible = true; Category = category; }
    }

    /// <summary>
    /// Used to indicate whether this object/member should be visible in external editors
    /// </summary>
    public class HiddenAttribute : Attribute { }

    /// <summary>
    ///  Used to indicate member that is used as a color to external editors
    /// </summary>
    public class IntColorAttribute : Attribute { }

    /// <summary>
    /// Used to indicate a member that is used as a texture to external editors
    /// </summary>
    public class TextureAttribute : Attribute { }

    /// <summary>
    /// Used to indicate a member that is used as a font to external editors
    /// </summary>
    public class FontAttribute : Attribute { }

    /// <summary>
    /// Used to indicate a member that is used as a Style to external editor
    /// </summary>
    public class StyleAttribute : Attribute { }

    /// <summary>
    /// Used to indicate a member that is used as a Cursor to external editor
    /// </summary>
    public class CursorAttribute : Attribute { }

    /// <summary>
    /// Used to indicate a value range to external editors
    /// </summary>
    public class ValueRangeAttribute : Attribute
    {
        public float Min { get; private set; }
        public float Max { get; private set; }

        public ValueRangeAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }
}
