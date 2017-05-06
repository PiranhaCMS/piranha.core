# Welcome to Piranha.Core

| Build server | Platform     | Build status |
|--------------|--------------|--------------|
| AppVeyor     | Windows      | [![Build status](https://ci.appveyor.com/api/projects/status/brw0cak0b5x4w17m?svg=true)](https://ci.appveyor.com/project/tidyui/piranha-core) |
| Travis       | Linux / OS X | [![Build status](https://travis-ci.org/PiranhaCMS/piranha.core.svg?branch=master)](https://travis-ci.org/PiranhaCMS/piranha.core) |

## About

This is a **complete rewrite** of Piranha CMS for `NetStandard`. The goal of this rewrite 
is to create a version capable of targeting multiple platforms & frameworks with minimal
depenencies, but still provide a flexible & high performance CMS library.

Piranha is currently built for `NetStandard 1.4` and uses the following awesome packages:

* AutoMapper `5.2.0`
* Dapper `1.50.2`
* Markdown `2.2.1`
* Newtonsoft.Json `9.0.1`

## Core Packages

### Piranha
**NetStandard 1.4** [`AutoMapper`, `Dapper`, `Markdown`, `Newtonsoft.Json`]

The core library that contains all data management, repositories, client models and extensibility features. In a way, everything you need to integrate Piranha into your existing solution.

### Piranha.AspNetCore
**NetStandard 1.4** [`Microsoft.AspNetCore.Http`]

Middleware components and other tools for building a .NET Core web application with Piranha.

### Piranha.AttributeBuilder
**NetStandard 1.4**

Components for automatically build and import page types by adding simple attributes to your models.

### Piranha.Manager
**CoreApp 1.1** [`Microsoft.AspNetCore.Mvc`, `Microsoft.AspNetCore.StaticFiles`, `Microsoft.Extensions.FileProviders.Embedded`]

Manager interface for Piranha. As `Microsoft.AspNetCore.Mvc` currently references `NetStandard 1.6` this package is not compatible with the .NET Framework. This will hopefully be fixed when `NetStandard 2.0` is released.

## Licensing
Piranha CMS is released under the **MIT** license. It is a permissive free software license,
meaning that it permits reuse within proprietary software provided all copies of the licensed
software include a copy of the MIT License terms and the copyright notice.

## Getting started
Get the latest source code and get going:

    > git clone https://github.com/PiranhaCMS/piranha.core.git
    > cd piranha.core
    > dotnet restore
    > dotnet build
    > cd examples/CoreWeb
    > dotnet run
    
To build and update the javascript/css in the **manager** and **example project**:

    > cd piranha.core/core/Piranha.Manager
    > npm install
    > bower install
    > cd ../../examples/CoreWeb
    > npm install
    > bower install
    
## Status
This repository is under development and is **not** intended for production use.
