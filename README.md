# Welcome to Piranha.Core — MinooTrading Fork

| Build server   | Platform | Build status |
|----------------|----------|--------------|
| GitHub Actions | Windows  | [![.NET Win](https://github.com/MinooTradingSPC/piranha.core/actions/workflows/dotnet_win.yml/badge.svg)](https://github.com/MinooTradingSPC/piranha.core/actions/workflows/dotnet_win.yml) |
| GitHub Actions | Linux    | [![.NET](https://github.com/MinooTradingSPC/piranha.core/actions/workflows/dotnet.yml/badge.svg)](https://github.com/MinooTradingSPC/piranha.core/actions/workflows/dotnet.yml) |
| NuGet          |          | [![NuGet](https://img.shields.io/nuget/v/Piranha.svg)](https://www.nuget.org/packages/Piranha) |

> **This is a security-hardened fork** of [PiranhaCMS/piranha.core](https://github.com/PiranhaCMS/piranha.core) maintained by [Kiarash Minoo](https://github.com/KiarashMinoo) / MinooTrading SPC.
> Upstream badges, sponsors, and community links are preserved below.

## About

Piranha CMS is a decoupled, cross-platform CMS built for `.NET8` and `Entity Framework Core`. It has a modular and extensible architecture and supports a multitude of hosting and deployment scenarios.

This fork targets `.NET 8`, `.NET 9`, and `.NET 10`, and ships with a comprehensive set of security remediations on top of the upstream 10.x codebase.

## Security Improvements (v1.0.1)

The following CodeQL findings from the upstream codebase have been remediated in this fork:

| # | Rule | File | Fix |
|---|------|------|-----|
| [#40](https://github.com/MinooTradingSPC/piranha.core/issues/40) | `cs/web/cookie-secure-not-set` | `AuthController.cs` | Added `Secure = true` + `SameSite = Strict` to XSRF cookie |
| [#39](https://github.com/MinooTradingSPC/piranha.core/issues/39) | `cs/web/missing-token-validation` | `CmsController.cs` | Added `[ValidateAntiForgeryToken]` to `SavePostComment` |
| [#38](https://github.com/MinooTradingSPC/piranha.core/issues/38) | `cs/user-controlled-bypass` | `ModelLoader.cs:72` | Explicit `return null` when `PagePreview` auth fails |
| [#37](https://github.com/MinooTradingSPC/piranha.core/issues/37) | `cs/user-controlled-bypass` | `ModelLoader.cs:154` | Explicit `return null` when `PostPreview` auth fails |
| [#86](https://github.com/MinooTradingSPC/piranha.core/issues/86)/[#87](https://github.com/MinooTradingSPC/piranha.core/issues/87) | `cs/user-controlled-bypass` | `ModelLoader.cs` | Hardened published-state checks; removed unsafe casts |

Additional JS dist-file findings (DOM XSS, incomplete sanitization, unsafe HTML expansion) were patched and the corresponding issues closed.

## Getting started

### Prerequisites

* [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download) (or .NET 9 / .NET 10)
* An IDE or Editor of your choice

### Clone this fork

~~~
> git clone https://github.com/MinooTradingSPC/piranha.core.git
> cd piranha.core
> dotnet restore
> dotnet build
> cd examples/MvcWeb
> dotnet run
~~~

### Create a new project from upstream templates

To use official project templates install them from NuGet:

~~~ bash
dotnet new install Piranha.Templates
~~~

When creating a new project with `dotnet new` you should first create a new empty folder. The default behaviour is that the new project is **named after its containing folder**.

> Please note that naming your project `Piranha` (even if it is a test project) will result in a circular reference error when you try to restore the packages. This is due to a limitation in `dotnet restore`.

~~~ bash
dotnet new piranha.razor
~~~

To read more about available project templates, see the [upstream docs](https://piranhacms.org/docs/basics/project-templates).

### Log into the Manager

The manager interface can be found at the URL `~/manager` with the default credentials:

~~~
admin / password
~~~

For production scenarios remove this user or change the password **and** update the password strength policy. More information can be found in the [official documentation](http://piranhacms.org/docs/architecture/authentication/identity).

### Build and update javascript/css assets

~~~
> cd piranha.core/core/Piranha.Manager
> npm install
> gulp min:js
> gulp min:css
~~~

## Upstream Project

This fork is based on [PiranhaCMS/piranha.core](https://github.com/PiranhaCMS/piranha.core). The upstream project is supported by the [.NET Foundation](http://www.dotnetfoundation.org) and its community of backers and sponsors.

Support the upstream project via [Piranha CMS @ Open Collective](https://opencollective.com/piranhacms).

<img src="https://opencollective.com/piranhacms/tiers/sponsor.svg?avatarHeight=36" />
<img src="https://opencollective.com/piranhacms/tiers/backer.svg?avatarHeight=36&width=600" />

Upstream sponsors include:

[![Arcady](https://piranhacms.azureedge.net/uploads/672d2600-8822-4b74-bb06-392f0c4aa38d-arcady_black.png)](https://www.arcady.nl)
[![Peak Crypto](https://piranhacms.azureedge.net/uploads/5b9b6a74-5cf6-456d-a8a4-5d831eed5509-peak-crypto-small.png)](https://www.peakcrypto.com/)

## Code of Conduct

This project follows the code of conduct defined by the [Contributor Covenant](http://contributor-covenant.org/).
For more information see the [.NET Foundation Code of Conduct](http://www.dotnetfoundation.org/code-of-conduct).
