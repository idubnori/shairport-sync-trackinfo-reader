ShairportSync.TrackInfoReader for .NET
===
[![NuGet](https://img.shields.io/nuget/v/ShairportSync.TrackInfoReader.svg?maxAge=3600)](https://www.nuget.org/packages/ShairportSync.TrackInfoReader/)
[![GitHub license](https://img.shields.io/github/license/idubnori/shairport-sync-trackinfo-reader.svg)](https://github.com/idubnori/shairport-sync-trackinfo-reader/blob/master/LICENSE)
[![Build status](https://ci.appveyor.com/api/projects/status/y4vtm6cnqvm4ie5i/branch/master?svg=true)](https://ci.appveyor.com/project/idubnori/shairport-sync-trackinfo-reader/branch/master)

ShairportSync.TrackInfoReader is a [shairport-sync](https://github.com/mikebrady/shairport-sync) [metadata](https://github.com/mikebrady/shairport-sync#metadata) parser which provides track information (artist, album, song title, artwork image, etc.) as Rx (`IObservable<TrackInfo>`).

This library works on .NET Standard 2.0 compliant platforms like .NET Core 2 / Mono.<br>
i.e. works on Raspberry Pi 2 / 3, Mac, Linux, etc.
## Examples
Contains two usage example apps below.
#### Web Viewer
![WebViewer](./docs/web-viewer-demo.gif)

#### Console Viewer
![ConsoleViewer](./docs/console-viewer-demo.gif)

## Quick Start
#### Install via NuGet
Package Manager: 
```
PM> Install-Package ShairportSync.TrackInfoReader –IncludePrerelease
```

.NET CLI:
```bash
dotnet add package ShairportSync.TrackInfoReader
```

#### Subscribe Track Information
```csharp
var trackInfoSource = TrackInfoReader.GetObservable("/tmp/shairport-sync-metadata");
trackInfoSource.Subscribe(t => Console.WriteLine($"{t.Artist} {t.Album} {t.Song}"));
```

## Run examples on Raspberry Pi 2 / 3
#### Preparation
On your Pi, download, unzip and set permission as follows.<br>
(No need to install .NET Core.)
```bash
curl -s https://api.github.com/repos/idubnori/shairport-sync-trackinfo-reader/releases/latest \
| grep "examples-linux-arm.zip" \
| cut -d '"' -f 4 \
| wget -qi -
unzip examples-linux-arm.zip
chmod 744 ./console-viewer/ConsoleViewer ./web-viewer/WebViewer
```
Run shairport-sync with metadata pipe parameter.
```bash
shairport-sync --pipe=/tmp/shairport-sync-metadata
```
#### ConsoleViewer
```bash
./ConsoleViewer /tmp/shairport-sync-metadata
```

#### WebViewer
```bash
./WebViewer pipe=/tmp/shairport-sync-metadata
```

Browse ```http://<Pi HostName>:5000/```<br>

## How to Build
Install [NET Core 2.1 SDK](https://www.microsoft.com/net/download/).
#### VisualStudio / Rider
Open `TrackInfoReader.sln`. And Build it.
#### Console
```bash
# Move to solution root directory.
dotnet build
```

#### Publish build example apps for Pi
```bash
# Move to ConsoleViewer or WebViewer directory.
dotnet publish -c Release -r linux-arm
# Deploy ./bin/Release/netcoreapp2.0/linux-arm/publish/ to Pi.
```

## License
ShairportSync.TrackInfoReader is Copyright © 2018 idubnori under the [MIT License](./LICENSE).

#### Dependencies
 - `build.cake` is based on [EventFlow](https://github.com/eventflow/EventFlow)