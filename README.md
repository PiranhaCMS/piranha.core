# Welcome to Piranha.Core

| Build server | Platform     | Build status |
|--------------|--------------|--------------|
| AppVeyor     | Windows      | [![Build status](https://ci.appveyor.com/api/projects/status/brw0cak0b5x4w17m?svg=true)](https://ci.appveyor.com/project/tidyui/piranha-core) |
| Travis       | Linux / OS X | [![Build status](https://travis-ci.org/PiranhaCMS/piranha.core.svg?branch=master)](https://travis-ci.org/PiranhaCMS/piranha.core) |
| CoverAlls    |              | [![Coverage Status](https://coveralls.io/repos/github/PiranhaCMS/piranha.core/badge.svg?branch=master&service=github)](https://coveralls.io/github/PiranhaCMS/piranha.core?branch=master) |
| NuGet        |              | [![NuGet](https://img.shields.io/nuget/v/Piranha.svg)](https://www.nuget.org/packages/Piranha) |

## About

This is a **complete rewrite** of Piranha CMS for `NetStandard`. The goal of this rewrite 
is to create a version capable of targeting multiple platforms & frameworks with minimal
depenencies, but still provide a flexible & high performance CMS library.

Piranha is currently built for `NetStandard 2.0` and uses the following awesome packages:

* AutoMapper `6.2.1`
* Markdig `0.14.6`
* Microsoft.EntityFrameworkCore `2.0.1`
* Newtonsoft.Json `10.0.3`

## Core Packages

### Piranha
[`AutoMapper`, `Markdig`, `Microsoft.EntityFrameworkCore`, `Newtonsoft.Json`]

The core library that contains all data management, repositories, client models and extensibility features. In a way, everything you need to integrate Piranha into your existing solution.

### Piranha.AspNetCore
[`Microsoft.AspNetCore.Http`]

Middleware components and other tools for building a .NET Core web application with Piranha.

### Piranha.AttributeBuilder

Components for automatically build and import page types by adding simple attributes to your models.

### Piranha.Local.FileStorage

Provider for storing uploaded media files on the local filesystem.

### Piranha.Manager
[`Microsoft.AspNetCore.Mvc`, `Microsoft.AspNetCore.Session`, `Microsoft.AspNetCore.StaticFiles`, `Microsoft.Extensions.FileProviders.Embedded`]

Manager interface for Piranha.

## Licensing
Piranha CMS is released under the **MIT** license. It is a permissive free software license,
meaning that it permits reuse within proprietary software provided all copies of the licensed
software include a copy of the MIT License terms and the copyright notice.

## Getting started

#### Prerequisites

* [.NET Core SDK 2.0](https://www.microsoft.com/net/core/)
* Any IDE or Editor good for coding :)

#### Get the latest source code and get going:

    > git clone https://github.com/PiranhaCMS/piranha.core.git
    > cd piranha.core
    > dotnet restore
    > dotnet build
    > cd examples/CoreWeb
    > dotnet run
    
#### Build and update javascript/css assets

    > cd piranha.core/core/Piranha.Manager
    > npm install
    > bower install
    > cd ../../examples/CoreWeb
    > npm install
    > bower install

#### Visual Studio users

For people running Visual Studio 2017 almost all of the above steps will be handled by the IDE. Just get the source code, open the `.sln` file and you're good to go.
