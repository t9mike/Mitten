using System;

namespace Mitten.Mobile.Graphics
{
    /// <summary>
    /// Utility class that provides methods for generating colors and color palettes.
    /// </summary>
    public static class ColorGenerator
    {
        private static class Constants
        {
            public const int GoldenRatioSeed = 2363;
            public const double GoldenRatioConjugate = 0.618033988749895;
            public const float Saturation = 0.75f;
            public const float Brightness = 0.85f;
        }

        /// <summary>
        /// Generates a new color using the golden ratio algorithm and returns the color at the specified palette index.
        /// </summary>
        /// <returns>A new color.</returns>
        public static Color FromPalette(int paletteIndex)
        {
            Random rand = new Random(Constants.GoldenRatioSeed);
            float offset = (float)rand.NextDouble();

            return ColorGenerator.FromHue((float)((offset + (Constants.GoldenRatioConjugate * paletteIndex)) % 1));
        }

        /// <summary>
        /// Generates a new color from the specified hue and using a standard saturation and brightness.
        /// </summary>
        /// <param name="hue">A value from 0.0 - 1.0 identifying a hue.</param>
        /// <returns>A new color.</returns>
        public static Color FromHue(float hue)
        {
            if (hue < 0.0f && hue > 1.0f)
            {
                throw new ArgumentOutOfRangeException(nameof(hue));
            }

            return Color.FromHSB(hue, Constants.Saturation, Constants.Brightness);
        }
    }
}