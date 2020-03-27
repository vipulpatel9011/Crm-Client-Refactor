// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextGroup.cs" company="Aurea Software Gmbh">
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
//   Defines the config text group confiurations
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Configuration
{
    using System.Collections.Generic;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Defines the config text group confiurations
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Configuration.ConfigUnit" />
    public class TextGroup : ConfigUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextGroup"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        public TextGroup(IReadOnlyList<object> definition)
        {
            if (definition == null || definition.Count < 2)
            {
                return;
            }

            this.UnitName = (string)definition[0];
            this.Texts = (definition[1] as JArray)?.ToObject<List<string>>();
        }

        /// <summary>
        /// Gets the number of texts.
        /// </summary>
        /// <value>
        /// The number of texts.
        /// </value>
        public int NumberOfTexts => this.Texts?.Count ?? 0;

        /// <summary>
        /// Gets the texts.
        /// </summary>
        /// <value>
        /// The texts.
        /// </value>
        public List<string> Texts { get; private set; }

        /// <summary>
        /// Texts at index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string TextAtIndex(int index)
        {
            return this.TextAtIndexDefaultText(index, null);
        }

        /// <summary>
        /// Texts at index default text.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="defaultText">
        /// The default text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string TextAtIndexDefaultText(int index, string defaultText)
        {
            if (this.Texts.Count <= index)
            {
                return defaultText;
            }

            var text = this.Texts[index];
            return string.IsNullOrWhiteSpace(text) ? defaultText : text;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"<{this.GetType()} {this.GetType()} count: {this.NumberOfTexts} texts: {this.Texts}>";
        }
    }
}
