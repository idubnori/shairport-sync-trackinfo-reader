let connection = new signalR.HubConnection("/tracks");

connection.start().then(() => {
    console.debug("connected.");
    startStreaming();
});

function startStreaming() {
    connection.stream("StreamTracks").subscribe({
        close: false,
        next: displayTrack,
        error: err => console.error(err)
    });
}

function displayTrack(track) {
    console.debug("displayTrack->" + JSON.stringify(track));

    document.getElementById("artist").textContent = track.artist;
    document.getElementById("album").textContent = track.album;
    document.getElementById("song").textContent = track.song;
    document.getElementById("artwork").src = `./artworkImages/${track.artwork}`;
}