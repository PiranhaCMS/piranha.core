

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Xunit;
using Aero.Cms.AttributeBuilder;
using Aero.Cms.Extend;
using Aero.Cms.Extend.Fields;
using Aero.Cms.Models;

namespace Aero.Cms.Tests.Services;

[Collection("Integration tests")]
public class ContentTestsMemoryCache : ContentTests
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        cache = new Cache.MemoryCache((IMemoryCache)services.GetService(typeof(IMemoryCache)));
    }
}

[Collection("Integration tests")]
public class ContentTestsDistributedCache : ContentTests
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        cache = new Cache.DistributedCache((IDistributedCache)services.GetService(typeof(IDistributedCache)));
    }
}

[Collection("Integration tests")]
public class ContentTests : BaseTestsAsync
{
    private readonly string ID1 = Snowflake.NewId();
    private readonly string ID2 = Snowflake.NewId();
    private readonly string ID3 = Snowflake.NewId();

    private readonly string IDLANG = Snowflake.NewId();

    [ContentGroup(Id = "MyContentGroup", Title = "My content group")]
    public abstract class MyContentGroup<T> : Content<T> where T : MyContentGroup<T>
    {
    }

    [ContentType(Id = "MyContent", Title = "My content")]
    public class MyContent : MyContentGroup<MyContent>
    {
        [Region]
        public HtmlField MainDescription { get; set; }
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        using var api = CreateApi();
        Aero.Cms.App.Init(api);

        // Add the content type
        var builder = new ContentTypeBuilder(api)
            .AddType(typeof(MyContent));
        await builder.BuildAsync();

        // Add a secondary language
        await api.Languages.SaveAsync(new Language
        {
            Id = IDLANG,
            Title = "Second Language",
            Culture = "sv-SE"
        });

        // Add some default content
        var content1 = await MyContent.CreateAsync(api);
        content1.Id = ID1;
        content1.Title = "My first content";
        content1.Excerpt = "My first excerpt";
        content1.MainDescription = "My first description";

        await api.Content.SaveAsync(content1);

        var content2 = await MyContent.CreateAsync(api);
        content2.Id = ID2;
        content2.Title = "My second content";
        content2.Excerpt = "My second excerpt";
        content2.MainDescription = "My second description";

        await api.Content.SaveAsync(content2);

        var content3 = await MyContent.CreateAsync(api);
        content3.Id = ID3;
        content3.Title = "My third content";
        content3.Excerpt = "My third excerpt";
        content3.MainDescription = "My third description";

        await api.Content.SaveAsync(content3);

        // Now let's translate content 1
        content1.Title = "Mitt första innehåll";
        content1.Excerpt = "Min första sammanfattning";
        content1.MainDescription = "Min första beskrivning";

        await api.Content.SaveAsync(content1, IDLANG);
    }

    public override async Task DisposeAsync()
    {
        using var api = CreateApi();
        // Delete added content
        var content = await api.Content.GetAllAsync();
        foreach (var c in content)
        {
            await api.Content.DeleteAsync(c);
        }

        // Delete added content groups
        var groups = await api.ContentGroups.GetAllAsync();
        foreach (var g in groups)
        {
            await api.ContentGroups.DeleteAsync(g);
        }

        // Delete added language
        await api.Languages.DeleteAsync(IDLANG);
    }

    [Fact]
    public async Task GetById()
    {
        using var api = CreateApi();
        var content = await api.Content.GetByIdAsync<MyContent>(ID1);
        var contentInfo = await api.Content.GetByIdAsync<ContentInfo>(ID1);

        Assert.NotNull(content);
        Assert.NotNull(contentInfo);
        Assert.Equal("My first content", content.Title);
        Assert.Equal(content.Title, contentInfo.Title);
    }

    [Fact]
    public async Task GetTranslationById()
    {
        using var api = CreateApi();
        var content = await api.Content.GetByIdAsync<MyContent>(ID1, IDLANG);
        var contentInfo = await api.Content.GetByIdAsync<ContentInfo>(ID1, IDLANG);

        Assert.NotNull(content);
        Assert.NotNull(contentInfo);
        Assert.Equal("Mitt första innehåll", content.Title);
        Assert.Equal(content.Title, contentInfo.Title);
    }

    [Fact]
    public async Task GetTranslatedStatus()
    {
        using var api = CreateApi();
        var status = await api.Content.GetTranslationStatusByIdAsync(ID1);

        Assert.NotNull(status);

        Assert.True(status.IsUpToDate);
        Assert.Equal(1, status.UpToDateCount);
        Assert.Equal(1, status.TotalCount);
    }

    [Fact]
    public async Task GetUntranslatedStatus()
    {
        using var api = CreateApi();
        var status = await api.Content.GetTranslationStatusByIdAsync(ID2);

        Assert.NotNull(status);

        Assert.False(status.IsUpToDate);
        Assert.Equal(0, status.UpToDateCount);
        Assert.Equal(1, status.TotalCount);
    }

    [Fact]
    public async Task GetTranslationSummary()
    {
        using var api = CreateApi();
        var test = await api.Content.GetAllAsync();

        var summary = await api.Content.GetTranslationStatusByGroupAsync("MyContentGroup");

        Assert.NotNull(summary);

        Assert.False(summary.IsUpToDate);
        Assert.Equal(1, summary.UpToDateCount);
        Assert.Equal(3, summary.TotalCount);
    }
}
