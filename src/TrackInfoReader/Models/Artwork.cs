namespace ShairportSync.Metadata.Models
{
    /// <summary>
    /// Artwork instance creates by reading each artwork image metadata item.
    /// </summary>
    public class Artwork : TrackInfo
    {
        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="trackInfo">Source track information.</param>
        public Artwork(TrackInfo trackInfo) : base(trackInfo) { }
    }
}