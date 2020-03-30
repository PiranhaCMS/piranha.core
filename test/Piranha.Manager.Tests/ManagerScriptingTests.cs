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
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Piranha.Manager.Tests
{
    public class ManagerScriptingTests
    {
        private readonly ITestOutputHelper _helper;

        /// <summary>
        /// Test to both demonstrate how the class works and what each of its various forms translate to.
        /// </summary>
        [Fact]
        public void CustomScriptListTest()
        {
            var scriptDefs = new List<ManagerScriptDefinition>();
            scriptDefs.AddRange(new[]
            {
                "https://unpkg.com/jquery",
                ("https://unpkg.com/react@16.7.0/umd/react.production.min.js","sha384-bDWFfmoLfqL0ZuPgUiUz3ekiv8NyiuJrrk1wGblri8Nut8UVD6mj7vXhjnenE9vy"),
                new ManagerScriptDefinition("This doesn't check for valid Urls atm, just nulls/whitespace only.", type:"text/babel"),
                new ManagerScriptDefinition("AGH", "HASHBROWNS",ECrossOriginPolicy.None),
                "/lib/js/dist/somescript.min.js"
            });

            var outputs = new[]
            {
                "<script type=\"text/javascript\" src=\"https://unpkg.com/jquery\"></script>",
                "<script type=\"text/javascript\" src=\"https://unpkg.com/react@16.7.0/umd/react.production.min.js\" integrity=\"sha384-bDWFfmoLfqL0ZuPgUiUz3ekiv8NyiuJrrk1wGblri8Nut8UVD6mj7vXhjnenE9vy\" crossorigin=\"anonymous\"></script>",
                "<script type=\"text/babel\" src=\"This doesn't check for valid Urls atm, just nulls/whitespace only.\"></script>",
                "<script type=\"text/javascript\" src=\"AGH\" integrity=\"HASHBROWNS\" ></script>",
                "<script type=\"text/javascript\" src=\"/lib/js/dist/somescript.min.js\"></script>"
            };
            foreach (var scriptDef in scriptDefs)
            {
                _helper.WriteLine(scriptDef.ToString());
            }
            Assert.Equal(scriptDefs.Select(c => c.ToString()), outputs);

            Assert.Throws<ArgumentNullException>(() => new ManagerScriptDefinition(null));
            Assert.Throws<ArgumentException>(() => new ManagerScriptDefinition(""));
        }

        public ManagerScriptingTests(ITestOutputHelper helper)
        {
            _helper = helper;
        }
    }
}
