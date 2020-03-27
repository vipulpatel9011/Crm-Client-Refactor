// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IQuestionnaireAnswerOption.cs" company="Aurea Software Gmbh">
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
//   The Questionnaire Answer Option
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Questionnaire
{
    /// <summary>
    /// Questionnaire Answer Option
    /// </summary>
    public interface IQuestionnaireAnswerOption
    {
        /// <summary>
        /// Gets the answer identifier.
        /// </summary>
        /// <value>
        /// The answer identifier.
        /// </value>
        string AnswerId { get; }

        /// <summary>
        /// Gets the answer text.
        /// </summary>
        /// <value>
        /// The answer text.
        /// </value>
        string AnswerText { get; }

        /// <summary>
        /// Gets the next question identifier.
        /// </summary>
        /// <value>
        /// The next question identifier.
        /// </value>
        string NextQuestionId { get; }

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        string RecordIdentification { get; }
    }
}
