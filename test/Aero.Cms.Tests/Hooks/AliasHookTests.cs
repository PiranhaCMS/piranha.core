

using Xunit;
using Aero.Cms.Models;

namespace Aero.Cms.Tests.Hooks;

[Collection("Integration tests")]
public class AliasHookTests : BaseTestsAsync
{
    private const string ALIAS = "/alias-url";
    private readonly string SITE_ID = Snowflake.NewId();
    private readonly string ID = Snowflake.NewId();

    public class AliasOnLoadException : Exception {}
    public class AliasOnBeforeSaveException : Exception {}
    public class AliasOnAfterSaveException : Exception {}
    public class AliasOnBeforeDeleteException : Exception {}
    public class AliasOnAfterDeleteException : Exception {}

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        using var api = CreateApi();
        // Initialize
        Aero.Cms.App.Init(api);

        // Create site
        await api.Sites.SaveAsync(new Site
        {
            Id = SITE_ID,
            Title = "Alias Hook Site"
        });

        // Create test alias
        await api.Aliases.SaveAsync(new Alias
        {
            Id = ID,
            SiteId = SITE_ID,
            AliasUrl = ALIAS,
            RedirectUrl = "/redirect"
        });
    }

    public override async Task DisposeAsync()
    {
        using var api = CreateApi();
        // Remove test data
        var aliases = await api.Aliases.GetAllAsync();

        foreach (var a in aliases)
        {
            await api.Aliases.DeleteAsync(a);
        }
        await api.Sites.DeleteAsync(SITE_ID);
    }

    [Fact]
    public async Task OnLoad()
    {
        Aero.Cms.App.Hooks.Alias.RegisterOnLoad(m => throw new AliasOnLoadException());
        using (var api = CreateApi())
        {
            await Assert.ThrowsAsync<AliasOnLoadException>(async () =>
            {
                await api.Aliases.GetByIdAsync(ID);
            });
        }
        Aero.Cms.App.Hooks.Alias.Clear();
    }

    [Fact]
    public async Task OnBeforeSave()
    {
        Aero.Cms.App.Hooks.Alias.RegisterOnBeforeSave(m => throw new AliasOnBeforeSaveException());
        using (var api = CreateApi())
        {
            await Assert.ThrowsAsync<AliasOnBeforeSaveException>(async () =>
            {
                await api.Aliases.SaveAsync(new Alias
                {
                    SiteId = SITE_ID,
                    AliasUrl = "/my-first-alias",
                    RedirectUrl = "/my-first-redirect"
                });
            });
        }
        Aero.Cms.App.Hooks.Alias.Clear();
    }

    [Fact]
    public async Task OnAfterSave()
    {
        Aero.Cms.App.Hooks.Alias.RegisterOnAfterSave(m => throw new AliasOnAfterSaveException());
        using (var api = CreateApi())
        {
            await Assert.ThrowsAsync<AliasOnAfterSaveException>(async () =>
            {
                await api.Aliases.SaveAsync(new Alias
                {
                    SiteId = SITE_ID,
                    AliasUrl = "/my-second-alias",
                    RedirectUrl = "/my-seconf-redirect"
                });
            });
        }
        Aero.Cms.App.Hooks.Alias.Clear();
    }

    [Fact]
    public async Task OnBeforeDelete()
    {
        Aero.Cms.App.Hooks.Alias.RegisterOnBeforeDelete(m => throw new AliasOnBeforeDeleteException());
        using (var api = CreateApi())
        {
            await Assert.ThrowsAsync<AliasOnBeforeDeleteException>(async () =>
            {
                await api.Aliases.DeleteAsync(ID);
            });
        }
        Aero.Cms.App.Hooks.Alias.Clear();
    }

    [Fact]
    public async Task OnAfterDelete()
    {
        Aero.Cms.App.Hooks.Alias.RegisterOnAfterDelete(m => throw new AliasOnAfterDeleteException());
        using (var api = CreateApi())
        {
            await Assert.ThrowsAsync<AliasOnAfterDeleteException>(async () =>
            {
                await api.Aliases.DeleteAsync(ID);
            });
        }
        Aero.Cms.App.Hooks.Alias.Clear();
    }
}
