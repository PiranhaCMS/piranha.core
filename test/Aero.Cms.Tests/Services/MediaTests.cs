

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Aero.Cms.Models;

namespace Aero.Cms.Tests.Services;

//[Collection("Integration tests")]
public class MediaTestsMemoryCache(MartenFixture fixture) : MediaTests(fixture)
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        cache = new Cache.MemoryCache((IMemoryCache)services.GetService(typeof(IMemoryCache)));
    }
}

//[Collection("Integration tests")]
public class MediaTestsDistributedCache(MartenFixture fixture) : MediaTests(fixture)
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        cache = new Cache.DistributedCache((IDistributedCache)services.GetService(typeof(IDistributedCache)));
    }
}

//[Collection("Integration tests")]
public class MediaTests(MartenFixture fixture) : AsyncTestBase(fixture)
{
    private string image1Id;
    private string image2Id;
    private string image3Id;
    private string image4Id;
    private string image5Id;
    private string folder1Id;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        
        Aero.Cms.App.Init(api);

        // Add media folders
        var folder1 = new MediaFolder
        {
            Name = "Images"
        };
        await api.Media.SaveFolderAsync(folder1);
        folder1Id = folder1.Id;

        // Add media
        using (var stream = File.OpenRead("Assets/HLD_Screenshot_01_mech_1080.png"))
        {
            var image1 = new Models.StreamMediaContent
            {
                Filename = "HLDScreenshot01mech1080.png",
                Data = stream
            };
            await api.Media.SaveAsync(image1);

            image1Id = image1.Id;

            // Add some additional meta data
            var image = await api.Media.GetByIdAsync(image1Id);
            image.Title = "Screenshot";
            image.AltText = "This is a screenshot";
            image.Description = "Screenshot from Hyper Light Drifter";
            image.Properties["Game"] = "Hyper Light Drifter";

            await api.Media.SaveAsync(image);
        }

        using (var stream = File.OpenRead("Assets/HLD_Screenshot_01_rise_1080.png"))
        {
            var image2 = new Models.StreamMediaContent
            {
                FolderId = folder1Id,
                Filename = "HLDScreenshot01rise1080.png",
                Data = stream
            };
            await api.Media.SaveAsync(image2);

            image2Id = image2.Id;
        }

        using (var stream = File.OpenRead("Assets/HLD_Screenshot_01_robot_1080.png"))
        {
            var image3 = new Models.StreamMediaContent
            {
                Filename = "HLDScreenshot01robot1080.png",
                Data = stream
            };
            await api.Media.SaveAsync(image3);

            image3Id = image3.Id;
        }

        using (var stream = File.OpenRead("Assets/HLD Screenshot 01 mech 1080.png"))
        {
            var image5 = new Models.StreamMediaContent
            {
                Filename = "HLD Screenshot 01 mech 1080.png",
                Data = stream
            };
            await api.Media.SaveAsync(image5);

            image5Id = image5.Id;
        }
    }

    public override async Task DisposeAsync()
    {
        
        var media = await api.Media.GetAllByFolderIdAsync();

        foreach (var item in media)
        {
            await api.Media.DeleteAsync(item);
        }

        var folders = await api.Media.GetAllFoldersAsync();

        foreach (var folder in folders)
        {
            media = await api.Media.GetAllByFolderIdAsync(folder.Id);

            foreach (var item in media)
            {
                await api.Media.DeleteAsync(item);
            }
            await api.Media.DeleteFolderAsync(folder);
        }
    }

    [Fact]
    public void IsCached()
    {
        
        Assert.Equal(((Api)api).IsCached,
            this.GetType() == typeof(MediaTestsMemoryCache) ||
            this.GetType() == typeof(MediaTestsDistributedCache));
    }

    [Fact]
    public async Task GetAll()
    {
        
        var media = await api.Media.GetAllByFolderIdAsync();

        Assert.NotEmpty(media);
    }

    [Fact]
    public async Task GetById()
    {
        
        var media = await api.Media.GetByIdAsync(image1Id);

        Assert.NotNull(media);
        Assert.Equal("HLDScreenshot01mech1080.png", media.Filename);
        Assert.Equal("image/png", media.ContentType);
        Assert.Equal(Models.MediaType.Image, media.Type);
    }

    [Fact]
    public async Task GetByFolderId()
    {
        
        var media = (await api.Media.GetAllByFolderIdAsync(folder1Id)).ToList();

        Assert.NotEmpty(media);
        Assert.Equal("HLDScreenshot01rise1080.png", media[0].Filename);
    }

    [Fact]
    public async Task FilenameHasNoSpaces()
    {
        
        var media = await api.Media.GetByIdAsync(image5Id);

        Assert.NotNull(media);
        Assert.Equal("HLD_Screenshot_01_mech_1080.png", media.Filename);
    }

    [Fact]
    public async Task TitleNotNull()
    {
        
        var media = await api.Media.GetByIdAsync(image1Id);

        Assert.NotNull(media.Title);
        Assert.Equal("Screenshot", media.Title);
    }

    [Fact]
    public async Task AltTextNotNull()
    {
        
        var media = await api.Media.GetByIdAsync(image1Id);

        Assert.NotNull(media.AltText);
        Assert.Equal("This is a screenshot", media.AltText);
    }

    [Fact]
    public async Task DescriptionNotNull()
    {
        
        var media = await api.Media.GetByIdAsync(image1Id);

        Assert.NotNull(media.Description);
        Assert.Equal("Screenshot from Hyper Light Drifter", media.Description);
    }

    [Fact]
    public async Task HasProperty()
    {
        
        var media = await api.Media.GetByIdAsync(image1Id);

        Assert.Equal("Hyper Light Drifter", media.Properties["Game"]);
    }

    [Fact]
    public async Task Move()
    {
        
        var media = await api.Media.GetByIdAsync(image1Id);
        Assert.NotNull(media);
        Assert.Null(media.FolderId);
        await api.Media.MoveAsync(media, folder1Id);

        media = await api.Media.GetByIdAsync(image1Id);
        Assert.NotNull(media.FolderId);
        Assert.Equal(folder1Id, media.FolderId);

        await api.Media.MoveAsync(media, null);
    }

    [Fact]
    public async Task Insert()
    {
        
        using var stream = File.OpenRead("Assets/HLD_Screenshot_BETA_entrance.png");
        var image = new Models.StreamMediaContent
        {
            Filename = "HLDScreenshotBETAentrance.png",
            Data = stream
        };
        await api.Media.SaveAsync(image);

        Assert.NotNull(image.Id);

        image4Id = image.Id;
    }

    [Fact]
    public async Task PublicUrl()
    {
        
        using (var config = new Aero.Cms.Config(api))
        {
            config.MediaCDN = null;
        }

        var media = await api.Media.GetByIdAsync(image1Id);

        Assert.NotNull(media);
        Assert.Equal($"~/uploads/{image1Id}-HLDScreenshot01mech1080.png", media.PublicUrl);
    }

    [Fact]
    public async Task PublicUrlCDN()
    {
        
        using (var config = new Aero.Cms.Config(api))
        {
            config.MediaCDN = "https://mycdn.org/uploads";
        }

        var media = await api.Media.GetByIdAsync(image1Id);

        Assert.NotNull(media);
        Assert.Equal($"https://mycdn.org/uploads/{image1Id}-HLDScreenshot01mech1080.png", media.PublicUrl);
    }

    [Fact]
    public async Task Delete()
    {
        
        var media = await api.Media.GetByIdAsync(image3Id);

        await api.Media.DeleteAsync(media);
    }

    [Fact]
    public async Task DeleteById()
    {
        
        await api.Media.DeleteAsync(image4Id);
    }
}
