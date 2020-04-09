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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Piranha.Data;
using Piranha.Extend.Blocks;
using Piranha.Services;

namespace Piranha.Tests
{
    public class Blocks : BaseTestsAsync
    {
        private Guid image1Id;
        private IContentService<Page, PageField, Models.PageBase> contentService;

        /// <summary>
        /// Sets up and initializes the tests.
        /// </summary>
        public override async Task InitializeAsync()
        {
            using (var api = CreateApi())
            {
                Piranha.App.Init(api);

                contentService = new ContentService<Page, PageField, Models.PageBase>(new ContentFactory(_services), Piranha.Data.EF.Module.Mapper);

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
                }
            }
        }

        /// <summary>
        /// Cleans up any possible data and resources
        /// created by the test.
        /// </summary>
        public override async Task DisposeAsync()
        {
            using (var api = CreateApi())
            {
                var media = await api.Media.GetAllByFolderIdAsync();

                foreach (var item in media)
                {
                    await api.Media.DeleteAsync(item);
                }
            }
        }

        [Fact]
        public void AudioBlockNoTitle()
        {
            var block = new AudioBlock();
            var title = block.GetTitle();

            Assert.Equal("No audio selected", title);
        }

        [Fact]
        public async Task AudioBlockHasTitle()
        {
            using (var api = CreateApi())
            {
                var media = await api.Media.GetByIdAsync(image1Id);

                var block = new AudioBlock()
                {
                    Body = new Extend.Fields.AudioField
                    {
                        Id = media.Id
                    }
                };
                await block.Body.Init(api);

                var title = block.GetTitle();

                Assert.Equal("HLD_Screenshot_01_mech_1080.png", title);
            }
        }

        [Fact]
        public void ImageBlockNoTitle()
        {
            var block = new ImageBlock();
            var title = block.GetTitle();

            Assert.Equal("No image selected", title);
        }

        [Fact]
        public async Task ImageBlockHasTitle()
        {
            using (var api = CreateApi())
            {
                var media = await api.Media.GetByIdAsync(image1Id);

                var block = new ImageBlock()
                {
                    Body = new Extend.Fields.ImageField
                    {
                        Id = media.Id
                    }
                };
                await block.Body.Init(api);

                var title = block.GetTitle();

                Assert.Equal("HLD_Screenshot_01_mech_1080.png", title);
            }
        }

        [Fact]
        public void HtmlBlockNoTitle()
        {
            var block = new HtmlBlock();
            var title = block.GetTitle();

            Assert.Equal("Empty", title);
        }

        [Fact]
        public void HtmlBlockHasTitle()
        {
            var block = new HtmlBlock()
            {
                Body = new Extend.Fields.HtmlField
                {
                    Value = "<p>Lorem ipsum</p>"
                }
            };
            var title = block.GetTitle();

            Assert.Equal("Lorem ipsum", title);
        }

        [Fact]
        public void HtmlBlockHasLongTitle()
        {
            var block = new HtmlBlock()
            {
                Body = new Extend.Fields.HtmlField
                {
                    Value = "<p>Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Integer posuere erat a ante venenatis dapibus posuere velit aliquet. Maecenas faucibus mollis interdum. Donec id elit non mi porta gravida at eget metus. Nulla vitae elit libero, a pharetra augue. Sed posuere consectetur est at lobortis. Etiam porta sem malesuada magna mollis euismod.</p>"
                }
            };
            var title = block.GetTitle();

            Assert.Equal(43, title.Length);
            Assert.EndsWith("...", title);
        }

        [Fact]
        public void QuoteBlockNoTitle()
        {
            var block = new QuoteBlock();
            var title = block.GetTitle();

            Assert.Equal("Empty", title);
        }

        [Fact]
        public void QuoteBlockHasTitle()
        {
            var block = new QuoteBlock()
            {
                Body = new Extend.Fields.TextField
                {
                    Value = "To be or not to be"
                }
            };
            var title = block.GetTitle();

            Assert.Equal("To be or not to be", title);
        }

        [Fact]
        public void SeparatorBlockTitle()
        {
            var block = new SeparatorBlock();
            var title = block.GetTitle();

            Assert.Equal("----", title);
        }

        [Fact]
        public void TextBlockNoTitle()
        {
            var block = new TextBlock();
            var title = block.GetTitle();

            Assert.Equal("Empty", title);
        }

        [Fact]
        public void TextBlockHasTitle()
        {
            var block = new TextBlock()
            {
                Body = new Extend.Fields.TextField
                {
                    Value = "Lorem ipsum"
                }
            };
            var title = block.GetTitle();

            Assert.Equal("Lorem ipsum", title);
        }

        [Fact]
        public void DeserializeHtmlBlock() {
            var blocks = new List<Block>();
            blocks.Add(new Block
            {
                CLRType = typeof(Extend.Blocks.HtmlBlock).FullName,
                Fields = new List<BlockField>
                {
                    new BlockField
                    {
                        CLRType = typeof(Extend.Fields.HtmlField).FullName,
                        FieldId = "Body",
                        Value = "<p>Lorem ipsum</p>"
                    }
                },
                Created = DateTime.Now,
                LastModified = DateTime.Now
            });

            var models = contentService.TransformBlocks(blocks);

            Assert.NotNull(models);
            Assert.Single(models);

            Assert.Equal(typeof(Extend.Blocks.HtmlBlock), models.First().GetType());
            Assert.Equal("<p>Lorem ipsum</p>", ((Extend.Blocks.HtmlBlock)models[0]).Body.Value);
        }

        [Fact]
        public void SerializeHtmlBlock() {
            var models = new List<Extend.Block>();
            models.Add(new Extend.Blocks.HtmlBlock
            {
                Body = new Extend.Fields.HtmlField
                {
                    Value = "<p>Lorem ipsum</p>"
                }
            });

            var blocks = contentService.TransformBlocks(models);

            Assert.NotNull(blocks);
            Assert.Single(blocks);

            Assert.Equal(typeof(Extend.Blocks.HtmlBlock).FullName, blocks[0].CLRType);
            Assert.Equal(typeof(Extend.Fields.HtmlField).FullName, blocks[0].Fields[0].CLRType);
            Assert.Equal("<p>Lorem ipsum</p>", blocks[0].Fields[0].Value);
        }

        /*
        [Fact]
        public void DeserializeHtmlColumnBlock() {
            var blocks = new List<Block>();
            blocks.Add(new Block() {
                CLRType = typeof(Extend.Blocks.HtmlColumnBlock).FullName,
                Fields = new List<BlockField>() {
                    new BlockField() {
                        CLRType = typeof(Extend.Fields.HtmlField).FullName,
                        FieldId = "Column1",
                        Value = "<p>Column 1</p>"
                    },
                    new BlockField() {
                        CLRType = typeof(Extend.Fields.HtmlField).FullName,
                        FieldId = "Column2",
                        Value = "<p>Column 2</p>"
                    }
                },
                Created = DateTime.Now,
                LastModified = DateTime.Now
            });

            var models = contentService.TransformBlocks(blocks);

            Assert.NotNull(models);
            Assert.Single(models);

            Assert.Equal(typeof(Extend.Blocks.HtmlColumnBlock), models.First().GetType());
            Assert.Equal("<p>Column 1</p>", ((Extend.Blocks.HtmlColumnBlock)models[0]).Column1.Value);
            Assert.Equal("<p>Column 2</p>", ((Extend.Blocks.HtmlColumnBlock)models[0]).Column2.Value);
        }

        [Fact]
        public void SerializeHtmlColumnBlock() {
            var models = new List<Extend.Block>();
            models.Add(new Extend.Blocks.HtmlColumnBlock() {
                Column1 = new Extend.Fields.HtmlField() {
                    Value = "<p>Column 1</p>"
                },
                Column2 = new Extend.Fields.HtmlField() {
                    Value = "<p>Column 2</p>"
                },
            });

            var blocks = contentService.TransformBlocks(models);

            Assert.NotNull(blocks);
            Assert.Single(blocks);
            Assert.Equal(2, blocks[0].Fields.Count);

            Assert.Equal(typeof(Extend.Blocks.HtmlColumnBlock).FullName, blocks[0].CLRType);
            Assert.Equal(typeof(Extend.Fields.HtmlField).FullName, blocks[0].Fields[0].CLRType);
            Assert.Equal(typeof(Extend.Fields.HtmlField).FullName, blocks[0].Fields[1].CLRType);
            Assert.Equal("<p>Column 1</p>", blocks[0].Fields[0].Value);
            Assert.Equal("<p>Column 2</p>", blocks[0].Fields[1].Value);
        }
        */

        [Fact]
        public void DeserializeImageBlock() {
            var blocks = new List<Block>();
            blocks.Add(new Block
            {
                CLRType = typeof(Extend.Blocks.ImageBlock).FullName,
                Fields = new List<BlockField>
                {
                    new BlockField
                    {
                        CLRType = typeof(Extend.Fields.ImageField).FullName,
                        FieldId = "Body",
                        Value = image1Id.ToString()
                    }
                },
                Created = DateTime.Now,
                LastModified = DateTime.Now
            });

            var models = contentService.TransformBlocks(blocks);

            Assert.NotNull(models);
            Assert.Single(models);

            Assert.Equal(typeof(Extend.Blocks.ImageBlock), models.First().GetType());
            Assert.Null(((Extend.Blocks.ImageBlock)models[0]).Body.Media);
        }

        [Fact]
        public void SerializeImageBlock() {
            var models = new List<Extend.Block>();
            models.Add(new Extend.Blocks.ImageBlock
            {
                Body = new Extend.Fields.ImageField
                {
                    Id = image1Id
                }
            });

            var blocks = contentService.TransformBlocks(models);

            Assert.NotNull(blocks);
            Assert.Single(blocks);

            Assert.Equal(typeof(Extend.Blocks.ImageBlock).FullName, blocks[0].CLRType);
            Assert.Equal(typeof(Extend.Fields.ImageField).FullName, blocks[0].Fields[0].CLRType);
            Assert.Equal(image1Id.ToString(), blocks[0].Fields[0].Value);
        }

        [Fact]
        public void DeserializeTextBlock() {
            var blocks = new List<Block>();
            blocks.Add(new Block
            {
                CLRType = typeof(Extend.Blocks.TextBlock).FullName,
                Fields = new List<BlockField>
                {
                    new BlockField
                    {
                        CLRType = typeof(Extend.Fields.TextField).FullName,
                        FieldId = "Body",
                        Value = "Lorem ipsum"
                    }
                },
                Created = DateTime.Now,
                LastModified = DateTime.Now
            });

            var models = contentService.TransformBlocks(blocks);

            Assert.NotNull(models);
            Assert.Single(models);

            Assert.Equal(typeof(Extend.Blocks.TextBlock), models.First().GetType());
            Assert.Equal("Lorem ipsum", ((Extend.Blocks.TextBlock)models[0]).Body.Value);
        }

        [Fact]
        public void SerializeTextBlock() {
            var models = new List<Extend.Block>();
            models.Add(new Extend.Blocks.TextBlock
            {
                Body = new Extend.Fields.TextField
                {
                    Value = "Lorem ipsum"
                }
            });

            var blocks = contentService.TransformBlocks(models);

            Assert.NotNull(blocks);
            Assert.Single(blocks);

            Assert.Equal(typeof(Extend.Blocks.TextBlock).FullName, blocks[0].CLRType);
            Assert.Equal(typeof(Extend.Fields.TextField).FullName, blocks[0].Fields[0].CLRType);
            Assert.Equal("Lorem ipsum", blocks[0].Fields[0].Value);
        }

        [Fact]
        public void DeserializeQuoteBlock() {
            var blocks = new List<Block>();
            blocks.Add(new Block
            {
                CLRType = typeof(Extend.Blocks.QuoteBlock).FullName,
                Fields = new List<BlockField>
                {
                    new BlockField
                    {
                        CLRType = typeof(Extend.Fields.TextField).FullName,
                        FieldId = "Body",
                        Value = "Lorem ipsum"
                    }
                },
                Created = DateTime.Now,
                LastModified = DateTime.Now
            });

            var models = contentService.TransformBlocks(blocks);

            Assert.NotNull(models);
            Assert.Single(models);

            Assert.Equal(typeof(Extend.Blocks.QuoteBlock), models.First().GetType());
            Assert.Equal("Lorem ipsum", ((Extend.Blocks.QuoteBlock)models[0]).Body.Value);
        }

        [Fact]
        public void SerializeQuoteBlock() {
            var models = new List<Extend.Block>();
            models.Add(new Extend.Blocks.QuoteBlock
            {
                Body = new Extend.Fields.TextField
                {
                    Value = "Lorem ipsum"
                }
            });

            var blocks = contentService.TransformBlocks(models);

            Assert.NotNull(blocks);
            Assert.Single(blocks);

            Assert.Equal(typeof(Extend.Blocks.QuoteBlock).FullName, blocks[0].CLRType);
            Assert.Equal(typeof(Extend.Fields.TextField).FullName, blocks[0].Fields[0].CLRType);
            Assert.Equal("Lorem ipsum", blocks[0].Fields[0].Value);
        }
    }
}
