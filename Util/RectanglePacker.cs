using System;
using System.Collections.Generic;

namespace Squid
{
    /// <summary>Rectangle packer using an algorithm by Javier Arevalo</summary>
    /// <remarks>
    ///   <para>
    ///     Original code by Javier Arevalo (jare at iguanademos dot com). Rewritten
    ///     to C# / .NET by Markus Ewald (cygon at nuclex dot org). The following comments
    ///     were written by the original author when he published his algorithm.
    ///   </para>
    ///   <para>
    ///     You have a bunch of rectangular pieces. You need to arrange them in a
    ///     rectangular surface so that they don't overlap, keeping the total area of the
    ///     rectangle as small as possible. This is fairly common when arranging characters
    ///     in a bitmapped font, lightmaps for a 3D engine, and I guess other situations as
    ///     well.
    ///   </para>
    ///   <para>
    ///     The idea of this algorithm is that, as we add rectangles, we can pre-select
    ///     "interesting" places where we can try to add the next rectangles. For optimal
    ///     results, the rectangles should be added in order. I initially tried using area
    ///     as a sorting criteria, but it didn't work well with very tall or very flat
    ///     rectangles. I then tried using the longest dimension as a selector, and it
    ///     worked much better. So much for intuition...
    ///   </para>
    ///   <para>
    ///     These "interesting" places are just to the right and just below the currently
    ///     added rectangle. The first rectangle, obviously, goes at the top left, the next
    ///     one would go either to the right or below this one, and so on. It is a weird way
    ///     to do it, but it seems to work very nicely.
    ///   </para>
    ///   <para>
    ///     The way we search here is fairly brute-force, the fact being that for most
    ///     offline purposes the performance seems more than adequate. I have generated a
    ///     japanese font with around 8500 characters and all the time was spent generating
    ///     the bitmaps.
    ///   </para>
    ///   <para>
    ///     Also, for all we care, we could grow the parent rectangle in a different way
    ///     than power of two. It just happens that power of 2 is very convenient for
    ///     graphics hardware textures.
    ///   </para>
    ///   <para>
    ///     I'd be interested in hearing of other approaches to this problem. Make sure
    ///     to post them on http://www.flipcode.com
    ///   </para>
    /// </remarks>
    public class RectanglePacker
    {
        #region class PackingRectangle

        private struct PackingRectangle
        {
            public int y;
            public int x;
            public int Width;
            public int Height;

            public int Right { get { return x + Width; } }
            public int Bottom { get { return y + Height; } }

            public PackingRectangle(int x, int y, int width, int height)
            {
                this.x = x;
                this.y = y;
                Width = width;
                Height = height;
            }

            public bool Intersects(PackingRectangle value)
            {
                return ((((value.x < (x + Width)) && (x < (value.x + value.Width))) && (value.y < (y + Height))) && (y < (value.y + value.Height)));
            }
        }

        #endregion

        #region class AnchorRankComparer

        /// <summary>Compares the 'rank' of anchoring points</summary>
        /// <remarks>
        ///   Anchoring points are potential locations for the placement of new rectangles.
        ///   Each time a rectangle is inserted, an anchor point is generated on its upper
        ///   right end and another one at its lower left end. The anchor points are kept
        ///   in a list that is ordered by their closeness to the upper left corner of the
        ///   packing area (their 'rank') so the packer favors positions that are closer to
        ///   the upper left for new rectangles.
        /// </remarks>
        private class AnchorRankComparer : IComparer<Point>
        {

            /// <summary>Provides a default instance for the anchor rank comparer</summary>
            public static AnchorRankComparer Default = new AnchorRankComparer();

            /// <summary>Compares the rank of two anchors against each other</summary>
            /// <param name="left">Left anchor point that will be compared</param>
            /// <param name="right">Right anchor point that will be compared</param>
            /// <returns>The relation of the two anchor point's ranks to each other</returns>
            public int Compare(Point left, Point right)
            {
                //return Math.Min(left.x, left.y) - Math.Min(right.x, right.y);
                return (left.x + left.y) - (right.x + right.y);
            }

        }

        #endregion

        /// <summary>Maximum allowed width of the packing area</summary>
        private int packingAreaWidth;
        /// <summary>Maximum allowed height of the packing area</summary>
        private int packingAreaHeight;

        /// <summary>Initializes a new rectangle packer</summary>
        /// <param name="packingAreaWidth">Maximum width of the packing area</param>
        /// <param name="packingAreaHeight">Maximum height of the packing area</param>
        public RectanglePacker(int packingAreaWidth, int packingAreaHeight)
        {
            this.packingAreaWidth = packingAreaWidth;
            this.packingAreaHeight = packingAreaHeight;

            this.packedRectangles = new List<PackingRectangle>();
            this.anchors = new List<Point>();
            this.anchors.Add(new Point(0, 0));

            this.actualPackingAreaWidth = 1;
            this.actualPackingAreaHeight = 1;
        }

        /// <summary>Tries to allocate space for a rectangle in the packing area</summary>
        /// <param name="rectangleWidth">Width of the rectangle to allocate</param>
        /// <param name="rectangleHeight">Height of the rectangle to allocate</param>
        /// <param name="placement">Output parameter receiving the rectangle's placement</param>
        /// <returns>True if space for the rectangle could be allocated</returns>
        public Point Pack(int rectangleWidth, int rectangleHeight)
        {
            Point placement = Point.Zero;
            // Try to find an anchor where the rectangle fits in, enlarging the packing
            // area and repeating the search recursively until it fits or the
            // maximum allowed size is exceeded.
            int anchorIndex = selectAnchorRecursive(
              rectangleWidth, rectangleHeight,
              this.actualPackingAreaWidth, this.actualPackingAreaHeight
            );

            // No anchor could be found at which the rectangle did fit in
            if (anchorIndex == -1)
                return Point.Zero;

            placement = this.anchors[anchorIndex];

            // Move the rectangle either to the left or to the top until it collides with
            // a neightbouring rectangle. This is done to combat the effect of lining up
            // rectangles with gaps to the left or top of them because the anchor that
            // would allow placement there has been blocked by another rectangle
            optimizePlacement(ref placement, rectangleWidth, rectangleHeight);

            // Remove the used anchor and add new anchors at the upper right and lower left
            // positions of the new rectangle
            {
                // The anchor is only removed if the placement optimization didn't
                // move the rectangle so far that the anchor isn't blocked anymore
                bool blocksAnchor =
                  ((placement.x + rectangleWidth) > this.anchors[anchorIndex].x) &&
                  ((placement.y + rectangleHeight) > this.anchors[anchorIndex].y);

                if (blocksAnchor)
                    this.anchors.RemoveAt(anchorIndex);

                // Add new anchors at the upper right and lower left coordinates of the rectangle
                insertAnchor(new Point(placement.x + rectangleWidth, placement.y));
                insertAnchor(new Point(placement.x, placement.y + rectangleHeight));
            }

            // Finally, we can add the rectangle to our packed rectangles list
            this.packedRectangles.Add(new PackingRectangle(placement.x, placement.y, rectangleWidth, rectangleHeight)
            );

            return placement;
        }

        /// <summary>
        ///   Optimizes the rectangle's placement by moving it either left or up to fill
        ///   any gaps resulting from rectangles blocking the anchors of the most optimal
        ///   placements.
        /// </summary>
        /// <param name="placement">Placement to be optimized</param>
        /// <param name="rectangleWidth">Width of the rectangle to be optimized</param>
        /// <param name="rectangleHeight">Height of the rectangle to be optimized</param>
        private void optimizePlacement(
          ref Point placement, int rectangleWidth, int rectangleHeight
        )
        {
            PackingRectangle rectangle = new PackingRectangle(
            placement.x, placement.y, rectangleWidth, rectangleHeight
          );

            // Try to move the rectangle to the left as far as possible
            int leftMost = placement.x;
            while (isFree(ref rectangle, packingAreaWidth, packingAreaHeight))
            {
                leftMost = rectangle.x;
                --rectangle.x;
            }

            // Reset rectangle to original position
            rectangle.x = placement.x;

            // Try to move the rectangle upwards as far as possible
            int topMost = placement.y;
            while (isFree(ref rectangle, packingAreaWidth, packingAreaHeight))
            {
                topMost = rectangle.y;
                --rectangle.y;
            }

            // Use the dimension in which the rectangle could be moved farther
            if ((placement.x - leftMost) > (placement.y - topMost))
                placement.x = leftMost;
            else
                placement.y = topMost;
        }

        /// <summary>
        ///   Searches for a free anchor and recursively enlarges the packing area
        ///   if none can be found.
        /// </summary>
        /// <param name="rectangleWidth">Width of the rectangle to be placed</param>
        /// <param name="rectangleHeight">Height of the rectangle to be placed</param>
        /// <param name="testedPackingAreaWidth">Width of the tested packing area</param>
        /// <param name="testedPackingAreaHeight">Height of the tested packing area</param>
        /// <returns>
        ///   Index of the anchor the rectangle is to be placed at or -1 if the rectangle
        ///   does not fit in the packing area anymore.
        /// </returns>
        private int selectAnchorRecursive(
          int rectangleWidth, int rectangleHeight,
          int testedPackingAreaWidth, int testedPackingAreaHeight
        )
        {

            // Try to locate an anchor point where the rectangle fits in
            int freeAnchorIndex = findFirstFreeAnchor(
              rectangleWidth, rectangleHeight, testedPackingAreaWidth, testedPackingAreaHeight
            );

            // If a the rectangle fits without resizing packing area (any further in case
            // of a recursive call), take over the new packing area size and return the
            // anchor at which the rectangle can be placed.
            if (freeAnchorIndex != -1)
            {
                this.actualPackingAreaWidth = testedPackingAreaWidth;
                this.actualPackingAreaHeight = testedPackingAreaHeight;

                return freeAnchorIndex;
            }

            //
            // If we reach this point, the rectangle did not fit in the current packing
            // area and our only choice is to try and enlarge the packing area.
            //

            // For readability, determine whether the packing area can be enlarged
            // any further in its width and in its height
            bool canEnlargeWidth = (testedPackingAreaWidth < packingAreaWidth);
            bool canEnlargeHeight = (testedPackingAreaHeight < packingAreaHeight);
            bool shouldEnlargeHeight =
              (!canEnlargeWidth) ||
              (testedPackingAreaHeight < testedPackingAreaWidth);

            // Try to enlarge the smaller of the two dimensions first (unless the smaller
            // dimension is already at its maximum size). 'shouldEnlargeHeight' is true
            // when the height was the smaller dimension or when the width is maxed out.
            if (canEnlargeHeight && shouldEnlargeHeight)
            {

                // Try to double the height of the packing area
                return selectAnchorRecursive(
                  rectangleWidth, rectangleHeight,
                  testedPackingAreaWidth, Math.Min(testedPackingAreaHeight * 2, packingAreaHeight)
                );

            }
            else if (canEnlargeWidth)
            {

                // Try to double the width of the packing area
                return selectAnchorRecursive(
                  rectangleWidth, rectangleHeight,
                  Math.Min(testedPackingAreaWidth * 2, packingAreaWidth), testedPackingAreaHeight
                );

            }
            else
            {

                // Both dimensions are at their maximum sizes and the rectangle still
                // didn't fit. We give up!
                return -1;

            }

        }

        /// <summary>Locates the first free anchor at which the rectangle fits</summary>
        /// <param name="rectangleWidth">Width of the rectangle to be placed</param>
        /// <param name="rectangleHeight">Height of the rectangle to be placed</param>
        /// <param name="testedPackingAreaWidth">Total width of the packing area</param>
        /// <param name="testedPackingAreaHeight">Total height of the packing area</param>
        /// <returns>The index of the first free anchor or -1 if none is found</returns>
        private int findFirstFreeAnchor(
          int rectangleWidth, int rectangleHeight,
          int testedPackingAreaWidth, int testedPackingAreaHeight
        )
        {
            PackingRectangle potentialLocation = new PackingRectangle(
            0, 0, rectangleWidth, rectangleHeight
          );

            // Walk over all anchors (which are ordered by their distance to the
            // upper left corner of the packing area) until one is discovered that
            // can house the new rectangle.
            for (int index = 0; index < this.anchors.Count; ++index)
            {
                potentialLocation.x = this.anchors[index].x;
                potentialLocation.y = this.anchors[index].y;

                // See if the rectangle would fit in at this anchor point
                if (isFree(ref potentialLocation, testedPackingAreaWidth, testedPackingAreaHeight))
                    return index;
            }

            // No anchor points were found where the rectangle would fit in
            return -1;
        }

        /// <summary>
        ///   Determines whether the rectangle can be placed in the packing area
        ///   at its current location.
        /// </summary>
        /// <param name="rectangle">Rectangle whose position to check</param>
        /// <param name="testedPackingAreaWidth">Total width of the packing area</param>
        /// <param name="testedPackingAreaHeight">Total height of the packing area</param>
        /// <returns>True if the rectangle can be placed at its current position</returns>
        private bool isFree(
          ref PackingRectangle rectangle, int testedPackingAreaWidth, int testedPackingAreaHeight
        )
        {

            // If the rectangle is partially or completely outside of the packing
            // area, it can't be placed at its current location
            bool leavesPackingArea =
              (rectangle.x < 0) ||
              (rectangle.y < 0) ||
              (rectangle.Right > testedPackingAreaWidth) ||
              (rectangle.Bottom > testedPackingAreaHeight);

            if (leavesPackingArea)
                return false;

            // Brute-force search whether the rectangle touches any of the other
            // rectangles already in the packing area
            for (int index = 0; index < this.packedRectangles.Count; ++index)
            {

                if (this.packedRectangles[index].Intersects(rectangle))
                    return false;

            }

            // Success! The rectangle is inside the packing area and doesn't overlap
            // with any other rectangles that have already been packed.
            return true;

        }

        /// <summary>Inserts a new anchor point into the anchor list</summary>
        /// <param name="anchor">Anchor point that will be inserted</param>
        /// <remarks>
        ///   This method tries to keep the anchor list ordered by ranking the anchors
        ///   depending on the distance from the top left corner in the packing area.
        /// </remarks>
        private void insertAnchor(Point anchor)
        {

            // Find out where to insert the new anchor based on its rank (which is
            // calculated based on the anchor's distance to the top left corner of
            // the packing area).
            //
            // From MSDN on BinarySearch():
            //   "If the List does not contain the specified value, the method returns
            //    a negative integer. You can apply the bitwise complement operation (~) to
            //    this negative integer to get the index of the first element that is
            //    larger than the search value."
            int insertIndex = this.anchors.BinarySearch(anchor, AnchorRankComparer.Default);
            if (insertIndex < 0)
                insertIndex = ~insertIndex;

            // Insert the anchor at the index matching its rank
            this.anchors.Insert(insertIndex, anchor);

        }

        /// <summary>Current width of the packing area</summary>
        private int actualPackingAreaWidth;
        /// <summary>Current height of the packing area</summary>
        private int actualPackingAreaHeight;
        /// <summary>Rectangles contained in the packing area</summary>
        private List<PackingRectangle> packedRectangles;
        /// <summary>Anchoring points where new rectangles can potentially be placed</summary>
        private List<Point> anchors;

    }

} // namespace Overdose
