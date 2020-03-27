// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSerializer.cs" company="Aurea Software Gmbh">
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
//   Defines the custom serializable interface
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Utilities
{
    using System;
    using System.Collections.Generic;

    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Defines the custom serializable interface
    /// </summary>
    public interface UPSerializable
    {
        /// <summary>
        /// Serializes the specified writer.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        void Serialize(UPSerializer writer);
    }

    /// <summary>
    /// Implements custom serializer
    /// </summary>
    public class UPSerializer
    {
        /// <summary>
        /// The properties
        /// </summary>
        private Dictionary<string, object> properties;

        /// <summary>
        /// Objects for key.
        /// </summary>
        /// <param name="obj">
        /// The object.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        public virtual void ObjectForKey(object obj, string key)
        {
            if (this.properties == null)
            {
                this.properties = new Dictionary<string, object> { { key, obj } };
            }
            else
            {
                this.properties.SetObjectForKey(obj, key);
            }
        }

        /// <summary>
        /// Values the or default.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public virtual object ValueOrDefault(string key)
        {
            return this.properties?.ValueOrDefault(key);
        }

        /// <summary>
        /// Writes the attribute integer value.
        /// </summary>
        /// <param name="attributeName">
        /// Name of the attribute.
        /// </param>
        /// <param name="attributeValue">
        /// The attribute value.
        /// </param>
        public virtual void WriteAttributeIntegerValue(string attributeName, int attributeValue)
        {
            this.WriteAttributeValue(attributeName, $"{attributeValue}");
        }

        /// <summary>
        /// Writes the attributes.
        /// </summary>
        /// <param name="attributes">
        /// The attributes.
        /// </param>
        public virtual void WriteAttributes(Dictionary<string, string> attributes)
        {
            foreach (var key in attributes.Keys)
            {
                this.WriteAttributeValue(key, attributes.ValueOrDefault(key));
            }
        }

        /// <summary>
        /// Writes the attribute value.
        /// </summary>
        /// <param name="attributeName">
        /// Name of the attribute.
        /// </param>
        /// <param name="attributeValue">
        /// The attribute value.
        /// </param>
        public virtual void WriteAttributeValue(string attributeName, string attributeValue)
        {
        }

        /// <summary>
        /// Writes the element end.
        /// </summary>
        public virtual void WriteElementEnd()
        {
        }

        /// <summary>
        /// Writes the element Start.
        /// </summary>
        /// <param name="elementName">
        /// Name of the element.
        /// </param>
        public virtual void WriteElementStart(string elementName)
        {
        }

        /// <summary>
        /// Writes the element string value.
        /// </summary>
        /// <param name="stringValue">
        /// The string value.
        /// </param>
        public virtual void WriteElementStringValue(string stringValue)
        {
        }

        /// <summary>
        /// Writes the element value.
        /// </summary>
        /// <param name="elementName">
        /// Name of the element.
        /// </param>
        /// <param name="elementValue">
        /// The element value.
        /// </param>
        public virtual void WriteElementValue(string elementName, string elementValue)
        {
            this.WriteElementValueAttributes(elementName, elementValue, null);
        }

        /// <summary>
        /// Writes the element value attributes.
        /// </summary>
        /// <param name="elementName">
        /// Name of the element.
        /// </param>
        /// <param name="elementValue">
        /// The element value.
        /// </param>
        /// <param name="attributes">
        /// The attributes.
        /// </param>
        public virtual void WriteElementValueAttributes(
            string elementName,
            string elementValue,
            Dictionary<string, string> attributes)
        {
            this.WriteElementStart(elementName);
            if (attributes?.Count > 0)
            {
                this.WriteAttributes(attributes);
            }

            if (!string.IsNullOrEmpty(elementValue))
            {
                this.WriteElementStringValue(elementValue);
            }

            this.WriteElementEnd();
        }

        /// <summary>
        /// Writes the locale settings element.
        /// </summary>
        /// <param name="elementName">
        /// Name of the element.
        /// </param>
        public virtual void WriteLocaleSettingsElement(string elementName)
        {
            this.WriteElementStart(elementName);
            this.WriteElementValue("DateFormat", "YYYY-MM-DD");
            this.WriteElementValue("DateFormatShort", "d");
            this.WriteElementValue("TimeFormat", "hh:mm");
            this.WriteElementValue("TimeFormatShort", "hh:mm");
            this.WriteElementValue("Timezone", TimeZoneInfo.Local.DisplayName);
            this.WriteElementValue("ThousandSeparator", ",");
            this.WriteElementValue("DecimalSeparator", ".");
            this.WriteElementEnd();
        }

        /// <summary>
        /// Writes the locale settings element.
        /// </summary>
        public void WriteLocaleSettingsElement()
        {
            this.WriteLocaleSettingsElement("LocaleSettings");
            this.WriteElementValue("DateToday", StringExtensions.CrmValueFromDate(DateTime.UtcNow));
        }

        /// <summary>
        /// Writes the object.
        /// </summary>
        /// <param name="obj">
        /// The object.
        /// </param>
        public virtual void WriteObject(UPSerializable obj)
        {
            obj.Serialize(this);
        }

        /// <summary>
        /// Writes the string.
        /// </summary>
        /// <param name="theString">
        /// The string.
        /// </param>
        public virtual void WriteString(string theString)
        {
        }
    }

    /// <summary>
    /// Custom Xml serializer
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Utilities.UPSerializer" />
    public class UPXmlWriter : UPSerializer
    {
        /// <summary>
        /// The depth.
        /// </summary>
        private int depth;

        /// <summary>
        /// The element closed.
        /// </summary>
        private bool elementClosed;

        /// <summary>
        /// The open elements.
        /// </summary>
        private readonly List<string> openElements;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPXmlWriter"/> class.
        /// </summary>
        public UPXmlWriter()
        {
            this.openElements = new List<string>();
            this.depth = 0;
            this.elementClosed = true;
        }

        /// <summary>
        /// Closes the Start element.
        /// </summary>
        public void CloseStartElement()
        {
            if (this.elementClosed)
            {
                return;
            }

            this.elementClosed = true;
            this.WriteString(">");
        }

        /// <summary>
        /// Encodes the string.
        /// </summary>
        /// <param name="theString">
        /// The string.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string EncodeString(string theString)
        {
            var encodedString = theString.Replace("&", @"&amp;amp;");
            encodedString = encodedString.Replace("\"", @"&amp;quot;");
            encodedString = encodedString.Replace("'", @"&amp;#39;");
            encodedString = encodedString.Replace(">", @"&amp;gt;");
            encodedString = encodedString.Replace("<", @"&amp;lt;");

            return encodedString;
        }

        /// <summary>
        /// Writes the attribute value.
        /// </summary>
        /// <param name="attributeName">
        /// Name of the attribute.
        /// </param>
        /// <param name="attributeValue">
        /// The attribute value.
        /// </param>
        public override void WriteAttributeValue(string attributeName, string attributeValue)
        {
            if (this.elementClosed)
            {
                return;
            }

            this.WriteString($" {attributeName}=\"{this.EncodeString(attributeValue)}\"");
        }

        /// <summary>
        /// Writes the element end.
        /// </summary>
        public override void WriteElementEnd()
        {
            if (this.depth <= 0)
            {
                return;
            }

            if (!this.elementClosed)
            {
                this.WriteString(" />");
                this.elementClosed = true;
                --this.depth;
            }
            else
            {
                this.WriteString($"</{this.openElements[--this.depth]}>");
            }
        }

        /// <summary>
        /// Writes the element Start.
        /// </summary>
        /// <param name="elementName">
        /// Name of the element.
        /// </param>
        public override void WriteElementStart(string elementName)
        {
            this.CloseStartElement();
            if (this.depth < this.openElements.Count)
            {
                this.openElements[this.depth] = elementName;
            }
            else
            {
                this.openElements.Add(elementName);
            }

            ++this.depth;
            this.elementClosed = false;
            this.WriteString($"<{elementName}");
        }

        /// <summary>
        /// Writes the element string value.
        /// </summary>
        /// <param name="stringValue">
        /// The string value.
        /// </param>
        public override void WriteElementStringValue(string stringValue)
        {
            this.CloseStartElement();
            this.WriteStringEncoded(stringValue);
        }

        /// <summary>
        /// Writes the string encoded.
        /// </summary>
        /// <param name="theString">
        /// The string.
        /// </param>
        public void WriteStringEncoded(string theString)
        {
            var encodedString = this.EncodeString(theString);
            this.WriteString(encodedString);
        }
    }

    /// <summary>
    /// Custom Xml memory serializer
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Utilities.UPXmlWriter" />
    public class UPXmlMemoryWriter : UPXmlWriter
    {
        /// <summary>
        /// The string array.
        /// </summary>
        private readonly List<string> stringArray;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPXmlMemoryWriter"/> class.
        /// </summary>
        public UPXmlMemoryWriter()
        {
            this.stringArray = new List<string>();
        }

        /// <summary>
        /// Writes the string.
        /// </summary>
        /// <param name="theString">
        /// The string.
        /// </param>
        public override void WriteString(string theString)
        {
            this.stringArray.Add(theString);
        }

        /// <summary>
        /// XMLs the content string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string XmlContentString()
        {
            return string.Join(string.Empty, this.stringArray);
        }

        /// <summary>
        /// XMLs the content string amp decoded.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string XmlContentStringAmpDecoded()
        {
            var xmlContentString = this.XmlContentString();
            xmlContentString = xmlContentString.Replace("&amp;amp;", "&amp;");
            xmlContentString = xmlContentString.Replace("&amp;quot;", "&quot;");
            xmlContentString = xmlContentString.Replace("&amp;#39;", "&#39;");
            xmlContentString = xmlContentString.Replace("&amp;gt;", "&gt;");
            xmlContentString = xmlContentString.Replace("&amp;lt;", "&lt;");
            return xmlContentString;
        }
    }
}
