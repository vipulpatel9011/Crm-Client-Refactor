// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSurveyForeignField.cs" company="Aurea Software Gmbh">
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
//   The Survey Foreign Field
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Questionnaire
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.DataModel;

    /// <summary>
    /// Survey Foreign Field
    /// </summary>
    public class UPSurveyForeignField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSurveyForeignField"/> class.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <exception cref="InvalidOperationException">FieldInfo is null</exception>
        public UPSurveyForeignField(UPQuestionnaireQuestion question)
        {
            this.FieldInfo = question.FieldInfo;
            if (this.FieldInfo == null)
            {
                throw new InvalidOperationException("FieldInfo is null");
            }

            this.QuestionArray = new List<UPQuestionnaireQuestion> { question };
            this.Key = $"{question.InfoAreaId}.{question.FieldId}";
            this.PositionInResult = -1;
        }

        /// <summary>
        /// Gets the question array.
        /// </summary>
        /// <value>
        /// The question array.
        /// </value>
        public List<UPQuestionnaireQuestion> QuestionArray { get; }

        /// <summary>
        /// Gets the field information.
        /// </summary>
        /// <value>
        /// The field information.
        /// </value>
        public UPCRMFieldInfo FieldInfo { get; }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Key { get; private set; }

        /// <summary>
        /// Gets or sets the position in result.
        /// </summary>
        /// <value>
        /// The position in result.
        /// </value>
        public int PositionInResult { get; set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; private set; }

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get; private set; }

        /// <summary>
        /// Adds the question.
        /// </summary>
        /// <param name="question">The question.</param>
        public void AddQuestion(UPQuestionnaireQuestion question)
        {
            this.QuestionArray.Add(question);
        }

        /// <summary>
        /// Sets the value record identification.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="recordIdentification">The record identification.</param>
        public void SetValueRecordIdentification(string value, string recordIdentification)
        {
            this.Value = value;
            this.RecordIdentification = recordIdentification;
        }
    }
}
