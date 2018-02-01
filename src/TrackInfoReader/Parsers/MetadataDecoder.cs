using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Xml;
using ShairportSync.Metadata.Models;
using ShairportSync.Metadata.Resolvers;
using ShairportSync.Metadata.Utilities;

namespace ShairportSync.Metadata.Parsers
{
    internal static class MetadataDecoder
    {
        private static readonly string[] HexStringCodes = { "asdc", "astn", "asdk", "astm", "mper", "astc" };

        private static readonly XmlReaderSettings MetadataXmlReaderSettings =
            new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment /*, Async = true,*/ };

        public static IObservable<Item> GetObservable(string metadataPipePath)
        {
            return Observable.Using(
                () => new FileStream(metadataPipePath, FileMode.Open, FileAccess.Read, FileShare.Write, 4096/*, FileOptions.Asynchronous,*/),
                fileStream => GetObservable(fileStream));
        }

        public static IObservable<Item> GetObservable(Stream stream)
        {
            return Observable.Using(() => XmlReader.Create(stream, MetadataXmlReaderSettings),
                xmlReader => GetObservable(xmlReader));
        }

        private static IObservable<Item> GetObservable(XmlReader xmlReader)
        {
            return Observable.Create<Item>(o =>
            {
                try
                {
                    while (xmlReader.ReadToFollowing("item"))
                    {
                        var item = DecodeItem(xmlReader);
                        o.OnNext(item);
                    }
                    o.OnCompleted();
                    Debug.WriteLine("OnCompleted Item.");
                }
                catch (Exception e)
                {
                    o.OnError(e);
                }
                return Disposable.Empty;
            });
        }

        private static Item DecodeItem(XmlReader xmlReader)
        {
            xmlReader.ReadToFollowing("type");
            var type = xmlReader.ReadElementContentAsString().HexStringToString();
            var code = xmlReader.ReadElementContentAsString().HexStringToString();
            var length = xmlReader.ReadElementContentAsInt();
            var data = DecodeData(xmlReader, code, length);
            Debug.WriteLine($"{type}, {code}, {length}, {data}");

            var item = new Item {Type = type, Code = code, Data = data};
            return item;
        }

        private static string DecodeData(XmlReader reader, string code, int length)
        {
            if (code == "PICT")
                return TrackInfoReaderResolver.ArtworkReader.ReadArtwork(reader, length);

            if (length == 0)
                return null;

            var bytes = GetDataAsBytes(reader, length);

            if (HexStringCodes.Contains(code))
                return BitConverter.ToString(bytes).Replace("-", string.Empty);

            return Encoding.UTF8.GetString(bytes);
        }

        private static byte[] GetDataAsBytes(XmlReader reader, int length)
        {
            var bytes = new byte[length];

            reader.ReadToFollowing("data");
            reader.ReadElementContentAsBase64(bytes, 0, length);

            return bytes;
        }
    }
}