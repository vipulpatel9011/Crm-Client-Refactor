// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPQuestionnaireQuestion.cs" company="Aurea Software Gmbh">
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
//   The Questionnaire Question
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Questionnaire
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.Catalogs;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// The Questionnaire Question
    /// </summary>
    public class UPQuestionnaireQuestion
    {
        private List<IQuestionnaireAnswerOption> answers;
        private Dictionary<string, IQuestionnaireAnswerOption> answerDictionary;
        private bool answersInitialized;
        private bool multiple;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPQuestionnaireQuestion"/> class.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="data">The data.</param>
        /// <param name="questionnaire">The questionnaire.</param>
        public UPQuestionnaireQuestion(string recordIdentification, Dictionary<string, object> data, UPQuestionnaire questionnaire)
        {
            this.Label = data.ValueOrDefault(Constants.QuestionnaireLabel) as string;
            this.QuestionId = data.ValueOrDefault(Constants.QuestionnaireQuestionNumber) as string;
            this.QuestionSortKey = Convert.ToInt32(this.QuestionId).ToString("D8");
            this.InfoAreaId = data.ValueOrDefault(Constants.QuestionnaireInfoAreaId) as string;
            this.NextQuestionId = data.ValueOrDefault(Constants.QuestionnaireFollowUpNumber) as string;
            int nextQuestionIntId = Convert.ToInt32(this.NextQuestionId);
            if (nextQuestionIntId > 0)
            {
                int questionIntId = Convert.ToInt32(this.QuestionId);
                if (nextQuestionIntId <= questionIntId)
                {
                    this.NextQuestionId = string.Empty;
                }
            }

            this.Mandatory = data.ValueOrDefault(Constants.QuestionnaireMandatory) as string == "true";
            this.Read = data.ValueOrDefault(Constants.QuestionnaireRead) as string == "true";
            this.Save = data.ValueOrDefault(Constants.QuestionnaireSave) as string == "true";
            this.multiple = data.ValueOrDefault(Constants.QuestionnaireMultiple) as string == "true";
            this.AdditionalInfo = data.ValueOrDefault(Constants.QuestionnaireAdditionalInfo) as string;
            this.StartsNewGroup = data.ValueOrDefault(Constants.QuestionnaireNewSection) as string == "true";
            this.DefaultAnswer = data.ValueOrDefault(Constants.QuestionnaireDefault) as string;
            this.Hide = data.ValueOrDefault(Constants.QuestionnaireHide) as string == "true";

            string fieldIdString = data.ValueOrDefault(Constants.QuestionnaireFieldId) as string;
            if (!string.IsNullOrEmpty(fieldIdString))
            {
                this.FieldId = Convert.ToInt32(fieldIdString);
                if (this.FieldId >= 0)
                {
                    ICRMDataStore dataStore = UPCRMDataStore.DefaultStore;
                    this.Field = new UPCRMField(this.FieldId, this.InfoAreaId);
                    this.FieldInfo = dataStore.FieldInfoForField(this.Field);
                }
                else
                {
                    this.FieldId = -1;
                }
            }
            else
            {
                this.FieldId = -1;
            }

            this.RecordIdentification = recordIdentification;
            this.Questionnaire = questionnaire;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPQuestionnaireQuestion"/> class.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="questionnaire">The questionnaire.</param>
        public UPQuestionnaireQuestion(UPCRMResultRow row, UPQuestionnaire questionnaire)
            : this(row.RootRecordIdentification, row.ValuesWithFunctions(), questionnaire)
        {
        }

        /// <summary>
        /// Gets the answers.
        /// </summary>
        /// <value>
        /// The answers.
        /// </value>
        public List<IQuestionnaireAnswerOption> Answers()
        {
            if (!this.answersInitialized && this.answers == null)
            {
                if (!string.IsNullOrEmpty(this.InfoAreaId) && this.FieldId >= 0)
                {
                    if (this.FieldInfo.IsCatalogField)
                    {
                        UPCatalog catalog = UPCRMDataStore.DefaultStore.CatalogForCrmField(this.Field);
                        if (!catalog.IsDependent)
                        {
                            Dictionary<string, string> values = catalog.TextValuesForFieldValues(false);
                            if (values.Count > 0)
                            {
                                this.answers = UPQuestionnaireAnswer.AnswersFromCatalogValueDictionary(values);
                            }

                            if (this.multiple && !this.Questionnaire.Manager.MultipleAnswersForCatalogs)
                            {
                                this.multiple = false;
                            }
                        }
                    }
                }

                this.answersInitialized = true;
                if (this.answers?.Count > 0)
                {
                    this.answerDictionary = new Dictionary<string, IQuestionnaireAnswerOption>();
                    foreach (IQuestionnaireAnswerOption answerOption in this.answers)
                    {
                        if (answerOption.AnswerId != null)
                        {
                            this.answerDictionary[answerOption.AnswerId] = answerOption;
                        }
                    }
                }
                else
                {
                    this.multiple = false;
                }
            }

            return this.answers;
        }

        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        /// <value>
        /// The type of the field.
        /// </value>
        public string FieldType
        {
            get
            {
                if (this.FieldInfo != null)
                {
                    return this.FieldInfo.FieldType;
                }

                return this.answers.Count > 0 ? "K" : "C";
            }
        }

        /// <summary>
        /// Gets a value indicating whether [answers from catalog].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [answers from catalog]; otherwise, <c>false</c>.
        /// </value>
        public bool AnswersFromCatalog => this.FieldInfo.IsCatalogField;

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label { get; }

        /// <summary>
        /// Gets the questionnaire.
        /// </summary>
        /// <value>
        /// The questionnaire.
        /// </value>
        public UPQuestionnaire Questionnaire { get; }

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
        /// Gets or sets the questionnaire group.
        /// </summary>
        /// <value>
        /// The questionnaire group.
        /// </value>
        public UPQuestionnaireQuestionGroup QuestionnaireGroup { get; set; }

        /// <summary>
        /// Gets the next question identifier.
        /// </summary>
        /// <value>
        /// The next question identifier.
        /// </value>
        public string NextQuestionId { get; }

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; }

        /// <summary>
        /// Gets the field identifier.
        /// </summary>
        /// <value>
        /// The field identifier.
        /// </value>
        public int FieldId { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="UPQuestionnaireQuestion"/> is mandatory.
        /// </summary>
        /// <value>
        ///   <c>true</c> if mandatory; otherwise, <c>false</c>.
        /// </value>
        public bool Mandatory { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="UPQuestionnaireQuestion"/> is read.
        /// </summary>
        /// <value>
        ///   <c>true</c> if read; otherwise, <c>false</c>.
        /// </value>
        public bool Read { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="UPQuestionnaireQuestion"/> is save.
        /// </summary>
        /// <value>
        ///   <c>true</c> if save; otherwise, <c>false</c>.
        /// </value>
        public bool Save { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="UPQuestionnaireQuestion"/> is multiple.
        /// </summary>
        /// <value>
        ///   <c>true</c> if multiple; otherwise, <c>false</c>.
        /// </value>
        public bool Multiple
        {
            get
            {
                if (!this.answersInitialized && this.answers == null)
                {
                    this.Answers();
                }

                return this.multiple;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [starts new group].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [starts new group]; otherwise, <c>false</c>.
        /// </value>
        public bool StartsNewGroup { get; private set; }

        /// <summary>
        /// Gets the default answer.
        /// </summary>
        /// <value>
        /// The default answer.
        /// </value>
        public string DefaultAnswer { get; private set; }

        /// <summary>
        /// Gets the field information.
        /// </summary>
        /// <value>
        /// The field information.
        /// </value>
        public UPCRMFieldInfo FieldInfo { get; }

        /// <summary>
        /// Gets the field.
        /// </summary>
        /// <value>
        /// The field.
        /// </value>
        public UPCRMField Field { get; }

        /// <summary>
        /// Gets or sets the index of the question.
        /// </summary>
        /// <value>
        /// The index of the question.
        /// </value>
        public int QuestionIndex { get; set; }

        /// <summary>
        /// Gets the answer dictionary.
        /// </summary>
        /// <value>
        /// The answer dictionary.
        /// </value>
        public Dictionary<string, IQuestionnaireAnswerOption> AnswerDictionary
        {
            get
            {
                if (!this.answersInitialized && this.answers == null)
                {
                    this.Answers();
                }

                return this.answerDictionary;
            }
        }

        /// <summary>
        /// Gets the question sort key.
        /// </summary>
        /// <value>
        /// The question sort key.
        /// </value>
        public string QuestionSortKey { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UPQuestionnaireQuestion"/> is hide.
        /// </summary>
        /// <value>
        ///   <c>true</c> if hide; otherwise, <c>false</c>.
        /// </value>
        public bool Hide { get; set; }

        /// <summary>
        /// Gets the additional information.
        /// </summary>
        /// <value>
        /// The additional information.
        /// </value>
        public string AdditionalInfo { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [no answer jump possible].
        /// </summary>
        /// <value>
        /// <c>true</c> if [no answer jump possible]; otherwise, <c>false</c>.
        /// </value>
        public bool NoAnswerJumpPossible => this.answers != null ? this.answers.Count < 1 || !(this.answers[0] is UPQuestionnaireAnswer) : false;

        /// <summary>
        /// Gets the next question.
        /// </summary>
        /// <value>
        /// The next question.
        /// </value>
        public UPQuestionnaireQuestion NextQuestion
        {
            get
            {
                if (IsEmptyQuestionId(this.NextQuestionId))
                {
                    return this.QuestionIndex + 1 < this.Questionnaire.Questions.Count ? this.Questionnaire.Questions[this.QuestionIndex + 1] : null;
                }

                UPQuestionnaireQuestion nextQuestion = this.Questionnaire.QuestionForId(this.NextQuestionId);
                return nextQuestion.QuestionIndex <= this.QuestionIndex ? null : nextQuestion;
            }
        }

        /// <summary>
        /// Gets the next possible question.
        /// </summary>
        /// <returns></returns>
        public UPQuestionnaireQuestion NextPossibleQuestion
        {
            get
            {
                if (this.NoAnswerJumpPossible)
                {
                    return this.NextQuestion;
                }

                UPQuestionnaireQuestion nextQuestion = null;
                foreach (IQuestionnaireAnswerOption answer in this.answers)
                {
                    UPQuestionnaireQuestion currentNextQuestion;
                    if (!IsEmptyQuestionId(answer.NextQuestionId))
                    {
                        currentNextQuestion = this.Questionnaire.QuestionForId(answer.NextQuestionId);
                    }
                    else if (IsEmptyQuestionId(this.NextQuestionId))
                    {
                        return this.NextQuestion;
                    }
                    else
                    {
                        currentNextQuestion = this.Questionnaire.QuestionForId(this.NextQuestionId);
                    }

                    if (currentNextQuestion == null)
                    {
                        continue;
                    }

                    if (nextQuestion == null || currentNextQuestion.QuestionIndex < nextQuestion.QuestionIndex)
                    {
                        nextQuestion = currentNextQuestion;
                    }
                }

                return nextQuestion;
            }
        }

        /// <summary>
        /// Gets the next mandatory question.
        /// </summary>
        /// <returns></returns>
        public UPQuestionnaireQuestion NextMandatoryQuestion
        {
            get
            {
                if (this.NoAnswerJumpPossible)
                {
                    return this.NextQuestion;
                }

                if (this.QuestionIndex + 1 == this.Questionnaire.Questions.Count)
                {
                    return null;
                }

                UPQuestionnaireQuestion nextQuestion = this.NextPossibleQuestion;
                while (nextQuestion != null)
                {
                    UPQuestionnaireQuestion foundQuestion = this.EarliestNextQuestionFromQuestion(nextQuestion);
                    if (foundQuestion != nextQuestion)
                    {
                        nextQuestion = foundQuestion;
                        continue;
                    }

                    break;
                }

                return nextQuestion;
            }
        }

        /// <summary>
        /// Determines whether [has answer options].
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has answer options; otherwise, <c>false</c>.
        /// </value>
        public bool HasAnswerOptions
        {
            get
            {
                return this.answerDictionary.Values.Select(answerOption => answerOption is UPQuestionnaireAnswer).FirstOrDefault();
            }
        }

        /// <summary>
        /// Nexts the question with answer.
        /// </summary>
        /// <param name="answer">The answer.</param>
        /// <returns></returns>
        public UPQuestionnaireQuestion NextQuestionWithAnswer(string answer)
        {
            UPQuestionnaireAnswer answerOption = this.answerDictionary.ValueOrDefault(answer) as UPQuestionnaireAnswer;
            if (answerOption != null)
            {
                string nextQuestionId = answerOption.NextQuestionId;
                return IsEmptyQuestionId(nextQuestionId) ? this.NextQuestion : this.Questionnaire.QuestionForId(nextQuestionId);
            }

            return this.NextQuestion;
        }

        /// <summary>
        /// Texts for answer identifier.
        /// </summary>
        /// <param name="answerId">The answer identifier.</param>
        /// <returns></returns>
        public string TextForAnswerId(string answerId)
        {
            return this.AnswerDictionary[answerId].AnswerText;
        }

        /// <summary>
        /// Answers for identifier.
        /// </summary>
        /// <param name="answerId">The answer identifier.</param>
        /// <returns></returns>
        public IQuestionnaireAnswerOption AnswerForId(string answerId)
        {
            return this.AnswerDictionary[answerId];
        }

        /// <summary>
        /// Answers the identifier for text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public string AnswerIdForText(string text)
        {
            return this.Answers().FirstOrDefault(x => x.AnswerText == text)?.AnswerId;
        }

        /// <summary>
        /// Adds the answer.
        /// </summary>
        /// <param name="answer">The answer.</param>
        public void AddAnswer(UPQuestionnaireAnswer answer)
        {
            if (this.answers == null)
            {
                this.answers = new List<IQuestionnaireAnswerOption> { answer };
            }
            else
            {
                this.answers.Add(answer);
            }

            if (answer.AnswerId != null)
            {
                if (this.answerDictionary == null)
                {
                    this.answerDictionary = new Dictionary<string, IQuestionnaireAnswerOption> { { answer.AnswerId, answer } };
                }
                else
                {
                    this.answerDictionary[answer.AnswerId] = answer;
                }
            }
        }

        /// <summary>
        /// Determines whether [is empty question identifier] [the specified question identifier].
        /// </summary>
        /// <param name="questionId">The question identifier.</param>
        /// <returns>
        ///   <c>true</c> if [is empty question identifier] [the specified question identifier]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEmptyQuestionId(string questionId)
        {
            return string.IsNullOrEmpty(questionId) || questionId == "0";
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            StringBuilder desc = new StringBuilder();
            desc.AppendFormat("\n{0}({1}): {2}", this.RecordIdentification, this.QuestionId, this.Label);
            if (this.answers != null)
            {
                desc.AppendFormat("\nAnswers={0}", this.answers);
            }
            else if (!string.IsNullOrEmpty(this.InfoAreaId))
            {
                desc.AppendFormat("\nInfoAreaId={0}, FieldId={1}", this.InfoAreaId, this.FieldId);
            }

            desc.AppendFormat("mand={0}, next={1}", this.Mandatory, this.NextQuestionId);

            return desc.ToString();
        }

        private UPQuestionnaireQuestion EarliestNextQuestionFromQuestion(UPQuestionnaireQuestion testQuestion)
        {
            bool checkQuestionNextQuestion = false;
            UPQuestionnaireQuestion nextQuestion = testQuestion;
            if (!this.NoAnswerJumpPossible)
            {
                foreach (IQuestionnaireAnswerOption answer in this.answers)
                {
                    if (!IsEmptyQuestionId(answer.NextQuestionId))
                    {
                        UPQuestionnaireQuestion answerNextQuestion = this.Questionnaire.QuestionForId(answer.NextQuestionId);
                        while (answerNextQuestion != null && answerNextQuestion.QuestionIndex < nextQuestion.QuestionIndex)
                        {
                            answerNextQuestion = answerNextQuestion.NextMandatoryQuestion;
                        }

                        if (answerNextQuestion == null)
                        {
                            return null;
                        }

                        if (answerNextQuestion.QuestionIndex > nextQuestion.QuestionIndex)
                        {
                            nextQuestion = answerNextQuestion;
                        }
                    }
                    else
                    {
                        checkQuestionNextQuestion = true;
                    }
                }
            }
            else
            {
                checkQuestionNextQuestion = true;
            }

            if (checkQuestionNextQuestion)
            {
                UPQuestionnaireQuestion answerNextQuestion = this.NextQuestion;
                while (answerNextQuestion != null && answerNextQuestion.QuestionIndex < nextQuestion.QuestionIndex)
                {
                    answerNextQuestion = answerNextQuestion.EarliestNextQuestionFromQuestion(nextQuestion);
                }

                if (answerNextQuestion == null)
                {
                    return null;
                }

                if (answerNextQuestion.QuestionIndex > nextQuestion.QuestionIndex)
                {
                    nextQuestion = answerNextQuestion;
                }
            }

            return nextQuestion;
        }
    }
}
