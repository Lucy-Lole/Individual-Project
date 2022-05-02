# Lucy Lole's C# Sonifier

## Building

Simply clone or download the respository then open `.src\CodeSonification\CodeSonification.sln` within visual studio.

If you get an 'Asset file not found' error, open the package manager console and type `dotnet restore`.

## Requirements

Both of these **should** be downloaded by nuget.

NAudio - https://github.com/naudio/NAudio

Roslyn - https://github.com/dotnet/roslyn

## Mappings

Different aspects of C# code have been mapped to different sounds, separated into 3 layers as follows:


Class Layer:

|Code Data|Sound|Notes|
|-|-|-|
|Imports|Wind sounds||
|Namespace|Low Drone||
|Classes|Synth Chords|Played at the start and end of the class.|

Method Layer:
|Code Data|Sound|Notes|
|-|-|-|
|Methods|Guitar Chords|Played at the start and end of the method.|
|Fields/Properties|Guitar Notes||

Internals Layer:
|Code Data|Sound|Notes|
|-|-|-|
|Code Blocks (if, else if, else, for, while, foreach, using)|Bell|Played at the start/end of the block (Each indentation increases the pitch of the bell)|
|Method Calls|Bird sounds||
