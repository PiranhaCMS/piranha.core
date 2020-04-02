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
using System.Threading.Tasks;
using Xunit;
using Piranha.ImageSharp;
using Piranha.Repositories;
using Piranha.Services;

namespace Piranha.Tests.ImageSharp
{
    [Collection("Integration tests")]
    public class MediaServiceTests : BaseTestsAsync
    {
        private Guid imageId;

        public override async Task InitializeAsync()
        {
            using (var api = CreateApi())
            {
                // Add media
                using (var stream = File.OpenRead("../../../Assets/HLD_Screenshot_01_mech_1080.png"))
                {
                    var image1 = new Models.StreamMediaContent()
                    {
                        Filename = "HLD_Screenshot_01_mech_1080.png",
                        Data = stream
                    };
                    await api.Media.SaveAsync(image1);

                    imageId = image1.Id.Value;
                }
            }
        }
        public override async Task DisposeAsync()
        {
            using (var api = CreateApi())
            {
                await api.Media.DeleteAsync(imageId);
            }
        }

        [Fact]
        public async Task GetOriginal()
        {
            using (var api = CreateApi())
            {
                var media = await api.Media.GetByIdAsync(imageId);

                Assert.NotNull(media);
                Assert.Equal($"~/uploads/{imageId}-{media.Filename}", media.PublicUrl);
            }
        }

        [Fact]
        public async Task GetScaled()
        {
            using (var api = CreateApi())
            {
                var url = await api.Media.EnsureVersionAsync(imageId, 640);

                Assert.NotNull(url);
                Assert.Equal($"~/uploads/{imageId}-HLD_Screenshot_01_mech_1080_640.png", url);
            }
        }

        [Fact]
        public async Task GetCropped()
        {
            using (var api = CreateApi())
            {
                var url = await api.Media.EnsureVersionAsync(imageId, 640, 300);

                Assert.NotNull(url);
                Assert.Equal($"~/uploads/{imageId}-HLD_Screenshot_01_mech_1080_640x300.png", url);
            }
        }

        [Fact]
        public async Task GetScaledOrgSize()
        {
            using (var api = CreateApi())
            {
                var url = await api.Media.EnsureVersionAsync(imageId, 1920);

                Assert.NotNull(url);
                Assert.Equal($"~/uploads/{imageId}-HLD_Screenshot_01_mech_1080.png", url);
            }
        }

        [Fact]
        public async Task GetCroppedOrgSize()
        {
            using (var api = CreateApi())
            {
                var url = await api.Media.EnsureVersionAsync(imageId, 1920, 1080);

                Assert.NotNull(url);
                Assert.Equal($"~/uploads/{imageId}-HLD_Screenshot_01_mech_1080.png", url);
            }
        }
    }
}
