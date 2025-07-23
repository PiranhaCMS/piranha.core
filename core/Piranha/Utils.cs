/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Piranha.Extend;
using Piranha.Runtime;

namespace Piranha;

/// <summary>
/// Utility methods.
/// </summary>
public static class Utils
{
    /// <summary>
    /// Gets a subset of the given array as a new array.
    /// </summary>
    /// <typeparam name="T">The array type</typeparam>
    /// <param name="arr">The array</param>
    /// <param name="startpos">The startpos</param>
    /// <param name="length">The length</param>
    /// <returns>The new array</returns>
    public static T[] Subset<T>(this T[] arr, int startpos = 0, int length = 0)
    {
        List<T> tmp = new List<T>();

        length = length > 0 ? length : arr.Length - startpos;

        for (var i = 0; i < arr.Length; i++)
        {
            if (i >= startpos && i < (startpos + length))
            {
                tmp.Add(arr[i]);
            }
        }
        return tmp.ToArray();
    }

    /// <summary>
    /// Generates a slug from the given string.
    /// </summary>
    /// <param name="str">The string</param>
    /// <param name="hierarchical">If forward slashes should be allowed</param>
    /// <returns>The slug</returns>
    public static string GenerateSlug(string str, bool hierarchical = true)
    {
        if (App.Hooks != null && App.Hooks.OnGenerateSlug != null)
        {
            // Call the registered slug generation
            return App.Hooks.OnGenerateSlug(str);
        }

        // Trim & make lower case
        var slug = str.Trim().ToLower();

        // https://github.com/simov/slugify
        // Convert culture specific characters
        slug = slug
            .Replace("$", "dollar")
            .Replace("%", "percent")
            .Replace("&", "and")
            .Replace("<", "less")
            .Replace(">", "greater")
            .Replace("|", "or")
            .Replace("¢", "cent")
            .Replace("£", "pound")
            .Replace("¤", "currency")
            .Replace("¥", "yen")
            .Replace("©", "c")
            .Replace("ª", "a")
            .Replace("®", "r")
            .Replace("º", "o")
            .Replace("À", "a")
            .Replace("Á", "a")
            .Replace("Â", "a")
            .Replace("Ã", "a")
            .Replace("Ä", "a")
            .Replace("Å", "a")
            .Replace("Æ", "ae")
            .Replace("Ç", "c")
            .Replace("È", "e")
            .Replace("É", "e")
            .Replace("Ê", "e")
            .Replace("Ë", "e")
            .Replace("Ì", "i")
            .Replace("Í", "i")
            .Replace("Î", "i")
            .Replace("Ï", "i")
            .Replace("Ð", "d")
            .Replace("Ñ", "n")
            .Replace("Ò", "o")
            .Replace("Ó", "o")
            .Replace("Ô", "o")
            .Replace("Õ", "o")
            .Replace("Ö", "o")
            .Replace("Ø", "o")
            .Replace("Ù", "u")
            .Replace("Ú", "u")
            .Replace("Û", "u")
            .Replace("Ü", "u")
            .Replace("Ý", "y")
            .Replace("Þ", "th")
            .Replace("ß", "ss")
            .Replace("à", "a")
            .Replace("á", "a")
            .Replace("â", "a")
            .Replace("ã", "a")
            .Replace("ä", "a")
            .Replace("å", "a")
            .Replace("æ", "ae")
            .Replace("ç", "c")
            .Replace("è", "e")
            .Replace("é", "e")
            .Replace("ê", "e")
            .Replace("ë", "e")
            .Replace("ì", "i")
            .Replace("í", "i")
            .Replace("î", "i")
            .Replace("ï", "i")
            .Replace("ð", "d")
            .Replace("ñ", "n")
            .Replace("ò", "o")
            .Replace("ó", "o")
            .Replace("ô", "o")
            .Replace("õ", "o")
            .Replace("ö", "o")
            .Replace("ø", "o")
            .Replace("ù", "u")
            .Replace("ú", "u")
            .Replace("û", "u")
            .Replace("ü", "u")
            .Replace("ý", "y")
            .Replace("þ", "th")
            .Replace("ÿ", "y")
            .Replace("Ā", "a")
            .Replace("ā", "a")
            .Replace("Ă", "a")
            .Replace("ă", "a")
            .Replace("Ą", "a")
            .Replace("ą", "a")
            .Replace("Ć", "c")
            .Replace("ć", "c")
            .Replace("Č", "c")
            .Replace("č", "c")
            .Replace("Ď", "d")
            .Replace("ď", "d")
            .Replace("Đ", "dj")
            .Replace("đ", "dj")
            .Replace("Ē", "e")
            .Replace("ē", "e")
            .Replace("Ė", "e")
            .Replace("ė", "e")
            .Replace("Ę", "e")
            .Replace("ę", "e")
            .Replace("Ě", "e")
            .Replace("ě", "e")
            .Replace("Ğ", "g")
            .Replace("ğ", "g")
            .Replace("Ģ", "g")
            .Replace("ģ", "g")
            .Replace("Ĩ", "i")
            .Replace("ĩ", "i")
            .Replace("Ī", "i")
            .Replace("ī", "i")
            .Replace("Į", "i")
            .Replace("į", "i")
            .Replace("İ", "i")
            .Replace("ı", "i")
            .Replace("Ķ", "k")
            .Replace("ķ", "k")
            .Replace("Ļ", "l")
            .Replace("ļ", "l")
            .Replace("Ľ", "l")
            .Replace("ľ", "l")
            .Replace("Ł", "l")
            .Replace("ł", "l")
            .Replace("Ń", "n")
            .Replace("ń", "n")
            .Replace("Ņ", "n")
            .Replace("ņ", "n")
            .Replace("Ň", "n")
            .Replace("ň", "n")
            .Replace("Ō", "o")
            .Replace("ō", "o")
            .Replace("Ő", "o")
            .Replace("ő", "o")
            .Replace("Œ", "oe")
            .Replace("œ", "oe")
            .Replace("Ŕ", "r")
            .Replace("ŕ", "r")
            .Replace("Ř", "r")
            .Replace("ř", "r")
            .Replace("Ś", "s")
            .Replace("ś", "s")
            .Replace("Ş", "s")
            .Replace("ş", "s")
            .Replace("Š", "s")
            .Replace("š", "s")
            .Replace("Ţ", "t")
            .Replace("ţ", "t")
            .Replace("Ť", "t")
            .Replace("ť", "t")
            .Replace("Ũ", "u")
            .Replace("ũ", "u")
            .Replace("Ū", "u")
            .Replace("ū", "u")
            .Replace("Ů", "u")
            .Replace("ů", "u")
            .Replace("Ű", "u")
            .Replace("ű", "u")
            .Replace("Ų", "u")
            .Replace("ų", "u")
            .Replace("Ŵ", "w")
            .Replace("ŵ", "w")
            .Replace("Ŷ", "y")
            .Replace("ŷ", "y")
            .Replace("Ÿ", "y")
            .Replace("Ź", "z")
            .Replace("ź", "z")
            .Replace("Ż", "z")
            .Replace("ż", "z")
            .Replace("Ž", "z")
            .Replace("ž", "z")
            .Replace("Ə", "e")
            .Replace("ƒ", "f")
            .Replace("Ơ", "o")
            .Replace("ơ", "o")
            .Replace("Ư", "u")
            .Replace("ư", "u")
            .Replace("ǈ", "lj")
            .Replace("ǉ", "lj")
            .Replace("ǋ", "nj")
            .Replace("ǌ", "nj")
            .Replace("Ș", "s")
            .Replace("ș", "s")
            .Replace("Ț", "t")
            .Replace("ț", "t")
            .Replace("ə", "e")
            .Replace("˚", "o")
            .Replace("Ά", "a")
            .Replace("Έ", "e")
            .Replace("Ή", "h")
            .Replace("Ί", "i")
            .Replace("Ό", "o")
            .Replace("Ύ", "y")
            .Replace("Ώ", "w")
            .Replace("ΐ", "i")
            .Replace("Α", "a")
            .Replace("Β", "b")
            .Replace("Γ", "g")
            .Replace("Δ", "d")
            .Replace("Ε", "e")
            .Replace("Ζ", "z")
            .Replace("Η", "h")
            .Replace("Θ", "8")
            .Replace("Ι", "i")
            .Replace("Κ", "k")
            .Replace("Λ", "l")
            .Replace("Μ", "m")
            .Replace("Ν", "n")
            .Replace("Ξ", "3")
            .Replace("Ο", "o")
            .Replace("Π", "p")
            .Replace("Ρ", "r")
            .Replace("Σ", "s")
            .Replace("Τ", "t")
            .Replace("Υ", "y")
            .Replace("Φ", "f")
            .Replace("Χ", "x")
            .Replace("Ψ", "ps")
            .Replace("Ω", "w")
            .Replace("Ϊ", "i")
            .Replace("Ϋ", "y")
            .Replace("ά", "a")
            .Replace("έ", "e")
            .Replace("ή", "h")
            .Replace("ί", "i")
            .Replace("ΰ", "y")
            .Replace("α", "a")
            .Replace("β", "b")
            .Replace("γ", "g")
            .Replace("δ", "d")
            .Replace("ε", "e")
            .Replace("ζ", "z")
            .Replace("η", "h")
            .Replace("θ", "8")
            .Replace("ι", "i")
            .Replace("κ", "k")
            .Replace("λ", "l")
            .Replace("μ", "m")
            .Replace("ν", "n")
            .Replace("ξ", "3")
            .Replace("ο", "o")
            .Replace("π", "p")
            .Replace("ρ", "r")
            .Replace("ς", "s")
            .Replace("σ", "s")
            .Replace("τ", "t")
            .Replace("υ", "y")
            .Replace("φ", "f")
            .Replace("χ", "x")
            .Replace("ψ", "ps")
            .Replace("ω", "w")
            .Replace("ϊ", "i")
            .Replace("ϋ", "y")
            .Replace("ό", "o")
            .Replace("ύ", "y")
            .Replace("ώ", "w")
            .Replace("Ё", "yo")
            .Replace("Ђ", "dj")
            .Replace("Є", "ye")
            .Replace("І", "i")
            .Replace("Ї", "yi")
            .Replace("Ј", "j")
            .Replace("Љ", "lj")
            .Replace("Њ", "nj")
            .Replace("Ћ", "c")
            .Replace("Џ", "dz")
            .Replace("А", "a")
            .Replace("Б", "b")
            .Replace("В", "v")
            .Replace("Г", "g")
            .Replace("Д", "d")
            .Replace("Е", "e")
            .Replace("Ж", "zh")
            .Replace("З", "z")
            .Replace("И", "i")
            .Replace("Й", "j")
            .Replace("К", "k")
            .Replace("Л", "l")
            .Replace("М", "m")
            .Replace("Н", "n")
            .Replace("О", "o")
            .Replace("П", "p")
            .Replace("Р", "r")
            .Replace("С", "s")
            .Replace("Т", "t")
            .Replace("У", "u")
            .Replace("Ф", "f")
            .Replace("Х", "h")
            .Replace("Ц", "c")
            .Replace("Ч", "ch")
            .Replace("Ш", "sh")
            .Replace("Щ", "sh")
            .Replace("Ъ", "u")
            .Replace("Ы", "y")
            .Replace("Ь", "")
            .Replace("Э", "e")
            .Replace("Ю", "yu")
            .Replace("Я", "ya")
            .Replace("а", "a")
            .Replace("б", "b")
            .Replace("в", "v")
            .Replace("г", "g")
            .Replace("д", "d")
            .Replace("е", "e")
            .Replace("ж", "zh")
            .Replace("з", "z")
            .Replace("и", "i")
            .Replace("й", "j")
            .Replace("к", "k")
            .Replace("л", "l")
            .Replace("м", "m")
            .Replace("н", "n")
            .Replace("о", "o")
            .Replace("п", "p")
            .Replace("р", "r")
            .Replace("с", "s")
            .Replace("т", "t")
            .Replace("у", "u")
            .Replace("ф", "f")
            .Replace("х", "h")
            .Replace("ц", "c")
            .Replace("ч", "ch")
            .Replace("ш", "sh")
            .Replace("щ", "sh")
            .Replace("ъ", "u")
            .Replace("ы", "y")
            .Replace("ь", "")
            .Replace("э", "e")
            .Replace("ю", "yu")
            .Replace("я", "ya")
            .Replace("ё", "yo")
            .Replace("ђ", "dj")
            .Replace("є", "ye")
            .Replace("і", "i")
            .Replace("ї", "yi")
            .Replace("ј", "j")
            .Replace("љ", "lj")
            .Replace("њ", "nj")
            .Replace("ћ", "c")
            .Replace("ѝ", "u")
            .Replace("џ", "dz")
            .Replace("Ґ", "g")
            .Replace("ґ", "g")
            .Replace("Ғ", "gh")
            .Replace("ғ", "gh")
            .Replace("Қ", "kh")
            .Replace("қ", "kh")
            .Replace("Ң", "ng")
            .Replace("ң", "ng")
            .Replace("Ү", "ue")
            .Replace("ү", "ue")
            .Replace("Ұ", "u")
            .Replace("ұ", "u")
            .Replace("Һ", "h")
            .Replace("һ", "h")
            .Replace("Ә", "ae")
            .Replace("ә", "ae")
            .Replace("Ө", "oe")
            .Replace("ө", "oe")
            .Replace("Ա", "a")
            .Replace("Բ", "b")
            .Replace("Գ", "g")
            .Replace("Դ", "d")
            .Replace("Ե", "e")
            .Replace("Զ", "z")
            .Replace("Է", "e")
            .Replace("Ը", "y")
            .Replace("Թ", "t")
            .Replace("Ժ", "jh")
            .Replace("Ի", "i")
            .Replace("Լ", "l")
            .Replace("Խ", "x")
            .Replace("Ծ", "c")
            .Replace("Կ", "k")
            .Replace("Հ", "h")
            .Replace("Ձ", "d")
            .Replace("Ղ", "gh")
            .Replace("Ճ", "tw")
            .Replace("Մ", "m")
            .Replace("Յ", "y")
            .Replace("Ն", "n")
            .Replace("Շ", "sh")
            .Replace("Չ", "ch")
            .Replace("Պ", "p")
            .Replace("Ջ", "j")
            .Replace("Ռ", "r")
            .Replace("Ս", "s")
            .Replace("Վ", "v")
            .Replace("Տ", "t")
            .Replace("Ր", "r")
            .Replace("Ց", "c")
            .Replace("Փ", "p")
            .Replace("Ք", "q")
            .Replace("Օ", "o")
            .Replace("Ֆ", "f")
            .Replace("և", "ev")
            .Replace("ء", "a")
            .Replace("آ", "aa")
            .Replace("أ", "a")
            .Replace("ؤ", "u")
            .Replace("إ", "i")
            .Replace("ئ", "e")
            .Replace("ا", "a")
            .Replace("ب", "b")
            .Replace("ة", "h")
            .Replace("ت", "t")
            .Replace("ث", "th")
            .Replace("ج", "j")
            .Replace("ح", "h")
            .Replace("خ", "kh")
            .Replace("د", "d")
            .Replace("ذ", "th")
            .Replace("ر", "r")
            .Replace("ز", "z")
            .Replace("س", "s")
            .Replace("ش", "sh")
            .Replace("ص", "s")
            .Replace("ض", "dh")
            .Replace("ط", "t")
            .Replace("ظ", "z")
            .Replace("ع", "a")
            .Replace("غ", "gh")
            .Replace("ف", "f")
            .Replace("ق", "q")
            .Replace("ك", "k")
            .Replace("ل", "l")
            .Replace("م", "m")
            .Replace("ن", "n")
            .Replace("ه", "h")
            .Replace("و", "w")
            .Replace("ى", "a")
            .Replace("ي", "y")
            .Replace("ً", "an")
            .Replace("ٌ", "on")
            .Replace("ٍ", "en")
            .Replace("َ", "a")
            .Replace("ُ", "u")
            .Replace("ِ", "e")
            .Replace("ْ", "")
            .Replace("٠", "0")
            .Replace("١", "1")
            .Replace("٢", "2")
            .Replace("٣", "3")
            .Replace("٤", "4")
            .Replace("٥", "5")
            .Replace("٦", "6")
            .Replace("٧", "7")
            .Replace("٨", "8")
            .Replace("٩", "9")
            .Replace("پ", "p")
            .Replace("چ", "ch")
            .Replace("ژ", "zh")
            .Replace("ک", "k")
            .Replace("گ", "g")
            .Replace("ی", "y")
            .Replace("۰", "0")
            .Replace("۱", "1")
            .Replace("۲", "2")
            .Replace("۳", "3")
            .Replace("۴", "4")
            .Replace("۵", "5")
            .Replace("۶", "6")
            .Replace("۷", "7")
            .Replace("۸", "8")
            .Replace("۹", "9")
            .Replace("฿", "baht")
            .Replace("ა", "a")
            .Replace("ბ", "b")
            .Replace("გ", "g")
            .Replace("დ", "d")
            .Replace("ე", "e")
            .Replace("ვ", "v")
            .Replace("ზ", "z")
            .Replace("თ", "t")
            .Replace("ი", "i")
            .Replace("კ", "k")
            .Replace("ლ", "l")
            .Replace("მ", "m")
            .Replace("ნ", "n")
            .Replace("ო", "o")
            .Replace("პ", "p")
            .Replace("ჟ", "zh")
            .Replace("რ", "r")
            .Replace("ს", "s")
            .Replace("ტ", "t")
            .Replace("უ", "u")
            .Replace("ფ", "f")
            .Replace("ქ", "k")
            .Replace("ღ", "gh")
            .Replace("ყ", "q")
            .Replace("შ", "sh")
            .Replace("ჩ", "ch")
            .Replace("ც", "ts")
            .Replace("ძ", "dz")
            .Replace("წ", "ts")
            .Replace("ჭ", "ch")
            .Replace("ხ", "kh")
            .Replace("ჯ", "j")
            .Replace("ჰ", "h")
            .Replace("Ṣ", "s")
            .Replace("ṣ", "s")
            .Replace("Ẁ", "w")
            .Replace("ẁ", "w")
            .Replace("Ẃ", "w")
            .Replace("ẃ", "w")
            .Replace("Ẅ", "w")
            .Replace("ẅ", "w")
            .Replace("ẞ", "ss")
            .Replace("Ạ", "a")
            .Replace("ạ", "a")
            .Replace("Ả", "a")
            .Replace("ả", "a")
            .Replace("Ấ", "a")
            .Replace("ấ", "a")
            .Replace("Ầ", "a")
            .Replace("ầ", "a")
            .Replace("Ẩ", "a")
            .Replace("ẩ", "a")
            .Replace("Ẫ", "a")
            .Replace("ẫ", "a")
            .Replace("Ậ", "a")
            .Replace("ậ", "a")
            .Replace("Ắ", "a")
            .Replace("ắ", "a")
            .Replace("Ằ", "a")
            .Replace("ằ", "a")
            .Replace("Ẳ", "a")
            .Replace("ẳ", "a")
            .Replace("Ẵ", "a")
            .Replace("ẵ", "a")
            .Replace("Ặ", "a")
            .Replace("ặ", "a")
            .Replace("Ẹ", "e")
            .Replace("ẹ", "e")
            .Replace("Ẻ", "e")
            .Replace("ẻ", "e")
            .Replace("Ẽ", "e")
            .Replace("ẽ", "e")
            .Replace("Ế", "e")
            .Replace("ế", "e")
            .Replace("Ề", "e")
            .Replace("ề", "e")
            .Replace("Ể", "e")
            .Replace("ể", "e")
            .Replace("Ễ", "e")
            .Replace("ễ", "e")
            .Replace("Ệ", "e")
            .Replace("ệ", "e")
            .Replace("Ỉ", "i")
            .Replace("ỉ", "i")
            .Replace("Ị", "i")
            .Replace("ị", "i")
            .Replace("Ọ", "o")
            .Replace("ọ", "o")
            .Replace("Ỏ", "o")
            .Replace("ỏ", "o")
            .Replace("Ố", "o")
            .Replace("ố", "o")
            .Replace("Ồ", "o")
            .Replace("ồ", "o")
            .Replace("Ổ", "o")
            .Replace("ổ", "o")
            .Replace("Ỗ", "o")
            .Replace("ỗ", "o")
            .Replace("Ộ", "o")
            .Replace("ộ", "o")
            .Replace("Ớ", "o")
            .Replace("ớ", "o")
            .Replace("Ờ", "o")
            .Replace("ờ", "o")
            .Replace("Ở", "o")
            .Replace("ở", "o")
            .Replace("Ỡ", "o")
            .Replace("ỡ", "o")
            .Replace("Ợ", "o")
            .Replace("ợ", "o")
            .Replace("Ụ", "u")
            .Replace("ụ", "u")
            .Replace("Ủ", "u")
            .Replace("ủ", "u")
            .Replace("Ứ", "u")
            .Replace("ứ", "u")
            .Replace("Ừ", "u")
            .Replace("ừ", "u")
            .Replace("Ử", "u")
            .Replace("ử", "u")
            .Replace("Ữ", "u")
            .Replace("ữ", "u")
            .Replace("Ự", "u")
            .Replace("ự", "u")
            .Replace("Ỳ", "y")
            .Replace("ỳ", "y")
            .Replace("Ỵ", "y")
            .Replace("ỵ", "y")
            .Replace("Ỷ", "y")
            .Replace("ỷ", "y")
            .Replace("Ỹ", "y")
            .Replace("ỹ", "y")
            .Replace("–", "-")
            .Replace("₠", "ecu")
            .Replace("₢", "cruzeiro")
            .Replace("₣", "french-franc")
            .Replace("₤", "lira")
            .Replace("₥", "mill")
            .Replace("₦", "naira")
            .Replace("₧", "peseta")
            .Replace("₨", "rupee")
            .Replace("₩", "won")
            .Replace("₪", "new-shequel")
            .Replace("₫", "dong")
            .Replace("€", "euro")
            .Replace("₭", "kip")
            .Replace("₮", "tugrik")
            .Replace("₯", "drachma")
            .Replace("₰", "penny")
            .Replace("₱", "peso")
            .Replace("₲", "guarani")
            .Replace("₳", "austral")
            .Replace("₴", "hryvnia")
            .Replace("₵", "cedi")
            .Replace("₸", "kazakhstani-tenge")
            .Replace("₹", "indian-rupee")
            .Replace("₺", "turkish-lira")
            .Replace("₽", "russian-ruble")
            .Replace("₿", "bitcoin")
            .Replace("℠", "sm")
            .Replace("™", "tm")
            .Replace("∂", "d")
            .Replace("∆", "delta")
            .Replace("∑", "sum")
            .Replace("∞", "infinity")
            .Replace("♥", "love")
            .Replace("元", "yuan")
            .Replace("円", "yen")
            .Replace("﷼", "rial")
            .Replace("ﻵ", "laa")
            .Replace("ﻷ", "laa")
            .Replace("ﻹ", "lai")
            .Replace("ﻻ", "la");

        // Remove special characters
        slug = Regex.Replace(slug, @"[^a-z\u0600-\u06FF0-9-/ ]", "").Replace("--", "-");

        // Remove whitespaces
        slug = Regex.Replace(slug.Replace("-", " "), @"\s+", " ").Replace(" ", "-");

        // Remove slash if non-hierarchical
        if (!hierarchical)
            slug = slug.Replace("/", "-");

        // Remove multiple dashes
        slug = Regex.Replace(slug, @"[-]+", "-");

        // Remove leading & trailing dashes
        if (slug.EndsWith("-"))
            slug = slug.Substring(0, slug.LastIndexOf("-"));
        if (slug.StartsWith("-"))
            slug = slug.Substring(Math.Min(slug.IndexOf("-") + 1, slug.Length));
        return slug;
    }

    /// <summary>
    /// Generates a camel cased internal id from the given string.
    /// </summary>
    /// <param name="str">The string</param>
    /// <returns>The internal id</returns>
    public static string GenerateInternalId(string str)
    {
        var ti = new CultureInfo("en-US", false).TextInfo;
        return ti.ToTitleCase(GenerateSlug(str).Replace('-', ' ')).Replace(" ", "");
    }

    /// <summary>
    /// Generates a ETag from the given name and date.
    /// </summary>
    /// <param name="name">The resource name</param>
    /// <param name="date">The modification date</param>
    /// <returns>The etag</returns>
    public static string GenerateETag(string name, DateTime date)
    {
        var encoding = new UTF8Encoding();

        using (var crypto = MD5.Create())
        {
            var str = name + date.ToString("yyyy-MM-dd HH:mm:ss");
            var bytes = crypto.ComputeHash(encoding.GetBytes(str));
            return $"\"{Convert.ToBase64String(bytes)}\"";
        }
    }

    /// <summary>
    /// Gets the gravatar URL from the given parameters.
    /// </summary>
    /// <param name="email">The email address</param>
    /// <param name="size">The requested size</param>
    /// <returns>The gravatar URL</returns>
    public static string GenerateGravatarUrl(string email, int size = 0)
    {
        using (var md5 = MD5.Create())
        {
            var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(email));

            var sb = new StringBuilder(bytes.Length * 2);
            for (var n = 0; n < bytes.Length; n++)
            {
                sb.Append(bytes[n].ToString("X2"));
            }
            return "https://www.gravatar.com/avatar/" + sb.ToString().ToLower() +
                    (size > 0 ? "?s=" + size : "");
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static string FormatByteSize(double bytes)
    {
        string[] SizeSuffixes = { "bytes", "KB", "MB", "GB" };

        int index = 0;
        if (bytes > 1023)
        {
            do
            {
                bytes /= 1024;
                index++;
            } while (bytes >= 1024 && index < 3);
        }

        return $"{bytes:0.00} {SizeSuffixes[index]}";
    }

    /// <summary>
    /// Gets the first paragraph from the given html string.
    /// </summary>
    /// <param name="str">The string</param>
    /// <returns>The first paragraph</returns>
    public static string FirstParagraph(string str)
    {
        Regex reg = new Regex("<p[^>]*>.*?</p>");
        var matches = reg.Matches(str);

        return matches.Count > 0 ? matches[0].Value : "";
    }

    /// <summary>
    /// Gets the first paragraph from the given markdown field.
    /// </summary>
    /// <param name="md">The field</param>
    /// <returns>The first paragraph</returns>
    public static string FirstParagraph(Extend.Fields.MarkdownField md)
    {
        Regex reg = new Regex("<p[^>]*>.*?</p>");
        var matches = reg.Matches(md.ToHtml());

        return matches.Count > 0 ? matches[0].Value : "";
    }

    /// <summary>
    /// Gets the first paragraph from the given html field.
    /// </summary>
    /// <param name="html">The field</param>
    /// <returns>The first paragraph</returns>
    public static string FirstParagraph(Extend.Fields.HtmlField html)
    {
        Regex reg = new Regex("<p[^>]*>.*?</p>");
        var matches = reg.Matches(html.Value);

        return matches.Count > 0 ? matches[0].Value : "";
    }

    /// <summary>
    /// Gets the formatted three digit version number of the given assembly.
    /// </summary>
    /// <param name="assembly">The assembly</param>
    /// <returns>The version string</returns>
    public static string GetAssemblyVersion(Assembly assembly)
    {
        var version = assembly.GetName().Version;

        return $"{version.Major}.{version.Minor}.{version.Build}";
    }

    /// <summary>
    /// Gets the hashed version string of the given assembly.
    /// </summary>
    /// <param name="assembly">The assembly</param>
    /// <returns>The hashed version string</returns>
    public static string GetAssemblyVersionHash(Assembly assembly)
    {
        return Math.Abs(GetAssemblyVersion(assembly).GetHashCode()).ToString();
    }

    /// <summary>
    /// Checks if the given assembly is a pre-release.
    /// </summary>
    /// <param name="assembly">The assembly</param>
    /// <returns>If it is a pre-release</returns>
    public static bool IsPreRelease(Assembly assembly)
    {
        var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion.ToLower();

        return version.Contains("alpha") || version.Contains("beta");
    }

    /// <summary>
    /// Clones the entire given object into a new instance.
    /// </summary>
    /// <param name="obj">The object to clone</param>
    /// <typeparam name="T">The object type</typeparam>
    /// <returns>The cloned instance</returns>
    public static T DeepClone<T>(T obj)
    {
        if (obj == null)
        {
            // Null value does not need to be cloned.
            return default(T);
        }

        if (obj is ValueType)
        {
            // Value types do not need to be cloned.
            return obj;
        }

        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };
        var json = JsonConvert.SerializeObject(obj, settings);

        return JsonConvert.DeserializeObject<T>(json, settings);
    }

    /// <summary>
    /// Gets the value of the property with the given name for the
    /// given instance.
    /// </summary>
    /// <param name="type">The current type</param>
    /// <param name="propertyName">The property name</param>
    /// <param name="instance">The object instance</param>
    /// <returns>The property value</returns>
    public static object GetPropertyValue(this Type type, string propertyName, object instance)
    {
        var property = type.GetProperty(propertyName, App.PropertyBindings);

        if (property != null)
        {
            return property.GetValue(instance);
        }
        return null;
    }

    /// <summary>
    /// Sets the value of the property with the given name for the
    /// given instance.
    /// </summary>
    /// <param name="type">The current type</param>
    /// <param name="propertyName">The property name</param>
    /// <param name="instance">The object instance</param>
    /// <param name="value">The value to set</param>
    public static void SetPropertyValue(this Type type, string propertyName, object instance, object value)
    {
        var property = type.GetProperty(propertyName, App.PropertyBindings);

        if (property != null)
        {
            property.SetValue(instance, value);
        }
    }

    /// <summary>
    /// Gets the app method with the given name for the specified type.
    /// </summary>
    /// <param name="name">The method name</param>
    /// <typeparam name="T">The type</typeparam>
    /// <returns>The method if found, otherwise null</returns>
    internal static AppMethod GetMethod<T>(string name)
    {
        var methodInfo = typeof(T).GetMethod(name);

        if (methodInfo != null)
        {
            var method = new AppMethod
            {
                Method = methodInfo,
                IsAsync = typeof(Task).IsAssignableFrom(methodInfo.ReturnType)
            };

            foreach (var p in methodInfo.GetParameters())
            {
                method.ParameterTypes.Add(p.ParameterType);
            }
            return method;
        }
        return null;
    }

    /// <summary>
    /// Gets the attribute defined settings for the given property.
    /// </summary>
    /// <param name="prop">The property info</param>
    /// <returns>The settings</returns>
    public static IDictionary<string, object> GetFieldSettings(PropertyInfo prop)
    {
        var settings = new Dictionary<string, object>();

        // Get optional settings
        var settingsAttr = prop.GetCustomAttribute<FieldSettingsAttribute>();
        if (settingsAttr != null)
        {
            foreach (var setting in settingsAttr.GetType().GetProperties(BindingFlags.Instance|BindingFlags.Public|BindingFlags.DeclaredOnly))
            {
                if (settings.TryGetValue(setting.Name, out var existing))
                {
                    if (!(existing is IList))
                    {
                        existing = new ArrayList()
                        {
                            existing
                        };
                        settings[setting.Name] = existing;
                    }

                    ((IList)existing).Add(setting.GetValue(settingsAttr));
                }
                else
                {
                    settings[setting.Name] = setting.GetValue(settingsAttr);
                }
            }
        }
        return settings;
    }
}
