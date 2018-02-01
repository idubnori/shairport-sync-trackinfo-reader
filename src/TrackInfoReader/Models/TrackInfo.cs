using System;

namespace ShairportSync.Metadata.Models
{
    public class TrackInfo
    {
        public TrackInfo()
        {
        }

        public TrackInfo(TrackInfo source)
        {
            PlaybackDateTime = source.PlaybackDateTime;
            Mper = source.Mper;
            Artist = source.Artist;
            Album = source.Album;
            Song = source.Song;
            Artwork = source.Artwork;
        }

        public DateTime PlaybackDateTime { get; set; }
        public string Mper { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Song { get; set; }
        public string Artwork { get; set; }
    }
}