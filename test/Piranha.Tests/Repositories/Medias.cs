/*
 * Copyright (c) 2017-2019 Håkan Edling
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
using Xunit;
using Piranha.Models;
using Piranha.Repositories;
using Piranha.Services;

namespace Piranha.Tests.Repositories
{
    [Collection("Integration tests")]
    public class MediasCached : Medias
    {
        protected override void Init() {
            cache = new Cache.SimpleCache();

            base.Init();
        }
    }

    [Collection("Integration tests")]
    public class Medias : BaseTests
    {
        private Guid image1Id;
        private Guid image2Id;
        private Guid image3Id;
        private Guid image4Id;
        private Guid folder1Id;
        protected ICache cache;

        protected override void Init() {
            using (var api = CreateApi()) {
                Piranha.App.Init(api);

                // Add media folders
                var folder1 = new MediaFolder
                {
                    Name = "Images"
                };
                api.Media.SaveFolder(folder1);
                folder1Id = folder1.Id;

                // Add media
                using (var stream = File.OpenRead("../../../Assets/HLD_Screenshot_01_mech_1080.png")) {
                    var image1 = new Models.StreamMediaContent
                    {
                        Filename = "HLD_Screenshot_01_mech_1080.png",
                        Data = stream
                    };
                    api.Media.Save(image1);

                    image1Id = image1.Id.Value;

                    // Add some additional meta data
                    var image = api.Media.GetById(image1Id);
                    image.Title = "Screenshot";
                    image.AltText = "This is a screenshot";
                    image.Description = "Screenshot from Hyper Light Drifter";
                    image.Properties["Game"] = "Hyper Light Drifter";

                    api.Media.SaveAsync(image).GetAwaiter().GetResult();
                }

                using (var stream = File.OpenRead("../../../Assets/HLD_Screenshot_01_rise_1080.png")) {
                    var image2 = new Models.StreamMediaContent
                    {
                        FolderId = folder1Id,
                        Filename = "HLD_Screenshot_01_rise_1080.png",
                        Data = stream
                    };
                    api.Media.Save(image2);

                    image2Id = image2.Id.Value;
                }

                using (var stream = File.OpenRead("../../../Assets/HLD_Screenshot_01_robot_1080.png")) {
                    var image3 = new Models.StreamMediaContent
                    {
                        Filename = "HLD_Screenshot_01_robot_1080.png",
                        Data = stream
                    };
                    api.Media.Save(image3);

                    image3Id = image3.Id.Value;
                }
            }
        }

        protected override void Cleanup() {
            using (var api = CreateApi()) {
                var media = api.Media.GetAll();

                foreach (var item in media) {
                    api.Media.Delete(item);
                }

                var folders = api.Media.GetAllFolders();

                foreach (var folder in folders) {
                    media = api.Media.GetAll(folder.Id);

                    foreach (var item in media) {
                        api.Media.Delete(item);
                    }
                    api.Media.DeleteFolder(folder);
                }
            }
        }

        [Fact]
        public void IsCached() {
            using (var api = CreateApi()) {
                Assert.Equal(this.GetType() == typeof(MediasCached), ((Api)api).IsCached);
            }
        }

        [Fact]
        public void GetAll() {
            using (var api = CreateApi()) {
                var media = api.Media.GetAll();

                Assert.NotEmpty(media);
            }
        }

        [Fact]
        public void GetById() {
            using (var api = CreateApi()) {
                var media = api.Media.GetById(image1Id);

                Assert.NotNull(media);
                Assert.Equal("HLD_Screenshot_01_mech_1080.png", media.Filename);
                Assert.Equal("image/png", media.ContentType);
                Assert.Equal(Models.MediaType.Image, media.Type);
            }
        }

        [Fact]
        public void GetByFolderId() {
            using (var api = CreateApi()) {
                var media = api.Media.GetAll(folder1Id).ToList();

                Assert.NotEmpty(media);
                Assert.Equal("HLD_Screenshot_01_rise_1080.png", media[0].Filename);
            }
        }

        [Fact]
        public void TitleNotNull()
        {
            using (var api = CreateApi()) {
                var media = api.Media.GetById(image1Id);

                Assert.NotNull(media.Title);
                Assert.Equal("Screenshot", media.Title);
            }
        }

        [Fact]
        public void AltTextNotNull()
        {
            using (var api = CreateApi()) {
                var media = api.Media.GetById(image1Id);

                Assert.NotNull(media.AltText);
                Assert.Equal("This is a screenshot", media.AltText);
            }
        }

        [Fact]
        public void DescriptionNotNull()
        {
            using (var api = CreateApi()) {
                var media = api.Media.GetById(image1Id);

                Assert.NotNull(media.Description);
                Assert.Equal("Screenshot from Hyper Light Drifter", media.Description);
            }
        }

        [Fact]
        public void HasProperty()
        {
            using (var api = CreateApi()) {
                var media = api.Media.GetById(image1Id);

                Assert.Equal("Hyper Light Drifter", media.Properties["Game"]);
            }
        }

        [Fact]
        public void Move() {
            using (var api = CreateApi()) {
                var media = api.Media.GetById(image1Id);
                Assert.NotNull(media);
                Assert.Null(media.FolderId);
                api.Media.Move(media, folder1Id);

                media = api.Media.GetById(image1Id);
                Assert.NotNull(media.FolderId);
                Assert.Equal(folder1Id, media.FolderId.Value);

                api.Media.Move(media, null);
            }
        }

        [Fact]
        public void Insert() {
            using (var api = CreateApi()) {
                using (var stream = File.OpenRead("../../../Assets/HLD_Screenshot_BETA_entrance.png")) {
                    var image = new Models.StreamMediaContent
                    {
                        Filename = "HLD_Screenshot_BETA_entrance.png",
                        Data = stream
                    };
                    api.Media.Save(image);

                    Assert.NotNull(image.Id);

                    image4Id = image.Id.Value;
                }
            }
        }

        [Fact]
        public void PublicUrl() {
            using (var api = CreateApi()) {
                using (var config = new Piranha.Config(api)) {
                    config.MediaCDN = null;
                }

                var media = api.Media.GetById(image1Id);

                Assert.NotNull(media);
                Assert.Equal($"~/uploads/{image1Id}-HLD_Screenshot_01_mech_1080.png", media.PublicUrl);
            }
        }

        [Fact]
        public void PublicUrlCDN() {
            using (var api = CreateApi()) {
                using (var config = new Piranha.Config(api)) {
                    config.MediaCDN = "https://mycdn.org/uploads";
                }

                var media = api.Media.GetById(image1Id);

                Assert.NotNull(media);
                Assert.Equal($"https://mycdn.org/uploads/{image1Id}-HLD_Screenshot_01_mech_1080.png", media.PublicUrl);
            }
        }

        [Fact]
        public void Delete() {
            using (var api = CreateApi()) {
                var media = api.Media.GetById(image3Id);

                api.Media.Delete(media);
            }
        }

        [Fact]
        public void DeleteById() {
            using (var api = CreateApi()) {
                api.Media.Delete(image4Id);
            }
        }

        private IApi CreateApi()
        {
            var factory = new LegacyContentFactory(services);
            var serviceFactory = new ContentServiceFactory(factory);

            var db = GetDb();

            return new Api(
                factory,
                new AliasRepository(db),
                new ArchiveRepository(db),
                new ContentTypeRepository(db),
                new Piranha.Repositories.MediaRepository(db),
                new PageRepository(db, serviceFactory),
                new PageTypeRepository(db),
                new ParamRepository(db),
                new PostRepository(db, serviceFactory),
                new PostTypeRepository(db),
                new SiteRepository(db, serviceFactory),
                new SiteTypeRepository(db),
                cache: cache,
                storage: storage
            );
        }
    }
}