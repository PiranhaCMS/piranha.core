# Welcome to Piranha.Core

[![Codacy Badge](https://api.codacy.com/project/badge/Grade/ba7cbafe380b4c2796b731562c5166e0)](https://www.codacy.com/app/tidyui/piranha.core?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=PiranhaCMS/piranha.core&amp;utm_campaign=Badge_Grade)
[![CodeFactor](https://www.codefactor.io/repository/github/piranhacms/piranha.core/badge)](https://www.codefactor.io/repository/github/piranhacms/piranha.core)
[![Total alerts](https://img.shields.io/lgtm/alerts/g/PiranhaCMS/piranha.core.svg?logo=lgtm&logoWidth=18)](https://lgtm.com/projects/g/PiranhaCMS/piranha.core/alerts/)
[![Language grade: JavaScript](https://img.shields.io/lgtm/grade/javascript/g/PiranhaCMS/piranha.core.svg?logo=lgtm&logoWidth=18)](https://lgtm.com/projects/g/PiranhaCMS/piranha.core/context:javascript)
[![Sponsors](https://opencollective.com/piranhacms/tiers/sponsor/badge.svg?label=sponsor&color=brightgreen)](Sponsors)
[![Backers](https://opencollective.com/piranhacms/tiers/backer/badge.svg?label=backer&color=brightgreen)](Backers)
[![Gitter chat](https://badges.gitter.im/PiranhaCMS/Piranha.png)](https://gitter.im/PiranhaCMS/Piranha)

| Build server           | Platform     | Build status |
|------------------------|--------------|--------------|
| AppVeyor               | Windows      | [![Build status](https://ci.appveyor.com/api/projects/status/brw0cak0b5x4w17m?svg=true)](https://ci.appveyor.com/project/tidyui/piranha-core) |
| Travis                 | Linux / OS X | [![Build status](https://travis-ci.org/PiranhaCMS/piranha.core.svg?branch=master)](https://travis-ci.org/PiranhaCMS/piranha.core) |
| CoverAlls              |              | [![Coverage Status](https://coveralls.io/repos/github/PiranhaCMS/piranha.core/badge.svg?branch=master&service=github&random=1)](https://coveralls.io/github/PiranhaCMS/piranha.core?branch=master) |
| NuGet                  |              | [![NuGet](https://img.shields.io/nuget/v/Piranha.svg)](https://www.nuget.org/packages/Piranha) |
| Crowdin (Localization) |              | [![Crowdin](https://badges.crowdin.net/piranhacms/localized.svg)](https://crowdin.com/project/piranhacms) |

## About

This is a **complete rewrite** of Piranha CMS for `.NET Core`. The goal of this rewrite
is to create a version capable of targeting multiple platforms & frameworks with minimal
depenencies, but still provide a flexible & high performance CMS library.

Piranha is currently built for `.NET 5` and uses in its simplest form the following awesome packages:

* [AutoMapper](https://github.com/AutoMapper/AutoMapper)
* [Markdig](https://github.com/xoofx/markdig)
* [Microsoft.EntityFrameworkCore](https://github.com/dotnet/efcore)
* [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)

## Getting started

### Prerequisites

* [.NET 5.0 SDK](https://dotnet.microsoft.com/download/dotnet/5.0)
* An IDE or Editor of your choice

### Create a new project from our templates

To use our project templates you first need to download and install them from NuGet. This can be done with:

~~~ bash
dotnet new -i Piranha.Templates
~~~

When creating a new project with `dotnet new` you should first create a new empty folder. The default behaviour is that the new project is **named after its containing folder**.

> Please note that naming your project `Piranha` (even if it is a test project) will result in a circular reference error when you try to restore the packages. This is due to a limitation in `dotnet restore`.

After this is done you can create a new web project for razor pages with:

~~~ bash
dotnet new piranha.razor
~~~

To read more about of our available project templates, please read more on https://piranhacms.org/docs/basics/project-templates

### Get the latest source code and get going

    > git clone https://github.com/PiranhaCMS/piranha.core.git
    > cd piranha.core
    > dotnet restore
    > dotnet build
    > cd examples/MvcWeb
    > dotnet run

### Log into the Manager

The manager interface can be found at the URL `~/manager` with the default credentials:

    admin / password

For production scenarios we advise you to remove this user, or change the password
**and** update the password strength policy. More information on this can be found in
the official documentation [here](http://piranhacms.org/docs/architecture/authentication/identity).

### Build and update javascript/css assets

    > cd piranha.core/core/Piranha.Manager
    > npm install
    > gulp min:js
	> gulp min:css

### Visual Studio users

For people running Visual Studio 2017 almost all of the above steps will be handled by the IDE. Just get the source code, open the `.sln` file and you're good to go.

## Backers

Support Piranha CMS with a monthly donation and help us focus on giving you even more features and better support. [Piranha CMS @ Open Collective](https://opencollective.com/piranhacms)

<img src="https://opencollective.com/piranhacms/tiers/sponsor.svg?avatarHeight=36" />
<img src="https://opencollective.com/piranhacms/tiers/backer.svg?avatarHeight=36&width=600" />

## Code of Conduct

This project has adopted the code of conduct defined by the [Contributor Covenant](http://contributor-covenant.org/) to clarify expected behavior in our community.
For more information see the [.NET Foundation Code of Conduct](http://www.dotnetfoundation.org/code-of-conduct).

## .NET Foundation

This project is supported by the [.NET Foundation](http://www.dotnetfoundation.org).
