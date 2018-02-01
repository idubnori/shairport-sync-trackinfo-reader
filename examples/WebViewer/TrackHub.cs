using System;
using Microsoft.AspNetCore.SignalR;
using ShairportSync.Metadata.Models;

namespace WebViewer
{
    public class TrackHub : Hub
    {
        private readonly TrackSource _trackSource;

        public TrackHub(TrackSource trackSource)
        {
            _trackSource = trackSource;
        }

        public IObservable<TrackInfo> StreamTracks()
        {
            return _trackSource.TrackObservable;
        }
    }
}
