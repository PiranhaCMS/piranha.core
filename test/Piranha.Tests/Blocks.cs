/*
 * Copyright (c) 2018-2019 Håkan Edling
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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Piranha.Data;
using Piranha.Data.EF.SQLite;
using Piranha.Repositories;
using Piranha.Services;

namespace Piranha.Tests
{
    public class Blocks : BaseTests
    {
        private Guid image1Id;
        private IContentService<Page, PageField, Models.PageBase> contentService;

        /// <summary>
        /// Sets up and initializes the tests.
        /// </summary>
        protected override void Init() {
            services = new ServiceCollection()
                .AddDbContext<SQLiteDb>(options =>
                    options.UseSqlite("Filename=./piranha.tests.db"))
                .AddPiranhaEF<SQLiteDb>()
                .AddSingleton<IStorage, Local.FileStorage>()
                .BuildServiceProvider();

            using (var api = CreateApi()) {
                Piranha.App.Init(api);

                contentService = new ContentService<Page, PageField, Models.PageBase>(new LegacyContentFactory(services), Piranha.Data.EF.Module.Mapper);

                // Add media
                using (var stream = File.OpenRead("../../../Assets/HLD_Screenshot_01_mech_1080.png")) {
                    var image1 = new Models.StreamMediaContent
                    {
                        Filename = "HLD_Screenshot_01_mech_1080.png",
                        Data = stream
                    };
                    api.Media.Save(image1);

                    image1Id = image1.Id.Value;
                }
            }
        }

        /// <summary>
        /// Cleans up any possible data and resources
        /// created by the test.
        /// </summary>
        protected override void Cleanup() {
            using (var api = CreateApi()) {
                var media = api.Media.GetAll();

                foreach (var item in media) {
                    api.Media.Delete(item);
                }
            }
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
            //Assert.NotNull(((Extend.Blocks.ImageBlock)models[0]).Body.Media);
            //Assert.Equal("HLD_Screenshot_01_mech_1080.png", ((Extend.Blocks.ImageBlock)models[0]).Body.Media.Filename);
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
                storage: storage
            );
        }
    }
}
