using System;
using UIKit;

namespace Mitten.Mobile.iOS.Views
{
    /// <summary>
    /// Represents an item to display in a segmented control.
    /// </summary>
    public class SegmentedControlItem
    {
        /// <summary>
        /// Initializes a new instance of the SegmentedControlItem class.
        /// </summary>
        /// <param name="title">The title for the item.</param>
        /// <param name="image">An image for the item.</param>
        /// <param name="itemPressed">The function to invoke when the item has been pressed.</param>
        public SegmentedControlItem(string title, UIImage image, Action itemPressed)
            : this(
                title,
                image,
                null,
                () =>
                {
                    itemPressed();
                    return true;
                })
        {
        }

        /// <summary>
        /// Initializes a new instance of the SegmentedControlItem class.
        /// </summary>
        /// <param name="title">The title for the item.</param>
        /// <param name="image">An image for the item.</param>
        /// <param name="itemPressed">The function to invoke when the item has been pressed and returns a value indicating whether or not the operation was successful.</param>
        public SegmentedControlItem(string title, UIImage image, Func<bool> itemPressed)
            : this(title, image, null, itemPressed)
        {
        }

        /// <summary>
        /// Initializes a new instance of the SegmentedControlItem class.
        /// </summary>
        /// <param name="title">The title for the item.</param>
        /// <param name="image">An image for the item.</param>
        /// <param name="tag">An optional object to associate with the item.</param>
        /// <param name="itemPressed">The function to invoke when the item has been pressed and returns a value indicating whether or not the operation was successful.</param>
        public SegmentedControlItem(string title, UIImage image, object tag, Func<bool> itemPressed)
        {
            Throw.IfArgumentNullOrWhitespace(title, nameof(title));
            Throw.IfArgumentNull(itemPressed, nameof(itemPressed));

            this.Title = title;
            this.Image = image;
            this.ItemPressed = itemPressed;
            this.Tag = tag;
        }

        /// <summary>
        /// Gets the title for the item.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the image for the item.
        /// </summary>
        public UIImage Image { get; }

        /// <summary>
        /// Gets a function to invoke when the item has been pressed and returns a value indicating whether or not the operation was successful.
        /// </summary>
        public Func<bool> ItemPressed { get; }

        /// <summary>
        /// Gets the tag for the item.
        /// </summary>
        public object Tag { get; }
    }
}