// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PListService.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Rashan Anushka
// </author>
// <summary>
//   Helper routines to handle plists
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;

    using Aurea.CRM.Core.Platform;
    using Aurea.CRM.Core.Session;

    //using Microsoft.Practices.ServiceLocation;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// Helper routines to handle plists
    /// </summary>
    public static class PListService
    {
        /// <summary>
        /// The login application mapping plist name
        /// </summary>
        public const string LoginAppMapping = "loginMapping";

        /// <summary>
        /// The calender related plist file name
        /// </summary>
        public const string UPPListiPadCalender = "iPadCalendar";

        /// <summary>
        /// The list layout constants plist name
        /// </summary>
        public const string UPPListLayoutConstants = "layoutConstants";

        /// <summary>
        /// Gets the cache path.
        /// </summary>
        /// <value>
        /// The cache path.
        /// </value>
        public static string CachePath
            => SimpleIoc.Default.GetInstance<IPlatformService>().StorageProvider.CachesFolderPath;

        /// <summary>
        /// Gets the document path.
        /// </summary>
        /// <value>
        /// The document path.
        /// </value>
        public static string DocumentPath
            => SimpleIoc.Default.GetInstance<IPlatformService>().StorageProvider.DocumentsFolderPath;

        /// <summary>
        /// Loads the properties defined ona given plist.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="rootpath">
        /// The rootpath.
        /// </param>
        /// <returns>
        /// the plist instance
        /// </returns>
        public static PList LoadProperties(string name, string rootpath)
        {
            var path = ListPath(name, rootpath);

            return SimpleIoc.Default.GetInstance<IPlatformService>().StorageProvider.FileExists(path)
                       ? Load(path)
                       : null;
        }

        /// <summary>
        /// Loads the properties as an array.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="rootpath">
        /// The rootpath.
        /// </param>
        /// <returns>
        /// an array of plist values
        /// </returns>
        public static List<PListValue> LoadPropertiesAsArray(string name, string rootpath)
        {
            var path = ListPath(name, rootpath);

            return SimpleIoc.Default.GetInstance<IPlatformService>().StorageProvider.FileExists(path)
                       ? Load(path, ListRootType.Array)
                       : null;
        }

        /// <summary>
        /// Saves the specified properties.
        /// </summary>
        /// <param name="properties">
        /// The properties.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="rootpath">
        /// The rootpath.
        /// </param>
        /// <returns>
        /// true if success;else false
        /// </returns>
        public static bool Save(PList properties, string name, string rootpath)
        {
            var path = ListPath(name, rootpath);
            Save(path, w => ComposeDictionary(properties, w));
            return true;
        }

        /// <summary>
        /// Saves the specified properties.
        /// </summary>
        /// <param name="properties">
        /// The properties.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="rootpath">
        /// The rootpath.
        /// </param>
        /// <returns>
        /// true if success;else false
        /// </returns>
        public static bool Save(List<PListValue> properties, string name, string rootpath)
        {
            var path = ListPath(name, rootpath);
            Save(path, w => ComposeArray(properties, w));
            return true;
        }

        /// <summary>
        /// Composes the array.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="writer">
        /// The writer.
        /// </param>
        private static void ComposeArray(List<PListValue> value, XmlWriter writer)
        {
            writer.WriteStartElement("array");
            foreach (var obj in value)
            {
                ComposeValue(obj, writer);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Composes the dictionary.
        /// </summary>
        /// <param name="list">
        /// The list.
        /// </param>
        /// <param name="writer">
        /// The writer.
        /// </param>
        private static void ComposeDictionary(Dictionary<string, PListValue> list, XmlWriter writer)
        {
            if (list == null)
            {
                return;
            }

            writer.WriteStartElement("dict");
            foreach (var item in list)
            {
                writer.WriteElementString("key", item.Key);
                ComposeValue(item.Value, writer);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Composes the value.
        /// </summary>
        /// <param name="itemValue">
        /// The item value.
        /// </param>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// Unsupported
        /// </exception>
        private static void ComposeValue(PListValue itemValue, XmlWriter writer)
        {
            switch (itemValue.PropertyType)
            {
                case "string":
                case "integer":
                case "real":
                    writer.WriteElementString(itemValue.PropertyType, itemValue.Value.ToString());
                    break;
                case "true":
                case "false":
                    writer.WriteElementString(itemValue.PropertyType, string.Empty);
                    break;
                case "dict":
                    ComposeDictionary(itemValue.Value, writer);
                    break;
                case "array":
                    ComposeArray(itemValue.Value as List<PListValue>, writer);
                    break;
                default:
                    throw new ArgumentException("Unsupported");
            }
        }

        /// <summary>
        /// Lists the path.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="rootPath">
        /// The root path.
        /// </param>
        /// <returns>
        /// the plist path
        /// </returns>
        private static string ListPath(string name, string rootPath)
        {
            string path;
            if (rootPath != null)
            {
                path = rootPath;
            }
            else
            {
                var session = ServerSession.CurrentSession;
                path = session.CrmAccount.AccountPath;
            }

            return Path.Combine(path, $"{name}.plist");
        }

        /// <summary>
        /// Loads the specified file.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <param name="rootNodeName">
        /// Name of the root node.
        /// </param>
        /// <returns>
        /// the loaded contents as either a pList or an array of plist values
        /// </returns>
        private static dynamic Load(string file, string rootNodeName = ListRootType.Dictionary)
        {
            var doc = XDocument.Load(file);
            var plist = doc.Element("plist");

            var root = plist?.Element(rootNodeName);
            if (root == null)
            {
                return null;
            }

            var rootElements = root.Elements();

            if (rootNodeName == ListRootType.Dictionary)
            {
                var newList = new PList();
                Parse(newList, rootElements);
                return newList;
            }
            else if (rootNodeName == ListRootType.Array)
            {
                return ParseArray(rootElements);
            }

            return null;
        }

        /// <summary>
        /// Parses the specified dictionary.
        /// </summary>
        /// <param name="dict">
        /// The dictionary.
        /// </param>
        /// <param name="elements">
        /// The elements.
        /// </param>
        private static void Parse(PList dict, IEnumerable<XElement> elements)
        {
            var xElements = elements as XElement[] ?? elements.ToArray();
            for (var i = 0; i < xElements.Count(); i += 2)
            {
                var key = xElements.ElementAt(i);
                var val = xElements.ElementAt(i + 1);

                dict[key.Value] = ParseValue(val);
            }
        }

        /// <summary>
        /// Parses the array.
        /// </summary>
        /// <param name="elements">
        /// The elements.
        /// </param>
        /// <returns>
        /// an array of plist values
        /// </returns>
        private static List<PListValue> ParseArray(IEnumerable<XElement> elements)
        {
            return elements.Select(ParseValue).ToList();
        }

        /// <summary>
        /// Parses the value.
        /// </summary>
        /// <param name="val">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="PListValue"/>.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// Unsupported
        /// </exception>
        private static PListValue ParseValue(XElement val)
        {
            var itemValue = new PListValue { PropertyType = val.Name.ToString() };
            switch (itemValue.PropertyType)
            {
                case "string":
                    itemValue.Value = val.Value;
                    break;
                case "integer":
                    itemValue.Value = int.Parse(val.Value);
                    break;
                case "real":
                    itemValue.Value = float.Parse(val.Value);
                    break;
                case "true":
                    itemValue.Value = true;
                    break;
                case "false":
                    itemValue.Value = false;
                    break;
                case "dict":
                    var plist = new PList();
                    Parse(plist, val.Elements());
                    itemValue.Value = plist;
                    break;
                case "array":
                    var list = ParseArray(val.Elements());
                    itemValue.Value = list;
                    break;
                default:
                    throw new ArgumentException("Unsupported");
            }

            return itemValue;
        }

        /// <summary>
        /// Saves the specified file.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <param name="compose">
        /// The compose action.
        /// </param>
        private static void Save(string file, Action<XmlWriter> compose)
        {
            using (var ms = new MemoryStream())
            {
                var xmlWriterSettings = new XmlWriterSettings
                                            {
                                                Encoding = new System.Text.UTF8Encoding(false),
                                                ConformanceLevel = ConformanceLevel.Document,
                                                Indent = true
                                            };

                using (var xmlWriter = XmlWriter.Create(ms, xmlWriterSettings))
                {
                    xmlWriter.WriteStartDocument();

                    // xmlWriter.WriteComment("DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" " + "\"http://www.apple.com/DTDs/PropertyList-1.0.dtd\"");
                    xmlWriter.WriteDocType(
                        "plist",
                        "-//Apple Computer//DTD PLIST 1.0//EN",
                        "http://www.apple.com/DTDs/PropertyList-1.0.dtd",
                        null);
                    xmlWriter.WriteStartElement("plist");
                    xmlWriter.WriteAttributeString("version", "1.0");

                    // Perform the compose action
                    compose?.Invoke(xmlWriter);

                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndDocument();
                    xmlWriter.Flush();

                    // reset the memory stream position
                    ms.Position = 0;

                    // save the file
                    SimpleIoc.Default.GetInstance<IPlatformService>().StorageProvider.SaveFile(file, ms.ToArray());
                }
            }
        }
    }
}
