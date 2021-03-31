# NM4PIG 🐷

This is a rayTracer library called NM4PIG written in C#. It was developed for the course _Numerical Methods for Photorealistic Image Generation_ held by Prof. [Maurizio Tomasi][1] at Università degli Studi di Milano during March-June 2021.

The contibutors to the project are [T. Armadillo][2] - [P. Klausner][3] - [A. Sala][4]

## Prerequisites
This library has been developed and tested with .NET version 5.0.103. It is possible to download the latest version [here](https://dotnet.microsoft.com/download).

It is also necessary to install the following packages:
- [ImageSharp][5] `dotnet add package SixLabors.ImageSharp`

## Usage

In order to use the library you can clone the repository:

    git clone git@github.com:andreasala98/NM4PIG.git

To check that the code works as expected, you can run a set of tests using the following command:

    cd Trace.Test
    dotnet test

To run the console application:

    cd NM4PIG
    dotnet run <args>

## License

The code is released under a MIT license. See the file [LICENSE.md](./LICENSE.md)


[1]: https://github.com/ziotom78
[2]: https://github.com/TommasoArmadillo
[3]: https://github.com/PietroKlausner
[4]: https://github.com/andreasala98
[5]: https://docs.sixlabors.com/articles/imagesharp/index.html?tabs=tabid-1
