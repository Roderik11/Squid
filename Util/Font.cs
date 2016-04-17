using System;
using System.Collections.Generic;
using System.Text;

namespace Squid
{
    /// <summary>
    /// Helper class to represent a font.
    /// This class will eventually be obsolete. Do not use.
    /// </summary>
    [Obsolete]
    public class Font
    {
        public static readonly string Default = "default";

        public string Name { get; set; }
        public string Family { get; set; }
        
        public bool Bold { get; set; }
        public bool Italic { get; set; }
        public bool Underlined { get; set; }
        public bool International { get; set; }

        public int Size { get; set; }
    }
}
