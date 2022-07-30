using System;

namespace NesEmulator
{
    public struct Color : IEquatable<Color>
    {
        public Color(int red, int green, int blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Color color && Equals(color);
        }

        public bool Equals(Color other)
        {
            return Red == other.Red &&
                   Green == other.Green &&
                   Blue == other.Blue;
        }

        public override int GetHashCode()
        {
            int hashCode = -1058441243;
            hashCode = hashCode * -1521134295 + Red.GetHashCode();
            hashCode = hashCode * -1521134295 + Green.GetHashCode();
            hashCode = hashCode * -1521134295 + Blue.GetHashCode();
            return hashCode;
        }
    }
}