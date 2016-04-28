# Squid

C# Realtime GUI System


SQUID is an SDK to create user interfaces for games and other 2D/3D realtime applications, using a Retained Mode system.
SQUID does not depend on a certain rendering engine, you can use it with any engine you like, on any platform that supports the .NET 2.0 framework.

- it's a GUI blackbox
- it's easy to integrate
- it is engine independent
- it does not manage textures or fonts, only integers and strings
- it doesn’t draw anything, you do

All you need to do is to implement 1 interface, the ISquidRenderer.

#Features:

- scale9 texture grid!
- docking, anchoring
- margin,padding
- z-order, clipping, scissoring
- hierarchical opacity
- input event handlers
- international keyboard support
- custom mouse cursors
- tooltips
- drag & drop
- snapping windows
- modal windows
- control state fading
- easy skinning via styles
- texture UV mapping
- full scissoring control
- automatic batching
- it’s fast!

#Standard Controls:

- Button
- CheckBox
- DropDownList
- Dialog
- FlowLayoutFrame
- Frame
- ImageControl
- Label
- ListBox
- ListView
- Panel
- RadioButton
- Resizer
- Scrollbar
- Slider
- SplitContainer
- TabControl
- TextArea
- TextBox
- TreeView
- Window

TODOs:
- merge TextBox (single line) and TextArea (multi line)
- remove Xml namespace, classes and serializer. should be external (json/yaml/etc)
- clean up comments and commented code
