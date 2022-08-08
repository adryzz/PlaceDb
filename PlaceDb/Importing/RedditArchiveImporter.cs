using System.Drawing;
using System.Globalization;
using System.IO.Compression;
using System.Text.RegularExpressions;
using CsvHelper;
using NodaTime;
using NodaTime.Text;
using PlaceDb.Types;
using Color = PlaceDb.Types.Color;

namespace PlaceDb.Importing
{
    public class RedditArchiveImporter
    {
        public static async IAsyncEnumerable<Pixel> Import(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                Console.WriteLine($"Loading {file}...");
                using(FileStream fs = File.OpenRead(file))
                {
                    using(BufferedStream buf = new BufferedStream(fs))
                    {
                        using(GZipStream gzip = new GZipStream(buf, CompressionMode.Decompress))
                        {
                            using(var reader = new StreamReader(gzip))
                            {
                                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                                {
                                    var records = csv.GetRecords<RedditPixel>();

                                    foreach (RedditPixel rpix in records)
                                    {
                                        yield return ConvertPixel(rpix);;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
        private static Pixel ConvertPixel(RedditPixel pix)
        {
            int comma = pix.Coordinates.IndexOf(',');
            int nextComma = pix.Coordinates.IndexOf(',', comma + 1);

            if (nextComma == -1) // doesn't have width/height
            {
                return new Pixel(ConvertTimestampFast(pix.Timestamp), ConvertColor(pix.Color),
                    uint.Parse(pix.Coordinates[..comma]), uint.Parse(pix.Coordinates[(comma + 1)..]));
            }
            else
            {
                int lastComma = pix.Coordinates.IndexOf(',', nextComma + 1);
                return new Pixel(ConvertTimestampFast(pix.Timestamp), ConvertColor(pix.Color),
                    uint.Parse(pix.Coordinates[..comma]), uint.Parse(pix.Coordinates[(comma + 1)..nextComma]),
                    uint.Parse(pix.Coordinates[(nextComma + 1)..lastComma]),
                    uint.Parse(pix.Coordinates[(lastComma + 1)..]));
            }
        }

        private static DateTime ConvertTimestamp(string timestamp)
        {
            MatchCollection m = dateTime.Matches(timestamp);
            return new DateTime(int.Parse(m[0].Value), int.Parse(m[1].Value), 
                int.Parse(m[2].Value), int.Parse(m[3].Value),
                int.Parse(m[4].Value), int.Parse(m[5].Value),
                m.Count > 6 ? int.Parse(m[6].Value) : 0, DateTimeKind.Utc);
        }
        
        private static DateTime ConvertTimestampFast(string timestamp)
        {
            ReadOnlySpan<char> time = timestamp;
            
            int[] components = new int[7];
            int current = 0;

            int multiplier = 10000;
            for (int i = 0; i < time.Length; i++)
            {
                if (char.IsDigit(time[i]))
                {
                    components[current] += time[i].ToInt() * multiplier;
                    multiplier /= 10;
                }
                else
                {
                    if (current >= components.Length)
                    {
                        return new DateTime(components[0], components[1],
                            components[2], components[3], components[4],
                            components[5], components[6], DateTimeKind.Utc);
                    }
                    components[current] /= (multiplier * 10);
                    current++;
                    multiplier = 1000;
                }
            }
            return DateTime.MaxValue;
        }

        private static Color ConvertColor(string color)
        {
            return (Color)int.Parse(color[1..], NumberStyles.HexNumber);
        }

        private static Regex dateTime = new Regex("\\d+", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    }
}