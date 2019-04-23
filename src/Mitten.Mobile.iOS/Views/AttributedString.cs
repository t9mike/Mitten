using Foundation;
using Mitten.Mobile.iOS.Extensions;
using Mitten.Mobile.Themes;
using Mitten.Mobile.ViewModels;
using UIKit;

namespace Mitten.Mobile.iOS.Views
{
    /// <summary>
    /// Contains helper methods for working with attributed strings.
    /// </summary>
    public static class AttributedString
    {
        /// <summary>
        /// Applies a label theme to the specified attributed string for a label.
        /// </summary>
        /// <param name="attributedText">A attributed string to apply the theme to.</param>
        /// <param name="theme">The theme to apply.</param>
        /// <param name="range">Identifies the range of text that the theme will be applied to.</param>
        public static void ApplyTheme(NSMutableAttributedString attributedText, LabelTheme theme, NSRange range)
        {
            AttributedString.ApplyTheme(attributedText, theme, range, ViewFontSizes.StandardFontSize);
        }

        /// <summary>
        /// Applies a label theme to the specified attributed string for a label.
        /// </summary>
        /// <param name="attributedText">A attributed string to apply the theme to.</param>
        /// <param name="theme">The theme to apply.</param>
        /// <param name="range">Identifies the range of text that the theme will be applied to.</param>
        /// <param name="fontSize">The size of the font.</param>
        public static void ApplyTheme(NSMutableAttributedString attributedText, LabelTheme theme, NSRange range, int fontSize)
        {
            attributedText.AddAttribute(UIStringAttributeKey.BackgroundColor, theme.BackgroundColor.ToUIColor(), range);
            attributedText.AddAttribute(UIStringAttributeKey.ForegroundColor, theme.FontColor.ToUIColor(), range);
            attributedText.AddAttribute(UIStringAttributeKey.Font, UIFont.FromName(theme.FontName, fontSize), range);
        }

        /// <summary>
        /// Creates a new attributed string from the specified styled text and theme.
        /// </summary>
        /// <param name="styledText">The styled text.</param>
        /// <param name="theme">The theme to apply.</param>
        /// <returns>A new attributed string.</returns>
        public static NSMutableAttributedString FromStyledText(StyledText styledText, LabelTheme theme)
        {
            return AttributedString.FromStyledText(styledText, theme, ViewFontSizes.StandardFontSize);
        }

        /// <summary>
        /// Creates a new attributed string from the specified styled text and theme.
        /// </summary>
        /// <param name="styledText">The styled text.</param>
        /// <param name="theme">The theme to apply.</param>
        /// <param name="fontSize">The size of the font to use.</param>
        /// <returns>A new attributed string.</returns>
        public static NSMutableAttributedString FromStyledText(StyledText styledText, LabelTheme theme, int fontSize)
        {
            string text = styledText.ToString();
            NSMutableAttributedString attributedText = new NSMutableAttributedString(text);
            NSRange fullRange = new NSRange(0, text.Length);

            foreach (StyledText.TextPart part in styledText.Parts)
            {
                FontStyle fontStyle = FontStyle.None;
                if (part.Style.HasFlag(StyledText.Style.Bold))
                {
                    fontStyle |= FontStyle.Bold;
                }
                if (part.Style.HasFlag(StyledText.Style.Italic))
                {
                    fontStyle |= FontStyle.Italic;
                }

                UIFont font =
                    string.IsNullOrEmpty(theme.FontName)
                    ? UIFont.SystemFontOfSize(fontSize)
                    : UIFont.FromName(theme.FontName, fontSize);

                font = font.ApplyStyle(fontStyle);

                NSRange range = new NSRange(part.StartIndex, part.Text.Length);

                attributedText.AddAttribute(UIStringAttributeKey.Font, font, range);
                if (part.Style.HasFlag(StyledText.Style.Underline))
                {
                    attributedText.AddAttribute(UIStringAttributeKey.UnderlineStyle,
                        NSNumber.FromInt32((int)NSUnderlineStyle.Single), range);
                }
            }

            attributedText.AddAttribute(UIStringAttributeKey.BackgroundColor, theme.BackgroundColor.ToUIColor(), fullRange);
            attributedText.AddAttribute(UIStringAttributeKey.ForegroundColor, theme.FontColor.ToUIColor(), fullRange);

            return attributedText;
        }
    }
}
