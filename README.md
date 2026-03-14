# Welcome to Aero.Core

[![Codacy Badge](https://app.codacy.com/project/badge/Grade/0fa7c8bcd5234443b79b075436e92d7e)](https://www.codacy.com/gh/AeroCMS/Aero.core/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=AeroCMS/Aero.core&amp;utm_campaign=Badge_Grade)
[![CodeFactor](https://www.codefactor.io/repository/github/Aerocms/Aero.core/badge)](https://www.codefactor.io/repository/github/Aerocms/Aero.core)
[![Sponsors](https://opencollective.com/Aerocms/tiers/sponsor/badge.svg?label=sponsor&color=brightgreen)](Sponsors)
[![Backers](https://opencollective.com/Aerocms/tiers/backer/badge.svg?label=backer&color=brightgreen)](Backers)
[![Gitter chat](https://badges.gitter.im/AeroCMS/Aero.png)](https://gitter.im/AeroCMS/Aero)

| Build server           | Platform     | Build status |
|------------------------|--------------|--------------|
| GitHub Actions         | Windows      | [![.NET Win](https://github.com/AeroCMS/Aero.core/actions/workflows/dotnet_win.yml/badge.svg)](https://github.com/AeroCMS/Aero.core/actions/workflows/dotnet_win.yml) |
| GitHub Actions         | Linux        | [![.NET](https://github.com/AeroCMS/Aero.core/actions/workflows/dotnet.yml/badge.svg)](https://github.com/AeroCMS/Aero.core/actions/workflows/dotnet.yml) |
| CoverAlls              |              | [![Coverage Status](https://coveralls.io/repos/github/AeroCMS/Aero.core/badge.svg?branch=master&service=github&random=1)](https://coveralls.io/github/AeroCMS/Aero.core?branch=master) |
| NuGet                  |              | [![NuGet](https://img.shields.io/nuget/v/Aero.svg)](https://www.nuget.org/packages/Aero) |
| Crowdin (Localization) |              | [![Crowdin](https://badges.crowdin.net/Aerocms/localized.svg)](https://crowdin.com/project/Aerocms) |

## About

Aero CMS is a decoupled, cross-platform CMS built for the latest version of `.NET` and `MartenDB` (PostgreSQL). It has a modular and extensible architecture and supports a multitude of hosting and deployment scenarios.

## Getting started

### Prerequisites

* [.NET 10.0 SDK](https://dotnet.microsoft.com/en-us/download)
* An IDE or Editor of your choice

### Create a new project from our templates

To use our project templates you first need to download and install them from NuGet. This can be done with:

~~~ bash
dotnet new install Aero.Templates
~~~

When creating a new project with `dotnet new` you should first create a new empty folder. The default behaviour is that the new project is **named after its containing folder**.

> Please note that naming your project `Aero` (even if it is a test project) will result in a circular reference error when you try to restore the packages. This is due to a limitation in `dotnet restore`.

After this is done you can create a new web project for razor pages with:

~~~ bash
dotnet new Aero.razor
~~~

To read more about of our available project templates, please read more on https://Aerocms.org/docs/basics/project-templates

### Get the latest source code and get going

~~~
> git clone https://github.com/AeroCMS/Aero.core.git
> cd Aero.core
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
the [official documentation here](http://Aerocms.org/docs/architecture/authentication/identity).

### Build and update javascript/css assets


## Backers

Support Aero CMS with a monthly donation and help us focus on giving you even more features and better support. [Aero CMS @ Open Collective](https://opencollective.com/Aerocms)

<!-- <img src="https://opencollective.com/Aerocms/tiers/sponsor.svg?avatarHeight=36" />
<img src="https://opencollective.com/Aerocms/tiers/backer.svg?avatarHeight=36&width=600" /> -->

## Sponsors

These are our financial sponsors! You can also become a sponsor either through GitHub or [Open Collective](https://opencollective.com/Aerocms).

- Your name can be here !  💕

## Code of Conduct

This project has adopted the code of conduct defined by the [Contributor Covenant](http://contributor-covenant.org/) to clarify expected behavior in our community.


