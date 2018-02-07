using System;
using System.IO;
using System.IO.Pipes;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Reactive.Testing;
using ShairportSync.Metadata;
using ShairportSync.Metadata.Models;
using TrackInfoReader.Tests.Logger;
using Xunit.Abstractions;

namespace TrackInfoReader.Tests
{
    public class TrackInfoReaderTestBase : ReactiveTest, IDisposable
    {
        protected readonly ILogger<TrackInfoReaderTest> Logger;
        private readonly string _artworkImagePath;

        public TrackInfoReaderTestBase(ITestOutputHelper testOutputHelper)
        {
            Logger = testOutputHelper.CreateLogger<TrackInfoReaderTest>();
            _artworkImagePath = "./artworkImages/"; // TODO
        }

        public void Dispose()
        {
            Directory.Delete(_artworkImagePath, true);
        }

        protected async Task<ITestableObserver<TrackInfo>> SubscribeMetadataFileAsync(string metadataFile)
        {
#if !DEBUG_LINUX // TODO https://github.com/dotnet/project-system/issues/2733
            var pipeServer = new AnonymousPipeServerStream(PipeDirection.Out);
            var pipeClient = new AnonymousPipeClientStream(PipeDirection.In, pipeServer.GetClientHandleAsString());

            var testScheduler = new TestScheduler();
            var mockObserver = testScheduler.CreateObserver<TrackInfo>();

            var trackSource = ShairportSync.Metadata.TrackInfoReader.GetObservable(pipeClient);
            var trackSubscriber = trackSource.Subscribe(mockObserver);

            //pipeServer.WaitForPipeDrain();
            await pipeServer.WriteMetadataAsync(metadataFile);

            Logger.LogDebug(mockObserver.Messages.DebugInfo(t => t.DebugInfo()));

            using (new CompositeDisposable {trackSubscriber, pipeClient, pipeServer})
            {
                return mockObserver;
            }
#else
            var pipePath = "/tmp/shairport-sync-metadata"; // TODO mkfifo and delete.
            
            var testScheduler = new TestScheduler();
            var mockObserver = testScheduler.CreateObserver<TrackInfo>();

            var trackSource = ShairportSync.Metadata.TrackInfoReader.GetObservable(pipePath);
            using (var trackSubscriber = trackSource.Subscribe(mockObserver))
            {
                using (var pipeStream = new FileStream(pipePath, FileMode.Open))
                {
                    await pipeStream.WriteMetadataAsync(metadataFile);
                }
            }

            return mockObserver;
#endif
        }
    }
}