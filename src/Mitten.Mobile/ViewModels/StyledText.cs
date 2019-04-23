using System.Collections.Generic;
using System.Text;
using System;

namespace Mitten.Mobile.ViewModels
{
    /// <summary>
    /// Represents a text string with basic styling.
    /// </summary>
    public class StyledText
    {
        /// <summary>
        /// Defines the types of styles supported.
        /// </summary>
        [Flags]
        public enum Style
        {
            /// <summary>
            /// Represents no additional style.
            /// </summary>
            None = 0,

            /// <summary>
            /// Represents a bold styling.
            /// </summary>
            Bold = 1,

            Italic = 2,

            Underline = 4
        }

        private readonly List<TextPart> parts;
        private int nextStartIndex;

        /// <summary>
        /// Initializes a new instance of the StyledText class.
        /// </summary>
        public StyledText()
        {
            this.parts = new List<TextPart>();
        }

        /// <summary>
        /// Gets a list of text parts.
        /// </summary>
        public IEnumerable<TextPart> Parts
        {
            get { return this.parts; }
        }

        /// <summary>
        /// Gets the styled text as a full string.
        /// </summary>
        /// <returns>A string representing the styled text.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (TextPart part in this.parts)
            {
                sb.Append(part.Text);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Appends the specified text with an optional style.
        /// </summary>
        /// <param name="text">The text to append.</param>
        /// <param name="style">An optional style for the text.</param>
        public StyledText Append(string text, Style style = Style.None)
        {
            Throw.IfArgumentNull(text, "text");

            this.parts.Add(new TextPart(text, style, this.nextStartIndex));
            this.nextStartIndex += text.Length;

            return this;
        }

        /// <summary>
        /// Represents a styled text part within the overall text string.
        /// </summary>
        public class TextPart
        {
            /// <summary>
            /// Initializes a new instance of the TextPart class.
            /// </summary>
            /// <param name="text">The text for this part.</param>
            /// <param name="style">The style for this part.</param>
            /// <param name="startIndex">The start index for this part in the overall styled text.</param>
            internal TextPart(string text, Style style, int startIndex)
            {
                this.Text = text;
                this.Style = style;
                this.StartIndex = startIndex;
            }

            /// <summary>
            /// Gets the text for this part.
            /// </summary>
            public string Text { get; private set; }

            /// <summary>
            /// Gets the style for this part.
            /// </summary>
            public Style Style { get; private set; }

            /// <summary>
            /// Gets the start index for this part in the overall styled text.
            /// </summary>
            public int StartIndex { get; private set; }
        }
    }
}