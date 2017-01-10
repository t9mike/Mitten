using Mitten.Mobile.Graphics;

namespace Mitten.Mobile.Themes
{
    /// <summary>
    /// Defines a theme for a standard button view.
    /// </summary>
    public class ButtonTheme
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonTheme"/> class.
        /// </summary>
        /// <param name="backgroundColor">The background color for the button.</param>
        /// <param name="borderColor">The border color for the button.</param>
        /// <param name="fontColor">The font color for the button.</param>
        public ButtonTheme(Color backgroundColor, Color borderColor, Color fontColor)
            : this(backgroundColor, borderColor, fontColor, Colors.LightGrey, Colors.Transparent)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonTheme"/> class.
        /// </summary>
        /// <param name="backgroundColor">The background color for the button.</param>
        /// <param name="borderColor">The border color for the button.</param>
        /// <param name="fontColor">The font color for the button.</param>
        /// <param name="disabledColor">The color to use when the button is disabled.</param>
        /// <param name="imageTintColor">An optional tint color to use for any images associated with the button.</param>
        /// <param name="fontName">The name of the font for the button text or null to use the default system's font.</param>
        public ButtonTheme(
            Color backgroundColor, 
            Color borderColor, 
            Color fontColor,
            Color disabledColor,
            Color imageTintColor,
            string fontName = null)
        {
            this.BackgroundColor = backgroundColor;
            this.BorderColor = borderColor;
            this.FontColor = fontColor;
            this.DisabledColor = disabledColor;
            this.ImageTintColor = imageTintColor;
            this.FontName = fontName;
        }

        /// <summary>
        /// Gets the background color for the button.
        /// </summary>
        public Color BackgroundColor { get; private set; }

        /// <summary>
        /// Gets the border color for the button.
        /// </summary>
        public Color BorderColor { get; private set; }

        /// <summary>
        /// Gets the font for the label text or null to use the default system's font
        /// </summary>
        public string FontName { get; private set; }

        /// <summary>
        /// Gets the color to use when the button is disabled.
        /// </summary>
        public Color DisabledColor { get; private set; }

        /// <summary>
        /// Gets the color of the font.
        /// </summary>
        public Color FontColor { get; private set; }

        /// <summary>
        /// Gets the tint color to use for any images associated with the button.
        /// </summary>
        public Color ImageTintColor { get; private set; }

        /// <summary>
        /// Gets a version of the current button theme with all non-transparent colors updated with the specified color.
        /// The disabled color for the theme will not be modified.
        /// </summary>
        /// <param name="color">The new color for the button.</param>
        /// <returns>A new button theme.</returns>
        public ButtonTheme WithColor(Color color)
        {
            return
                new ButtonTheme(
                    this.SwapColorIfNotTransparent(this.BackgroundColor, color),
                    this.SwapColorIfNotTransparent(this.BorderColor, color),
                    this.SwapColorIfNotTransparent(this.FontColor, color),
                    this.DisabledColor,
                    this.SwapColorIfNotTransparent(this.ImageTintColor, color));  
        }

        /// <summary>
        /// Gets a version of the current button theme with a transparent border.
        /// </summary>
        /// <returns>A new button theme.</returns>
        public ButtonTheme WithTransparentBorder()
        {
            return 
                new ButtonTheme(
                    this.BackgroundColor, 
                    Colors.Transparent, 
                    this.FontColor, 
                    this.DisabledColor,
                    this.ImageTintColor);
        }

        /// <summary>
        /// Gets a version of the current button theme with a transparent background.
        /// </summary>
        /// <returns>A new button theme.</returns>
        public ButtonTheme WithTransparentBackground()
        {
            return 
                new ButtonTheme(
                    Colors.Transparent, 
                    this.BorderColor, 
                    this.FontColor, 
                    this.DisabledColor,
                    this.ImageTintColor);
        }

        /// <summary>
        /// Gets a version of the current button theme with the image tint color the same as the current button's font color.
        /// </summary>
        /// <returns>A new button theme.</returns>
        public ButtonTheme WithImageTintAsFontColor()
        {
            return 
                new ButtonTheme(
                    this.BackgroundColor, 
                    this.BorderColor, 
                    this.FontColor,
                    this.DisabledColor,
                    this.FontColor);
        }

        private Color SwapColorIfNotTransparent(Color source, Color newColor)
        {
            return
                source.Alpha == 0
                  ? source
                  : newColor;
        }
    }
}