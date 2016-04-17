using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Squid
{
    /// <summary>
    /// Interface ICheckable
    /// </summary>
    public interface ICheckable
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ICheckable"/> is checked.
        /// </summary>
        /// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        bool Checked { get; set; }
    }
}
