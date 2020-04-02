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
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Piranha.Models;

namespace Piranha.Tests.Services
{
    [Collection("Integration tests")]
    public class AliasTestsCached : AliasTests
    {
        public override Task InitializeAsync()
        {
            _cache = new Cache.SimpleCache();

            return base.InitializeAsync();
        }
    }

    [Collection("Integration tests")]
    public class AliasTests : BaseTestsAsync
    {
        private const string ALIAS_1 = "/old-url";
        private const string ALIAS_2 = "/another-old-url";
        private const string ALIAS_3 = "/moved/page";
        private const string ALIAS_4 = "/another-moved-page";
        private const string ALIAS_5 = "/the-last-moved-page";

        private readonly Guid SITE_ID = Guid.NewGuid();
        private readonly Guid ALIAS_1_ID = Guid.NewGuid();

        public override async Task InitializeAsync()
        {
            using (var api = CreateApi())
            {
                // Add site
                var site = new Site
                {
                    Id = SITE_ID,
                    Title = "Alias Site",
                    InternalId = "AliasSite",
                    IsDefault = true
                };
                await api.Sites.SaveAsync(site);

                // Add aliases
                await api.Aliases.SaveAsync(new Alias
                {
                    Id = ALIAS_1_ID,
                    SiteId = SITE_ID,
                    AliasUrl = ALIAS_1,
                    RedirectUrl = "/redirect-1"
                });
                await api.Aliases.SaveAsync(new Alias
                {
                    SiteId = SITE_ID,
                    AliasUrl = ALIAS_4,
                    RedirectUrl = "/redirect-4"
                });
                await api.Aliases.SaveAsync(new Alias
                {
                    SiteId = SITE_ID,
                    AliasUrl = ALIAS_5,
                    RedirectUrl = "/redirect-5"
                });
            }
        }

        public override async Task DisposeAsync()
        {
            using (var api = CreateApi())
            {
                var aliases = await api.Aliases.GetAllAsync();
                foreach (var a in aliases)
                {
                    await api.Aliases.DeleteAsync(a);
                }

                var sites = await api.Sites.GetAllAsync();
                foreach (var s in sites)
                {
                    await api.Sites.DeleteAsync(s);
                }
            }
        }

        [Fact]
        public void IsCached() {
            using (var api = CreateApi()) {
                Assert.Equal(this.GetType() == typeof(AliasTestsCached), ((Api)api).IsCached);
            }
        }

        [Fact]
        public async Task Add()
        {
            using (var api = CreateApi())
            {
                await api.Aliases.SaveAsync(new Alias
                {
                    SiteId = SITE_ID,
                    AliasUrl = ALIAS_2,
                    RedirectUrl = "/redirect-2"
                });
            }
        }

        [Fact]
        public async Task AddDuplicateKey()
        {
            using (var api = CreateApi())
            {
                await Assert.ThrowsAnyAsync<Exception>(async () =>
                    await api.Aliases.SaveAsync(new Alias
                    {
                        SiteId = SITE_ID,
                        AliasUrl = ALIAS_1,
                        RedirectUrl = "/duplicate-alias"
                    })
                );
            }
        }

        [Fact]
        public async Task GetNoneById()
        {
            using (var api = CreateApi())
            {
                var none = await api.Aliases.GetByIdAsync(Guid.NewGuid());

                Assert.Null(none);
            }
        }

        [Fact]
        public async Task GetNoneByAliasUrl()
        {
            using (var api = CreateApi())
            {
                var none = await api.Aliases.GetByAliasUrlAsync("/none-existing-alias");

                Assert.Null(none);
            }
        }

        [Fact]
        public async Task GetNoneByRedirectUrl()
        {
            using (var api = CreateApi())
            {
                var none = await api.Aliases.GetByRedirectUrlAsync("/none-existing-alias");

                Assert.Empty(none);
            }
        }

        [Fact]
        public async Task GetAll()
        {
            using (var api = CreateApi())
            {
                var models = await api.Aliases.GetAllAsync();

                Assert.NotNull(models);
                Assert.NotEmpty(models);
            }
        }

        [Fact]
        public async Task GetById()
        {
            using (var api = CreateApi())
            {
                var model = await api.Aliases.GetByIdAsync(ALIAS_1_ID);

                Assert.NotNull(model);
                Assert.Equal(ALIAS_1, model.AliasUrl);
            }
        }

        [Fact]
        public async Task GetByAliasUrl()
        {
            using (var api = CreateApi())
            {
                var model = await api.Aliases.GetByAliasUrlAsync(ALIAS_1);

                Assert.NotNull(model);
                Assert.Equal(ALIAS_1, model.AliasUrl);
            }
        }

        [Fact]
        public async Task GetByRedirectUrl()
        {
            using (var api = CreateApi())
            {
                var models = await api.Aliases.GetByRedirectUrlAsync("/redirect-1");

                Assert.Single(models);
                Assert.Equal(ALIAS_1, models.First().AliasUrl);
            }
        }

        [Fact]
        public async Task Update()
        {
            using (var api = CreateApi())
            {
                var model = await api.Aliases.GetByIdAsync(ALIAS_1_ID);

                Assert.Equal("/redirect-1", model.RedirectUrl);

                model.RedirectUrl = "/redirect-updated";

                await api.Aliases.SaveAsync(model);
            }
        }

        [Fact]
        public async Task FixAliasUrl()
        {
            using (var api = CreateApi())
            {
                var model = new Alias
                {
                    SiteId = SITE_ID,
                    AliasUrl = "the-alias-url-1",
                    RedirectUrl = "/the-redirect-1"
                };

                await api.Aliases.SaveAsync(model);

                Assert.Equal("/the-alias-url-1", model.AliasUrl);
            }
        }

        [Fact]
        public async Task FixRedirectUrl()
        {
            using (var api = CreateApi())
            {
                var model = new Alias
                {
                    SiteId = SITE_ID,
                    AliasUrl = "/the-alias-url-2",
                    RedirectUrl = "the-redirect-2"
                };

                await api.Aliases.SaveAsync(model);

                Assert.Equal("/the-redirect-2", model.RedirectUrl);
            }
        }

        [Fact]
        public async Task AllowHttpUrl()
        {
            using (var api = CreateApi())
            {
                var model = new Alias
                {
                    SiteId = SITE_ID,
                    AliasUrl = "/the-alias-url-3",
                    RedirectUrl = "http://redirect.com"
                };

                await api.Aliases.SaveAsync(model);

                Assert.Equal("http://redirect.com", model.RedirectUrl);
            }
        }

        [Fact]
        public async Task AllowHttpsUrl()
        {
            using (var api = CreateApi())
            {
                var model = new Alias
                {
                    SiteId = SITE_ID,
                    AliasUrl = "/the-alias-url-4",
                    RedirectUrl = "https://redirect.com"
                };

                await api.Aliases.SaveAsync(model);

                Assert.Equal("https://redirect.com", model.RedirectUrl);
            }
        }

        [Fact]
        public async Task Delete()
        {
            using (var api = CreateApi())
            {
                var model = await api.Aliases.GetByAliasUrlAsync(ALIAS_4);

                Assert.NotNull(model);

                await api.Aliases.DeleteAsync(model);
            }
        }

        [Fact]
        public async Task DeleteById()
        {
            using (var api = CreateApi())
            {
                var model = await api.Aliases.GetByAliasUrlAsync(ALIAS_5);

                Assert.NotNull(model);

                await api.Aliases.DeleteAsync(model.Id);
            }
        }
    }
}
