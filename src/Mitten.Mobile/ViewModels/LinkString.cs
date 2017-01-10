using System;
using System.Collections.Generic;

namespace Mitten.Mobile.ViewModels
{
    /// <summary>
    /// Defines a string made up of parts containing optional links to additional information.
    /// </summary>
    public class LinkString
    {
        /// <summary>
        /// Initializes a new instance of the LinkString class.
        /// </summary>
        public LinkString()
        {
            this.Parts = new Part[0];
        }

        private LinkString(IEnumerable<Part> parts, Part part)
        {
            List<Part> list = new List<Part>(parts);
            list.Add(part);

            this.Parts = list;
        }

        /// <summary>
        /// Gets a list of parts.
        /// </summary>
        public IEnumerable<Part> Parts { get; private set; }

        /// <summary>
        /// Append the given text to the current link string.
        /// </summary>
        /// <param name="text">The text to append.</param>
        /// <param name="linkHandler">An optional callback meant for handling the specified text.</param>
        public LinkString Append(string text, Action linkHandler = null)
        {
            return new LinkString(this.Parts, new Part(text, linkHandler));
        }

        /// <summary>
        /// Defines a text portion of a link string.
        /// </summary>
        public class Part
        {
            private readonly Action linkHandler;

            internal Part(string text, Action linkHandler)
            {
                this.Text = text;
                this.linkHandler = linkHandler;
            }

            /// <summary>
            /// Gets whether or not a link handler is registered for this part.
            /// </summary>
            public bool HasHandler
            {
                get { return this.linkHandler != null; }
            }

            /// <summary>
            /// Gets the text that represents this part.
            /// </summary>
            public string Text { get; private set; }

            public void HandleLink()
            {
                if (this.linkHandler != null)
                {
                    this.linkHandler();
                }
            }
        }
    }
}