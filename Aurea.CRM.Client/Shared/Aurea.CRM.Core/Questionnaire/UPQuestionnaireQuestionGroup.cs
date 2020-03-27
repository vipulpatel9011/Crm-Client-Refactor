// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPQuestionnaireQuestionGroup.cs" company="Aurea Software Gmbh">
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
//   The Questionnaire Question Group
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Questionnaire
{
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;

    /// <summary>
    /// The Questionnaire Question Group
    /// </summary>
    public class UPQuestionnaireQuestionGroup
    {
        private readonly List<UPQuestionnaireQuestion> questions;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPQuestionnaireQuestionGroup"/> class.
        /// </summary>
        /// <param name="question">The question.</param>
        public UPQuestionnaireQuestionGroup(UPQuestionnaireQuestion question)
        {
            this.questions = new List<UPQuestionnaireQuestion>();
            this.Questionnaire = question.Questionnaire;
            this.Label = question.Label;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPQuestionnaireQuestionGroup"/> class.
        /// </summary>
        /// <param name="questionnaire">The questionnaire.</param>
        public UPQuestionnaireQuestionGroup(UPQuestionnaire questionnaire)
        {
            this.questions = new List<UPQuestionnaireQuestion>();
            this.Questionnaire = questionnaire;
            this.Label = LocalizedString.TextProcessSurvey;
        }

        /// <summary>
        /// Gets the questions.
        /// </summary>
        /// <value>
        /// The questions.
        /// </value>
        public List<UPQuestionnaireQuestion> Questions => this.questions;

        /// <summary>
        /// Gets the first question.
        /// </summary>
        /// <value>
        /// The first question.
        /// </value>
        public UPQuestionnaireQuestion FirstQuestion => this.questions.FirstOrDefault();

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label { get; private set; }

        /// <summary>
        /// Gets the questionnaire.
        /// </summary>
        /// <value>
        /// The questionnaire.
        /// </value>
        public UPQuestionnaire Questionnaire { get; private set; }

        /// <summary>
        /// Adds the question.
        /// </summary>
        /// <param name="question">The question.</param>
        public void AddQuestion(UPQuestionnaireQuestion question)
        {
            this.questions.Add(question);
        }
    }
}
