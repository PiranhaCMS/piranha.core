/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System.IO;
using Xunit;
using Piranha.ImageSharp;

namespace Piranha.Tests.ImageSharp
{
    public class ProcessorTests
    {
        [Fact]
        public void GetSizeStream() {
            using (var file = File.OpenRead("../../../Assets/HLD_Screenshot_01_mech_1080.png")) {
                var processor = new ImageSharpProcessor();

                processor.GetSize(file, out var width, out var height);

                Assert.Equal(1920, width);
                Assert.Equal(1080, height);
            }
        }

        [Fact]
        public void GetSizeBytes() {
            using (var file = File.OpenRead("../../../Assets/HLD_Screenshot_01_mech_1080.png")) {
                using (var reader = new BinaryReader(file)) {
                    var bytes = reader.ReadBytes((int) file.Length);

                    var processor = new ImageSharpProcessor();

                    processor.GetSize(bytes, out var width, out var height);

                    Assert.Equal(1920, width);
                    Assert.Equal(1080, height);
                }
            }
        }

        [Fact]
        public void Crop() {
            using (var file = File.OpenRead("../../../Assets/HLD_Screenshot_01_mech_1080.png")) {
                var processor = new ImageSharpProcessor();

                using (var outStream = new MemoryStream()) {
                    processor.Crop(file, outStream, 640, 480);

                    outStream.Position = 0;

                    processor.GetSize(outStream, out var width, out var height);

                    Assert.Equal(640, width);
                    Assert.Equal(480, height);
                }
            }
        }

        [Fact]
        public void Scale() {
            using (var file = File.OpenRead("../../../Assets/HLD_Screenshot_01_mech_1080.png")) {
                var processor = new ImageSharpProcessor();

                using (var outStream = new MemoryStream()) {
                    processor.Scale(file, outStream, 960);

                    outStream.Position = 0;

                    processor.GetSize(outStream, out var width, out var height);

                    Assert.Equal(960, width);
                    Assert.Equal(540, height);
                }
            }
        }

        [Fact]
        public void CropScale() {
            using (var file = File.OpenRead("../../../Assets/HLD_Screenshot_01_mech_1080.png")) {
                var processor = new ImageSharpProcessor();

                using (var outStream = new MemoryStream()) {
                    processor.CropScale(file, outStream, 640, 480);

                    outStream.Position = 0;

                    processor.GetSize(outStream, out var width, out var height);

                    Assert.Equal(640, width);
                    Assert.Equal(480, height);
                }
            }
        }
    }
}
