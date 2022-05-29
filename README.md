# An STL Reading and Writing Library for .NET

This library facilitates the reading and writing of Stereo Lithograph (STL) files. It is written in C# 4 and was created with Visual Studio 2012.

## Features

* Reads ASCII STL files.
* Reads binary STL files.
* Writes ASCII STL files.
* Writes binary STL files.
* Provides an object-oriented mechanism by which to create STL files from scratch.

## Installation

You may [find the latest release here](https://github.com/QuantumConcepts/STLdotNET/releases). You can download the source and build it yourself, download the binaries from the release, or install the NuGet package:

    Install-Package QuantumConcepts.Formats.StereoLithography

## Supported Runtimes

Multiple runtimes are supported:

- .NET 5
- .NET 6
- .NET Standard 2.1
- .NET Core

## Contributing

To contribute a code change, please submit a Pull Request. If you find an issue, please feel free to report it via GitHub.

If you do decide to make a code change, before pushing your branch and opening the Pull Request, please make sure you...

- Run `dotnet build` and fix any errors _and_ warnings.
- Run `dotnet test` and fix any broken tests.
- Run `dotnet format` to automatically fix any formatting issues.

Thank you!
