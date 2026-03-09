

using Xunit;
using Aero.Cms.Models;

namespace Aero.Cms.Tests.Hooks;

[Collection("Integration tests")]
public class ParamHookTests : BaseTestsAsync
{
    private const string KEY = "MyHookParam";
    private readonly string ID = Snowflake.NewId();

    public class ParamOnLoadException : Exception {}
    public class ParamOnBeforeSaveException : Exception {}
    public class ParamOnAfterSaveException : Exception {}
    public class ParamOnBeforeDeleteException : Exception {}
    public class ParamOnAfterDeleteException : Exception {}

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        using var api = CreateApi();
        // Initialize
        Aero.Cms.App.Init(api);

        // Create test param
        await api.Params.SaveAsync(new Param
        {
            Id = ID,
            Key = KEY
        });
    }

    public override async Task DisposeAsync()
    {
        using var api = CreateApi();
        // Remove test data
        var param = await api.Params.GetAllAsync();

        foreach (var p in param)
        {
            await api.Params.DeleteAsync(p);
        }
    }

    [Fact]
    public async Task OnLoad()
    {
        Aero.Cms.App.Hooks.Param.RegisterOnLoad(m => throw new ParamOnLoadException());
        using (var api = CreateApi())
        {
            await Assert.ThrowsAsync<ParamOnLoadException>(async () =>
            {
                await api.Params.GetByIdAsync(ID);
            });
        }
        Aero.Cms.App.Hooks.Param.Clear();
    }

    [Fact]
    public async Task OnBeforeSave()
    {
        Aero.Cms.App.Hooks.Param.RegisterOnBeforeSave(m => throw new ParamOnBeforeSaveException());
        using (var api = CreateApi())
        {
            await Assert.ThrowsAsync<ParamOnBeforeSaveException>(async () =>
            {
                await api.Params.SaveAsync(new Param {
                    Key = "MyFirstHookKey"
                });
            });
        }
        Aero.Cms.App.Hooks.Param.Clear();
    }

    [Fact]
    public async Task OnAfterSave()
    {
        Aero.Cms.App.Hooks.Param.RegisterOnAfterSave(m => throw new ParamOnAfterSaveException());
        using (var api = CreateApi())
        {
            await Assert.ThrowsAsync<ParamOnAfterSaveException>(async () =>
            {
                await api.Params.SaveAsync(new Param {
                    Key = "MySecondHookKey"
                });
            });
        }
        Aero.Cms.App.Hooks.Param.Clear();
    }

    [Fact]
    public async Task OnBeforeDelete()
    {
        Aero.Cms.App.Hooks.Param.RegisterOnBeforeDelete(m => throw new ParamOnBeforeDeleteException());
        using (var api = CreateApi())
        {
            await Assert.ThrowsAsync<ParamOnBeforeDeleteException>(async () =>
            {
                await api.Params.DeleteAsync(ID);
            });
        }
        Aero.Cms.App.Hooks.Param.Clear();
    }

    [Fact]
    public async Task OnAfterDelete()
    {
        Aero.Cms.App.Hooks.Param.RegisterOnAfterDelete(m => throw new ParamOnAfterDeleteException());
        using (var api = CreateApi())
        {
            await Assert.ThrowsAsync<ParamOnAfterDeleteException>(async () =>
            {
                await api.Params.DeleteAsync(ID);
            });
        }
        Aero.Cms.App.Hooks.Param.Clear();
    }
}
