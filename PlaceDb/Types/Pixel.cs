using NodaTime;
using Realms;

namespace PlaceDb.Types
{
    public readonly struct Pixel
    {
        public Pixel(DateTime timestamp, Color color, uint x, uint y, uint width = 0, uint height = 0)
        {
            X = x;
            Y = y;
            Color = color;
            Timestamp = timestamp;
            Width = width;
            Height = height;
        }

        public readonly uint X;
        public readonly uint Y;
        public readonly uint Width;
        public readonly uint Height;
        public readonly Color Color;
        public readonly DateTime Timestamp;
    }
}