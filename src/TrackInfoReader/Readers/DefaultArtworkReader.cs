using System;
using System.IO;
using System.Xml;
using ShairportSync.Metadata.Interfaces;
using ShairportSync.Metadata.Resolvers;

namespace ShairportSync.Metadata.Readers
{
    internal class DefaultArtworkReader : IArtworkReader
    {
        private static readonly int BufferSize = 8192;

        public string ReadArtwork(XmlReader reader, int length)
        {
            if (length <= 0) return TrackInfoReaderResolver.ArtworkSettings.DefaultArtwork;

            var fileName = Guid.NewGuid().ToString("N") + ".jpg"; // TODO extension.
            Directory.CreateDirectory(TrackInfoReaderResolver.ArtworkSettings.ArtworkRootPath); // TODO create directory.
            var filePath = Path.Combine(TrackInfoReaderResolver.ArtworkSettings.ArtworkRootPath, fileName);

            reader.ReadToFollowing("data");
            using (var fileStream = new FileStream(filePath, FileMode.CreateNew))
            {
                var index = 0;
                var artworkBuffer = new byte[BufferSize];
                while (index < length)
                {
                    var count = reader.ReadElementContentAsBase64(artworkBuffer, 0, BufferSize);
                    fileStream.Write(artworkBuffer, 0, count);
                    index += count;
                }
            }

            return fileName;
        }
    }
}