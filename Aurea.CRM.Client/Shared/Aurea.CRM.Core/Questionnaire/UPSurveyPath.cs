// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSurveyPath.cs" company="Aurea Software Gmbh">
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
//   The Survey Path
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Questionnaire
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Survey Path
    /// </summary>
    public class UPSurveyPath
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSurveyPath"/> class.
        /// </summary>
        /// <param name="survey">The survey.</param>
        public UPSurveyPath(UPSurvey survey)
        {
            this.Survey = survey;
            this.Questionnaire = this.Survey.Questionnaire;
            this.Build();
        }

        /// <summary>
        /// Gets the current question group.
        /// </summary>
        /// <value>
        /// The current question group.
        /// </value>
        public UPQuestionnaireQuestionGroup CurrentQuestionGroup => this.CurrentQuestion.QuestionnaireGroup;

        /// <summary>
        /// Gets the survey.
        /// </summary>
        /// <value>
        /// The survey.
        /// </value>
        public UPSurvey Survey { get; }

        /// <summary>
        /// Gets the questionnaire.
        /// </summary>
        /// <value>
        /// The questionnaire.
        /// </value>
        public UPQuestionnaire Questionnaire { get; }

        /// <summary>
        /// Gets the visible questionnaire group array.
        /// </summary>
        /// <value>
        /// The visible questionnaire group array.
        /// </value>
        public List<UPQuestionnaireQuestionGroup> VisibleQuestionnaireGroupArray { get; private set; }

        /// <summary>
        /// Gets the answer array.
        /// </summary>
        /// <value>
        /// The answer array.
        /// </value>
        public List<UPSurveyAnswer> AnswerArray { get; private set; }

        /// <summary>
        /// Gets or sets the current question.
        /// </summary>
        /// <value>
        /// The current question.
        /// </value>
        public UPQuestionnaireQuestion CurrentQuestion { get; set; }

        /// <summary>
        /// Rebuilds this instance.
        /// </summary>
        /// <returns></returns>
        public bool Rebuild()
        {
            List<UPQuestionnaireQuestionGroup> questionnaireGroups = new List<UPQuestionnaireQuestionGroup>();
            List<UPSurveyAnswer> answerArray = new List<UPSurveyAnswer>();
            UPQuestionnaireQuestion currentQuestion = this.Questionnaire.Questions[0];

            while (currentQuestion != null)
            {
                if (currentQuestion.Hide)
                {
                    currentQuestion = currentQuestion.NextQuestion;
                }
                else
                {
                    UPSurveyAnswer currentAnswer = this.Survey.AnswerDictionary.ValueOrDefault(currentQuestion.QuestionId);
                    if (currentAnswer == null)
                    {
                        return false;   // should never happen
                    }

                    answerArray.Add(currentAnswer);
                    if (!questionnaireGroups.Contains(currentQuestion.QuestionnaireGroup))
                    {
                        questionnaireGroups.Add(currentQuestion.QuestionnaireGroup);
                    }

                    currentQuestion = currentAnswer.NextQuestion;
                }
            }

            this.VisibleQuestionnaireGroupArray = questionnaireGroups;
            this.AnswerArray = answerArray;
            return true;
        }

        /// <summary>
        /// Visibles the questions for group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <returns></returns>
        public List<UPQuestionnaireQuestion> VisibleQuestionsForGroup(UPQuestionnaireQuestionGroup group)
        {
            List<UPQuestionnaireQuestion> questions = new List<UPQuestionnaireQuestion>();
            foreach (UPSurveyAnswer answer in this.AnswerArray)
            {
                if (answer.Question.QuestionnaireGroup == group)
                {
                    questions.Add(answer.Question);
                }
            }

            return questions;
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        private void Build()
        {
            if (this.Questionnaire == null || this.Questionnaire.Questions.Count == 0)
            {
                return; // empty questionnaire
            }

            this.Rebuild();
            this.CurrentQuestion = this.Questionnaire.Questions[0];
        }
    }
}
