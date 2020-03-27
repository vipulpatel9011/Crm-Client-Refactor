// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPQuestionnaire.cs" company="Aurea Software Gmbh">
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
//   The Questionnaire
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Questionnaire
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.OperationHandling;

    /// <summary>
    /// UPQuestionnaire
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.ISearchOperationHandler" />
    public class UPQuestionnaire : ISearchOperationHandler
    {
        private int loadStep;
        private UPContainerMetaInfo currentQuery;
        private UPRequestOption requestOption;
        private Dictionary<string, object> filterParameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPQuestionnaire"/> class.
        /// </summary>
        /// <param name="catalogCode">The catalog code.</param>
        /// <param name="manager">The manager.</param>
        public UPQuestionnaire(int catalogCode, UPQuestionnaireManager manager)
        {
            this.CatalogCode = catalogCode;
            this.Manager = manager;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPQuestionnaire"/> class.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="manager">The manager.</param>
        public UPQuestionnaire(string recordIdentification, UPQuestionnaireManager manager)
        {
            this.RecordIdentification = recordIdentification;
            this.Manager = manager;
        }

        /// <summary>
        /// Gets the manager.
        /// </summary>
        /// <value>
        /// The manager.
        /// </value>
        public UPQuestionnaireManager Manager { get; }

        /// <summary>
        /// Gets the load delegate.
        /// </summary>
        /// <value>
        /// The load delegate.
        /// </value>
        public IQuestionnaireDelegate LoadDelegate { get; private set; }

        /// <summary>
        /// Gets the catalog code.
        /// </summary>
        /// <value>
        /// The catalog code.
        /// </value>
        public int CatalogCode { get; private set; }

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get; private set; }

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label { get; private set; }

        /// <summary>
        /// Gets the questions.
        /// </summary>
        /// <value>
        /// The questions.
        /// </value>
        public List<UPQuestionnaireQuestion> Questions { get; private set; }

        /// <summary>
        /// Gets the questions by identifier.
        /// </summary>
        /// <value>
        /// The questions by identifier.
        /// </value>
        public Dictionary<string, UPQuestionnaireQuestion> QuestionsById { get; private set; }

        /// <summary>
        /// Gets the question groups.
        /// </summary>
        /// <value>
        /// The question groups.
        /// </value>
        public List<UPQuestionnaireQuestionGroup> QuestionGroups { get; private set; }

        /// <summary>
        /// Gets the data dictionary.
        /// </summary>
        /// <value>
        /// The data dictionary.
        /// </value>
        public Dictionary<string, object> DataDictionary { get; private set; }

        /// <summary>
        /// Gets the questions by record identification.
        /// </summary>
        /// <value>
        /// The questions by record identification.
        /// </value>
        public Dictionary<string, UPQuestionnaireQuestion> QuestionsByRecordIdentification { get; private set; }

        /// <summary>
        /// Gets the questions group by identifier.
        /// </summary>
        /// <value>
        /// The questions group by identifier.
        /// </value>
        public Dictionary<string, UPQuestionnaireQuestionGroup> QuestionsGroupById { get; private set; }

        /// <summary>
        /// Gets the foreign field questions.
        /// </summary>
        /// <value>
        /// The foreign field questions.
        /// </value>
        public List<UPQuestionnaireQuestion> ForeignFieldQuestions { get; private set; }

        /// <summary>
        /// Loads the specified the delegate.
        /// </summary>
        /// <param name="theDelegate">The delegate.</param>
        /// <returns></returns>
        public bool Load(IQuestionnaireDelegate theDelegate)
        {
            if (this.LoadDelegate != null)
            {
                return false;
            }

            this.LoadDelegate = theDelegate;
            this.currentQuery = new UPContainerMetaInfo(this.Manager.QuestionnaireList);
            if (!string.IsNullOrEmpty(this.RecordIdentification))
            {
                this.currentQuery.SetLinkRecordIdentification(this.RecordIdentification);
            }
            else
            {
                UPConfigFilter filter = ConfigurationUnitStore.DefaultStore.FilterByName(this.Manager.QuestionnaireSearchAndList.FilterName);
                filter = filter.FilterByApplyingValueDictionary(
                    new Dictionary<string, object> { { Constants.QuestionnaireQuestionnaireID, this.CatalogCode.ToString() } });

                if (filter != null)
                {
                    this.currentQuery.ApplyFilter(filter);
                }
            }

            this.requestOption = this.Manager.QuestionnaireRequestOption;
            this.loadStep = 0;
            this.currentQuery.Find(this.requestOption, this);
            return true;
        }

        /// <summary>
        /// Questions for identifier.
        /// </summary>
        /// <param name="questionId">The question identifier.</param>
        /// <returns></returns>
        public UPQuestionnaireQuestion QuestionForId(string questionId)
        {
            return this.QuestionsById.ValueOrDefault(questionId) ?? this.QuestionsGroupById[questionId].FirstQuestion;
        }

        /// <summary>
        /// Signals the error.
        /// </summary>
        /// <param name="error">The error.</param>
        public void SignalError(Exception error)
        {
            IQuestionnaireDelegate theDelegate = this.LoadDelegate;
            this.LoadDelegate = null;
            this.Manager.QuestionnaireLoadedError(this, error);
            theDelegate.QuestionnaireDidFailWithError(this, error);
        }

        /// <summary>
        /// Searches the operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="result">The result.</param>
        public void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            switch (this.loadStep)
            {
                case 0:
                    this.QuestionnaireFromResult(result);
                    break;

                case 1:
                    this.QuestionsFromResult(result);
                    break;

                case 2:
                    this.AnswersFromResult(result);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.currentQuery = null;
            this.SignalError(error);
        }

        /// <summary>
        /// Searches the operation did finish with results.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="results">The results.</param>
        public void SearchOperationDidFinishWithResults(Operation operation, List<UPCRMResult> results)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches the operation did finish with count.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="count">The count.</param>
        public void SearchOperationDidFinishWithCount(Operation operation, int count)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches the operation did finish with counts.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="counts">The counts.</param>
        public void SearchOperationDidFinishWithCounts(Operation operation, List<int?> counts)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        private void QuestionnaireFromResult(UPCRMResult result)
        {
            if (result.RowCount == 0)
            {
                this.SignalError(new Exception("questionnaire not found"));
                return;
            }

            UPCRMResultRow row = (UPCRMResultRow)result.ResultRowAtIndex(0);
            this.RecordIdentification = row.RootRecordIdentification;
            this.DataDictionary = row.ValuesWithFunctions();
            this.Label = this.DataDictionary.ValueOrDefault(Constants.QuestionnaireLabel) as string;

            int catCode = 0;
            int.TryParse(this.DataDictionary.ValueOrDefault(Constants.QuestionnaireQuestionnaireID) as string, out catCode);
            this.CatalogCode = catCode;

            if (string.IsNullOrEmpty(this.Label) && this.CatalogCode > 0)
            {
                UPConfigFieldControlField field = this.Manager.QuestionnaireList.FieldWithFunction(Constants.QuestionnaireQuestionnaireID);
                if (field != null)
                {
                    this.Label = field.Field.ValueForRawValueOptions(this.DataDictionary[Constants.QuestionnaireQuestionnaireID] as string, 0);
                }
            }

            this.currentQuery = new UPContainerMetaInfo(this.Manager.QuestionSearchAndList, this.filterParameters);
            this.currentQuery.SetLinkRecordIdentification(this.RecordIdentification);
            this.loadStep = 1;
            if (this.requestOption == UPRequestOption.BestAvailable || this.requestOption == UPRequestOption.FastestAvailable)
            {
                this.requestOption = row.IsServerResponse ? UPRequestOption.Online : UPRequestOption.Offline;
            }

            this.currentQuery.Find(this.requestOption, this);
        }

        private void QuestionsFromResult(UPCRMResult result)
        {
            int count = result.RowCount;
            List<UPQuestionnaireQuestion> questions = new List<UPQuestionnaireQuestion>(count);
            Dictionary<string, UPQuestionnaireQuestion> questionDictionary = new Dictionary<string, UPQuestionnaireQuestion>(count);
            Dictionary<string, UPQuestionnaireQuestionGroup> questionGroupDictionary = new Dictionary<string, UPQuestionnaireQuestionGroup>();
            Dictionary<string, UPQuestionnaireQuestion> questionRecordIdentificationDictionary = new Dictionary<string, UPQuestionnaireQuestion>(count);
            List<UPQuestionnaireQuestionGroup> questionGroupArray = new List<UPQuestionnaireQuestionGroup>();
            UPQuestionnaireQuestionGroup questionGroup = null;
            List<UPQuestionnaireQuestion> questionArray = new List<UPQuestionnaireQuestion>(count);

            for (int i = 0; i < count; i++)
            {
                UPQuestionnaireQuestion question = new UPQuestionnaireQuestion((UPCRMResultRow)result.ResultRowAtIndex(i), this);
                questionArray.Add(question);
            }

            questionArray = questionArray.OrderBy(x => x.QuestionSortKey).ToList();

            List<UPQuestionnaireQuestion> foreignFieldQuestions = null;
            foreach (UPQuestionnaireQuestion question in questionArray)
            {
                if (question.StartsNewGroup)
                {
                    questionGroup = new UPQuestionnaireQuestionGroup(question);
                    questionGroupArray.Add(questionGroup);
                    questionGroupDictionary[question.QuestionId] = questionGroup;
                    question.Hide = true;
                    question.QuestionnaireGroup = questionGroup;
                    continue;
                }

                if (questionGroup == null)
                {
                    questionGroup = new UPQuestionnaireQuestionGroup(this);
                    questionGroupArray.Add(questionGroup);
                    questionGroupDictionary[question.QuestionId] = questionGroup;
                }

                questionGroup.AddQuestion(question);
                question.QuestionnaireGroup = questionGroup;
                if (!string.IsNullOrEmpty(question.QuestionId))
                {
                    question.QuestionIndex = questions.Count;
                    if (!questionDictionary.ContainsKey(question.QuestionId))
                    {
                        questions.Add(question);
                        questionDictionary[question.QuestionId] = question;
                        if (!string.IsNullOrEmpty(question.RecordIdentification))
                        {
                            questionRecordIdentificationDictionary[question.RecordIdentification] = question;
                        }

                        if (question.FieldInfo != null && (question.Read || question.Save))
                        {
                            if (foreignFieldQuestions == null)
                            {
                                foreignFieldQuestions = new List<UPQuestionnaireQuestion> { question };
                            }
                            else
                            {
                                foreignFieldQuestions.Add(question);
                            }
                        }
                    }
                }
            }

            this.ForeignFieldQuestions = foreignFieldQuestions;
            this.Questions = questions;
            this.QuestionsById = questionDictionary;
            this.QuestionsGroupById = questionGroupDictionary;
            this.QuestionGroups = questionGroupArray;
            this.QuestionsByRecordIdentification = questionRecordIdentificationDictionary;
            this.currentQuery = new UPContainerMetaInfo(this.Manager.QuestionAnswersSearchAndList, this.filterParameters);
            this.currentQuery.SetLinkRecordIdentification(this.RecordIdentification);
            this.loadStep = 2;
            this.currentQuery.Find(this.requestOption, this);
        }

        /// <summary>
        /// Answerses from result.
        /// </summary>
        /// <param name="result">The result.</param>
        private void AnswersFromResult(UPCRMResult result)
        {
            int count = result.RowCount;
            for (int i = 0; i < count; i++)
            {
                UPQuestionnaireAnswer answer = new UPQuestionnaireAnswer((UPCRMResultRow)result.ResultRowAtIndex(i));

                UPQuestionnaireQuestion question = this.QuestionForId(answer.QuestionId);
                question.AddAnswer(answer);
            }

            IQuestionnaireDelegate theDelegate = this.LoadDelegate;
            this.LoadDelegate = null;
            this.Manager.QuestionnaireLoadedError(this, null);
            theDelegate.QuestionnaireDidFinishWithResult(this, this);
        }
    }
}
