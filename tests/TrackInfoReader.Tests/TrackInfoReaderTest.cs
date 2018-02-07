using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Microsoft.Reactive.Testing;
using ShairportSync.Metadata.Models;
using Xunit;
using Xunit.Abstractions;

namespace TrackInfoReader.Tests
{
    public class TrackInfoReaderTest : TrackInfoReaderTestBase
    {
        public TrackInfoReaderTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public async Task OneTrackWithArtwork_OneTrackAndOneArtwork()
        {
            var metadataTestFile = @"./TestData/metadata-onetrack-oneartwork";
            var expectedTrack = new TrackInfo { Artist = "Pepe California", Album = "White Flag", Song = "Seashore Park" };

            var actual = await SubscribeMetadataFileAsync(metadataTestFile);

            actual.Messages.AssertEqual(
                OnNext<TrackInfo>(0, t => t.IsSameSong(expectedTrack) && t.AssertDefaultArtwork()),
                OnNext<TrackInfo>(0, t => t.IsSameSong(expectedTrack) && t.IsArtWork()),
                OnCompleted<TrackInfo>(0)
            );
        }

        [Fact]
        public async Task SecondSongWithNoArtwork_LastArtworkIsDefault()
        {
            var metadataTestFile = @"./TestData/next-song-no-image-in-same-album";

            var actual = await SubscribeMetadataFileAsync(metadataTestFile);

            var actualLastArtwork = actual.Messages[3].Value.Value;
            actualLastArtwork.AssertDefaultArtwork();
        }
    }
}