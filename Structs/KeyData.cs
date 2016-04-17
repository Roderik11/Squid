using System;
using System.Collections.Generic;
using System.Text;

namespace Squid
{
    /// <summary>
    /// Struct KeyData
    /// </summary>
    public struct KeyData
    {
        /// <summary>
        /// The scancode
        /// </summary>
        public int Scancode;

        /// <summary>
        /// The character
        /// </summary>
        public char? Char;

        /// <summary>
        /// The pressed
        /// </summary>
        public bool Pressed;
        /// <summary>
        /// The released
        /// </summary>
        public bool Released;

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>The key.</value>
        public Keys Key { get { return (Keys)Scancode; } }
    }
}
