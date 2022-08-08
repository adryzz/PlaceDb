using System.Runtime;
using CommandLine;
using PlaceDb.CommandLine;
using PlaceDb.Importing;
using PlaceDb.Types;
using Realms;

namespace PlaceDb
{
    public static class Program
    {
        static int count = 0;
        private static int oldCount = 0;
        public static async Task Main(string[] args)
        {
            GCSettings.LatencyMode = GCLatencyMode.Batch; //more throughput
            
            var result = Parser.Default.ParseArguments<ImportOptions, QueryOptions>(args);

            switch (result.Value)
            {
                case ImportOptions i:
                    await importToRealmAsync(i.Path);
                    break;
                case QueryOptions q:
                    break;
            }
            
        }

        static async Task importToRealmAsync(string path)
        {
            IEnumerable<string> files = Directory.EnumerateFiles(path, "*.csv.gzip");

            Timer t = new Timer(_callback, null, 2000, 2000);

            await foreach (Pixel p in RedditArchiveImporter.Import(files))
            {
                count++;
            }

            /*Realm realm = await Realm.GetInstanceAsync(new RealmConfiguration("my.realm"));

            IAsyncEnumerator<Pixel> enumerator = RedditArchiveImporter.Import(files).GetAsyncEnumerator();

            while (await enumerator.MoveNextAsync())
            {
                await realm.WriteAsync(async () =>
                {
                    for (int i = 0; i < 15000; i++) // split the write in multiple chunks
                    {
                        realm.Add(new RealmPixel(enumerator.Current));
                        count++;
                    }
                });
            }*/
            await t.DisposeAsync();
        }

        private static void _callback(object? state)
        {
            Console.WriteLine($"Importing... {(count - oldCount) / 2f} pixels/s | {count} total.");
            oldCount = count;
        }
    }
}