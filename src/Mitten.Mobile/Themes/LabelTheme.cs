using Mitten.Mobile.Graphics;

namespace Mitten.Mobile.Themes
{
    /// <summary>
    /// Defines a theme for a label.
    /// </summary>
    public class LabelTheme
    {
        /// <summary>
        /// Initializes a new instance of the LabelTheme class.
        /// </summary>
        /// <param name="backgroundColor">The background color for the label.</param>
        /// <param name="fontColor">The font color for the label.</param>
        /// <param name="fontStyle">Defines additional styling for the font.</param>
        /// <param name="fontName">The name of the font for the label text or null to use the default system's font.</param>
        public LabelTheme(
            Color backgroundColor, 
            Color fontColor, 
            FontStyle fontStyle = FontStyle.None, 
            string fontName = null)
        {
            this.BackgroundColor = backgroundColor;
            this.FontColor = fontColor;
            this.FontName = fontName;
            this.FontStyle = fontStyle;
        }

        /// <summary>
        /// Gets the background color for the label.
        /// </summary>
        public Color BackgroundColor { get; private set; }

        /// <summary>
        /// Gets the name of the font for the label or null if the default system's font should be used.
        /// </summary>
        public string FontName { get; private set; }

        /// <summary>
        /// Gets the color of the font.
        /// </summary>
        public Color FontColor { get; private set; }

        /// <summary>
        /// Gets additional styling for the font.
        /// </summary>
        public FontStyle FontStyle { get; private set; }

        /// <summary>
        /// Gets a new instance of the current theme with an updated font color.
        /// </summary>
        /// <param name="color">The new font color.</param>
        /// <returns>A new label theme.</returns>
        public LabelTheme WithFontColor(Color color)
        {
            return 
                new LabelTheme(
                    this.BackgroundColor,
                    color,
                    this.FontStyle,
                    this.FontName);
        }

        /// <summary>
        /// Gets a new instance of the current theme with an updated font style.
        /// </summary>
        /// <param name="fontStyle">The font style.</param>
        /// <returns>A new label theme.</returns>
        public LabelTheme WithFontStyle(FontStyle fontStyle)
        {
            return 
                new LabelTheme(
                    this.BackgroundColor,
                    this.FontColor,
                    fontStyle,
                    this.FontName);
        }
    }
}