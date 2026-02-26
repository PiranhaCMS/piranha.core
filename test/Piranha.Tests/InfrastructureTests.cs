
using Microsoft.Extensions.DependencyInjection;
using Piranha;
using Piranha.Cache;
using Piranha.Repositories;
using Xunit;

namespace Piranha.Tests;

public class InfrastructureTests : BaseTestsAsync
{
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
        Assert.NotNull(_services.GetService<IStorage>());
        Assert.NotNull(_services.GetService<IImageProcessor>());
        Assert.NotNull(_services.GetService<ICache>());
    }
}
