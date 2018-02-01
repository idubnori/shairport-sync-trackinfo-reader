using ShairportSync.Metadata.Models;

namespace ConsoleViewer
{
    public static class TrackExtensions
    {
        public static string DebugInfo(this TrackInfo t)
        {
            return $"{t.PlaybackDateTime.ToLongTimeString()} <{t.GetType().Name}> " +
                   $"[{t.Artist} / {t.Album} / {t.Song} / {t.Artwork}] {t.Mper}";
        }
    }
}