
using Microsoft.Extensions.DependencyInjection;
using Aero.Cms.Cache;
using Aero.Cms.Repositories;
using Xunit;

namespace Aero.Cms.Tests;

public class InfrastructureTests : BaseTestsAsync
{
    public async override Task InitializeAsync()
    {
        await base.InitializeAsync();
    }

    [Fact]
    public void ResolveApiFromDI()
    {
        var api = services.GetRequiredService<IApi>();
        Assert.NotNull(api);
    }

    [Fact]
    public void ResolveAllRepositoriesFromDI()
    {
        Assert.NotNull(services.GetRequiredService<IAliasRepository>());
        Assert.NotNull(services.GetRequiredService<IArchiveRepository>());
        Assert.NotNull(services.GetRequiredService<IContentRepository>());
        Assert.NotNull(services.GetRequiredService<IContentGroupRepository>());
        Assert.NotNull(services.GetRequiredService<IContentTypeRepository>());
        Assert.NotNull(services.GetRequiredService<ILanguageRepository>());
        Assert.NotNull(services.GetRequiredService<IMediaRepository>());
        Assert.NotNull(services.GetRequiredService<IPageRepository>());
        Assert.NotNull(services.GetRequiredService<IPageTypeRepository>());
        Assert.NotNull(services.GetRequiredService<IParamRepository>());
        Assert.NotNull(services.GetRequiredService<IPostRepository>());
        Assert.NotNull(services.GetRequiredService<IPostTypeRepository>());
        Assert.NotNull(services.GetRequiredService<ISiteRepository>());
        Assert.NotNull(services.GetRequiredService<ISiteTypeRepository>());
    }

    [Fact]
    public void ResolveOptionalServicesFromDI()
    {
        // These should be registered in CreateServiceCollection
        var storage = services.GetService<IStorage>();
        var processor = services.GetService<IImageProcessor>();
        var cache = services.GetService<ICache>();

        Assert.NotNull(storage);
        Assert.NotNull(processor);
        Assert.NotNull(cache);
    }
}
