using Mitten.Mobile.Graphics;

namespace Mitten.Mobile.Themes
{
    /// <summary>
    /// Defines a theme for a segmented control.
    /// </summary>
    public class SegmentedControlTheme
    {
        /// <summary>
        /// Initializes a new instance of the SegmentedControlTheme class.
        /// </summary>
        /// <param name="backgroundColor">The background color for the segment control.</param>
        /// <param name="labelColor">The color for a label.</param>
        /// <param name="selectedSegmentForeColor">The fore color for a selected segment.</param>
        /// <param name="fontName">The name of the font for the segmented control labels or null to use the default system's font.</param>
        public SegmentedControlTheme(
            Color backgroundColor,
            Color labelColor,
            Color selectedSegmentForeColor,
            string fontName = null)
        {
            this.BackgroundColor = backgroundColor;
            this.LabelColor = labelColor;
            this.SelectedSegmentForeColor = selectedSegmentForeColor;
            this.FontName = fontName;
        }

        /// <summary>
        /// Gets the background color for the segment control.
        /// </summary>
        public Color BackgroundColor { get; private set; }

        /// <summary>
        /// Gets the color for a label.
        /// </summary>
        public Color LabelColor { get; private set; }

        /// <summary>
        /// Gets the fore color for a selected segment.
        /// </summary>
        public Color SelectedSegmentForeColor { get; private set; }

        /// <summary>
        /// Gets the name of the font for the segmented control labels.
        /// </summary>
        public string FontName { get; private set; }
    }
}