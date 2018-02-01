using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using ShairportSync.Metadata.Models;
using ShairportSync.Metadata.Resolvers;
using ShairportSync.Metadata.Utilities;

namespace ShairportSync.Metadata.Parsers
{
    internal static class TrackInfoParser
    {
        public static IObservable<TrackInfo> GetObservable(IObservable<Item> itemSource)
        {
            var publishItemSource = itemSource.SubscribeOn(TaskPoolScheduler.Default).Publish().RefCount(); // TODO SubscribeOn/Publish/RefCount.
            var trackSource = publishItemSource.Parse();
            return trackSource;
        }

        private static IObservable<TrackInfo> Parse(this IObservable<Item> itemSource)
        {
            var artworkMetadataSource = itemSource.Where(m => m.Code == "PICT");

            var bufferClosing = itemSource.Where(m => m.Code == "mden");
            var trackSource = itemSource
                .Buffer(() => bufferClosing)
                .Select(items => items.ToTrack());

            var mperChangesTrackSource = trackSource
                .PairWithPrevious()
                .Where(t => t.Item1?.Mper != t.Item2.Mper && t.Item2.Mper != null)
                .Select(t => t.Item2);

            var artworkTrackSource = artworkMetadataSource
                .WithLatestFrom(trackSource, (i, h) => new Artwork(h) { Artwork = i.Data });

            return mperChangesTrackSource.Merge(artworkTrackSource).ToWithArtwork();
        }

        private static TrackInfo ToTrack(this IList<Item> items)
        {
            var trackInfo = new TrackInfo
            {
                PlaybackDateTime = DateTime.Now, // TODO an approximate value.
                Mper = items.SingleOrDefault(m => m.Code == "mper")?.Data,
                Artist = items.SingleOrDefault(m => m.Code == "asar")?.Data,
                Album = items.SingleOrDefault(m => m.Code == "asal")?.Data,
                Song = items.SingleOrDefault(m => m.Code == "minm")?.Data,
                Artwork = /*items.SingleOrDefault(m => m.Code == "PICT")?.Data ??*/ TrackInfoReaderResolver.ArtworkSettings.DefaultArtwork,
            };
            return trackInfo;
        }

        private static IObservable<TrackInfo> ToWithArtwork(this IObservable<TrackInfo> trackSource)
        {
            var artworkSource = trackSource.Where(t => t is Artwork);

            var trackWithArtworkSource = trackSource
                .WithLatestFrom(artworkSource.StartWith(default(Artwork)), (t, a) => t.UpdateArtwork((Artwork)a));

            return trackWithArtworkSource;
        }

        private static TrackInfo UpdateArtwork(this TrackInfo trackInfo, Artwork artwork)
        {
            if (trackInfo.IsSameArtwork(artwork))
                trackInfo.Artwork = artwork.Artwork;

            return trackInfo;
        }

        private static bool IsSameArtwork(this TrackInfo trackInfo, Artwork artwork)
        {
            if (artwork == null)
                return false;

            if (trackInfo.Mper == artwork.Mper)
                return true;

            if (string.IsNullOrEmpty(trackInfo.Artist) || string.IsNullOrEmpty(trackInfo.Album))
                return false;

            if (trackInfo.Artist == artwork.Artist && trackInfo.Album == artwork.Album)
                return true;

            return false;
        }
    }
}