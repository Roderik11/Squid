using System;
using System.Collections.Generic;
using System.Text;

namespace Squid
{
    /// <summary>
    /// A CheckBox
    /// </summary>
    [Toolbox]
    public class CheckBox : Control, ICheckable, IText
    {
        /// <summary>
        /// Gets the button.
        /// </summary>
        /// <value>The button.</value>
        public Button Button { get; private set; }
        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <value>The label.</value>
        public Label Label { get; private set; }

        /// <summary>
        /// Raised when [checked changed].
        /// </summary>
        public event VoidEvent CheckedChanged;

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get => Label.Text;
            set => Label.Text = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CheckBox"/> is checked.
        /// </summary>
        /// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
        public bool Checked
        {
            get => Button.Checked;
            set => Button.Checked = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckBox"/> class.
        /// </summary>
        public CheckBox()
        {
            Size = new Point(100, 30);
            Style = "checkbox";

            MouseClick += CheckBox_MouseClick;

            Button = new Button
            {
                Dock = DockStyle.Left,
                Size = new Point(29, 30),
                CheckOnClick = true,
                Style = "checkboxButton",
                NoEvents = true
            };
            Button.CheckedChanged += Button_CheckedChanged;
            Elements.Add(Button);

            Label = new Label
            {
                Dock = DockStyle.Fill,
                Style = "checkboxLabel",
                NoEvents = true
            };
            Elements.Add(Label);
        }

        void CheckBox_MouseClick(Control sender, MouseEventArgs args)
        {
            Button.Click(args.Button);
        }

        void Button_CheckedChanged(Control sender)
        {
            CheckedChanged?.Invoke(this);
        }
     
        protected override void OnStateChanged()
        {
            base.OnStateChanged();

            Button.State = State;
            Label.State = State;
        }
    }
}
