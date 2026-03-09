

using Xunit;
using Aero.Cms.Models;

namespace Aero.Cms.Tests.Hooks;

[Collection("Integration tests")]
public class SiteHookTests : BaseTestsAsync
{
    private const string TITLE = "My Hook Site";
    private readonly string ID = Snowflake.NewId();

    public class SiteOnLoadException : Exception {}
    public class SiteOnBeforeSaveException : Exception {}
    public class SiteOnAfterSaveException : Exception {}
    public class SiteOnBeforeDeleteException : Exception {}
    public class SiteOnAfterDeleteException : Exception {}

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        using var api = CreateApi();
        // Initialize
        Aero.Cms.App.Init(api);

        // Create test param
        await api.Sites.SaveAsync(new Site
        {
            Id = ID,
            Title = TITLE
        });
    }

    public override async Task DisposeAsync()
    {
        using var api = CreateApi();
        // Remove test data
        var sites = await api.Sites.GetAllAsync();

        foreach (var s in sites)
        {
            await api.Sites.DeleteAsync(s);
        }
    }

    [Fact]
    public async Task OnLoad()
    {
        Aero.Cms.App.Hooks.Site.RegisterOnLoad(m => throw new SiteOnLoadException());
        using (var api = CreateApi())
        {
            await Assert.ThrowsAsync<SiteOnLoadException>(async () =>
            {
                await api.Sites.GetByIdAsync(ID);
            });
        }
        Aero.Cms.App.Hooks.Site.Clear();
    }

    [Fact]
    public async Task OnBeforeSave()
    {
        Aero.Cms.App.Hooks.Site.RegisterOnBeforeSave(m => throw new SiteOnBeforeSaveException());
        using (var api = CreateApi())
        {
            await Assert.ThrowsAsync<SiteOnBeforeSaveException>(async () =>
            {
                await api.Sites.SaveAsync(new Site {
                    Title = "My First Hook Site"
                });
            });
        }
        Aero.Cms.App.Hooks.Site.Clear();
    }

    [Fact]
    public async Task OnAfterSave()
    {
        Aero.Cms.App.Hooks.Site.RegisterOnAfterSave(m => throw new SiteOnAfterSaveException());
        using (var api = CreateApi())
        {
            await Assert.ThrowsAsync<SiteOnAfterSaveException>(async () =>
            {
                await api.Sites.SaveAsync(new Site {
                    Title = "My Second Hook Site"
                });
            });
        }
        Aero.Cms.App.Hooks.Site.Clear();
    }

    [Fact]
    public async Task OnBeforeDelete()
    {
        Aero.Cms.App.Hooks.Site.RegisterOnBeforeDelete(m => throw new SiteOnBeforeDeleteException());
        using (var api = CreateApi())
        {
            await Assert.ThrowsAsync<SiteOnBeforeDeleteException>(async () =>
            {
                await api.Sites.DeleteAsync(ID);
            });
        }
        Aero.Cms.App.Hooks.Site.Clear();
    }

    [Fact]
    public async Task OnAfterDelete()
    {
        Aero.Cms.App.Hooks.Site.RegisterOnAfterDelete(m => throw new SiteOnAfterDeleteException());
        using (var api = CreateApi())
        {
            await Assert.ThrowsAsync<SiteOnAfterDeleteException>(async () =>
            {
                await api.Sites.DeleteAsync(ID);
            });
        }
        Aero.Cms.App.Hooks.Site.Clear();
    }
}
