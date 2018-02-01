using ShairportSync.Metadata.Interfaces;

namespace ShairportSync.Metadata.Settings
{
    internal class DefaultArtworkSettings : IArtworkSettings
    {
        public string ArtworkRootPath { get; } = "./artworkImages/";
        public string DefaultArtwork { get; } = "default.png"; // TODO
    }
}