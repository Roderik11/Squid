using System;
using System.Collections.Generic;
using System.Text;

namespace Squid
{
    /// <summary>
    /// Enum ButtonState
    /// </summary>
    public enum ButtonState
    {
        /// <summary>
        /// The none
        /// </summary>
        None,
        /// <summary>
        /// Down
        /// </summary>
        Down,
        /// <summary>
        /// The press
        /// </summary>
        Press,
        /// <summary>
        /// Up
        /// </summary>
        Up
    }

    /// <summary>
    /// Enum TextBoxMode
    /// </summary>
    public enum TextBoxMode
    {
        /// <summary>
        /// The alpha numeric
        /// </summary>
        AlphaNumeric,
        /// <summary>
        /// The numeric
        /// </summary>
        Numeric
    }

    /// <summary>
    /// Enum TextureMode
    /// </summary>
    public enum TextureMode
    {
        /// <summary>
        /// Stretch
        /// </summary>
        Stretch,
        /// <summary>
        /// Use sliced texture grid
        /// </summary>
        Grid,
        /// <summary>
        /// Repeat the edges of the grid
        /// </summary>
        GridRepeat,
        /// <summary>
        /// Repeat the texture
        /// </summary>
        Repeat,
        /// <summary>
        /// Repeat the texture on x
        /// </summary>
        RepeatX,
        /// <summary>
        /// Repeat the texture on y
        /// </summary>
        RepeatY,
        /// <summary>
        /// Center the texture
        /// </summary>
        Center,

        StretchAspect
    }

    /// <summary>
    /// Enum DialogResult
    /// </summary>
    public enum DialogResult
    {
        /// <summary>
        /// The none
        /// </summary>
        None,
        /// <summary>
        /// The OK
        /// </summary>
        OK,
        /// <summary>
        /// The cancel
        /// </summary>
        Cancel,
        /// <summary>
        /// The abort
        /// </summary>
        Abort,
        /// <summary>
        /// The retry
        /// </summary>
        Retry,
        /// <summary>
        /// The ignore
        /// </summary>
        Ignore,
        /// <summary>
        /// The yes
        /// </summary>
        Yes,
        /// <summary>
        /// The no
        /// </summary>
        No
    }

    /// <summary>
    /// Enum FlowDirection
    /// </summary>
    public enum FlowDirection
    {
        None,
        /// <summary>
        /// The left to right
        /// </summary>
        LeftToRight,
        /// <summary>
        /// The right to left
        /// </summary>
        RightToLeft,
        /// <summary>
        /// The top to bottom
        /// </summary>
        TopToBottom,
        /// <summary>
        /// The bottom to top
        /// </summary>
        BottomToTop
    }

    /// <summary>
    /// Enum AutoSize
    /// </summary>
    public enum AutoSize
    {
        /// <summary>
        /// The none
        /// </summary>
        None,
        /// <summary>
        /// The horizontal
        /// </summary>
        Horizontal,
        /// <summary>
        /// The vertical
        /// </summary>
        Vertical,
        /// <summary>
        /// The horizontal vertical
        /// </summary>
        HorizontalVertical
    }

    /// <summary>
    /// Enum DragMode
    /// </summary>
    public enum DragMode
    {
        /// <summary>
        /// The axis X
        /// </summary>
        AxisX,
        /// <summary>
        /// The axis Y
        /// </summary>
        AxisY,
        /// <summary>
        /// The axis XY
        /// </summary>
        AxisXY
    }

    /// <summary>
    /// Enum DockStyle
    /// </summary>
    public enum DockStyle
    {
        /// <summary>
        /// The none
        /// </summary>
        None,
        /// <summary>
        /// The left
        /// </summary>
        Left,
        /// <summary>
        /// The top
        /// </summary>
        Top,
        /// <summary>
        /// The right
        /// </summary>
        Right,
        /// <summary>
        /// The bottom
        /// </summary>
        Bottom,
        /// <summary>
        /// The fill
        /// </summary>
        Fill,
        /// <summary>
        /// The center X
        /// </summary>
        CenterX,
        /// <summary>
        /// The center Y
        /// </summary>
        CenterY,
        /// <summary>
        /// The center
        /// </summary>
        Center,

        FillY,

        FillX
    }

    /// <summary>
    /// Enum Orientation
    /// </summary>
    public enum Orientation
    {
        /// <summary>
        /// The horizontal
        /// </summary>
        Horizontal,
        /// <summary>
        /// The vertical
        /// </summary>
        Vertical
    }

    /// <summary>
    /// Enum ControlState
    /// </summary>
    public enum ControlState
    {
        /// <summary>
        /// The default
        /// </summary>
        Default,
        /// <summary>
        /// The hot
        /// </summary>
        Hot,
        /// <summary>
        /// The pressed
        /// </summary>
        Pressed,
        /// <summary>
        /// The disabled
        /// </summary>
        Disabled,
        /// <summary>
        /// The focused
        /// </summary>
        Focused,
        /// <summary>
        /// The checked
        /// </summary>
        Checked,
        /// <summary>
        /// The checked hot
        /// </summary>
        CheckedHot,
        /// <summary>
        /// The checked pressed
        /// </summary>
        CheckedPressed,
        /// <summary>
        /// The checked disabled
        /// </summary>
        CheckedDisabled,
        /// <summary>
        /// The checked focused
        /// </summary>
        CheckedFocused,
        /// <summary>
        /// The selected
        /// </summary>
        Selected,
        /// <summary>
        /// The selected hot
        /// </summary>
        SelectedHot,
        /// <summary>
        /// The selected pressed
        /// </summary>
        SelectedPressed,
        /// <summary>
        /// The selected disabled
        /// </summary>
        SelectedDisabled,
        /// <summary>
        /// The selected focused
        /// </summary>
        SelectedFocused,
    }

    /// <summary>
    /// Enum Alignment
    /// </summary>
    public enum Alignment
    {
        /// <summary>
        /// The top left
        /// </summary>
        TopLeft,
        /// <summary>
        /// The top center
        /// </summary>
        TopCenter,
        /// <summary>
        /// The top right
        /// </summary>
        TopRight,
        /// <summary>
        /// The middle left
        /// </summary>
        MiddleLeft,
        /// <summary>
        /// The middle center
        /// </summary>
        MiddleCenter,
        /// <summary>
        /// The middle right
        /// </summary>
        MiddleRight,
        /// <summary>
        /// The bottom left
        /// </summary>
        BottomLeft,
        /// <summary>
        /// The bottom center
        /// </summary>
        BottomCenter,
        /// <summary>
        /// The bottom right
        /// </summary>
        BottomRight,
        /// <summary>
        /// The inherit
        /// </summary>
        Inherit
    }

    /// <summary>
    /// Enum AnchorStyles
    /// </summary>
    [Flags]
    public enum AnchorStyles
    {
        /// <summary>
        /// The none
        /// </summary>
        None = 0,
        /// <summary>
        /// The top
        /// </summary>
        Top = 1,
        /// <summary>
        /// The bottom
        /// </summary>
        Bottom = 2,
        /// <summary>
        /// The left
        /// </summary>
        Left = 4,
        /// <summary>
        /// The right
        /// </summary>
        Right = 8
    }
}
