using System;
using System.Reactive.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ShairportSync.Metadata;
using ShairportSync.Metadata.Models;

namespace WebViewer
{
    public class TrackSource
    {
        public IObservable<TrackInfo> TrackObservable { get; }

        public TrackSource(IConfiguration configuration, ILogger<TrackSource> logger)
        {
            var metadataPipePath = configuration["pipe"] ?? "/tmp/shairport-sync-metadata";
            logger.LogInformation($"MetadataPipePath-> {metadataPipePath}");

            var trackInfoSource = TrackInfoReader.GetObservable(metadataPipePath)
                .Catch((Exception e) =>
                {
                    logger.LogError(e, "OnError from TrackInfoReader.GetObservable.");
                    return Observable.Empty<TrackInfo>();
                })
                .Publish();
            trackInfoSource.Connect();
            TrackObservable = trackInfoSource;
        }
    }
}