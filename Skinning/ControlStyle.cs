using System;
using System.Collections.Generic;
using System.Text;
using Squid.Xml;

namespace Squid
{
    /// <summary>
    /// A ControlStyle. This is a set of Styles.
    /// There is one Style per ControlState.
    /// </summary>
    public sealed class ControlStyle
    {
        /// <summary>
        /// Gets or sets the styles.
        /// </summary>
        /// <value>The styles.</value>
        [Hidden]
        public StyleCollection Styles { get; set; }

        /// <summary>
        /// user data
        /// </summary>
        /// <value>The tag.</value>
        [Hidden, XmlIgnore]
        public object Tag{ get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlStyle"/> class.
        /// </summary>
        public ControlStyle()
        {
            Styles = new StyleCollection();

            foreach (ControlState state in Enum.GetValues(typeof(ControlState)))
                Styles.Add(state, new Style());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlStyle"/> class.
        /// </summary>
        /// <param name="style">The style.</param>
        public ControlStyle(ControlStyle style)
        {
            Styles = new StyleCollection();

            foreach (ControlState state in Enum.GetValues(typeof(ControlState)))
                Styles.Add(state, new Style(style.Styles[state]));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlStyle"/> class.
        /// </summary>
        /// <param name="style">The style.</param>
        public ControlStyle(Style style)
        {
            Styles = new StyleCollection();

            foreach (ControlState state in Enum.GetValues(typeof(ControlState)))
                Styles.Add(state, new Style(style));
        }

        /// <summary>
        /// Copies this instance.
        /// </summary>
        /// <returns>ControlStyle.</returns>
        public ControlStyle Copy()
        {
            return new ControlStyle(this);
        }

        /// <summary>
        /// Pastes the specified style.
        /// </summary>
        /// <param name="style">The style.</param>
        public void Paste(ControlStyle style)
        {
            Styles = new StyleCollection();

            foreach (ControlState state in style.Styles.Keys)
                Styles.Add(state, new Style(style.Styles[state]));
        }

        /// <summary>
        /// Pastes the specified style.
        /// </summary>
        /// <param name="style">The style.</param>
        public void Paste(Style style)
        {
            Styles = new StyleCollection();

            foreach (ControlState state in Enum.GetValues(typeof(ControlState)))
                Styles.Add(state, new Style(style));
        }

        /// <summary>
        /// Gets or sets the default.
        /// </summary>
        /// <value>The default.</value>
        [XmlIgnore]
        public Style Default
        {
            get { return Styles[ControlState.Default]; }
            set { Styles[ControlState.Default] = value; }
        }

        /// <summary>
        /// Gets or sets the hot.
        /// </summary>
        /// <value>The hot.</value>
        [XmlIgnore]
        public Style Hot
        {
            get { return Styles[ControlState.Hot]; }
            set { Styles[ControlState.Hot] = value; }
        }

        /// <summary>
        /// Gets or sets the pressed.
        /// </summary>
        /// <value>The pressed.</value>
        [XmlIgnore]
        public Style Pressed
        {
            get { return Styles[ControlState.Pressed]; }
            set { Styles[ControlState.Pressed] = value; }
        }

        /// <summary>
        /// Gets or sets the disabled.
        /// </summary>
        /// <value>The disabled.</value>
        [XmlIgnore]
        public Style Disabled
        {
            get { return Styles[ControlState.Disabled]; }
            set { Styles[ControlState.Disabled] = value; }
        }

        /// <summary>
        /// Gets or sets the focused.
        /// </summary>
        /// <value>The focused.</value>
        [XmlIgnore]
        public Style Focused
        {
            get { return Styles[ControlState.Focused]; }
            set { Styles[ControlState.Focused] = value; }
        }

        /// <summary>
        /// Gets or sets the checked.
        /// </summary>
        /// <value>The checked.</value>
        [XmlIgnore]
        public Style Checked
        {
            get { return Styles[ControlState.Checked]; }
            set { Styles[ControlState.Checked] = value; }
        }

        /// <summary>
        /// Gets or sets the checked hot.
        /// </summary>
        /// <value>The checked hot.</value>
        [XmlIgnore]
        public Style CheckedHot
        {
            get { return Styles[ControlState.CheckedHot]; }
            set { Styles[ControlState.CheckedHot] = value; }
        }

        /// <summary>
        /// Gets or sets the checked pressed.
        /// </summary>
        /// <value>The checked pressed.</value>
        [XmlIgnore]
        public Style CheckedPressed
        {
            get { return Styles[ControlState.CheckedPressed]; }
            set { Styles[ControlState.CheckedPressed] = value; }
        }

        /// <summary>
        /// Gets or sets the checked disabled.
        /// </summary>
        /// <value>The checked disabled.</value>
        [XmlIgnore]
        public Style CheckedDisabled
        {
            get { return Styles[ControlState.CheckedDisabled]; }
            set { Styles[ControlState.CheckedDisabled] = value; }
        }

        /// <summary>
        /// Gets or sets the checked focused.
        /// </summary>
        /// <value>The checked focused.</value>
        [XmlIgnore]
        public Style CheckedFocused
        {
            get { return Styles[ControlState.CheckedFocused]; }
            set { Styles[ControlState.CheckedFocused] = value; }
        }

        /// <summary>
        /// Gets or sets the selected.
        /// </summary>
        /// <value>The selected.</value>
        [XmlIgnore]
        public Style Selected
        {
            get { return Styles[ControlState.Selected]; }
            set { Styles[ControlState.Selected] = value; }
        }

        /// <summary>
        /// Gets or sets the selected hot.
        /// </summary>
        /// <value>The selected hot.</value>
        [XmlIgnore]
        public Style SelectedHot
        {
            get { return Styles[ControlState.SelectedHot]; }
            set { Styles[ControlState.SelectedHot] = value; }
        }

        /// <summary>
        /// Gets or sets the selected pressed.
        /// </summary>
        /// <value>The selected pressed.</value>
        [XmlIgnore]
        public Style SelectedPressed
        {
            get { return Styles[ControlState.SelectedPressed]; }
            set { Styles[ControlState.SelectedPressed] = value; }
        }

        /// <summary>
        /// Gets or sets the selected disabled.
        /// </summary>
        /// <value>The selected disabled.</value>
        [XmlIgnore]
        public Style SelectedDisabled
        {
            get { return Styles[ControlState.SelectedDisabled]; }
            set { Styles[ControlState.SelectedDisabled] = value; }
        }

        /// <summary>
        /// Gets or sets the selected focused.
        /// </summary>
        /// <value>The selected focused.</value>
        [XmlIgnore]
        public Style SelectedFocused
        {
            get { return Styles[ControlState.SelectedFocused]; }
            set { Styles[ControlState.SelectedFocused] = value; }
        }

        /// <summary>
        /// color to tint the texture (argb)
        /// </summary>
        /// <value>The tint.</value>
        [XmlIgnore]
        public int Tint
        {
            set
            {
                foreach (Style state in Styles.Values)
                    state.Tint = value;
            }
        }

        /// <summary>
        /// color for any text to be drawn (argb)
        /// </summary>
        /// <value>The color of the text.</value>
        [XmlIgnore]
        public int TextColor
        {
            set
            {
                foreach (Style state in Styles.Values)
                    state.TextColor = value;
            }
        }

        /// <summary>
        /// background color (argb)
        /// </summary>
        /// <value>The color of the back.</value>
        [XmlIgnore]
        public int BackColor
        {
            set
            {
                foreach (Style state in Styles.Values)
                    state.BackColor = value;
            }
        }

        /// <summary>
        /// opacity (0-1)
        /// </summary>
        /// <value>The opacity.</value>
        [XmlIgnore]
        public float Opacity
        {
            set
            {
                foreach (Style state in Styles.Values)
                    state.Opacity = value;
            }
        }

        /// <summary>
        /// name of the font to use for text
        /// </summary>
        /// <value>The font.</value>
        [XmlIgnore]
        public string Font
        {
            set
            {
                foreach (Style state in Styles.Values)
                    state.Font = value;
            }
        }

        /// <summary>
        /// name of the texture to draw
        /// </summary>
        /// <value>The texture.</value>
        [XmlIgnore]
        public string Texture
        {
            set
            {
                foreach (Style state in Styles.Values)
                    state.Texture = value;
            }
        }

        /// <summary>
        /// texture tiling mode
        /// </summary>
        /// <value>The tiling.</value>
        [XmlIgnore]
        public TextureMode Tiling
        {
            set
            {
                foreach (Style state in Styles.Values)
                    state.Tiling = value;
            }
        }

        /// <summary>
        /// source rectangle of the texture expressed in pixels
        /// </summary>
        /// <value>The texture rect.</value>
        [XmlIgnore]
        public Rectangle TextureRect
        {
            set
            {
                foreach (Style state in Styles.Values)
                    state.TextureRect = value;
            }
        }

        /// <summary>
        /// text padding (distance to control borders)
        /// </summary>
        /// <value>The text padding.</value>
        [XmlIgnore]
        public Margin TextPadding
        {
            set
            {
                foreach (Style state in Styles.Values)
                    state.TextPadding = value;
            }
        }

        /// <summary>
        /// text alignment
        /// </summary>
        /// <value>The text align.</value>
        [XmlIgnore]
        public Alignment TextAlign
        {
            set
            {
                foreach (Style state in Styles.Values)
                    state.TextAlign = value;
            }
        }

        /// <summary>
        /// describes the 9sclice texture regions expressed as margin
        /// </summary>
        /// <value>The grid.</value>
        [XmlIgnore]
        public Margin Grid
        {
            set
            {
                foreach (Style state in Styles.Values)
                    state.Grid = value;
            }
        }
    }
}
