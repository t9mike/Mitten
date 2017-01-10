using Foundation;
using UIKit;

namespace Mitten.Mobile.iOS.Views
{
    /// <summary>
    /// Defines standard font sizes for all the views in an application.
    /// </summary>
    public static class ViewFontSizes
    {
        private static class Constants
        {
            public const string ContentSizeCategoryLarge = "UICTContentSizeCategoryL";
            public const string ContentSizeCategoryExtraLarge = "UICTContentSizeCategoryXL";
            public const string ContentSizeCategoryExtraExtraLarge = "UICTContentSizeCategoryXXL";
            public const string ContentSizeCategoryExtraExtraExtraLarge = "UICTContentSizeCategoryXXXL";

            public const string ContentSizeCategoryAccessibilityLarge = "UICTContentSizeCategoryAccessibilityL";
            public const string ContentSizeCategoryAccessibilityExtraLarge = "UICTContentSizeCategoryAccessibilityXL";
            public const string ContentSizeCategoryAccessibilityExtraExtraLarge = "UICTContentSizeCategoryAccessibilityXXL";
            public const string ContentSizeCategoryAccessibilityExtraExtraExtraLarge = "UICTContentSizeCategoryAccessibilityXXXL";
        }

        /// <summary>
        /// Gets the smallest font size supported by the app.
        /// </summary>
        public static int TinyFontSize { get; private set; }

        /// <summary>
        /// Gets a small font size used for sub-categories or footer text.
        /// </summary>
        public static int SmallFontSize { get; private set; }

        /// <summary>
        /// Gets the standard font size.
        /// </summary>
        public static int StandardFontSize { get; private set; }

        /// <summary>
        /// Gets a large font size.
        /// </summary>
        public static int LargeFontSize { get; private set; }

        /// <summary>
        /// Gets a huge font size used for headers.
        /// </summary>
        public static int HugeFontSize { get; private set; }

        /// <summary>
        /// Gets a font size used for very large words.
        /// </summary>
        public static int EnormousFontSize { get; private set; }

        /// <summary>
        /// Gets a slightly larger than enormous font sized and is the largest font size supported by the app.
        /// </summary>
        public static int GinormousFontSize { get; private set; }

        static ViewFontSizes()
        {
            NSNotificationCenter.DefaultCenter.AddObserver(
                UIApplication.ContentSizeCategoryChangedNotification,
                _ => ViewFontSizes.UpdateFontSizes());

            ViewFontSizes.UpdateFontSizes();
        }

        private static void UpdateFontSizes()
        {
            string preferredContentSizeCategory = UIApplication.SharedApplication.PreferredContentSizeCategory.ToString();

            switch (preferredContentSizeCategory)
            {
                case Constants.ContentSizeCategoryExtraLarge:
                case Constants.ContentSizeCategoryAccessibilityExtraLarge:
                    ViewFontSizes.SetLargeFontSizes();
                    break;

                case Constants.ContentSizeCategoryExtraExtraLarge:
                case Constants.ContentSizeCategoryExtraExtraExtraLarge:
                case Constants.ContentSizeCategoryAccessibilityExtraExtraLarge:
                case Constants.ContentSizeCategoryAccessibilityExtraExtraExtraLarge:
                    ViewFontSizes.SetExtraLargeFontSizes();
                    break;

                default:
                    ViewFontSizes.SetNormalFontSizes();
                    break;
            }
        }

        private static void SetNormalFontSizes()
        {
            ViewFontSizes.TinyFontSize = 12;
            ViewFontSizes.SmallFontSize = 13;
            ViewFontSizes.StandardFontSize = 14;
            ViewFontSizes.LargeFontSize = 16;
            ViewFontSizes.HugeFontSize = 18;
            ViewFontSizes.EnormousFontSize = 36;
            ViewFontSizes.GinormousFontSize = 48;
        }

        private static void SetLargeFontSizes()
        {
            ViewFontSizes.TinyFontSize = 13;
            ViewFontSizes.SmallFontSize = 14;
            ViewFontSizes.StandardFontSize = 15;
            ViewFontSizes.LargeFontSize = 17;
            ViewFontSizes.HugeFontSize = 20;
            ViewFontSizes.EnormousFontSize = 36;
            ViewFontSizes.GinormousFontSize = 48;
        }

        private static void SetExtraLargeFontSizes()
        {
            ViewFontSizes.TinyFontSize = 14;
            ViewFontSizes.SmallFontSize = 15;
            ViewFontSizes.StandardFontSize = 17;
            ViewFontSizes.LargeFontSize = 20;
            ViewFontSizes.HugeFontSize = 24;
            ViewFontSizes.EnormousFontSize = 40;
            ViewFontSizes.GinormousFontSize = 52;
        }
    }
}