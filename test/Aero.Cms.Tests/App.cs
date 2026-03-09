using Aero.Cms.RavenDb.Repositories;
using Aero.Cms.RavenDb.Services.Internal;
using Xunit;
using Aero.Cms.Services;

namespace Aero.Cms.Tests;

public class App : BaseTests
{
    /// <summary>
    /// Sets up & initializes the tests.
    /// </summary>
    protected override void Init()
    {
        using var api = CreateApi();
        Aero.Cms.App.Init(api);
    }

    /// <summary>
    /// Cleans up any possible data and resources
    /// created by the test.
    /// </summary>
    protected override void Cleanup() { }

    [Fact]
    public void AppInit()
    {
        using var api = CreateApi();
        Aero.Cms.App.Init(api);
    }

    [Fact]
    public void Markdown() {
        var str =
            "# This is the title\n" +
            "This is the body";

        str = Aero.Cms.App.Markdown.Transform(str)
            .Replace("\n", "");

        Assert.Equal("<h1>This is the title</h1><p>This is the body</p>", str);
    }

    [Fact]
    public void MarkdownEmptyString() {
        Assert.Equal("", Aero.Cms.App.Markdown.Transform(""));
    }

    [Fact]
    public void MarkdownNullString() {
        Assert.Null(Aero.Cms.App.Markdown.Transform(null));
    }

    [Fact]
    public void Fields() {
        Assert.NotNull(Aero.Cms.App.Fields);
        Assert.NotEmpty(Aero.Cms.App.Fields);
    }

    [Fact]
    public void Modules() {
        Assert.NotNull(Aero.Cms.App.Modules);
    }

    [Fact]
    public void PropertyBindings() {
        Assert.True(Aero.Cms.App.PropertyBindings.HasFlag(System.Reflection.BindingFlags.IgnoreCase));
        Assert.True(Aero.Cms.App.PropertyBindings.HasFlag(System.Reflection.BindingFlags.Public));
        Assert.True(Aero.Cms.App.PropertyBindings.HasFlag(System.Reflection.BindingFlags.Instance));
    }

    private IApi CreateApi()
    {
        var factory = new ContentFactory(services);
        var serviceFactory = new ContentServiceFactory(factory);

        var db = GetDb();

        return new Api(
            factory,
            new AliasRepository(db),
            new ArchiveRepository(db),
            new ContentRepository(db, serviceFactory),
            new ContentGroupRepository(db),
            new ContentTypeRepository(db),
            new LanguageRepository(db),
            new MediaRepository(db),
            new PageRepository(db, serviceFactory),
            new PageTypeRepository(db),
            new ParamRepository(db),
            new PostRepository(db, serviceFactory),
            new PostTypeRepository(db),
            new SiteRepository(db, serviceFactory),
            new SiteTypeRepository(db)
        );
    }
}