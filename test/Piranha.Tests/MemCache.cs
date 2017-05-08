/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using System;
using Xunit;

namespace Piranha.Tests
{
    public class MemCache
    {
        private Piranha.Cache.MemCache cache;
        private string id1 = Guid.NewGuid().ToString();
        private string id2 = Guid.NewGuid().ToString();
        private string id3 = Guid.NewGuid().ToString();
        private string val1 = "My first value";
        private string val2 = "My second value";
        private string val3 = "My third value";
        private string val4 = "My fourth value";

        /// <summary>
        /// Initializes the test class.
        /// </summary>
        public MemCache() {
            cache = new Piranha.Cache.MemCache();

            cache.Set(id1, val1);
            cache.Set(id2, val2);
        }

        [Fact]
        public void AddEntry() {
            cache.Set(id3, val3);
        }

        [Fact]
        public void GetEntry() {
            var val = cache.Get<string>(id2);

            Assert.NotNull(val);
            Assert.Equal(val2, val);
        }

        [Fact]
        public void UpdateEntry() {
            cache.Set(id2, val4);

            var val = cache.Get<string>(id2);

            Assert.NotNull(val);
            Assert.Equal(val4, val);
        }

        [Fact]
        public void RemoveEntry() {
            cache.Remove(id1);

            var val = cache.Get<string>(id1);

            Assert.Null(val);
        }
    }
}