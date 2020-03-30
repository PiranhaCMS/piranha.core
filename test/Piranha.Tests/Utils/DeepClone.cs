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
using Piranha.Models;
using Xunit;

namespace Piranha.Tests.Utils
{
    public class DeepClone
    {
        [Fact]
        public void WithBoolean()
        {
            bool parameter = true;

            bool result = Piranha.Utils.DeepClone(parameter);
            parameter = false;

            // Result should be true and changing 'parameter' should not affect 'result'.
            Assert.True(result);
            Assert.NotEqual(parameter, result);
        }

        [Fact]
        public void WithAliasNull()
        {
            Alias alias = null;

            Alias result = Piranha.Utils.DeepClone(alias);
            alias = new Alias();

            // Result should be null and not referentially equal to 'parameter'.
            Assert.Null(result);
            Assert.NotEqual(alias, result);
        }

        [Fact]
        public void WithAliasObject()
        {
            Alias alias = new Alias();

            Alias result = Piranha.Utils.DeepClone<Alias>(alias);
            alias.Id = Guid.NewGuid();

            // Result should not be null true and not referentially equal to 'parameter'.
            Assert.NotNull(result);
            Assert.NotEqual(alias, result);
        }
    }
}
