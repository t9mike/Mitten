using System;

namespace Mitten.Mobile.ViewModels
{
    /// <summary>
    /// Represents an item in a menu.
    /// </summary>
    public class MenuItem
    {
        /// <summary>
        /// Initializes a new instance of the MenuItem class.
        /// </summary>
        /// <param name="text">The text for the item.</param>
        /// <param name="select">A function to invoke when the item should be selected.</param>
        public MenuItem(string text, Action select)
            : this(
                text,
                null,
                () =>
                {
                    select();
                    return true;
                })
        {
        }

        /// <summary>
        /// Initializes a new instance of the MenuItem class.
        /// </summary>
        /// <param name="text">The text for the item.</param>
        /// <param name="select">A function to invoke when the item should be selected and returns a value indicating whether or not the operation was successful.</param>
        public MenuItem(string text, Func<bool> select)
            : this(text, null, select)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MenuItem class.
        /// </summary>
        /// <param name="text">The text for the item.</param>
        /// <param name="tag">An optional object to associate with the item.</param>
        /// <param name="select">A function to invoke when the item should be selected and returns a value indicating whether or not the operation was successful.</param>
        public MenuItem(string text, object tag, Action select)
            : this(
                text,
                tag,
                () =>
                {
                    select();
                    return true;
                })
        {
        }

        /// <summary>
        /// Initializes a new instance of the MenuItem class.
        /// </summary>
        /// <param name="text">The text for the item.</param>
        /// <param name="tag">An optional object to associate with the item.</param>
        /// <param name="select">A function to invoke when the item should be selected and returns a value indicating whether or not the operation was successful.</param>
        public MenuItem(string text, object tag, Func<bool> select)
        {
            Throw.IfArgumentNullOrWhitespace(text, nameof(text));
            Throw.IfArgumentNull(select, nameof(select));

            this.Text = text;
            this.Select = select;
            this.Tag = tag;
        }

        /// <summary>
        /// Gets the text for the item.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Gets a function to invoke when the item should be selected and returns a value indicating whether or not the operation was successful.
        /// </summary>
        public Func<bool> Select { get; }

        /// <summary>
        /// Gets the tag for the item.
        /// </summary>
        public object Tag { get; }
    }
}