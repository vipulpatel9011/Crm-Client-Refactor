// <copyright file="IniConfig.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
namespace Aurea.CRM.Core.Platform
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A class for reading values by section and key from a standard ".ini" initialization file.
    /// </summary>
    /// <remarks>
    /// Section and key names are not case-sensitive. Values are loaded into a hash table for fast access.
    /// Use <see cref="GetAllValues"/> to read multiple values that share the same section and key.
    /// Sections in the initialization file must have the following form:
    /// <code>
    ///     ; comment line
    ///     [section]
    ///     key=value
    /// </code>
    /// </remarks>
    public class IniConfig
    {
        // "[section]key"   -> "value1"
        // "[section]key~2" -> "value2"
        // "[section]key~3" -> "value3"
        private readonly Dictionary<string, string> dictionary = new Dictionary<string, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="IniConfig"/> class.
        /// </summary>
        /// <param name="iniFileContents">The initialization file path.</param>
        /// <param name="commentDelimiter">The comment delimiter string (default value is ";").
        /// </param>
        public IniConfig(string iniFileContents, string commentDelimiter = ";")
        {
            CommentDelimiter = commentDelimiter;
            ParseString(iniFileContents);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniConfig"/> class.
        /// </summary>
        public IniConfig()
        {
            CommentDelimiter = ";";
        }

        /// <summary>
        /// The comment delimiter string (default value is ";").
        /// </summary>
        public string CommentDelimiter { get; set; }


        private void ParseString(string theFile)
        {
            dictionary.Clear();

            {
                var lines = theFile.Split(new[] {'\n'});

                {
                    var section = "";
                    foreach (var line1 in lines)
                    {
                        var line = line1;
                        line = line.Trim();
                        if (line.Length == 0) continue; // empty line
                        if (!String.IsNullOrEmpty(CommentDelimiter) && line.StartsWith(CommentDelimiter))
                            continue; // comment

                        if (line.StartsWith("[") && line.Contains("]")) // [section]
                        {
                            int index = line.IndexOf(']');
                            section = line.Substring(1, index - 1).Trim();
                            continue;
                        }

                        if (line.Contains("=")) // key=value
                        {
                            int index = line.IndexOf('=');
                            string key = line.Substring(0, index).Trim();
                            string val = line.Substring(index + 1).Trim();
                            string key2 = String.Format("[{0}]{1}", section, key).ToLower();

                            if (val.StartsWith("\"") && val.EndsWith("\"")) // strip quotes
                                val = val.Substring(1, val.Length - 2);

                            if (dictionary.ContainsKey(key2)) // multiple values can share the same key
                            {
                                index = 1;
                                string key3;
                                while (true)
                                {
                                    key3 = String.Format("{0}~{1}", key2, ++index);
                                    if (!dictionary.ContainsKey(key3))
                                    {
                                        dictionary.Add(key3, val);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                dictionary.Add(key2, val);
                            }
                        }
                    }
                }
            }
        }

        private bool TryGetValue(string section, string key, out string value)
        {
            string key2;
            if (section.StartsWith("["))
                key2 = String.Format("{0}{1}", section, key);
            else
                key2 = String.Format("[{0}]{1}", section, key);

            return dictionary.TryGetValue(key2.ToLower(), out value);
        }

        /// <summary>
        /// Gets a string value by section and key.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The value.</returns>
        /// <seealso cref="GetAllValues"/>
        public string GetValue(string section, string key, string defaultValue = "")
        {
            string value;
            if (!TryGetValue(section, key, out value))
            {
                if (!TryGetValue("[]", key, out value))
                {
                    return defaultValue;
                }
            }

            return value;
        }

        /// <summary>
        /// Gets a string value by section and key.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="key">The key.</param>
        /// <returns>The value.</returns>
        /// <seealso cref="GetValue"/>
        public string this[string section, string key]
        {
            get { return GetValue(section, key); }
        }

        /// <summary>
        /// Gets an array of string values by section and key.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="key">The key.</param>
        /// <returns>The array of values, or null if none found.</returns>
        /// <seealso cref="GetValue"/>
        public string[] GetAllValues(string section, string key)
        {
            string key2, key3, value;
            if (section.StartsWith("["))
                key2 = String.Format("{0}{1}", section, key).ToLower();
            else
                key2 = String.Format("[{0}]{1}", section, key).ToLower();

            if (!dictionary.TryGetValue(key2, out value))
                return null;

            List<string> values = new List<string>();
            values.Add(value);
            int index = 1;
            while (true)
            {
                key3 = String.Format("{0}~{1}", key2, ++index);
                if (!dictionary.TryGetValue(key3, out value))
                    break;
                values.Add(value);
            }

            return values.ToArray();
        }
    }
}
