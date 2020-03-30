/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System.Globalization;
using Xunit;

namespace Piranha.Tests.Utils
{
    public class FormatBytes
    {
        private readonly string separator;

        public FormatBytes() {
            separator = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
        }

        [Fact]
        public void FormatToBytes() {
            var res = Piranha.Utils.FormatByteSize(800);

            Assert.Equal($"800{separator}00 bytes", res);
        }

        [Fact]
        public void FormatToKB() {
            var res = Piranha.Utils.FormatByteSize(45 * 1024);

            Assert.Equal($"45{separator}00 KB", res);
        }

        [Fact]
        public void FormatToMB() {
            var res = Piranha.Utils.FormatByteSize(12 * 1024 * 1024);

            Assert.Equal($"12{separator}00 MB", res);
        }

        [Fact]
        public void FormatToGB() {
            var res = Piranha.Utils.FormatByteSize(1.2 * 1024 * 1024 * 1024);

            Assert.Equal($"1{separator}20 GB", res);
        }
    }
}
