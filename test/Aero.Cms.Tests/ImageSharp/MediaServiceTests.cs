

namespace Aero.Cms.Tests.ImageSharp;

//[Collection("Integration tests")]
public class MediaServiceTests(MartenFixture fixture) : AsyncTestBase(fixture)
{
    private string imageId;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        
        // Add media
        using var stream = File.OpenRead("Assets/HLD_Screenshot_01_mech_1080.png");
        var image1 = new Models.StreamMediaContent()
        {
            Filename = "HLD_Screenshot_01_mech_1080.png",
            Data = stream
        };
        await api.Media.SaveAsync(image1);

        imageId = image1.Id;
    }
    public override async Task DisposeAsync()
    {
        
        await api.Media.DeleteAsync(imageId);
    }

    [Fact]
    public async Task GetOriginal()
    {
        
        var media = await api.Media.GetByIdAsync(imageId);

        Assert.NotNull(media);
        Assert.Equal($"~/uploads/{imageId}-{media.Filename}", media.PublicUrl);
    }

    [Fact]
    public async Task GetScaled()
    {
        
        var url = await api.Media.EnsureVersionAsync(imageId, 640);

        Assert.NotNull(url);
        Assert.Equal($"~/uploads/{imageId}-HLD_Screenshot_01_mech_1080_640.png", url);
    }

    [Fact]
    public async Task GetCropped()
    {
        
        var url = await api.Media.EnsureVersionAsync(imageId, 640, 300);

        Assert.NotNull(url);
        Assert.Equal($"~/uploads/{imageId}-HLD_Screenshot_01_mech_1080_640x300.png", url);
    }

    [Fact]
    public async Task GetScaledOrgSize()
    {
        
        var url = await api.Media.EnsureVersionAsync(imageId, 1920);

        Assert.NotNull(url);
        Assert.Equal($"~/uploads/{imageId}-HLD_Screenshot_01_mech_1080.png", url);
    }

    [Fact]
    public async Task GetCroppedOrgSize()
    {
        
        var url = await api.Media.EnsureVersionAsync(imageId, 1920, 1080);

        Assert.NotNull(url);
        Assert.Equal($"~/uploads/{imageId}-HLD_Screenshot_01_mech_1080.png", url);
    }
}
