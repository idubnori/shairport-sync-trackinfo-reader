using System;

namespace ShairportSync.Metadata.Models
{
    /// <summary>
    /// Track information which parsed shairport-sync metadata.
    /// </summary>
    public class TrackInfo
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public TrackInfo()
        {
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source">Copy source.</param>
        public TrackInfo(TrackInfo source)
        {
            PlaybackDateTime = source.PlaybackDateTime;
            Mper = source.Mper;
            Artist = source.Artist;
            Album = source.Album;
            Song = source.Song;
            Artwork = source.Artwork;
        }

        /// <summary>
        /// Track Playback DateTime. 
        /// </summary>
        public DateTime PlaybackDateTime { get; set; }

        /// <summary>
        /// DMAP Persistent Id.
        /// </summary>
        public string Mper { get; set; }

        /// <summary>
        /// Artist Name.
        /// </summary>
        public string Artist { get; set; }

        /// <summary>
        /// Album Name.
        /// </summary>
        public string Album { get; set; }

        /// <summary>
        /// Song name.
        /// </summary>
        public string Song { get; set; }

        /// <summary>
        /// Artwork Image File Name. (as temporal)
        /// </summary>
        public string Artwork { get; set; }
    }
}