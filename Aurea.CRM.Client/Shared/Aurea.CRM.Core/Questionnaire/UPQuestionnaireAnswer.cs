// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPQuestionnaireAnswer.cs" company="Aurea Software Gmbh">
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
//   The Questionnaire Answer
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Questionnaire
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Questionnaire Answer
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.Delegates.IQuestionnaireAnswerOption" />
    public class UPQuestionnaireAnswer : IQuestionnaireAnswerOption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPQuestionnaireAnswer"/> class.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="dict">The dictionary.</param>
        public UPQuestionnaireAnswer(string recordIdentification, Dictionary<string, object> dict)
        {
            this.RecordIdentification = recordIdentification;
            this.QuestionId = dict.ValueOrDefault(Constants.QuestionnaireQuestionNumber) as string;
            this.NextQuestionId = dict.ValueOrDefault(Constants.QuestionnaireFollowUpNumber) as string;
            int nextQuestionIntId = 0;
            int.TryParse(this.NextQuestionId, out nextQuestionIntId);
            if (nextQuestionIntId > 0)
            {
                int questionIntId = Convert.ToInt32(this.QuestionId);
                if (nextQuestionIntId <= questionIntId)
                {
                    this.NextQuestionId = string.Empty;
                }
            }

            this.Answer = dict.ValueOrDefault(Constants.QuestionnaireLabel) as string;
            this.AnswerId = dict.ValueOrDefault(Constants.QuestionnaireAnswerNumber) as string;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPQuestionnaireAnswer"/> class.
        /// </summary>
        /// <param name="row">The row.</param>
        public UPQuestionnaireAnswer(UPCRMResultRow row)
            : this(row.RootRecordIdentification, row.ValuesWithFunctions())
        {
        }

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get; }

        /// <summary>
        /// Gets the question identifier.
        /// </summary>
        /// <value>
        /// The question identifier.
        /// </value>
        public string QuestionId { get; }

        /// <summary>
        /// Gets the next question identifier.
        /// </summary>
        /// <value>
        /// The next question identifier.
        /// </value>
        public string NextQuestionId { get; }

        /// <summary>
        /// Gets the answer.
        /// </summary>
        /// <value>
        /// The answer.
        /// </value>
        public string Answer { get; }

        /// <summary>
        /// Gets the answer identifier.
        /// </summary>
        /// <value>
        /// The answer identifier.
        /// </value>
        public string AnswerId { get; }

        /// <summary>
        /// Gets the answer text.
        /// </summary>
        /// <value>
        /// The answer text.
        /// </value>
        public string AnswerText => this.Answer;

        /// <summary>
        /// Answers from catalog value dictionary.
        /// </summary>
        /// <param name="catalogValueDictionary">The catalog value dictionary.</param>
        /// <returns></returns>
        public static List<IQuestionnaireAnswerOption> AnswersFromCatalogValueDictionary(Dictionary<string, string> catalogValueDictionary)
        {
            if (catalogValueDictionary.Count == 0)
            {
                return null;
            }

            List<IQuestionnaireAnswerOption> answerArray = new List<IQuestionnaireAnswerOption>(catalogValueDictionary.Count);
            answerArray.AddRange(catalogValueDictionary.Select(dictEntry => new UPCatalogQuestionnaireAnswerOption(dictEntry.Value, dictEntry.Key)));

            return answerArray;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return Convert.ToInt32(this.NextQuestionId) > 0
                ? $" A{this.RecordIdentification}({this.QuestionId}/{this.AnswerId}): {this.Answer} -> next:{this.NextQuestionId}"
                : $" A{this.RecordIdentification}({this.QuestionId}/{this.AnswerId}): {this.Answer}";
        }
    }
}
