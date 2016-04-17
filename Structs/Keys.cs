using System;
using System.Collections.Generic;
using System.Text;

namespace Squid
{
    /// <summary>
    /// Enum Keys
    /// </summary>
    public enum Keys
    {
        /// <summary>
        /// The ESCAPE
        /// </summary>
        ESCAPE = 1,
        /// <summary>
        /// The d1
        /// </summary>
        D1 = 2,
        /// <summary>
        /// The d2
        /// </summary>
        D2 = 3,
        /// <summary>
        /// The d3
        /// </summary>
        D3 = 4,
        /// <summary>
        /// The d4
        /// </summary>
        D4 = 5,
        /// <summary>
        /// The d5
        /// </summary>
        D5 = 6,
        /// <summary>
        /// The d6
        /// </summary>
        D6 = 7,
        /// <summary>
        /// The d7
        /// </summary>
        D7 = 8,
        /// <summary>
        /// The d8
        /// </summary>
        D8 = 9,
        /// <summary>
        /// The d9
        /// </summary>
        D9 = 10,
        /// <summary>
        /// The d0
        /// </summary>
        D0 = 11,
        /// <summary>
        /// The MINUS
        /// </summary>
        MINUS = 12,
        /// <summary>
        /// The EQUALS
        /// </summary>
        EQUALS = 13,
        /// <summary>
        /// The BACKSPACE
        /// </summary>
        BACKSPACE = 14,
        /// <summary>
        /// The TAB
        /// </summary>
        TAB = 15,
        /// <summary>
        /// The Q
        /// </summary>
        Q = 16,
        /// <summary>
        /// The W
        /// </summary>
        W = 17,
        /// <summary>
        /// The E
        /// </summary>
        E = 18,
        /// <summary>
        /// The R
        /// </summary>
        R = 19,
        /// <summary>
        /// The T
        /// </summary>
        T = 20,
        /// <summary>
        /// The Y
        /// </summary>
        Y = 21,
        /// <summary>
        /// The U
        /// </summary>
        U = 22,
        /// <summary>
        /// The I
        /// </summary>
        I = 23,
        /// <summary>
        /// The O
        /// </summary>
        O = 24,
        /// <summary>
        /// The P
        /// </summary>
        P = 25,
        /// <summary>
        /// The LEFTBRACKET
        /// </summary>
        LEFTBRACKET = 26,
        /// <summary>
        /// The RIGHTBRACKET
        /// </summary>
        RIGHTBRACKET = 27,
        /// <summary>
        /// The RETURN
        /// </summary>
        RETURN = 28,
        /// <summary>
        /// The LEFTCONTROL
        /// </summary>
        LEFTCONTROL = 29,
        /// <summary>
        /// The A
        /// </summary>
        A = 30,
        /// <summary>
        /// The S
        /// </summary>
        S = 31,
        /// <summary>
        /// The D
        /// </summary>
        D = 32,
        /// <summary>
        /// The F
        /// </summary>
        F = 33,
        /// <summary>
        /// The G
        /// </summary>
        G = 34,
        /// <summary>
        /// The H
        /// </summary>
        H = 35,
        /// <summary>
        /// The J
        /// </summary>
        J = 36,
        /// <summary>
        /// The K
        /// </summary>
        K = 37,
        /// <summary>
        /// The L
        /// </summary>
        L = 38,
        /// <summary>
        /// The SEMICOLON
        /// </summary>
        SEMICOLON = 39,
        /// <summary>
        /// The APOSTROPHE
        /// </summary>
        APOSTROPHE = 40,
        /// <summary>
        /// The GRAVE
        /// </summary>
        GRAVE = 41,
        /// <summary>
        /// The LEFTSHIFT
        /// </summary>
        LEFTSHIFT = 42,
        /// <summary>
        /// The BACKSLASH
        /// </summary>
        BACKSLASH = 43,
        /// <summary>
        /// The Z
        /// </summary>
        Z = 44,
        /// <summary>
        /// The X
        /// </summary>
        X = 45,
        /// <summary>
        /// The C
        /// </summary>
        C = 46,
        /// <summary>
        /// The V
        /// </summary>
        V = 47,
        /// <summary>
        /// The B
        /// </summary>
        B = 48,
        /// <summary>
        /// The N
        /// </summary>
        N = 49,
        /// <summary>
        /// The M
        /// </summary>
        M = 50,
        /// <summary>
        /// The COMMA
        /// </summary>
        COMMA = 51,
        /// <summary>
        /// The PERIOD
        /// </summary>
        PERIOD = 52,
        /// <summary>
        /// The SLASH
        /// </summary>
        SLASH = 53,
        /// <summary>
        /// The RIGHTSHIFT
        /// </summary>
        RIGHTSHIFT = 54,
        /// <summary>
        /// The NUMPADSTAR
        /// </summary>
        NUMPADSTAR = 55,
        /// <summary>
        /// The MULTIPLY
        /// </summary>
        MULTIPLY = 55,
        /// <summary>
        /// The LEFTMENU
        /// </summary>
        LEFTMENU = 56,
        /// <summary>
        /// The AL t_ LEFT
        /// </summary>
        ALT_LEFT = 56,
        /// <summary>
        /// The SPACE
        /// </summary>
        SPACE = 57,
        /// <summary>
        /// The CAPITAL
        /// </summary>
        CAPITAL = 58,
        /// <summary>
        /// The CAPSLOCK
        /// </summary>
        CAPSLOCK = 58,
        /// <summary>
        /// The f1
        /// </summary>
        F1 = 59,
        /// <summary>
        /// The f2
        /// </summary>
        F2 = 60,
        /// <summary>
        /// The f3
        /// </summary>
        F3 = 61,
        /// <summary>
        /// The f4
        /// </summary>
        F4 = 62,
        /// <summary>
        /// The f5
        /// </summary>
        F5 = 63,
        /// <summary>
        /// The f6
        /// </summary>
        F6 = 64,
        /// <summary>
        /// The f7
        /// </summary>
        F7 = 65,
        /// <summary>
        /// The f8
        /// </summary>
        F8 = 66,
        /// <summary>
        /// The f9
        /// </summary>
        F9 = 67,
        /// <summary>
        /// The F10
        /// </summary>
        F10 = 68,
        /// <summary>
        /// The NUMLOCK
        /// </summary>
        NUMLOCK = 69,
        /// <summary>
        /// The SCROLL
        /// </summary>
        SCROLL = 70,
        /// <summary>
        /// The NUMPA d7
        /// </summary>
        NUMPAD7 = 71,
        /// <summary>
        /// The NUMPA d8
        /// </summary>
        NUMPAD8 = 72,
        /// <summary>
        /// The NUMPA d9
        /// </summary>
        NUMPAD9 = 73,
        /// <summary>
        /// The SUBTRACT
        /// </summary>
        SUBTRACT = 74,
        /// <summary>
        /// The NUMPADMINUS
        /// </summary>
        NUMPADMINUS = 74,
        /// <summary>
        /// The NUMPA d4
        /// </summary>
        NUMPAD4 = 75,
        /// <summary>
        /// The NUMPA d5
        /// </summary>
        NUMPAD5 = 76,
        /// <summary>
        /// The NUMPA d6
        /// </summary>
        NUMPAD6 = 77,
        /// <summary>
        /// The NUMPADPLUS
        /// </summary>
        NUMPADPLUS = 78,
        /// <summary>
        /// The ADD
        /// </summary>
        ADD = 78,
        /// <summary>
        /// The NUMPA d1
        /// </summary>
        NUMPAD1 = 79,
        /// <summary>
        /// The NUMPA d2
        /// </summary>
        NUMPAD2 = 80,
        /// <summary>
        /// The NUMPA d3
        /// </summary>
        NUMPAD3 = 81,
        /// <summary>
        /// The NUMPA d0
        /// </summary>
        NUMPAD0 = 82,
        /// <summary>
        /// The DECIMAL
        /// </summary>
        DECIMAL = 83,
        /// <summary>
        /// The NUMPADPERIOD
        /// </summary>
        NUMPADPERIOD = 83,
        /// <summary>
        /// The OE M_102
        /// </summary>
        OEM_102 = 86,
        /// <summary>
        /// The F11
        /// </summary>
        F11 = 87,
        /// <summary>
        /// The F12
        /// </summary>
        F12 = 88,
        /// <summary>
        /// The F13
        /// </summary>
        F13 = 100,
        /// <summary>
        /// The F14
        /// </summary>
        F14 = 101,
        /// <summary>
        /// The F15
        /// </summary>
        F15 = 102,
        /// <summary>
        /// The KANA
        /// </summary>
        KANA = 112,
        /// <summary>
        /// The ABN t_ c1
        /// </summary>
        ABNT_C1 = 115,
        /// <summary>
        /// The CONVERT
        /// </summary>
        CONVERT = 121,
        /// <summary>
        /// The NOCONVERT
        /// </summary>
        NOCONVERT = 123,
        /// <summary>
        /// The YEN
        /// </summary>
        YEN = 125,
        /// <summary>
        /// The ABN t_ c2
        /// </summary>
        ABNT_C2 = 126,
        /// <summary>
        /// The NUMPADEQUALS
        /// </summary>
        NUMPADEQUALS = 141,
        /// <summary>
        /// The CIRCUMFLEX
        /// </summary>
        CIRCUMFLEX = 144,
        /// <summary>
        /// The PREVTRACK
        /// </summary>
        PREVTRACK = 144,
        /// <summary>
        /// The AT
        /// </summary>
        AT = 145,
        /// <summary>
        /// The COLON
        /// </summary>
        COLON = 146,
        /// <summary>
        /// The UNDERLINE
        /// </summary>
        UNDERLINE = 147,
        /// <summary>
        /// The KANJI
        /// </summary>
        KANJI = 148,
        /// <summary>
        /// The STOP
        /// </summary>
        STOP = 149,
        /// <summary>
        /// The AX
        /// </summary>
        AX = 150,
        /// <summary>
        /// The UNLABELED
        /// </summary>
        UNLABELED = 151,
        /// <summary>
        /// The NEXTTRACK
        /// </summary>
        NEXTTRACK = 153,
        /// <summary>
        /// The NUMPADENTER
        /// </summary>
        NUMPADENTER = 156,
        /// <summary>
        /// The RIGHTCONTROL
        /// </summary>
        RIGHTCONTROL = 157,
        /// <summary>
        /// The MUTE
        /// </summary>
        MUTE = 160,
        /// <summary>
        /// The CALCULATOR
        /// </summary>
        CALCULATOR = 161,
        /// <summary>
        /// The PLAYPAUSE
        /// </summary>
        PLAYPAUSE = 162,
        /// <summary>
        /// The MEDIASTOP
        /// </summary>
        MEDIASTOP = 164,
        /// <summary>
        /// The VOLUMEDOWN
        /// </summary>
        VOLUMEDOWN = 174,
        /// <summary>
        /// The VOLUMEUP
        /// </summary>
        VOLUMEUP = 176,
        /// <summary>
        /// The WEBHOME
        /// </summary>
        WEBHOME = 178,
        /// <summary>
        /// The NUMPADCOMMA
        /// </summary>
        NUMPADCOMMA = 179,
        /// <summary>
        /// The NUMPADSLASH
        /// </summary>
        NUMPADSLASH = 181,
        /// <summary>
        /// The DIVIDE
        /// </summary>
        DIVIDE = 181,
        /// <summary>
        /// The SYSRQ
        /// </summary>
        SYSRQ = 183,
        /// <summary>
        /// The AL t_ RIGHT
        /// </summary>
        ALT_RIGHT = 184,
        /// <summary>
        /// The RIGHTMENU
        /// </summary>
        RIGHTMENU = 184,
        /// <summary>
        /// The PAUSE
        /// </summary>
        PAUSE = 197,
        /// <summary>
        /// The HOME
        /// </summary>
        HOME = 199,
        /// <summary>
        /// The UP
        /// </summary>
        UP = 200,
        /// <summary>
        /// The UPARROW
        /// </summary>
        UPARROW = 200,
        /// <summary>
        /// The PAGEUP
        /// </summary>
        PAGEUP = 201,
        /// <summary>
        /// The PRIOR
        /// </summary>
        PRIOR = 201,
        /// <summary>
        /// The LEFTARROW
        /// </summary>
        LEFTARROW = 203,
        /// <summary>
        /// The LEFT
        /// </summary>
        LEFT = 203,
        /// <summary>
        /// The RIGHT
        /// </summary>
        RIGHT = 205,
        /// <summary>
        /// The RIGHTARROW
        /// </summary>
        RIGHTARROW = 205,
        /// <summary>
        /// The END
        /// </summary>
        END = 207,
        /// <summary>
        /// The DOWN
        /// </summary>
        DOWN = 208,
        /// <summary>
        /// The DOWNARROW
        /// </summary>
        DOWNARROW = 208,
        /// <summary>
        /// The PAGEDOWN
        /// </summary>
        PAGEDOWN = 209,
        /// <summary>
        /// The NEXT
        /// </summary>
        NEXT = 209,
        /// <summary>
        /// The INSERT
        /// </summary>
        INSERT = 210,
        /// <summary>
        /// The DELETE
        /// </summary>
        DELETE = 211,
        /// <summary>
        /// The LEFTWINDOWS
        /// </summary>
        LEFTWINDOWS = 219,
        /// <summary>
        /// The RWIN
        /// </summary>
        RWIN = 220,
        /// <summary>
        /// The APPS
        /// </summary>
        APPS = 221,
        /// <summary>
        /// The POWER
        /// </summary>
        POWER = 222,
        /// <summary>
        /// The SLEEP
        /// </summary>
        SLEEP = 223,
        /// <summary>
        /// The WAKE
        /// </summary>
        WAKE = 227,
        /// <summary>
        /// The WEBSEARCH
        /// </summary>
        WEBSEARCH = 229,
        /// <summary>
        /// The WEBFAVORITES
        /// </summary>
        WEBFAVORITES = 230,
        /// <summary>
        /// The WEBREFRESH
        /// </summary>
        WEBREFRESH = 231,
        /// <summary>
        /// The WEBSTOP
        /// </summary>
        WEBSTOP = 232,
        /// <summary>
        /// The WEBFORWARD
        /// </summary>
        WEBFORWARD = 233,
        /// <summary>
        /// The WEBBACK
        /// </summary>
        WEBBACK = 234,
        /// <summary>
        /// The MYCOMPUTER
        /// </summary>
        MYCOMPUTER = 235,
        /// <summary>
        /// The MAIL
        /// </summary>
        MAIL = 236,
        /// <summary>
        /// The MEDIASELECT
        /// </summary>
        MEDIASELECT = 237,
    }
}
