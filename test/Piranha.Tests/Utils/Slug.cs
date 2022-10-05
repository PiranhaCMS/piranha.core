/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Xunit;

namespace Piranha.Tests.Utils;

public class Slug
{
    [Fact]
    public void ToLowerCase() {
        Assert.Equal("mycamelcasestring", Piranha.Utils.GenerateSlug("MyCamelCaseString"));
    }

    [Fact]
    public void Trim() {
        Assert.Equal("trimmed", Piranha.Utils.GenerateSlug(" trimmed  "));
    }

    [Fact]
    public void ReplaceWhitespace() {
        Assert.Equal("no-whitespaces", Piranha.Utils.GenerateSlug("no whitespaces"));
    }

    [Fact]
    public void ReplaceDoubleDashes() {
        Assert.Equal("no-whitespaces", Piranha.Utils.GenerateSlug("no - whitespaces"));
        Assert.Equal("no-and-whitespaces", Piranha.Utils.GenerateSlug("no & whitespaces"));
    }

    [Fact]
    public void TrimDashes() {
        Assert.Equal("trimmed", Piranha.Utils.GenerateSlug("-trimmed-"));
    }

    [Fact]
    public void ReplaceLatinCharacters() {
        var charMap = new Dictionary<string, string>
        {
            ["À"] = "A",
            ["Á"] = "A",
            ["Â"] = "A",
            ["Ã"] = "A",
            ["Ä"] = "A",
            ["Å"] = "A",
            ["Æ"] = "AE",
            ["Ç"] = "C",
            ["È"] = "E",
            ["É"] = "E",
            ["Ê"] = "E",
            ["Ë"] = "E",
            ["Ì"] = "I",
            ["Í"] = "I",
            ["Î"] = "I",
            ["Ï"] = "I",
            ["Ð"] = "D",
            ["Ñ"] = "N",
            ["Ò"] = "O",
            ["Ó"] = "O",
            ["Ô"] = "O",
            ["Õ"] = "O",
            ["Ö"] = "O",
            ["Ő"] = "O",
            ["Ø"] = "O",
            ["Ù"] = "U",
            ["Ú"] = "U",
            ["Û"] = "U",
            ["Ü"] = "U",
            ["Ű"] = "U",
            ["Ý"] = "Y",
            ["Þ"] = "TH",
            ["ß"] = "ss",
            ["à"] = "a",
            ["á"] = "a",
            ["â"] = "a",
            ["ã"] = "a",
            ["ä"] = "a",
            ["å"] = "a",
            ["æ"] = "ae",
            ["ç"] = "c",
            ["è"] = "e",
            ["é"] = "e",
            ["ê"] = "e",
            ["ë"] = "e",
            ["ì"] = "i",
            ["í"] = "i",
            ["î"] = "i",
            ["ï"] = "i",
            ["ð"] = "d",
            ["ñ"] = "n",
            ["ò"] = "o",
            ["ó"] = "o",
            ["ô"] = "o",
            ["õ"] = "o",
            ["ö"] = "o",
            ["ő"] = "o",
            ["ø"] = "o",
            ["ù"] = "u",
            ["ú"] = "u",
            ["û"] = "u",
            ["ü"] = "u",
            ["ű"] = "u",
            ["ý"] = "y",
            ["þ"] = "th",
            ["ÿ"] = "y",
            ["ẞ"] = "SS"
        };

        foreach (var kvp in charMap)
        {
            Assert.Equal($"foo-{kvp.Value.ToLower()}-bar-baz",
                Piranha.Utils.GenerateSlug($"foo {kvp.Key} bar baz"));
        }
    }

    [Fact]
    public void ReplaceGreekCharacters() {
        var charMap = new Dictionary<string, string>
        {
            ["α"] = "a",
            ["β"] = "b",
            ["γ"] = "g",
            ["δ"] = "d",
            ["ε"] = "e",
            ["ζ"] = "z",
            ["η"] = "h",
            ["θ"] = "8",
            ["ι"] = "i",
            ["κ"] = "k",
            ["λ"] = "l",
            ["μ"] = "m",
            ["ν"] = "n",
            ["ξ"] = "3",
            ["ο"] = "o",
            ["π"] = "p",
            ["ρ"] = "r",
            ["σ"] = "s",
            ["τ"] = "t",
            ["υ"] = "y",
            ["φ"] = "f",
            ["χ"] = "x",
            ["ψ"] = "ps",
            ["ω"] = "w",
            ["ά"] = "a",
            ["έ"] = "e",
            ["ί"] = "i",
            ["ό"] = "o",
            ["ύ"] = "y",
            ["ή"] = "h",
            ["ώ"] = "w",
            ["ς"] = "s",
            ["ϊ"] = "i",
            ["ΰ"] = "y",
            ["ϋ"] = "y",
            ["ΐ"] = "i",
            ["Α"] = "A",
            ["Β"] = "B",
            ["Γ"] = "G",
            ["Δ"] = "D",
            ["Ε"] = "E",
            ["Ζ"] = "Z",
            ["Η"] = "H",
            ["Θ"] = "8",
            ["Ι"] = "I",
            ["Κ"] = "K",
            ["Λ"] = "L",
            ["Μ"] = "M",
            ["Ν"] = "N",
            ["Ξ"] = "3",
            ["Ο"] = "O",
            ["Π"] = "P",
            ["Ρ"] = "R",
            ["Σ"] = "S",
            ["Τ"] = "T",
            ["Υ"] = "Y",
            ["Φ"] = "F",
            ["Χ"] = "X",
            ["Ψ"] = "PS",
            ["Ω"] = "W",
            ["Ά"] = "A",
            ["Έ"] = "E",
            ["Ί"] = "I",
            ["Ό"] = "O",
            ["Ύ"] = "Y",
            ["Ή"] = "H",
            ["Ώ"] = "W",
            ["Ϊ"] = "I",
            ["Ϋ"] = "Y"
        };

        foreach (var kvp in charMap)
        {
            Assert.Equal($"foo-{kvp.Value.ToLower()}-bar-baz",
                Piranha.Utils.GenerateSlug($"foo {kvp.Key} bar baz"));
        }
    }

    [Fact]
    public void ReplaceTurkishCharacters() {
        var charMap = new Dictionary<string, string>
        {
            ["ş"] = "s",
            ["Ş"] = "S",
            ["ı"] = "i",
            ["İ"] = "I",
            ["ç"] = "c",
            ["Ç"] = "C",
            ["ü"] = "u",
            ["Ü"] = "U",
            ["ö"] = "o",
            ["Ö"] = "O",
            ["ğ"] = "g",
            ["Ğ"] = "G"
        };

        foreach (var kvp in charMap)
        {
            Assert.Equal($"foo-{kvp.Value.ToLower()}-bar-baz",
                Piranha.Utils.GenerateSlug($"foo {kvp.Key} bar baz"));
        }
    }

    [Fact]
    public void ReplaceCyrillicCharacters() {
        var charMap = new Dictionary<string, string>
        {
            ["а"] = "a",
            ["б"] = "b",
            ["в"] = "v",
            ["г"] = "g",
            ["д"] = "d",
            ["е"] = "e",
            ["ё"] = "yo",
            ["ж"] = "zh",
            ["з"] = "z",
            ["и"] = "i",
            ["й"] = "j",
            ["к"] = "k",
            ["л"] = "l",
            ["м"] = "m",
            ["н"] = "n",
            ["о"] = "o",
            ["п"] = "p",
            ["р"] = "r",
            ["с"] = "s",
            ["т"] = "t",
            ["у"] = "u",
            ["ф"] = "f",
            ["х"] = "h",
            ["ц"] = "c",
            ["ч"] = "ch",
            ["ш"] = "sh",
            ["щ"] = "sh",
            ["ъ"] = "u",
            ["ы"] = "y",
            ["ь"] = "",
            ["э"] = "e",
            ["ю"] = "yu",
            ["я"] = "ya",
            ["А"] = "A",
            ["Б"] = "B",
            ["В"] = "V",
            ["Г"] = "G",
            ["Д"] = "D",
            ["Е"] = "E",
            ["Ё"] = "Yo",
            ["Ж"] = "Zh",
            ["З"] = "Z",
            ["И"] = "I",
            ["Й"] = "J",
            ["К"] = "K",
            ["Л"] = "L",
            ["М"] = "M",
            ["Н"] = "N",
            ["О"] = "O",
            ["П"] = "P",
            ["Р"] = "R",
            ["С"] = "S",
            ["Т"] = "T",
            ["У"] = "U",
            ["Ф"] = "F",
            ["Х"] = "H",
            ["Ц"] = "C",
            ["Ч"] = "Ch",
            ["Ш"] = "Sh",
            ["Щ"] = "Sh",
            ["Ъ"] = "U",
            ["Ы"] = "Y",
            ["Ь"] = "",
            ["Э"] = "E",
            ["Ю"] = "Yu",
            ["Я"] = "Ya",
            ["Є"] = "Ye",
            ["І"] = "I",
            ["Ї"] = "Yi",
            ["Ґ"] = "G",
            ["є"] = "ye",
            ["і"] = "i",
            ["ї"] = "yi",
            ["ґ"] = "g"
        };

        foreach (var kvp in charMap.Where(kvp => !string.IsNullOrEmpty(kvp.Value)))
        {
            Assert.Equal($"foo-{kvp.Value.ToLower()}-bar-baz",
                Piranha.Utils.GenerateSlug($"foo {kvp.Key} bar baz"));
        }
    }

    [Fact]
    public void ReplaceKazakhCharacters() {
        var charMap = new Dictionary<string, string>
        {
            ["Ә"] = "AE",
            ["ә"] = "ae",
            ["Ғ"] = "GH",
            ["ғ"] = "gh",
            ["Қ"] = "KH",
            ["қ"] = "kh",
            ["Ң"] = "NG",
            ["ң"] = "ng",
            ["Ү"] = "UE",
            ["ү"] = "ue",
            ["Ұ"] = "U",
            ["ұ"] = "u",
            ["Һ"] = "H",
            ["һ"] = "h",
            ["Ө"] = "OE",
            ["ө"] = "oe"
        };

        foreach (var kvp in charMap)
        {
            Assert.Equal($"foo-{kvp.Value.ToLower()}-bar-baz",
                Piranha.Utils.GenerateSlug($"foo {kvp.Key} bar baz"));
        }
    }

    [Fact]
    public void ReplaceCzechCharacters() {
        var charMap = new Dictionary<string, string>
        {
            ["č"] = "c",
            ["ď"] = "d",
            ["ě"] = "e",
            ["ň"] = "n",
            ["ř"] = "r",
            ["š"] = "s",
            ["ť"] = "t",
            ["ů"] = "u",
            ["ž"] = "z",
            ["Č"] = "C",
            ["Ď"] = "D",
            ["Ě"] = "E",
            ["Ň"] = "N",
            ["Ř"] = "R",
            ["Š"] = "S",
            ["Ť"] = "T",
            ["Ů"] = "U",
            ["Ž"] = "Z"
        };

        foreach (var kvp in charMap)
        {
            Assert.Equal($"foo-{kvp.Value.ToLower()}-bar-baz",
                Piranha.Utils.GenerateSlug($"foo {kvp.Key} bar baz"));
        }
    }

    [Fact]
    public void ReplacePolishCharacters() {
        var charMap = new Dictionary<string, string>
        {
            ["ą"] = "a",
            ["ć"] = "c",
            ["ę"] = "e",
            ["ł"] = "l",
            ["ń"] = "n",
            ["ó"] = "o",
            ["ś"] = "s",
            ["ź"] = "z",
            ["ż"] = "z",
            ["Ą"] = "A",
            ["Ć"] = "C",
            ["Ę"] = "e",
            ["Ł"] = "L",
            ["Ń"] = "N",
            ["Ś"] = "S",
            ["Ź"] = "Z",
            ["Ż"] = "Z"
        };

        foreach (var kvp in charMap)
        {
            Assert.Equal($"foo-{kvp.Value.ToLower()}-bar-baz",
                Piranha.Utils.GenerateSlug($"foo {kvp.Key} bar baz"));
        }
    }

    [Fact]
    public void ReplaceLatvianCharacters() {
        var charMap = new Dictionary<string, string>
        {
            ["ā"] = "a",
            ["č"] = "c",
            ["ē"] = "e",
            ["ģ"] = "g",
            ["ī"] = "i",
            ["ķ"] = "k",
            ["ļ"] = "l",
            ["ņ"] = "n",
            ["š"] = "s",
            ["ū"] = "u",
            ["ž"] = "z",
            ["Ā"] = "A",
            ["Č"] = "C",
            ["Ē"] = "E",
            ["Ģ"] = "G",
            ["Ī"] = "i",
            ["Ķ"] = "k",
            ["Ļ"] = "L",
            ["Ņ"] = "N",
            ["Š"] = "S",
            ["Ū"] = "u",
            ["Ž"] = "Z"
        };

        foreach (var kvp in charMap)
        {
            Assert.Equal($"foo-{kvp.Value.ToLower()}-bar-baz",
                Piranha.Utils.GenerateSlug($"foo {kvp.Key} bar baz"));
        }
    }

    [Fact]
    public void ReplaceSerbianCharacters() {
        var charMap = new Dictionary<string, string>
        {
            ["đ"] = "dj",
            ["ǌ"] = "nj",
            ["ǉ"] = "lj",
            ["Đ"] = "DJ",
            ["ǋ"] = "NJ",
            ["ǈ"] = "LJ",
            ["ђ"] = "dj",
            ["ј"] = "j",
            ["љ"] = "lj",
            ["њ"] = "nj",
            ["ћ"] = "c",
            ["џ"] = "dz",
            ["Ђ"] = "DJ",
            ["Ј"] = "J",
            ["Љ"] = "LJ",
            ["Њ"] = "NJ",
            ["Ћ"] = "C",
            ["Џ"] = "DZ"
        };

        foreach (var kvp in charMap)
        {
            Assert.Equal($"foo-{kvp.Value.ToLower()}-bar-baz",
                Piranha.Utils.GenerateSlug($"foo {kvp.Key} bar baz"));
        }
    }

    [Fact]
    public void ReplaceCurrencyCharacters() {
        var charMap = new Dictionary<string, string>
        {
            ["€"] = "euro",
            ["₢"] = "cruzeiro",
            ["₣"] = "french-franc",
            ["£"] = "pound",
            ["₤"] = "lira",
            ["₥"] = "mill",
            ["₦"] = "naira",
            ["₧"] = "peseta",
            ["₨"] = "rupee",
            ["₩"] = "won",
            ["₪"] = "new-shequel",
            ["₫"] = "dong",
            ["₭"] = "kip",
            ["₮"] = "tugrik",
            ["₸"] = "kazakhstani-tenge",
            ["₯"] = "drachma",
            ["₰"] = "penny",
            ["₱"] = "peso",
            ["₲"] = "guarani",
            ["₳"] = "austral",
            ["₴"] = "hryvnia",
            ["₵"] = "cedi",
            ["¢"] = "cent",
            ["¥"] = "yen",
            ["元"] = "yuan",
            ["円"] = "yen",
            ["﷼"] = "rial",
            ["₠"] = "ecu",
            ["¤"] = "currency",
            ["฿"] = "baht",
            ["$"] = "dollar",
            ["₽"] = "russian-ruble",
            ["₿"] = "bitcoin",
            ["₺"] = "turkish-lira"
        };

        foreach (var kvp in charMap)
        {
            Assert.Equal($"foo-{kvp.Value.ToLower()}-bar-baz",
                Piranha.Utils.GenerateSlug($"foo {kvp.Key} bar baz"));
        }
    }

    [Fact]
    public void ReplaceSymbolCharacters() {
        var charMap = new Dictionary<string, string>
        {
            ["©"] = "c",
            ["œ"] = "oe",
            ["Œ"] = "OE",
            ["∑"] = "sum",
            ["®"] = "r",
            ["∂"] = "d",
            ["ƒ"] = "f",
            ["™"] = "tm",
            ["℠"] = "sm",
            ["˚"] = "o",
            ["º"] = "o",
            ["ª"] = "a",
            ["∆"] = "delta",
            ["∞"] = "infinity",
            ["♥"] = "love",
            ["&"] = "and",
            ["|"] = "or",
            ["<"] = "less",
            [">"] = "greater"
        };

        foreach (var kvp in charMap)
        {
            Assert.Equal($"foo-{kvp.Value.ToLower()}-bar-baz",
                Piranha.Utils.GenerateSlug($"foo {kvp.Key} bar baz"));
        }
    }

    [Fact]
    public void RemoveSlashesForNonHierarchical() {
        Assert.Equal("no-more-dashes", Piranha.Utils.GenerateSlug("no/more / dashes", false));
    }
}
