// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPCatalogQuestionnaireAnswerOption.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Max Menezes
// </author>
// <summary>
//   The Catalog Questionnaire Answer Option
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Questionnaire
{
    /// <summary>
    /// Catalog Questionnaire Answer Option
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Questionnaire.IQuestionnaireAnswerOption" />
    public class UPCatalogQuestionnaireAnswerOption : IQuestionnaireAnswerOption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPCatalogQuestionnaireAnswerOption"/> class.
        /// </summary>
        /// <param name="catalogValue">The catalog value.</param>
        /// <param name="catalogValueKey">The catalog value key.</param>
        public UPCatalogQuestionnaireAnswerOption(string catalogValue, string catalogValueKey)
        {
            this.CatalogValueText = catalogValue;
            this.CatalogValueKey = catalogValueKey;
        }

        /// <summary>
        /// Gets the catalog value text.
        /// </summary>
        /// <value>
        /// The catalog value text.
        /// </value>
        public string CatalogValueText { get; }

        /// <summary>
        /// Gets the catalog value key.
        /// </summary>
        /// <value>
        /// The catalog value key.
        /// </value>
        public string CatalogValueKey { get; }

        /// <summary>
        /// Gets the answer identifier.
        /// </summary>
        /// <value>
        /// The answer identifier.
        /// </value>
        public string AnswerId => this.CatalogValueKey;

        /// <summary>
        /// Gets the answer text.
        /// </summary>
        /// <value>
        /// The answer text.
        /// </value>
        public string AnswerText => this.CatalogValueText;

        /// <summary>
        /// Gets the next question identifier.
        /// </summary>
        /// <value>
        /// The next question identifier.
        /// </value>
        public string NextQuestionId => null;

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification => null;

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"{this.CatalogValueText} ({this.CatalogValueKey})";
        }
    }
}
