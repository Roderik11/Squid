using System;
using System.Collections.Generic;
using System.Text;

namespace Squid
{
    /// <summary>
    /// Interface IControlContainer
    /// </summary>
    public interface IControlContainer
    {
        /// <summary>
        /// Gets or sets the controls.
        /// </summary>
        /// <value>The controls.</value>
        [Hidden]
        ControlCollection Controls { get; set; }
    }
}
