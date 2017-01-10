namespace Mitten.Mobile.Graphics
{
    /// <summary>
    /// Defines a set of standard colors.
    /// </summary>
    public static class Colors
    {
        /// <summary>
        /// A transparent color.
        /// </summary>
        public static Color Transparent
        {
            get { return new Color(0, 0, 0, 0); }
        }

        /// <summary>
        /// A white color.
        /// </summary>
        public static Color White
        {
            get { return new Color(255, 255, 255); }
        }

        /// <summary>
        /// A slightly off-white color.
        /// </summary>
        public static Color Cloud
        {
            get { return new Color(249, 250, 250); }
        }

        /// <summary>
        /// A darker version of the Cloud color.
        /// </summary>
        public static Color StormCloud
        {
            get { return new Color(207, 217, 217); }
        }

        /// <summary>
        /// A black color.
        /// </summary>
        public static Color Black
        {
            get { return new Color(0, 0, 0); }
        }

        /// <summary>
        /// A very light grey color.
        /// </summary>
        public static Color LightLightGrey
        {
            get { return new Color(240, 240, 240); }
        }

        /// <summary>
        /// A light grey color.
        /// </summary>
        public static Color LightGrey
        {
            get { return new Color(215, 215, 215); }
        }

        /// <summary>
        /// A grey color.
        /// </summary>
        public static Color Grey
        {
            get { return new Color(178, 178, 178); }
        }

        /// <summary>
        /// A dark grey.
        /// </summary>
        public static Color DarkGrey
        {
            get { return new Color(116, 116, 116); }
        }

        /// <summary>
        /// A very dark, almost black, grey color.
        /// </summary>
        public static Color DarkDarkGrey
        {
            get { return new Color(52, 52, 52); }
        }
    }
}