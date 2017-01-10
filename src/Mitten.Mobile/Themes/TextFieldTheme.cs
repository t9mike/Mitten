using Mitten.Mobile.Graphics;

namespace Mitten.Mobile.Themes
{
    /// <summary>
    /// Defines a theme for a standard edit text view.
    /// </summary>
    public class TextFieldTheme
    {
        /// <summary>
        /// Initializes a new instance of the TextFieldTheme class.
        /// </summary>
        /// <param name="backgroundColor">The background color for the text field.</param>
        /// <param name="placeHolderColor">The color to use for any place holder or hint text on the field.</param>
        /// <param name="fontColor">The font color for the text field.</param>
        /// <param name="cursorColor">The color for the text field's cursor.</param>
        /// <param name="fontName">The name of the font for the text field or null to use the default system's font.</param>
        public TextFieldTheme(
            Color backgroundColor, 
            Color placeHolderColor,
            Color fontColor, 
            Color cursorColor,
            string fontName = null)
        {
            this.BackgroundColor = backgroundColor;
            this.CursorColor = cursorColor;
            this.FontColor = fontColor;
            this.PlaceHolderColor = placeHolderColor;
            this.FontName = fontName;
        }

        /// <summary>
        /// Gets the background color for the text field.
        /// </summary>
        public Color BackgroundColor { get; private set; }

        /// <summary>
        /// Gets the color for the text field's cursor.
        /// </summary>
        public Color CursorColor { get; private set; }

        /// <summary>
        /// Gets the name of the font for the text field or null to use the default system's font.
        /// </summary>
        public string FontName { get; private set; }

        /// <summary>
        /// Gets the color of the text field.
        /// </summary>
        public Color FontColor { get; private set; }

        /// <summary>
        /// Gets the color to use for any place holder or hint text on the field.
        /// </summary>
        public Color PlaceHolderColor { get; private set; }
    }
}