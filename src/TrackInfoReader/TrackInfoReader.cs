using System;
using System.IO;
using ShairportSync.Metadata.Models;
using ShairportSync.Metadata.Parsers;

namespace ShairportSync.Metadata
{
    /// <summary>
    /// Represents a reader that decodes shairport-sync metadata and parse it as track information.
    /// </summary>
    public class TrackInfoReader
    {
        /// <summary>
        /// Get track information from shairport-sync metadata pipe observable sequences.
        /// (Note that currently, artwork images are to serialize in "./artworkImages".)
        /// </summary>
        /// <param name="metadataPipePath">shairport-sync metadata pipe file path.</param>
        /// <returns></returns>
        public static IObservable<TrackInfo> GetObservable(string metadataPipePath)
        {
            var itemSource = MetadataDecoder.GetObservable(metadataPipePath);
            var trackSource = TrackInfoParser.GetObservable(itemSource);
            return trackSource;
        }

        /// <summary>
        /// Get track information from shairport-sync metadata stream observable sequences.
        /// (Note that currently, artwork images are to serialize in "./artworkImages".)
        /// </summary>
        /// <param name="stream">shairport-sync metadata stream.</param>
        /// <returns></returns>
        public static IObservable<TrackInfo> GetObservable(Stream stream)
        {
            var itemSource = MetadataDecoder.GetObservable(stream);
            var trackObservable = TrackInfoParser.GetObservable(itemSource);
            return trackObservable;
        }
    }
}