using Mitten.Mobile.Graphics;

namespace Mitten.Mobile.Themes
{
    /// <summary>
    /// Defines a theme for the navigation bar.
    /// </summary>
    public class NavigationBarTheme
    {
        /// <summary>
        /// Initializes a new instance of the NavigationBarTheme class.
        /// </summary>
        /// <param name="backgroundColor">The background color for the navigation bar.</param>
        /// <param name="itemColor">The color for navigation bar items that do not have an image applied.</param>
        /// <param name="fontColor">The font color for the navigation bar.</param>
        /// <param name="shadowColor">The color for the bottom shadow of the navigation bar.</param>
        /// <param name="fontName">The name of the font for the navigation bar text or null to use the default system's font.</param>
        public NavigationBarTheme(
            Color backgroundColor, 
            Color itemColor, 
            Color fontColor, 
            Color shadowColor,
            string fontName = null)
        {
            this.BackgroundColor = backgroundColor;
            this.ItemColor = itemColor;
            this.FontColor = fontColor;
            this.ShadowColor = shadowColor;
            this.FontName = fontName;
        }

        /// <summary>
        /// Gets the background color for the navigation bar.
        /// </summary>
        public Color BackgroundColor { get; private set; }

        /// <summary>
        /// Gets the color for navigation bar items that do not have an image applied.
        /// </summary>
        public Color ItemColor { get; private set; }

        /// <summary>
        /// Gets the color for the bottom shadow of the navigation bar.
        /// </summary>
        public Color ShadowColor { get; private set; }

        /// <summary>
        /// Gets the name of the font for the navigation bar items or null to use the default system's font.
        /// </summary>
        public string FontName { get; private set; }

        /// <summary>
        /// Gets the color of the font.
        /// </summary>
        public Color FontColor { get; private set; }

        /// <summary>
        /// Gets a new theme based on the current them with an updated background color.
        /// </summary>
        /// <param name="color">A background color.</param>
        /// <returns>A new theme.</returns>
        public NavigationBarTheme WithBackgroundColor(Color color)
        {
            return
                new NavigationBarTheme(
                    color,
                    this.ItemColor,
                    this.FontColor,
                    this.ShadowColor);
        }
    }
}