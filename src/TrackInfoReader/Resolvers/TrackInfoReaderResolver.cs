using ShairportSync.Metadata.Interfaces;
using ShairportSync.Metadata.Readers;
using ShairportSync.Metadata.Settings;

namespace ShairportSync.Metadata.Resolvers
{
    internal class TrackInfoReaderResolver
    {
        public static IArtworkSettings ArtworkSettings { get; set; } = new DefaultArtworkSettings();
        public static IArtworkReader ArtworkReader { get; set; } = new DefaultArtworkReader();
    }
}