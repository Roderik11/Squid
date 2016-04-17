using System;
using System.ComponentModel;

namespace Squid
{
    /// <summary>
    /// A Button
    /// </summary>
    [Toolbox]
    public class Button : Label, ICheckable
    {
        private bool _checked;

        /// <summary>
        /// Gets or sets a value indicating whether Checked changes on MouseClick.
        /// </summary>
        /// <value><c>true</c> if [check on click]; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        public bool CheckOnClick { get; set; }

        /// <summary>
        /// Raised when Checked changed].
        /// </summary>
        public event VoidEvent CheckedChanged;

        /// <summary>
        /// Raised before Checked
        /// </summary>
        public event EventWithArgs BeforeCheckedChanged;

       
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Button"/> is checked.
        /// </summary>
        /// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        public bool Checked
        {
            get { return _checked; }
            set
            {
                if (value == _checked) return;

                if (BeforeCheckedChanged != null)
                {
                    SquidEventArgs args = new SquidEventArgs();
                    BeforeCheckedChanged(this, args);
                    if (args.Cancel) return;
                }

                _checked = value;

                if (CheckedChanged != null)
                    CheckedChanged(this);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Button"/> class.
        /// </summary>
        public Button()
        {
            AutoEllipsis = false;
            Style = "button";
            MouseClick += Button_MouseClick;
        }

        void Button_MouseClick(Control sender, MouseEventArgs args)
        {
            if (args.Button > 0) return;

            if (CheckOnClick)
                Checked = !Checked;
        }
    }
}
