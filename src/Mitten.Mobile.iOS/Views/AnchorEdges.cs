using System;

namespace Mitten.Mobile.iOS.Views
{
    /// <summary>
    /// Defines the edges in which a view can be anchored to its parent.
    /// When anchoring, a parent is a container so the child view will be
    /// anchored on the inside of its parent.
    /// </summary>
    [Flags]
    public enum AnchorEdges
    {
        /// <summary>
        /// Represents no anchoring.
        /// </summary>
        None = 0,

        /// <summary>
        /// The top edge of the view is anchored to the top edge of the parent.
        /// </summary>
        Top = 1,

        /// <summary>
        /// The bottom edge of the view is anchored to the bottom edge of the parent.
        /// </summary>
        Bottom = 2,

        /// <summary>
        /// The left edge of the view is anchored to the left edge of the parent.
        /// </summary>
        Left = 4,

        /// <summary>
        /// The right edge of the view is anchored to the right edge of the parent.
        /// </summary>
        Right = 8,

        /// <summary>
        /// The left and right edges of the view are anchored to the left and right edges of the parent.
        /// </summary>
        Horizontal = Left | Right,

        /// <summary>
        /// The top and bottom edges of the view are anchored to the top and bottom edges of the parent.
        /// </summary>
        Vertical = Top | Bottom,

        /// <summary>
        /// The view is anchored to all the edges of the parent.
        /// </summary>
        All = Top | Bottom | Left | Right,
    }
}