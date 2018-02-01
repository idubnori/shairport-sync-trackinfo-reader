using System;
using System.Linq;
using ShairportSync.Metadata;

namespace ConsoleViewer
{
    class Program
    {
        static void Main(string[] args)
        {
            var metadataPipePath = args.Any() ? args[0] : "/tmp/shairport-sync-metadata";
            Console.WriteLine($"MetadataPipePath-> {metadataPipePath}");

            var trackSource = TrackInfoReader.GetObservable(metadataPipePath);
            trackSource.Subscribe(h => Console.WriteLine(h.DebugInfo()));

            Console.WriteLine("Started subscribing track information.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }
    }
}
