using System;

namespace Mitten.Mobile.Graphics
{
    /// <summary>
    /// Represents a color in RGB.
    /// </summary>
    public struct Color
    {
        /// <summary>
        /// Initializes a new instance of the Color class.
        /// </summary>
        /// <param name="red">The red component of the color.</param>
        /// <param name="green">The green component of the color.</param>
        /// <param name="blue">The blue component of the color.</param>
        /// <param name="alpha">The opacity value of the color.</param>
        public Color(byte red, byte green, byte blue, byte alpha = 255)
        {
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
            this.Alpha = alpha;
        }

        /// <summary>
        /// Gets the opacity value of the color.
        /// </summary>
        public byte Alpha { get; private set; }

        /// <summary>
        /// Gets the red component of the color.
        /// </summary>
        public byte Red { get; private set; }

        /// <summary>
        /// Gets the green component of the color.
        /// </summary>
        public byte Green { get; private set; }

        /// <summary>
        /// Gets the blue component of the color.
        /// </summary>
        public byte Blue { get; private set; }

        /// <summary>
        /// Gets the hue as a value from 0.0 - 1.0f;
        /// </summary>
        /// <returns>A hue.</returns>
        public float GetHue()
        {
            float red = this.Red / 255.0f;
            float green = this.Green / 255.0f;
            float blue = this.Blue / 255.0f;

            float minimum = Math.Min(Math.Min(red, green), blue);
            float maximum = Math.Max(Math.Max(red, green), blue);
            float deltaMaximum = maximum - minimum;
            float hue = 0.0f;

            if (deltaMaximum > 0.0f)
            {
                Func<float, float> calculateDelta = color => (((maximum - color) / 6.0f) + (deltaMaximum / 2.0f)) / deltaMaximum;
                float deltaRed = calculateDelta(red);
                float deltaGreen = calculateDelta(green);
                float deltaBlue = calculateDelta(blue);

                if (red == maximum)
                {
                    hue = deltaBlue - deltaGreen;
                }
                else if (green == maximum)
                {
                    hue = (1.0f / 3.0f) + deltaRed - deltaBlue;
                }
                else if (blue == maximum)
                {
                    hue = (2.0f / 3.0f) + deltaGreen - deltaRed;
                }

                if (hue < 0.0f)
                {
                    hue += 1.0f;
                }
                else if (hue > 1.0f)
                {
                    hue -= 1.0f;
                }
            }

            return hue;
        }

        /// <summary>
        /// Gets the saturation as a value from 0.0 - 1.0f;
        /// </summary>
        /// <returns>The saturation.</returns>
        public float GetSaturation()
        {
            float red = this.Red / 255.0f;
            float green = this.Green / 255.0f;
            float blue = this.Blue / 255.0f;

            float minimum = Math.Min(Math.Min(red, green), blue);
            float maximum = Math.Max(Math.Max(red, green), blue);
            float deltaMaximum = maximum - minimum;

            return
                deltaMaximum > 0.0f
                ? deltaMaximum / maximum
                : 0.0f;
        }

        /// <summary>
        /// Gets the brightness as a value from 0.0 - 1.0f;
        /// </summary>
        /// <returns>The brightness.</returns>
        public float GetBrightness()
        {
            float red = this.Red / 255.0f;
            float green = this.Green / 255.0f;
            float blue = this.Blue / 255.0f;

            return Math.Max(Math.Max(red, green), blue);
        }

        /// <summary>
        /// Determines if the current instance is equal to the given object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if the this instance is considered equal to the given object, otherwise false.</returns>
        public override bool Equals(object obj) 
        {
            return 
                obj is Color && 
                this == (Color)obj;
        }

        /// <summary>
        /// Gets a hash code for the current Color.
        /// </summary>
        /// <returns>A hash code identifying the current Color.</returns>
        public override int GetHashCode() 
        {
            return 
                BitConverter.ToInt32(
                    new [] { this.Alpha, this.Blue, this.Green, this.Red },
                    0);
        }

        /// <summary>
        /// Gets the current color with the specified alpha value.
        /// </summary>
        /// <param name="alpha">The alpha value for the color.</param>
        /// <returns>A new color based on the current color but with an updated Alpha.</returns>
        public Color WithAlpha(byte alpha)
        {
            return
                new Color(
                    this.Red,
                    this.Green,
                    this.Blue,
                    alpha);
        }

        public static bool operator ==(Color lhs, Color rhs) 
        {
            return 
                lhs.Red == rhs.Red &&
                lhs.Green == rhs.Green &&
                lhs.Blue == rhs.Blue &&
                lhs.Alpha == rhs.Alpha;
        }

        public static bool operator !=(Color lhs, Color rhs) 
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Creates a new color instance from a given hue, saturation, and brightness.
        /// </summary>
        /// <param name="hue">The hue as a value from 0.0 - 1.0f.</param>
        /// <param name="saturation">The saturation as a value from 0.0 - 1.0f.</param>
        /// <param name="brightness">The brightness as a value from 0.0 - 1.0f.</param>
        /// <returns>A new color instance.</returns>
        public static Color FromHSB(float hue, float saturation, float brightness)
        {
            // The logic for converting to and from HSB was taken from here:
            // http://www.easyrgb.com/index.php?X=MATH&H=21#text21
            // http://www.easyrgb.com/index.php?X=MATH&H=20#text20

            byte red = 0;
            byte green = 0;
            byte blue = 0;

            if (saturation == 0.0f)
            {
                red = Convert.ToByte(brightness * 255);
                green = Convert.ToByte(brightness * 255);
                blue = Convert.ToByte(brightness * 255);
            }
            else
            {
                float sextant = hue * 6.0f;
                if (sextant == 6.0f)
                {
                    sextant = 0.0f;
                }

                int sector = (int)Math.Floor(sextant);
                float fractionalPart = sextant - sector;

                float var1 = brightness * (1.0f - saturation);
                float var2 = brightness * (1.0f - saturation * fractionalPart);
                float var3 = brightness * (1.0f - saturation * (1.0f - fractionalPart));

                switch (sector)
                {
                    case 0:
                        red = Convert.ToByte(brightness * 255);
                        green = Convert.ToByte(var3 * 255);
                        blue = Convert.ToByte(var1 * 255);
                        break;

                    case 1:
                        red = Convert.ToByte(var2 * 255);
                        green = Convert.ToByte(brightness * 255);
                        blue = Convert.ToByte(var1 * 255);
                        break;

                    case 2:
                        red = Convert.ToByte(var1 * 255);
                        green = Convert.ToByte(brightness * 255);
                        blue = Convert.ToByte(var3 * 255);
                        break;

                    case 3:
                        red = Convert.ToByte(var1 * 255);
                        green = Convert.ToByte(var2 * 255);
                        blue = Convert.ToByte(brightness * 255);
                        break;

                    case 4:
                        red = Convert.ToByte(var3 * 255);
                        green = Convert.ToByte(var1 * 255);
                        blue = Convert.ToByte(brightness * 255);
                        break;

                    case 5:
                        red = Convert.ToByte(brightness * 255);
                        green = Convert.ToByte(var1 * 255);
                        blue = Convert.ToByte(var2 * 255);
                        break;

                    default:
                        throw new InvalidOperationException("Unexpected sector (" + sector + ").");
                }
            }

            return new Color(red, green, blue);
        }

        private static void CheckRange(float value, string argumentName)
        {
            if (value < 0.0f || value > 1.0f)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }
    }
}