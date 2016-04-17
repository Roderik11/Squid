using System;
using System.Collections.Generic;
using System.Text;

namespace Squid
{
    /// <summary>
    /// Interface ISquidRenderer
    /// </summary>
    public interface ISquidRenderer : IDisposable
    {
        /// <summary>
        /// Returns a unique integer for the given texture name
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.Int32.</returns>
        int GetTexture(string name);

        /// <summary>
        /// Returns a unique integer for the given font name
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.Int32.</returns>
        int GetFont(string name);

        /// <summary>
        /// Gets the size of the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <returns>Point.</returns>
        Point GetTextSize(string text, int font);

        /// <summary>
        /// Gets the size of the texture.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <returns>Point.</returns>
        Point GetTextureSize(int texture);

        /// <summary>
        /// Set the scissor rectangle
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        void Scissor(int x, int y, int width, int height);

        /// <summary>
        /// Draws a box.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="color">The color.</param>
        void DrawBox(int x, int y, int width, int height, int color);

        /// <summary>
        /// Draws the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="font">The font.</param>
        /// <param name="color">The color.</param>
        void DrawText(string text, int x, int y, int font, int color);

        /// <summary>
        /// Draws the texture.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="source">The source.</param>
        /// <param name="color">The color.</param>
        void DrawTexture(int texture, int x, int y, int width, int height, Rectangle source, int color);

        /// <summary>
        /// Starts the batch.
        /// </summary>
        void StartBatch();

        /// <summary>
        /// Ends the batch.
        /// </summary>
        /// <param name="final">if set to <c>true</c> [final].</param>
        void EndBatch(bool final);
    }

    /// <summary>
    /// And empty implementation of the ISquidRenderer interface.
    /// This is the default value of Gui.Renderer.
    /// </summary>
    public sealed class NoRenderer : ISquidRenderer
    {
        public void StartBatch() { }

        public void EndBatch(bool final) { }

        public int GetFont(string name) { return -1; }

        public int GetTexture(string name) { return -1; }

        public Point GetTextSize(string text, int font) { return new Point(); }

        public Point GetTextureSize(int texture) { return new Point(); }

        public void Scissor(int x, int y, int width, int height) { }

        public void DrawBox(int x, int y, int width, int height, int color) { }

        public void DrawTexture(int texture, int x, int y, int width, int height, Rectangle source, int color) { }

        public void DrawText(string text, int x, int y, int font, int color) { }

        public void Dispose() { }
    }
}
