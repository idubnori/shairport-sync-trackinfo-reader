using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Microsoft.Reactive.Testing;
using ShairportSync.Metadata.Models;

namespace TrackInfoReader.Tests
{
    internal static class HelperExtensions
    {
        public static bool IsDefaultArtwork(this TrackInfo actual)
        {
            return actual.Artwork == "default.png"; // TODO 
        }

        public static bool IsArtWork(this TrackInfo actual)
        {
            return actual is Artwork && !string.IsNullOrEmpty(actual.Artwork);
        }

        public static bool IsSameSong(this TrackInfo actual, TrackInfo expected)
        {
            return expected.Artist == actual.Artist &&
                   expected.Album == actual.Album &&
                   expected.Song == actual.Song;
        }

        public static async Task WriteMetadataAsync(this Stream pipeStream, string metadataFilePath)
        {
            var allLines = File.ReadAllLines(metadataFilePath);

            using (var streamWriter = new StreamWriter(pipeStream))
            {
                streamWriter.AutoFlush = true;

                foreach (var line in allLines)
                {
                    await streamWriter.WriteLineAsync(line);
                }
            }

            await Task.Delay(1000); // TODO
        }

        public static string DebugInfo<T>(this IList<Recorded<Notification<T>>> messages, Func<T, string> selector)
        {
            return messages
                .Where(m => m.Value.HasValue)
                .Select(m => m.Value.Value != null ? selector(m.Value.Value) : null)
                .Aggregate(Environment.NewLine, (f, n) => f + Environment.NewLine + n);
        }

        public static string DebugInfo(this TrackInfo t)
        {
            return $"{t.PlaybackDateTime.ToLongTimeString()} <{t.GetType().Name}> " +
                   $"[{t.Artist} / {t.Album} / {t.Song} / {t.Artwork}] {t.Mper}";
        }
    }
}