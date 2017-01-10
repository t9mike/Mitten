using System;

namespace Mitten.Mobile.Themes
{
    /// <summary>
    /// Defines one or more styles for a font.
    /// </summary>
    [Flags]
    public enum FontStyle
    {
        /// <summary>
        /// Identifies no additional font styling.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Identifies a bold font styling.
        /// </summary>
        Bold = 0x01,

        /// <summary>
        /// Identifies an italic font styling.
        /// </summary>
        Italic = 0x02,
    }
}