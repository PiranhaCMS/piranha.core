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

Piranha is currently built for `NetStandard 2.0` and `NetCoreApp 3.1` and uses in its simplest form the following awesome packages:

* Markdig `0.18.0`
* Microsoft.EntityFrameworkCore `3.1.0`
* Newtonsoft.Json `12.0.3`

## Getting started

### Prerequisites

* [.NET Core SDK 3.1.100](https://dotnet.microsoft.com/download/dotnet-core/3.1)
* An IDE or Editor of your choice

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
    > bower install
    > cd ../../examples/MvcWeb
    > npm install
    > bower install

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