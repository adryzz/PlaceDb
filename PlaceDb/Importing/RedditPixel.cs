using CsvHelper.Configuration.Attributes;

namespace PlaceDb.Importing
{
    public struct RedditPixel
    {
        [Name("timestamp")]
        public string Timestamp { get; set; }
    
        [Name("user_id")]
        public string Uid { get; set; }
    
        [Name("pixel_color")]
        public string Color { get; set; }
    
        [Name("coordinate")]
        public string Coordinates { get; set; }
    }
}