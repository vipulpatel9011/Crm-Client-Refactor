// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSurveyAnswer.cs" company="Aurea Software Gmbh">
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
//   The Survey Answer
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Questionnaire
{
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Survey Answer
    /// </summary>
    public class UPSurveyAnswer
    {
        private string defaultAnswer;
        private Dictionary<string, UPSurveyAnswerSingle> singleAnswerDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSurveyAnswer"/> class.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="data">The data.</param>
        /// <param name="question">The question.</param>
        /// <param name="root">The root.</param>
        public UPSurveyAnswer(string recordIdentification, Dictionary<string, object> data, UPQuestionnaireQuestion question, UPSurvey root)
        {
            this.RecordIdentification = recordIdentification;
            this.Question = question;
            this.Root = root;
            if (this.Question.Multiple)
            {
                this.singleAnswerDictionary = new Dictionary<string, UPSurveyAnswerSingle>();
                this.AddAnswer(data, recordIdentification);
            }

            this.AnswerText = data[Constants.QuestionnaireAnswer] as string;
            if (this.Question.AnswersFromCatalog && this.Question.Questionnaire.Manager.SaveCatalogValuesAsText)
            {
                this.AnswerText = this.Question.AnswerIdForText(this.AnswerText) ?? string.Empty;
            }

            this.AnswerNumber = data[Constants.QuestionnaireAnswerNumber] as string;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSurveyAnswer"/> class.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <param name="root">The root.</param>
        public UPSurveyAnswer(UPQuestionnaireQuestion question, UPSurvey root)
            : this(null, null, question, root)
        {
        }

        /// <summary>
        /// Gets or sets the answer.
        /// </summary>
        /// <value>
        /// The answer.
        /// </value>
        public string Answer
        {
            get
            {
                return this.Question.HasAnswerOptions ? this.AnswerNumber : this.AnswerText;
            }

            set
            {
                if ((value != this.Answer) && (value != null || this.Answer != null))
                {
                    if (this.Question.HasAnswerOptions)
                    {
                        this.AnswerNumber = value;
                    }
                    else
                    {
                        this.AnswerText = value;
                    }

                    this.Changed = true;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="UPSurveyAnswer"/> is multiple.
        /// </summary>
        /// <value>
        ///   <c>true</c> if multiple; otherwise, <c>false</c>.
        /// </value>
        public bool Multiple => this.Question.Multiple;

        /// <summary>
        /// Gets or sets the survey foreign field.
        /// </summary>
        /// <value>
        /// The survey foreign field.
        /// </value>
        public UPSurveyForeignField SurveyForeignField { get; set; }

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get; }

        /// <summary>
        /// Gets the root.
        /// </summary>
        /// <value>
        /// The root.
        /// </value>
        public UPSurvey Root { get; }

        /// <summary>
        /// Gets the question.
        /// </summary>
        /// <value>
        /// The question.
        /// </value>
        public UPQuestionnaireQuestion Question { get; }

        /// <summary>
        /// Gets the answer text.
        /// </summary>
        /// <value>
        /// The answer text.
        /// </value>
        public string AnswerText { get; private set; }

        /// <summary>
        /// Gets the answer number.
        /// </summary>
        /// <value>
        /// The answer number.
        /// </value>
        public string AnswerNumber { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="UPSurveyAnswer"/> is changed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if changed; otherwise, <c>false</c>.
        /// </value>
        public bool Changed { get; private set; }

        /// <summary>
        /// Gets or sets the default answer.
        /// </summary>
        /// <value>
        /// The default answer.
        /// </value>
        public string DefaultAnswer
        {
            get
            {
                return this.defaultAnswer;
            }

            set
            {
                this.Answer = value;
                this.defaultAnswer = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is default answer.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is default answer; otherwise, <c>false</c>.
        /// </value>
        public bool IsDefaultAnswer => this.Question.HasAnswerOptions ? this.AnswerNumber == this.defaultAnswer : this.AnswerText == this.defaultAnswer;

        /// <summary>
        /// Gets the answer ids.
        /// </summary>
        /// <value>
        /// The answer ids.
        /// </value>
        public List<string> AnswerIds
        {
            get
            {
                if (this.Question.Multiple)
                {
                    List<string> answerIds = null;
                    foreach (UPSurveyAnswerSingle single in this.singleAnswerDictionary.Values)
                    {
                        if (!single.Deleted)
                        {
                            if (answerIds == null)
                            {
                                answerIds = new List<string> { single.AnswerId };
                            }
                            else
                            {
                                answerIds.Add(single.AnswerId);
                            }
                        }
                    }

                    return answerIds;
                }

                string answer = this.Answer;
                return !string.IsNullOrEmpty(answer) ? new List<string> { answer } : null;
            }
        }

        /// <summary>
        /// Gets the next question.
        /// </summary>
        /// <value>
        /// The next question.
        /// </value>
        public UPQuestionnaireQuestion NextQuestion => this.Answer != null
            ? this.Question.NextQuestionWithAnswer(this.Answer) : this.Question.NextMandatoryQuestion;

        /// <summary>
        /// Sets the answer array.
        /// </summary>
        /// <param name="answers">The answers.</param>
        public void SetAnswerArray(List<string> answers)
        {
            foreach (UPSurveyAnswerSingle singleAnswer in this.singleAnswerDictionary.Values)
            {
                if (!singleAnswer.Deleted)
                {
                    singleAnswer.CheckDelete = true;
                }
            }

            foreach (string currentAnswer in answers)
            {
                UPSurveyAnswerSingle singleAnswer = this.singleAnswerDictionary.ValueOrDefault(currentAnswer);
                if (singleAnswer != null)
                {
                    if (singleAnswer.Deleted)
                    {
                        singleAnswer.Deleted = false;
                        this.Changed = true;
                    }
                    else
                    {
                        singleAnswer.CheckDelete = false;
                    }
                }
                else
                {
                    this.singleAnswerDictionary[currentAnswer] = new UPSurveyAnswerSingle(currentAnswer);
                    this.Changed = true;
                }
            }

            foreach (UPSurveyAnswerSingle singleAnswer in this.singleAnswerDictionary.Values)
            {
                if (singleAnswer.CheckDelete)
                {
                    singleAnswer.CheckDelete = false;
                    singleAnswer.Deleted = true;
                    this.Changed = true;
                }
            }
        }

        /// <summary>
        /// Adds the answer data.
        /// </summary>
        /// <param name="answerData">The answer data.</param>
        /// <param name="recordIdentification">The record identification.</param>
        public void AddAnswer(Dictionary<string, object> answerData, string recordIdentification)
        {
            string answerId;
            if (this.Question.HasAnswerOptions)
            {
                answerId = answerData.ValueOrDefault(Constants.QuestionnaireAnswerNumber) as string;
            }
            else
            {
                answerId = answerData.ValueOrDefault(Constants.QuestionnaireAnswer) as string;
                if (this.Question.AnswersFromCatalog && this.Question.Questionnaire.Manager.SaveCatalogValuesAsText)
                {
                    answerId = this.Question.AnswerIdForText(answerId);
                    if (answerId == null)
                    {
                        return;
                    }
                }
            }

            if (answerId != null)
            {
                UPSurveyAnswerSingle singleAnswer = new UPSurveyAnswerSingle(answerId, recordIdentification);
                this.singleAnswerDictionary[answerId] = singleAnswer;
            }
        }

        /// <summary>
        /// Singles the answer changed records.
        /// </summary>
        /// <param name="rootRecord">The root record.</param>
        /// <param name="data">The data.</param>
        /// <param name="ignoreExisting">if set to <c>true</c> [ignore existing].</param>
        /// <param name="ignoreDefault">if set to <c>true</c> [ignore default].</param>
        /// <returns></returns>
        public List<UPCRMRecord> SingleAnswerChangedRecords(UPCRMRecord rootRecord, Dictionary<string, object> data, bool ignoreExisting, bool ignoreDefault)
        {
            if (this.Changed)
            {
                UPCRMRecord record;
                string infoAreaId = this.Root.SurveyAnswerSearchAndList.InfoAreaId;
                bool existsRecord = false;
                if (!ignoreExisting && !string.IsNullOrEmpty(this.RecordIdentification))
                {
                    record = new UPCRMRecord(this.RecordIdentification);
                    existsRecord = true;
                }
                else if (!ignoreDefault || !this.IsDefaultAnswer)
                {
                    record = new UPCRMRecord(infoAreaId);
                    record.AddLink(new UPCRMLink(rootRecord, -1));
                    if (this.Root.AnswerQuestionNumberField != null)
                    {
                        record.AddValue(new UPCRMFieldValue(this.Question.QuestionId, infoAreaId, this.Root.AnswerQuestionNumberField.FieldId));
                    }

                    if (this.Question.Questionnaire.Manager.LinkAnswerToQuestionnaire)
                    {
                        record.AddLink(new UPCRMLink(this.Question.Questionnaire.RecordIdentification));
                    }

                    if (this.Question.Questionnaire.Manager.LinkAnswerToQuestion)
                    {
                        record.AddLink(new UPCRMLink(this.Question.RecordIdentification));
                    }
                }
                else
                {
                    return null;
                }

                if (this.Root.SurveyAnswerTemplateFilter != null)
                {
                    UPConfigFilter filter = this.Root.SurveyAnswerTemplateFilter.FilterByApplyingValueDictionaryDefaults(data, true);
                    record.ApplyValuesFromTemplateFilter(filter);
                }

                if (this.Root.AnswerAnswerNumberField != null)
                {
                    record.AddValue(new UPCRMFieldValue(this.AnswerNumber, infoAreaId, this.Root.AnswerAnswerNumberField.FieldId));
                }

                if (this.Root.AnswerAnswerTextField != null)
                {
                    string text = this.AnswerText;
                    if (this.Question.AnswersFromCatalog && this.Question.Questionnaire.Manager.SaveCatalogValuesAsText)
                    {
                        text = this.Question.TextForAnswerId(this.AnswerText) ?? string.Empty;
                    }

                    record.AddValue(new UPCRMFieldValue(text, infoAreaId, this.Root.AnswerAnswerTextField.FieldId));
                }

                if (this.Question.Questionnaire.Manager.LinkAnswerToQuestionAnswer && this.Question.HasAnswerOptions)
                {
                    IQuestionnaireAnswerOption answerOption = this.Question.AnswerForId(this.AnswerNumber);
                    if (answerOption != null)
                    {
                        if (existsRecord && this.Question.Questionnaire.Manager.DeleteAndInsertOnAnswerOptionChange)
                        {
                            UPCRMRecord deleteRecord = new UPCRMRecord(this.RecordIdentification, "Delete");
                            List<UPCRMRecord> createdRecords = this.SingleAnswerChangedRecords(rootRecord, data, true, ignoreDefault);
                            List<UPCRMRecord> changedRecords = new List<UPCRMRecord> { deleteRecord };
                            changedRecords.AddRange(createdRecords);
                            return changedRecords;
                        }

                        record.AddLink(new UPCRMLink(answerOption.RecordIdentification));
                    }
                    else if (this.Question.Questionnaire.Manager.DeleteSingleAnswerOptionIfEmpty)
                    {
                        return existsRecord ? new List<UPCRMRecord> { new UPCRMRecord(record.RecordIdentification, "Delete") } : null;
                    }
                }

                return new List<UPCRMRecord> { record };
            }

            return null;
        }

        /// <summary>
        /// Changeds the records.
        /// </summary>
        /// <param name="rootRecord">The root record.</param>
        /// <param name="data">The data.</param>
        /// <param name="ignoreDefault">if set to <c>true</c> [ignore default].</param>
        /// <returns></returns>
        public List<UPCRMRecord> ChangedRecords(UPCRMRecord rootRecord, Dictionary<string, object> data, bool ignoreDefault)
        {
            if (!this.Multiple)
            {
                return this.SingleAnswerChangedRecords(rootRecord, data, false, ignoreDefault);
            }

            if (!this.Changed)
            {
                return null;
            }

            string infoAreaId = this.Root.SurveyAnswerSearchAndList.InfoAreaId;
            List<string> recordsToDelete = new List<string>();
            foreach (UPSurveyAnswerSingle singleRecord in this.singleAnswerDictionary.Values)
            {
                if (singleRecord.Deleted && !string.IsNullOrEmpty(singleRecord.RecordIdentification))
                {
                    recordsToDelete.Add(singleRecord.RecordIdentification);
                }
            }

            List<UPCRMRecord> changedRecords = new List<UPCRMRecord>();
            foreach (UPSurveyAnswerSingle singleRecord in this.singleAnswerDictionary.Values)
            {
                if (!singleRecord.Deleted && string.IsNullOrEmpty(singleRecord.RecordIdentification))
                {
                    UPCRMRecord record;
                    if (recordsToDelete.Count > 0)
                    {
                        string reUseRecordIdentification = recordsToDelete[0];
                        recordsToDelete.RemoveAt(0);
                        record = new UPCRMRecord(reUseRecordIdentification);
                    }
                    else
                    {
                        record = new UPCRMRecord(infoAreaId);
                        record.AddLink(new UPCRMLink(rootRecord, -1));
                        if (this.Root.AnswerQuestionNumberField != null)
                        {
                            record.AddValue(new UPCRMFieldValue(this.Question.QuestionId, infoAreaId, this.Root.AnswerQuestionNumberField.FieldId));
                        }
                    }

                    if (this.Root.SurveyAnswerTemplateFilter != null)
                    {
                        UPConfigFilter filter = this.Root.SurveyAnswerTemplateFilter.FilterByApplyingValueDictionaryDefaults(data, true);
                        record.ApplyValuesFromTemplateFilter(filter);
                    }

                    if (!string.IsNullOrEmpty(this.Question.RecordIdentification) && this.Question.Questionnaire.Manager.LinkAnswerToQuestion)
                    {
                        record.AddLink(new UPCRMLink(this.Question.RecordIdentification));
                    }

                    if (this.Question.HasAnswerOptions)
                    {
                        if (this.Root.AnswerAnswerNumberField != null)
                        {
                            record.AddValue(new UPCRMFieldValue(singleRecord.AnswerId, infoAreaId, this.Root.AnswerAnswerNumberField.FieldId));
                        }

                        if (this.Question.Questionnaire.Manager.LinkAnswerToQuestionAnswer && this.Question.HasAnswerOptions)
                        {
                            IQuestionnaireAnswerOption answerOption = this.Question.AnswerForId(singleRecord.AnswerId);
                            if (answerOption != null)
                            {
                                record.AddLink(new UPCRMLink(answerOption.RecordIdentification));
                            }
                        }
                    }
                    else
                    {
                        if (this.Root.AnswerAnswerTextField != null)
                        {
                            string text = singleRecord.AnswerId;
                            if (this.Question.AnswersFromCatalog && this.Question.Questionnaire.Manager.SaveCatalogValuesAsText)
                            {
                                text = this.Question.TextForAnswerId(this.AnswerText) ?? string.Empty;
                            }

                            record.AddValue(new UPCRMFieldValue(text, infoAreaId, this.Root.AnswerAnswerTextField.FieldId));
                        }
                    }

                    changedRecords.Add(record);
                }
            }

            changedRecords.AddRange(recordsToDelete.Select(deleteRecordId => new UPCRMRecord(deleteRecordId, "Delete")));
            return changedRecords;
        }
    }
}
