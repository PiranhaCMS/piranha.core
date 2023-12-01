# Welcome to Piranha.Core

[![Codacy Badge](https://app.codacy.com/project/badge/Grade/0fa7c8bcd5234443b79b075436e92d7e)](https://www.codacy.com/gh/PiranhaCMS/piranha.core/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=PiranhaCMS/piranha.core&amp;utm_campaign=Badge_Grade)
[![CodeFactor](https://www.codefactor.io/repository/github/piranhacms/piranha.core/badge)](https://www.codefactor.io/repository/github/piranhacms/piranha.core)
[![Sponsors](https://opencollective.com/piranhacms/tiers/sponsor/badge.svg?label=sponsor&color=brightgreen)](Sponsors)
[![Backers](https://opencollective.com/piranhacms/tiers/backer/badge.svg?label=backer&color=brightgreen)](Backers)
[![Gitter chat](https://badges.gitter.im/PiranhaCMS/Piranha.png)](https://gitter.im/PiranhaCMS/Piranha)

| Build server           | Platform     | Build status |
|------------------------|--------------|--------------|
| GitHub Actions         | Windows      | [![.NET Win](https://github.com/PiranhaCMS/piranha.core/actions/workflows/dotnet_win.yml/badge.svg)](https://github.com/PiranhaCMS/piranha.core/actions/workflows/dotnet_win.yml) |
| GitHub Actions         | Linux        | [![.NET](https://github.com/PiranhaCMS/piranha.core/actions/workflows/dotnet.yml/badge.svg)](https://github.com/PiranhaCMS/piranha.core/actions/workflows/dotnet.yml) |
| CoverAlls              |              | [![Coverage Status](https://coveralls.io/repos/github/PiranhaCMS/piranha.core/badge.svg?branch=master&service=github&random=1)](https://coveralls.io/github/PiranhaCMS/piranha.core?branch=master) |
| NuGet                  |              | [![NuGet](https://img.shields.io/nuget/v/Piranha.svg)](https://www.nuget.org/packages/Piranha) |
| Crowdin (Localization) |              | [![Crowdin](https://badges.crowdin.net/piranhacms/localized.svg)](https://crowdin.com/project/piranhacms) |

## About

Piranha CMS is a decoupled, cross-platform CMS built for `.NET8` and `Entity Framework Core`. It has a modular and extensible architecture and supports a multitude of hosting and deployment scenarios.

## Getting started

### Prerequisites

* [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download)
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

~~~
> git clone https://github.com/PiranhaCMS/piranha.core.git
> cd piranha.core
> dotnet restore
> dotnet build
> cd examples/MvcWeb
> dotnet run
~~~

### Log into the Manager

The manager interface can be found at the URL `~/manager` with the default credentials:

~~~
admin / password
~~~

For production scenarios we advise you to remove this user, or change the password
**and** update the password strength policy. More information on this can be found in
the [official documentation here](http://piranhacms.org/docs/architecture/authentication/identity).

### Build and update javascript/css assets

~~~
> cd piranha.core/core/Piranha.Manager
> npm install
> gulp min:js
> gulp min:css
~~~

## Backers

Support Piranha CMS with a monthly donation and help us focus on giving you even more features and better support. [Piranha CMS @ Open Collective](https://opencollective.com/piranhacms)

<img src="https://opencollective.com/piranhacms/tiers/sponsor.svg?avatarHeight=36" />
<img src="https://opencollective.com/piranhacms/tiers/backer.svg?avatarHeight=36&width=600" />

## Sponsors

These are our financial sponsors! You can also become a sponsor either through GitHub or [Open Collective](https://opencollective.com/piranhacms).

[![Arcady](https://piranhacms.azureedge.net/uploads/672d2600-8822-4b74-bb06-392f0c4aa38d-arcady_black.png)](https://www.arcady.nl)

[![Peak Crypto](https://piranhacms.azureedge.net/uploads/5b9b6a74-5cf6-456d-a8a4-5d831eed5509-peak-crypto-small.png)](https://www.peakcrypto.com/)

## Code of Conduct

This project has adopted the code of conduct defined by the [Contributor Covenant](http://contributor-covenant.org/) to clarify expected behavior in our community.
For more information see the [.NET Foundation Code of Conduct](http://www.dotnetfoundation.org/code-of-conduct).

## .NET Foundation

This project is supported by the [.NET Foundation](http://www.dotnetfoundation.org).
