using System;
using Mitten.Mobile.Themes;
using UIKit;

namespace Mitten.Mobile.iOS.Views
{
    /// <summary>
    /// Contains extensions for a UIFont.
    /// </summary>
    public static class UIFontExtensions
    {
        /// <summary>
        /// Applies a font style to the current font.
        /// </summary>
        /// <param name="font">A font.</param>
        /// <param name="fontStyle">The font style.</param>
        /// <returns>A new font with the specified style applied.</returns>
        public static UIFont ApplyStyle(this UIFont font, FontStyle fontStyle)
        {
            if (fontStyle == FontStyle.None)
            {
                return font;
            }

            UIFontDescriptorSymbolicTraits traits = UIFontDescriptorSymbolicTraits.ClassUnknown;

            if ((fontStyle & FontStyle.Bold) == FontStyle.Bold)
            {
                traits = UIFontDescriptorSymbolicTraits.Bold;
            }

            if ((fontStyle & FontStyle.Italic) == FontStyle.Italic)
            {
                traits |= UIFontDescriptorSymbolicTraits.Italic;
            }

            UIFontDescriptor fontDescriptor = font.FontDescriptor.CreateWithTraits(traits);
            if (fontDescriptor == null)
            {
                throw new ArgumentException("The font style (" + fontStyle + ") is not supported for font (" + font.Name + ").", nameof(fontStyle));
            }

            return UIFont.FromDescriptor(fontDescriptor, 0);
        }
    }
}
