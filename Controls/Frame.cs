using System;
using System.Collections.Generic;
using System.Text;

namespace Squid
{
    /// <summary>
    /// A Control container
    /// </summary>
    [Toolbox]
    public class Frame : Control, IControlContainer
    {
        public ControlCollection Controls { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Frame"/> class.
        /// </summary>
        public Frame()
        {
            NoEvents = true;
        }
    }
}
