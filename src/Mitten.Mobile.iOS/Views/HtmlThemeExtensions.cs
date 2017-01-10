using Mitten.Mobile.Themes;

namespace Mitten.Mobile.iOS.Views
{
    /// <summary>
    /// Contains extensions for the HtmlTheme.
    /// </summary>
    public static class HtmlThemeExtensions
    {
        private static class Constants
        {
            public const int HtmlHeaderSizeIncrement = 2;
        }

        /// <summary>
        /// Applies a theme with standard sizes to an html string by injecting/appending a stylesheet.
        /// </summary>
        /// <param name="theme">The theme.</param>
        /// <param name="html">The html to apply the theme to.</param>
        /// <returns>An updated html string with a theme applied.</returns>
        public static string ApplyToHtml(this HtmlTheme theme, string html)
        {
            return
                theme.ApplyToHtml(
                    html,
                    ViewFontSizes.StandardFontSize,
                    ViewFontSizes.StandardFontSize + (Constants.HtmlHeaderSizeIncrement * 5),
                    ViewFontSizes.StandardFontSize + (Constants.HtmlHeaderSizeIncrement * 4),
                    ViewFontSizes.StandardFontSize + (Constants.HtmlHeaderSizeIncrement * 3),
                    ViewFontSizes.StandardFontSize + (Constants.HtmlHeaderSizeIncrement * 2),
                    ViewFontSizes.StandardFontSize + (Constants.HtmlHeaderSizeIncrement * 1),
                    ViewFontSizes.StandardFontSize);
        }
    }
}