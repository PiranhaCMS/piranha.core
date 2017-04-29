/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using System.IO;
using System.Linq;
using Xunit;

namespace Piranha.Tests.Repositories
{
    [Collection("Integration tests")]
    public class Medias : BaseTests
    {
        private string image1Id;
        private string image2Id;
        private string image3Id;
        private string image4Id;
        private string folder1Id;

        protected override void Init() {
            using (var api = new Api(options, storage)) {
                Piranha.App.Init(api);

                // Add media folders
                var folder1 = new Data.MediaFolder() {
                    Name = "Images"
                };
                api.Media.SaveFolder(folder1);
                folder1Id = folder1.Id;

                // Add media
                using (var stream = File.OpenRead("../../../Assets/HLD_Screenshot_01_mech_1080.png")) {
                    var image1 = new Models.StreamMediaContent() {
                        Filename = "HLD_Screenshot_01_mech_1080.png",
                        Data = stream
                    };
                    api.Media.Save(image1);

                    image1Id = image1.Id;
                }

                using (var stream = File.OpenRead("../../../Assets/HLD_Screenshot_01_rise_1080.png")) {
                    var image2 = new Models.StreamMediaContent() {
                        FolderId = folder1Id,
                        Filename = "HLD_Screenshot_01_rise_1080.png",
                        Data = stream
                    };
                    api.Media.Save(image2);

                    image2Id = image2.Id;
                }                

                using (var stream = File.OpenRead("../../../Assets/HLD_Screenshot_01_robot_1080.png")) {
                    var image3 = new Models.StreamMediaContent() {
                        Filename = "HLD_Screenshot_01_robot_1080.png",
                        Data = stream
                    };
                    api.Media.Save(image3);

                    image3Id = image3.Id;
                }                
            }
        }

        protected override void Cleanup() {
            using (var api = new Api(options, storage)) {
                var media = api.Media.GetAll();

                foreach (var item in media) {
                    api.Media.Delete(item);
                }

                var folders = api.Media.GetAllFolders();
                foreach (var folder in folders) {
                    api.Media.DeleteFolder(folder);
                }
            }
        }

        [Fact]
        public void GetAll() {
            using (var api = new Api(options, storage)) {
                var media = api.Media.GetAll();

                Assert.NotEqual(0, media.Count());
            }
        }

        [Fact]
        public void GetById() {
            using (var api = new Api(options, storage)) {
                var media = api.Media.GetById(image1Id);

                Assert.NotNull(media);
                Assert.Equal("HLD_Screenshot_01_mech_1080.png", media.Filename);
                Assert.Equal("image/png", media.ContentType);
                Assert.Equal(Data.MediaType.Image, media.Type);
            }
        }

        [Fact]
        public void GetByFolderId() {
            using (var api = new Api(options, storage)) {
                var media = api.Media.GetAll(folder1Id).ToList();

                Assert.Equal(1, media.Count());
                Assert.Equal("HLD_Screenshot_01_rise_1080.png", media[0].Filename);
            }
        }

        [Fact]
        public void Insert() {
            using (var api = new Api(options, storage)) {
                using (var stream = File.OpenRead("../../../Assets/HLD_Screenshot_BETA_entrance.png")) {
                    var image = new Models.StreamMediaContent() {
                        Filename = "HLD_Screenshot_BETA_entrance.png",
                        Data = stream
                    };
                    api.Media.Save(image);

                    Assert.NotNull(image.Id);

                    image4Id = image.Id;
                }
            }            
        }

        [Fact]
        public void Delete() {
            using (var api = new Api(options, storage)) {
                var media = api.Media.GetById(image3Id);

                api.Media.Delete(media);
            }
        }

        [Fact]
        public void DeleteById() {
            using (var api = new Api(options, storage)) {
                var media = api.Media.GetById(image4Id);

                api.Media.Delete(image4Id);
            }
        }
    }
}