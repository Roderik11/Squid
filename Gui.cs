using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;

namespace Squid
{
    public delegate string TranslateStringHandler(string text);

    /// <summary>
    /// This is the main entry of Squid.
    /// </summary>
    public static class Gui
    {
        private static string Clipboard;

        public static TranslateStringHandler TranslateHandler;
        public static int Language { get; private set; }

        public static void SetLanguage(int language)
        {
            Language = language;
        }

        public static string Translate(string text)
        {
            if (TranslateHandler != null)
                return TranslateHandler(text);

            return text;
        }

        /// <summary>
        /// Raised when [mouse down].
        /// </summary>
        public static event MouseEvent MouseDown;
        public static event MouseEvent MouseUp;

        /// <summary>
        /// Gets or sets the renderer.
        /// This is set to NoRenderer by default.
        /// </summary>
        /// <value>The renderer.</value>
        public static ISquidRenderer Renderer { get; set; }

        /// <summary>
        /// Elapsed time since last frame in milliseconds
        /// </summary>
        public static float TimeElapsed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [always scissor].
        /// </summary>
        /// <value><c>true</c> if [always scissor]; otherwise, <c>false</c>.</value>
        public static bool AlwaysScissor { get; set; }

        /// <summary>
        /// Gets or sets the global fade speed.
        /// </summary>
        /// <value>The global fade speed.</value>
        public static float GlobalFadeSpeed { get; set; }

        internal static ButtonState[] Buttons;

        public static ReadOnlyCollection<KeyData> KeyEvents => readOnlyKeyList;
        
        private static readonly List<KeyData> keyList = new List<KeyData>();
        private static readonly ReadOnlyCollection<KeyData> readOnlyKeyList = new ReadOnlyCollection<KeyData>(keyList);

        public static int DragThreshold = 6;

        /// <summary>
        /// Gets the mouse position.
        /// </summary>
        /// <value>The mouse position.</value>
        public static Point MousePosition { get; private set; }

        /// <summary>
        /// Gets the mouse movement.
        /// </summary>
        /// <value>The mouse movement.</value>
        public static Point MouseMovement { get; private set; }

        /// <summary>
        /// Gets the mouse scroll.
        /// </summary>
        /// <value>The mouse scroll.</value>
        public static int MouseScroll { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [shift pressed].
        /// </summary>
        /// <value><c>true</c> if [shift pressed]; otherwise, <c>false</c>.</value>
        public static bool ShiftPressed { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [alt pressed].
        /// </summary>
        /// <value><c>true</c> if [alt pressed]; otherwise, <c>false</c>.</value>
        public static bool AltPressed { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [CTRL pressed].
        /// </summary>
        /// <value><c>true</c> if [CTRL pressed]; otherwise, <c>false</c>.</value>
        public static bool CtrlPressed { get; private set; }

        /// <summary>
        /// Gets or sets the double click speed.
        /// </summary>
        /// <value>The double click speed.</value>
        public static float DoubleClickSpeed { get; set; }

        /// <summary>
        /// return the state of the given button index
        /// </summary>
        /// <param name="index">index of the button</param>
        /// <returns>state of the button</returns>
        public static ButtonState GetButton(int index)
        {
            if (Buttons.Length > index)
                return Buttons[index];

            return ButtonState.None;
        }

        /// <summary>
        /// sets the currently pressed and released keys
        /// </summary>
        /// <param name="keys">array of KeyData</param>
        public static void SetKeyboard(KeyData[] keys, int length = -1)
        {
            if (keys == null) return;
           
            if (length < 0)
                length = keys.Length;

            var count = Math.Min(keys.Length, length);
            keyList.Clear();

            for (int i = 0; i < count; i++)
            {
                var key = keys[i];
                keyList.Add(key);

                if (key.Key == Keys.LEFTSHIFT || key.Key == Keys.RIGHTSHIFT)
                    ShiftPressed = key.Pressed;

                if (key.Key == Keys.ALT_LEFT || key.Key == Keys.ALT_RIGHT)
                    AltPressed = key.Pressed;

                if (key.Key == Keys.LEFTCONTROL || key.Key == Keys.RIGHTCONTROL)
                    CtrlPressed = key.Pressed;
            }
        }

        /// <summary>
        /// sets the current mouse position
        /// </summary>
        /// <param name="posX">x component of the position</param>
        /// <param name="posY">y component of the position</param>
        /// <param name="scroll">scrollwheel delta</param>
        public static void SetMouse(int posX, int posY, int scroll)
        {
            Point m = MousePosition;
            MousePosition = new Point(posX, posY);
            Point move = MousePosition - m;
            MouseMovement = move;
            MouseScroll = scroll;
        }

        /// <summary>
        /// sets the state of mouse buttons
        /// </summary>
        /// <param name="buttons">array of booleans. true = button down</param>
        public static void SetButtons(params bool[] buttons)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (i == Buttons.Length) break;

                if (buttons[i])
                {
                    if (Buttons[i] == ButtonState.None)
                        Buttons[i] = ButtonState.Down;
                    else if (Buttons[i] == ButtonState.Down)
                        Buttons[i] = ButtonState.Press;
                }
                else
                {
                    if (Buttons[i] == ButtonState.Press || Buttons[i] == ButtonState.Down)
                        Buttons[i] = ButtonState.Up;
                    else
                        Buttons[i] = ButtonState.None;
                }
            }
        }

        internal static void OnMouseDown(Control sender, MouseEventArgs args)
        {
            MouseDown?.Invoke(sender, args);
        }

        internal static void OnMouseUp(Control sender, MouseEventArgs args)
        {
            MouseUp?.Invoke(sender, args);
        }

        static Gui()
        {
            Renderer = new NoRenderer();
            DoubleClickSpeed = 250;
            Buttons = new ButtonState[5];
        }

        /// <summary>
        /// sets the clipboard string
        /// </summary>
        /// <param name="data"></param>
        public static void SetClipboard(string data) => Clipboard = data;

        /// <summary>
        /// returns the current clipboard string
        /// </summary>
        /// <returns></returns>
        public static string GetClipboard() => Clipboard;

        /// <summary>
        /// generates a standard skin
        /// this is only used for sample purposes
        /// </summary>
        /// <returns></returns>
        public static Skin GenerateStandardSkin()
        {
            ControlStyle baseStyle = new ControlStyle();
            baseStyle.Tiling = TextureMode.Grid;
            baseStyle.Grid = new Margin(3);
            baseStyle.Texture = "button_hot.dds";
            baseStyle.Default.Texture = "button_default.dds";
            baseStyle.Pressed.Texture = "button_down.dds";
            baseStyle.SelectedPressed.Texture = "button_down.dds";
            baseStyle.Focused.Texture = "button_down.dds";
            baseStyle.SelectedFocused.Texture = "button_down.dds";
            baseStyle.Selected.Texture = "button_down.dds";
            baseStyle.SelectedHot.Texture = "button_down.dds";

            ControlStyle itemStyle = new ControlStyle(baseStyle);
            itemStyle.TextPadding = new Margin(10, 0, 0, 0);
            itemStyle.TextAlign = Alignment.MiddleLeft;

            ControlStyle buttonStyle = new ControlStyle(baseStyle);
            buttonStyle.TextPadding = new Margin(0);
            buttonStyle.TextAlign = Alignment.MiddleCenter;

            ControlStyle tooltipStyle = new ControlStyle(buttonStyle);
            tooltipStyle.TextPadding = new Margin(8);
            tooltipStyle.TextAlign = Alignment.TopLeft;

            ControlStyle inputStyle = new ControlStyle();
            inputStyle.Texture = "input_default.dds";
            inputStyle.Hot.Texture = "input_focused.dds";
            inputStyle.Focused.Texture = "input_focused.dds";
            inputStyle.TextPadding = new Margin(8);
            inputStyle.Tiling = TextureMode.Grid;
            inputStyle.Focused.Tint = ColorInt.ARGB(1, 0, 0, 1);
            inputStyle.Grid = new Margin(3);

            ControlStyle windowStyle = new ControlStyle();
            windowStyle.Tiling = TextureMode.Grid;
            windowStyle.Grid = new Margin(9);
            windowStyle.Texture = "window.dds";

            ControlStyle frameStyle = new ControlStyle();
            frameStyle.Tiling = TextureMode.Grid;
            frameStyle.Grid = new Margin(4);
            frameStyle.Texture = "frame.dds";
            frameStyle.TextPadding = new Margin(8);

            ControlStyle vscrollTrackStyle = new ControlStyle();
            vscrollTrackStyle.Tiling = TextureMode.Grid;
            vscrollTrackStyle.Grid = new Margin(3);
            vscrollTrackStyle.Texture = "vscroll_track.dds";

            ControlStyle vscrollButtonStyle = new ControlStyle();
            vscrollButtonStyle.Tiling = TextureMode.Grid;
            vscrollButtonStyle.Grid = new Margin(3);
            vscrollButtonStyle.Texture = "vscroll_button.dds";
            vscrollButtonStyle.Hot.Texture = "vscroll_button_hot.dds";
            vscrollButtonStyle.Pressed.Texture = "vscroll_button_down.dds";

            ControlStyle vscrollUp = new ControlStyle();
            vscrollUp.Default.Texture = "vscrollUp_default.dds";
            vscrollUp.Hot.Texture = "vscrollUp_hot.dds";
            vscrollUp.Pressed.Texture = "vscrollUp_down.dds";
            vscrollUp.Focused.Texture = "vscrollUp_hot.dds";

            ControlStyle hscrollTrackStyle = new ControlStyle();
            hscrollTrackStyle.Tiling = TextureMode.Grid;
            hscrollTrackStyle.Grid = new Margin(3);
            hscrollTrackStyle.Texture = "hscroll_track.dds";

            ControlStyle hscrollButtonStyle = new ControlStyle();
            hscrollButtonStyle.Tiling = TextureMode.Grid;
            hscrollButtonStyle.Grid = new Margin(3);
            hscrollButtonStyle.Texture = "hscroll_button.dds";
            hscrollButtonStyle.Hot.Texture = "hscroll_button_hot.dds";
            hscrollButtonStyle.Pressed.Texture = "hscroll_button_down.dds";

            ControlStyle hscrollUp = new ControlStyle();
            hscrollUp.Default.Texture = "hscrollUp_default.dds";
            hscrollUp.Hot.Texture = "hscrollUp_hot.dds";
            hscrollUp.Pressed.Texture = "hscrollUp_down.dds";
            hscrollUp.Focused.Texture = "hscrollUp_hot.dds";

            ControlStyle checkButtonStyle = new ControlStyle();
            checkButtonStyle.Default.Texture = "checkbox_default.dds";
            checkButtonStyle.Hot.Texture = "checkbox_hot.dds";
            checkButtonStyle.Pressed.Texture = "checkbox_down.dds";
            checkButtonStyle.Checked.Texture = "checkbox_checked.dds";
            checkButtonStyle.CheckedFocused.Texture = "checkbox_checked_hot.dds";
            checkButtonStyle.CheckedHot.Texture = "checkbox_checked_hot.dds";
            checkButtonStyle.CheckedPressed.Texture = "checkbox_down.dds";

            ControlStyle comboLabelStyle = new ControlStyle();
            comboLabelStyle.TextPadding = new Margin(10, 0, 0, 0);
            comboLabelStyle.Default.Texture = "combo_default.dds";
            comboLabelStyle.Hot.Texture = "combo_hot.dds";
            comboLabelStyle.Pressed.Texture = "combo_down.dds";
            comboLabelStyle.Focused.Texture = "combo_hot.dds";
            comboLabelStyle.Tiling = TextureMode.Grid;
            comboLabelStyle.Grid = new Margin(3, 0, 0, 0);

            ControlStyle comboButtonStyle = new ControlStyle();
            comboButtonStyle.Default.Texture = "combo_button_default.dds";
            comboButtonStyle.Hot.Texture = "combo_button_hot.dds";
            comboButtonStyle.Pressed.Texture = "combo_button_down.dds";
            comboButtonStyle.Focused.Texture = "combo_button_hot.dds";

            ControlStyle labelStyle = new ControlStyle();
            labelStyle.TextAlign = Alignment.TopLeft;
            labelStyle.TextPadding = new Margin(8);

            Skin skin = new Skin();

            skin.Add("item", itemStyle);
            skin.Add("textbox", inputStyle);
            skin.Add("button", buttonStyle);
            skin.Add("window", windowStyle);
            skin.Add("frame", frameStyle);
            skin.Add("checkBox", checkButtonStyle);
            skin.Add("comboLabel", comboLabelStyle);
            skin.Add("comboButton", comboButtonStyle);
            skin.Add("vscrollTrack", vscrollTrackStyle);
            skin.Add("vscrollButton", vscrollButtonStyle);
            skin.Add("vscrollUp", vscrollUp);
            skin.Add("hscrollTrack", hscrollTrackStyle);
            skin.Add("hscrollButton", hscrollButtonStyle);
            skin.Add("hscrollUp", hscrollUp);
            skin.Add("multiline", labelStyle);
            skin.Add("tooltip", tooltipStyle);

            return skin;
        }

    }
}
