using MongoDB.Bson;
using Realms;
using Realms.Schema;
using Realms.Weaving;

namespace PlaceDb.Types
{
    public class RealmPixel : RealmObject
    {
        [PrimaryKey] public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        public DateTimeOffset Timestamp { get; set; }
        
        [Indexed] public int X { get; set; }
        
        [Indexed] public int Y { get; set; }
        
        [Indexed] public int Color { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public RealmPixel()
        {
        }

        public RealmPixel(Pixel p)
        {
            Timestamp = p.Timestamp;
            unchecked
            {
                X = (int)p.X;
                Y = (int)p.Y;
                Width = (int)p.Width;
                Height = (int)p.Height;
                Color = (int)p.Color;
            }
        }
    }
}