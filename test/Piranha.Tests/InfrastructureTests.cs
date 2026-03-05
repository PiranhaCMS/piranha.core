
using Microsoft.Extensions.DependencyInjection;
using Piranha;
using Piranha.Cache;
using Piranha.Repositories;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Piranha.Tests;

public class InfrastructureTests : BaseTestsAsync
{
    public async override Task InitializeAsync()
    {
        await base.InitializeAsync();
    }

    [Fact]
    public void ResolveApiFromDI()
    {
        var api = _services.GetRequiredService<IApi>();
        Assert.NotNull(api);
    }

    [Fact]
    public void ResolveAllRepositoriesFromDI()
    {
        Assert.NotNull(_services.GetRequiredService<IAliasRepository>());
        Assert.NotNull(_services.GetRequiredService<IArchiveRepository>());
        Assert.NotNull(_services.GetRequiredService<IContentRepository>());
        Assert.NotNull(_services.GetRequiredService<IContentGroupRepository>());
        Assert.NotNull(_services.GetRequiredService<IContentTypeRepository>());
        Assert.NotNull(_services.GetRequiredService<ILanguageRepository>());
        Assert.NotNull(_services.GetRequiredService<IMediaRepository>());
        Assert.NotNull(_services.GetRequiredService<IPageRepository>());
        Assert.NotNull(_services.GetRequiredService<IPageTypeRepository>());
        Assert.NotNull(_services.GetRequiredService<IParamRepository>());
        Assert.NotNull(_services.GetRequiredService<IPostRepository>());
        Assert.NotNull(_services.GetRequiredService<IPostTypeRepository>());
        Assert.NotNull(_services.GetRequiredService<ISiteRepository>());
        Assert.NotNull(_services.GetRequiredService<ISiteTypeRepository>());
    }

    [Fact]
    public void ResolveOptionalServicesFromDI()
    {
        // These should be registered in CreateServiceCollection
        var storage = _services.GetService<IStorage>();
        var processor = _services.GetService<IImageProcessor>();
        var cache = _services.GetService<ICache>();

        Assert.NotNull(storage);
        Assert.NotNull(processor);
        Assert.NotNull(cache);
    }
}
