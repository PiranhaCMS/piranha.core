/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Piranha.Models;

namespace Piranha.Tests.Services
{
    [Collection("Integration tests")]
    public class MediaTestsCached : MediaTests
    {
        public override Task InitializeAsync()
        {
            _cache = new Cache.SimpleCache();

            return base.InitializeAsync();
        }
    }

    [Collection("Integration tests")]
    public class MediaTests : BaseTestsAsync
    {
        private Guid image1Id;
        private Guid image2Id;
        private Guid image3Id;
        private Guid image4Id;
        private Guid folder1Id;

        public override async Task InitializeAsync()
        {
            using (var api = CreateApi())
            {
                Piranha.App.Init(api);

                // Add media folders
                var folder1 = new MediaFolder
                {
                    Name = "Images"
                };
                await api.Media.SaveFolderAsync(folder1);
                folder1Id = folder1.Id;

                // Add media
                using (var stream = File.OpenRead("../../../Assets/HLD_Screenshot_01_mech_1080.png"))
                {
                    var image1 = new Models.StreamMediaContent
                    {
                        Filename = "HLD_Screenshot_01_mech_1080.png",
                        Data = stream
                    };
                    await api.Media.SaveAsync(image1);

                    image1Id = image1.Id.Value;

                    // Add some additional meta data
                    var image = await api.Media.GetByIdAsync(image1Id);
                    image.Title = "Screenshot";
                    image.AltText = "This is a screenshot";
                    image.Description = "Screenshot from Hyper Light Drifter";
                    image.Properties["Game"] = "Hyper Light Drifter";

                    await api.Media.SaveAsync(image);
                }

                using (var stream = File.OpenRead("../../../Assets/HLD_Screenshot_01_rise_1080.png"))
                {
                    var image2 = new Models.StreamMediaContent
                    {
                        FolderId = folder1Id,
                        Filename = "HLD_Screenshot_01_rise_1080.png",
                        Data = stream
                    };
                    await api.Media.SaveAsync(image2);

                    image2Id = image2.Id.Value;
                }

                using (var stream = File.OpenRead("../../../Assets/HLD_Screenshot_01_robot_1080.png"))
                {
                    var image3 = new Models.StreamMediaContent
                    {
                        Filename = "HLD_Screenshot_01_robot_1080.png",
                        Data = stream
                    };
                    await api.Media.SaveAsync(image3);

                    image3Id = image3.Id.Value;
                }
            }
        }

        public override async Task DisposeAsync()
        {
            using (var api = CreateApi())
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
        }

        [Fact]
        public void IsCached()
        {
            using (var api = CreateApi())
            {
                Assert.Equal(this.GetType() == typeof(MediaTestsCached), ((Api)api).IsCached);
            }
        }

        [Fact]
        public async Task GetAll()
        {
            using (var api = CreateApi())
            {
                var media = await api.Media.GetAllByFolderIdAsync();

                Assert.NotEmpty(media);
            }
        }

        [Fact]
        public async Task GetById()
        {
            using (var api = CreateApi())
            {
                var media = await api.Media.GetByIdAsync(image1Id);

                Assert.NotNull(media);
                Assert.Equal("HLD_Screenshot_01_mech_1080.png", media.Filename);
                Assert.Equal("image/png", media.ContentType);
                Assert.Equal(Models.MediaType.Image, media.Type);
            }
        }

        [Fact]
        public async Task GetByFolderId()
        {
            using (var api = CreateApi())
            {
                var media = (await api.Media.GetAllByFolderIdAsync(folder1Id)).ToList();

                Assert.NotEmpty(media);
                Assert.Equal("HLD_Screenshot_01_rise_1080.png", media[0].Filename);
            }
        }

        [Fact]
        public async Task TitleNotNull()
        {
            using (var api = CreateApi())
            {
                var media = await api.Media.GetByIdAsync(image1Id);

                Assert.NotNull(media.Title);
                Assert.Equal("Screenshot", media.Title);
            }
        }

        [Fact]
        public async Task AltTextNotNull()
        {
            using (var api = CreateApi())
            {
                var media = await api.Media.GetByIdAsync(image1Id);

                Assert.NotNull(media.AltText);
                Assert.Equal("This is a screenshot", media.AltText);
            }
        }

        [Fact]
        public async Task DescriptionNotNull()
        {
            using (var api = CreateApi())
            {
                var media = await api.Media.GetByIdAsync(image1Id);

                Assert.NotNull(media.Description);
                Assert.Equal("Screenshot from Hyper Light Drifter", media.Description);
            }
        }

        [Fact]
        public async Task HasProperty()
        {
            using (var api = CreateApi())
            {
                var media = await api.Media.GetByIdAsync(image1Id);

                Assert.Equal("Hyper Light Drifter", media.Properties["Game"]);
            }
        }

        [Fact]
        public async Task Move()
        {
            using (var api = CreateApi())
            {
                var media = await api.Media.GetByIdAsync(image1Id);
                Assert.NotNull(media);
                Assert.Null(media.FolderId);
                await api.Media.MoveAsync(media, folder1Id);

                media = await api.Media.GetByIdAsync(image1Id);
                Assert.NotNull(media.FolderId);
                Assert.Equal(folder1Id, media.FolderId.Value);

                await api.Media.MoveAsync(media, null);
            }
        }

        [Fact]
        public async Task Insert()
        {
            using (var api = CreateApi())
            {
                using (var stream = File.OpenRead("../../../Assets/HLD_Screenshot_BETA_entrance.png"))
                {
                    var image = new Models.StreamMediaContent
                    {
                        Filename = "HLD_Screenshot_BETA_entrance.png",
                        Data = stream
                    };
                    await api.Media.SaveAsync(image);

                    Assert.NotNull(image.Id);

                    image4Id = image.Id.Value;
                }
            }
        }

        [Fact]
        public async Task PublicUrl()
        {
            using (var api = CreateApi())
            {
                using (var config = new Piranha.Config(api))
                {
                    config.MediaCDN = null;
                }

                var media = await api.Media.GetByIdAsync(image1Id);

                Assert.NotNull(media);
                Assert.Equal($"~/uploads/{image1Id}-HLD_Screenshot_01_mech_1080.png", media.PublicUrl);
            }
        }

        [Fact]
        public async Task PublicUrlCDN()
        {
            using (var api = CreateApi())
            {
                using (var config = new Piranha.Config(api))
                {
                    config.MediaCDN = "https://mycdn.org/uploads";
                }

                var media = await api.Media.GetByIdAsync(image1Id);

                Assert.NotNull(media);
                Assert.Equal($"https://mycdn.org/uploads/{image1Id}-HLD_Screenshot_01_mech_1080.png", media.PublicUrl);
            }
        }

        [Fact]
        public async Task Delete()
        {
            using (var api = CreateApi())
            {
                var media = await api.Media.GetByIdAsync(image3Id);

                await api.Media.DeleteAsync(media);
            }
        }

        [Fact]
        public async Task DeleteById()
        {
            using (var api = CreateApi())
            {
                await api.Media.DeleteAsync(image4Id);
            }
        }
    }
}