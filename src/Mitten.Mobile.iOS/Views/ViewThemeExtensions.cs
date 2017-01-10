using System;
using CoreGraphics;
using Foundation;
using Mitten.Mobile.iOS.Extensions;
using Mitten.Mobile.Themes;
using UIKit;

namespace Mitten.Mobile.iOS.Views
{
    /// <summary>
    /// Contains extension methods for UIViews for applying themes.
    /// </summary>
    public static class ViewThemeExtensions
    {
        private static class Constants
        {
            public const int ButtonBorderWidth = 1;
            public const int TextFieldBorderWidth = 4;
        }

        /// <summary>
        /// Applies a theme to the specified label.
        /// </summary>
        /// <param name="label">A label to apply the theme to.</param>
        /// <param name="theme">The theme to apply to the label.</param>
        public static void ApplyTheme(this UILabel label, LabelTheme theme)
        {
            label.ApplyTheme(theme, ViewFontSizes.StandardFontSize);
        }

        /// <summary>
        /// Applies a theme to the specified label.
        /// </summary>
        /// <param name="label">A label to apply the theme to.</param>
        /// <param name="theme">The theme to apply to the label.</param>
        /// <param name="fontSize">The size of the font for the label.</param>
        public static void ApplyTheme(this UILabel label, LabelTheme theme, int fontSize)
        {
            label.BackgroundColor = theme.BackgroundColor.ToUIColor();
            label.TextColor = theme.FontColor.ToUIColor();
            label.Font = ViewThemeExtensions.GetFont(theme.FontName, fontSize).ApplyStyle(theme.FontStyle);
        }

        /// <summary>
        /// Applies a theme to the specified button.
        /// </summary>
        /// <param name="button">A button to apply the theme to.</param>
        /// <param name="theme">The theme to apply to the button.</param>
        public static void ApplyTheme(this UIButton button, ButtonTheme theme)
        {
            button.ApplyTheme(theme, ViewFontSizes.StandardFontSize);
        }

        /// <summary>
        /// Applies a theme to the specified button.
        /// </summary>
        /// <param name="button">A button to apply the theme to.</param>
        /// <param name="theme">The theme to apply to the button.</param>
        /// <param name="fontSize">The size of the font for the button text.</param>
        public static void ApplyTheme(this UIButton button, ButtonTheme theme, int fontSize)
        {
            button.ApplyTheme(
                theme,
                null,
                default(UIEdgeInsets),
                false,
                fontSize);
        }

        /// <summary>
        /// Applies a theme to the specified button.
        /// </summary>
        /// <param name="button">A button to apply the theme to.</param>
        /// <param name="theme">The theme to apply to the button.</param>
        /// <param name="image">An image for the button.</param>
        /// <param name="imageInsets">Optional inset or outset margins for the rectangle around the button’s image.</param>
        /// <param name="imageLeft">Optional flag to identify if the image for the button should be shown to the left of the text, otherwise to the right; the default is true.</param>
        public static void ApplyTheme(
            this UIButton button,
            ButtonTheme theme,
            UIImage image,
            UIEdgeInsets imageInsets = default(UIEdgeInsets),
            bool imageLeft = true)
        {
            button.ApplyTheme(
                theme,
                image,
                imageInsets,
                imageLeft,
                ViewFontSizes.StandardFontSize);
        }

        /// <summary>
        /// Applies a theme to the specified button.
        /// </summary>
        /// <param name="button">A button to apply the theme to.</param>
        /// <param name="theme">The theme to apply to the button.</param>
        /// <param name="image">An image for the button.</param>
        /// <param name="imageInsets">Optional inset or outset margins for the rectangle around the button’s image.</param>
        /// <param name="imageLeft">Optional flag to identify if the image for the button should be shown to the left of the text, otherwise to the right; the default is true.</param>
        /// <param name="fontSize">The size of the font for the button text.</param>
        public static void ApplyTheme(
            this UIButton button,
            ButtonTheme theme,
            UIImage image,
            UIEdgeInsets imageInsets,
            bool imageLeft,
            int fontSize)
        {
            if (theme.BorderColor.Alpha > 0)
            {
                button.Layer.BorderColor =
                    button.Enabled
                    ? theme.BorderColor.ToCGColor()
                    : theme.DisabledColor.ToCGColor();

                button.Layer.BorderWidth = Constants.ButtonBorderWidth;

                button.Layer.CornerRadius = button.Frame.Height / 2;
            }

            if (image != null)
            {
                UIImage updatedImage = image;
                if (theme.ImageTintColor.Alpha > 0)
                {
                    if (updatedImage.RenderingMode != UIImageRenderingMode.AlwaysTemplate)
                    {
                        updatedImage = updatedImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                    }

                    button.TintColor = theme.ImageTintColor.ToUIColor();
                }

                button.SetImage(updatedImage, UIControlState.Normal);
                button.ImageEdgeInsets = imageInsets;

                if (!imageLeft)
                {
                    button.Transform = CGAffineTransform.MakeScale(-1.0f, 1.0f);
                    button.TitleLabel.Transform = CGAffineTransform.MakeScale(-1.0f, 1.0f);
                    button.ImageView.Transform = CGAffineTransform.MakeScale(-1.0f, 1.0f);
                }
            }

            button.BackgroundColor =
                button.Enabled || theme.BackgroundColor.Alpha == 0
                ? theme.BackgroundColor.ToUIColor()
                : theme.DisabledColor.ToUIColor();

            button.SetTitleColor(theme.FontColor.ToUIColor(), UIControlState.Normal);
            button.SetTitleColor(theme.DisabledColor.ToUIColor(), UIControlState.Disabled);

            button.Font = ViewThemeExtensions.GetFont(theme.FontName, fontSize);
        }

        /// <summary>
        /// Applies a theme to the specified text field.
        /// </summary>
        /// <param name="field">A text view to apply the theme to.</param>
        /// <param name="theme">The theme to apply to the text view.</param>
        public static void ApplyTheme(this UITextField field, TextFieldTheme theme)
        {
            field.ApplyTheme(theme, ViewFontSizes.LargeFontSize);
        }

        /// <summary>
        /// Applies a theme to the specified text field.
        /// </summary>
        /// <param name="field">A text field to apply the theme to.</param>
        /// <param name="theme">The theme to apply to the text field.</param>
        /// <param name="fontSize">The size of the font for the text field.</param>
        public static void ApplyTheme(this UITextField field, TextFieldTheme theme, int fontSize)
        {
            field.BackgroundColor = theme.BackgroundColor.ToUIColor();
            field.TintColor = theme.CursorColor.ToUIColor();

            if (field.BorderStyle != UITextBorderStyle.None)
            {
                // apply the background color to the boarder to give the field a 'thicker' appearance
                field.Layer.BorderColor = theme.BackgroundColor.ToCGColor();
                field.Layer.BorderWidth = Constants.TextFieldBorderWidth;
            }

            field.Font = ViewThemeExtensions.GetFont(theme.FontName, fontSize);
            field.TextColor = theme.FontColor.ToUIColor();

            if (!string.IsNullOrWhiteSpace(field.Placeholder))
            {
                field.AttributedPlaceholder = new NSAttributedString(field.Placeholder, null, theme.PlaceHolderColor.ToUIColor());
            }
        }

        /// <summary>
        /// Applies a theme to the specified text view.
        /// </summary>
        /// <param name="field">A text view to apply the theme to.</param>
        /// <param name="theme">The theme to apply to the text view.</param>
        public static void ApplyTheme(this UITextView field, TextFieldTheme theme)
        {
            field.ApplyTheme(theme, ViewFontSizes.LargeFontSize);
        }

        /// <summary>
        /// Applies a theme to the specified text view.
        /// </summary>
        /// <param name="field">A text view to apply the theme to.</param>
        /// <param name="theme">The theme to apply to the text view.</param>
        /// <param name="fontSize">The size of the font for the text field.</param>
        public static void ApplyTheme(this UITextView field, TextFieldTheme theme, int fontSize)
        {
            field.BackgroundColor = theme.BackgroundColor.ToUIColor();
            field.TintColor = theme.CursorColor.ToUIColor();

            field.Font = ViewThemeExtensions.GetFont(theme.FontName, fontSize);
            field.TextColor = theme.FontColor.ToUIColor();
        }

        /// <summary>
        /// Applies a theme to the navigation bar. Note, this does not update the theme for the title.
        /// </summary>
        /// <param name="navigationBar">A navigation bar to apply the theme to.</param>
        /// <param name="theme">The theme to apply to the navigation bar.</param>
        public static void ApplyTheme(this UINavigationBar navigationBar, NavigationBarTheme theme)
        {
            UIColor backgroundColor = theme.BackgroundColor.ToUIColor();

            navigationBar.BackgroundColor = backgroundColor;
            navigationBar.BarTintColor = backgroundColor;
            navigationBar.Translucent = theme.BackgroundColor.Alpha == 0;
            navigationBar.TintColor = theme.ItemColor.ToUIColor();

            if (theme.ShadowColor.Alpha > 0)
            {
                nfloat height = 1.0f / UIScreen.MainScreen.Scale;
                UIView shadow = new UIView(new CGRect(0, navigationBar.Frame.Height, navigationBar.Frame.Width, height));
                shadow.BackgroundColor = theme.ShadowColor.ToUIColor();

                navigationBar.ShadowImage = null;
                navigationBar.AddSubview(shadow);
            }
            else
            {
                navigationBar.ShadowImage = new UIImage();
            }

            UIStringAttributes attributes = new UIStringAttributes();

            attributes.Font = ViewThemeExtensions.GetFont(theme.FontName, ViewFontSizes.LargeFontSize, FontStyle.Bold);
            attributes.ForegroundColor = theme.FontColor.ToUIColor();

            navigationBar.TitleTextAttributes = attributes;
        }

        private static UIFont GetFont(string fontName, int fontSize, FontStyle fontStyle = FontStyle.None)
        {
            UIFont font = 
                string.IsNullOrEmpty(fontName)
                ? UIFont.SystemFontOfSize(fontSize)
                : UIFont.FromName(fontName, fontSize);

            return font.ApplyStyle(fontStyle);
        }
    }
}