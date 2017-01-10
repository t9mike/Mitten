namespace Mitten.Mobile.iOS.Views
{
    /// <summary>
    /// Represents the margins for each side of a rectangle.
    /// </summary>
    public struct Margins
    {
        /// <summary>
        /// Represents an empty set of margins.
        /// </summary>
        public static readonly Margins Empty = new Margins();

        /// <summary>
        /// Initializes a new instance of the <see cref="Margins"/> struct.
        /// </summary>
        /// <param name="margin">The margin for all sides.</param>
        public Margins(int margin)
            : this(margin, margin, margin, margin)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Margins"/> struct.
        /// </summary>
        /// <param name="horizontalMargin">A value for the top and bottom.</param>
        /// <param name="verticalMargin">A value for the trailing and leading.</param>
        public Margins(int horizontalMargin, int verticalMargin)
            : this(horizontalMargin, horizontalMargin, verticalMargin, verticalMargin)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Margins"/> struct.
        /// </summary>
        /// <param name="top">A value for the top.</param>
        /// <param name="bottom">A value for the bottom.</param>
        /// <param name="trailing">A value for the trailing.</param>
        /// <param name="leading">A value for the leading.</param>
        public Margins(int top, int bottom, int trailing, int leading)
        {
            this.Top = top;
            this.Bottom = bottom;
            this.Trailing = trailing;
            this.Leading = leading;
        }

        /// <summary>
        /// Gets the top margin.
        /// </summary>
        public int Top { get; private set; }

        /// <summary>
        /// Gets the bottom margin.
        /// </summary>
        public int Bottom { get; private set; }

        /// <summary>
        /// Gets the trailing margin.
        /// </summary>
        public int Trailing { get; private set; }

        /// <summary>
        /// Gets the leading margin.
        /// </summary>
        public int Leading { get; private set; }

        /// <summary>
        /// Gets an updated Margins with the specified top.
        /// </summary>
        /// <param name="value">The new value.</param>
        /// <returns>An updated Margins.</returns>
        public Margins WithTop(int value)
        {
            return new Margins(value, this.Bottom, this.Trailing, this.Leading);
        }

        /// <summary>
        /// Gets an updated Margins with the specified bottom.
        /// </summary>
        /// <param name="value">The new value.</param>
        /// <returns>An updated Margins.</returns>
        public Margins WithBottom(int value)
        {
            return new Margins(this.Top, value, this.Trailing, this.Leading);
        }

        /// <summary>
        /// Gets an updated Margins with the specified trailing.
        /// </summary>
        /// <param name="value">The new value.</param>
        /// <returns>An updated Margins.</returns>
        public Margins WithTrailing(int value)
        {
            return new Margins(this.Top, this.Bottom, value, this.Leading);
        }

        /// <summary>
        /// Gets an updated Margins with the specified leading.
        /// </summary>
        /// <param name="value">The new value.</param>
        /// <returns>An updated Margins.</returns>
        public Margins WithLeading(int value)
        {
            return new Margins(this.Top, this.Bottom, this.Trailing, value);
        }

        /// <summary>
        /// Gets an updated Margins with the specified leading and trailing margins.
        /// </summary>
        /// <param name="value">The new value.</param>
        /// <returns>An updated Margins.</returns>
        public Margins WithHorizontal(int value)
        {
            return new Margins(this.Top, this.Bottom, value, value);
        }

        /// <summary>
        /// Gets an updated Margins with the specified top and bottom margins.
        /// </summary>
        /// <param name="value">The new value.</param>
        /// <returns>An updated Margins.</returns>
        public Margins WithVertical(int value)
        {
            return new Margins(value, value, this.Trailing, this.Leading);
        }

        /// <summary>
        /// Creates a new Margins with the specified top.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A new Margins.</returns>
        public static Margins FromTop(int value)
        {
            return new Margins(value, 0, 0, 0);
        }

        /// <summary>
        /// Creates a new Margins with the specified top.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A new Margins.</returns>
        public static Margins FromBottom(int value)
        {
            return new Margins(0, value, 0, 0);
        }

        /// <summary>
        /// Creates a new Margins with the specified top.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A new Margins.</returns>
        public static Margins FromTrailing(int value)
        {
            return new Margins(0, 0, value, 0);
        }

        /// <summary>
        /// Creates a new Margins with the specified top.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A new Margins.</returns>
        public static Margins FromLeading(int value)
        {
            return new Margins(0, 0, 0, value);
        }

        /// <summary>
        /// Creates a new Margins with the specified horizontal value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A new Margins.</returns>
        public static Margins FromHorizontal(int value)
        {
            return new Margins(0, 0, value, value);
        }

        /// <summary>
        /// Creates a new Margins with the specified vertical value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A new Margins.</returns>
        public static Margins FromVertical(int value)
        {
            return new Margins(value, value, 0, 0);
        }
    }
}