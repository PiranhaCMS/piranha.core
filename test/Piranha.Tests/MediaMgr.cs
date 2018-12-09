/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using Piranha.Extend;
using Piranha.Runtime;
using Piranha.Services;
using System;
using System.Linq;
using System.Text;
using Xunit;

namespace Piranha.Tests
{
    public class MediaMgr : BaseTests
    {
        private MediaManager mgr = new MediaManager();

        /// <summary>
        /// Sets up & initializes the tests.
        /// </summary>
        protected override void Init() {
            mgr.Documents.Add(".pdf", "application/pdf");
            mgr.Images.Add(".jpg", "image/jpeg");
            mgr.Images.Add(".jpeg", "image/jpeg");
            mgr.Images.Add(".png", "image/png");
            mgr.Videos.Add(".mp4", "video/mp4");
        }

        /// <summary>
        /// Cleans up any possible data and resources
        /// created by the test.
        /// </summary>
        protected override void Cleanup() { }

        [Fact]
        public void GetDocumentMediaType()
        {
            var type = mgr.GetMediaType("myfile.pdf");
            Assert.Equal(Models.MediaType.Document, type);
        }

        [Fact]
        public void GetDocumentContentType()
        {
            var type = mgr.GetContentType("myfile.pdf");
            Assert.Equal("application/pdf", type);
        }

        [Fact]
        public void GetImageMediaType()
        {
            var type = mgr.GetMediaType("myimage.jpg");
            Assert.Equal(Models.MediaType.Image, type);
        }

        [Fact]
        public void GetVideoMediaType()
        {
            var type = mgr.GetMediaType("myvideo.mp4");
            Assert.Equal(Models.MediaType.Video, type);
        }
    }
}
