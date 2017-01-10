namespace Mitten.Mobile.ViewModels
{
    /// <summary>
    /// Represents the title to display in a navigation bar.
    /// </summary>
    public class NavigationBarTitle
    {
        /// <summary>
        /// Defines the type of title.
        /// </summary>
        public enum NavigationBarTitleType
        {
            /// <summary>
            /// Indicates no title should be displayed.
            /// </summary>
            Empty,

            /// <summary>
            /// Indicates the title to display as plain text.
            /// </summary>
            Text,

            /// <summary>
            /// Indicates the title should display the application's logo.
            /// </summary>
            Logo
        }

        /// <summary>
        /// Represents an empty title.
        /// </summary>
        public static readonly NavigationBarTitle Empty = new NavigationBarTitle(NavigationBarTitleType.Empty, string.Empty);

        /// <summary>
        /// Represents a title that displays the application's logo.
        /// </summary>
        public static readonly NavigationBarTitle Logo = new NavigationBarTitle(NavigationBarTitleType.Logo, string.Empty);

        private NavigationBarTitle(NavigationBarTitleType titleType, string text)
        {
            this.TitleType = titleType;
            this.Text = text;
        }

        /// <summary>
        /// Gets the type of title.
        /// </summary>
        public NavigationBarTitleType TitleType { get; private set; }

        /// <summary>
        /// Gets the text for the title when displayed as plain text.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Creates a title that displays the specified text.
        /// </summary>
        /// <param name="text">The text for the title.</param>
        /// <returns>A new title.</returns>
        public static NavigationBarTitle AsText(string text)
        {
            return new NavigationBarTitle(NavigationBarTitleType.Text, text);
        }
    }
}