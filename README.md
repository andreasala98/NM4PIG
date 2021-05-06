 <img align="right" width="300" src="https://github.com/andreasala98/NM4PIG/blob/master/logo/Pig_mirror.png">
 <h1 align="center">  NM4PIG </h1> <br>
 
 [![Unit tests](https://github.com/andreasala98/NM4PIG/actions/workflows/test.yml/badge.svg)](https://github.com/andreasala98/NM4PIG/actions/workflows/test.yml)
 [![DocFX Build and Publish](https://github.com/andreasala98/NM4PIG/actions/workflows/docfx-build-publish.yml/badge.svg?branch=master)](https://github.com/andreasala98/NM4PIG/actions/workflows/docfx-build-publish.yml)
 [![MIT License](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/andreasala98/NM4PIG/blob/master/LICENSE.md)

This is a rayTracer library called NM4PIG written in C#. It was developed for the course _Numerical Methods for Photorealistic Image Generation_ held by Prof. [Maurizio Tomasi][1] at Università degli Studi di Milano (A.Y. 2020-2021).

The contibutors to the project are [Tommaso Armadillo][2], [Pietro Klausner][3] and [Andrea Sala][4].

## Table of Contents

- [Prerequisites](#prerequisites)
- [Usage](#usage)
- [Documentation](#documentation)
- [License](#license)


## Prerequisites

This library has been developed and tested with .NET version 5.0.x. It is possible to download the latest version [here](https://dotnet.microsoft.com/download).

This library uses some external libraries. The user should not worry because .NET automatically import them
- [ImageSharp][5] `dotnet add package SixLabors.ImageSharp`
- [CommandLineUtils][8] `dotnet add package Microsoft.Extensions.CommandLineUtils`

## Usage

In order to use the library you can clone the repository:

    git clone git@github.com:andreasala98/NM4PIG.git

To check that the code works as expected, you can run a set of tests using the following command:

    cd Trace.Test
    dotnet test

#### Demo mode

To run the application and visualize a simple image, use the following command (from the NM4PIG/NM4PIG directory):

    dotnet run -d (or --demo)

#### Converter mode

To convert an existing `.pfm` file into a `.png` or `.jpg` file, type the following command:

 
    dotnet run -g (or --conv) <inputImage.pfm> <factor> <gamma> <outputImage.jpg/png>

    
To clean NM4PIG directory from generated `.png` and `.jpg` samples, type  `./clean.sh` . If you want to keep an image, make sure to add a 'G' before the format (e.g. SampleG.jpg). This will prevent your image from being deleted.

## Documentation

A webpage with all the documentation is available at [this link][7]. This webpage is generated with DocFX. [Learn more][6]. Any suggestion to improve the documentation website is welcome!


## License

The code is released under a MIT license. See the file [LICENSE.md](./LICENSE.md)


[1]: https://github.com/ziotom78
[2]: https://github.com/TommasoArmadillo
[3]: https://github.com/PietroKlausner
[4]: https://github.com/andreasala98
[5]: https://docs.sixlabors.com/articles/imagesharp/index.html?tabs=tabid-1
[6]: https://dotnet.github.io/docfx/
[7]: https://andreasala98.github.io/NM4PIG/
[8]: https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.commandlineutils.commandlineapplication?view=dotnet-plat-ext-1.1
