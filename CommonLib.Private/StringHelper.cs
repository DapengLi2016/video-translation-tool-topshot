// <copyright file="StringHelper.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.SpeechServices.CustomVoice.TtsLib
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading;

    public static class StringHelper
    {
        /// <summary>
        /// Formats the given formattable using the invariant culture.
        /// </summary>
        /// <remarks>Handy using with string interpolation</remarks>
        /// <param name="formattableString">The formattable string.</param>
        /// <returns>The string formatted using invariant culture.</returns>
        public static string AsInvariant(FormattableString formattableString)
        {
            if (formattableString == null)
            {
                throw new ArgumentNullException(nameof(formattableString));
            }

            return AsCulture(formattableString, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Formats the given formattable using the current culture of the current thread.
        /// </summary>
        /// <remarks>Handy using with string interpolation</remarks>
        /// <param name="formattableString">The formattable string.</param>
        /// <returns>The string formatted using the current culture.</returns>
        public static string AsCurrentCulture(FormattableString formattableString)
        {
            if (formattableString == null)
            {
                throw new ArgumentNullException(nameof(formattableString));
            }

            return AsCulture(formattableString, Thread.CurrentThread.CurrentCulture);
        }

        /// <summary>
        /// Formats the given formattable using the current UI culture of the current thread.
        /// </summary>
        /// <remarks>Handy using with string interpolation</remarks>
        /// <param name="formattableString">The formattable string.</param>
        /// <returns>The string formatted using the current UI culture.</returns>
        public static string AsCurrentUICulture(FormattableString formattableString)
        {
            if (formattableString == null)
            {
                throw new ArgumentNullException(nameof(formattableString));
            }

            return AsCulture(formattableString, Thread.CurrentThread.CurrentUICulture);
        }

        /// <summary>
        /// Returns the left substring of at most the given number of characters. The result is shorter if the original
        /// string is shorter than this number.
        /// </summary>
        /// <param name="original">The original string.</param>
        /// <param name="maxCount">The maximum number of characters that the substring should have.</param>
        /// <returns>The substring.</returns>
        public static string LeftSubstring(this string original, int maxCount)
        {
            if (original == null)
            {
                throw new ArgumentNullException(nameof(original));
            }

            return original.Substring(0, Math.Min(maxCount, original.Length));
        }

        public static string GetBase64StringFromString(string str)
        {
            byte[] toEncodeAsBytes = Encoding.UTF8.GetBytes(str);

            return Convert.ToBase64String(toEncodeAsBytes);
        }

        public static IReadOnlyDictionary<TKey, TValue> SplitKeyValuePairs<TKey, TValue>(
            this string serializedPairs,
            char pairSplitCharacter,
            char keyValueSplitCharacter,
            Func<string, TKey> keyFactory,
            Func<string, TValue> valueFactory)
        {
            if (serializedPairs == null)
            {
                return null;
            }

            return serializedPairs.Split(new[] { pairSplitCharacter }, StringSplitOptions.RemoveEmptyEntries)
                .Select(pair => pair.Split(new[] { keyValueSplitCharacter }, StringSplitOptions.RemoveEmptyEntries))
                .ToDictionary(splitPair => keyFactory(splitPair[0]), splitPair => valueFactory(splitPair[1]));
        }

        private static string AsCulture(FormattableString formattableString, CultureInfo cultureInfo)
        {
            return formattableString.ToString(cultureInfo);
        }
    }
}