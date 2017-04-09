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
        protected override void Init() {
            using (var api = new Api(options, storage)) {
                using (var stream = File.OpenRead("../../../Assets/HLD_Screenshot_01_mech_1080.png")) {
                    var image1 = new Models.StreamMediaContent() {
                        Filename = "HLD_Screenshot_01_mech_1080.png",
                        ContentType = "image/png",
                        Data = stream
                    };
                    api.Media.Save(image1);
                }
            }
        }

        protected override void Cleanup() {
            using (var api = new Api(options, storage)) {
                var media = api.Media.GetAll();

                foreach (var item in media) {
                    api.Media.Delete(item);
                }
            }
        }

        [Fact]
        public void GetAll() {
            using (var api = new Api(options, storage)) {
                var media = api.Media.GetAll();

                Assert.Equal(1, media.Count());
            }
        }
    }
}