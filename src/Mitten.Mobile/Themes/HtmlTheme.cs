using Mitten.Mobile.Graphics;

namespace Mitten.Mobile.Themes
{
    /// <summary>
    /// Defines a theme for a view that displays html content.
    /// </summary>
    public class HtmlTheme
    {
        private static class Constants
        {
            public const string StyleTags = "<style>{0}</style>";
            public const string RGB = "rgb({0}, {1}, {2})";
            public const string BodyStyleSheet = @"
            body {{
                    background-color: {0};
                    color:{1};
                    font-family: {2};
                    font-size: {3}px;
                }}";
            public const string FullStyleSheet = @"
                body {{
                    background-color: {0};
                    font-family: {3};
                    font-size: {4}px;
                    color:{1};
                }}

                h1 {{
                    font-size: {5}px;
                    color:{2};
                }}

                h2 {{
                    font-size: {6}px;
                    color:{2};
                }}

                h3 {{
                    font-size: {7}px;
                    color:{2};
                }}

                h4 {{
                    font-size: {8}px;
                    color:{2};
                }}

                h5 {{
                    font-size: {9}px;
                    color:{2};
                }}

                h6 {{
                    font-size: {10}px;
                    color:{2};
                }}";
        }

        /// <summary>
        /// Initializes a new instance of the HtmlTheme class.
        /// </summary>
        /// <param name="backgroundColor">The background color for the html.</param>
        /// <param name="contentFontColor">The font color for any content text.</param>
        /// <param name="headerFontColor">The font color for any header text.</param>
        /// <param name="fontName">The name of the font for the text or null to use the default system's font.</param>
        public HtmlTheme(
            Color backgroundColor, 
            Color contentFontColor,
            Color headerFontColor,
            string fontName = null)
        {
            this.BackgroundColor = backgroundColor;
            this.ContentFontColor = contentFontColor;
            this.HeaderFontColor = headerFontColor;
            this.FontName = fontName;
        }

        /// <summary>
        /// Gets the background color for the html.
        /// </summary>
        public Color BackgroundColor { get; private set; }

        /// <summary>
        /// Gets the name of the font for the text or null to use the default system's font.
        /// </summary>
        public string FontName { get; private set; }

        /// <summary>
        /// Gets the color of the font for any content text.
        /// </summary>
        public Color ContentFontColor { get; private set; }

        /// <summary>
        /// Gets the color of the font for any header text.
        /// </summary>
        public Color HeaderFontColor { get; private set; }

        /// <summary>
        /// Applies the current theme to an html string by injecting/appending a stylesheet.
        /// </summary>
        /// <param name="html">The html to apply the theme to.</param>
        /// <param name="bodySize">The pixel size for the body.</param>
        /// <returns>The html appended with a style sheet.</returns>
        public string ApplyToHtml(string html, int bodySize)
        {
            string css = string.Format(
                Constants.BodyStyleSheet,
                this.GetRGBString(this.BackgroundColor),
                this.GetRGBString(this.ContentFontColor),
                this.FontName,
                bodySize);

            return html + string.Format(Constants.StyleTags, css);
        }

        /// <summary>
        /// Applies the current theme to an html string by injecting/appending a stylesheet.
        /// </summary>
        /// <param name="html">The html to apply the theme to.</param>
        /// <param name="bodySize">The pixel size for the body.</param>
        /// <param name="header1Size">The pixel size for an h1 header.</param>
        /// <param name="header2Size">The pixel size for an h2 header.</param>
        /// <param name="header3Size">The pixel size for an h3 header.</param>
        /// <param name="header4Size">The pixel size for an h4 header.</param>
        /// <param name="header5Size">The pixel size for an h5 header.</param>
        /// <param name="header6Size">The pixel size for an h6 header.</param>
        /// <returns>The html appended with a style sheet.</returns>
        public string ApplyToHtml(
            string html,
            int bodySize,
            int header1Size,
            int header2Size,
            int header3Size,
            int header4Size,
            int header5Size,
            int header6Size)
        {
            string css = string.Format(
                Constants.FullStyleSheet,
                this.GetRGBString(this.BackgroundColor),
                this.GetRGBString(this.ContentFontColor),
                this.GetRGBString(this.HeaderFontColor),
                this.FontName,
                bodySize,
                header1Size,
                header2Size,
                header3Size,
                header4Size,
                header5Size,
                header6Size);

            return html + string.Format(Constants.StyleTags, css);
        }

        /// <summary>
        /// Creates a new HtmlTheme from the specified label theme.
        /// </summary>
        /// <param name="labelTheme">A label theme.</param>
        /// <returns>The label theme.</returns>
        public static HtmlTheme FromLabelTheme(LabelTheme labelTheme)
        {
            return
                new HtmlTheme(
                    labelTheme.BackgroundColor,
                    labelTheme.FontColor,
                    labelTheme.FontColor,
                    labelTheme.FontName);
        }

        private string GetRGBString(Color color)
        {
            return string.Format(Constants.RGB, color.Red, color.Green, color.Blue);
        }
    }
}