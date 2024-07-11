# Squid UI framework

### What is Squid?

SQUID is a framework to create user interfaces for games and other 2D/3D realtime applications, using a [Retained Mode](https://en.wikipedia.org/wiki/Retained_mode) system.
SQUID does not depend on a certain rendering engine, you can use it with any engine you like, on any platform that supports the .NET 2.0 framework.

- it's a UI blackbox
- it's easy to integrate
- it is engine agnostic
- it is unaware of texture or font resources, only integers and strings
- it doesn’t draw anything; you do

All you need to do is to implement a single interface - the ISquidRenderer.

Getting started: https://github.com/Roderik11/Squid/wiki/Using-Squid

Complex example that covers all areas of Squid: https://github.com/Roderik11/Spark/tree/main/Spark.Editor

### Example Screenshots

###### Sample Arrangement
![image](https://user-images.githubusercontent.com/5743257/122032701-2b671680-cdd0-11eb-830d-299888acdbae.png "Sample Arrangement")
###### Custom Game Engine
![image](https://github.com/Roderik11/Squid/assets/5743257/06f866bc-1e55-4a99-8dbd-725e4f7c10ff)
![image](https://github.com/Roderik11/Squid/assets/5743257/f478f149-ffc1-4a98-bee2-9c5b6deb754a)


### Features:

- scale9 texture grid
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
- automatic batching
- easy to extend
- it’s fast

### Standard Controls:

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
- VirtualList
- Window

### TODOs:
- merge TextBox (single line) and TextArea (multi line)
- remove Xml namespace, classes and serializer. should be external (json/yaml/etc)
- clean up comments and commented code
