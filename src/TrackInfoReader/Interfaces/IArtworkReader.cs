using System.Xml;

namespace ShairportSync.Metadata.Interfaces
{
    internal interface IArtworkReader
    {
        string ReadArtwork(XmlReader reader, int length);
    }
}