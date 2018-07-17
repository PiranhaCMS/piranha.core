# Welcome to Piranha Angular 6 Example

## About

This is an Angular 6 of Piranha.Core CMS. The goal of this Example 
is to Show how to intergrate Piranha.Core CMS in Angular.

Piranha Angular 6 is currently built for `NetStandard 2.0` and uses the following awesome packages:

* Microsoft.AspNetCore.App `2.1.1`
* Microsoft.AspNetCore.SpaServices.Extensions `2.1.1`

## Getting started

#### Prerequisites

* [.NET Core SDK 2.1.1](https://www.microsoft.com/net/core/)
* An IDE or Editor of your choice

#### Get the latest source code and get going:

    > git clone https://github.com/PiranhaCMS/piranha.core.git
    > cd piranha.core
    > dotnet restore
    > dotnet build
    > cd examples/CoreWebAngular
    > dotnet run
    
#### Build and update javascript/css assets

    > cd piranha.core/core/Piranha.Manager
    > npm install
    > bower install
    > cd ../../examples/CoreWebAngular/ClientApp
    > npm install


#### Visual Studio users

For people running Visual Studio 2017 almost all of the above steps will be handled by the IDE. Just get the source code, open the `.sln` file and you're good to go.
