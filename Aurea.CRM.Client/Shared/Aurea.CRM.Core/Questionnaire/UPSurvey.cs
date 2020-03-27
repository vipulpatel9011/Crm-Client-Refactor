// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSurvey.cs" company="Aurea Software Gmbh">
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
//   UPSurvey
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Questionnaire
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Common;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.OfflineStorage;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.Core.OperationHandling.Data;
    using Aurea.CRM.Core.Session;

    /// <summary>
    /// UPSurvey
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Questionnaire.IQuestionnaireDelegate" />
    /// <seealso cref="Aurea.CRM.Core.Common.ISyncDataSetsDelegate" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.ISearchOperationHandler" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.UPCRMLinkReaderDelegate" />
    public class UPSurvey : ISearchOperationHandler, IQuestionnaireDelegate, UPCRMLinkReaderDelegate, ISyncDataSetsDelegate
    {
        private Dictionary<string, UPSurveyAnswer> surveyAnswers;
        private UPContainerMetaInfo crmQuery;
        private UPCRMLinkReader linkReader;
        private int loadStep;
        private UPContainerFieldMetaInfo questionnairelinkFieldMetaInfo;
        private UPSyncDataSets syncDataSets;
        private UPOfflineQuestionnaireRequest offlineRequest;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSurvey"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPSurvey(ViewReference viewReference, string recordIdentification, ISurveyDelegate theDelegate)
        {
            this.ViewReference = viewReference;
            this.RecordIdentification = recordIdentification;
            this.TheDelegate = theDelegate;
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            this.SurveySearchAndList = configStore.SearchAndListByName(this.ViewReference.ContextValueForKey("SurveySearchAndListName"));
            this.SurveyTemplateFilter = configStore.FilterByName(this.ViewReference.ContextValueForKey("SurveyTemplateFilter"));
            this.SurveyAnswerSearchAndList = configStore.SearchAndListByName(this.ViewReference.ContextValueForKey("SurveyAnswerSearchAndListName"));
            this.SurveyAnswerTemplateFilter = configStore.FilterByName(this.ViewReference.ContextValueForKey("SurveyAnswerTemplateFilter"));
            this.ParentLink = this.ViewReference.ContextValueForKey("ParentLink");
            this.SourceCopyFieldGroup = configStore.FieldControlByNameFromGroup("List", this.ViewReference.ContextValueForKey("SourceCopyFieldGroupName"));
            FieldControl fieldControl = configStore.FieldControlByNameFromGroup("Edit", this.SurveyAnswerSearchAndList.FieldGroupName) ??
                                        configStore.FieldControlByNameFromGroup("List", this.SurveyAnswerSearchAndList.FieldGroupName);

            foreach (FieldControlTab tab in fieldControl.Tabs)
            {
                foreach (UPConfigFieldControlField field in tab.Fields)
                {
                    if (field.Function == Constants.QuestionnaireAnswer)
                    {
                        this.AnswerAnswerTextField = field;
                    }
                    else if (field.Function == Constants.QuestionnaireAnswerNumber)
                    {
                        this.AnswerAnswerNumberField = field;
                    }
                    else if (field.Function == Constants.QuestionnaireQuestionNumber)
                    {
                        this.AnswerQuestionNumberField = field;
                    }
                }
            }

            this.QuestionnaireLinkName = this.ViewReference.ContextValueForKey("QuestionnaireLinkName");
            this.SourceRequestOption = UPCRMDataStore.RequestOptionFromString(this.ViewReference.ContextValueForKey("SourceRequestOption"), UPRequestOption.FastestAvailable);
            this.DestinationRequestOption = UPCRMDataStore.RequestOptionFromString(this.ViewReference.ContextValueForKey("DestinationRequestOption"), UPRequestOption.BestAvailable);
            this.surveyAnswers = new Dictionary<string, UPSurveyAnswer>();
        }

        /// <summary>
        /// Gets the answer dictionary.
        /// </summary>
        /// <value>
        /// The answer dictionary.
        /// </value>
        public Dictionary<string, UPSurveyAnswer> AnswerDictionary => this.surveyAnswers;

        /// <summary>
        /// Gets the current question.
        /// </summary>
        /// <value>
        /// The current question.
        /// </value>
        public UPQuestionnaireQuestion CurrentQuestion => this.SurveyPath.CurrentQuestion;

        /// <summary>
        /// Gets a value indicating whether this <see cref="UPSurvey"/> is readonly.
        /// </summary>
        /// <value>
        ///   <c>true</c> if readonly; otherwise, <c>false</c>.
        /// </value>
        public bool Readonly { get; private set; }

        /// <summary>
        /// Gets the answer root record identification.
        /// </summary>
        /// <value>
        /// The answer root record identification.
        /// </value>
        public string AnswerRootRecordIdentification { get; private set; }

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get; }

        /// <summary>
        /// Gets the root record identification.
        /// </summary>
        /// <value>
        /// The root record identification.
        /// </value>
        public string RootRecordIdentification { get; private set; }

        /// <summary>
        /// Gets the questionnaire.
        /// </summary>
        /// <value>
        /// The questionnaire.
        /// </value>
        public UPQuestionnaire Questionnaire { get; private set; }

        /// <summary>
        /// Gets the view reference.
        /// </summary>
        /// <value>
        /// The view reference.
        /// </value>
        public ViewReference ViewReference { get; }

        /// <summary>
        /// Gets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public ISurveyDelegate TheDelegate { get; }

        /// <summary>
        /// Gets the survey answer search and list.
        /// </summary>
        /// <value>
        /// The survey answer search and list.
        /// </value>
        public SearchAndList SurveyAnswerSearchAndList { get; }

        /// <summary>
        /// Gets the survey answer template filter.
        /// </summary>
        /// <value>
        /// The survey answer template filter.
        /// </value>
        public UPConfigFilter SurveyAnswerTemplateFilter { get; private set; }

        /// <summary>
        /// Gets the survey search and list.
        /// </summary>
        /// <value>
        /// The survey search and list.
        /// </value>
        public SearchAndList SurveySearchAndList { get; }

        /// <summary>
        /// Gets the survey template filter.
        /// </summary>
        /// <value>
        /// The survey template filter.
        /// </value>
        public UPConfigFilter SurveyTemplateFilter { get; }

        /// <summary>
        /// Gets the parent link.
        /// </summary>
        /// <value>
        /// The parent link.
        /// </value>
        public string ParentLink { get; }

        /// <summary>
        /// Gets the source copy field group.
        /// </summary>
        /// <value>
        /// The source copy field group.
        /// </value>
        public FieldControl SourceCopyFieldGroup { get; }

        /// <summary>
        /// Gets the name of the questionnaire link.
        /// </summary>
        /// <value>
        /// The name of the questionnaire link.
        /// </value>
        public string QuestionnaireLinkName { get; private set; }

        /// <summary>
        /// Gets the source request option.
        /// </summary>
        /// <value>
        /// The source request option.
        /// </value>
        public UPRequestOption SourceRequestOption { get; }

        /// <summary>
        /// Gets the destination request option.
        /// </summary>
        /// <value>
        /// The destination request option.
        /// </value>
        public UPRequestOption DestinationRequestOption { get; }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        public Dictionary<string, object> Parameters { get; private set; }

        /// <summary>
        /// Gets the survey path.
        /// </summary>
        /// <value>
        /// The survey path.
        /// </value>
        public UPSurveyPath SurveyPath { get; private set; }

        /// <summary>
        /// Gets the offline request.
        /// </summary>
        /// <value>
        /// The offline request.
        /// </value>
        public UPOfflineQuestionnaireRequest OfflineRequest
        {
            get
            {
                if (this.offlineRequest == null)
                {
                    this.offlineRequest = new UPOfflineQuestionnaireRequest(
                        this.DestinationRequestOption == UPRequestOption.Online ? UPOfflineRequestMode.OnlineOnly : UPOfflineRequestMode.OnlineConfirm,
                        this.ViewReference);
                    this.offlineRequest.RelatedInfoDictionary = this.Parameters;
                }

                return this.offlineRequest;
            }
        }

        /// <summary>
        /// Gets the answer question number field.
        /// </summary>
        /// <value>
        /// The answer question number field.
        /// </value>
        public UPConfigFieldControlField AnswerQuestionNumberField { get; private set; }

        /// <summary>
        /// Gets the answer answer number field.
        /// </summary>
        /// <value>
        /// The answer answer number field.
        /// </value>
        public UPConfigFieldControlField AnswerAnswerNumberField { get; private set; }

        /// <summary>
        /// Gets the answer answer text field.
        /// </summary>
        /// <value>
        /// The answer answer text field.
        /// </value>
        public UPConfigFieldControlField AnswerAnswerTextField { get; private set; }

        /// <summary>
        /// Gets the foreign fields.
        /// </summary>
        /// <value>
        /// The foreign fields.
        /// </value>
        public Dictionary<string, UPSurveyForeignField> ForeignFields { get; private set; }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        /// <returns></returns>
        public bool Load()
        {
            bool isRecordLink = false;
            if (string.IsNullOrEmpty(this.QuestionnaireLinkName))
            {
                this.QuestionnaireLinkName = "RecordLink";
            }

            this.loadStep = 1;
            this.crmQuery = this.SourceCopyFieldGroup != null
                ? new UPContainerMetaInfo(this.SourceCopyFieldGroup)
                : new UPContainerMetaInfo(new List<UPCRMField>(), this.RecordIdentification.InfoAreaId());

            if (this.QuestionnaireLinkName == "RecordLink")
            {
                isRecordLink = true;
                UPQuestionnaireManager questionnaireManager = null; //ServerSession.CurrentSession.QuestionnaireManager;
                if (this.SourceRequestOption == UPRequestOption.Offline ||
                    ((this.SourceRequestOption == UPRequestOption.FastestAvailable || this.SourceRequestOption == UPRequestOption.Default)
                        && UPCRMDataStore.DefaultStore.RecordExistsOffline(this.RecordIdentification)))
                {
                    UPCRMLinkField questionnairelinkField = new UPCRMLinkField(questionnaireManager.QuestionnaireList.InfoAreaId, -1, this.RecordIdentification.InfoAreaId());
                    List<UPContainerFieldMetaInfo> fieldMetaInfos = this.crmQuery.AddCrmFields(new List<UPCRMField> { questionnairelinkField });
                    if (fieldMetaInfos.Count > 0)
                    {
                        this.questionnairelinkFieldMetaInfo = fieldMetaInfos[0];
                    }
                }
                else
                {
                    UPCRMField questionnairelinkField = new UPCRMField(0, questionnaireManager?.QuestionnaireList.InfoAreaId, -1);
                    UPContainerInfoAreaMetaInfo subInfoAreaMetaInfo = this.crmQuery.RootInfoAreaMetaInfo.SubTableForInfoAreaIdLinkId(questionnaireManager?.QuestionnaireList.InfoAreaId, -1);
                    if (subInfoAreaMetaInfo == null)
                    {
                        this.crmQuery.RootInfoAreaMetaInfo.AddTable(new UPContainerInfoAreaMetaInfo(questionnaireManager?.QuestionnaireList.InfoAreaId, -1));
                    }

                    this.crmQuery.AddCrmFields(new List<UPCRMField> { questionnairelinkField });
                }
            }

            this.crmQuery.SetLinkRecordIdentification(this.RecordIdentification);
            if (isRecordLink && this.SourceRequestOption == UPRequestOption.BestAvailable)
            {
                this.loadStep = 11;
                if (this.crmQuery.Find(UPRequestOption.Online, this) == null)
                {
                    this.TrySourceCopyFieldGroupOfflineWithRecordLink();
                }
            }
            else
            {
                this.crmQuery.Find(this.SourceRequestOption, this);
            }

            return true;
        }

        /// <summary>
        /// Gets the changed records.
        /// </summary>
        /// <param name="rootTemplateFilter">The root template filter.</param>
        /// <param name="baseTemplateFilter">The base template filter.</param>
        /// <param name="ignoreDefault">if set to <c>true</c> [ignore default].</param>
        /// <returns></returns>
        public List<UPCRMRecord> ChangedRecords(UPConfigFilter rootTemplateFilter, UPConfigFilter baseTemplateFilter, bool ignoreDefault)
        {
            UPCRMRecord rootRecord;
            bool changedRoot = false;
            if (!string.IsNullOrEmpty(this.AnswerRootRecordIdentification))
            {
                rootRecord = new UPCRMRecord(this.AnswerRootRecordIdentification);
            }
            else
            {
                rootRecord = new UPCRMRecord(this.SurveySearchAndList.InfoAreaId);
                rootRecord.AddLink(new UPCRMLink(this.RootRecordIdentification));
                if (this.Questionnaire.Manager.LinkSurveyToQuestionnaire)
                {
                    rootRecord.AddLink(new UPCRMLink(this.Questionnaire.RecordIdentification));
                }

                if (this.SurveyTemplateFilter != null)
                {
                    UPConfigFilter filter = this.SurveyTemplateFilter.FilterByApplyingValueDictionaryDefaults(this.Parameters, true);
                    rootRecord.ApplyValuesFromTemplateFilter(filter);
                }

                changedRoot = true;
            }

            Dictionary<string, UPCRMRecord> foreignFieldDictionary = new Dictionary<string, UPCRMRecord>();
            List<UPCRMRecord> answerRecords = new List<UPCRMRecord>();
            foreach (UPQuestionnaireQuestion question in this.Questionnaire.Questions)
            {
                UPSurveyAnswer answer = this.surveyAnswers[question.QuestionId];
                List<UPCRMRecord> currentAnswerRecords = answer.ChangedRecords(rootRecord, this.Parameters, ignoreDefault);
                if (currentAnswerRecords.Count > 0)
                {
                    answerRecords.AddRange(currentAnswerRecords);
                }

                if (question.Save)
                {
                    UPSurveyForeignField foreignField = answer.SurveyForeignField;
                    if (!string.IsNullOrEmpty(foreignField.RecordIdentification) && !string.IsNullOrEmpty(foreignField.Value)
                        && !string.IsNullOrEmpty(answer.Answer) && foreignField.Value != answer.Answer)
                    {
                        UPCRMRecord foreignRecord = foreignFieldDictionary.ValueOrDefault(foreignField.RecordIdentification);
                        if (foreignRecord == null)
                        {
                            foreignRecord = new UPCRMRecord(foreignField.RecordIdentification);
                            foreignFieldDictionary[foreignField.RecordIdentification] = foreignRecord;
                        }

                        UPCRMFieldValue fieldValue = new UPCRMFieldValue(answer.Answer, foreignRecord.InfoAreaId, foreignField.FieldInfo.FieldId, this.Questionnaire.Manager.AutomaticOnlineSaveOfForeignFields);
                        foreignRecord.AddValue(fieldValue);
                    }
                }
            }

            List<UPCRMRecord> changedRecords = new List<UPCRMRecord>();
            if (changedRoot)
            {
                changedRecords.Add(rootRecord);
            }

            if (answerRecords.Count > 0)
            {
                changedRecords.AddRange(answerRecords);
            }

            if (changedRecords.Count > 0)
            {
                UPCRMRecord rootSyncRecord = new UPCRMRecord(rootRecord, "Sync");
                changedRecords.Add(rootSyncRecord);
            }

            if (foreignFieldDictionary.Count > 0)
            {
                changedRecords.AddRange(foreignFieldDictionary.Values);
            }

            if (rootTemplateFilter != null)
            {
                rootTemplateFilter = rootTemplateFilter.FilterByApplyingValueDictionaryDefaults(this.Parameters, true);
                if (rootTemplateFilter != null)
                {
                    UPCRMRecord record = new UPCRMRecord(rootRecord);
                    record.ApplyValuesFromTemplateFilter(rootTemplateFilter);
                    changedRecords.Add(record);
                }
            }

            if (baseTemplateFilter != null)
            {
                baseTemplateFilter = baseTemplateFilter.FilterByApplyingValueDictionaryDefaults(this.Parameters, true);
                if (baseTemplateFilter != null)
                {
                    UPCRMRecord record = new UPCRMRecord(this.RecordIdentification);
                    record.ApplyValuesFromTemplateFilter(baseTemplateFilter);
                    changedRecords.Add(record);
                }
            }

            return changedRecords;
        }

        /// <summary>
        /// Sets the answer array for question key.
        /// </summary>
        /// <param name="answerArray">The answer array.</param>
        /// <param name="questionKey">The question key.</param>
        /// <returns></returns>
        public UPSurveyActionResult SetAnswerArrayForQuestionKey(List<string> answerArray, string questionKey)
        {
            UPSurveyActionResult result = new UPSurveyActionResult();
            if (!string.IsNullOrEmpty(questionKey) && this.CurrentQuestion.QuestionId != questionKey)
            {
                UPQuestionnaireQuestion quest = this.Questionnaire.QuestionForId(questionKey);
                if (quest == null)
                {
                    result.Ok = false;
                    return result;
                }

                this.SurveyPath.CurrentQuestion = quest;
            }
            else
            {
                questionKey = this.CurrentQuestion.QuestionId;
            }

            UPSurveyAnswer currentAnswer = this.surveyAnswers.ValueOrDefault(questionKey);
            if (currentAnswer == null)
            {
                result.Ok = false;
                return result;
            }

            UPQuestionnaireQuestion currentNextQuestion = currentAnswer.NextQuestion;
            if (currentAnswer.Multiple)
            {
                currentAnswer.SetAnswerArray(answerArray);
            }
            else
            {
                currentAnswer.Answer = answerArray.Count > 0 ? answerArray[0] : null;
            }

            if (!this.CurrentQuestion.HasAnswerOptions)
            {
                this.SurveyPath.CurrentQuestion = currentNextQuestion;
                return result;
            }

            UPQuestionnaireQuestion nextQuestion = currentAnswer.NextQuestion;
            if (nextQuestion != currentNextQuestion)
            {
                result.QuestionOrderChanged = true;
                this.SurveyPath.Rebuild();
            }

            this.SurveyPath.CurrentQuestion = nextQuestion;
            return result;
        }

        /// <summary>
        /// Sets the answer for question key.
        /// </summary>
        /// <param name="answer">The answer.</param>
        /// <param name="questionKey">The question key.</param>
        /// <returns></returns>
        public UPSurveyActionResult SetAnswerForQuestionKey(string answer, string questionKey)
        {
            return this.SetAnswerArrayForQuestionKey(answer != null ? new List<string> { answer } : new List<string>(), questionKey);
        }

        /// <summary>
        /// Gets the visible question groups.
        /// </summary>
        /// <value>
        /// The visible question groups.
        /// </value>
        public List<UPQuestionnaireQuestionGroup> VisibleQuestionGroups => this.SurveyPath.VisibleQuestionnaireGroupArray;

        /// <summary>
        /// Visibles the questions for group.
        /// </summary>
        /// <param name="questionnaireGroup">The questionnaire group.</param>
        /// <returns></returns>
        public List<UPQuestionnaireQuestion> VisibleQuestionsForGroup(UPQuestionnaireQuestionGroup questionnaireGroup)
        {
            return this.SurveyPath.VisibleQuestionsForGroup(questionnaireGroup);
        }

        /// <summary>
        /// Surveys the answer for question.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <returns></returns>
        public UPSurveyAnswer SurveyAnswerForQuestion(UPQuestionnaireQuestion question)
        {
            return this.SurveyAnswerForQuestionKey(question.QuestionId);
        }

        /// <summary>
        /// Surveys the answer for question key.
        /// </summary>
        /// <param name="questionKey">The question key.</param>
        /// <returns></returns>
        public UPSurveyAnswer SurveyAnswerForQuestionKey(string questionKey)
        {
            return !string.IsNullOrEmpty(questionKey) ? this.surveyAnswers[questionKey] : null;
        }

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            if (this.loadStep == 11 && error.IsConnectionOfflineError())
            {
                this.crmQuery = null;
                this.TrySourceCopyFieldGroupOfflineWithRecordLink();
                return;
            }

            this.TheDelegate.SurveyDidFailWithError(this, error);
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
                case 11:
                    if (result?.RowCount > 1)
                    {
                        this.TheDelegate.SurveyDidFailWithError(this, new Exception("SourceCopyFields could not be loaded"));
                        return;
                    }

                    if (result == null || result?.RowCount == 0)
                    {
                        this.crmQuery = null;
                        this.TrySourceCopyFieldGroupOfflineWithRecordLink();
                        return;
                    }

                    this.crmQuery = null;
                    this.SourceCopyFieldGroupLoaded((UPCRMResultRow)result.ResultRowAtIndex(0));
                    return;

                case 1:
                    if (result?.RowCount != 1)
                    {
                        this.TheDelegate.SurveyDidFailWithError(this, new Exception("SourceCopyFields could not be loaded"));
                        return;
                    }

                    this.crmQuery = null;
                    this.SourceCopyFieldGroupLoaded((UPCRMResultRow)result.ResultRowAtIndex(0));
                    return;

                case 2:
                    this.SurveyLoaded(result.RowCount == 0 ? null : (UPCRMResultRow)result.ResultRowAtIndex(0));
                    return;

                case 3:
                    this.SurveyAnswersLoaded(result);
                    return;

                case 4:
                    this.ForeignFieldsLoaded(result.RowCount == 0 ? null : (UPCRMResultRow)result.ResultRowAtIndex(0));
                    return;

                default:
                    break;
            }
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

        /// <summary>
        /// The link reader did finish with result.
        /// </summary>
        /// <param name="linkReader">The link reader.</param>
        /// <param name="result">The result.</param>
        public void LinkReaderDidFinishWithResult(UPCRMLinkReader linkReader, object result)
        {
            this.RootRecordIdentification = linkReader?.DestinationRecordIdentification;
            this.ParentLinkLoaded();
        }

        /// <summary>
        /// Links the reader did finish with error.
        /// </summary>
        /// <param name="linkReader">The link reader.</param>
        /// <param name="error">The error.</param>
        public void LinkReaderDidFinishWithError(UPCRMLinkReader linkReader, Exception error)
        {
            this.TheDelegate.SurveyDidFailWithError(this, error);
        }

        /// <summary>
        /// Questionnaire the did fail with error.
        /// </summary>
        /// <param name="questionnaire">The questionnaire.</param>
        /// <param name="error">The error.</param>
        public void QuestionnaireDidFailWithError(UPQuestionnaire questionnaire, Exception error)
        {
            this.TheDelegate.SurveyDidFailWithError(this, error);
        }

        /// <summary>
        /// Questionnaire the did finish with result.
        /// </summary>
        /// <param name="questionnaire">The questionnaire.</param>
        /// <param name="result">The result.</param>
        public void QuestionnaireDidFinishWithResult(UPQuestionnaire questionnaire, object result)
        {
            this.Questionnaire = questionnaire;
            this.QuestionnaireLoaded();
        }

        /// <summary>
        /// Synchronizing the data sets did fail.
        /// </summary>
        /// <param name="sets">
        /// The sets.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        public void SyncDataSetsDidFail(UPSyncDataSets sets, Exception error)
        {
            // continue anyway and hope for the best
            this.syncDataSets = null;
            this.SurveyRootExistsLocally();
        }

        /// <summary>
        /// Synchronizing the data sets did finish.
        /// </summary>
        /// <param name="sets">
        /// The sets.
        /// </param>
        /// <param name="json">
        /// The json.
        /// </param>
        /// <param name="changedRecordIdentifications">
        /// The changed record identifications.
        /// </param>
        public void SyncDataSetsDidFinishSync(UPSyncDataSets sets, Dictionary<string, object> json, List<string> changedRecordIdentifications)
        {
            this.syncDataSets = null;
            this.SurveyRootExistsLocally();
        }

        /// <summary>
        /// Synchronizing the data sets did finish.
        /// </summary>
        /// <param name="sets">
        /// The sets.
        /// </param>
        /// <param name="json">
        /// The json.
        /// </param>
        /// <param name="changedRecordIdentifications">
        /// The changed record identifications.
        /// </param>
        public void SyncDataSetsDidFinishSyncWithObject(UPSyncDataSets sets, DataModelSyncDeserializer json, List<string> changedRecordIdentifications)
        {
        }

        private void TrySourceCopyFieldGroupOfflineWithRecordLink()
        {
            if (!UPCRMDataStore.DefaultStore.RecordExistsOffline(this.RecordIdentification))
            {
                this.TheDelegate.SurveyDidFailWithError(this, new Exception("SourceCopyFields could not be loaded"));
                return;
            }

            this.crmQuery = this.SourceCopyFieldGroup != null
                ? new UPContainerMetaInfo(this.SourceCopyFieldGroup)
                : new UPContainerMetaInfo(new List<UPCRMField>(), this.RecordIdentification.InfoAreaId());

            UPQuestionnaireManager questionnaireManager = ServerSession.CurrentSession.QuestionnaireManager;
            UPCRMLinkField questionnairelinkField = new UPCRMLinkField(questionnaireManager.QuestionnaireList.InfoAreaId, -1, this.RecordIdentification.InfoAreaId());
            List<UPContainerFieldMetaInfo> fieldMetaInfos = this.crmQuery.AddCrmFields(new List<UPCRMField> { questionnairelinkField });
            if (fieldMetaInfos.Count > 0)
            {
                this.questionnairelinkFieldMetaInfo = fieldMetaInfos[0];
            }

            this.crmQuery.SetLinkRecordIdentification(this.RecordIdentification);
            this.loadStep = 1;
            this.crmQuery.Find(UPRequestOption.Offline, this);
        }

        private void SourceCopyFieldGroupLoaded(UPCRMResultRow resultRow)
        {
            this.Parameters = resultRow.ValuesWithFunctions();
            UPQuestionnaireManager questionnaireManager = null; //ServerSession.CurrentSession.QuestionnaireManager;
            if (this.QuestionnaireLinkName == "RecordLink")
            {
                string questionnaireRecordIdentification = resultRow.RecordIdentificationForLinkInfoAreaIdLinkId(questionnaireManager.QuestionnaireList.InfoAreaId, -1);
                if (questionnaireRecordIdentification.Length == 0 && this.questionnairelinkFieldMetaInfo != null)
                {
                    string questionnaireRecordId = resultRow.RawValueAtIndex(this.questionnairelinkFieldMetaInfo.PositionInResult);
                    if (!string.IsNullOrEmpty(questionnaireRecordId))
                    {
                        questionnaireRecordIdentification = StringExtensions.InfoAreaIdRecordId(questionnaireManager.QuestionnaireList.InfoAreaId, questionnaireRecordId);
                    }
                }

                if (!string.IsNullOrEmpty(questionnaireRecordIdentification))
                {
                    this.Questionnaire = questionnaireManager.LoadQuestionaire(questionnaireRecordIdentification, this);
                }
                else
                {
                    this.TheDelegate.SurveyDidFailWithError(this, new Exception("cannot load questionnaire, recordIdentification empty"));
                    return;
                }
            }
            else
            {
                int catalogCode = Convert.ToInt32(this.Parameters.ValueOrDefault(this.QuestionnaireLinkName));
                if (catalogCode > 0)
                {
                    this.Questionnaire = questionnaireManager.LoadQuestionaire(catalogCode, this);
                }
                else
                {
                    this.TheDelegate.SurveyDidFailWithError(this, new Exception("cannot load questionnaire with code, catalogCode empty"));
                    return;
                }
            }

            if (this.Questionnaire != null)
            {
                this.QuestionnaireLoaded();
            }
        }

        private void QuestionnaireLoaded()
        {
            if (this.Questionnaire == null)
            {
                this.TheDelegate.SurveyDidFailWithError(this, new Exception("questionnaire could not be loaded"));
                return;
            }

            Dictionary<string, object> dict = new Dictionary<string, object>(this.Questionnaire.DataDictionary);
            foreach (string key in this.Parameters.Keys)
            {
                dict[key] = this.Parameters[key];
            }

            this.Parameters = dict;
            if (!string.IsNullOrEmpty(this.ParentLink))
            {
                this.linkReader = new UPCRMLinkReader(this.RecordIdentification, this.ParentLink, this.SourceRequestOption, this);
                this.linkReader.Start();
            }
            else
            {
                this.RootRecordIdentification = this.RecordIdentification;
                this.ParentLinkLoaded();
            }
        }

        private void InitializeAndLoadForeignFields()
        {
            List<UPQuestionnaireQuestion> foreignFieldQuestions = this.Questionnaire.ForeignFieldQuestions;
            Dictionary<string, UPSurveyForeignField> foreignFieldDictionary = new Dictionary<string, UPSurveyForeignField>(foreignFieldQuestions.Count);
            List<UPCRMField> fieldArray = new List<UPCRMField>(foreignFieldQuestions.Count);
            string recordInfoAreaId = this.RootRecordIdentification.InfoAreaId();
            foreach (UPQuestionnaireQuestion question in foreignFieldQuestions)
            {
                UPSurveyForeignField foreignField = new UPSurveyForeignField(question);
                UPSurveyForeignField existingForeignField = foreignFieldDictionary.ValueOrDefault(foreignField.Key);

                if (existingForeignField != null)
                {
                    existingForeignField.AddQuestion(question);
                }
                else
                {
                    foreignFieldDictionary[foreignField.Key] = foreignField;
                    if (!string.IsNullOrEmpty(recordInfoAreaId))
                    {
                        bool addField = true;
                        UPCRMField field = UPCRMField.FieldFromFieldInfo(foreignField.FieldInfo);
                        if (recordInfoAreaId != field.InfoAreaId)
                        {
                            if (!UPCRMDataStore.DefaultStore.InfoAreaIdLinkIdIsParentOfInfoAreaId(recordInfoAreaId, -1, field.InfoAreaId))
                            {
                                addField = false;
                            }
                        }

                        if (addField)
                        {
                            foreignField.PositionInResult = fieldArray.Count;
                            fieldArray.Add(UPCRMField.FieldFromFieldInfo(foreignField.FieldInfo));
                        }
                    }
                }
            }

            this.ForeignFields = foreignFieldDictionary;
            if (string.IsNullOrEmpty(this.RootRecordIdentification))
            {
                this.ForeignFieldsLoaded(null);
            }

            this.crmQuery = new UPContainerMetaInfo(fieldArray, this.RootRecordIdentification.InfoAreaId());
            this.crmQuery.SetLinkRecordIdentification(this.RootRecordIdentification);
            this.loadStep = 4;
            this.crmQuery.Find(this.SourceRequestOption, this);
        }

        private void ParentLinkLoaded()
        {
            if (this.RootRecordIdentification == null)
            {
                this.TheDelegate.SurveyDidFailWithError(this, new Exception("root record could not be loaded"));
                return;
            }

            if (this.Questionnaire.ForeignFieldQuestions.Count > 0)
            {
                this.InitializeAndLoadForeignFields();
            }
            else
            {
                this.ForeignFieldsLoaded(null);
            }
        }

        private void ForeignFieldsLoaded(UPCRMResultRow row)
        {
            if (row != null)
            {
                foreach (UPSurveyForeignField foreignField in this.ForeignFields.Values)
                {
                    if (foreignField.PositionInResult < 0)
                    {
                        continue;
                    }

                    foreignField.SetValueRecordIdentification(row.RawValueAtIndex(foreignField.PositionInResult), row.RecordIdentificationAtFieldIndex(foreignField.PositionInResult));
                }
            }

            this.loadStep = 2;
            this.crmQuery = new UPContainerMetaInfo(this.SurveySearchAndList, this.Parameters);
            this.crmQuery.SetLinkRecordIdentification(this.RootRecordIdentification);
            this.crmQuery.Find(this.DestinationRequestOption, this);
        }

        private void SyncSurveyRecord()
        {
            this.syncDataSets = new UPSyncDataSets(new List<string>(), this.AnswerRootRecordIdentification, this);
            this.syncDataSets.Start();
        }

        private void SurveyLoaded(UPCRMResultRow row)
        {
            if (row == null)
            {
                this.AnswerRootRecordIdentification = null;
                this.LoadFinished();
                return;
            }

            this.AnswerRootRecordIdentification = row.RootRecordIdentification;
            if (this.DestinationRequestOption == UPRequestOption.Online || UPCRMDataStore.DefaultStore.RecordExistsOffline(this.AnswerRootRecordIdentification))
            {
                this.SurveyRootExistsLocally();
            }
            else
            {
                this.SyncSurveyRecord();
            }
        }

        private void SurveyRootExistsLocally()
        {
            this.loadStep = 3;
            this.crmQuery = new UPContainerMetaInfo(this.SurveyAnswerSearchAndList, this.Parameters);
            this.crmQuery.SetLinkRecordIdentification(this.AnswerRootRecordIdentification);
            this.crmQuery.Find(this.DestinationRequestOption, this);
        }

        private void SurveyAnswersLoaded(UPCRMResult result)
        {
            int count = result.RowCount;
            for (int i = 0; i < count; i++)
            {
                UPCRMResultRow row = (UPCRMResultRow)result.ResultRowAtIndex(i);
                Dictionary<string, object> valuesWithFunction = row.ValuesWithFunctions();
                string questionKey = valuesWithFunction.ValueOrDefault(Constants.QuestionnaireQuestionNumber) as string;
                UPQuestionnaireQuestion question = this.Questionnaire?.QuestionForId(questionKey);
                if (question == null)
                {
                    continue;
                }

                UPSurveyAnswer answer = null;
                if (question.Multiple)
                {
                    answer = this.surveyAnswers.ValueOrDefault(questionKey);
                }

                if (answer == null)
                {
                    answer = new UPSurveyAnswer(row.RootRecordIdentification, valuesWithFunction, question, this);
                    this.surveyAnswers[questionKey] = answer;
                }
                else
                {
                    answer.AddAnswer(valuesWithFunction, row.RootRecordIdentification);
                }
            }

            this.crmQuery = null;
            this.LoadFinished();
        }

        private void LoadFinished()
        {
            if (this.Questionnaire != null)
            {
                foreach (string questionId in this.Questionnaire.QuestionsById.Keys)
                {
                    UPSurveyAnswer currentAnswer = this.surveyAnswers.ValueOrDefault(questionId);
                    if (currentAnswer == null)
                    {
                        UPQuestionnaireQuestion quest = this.Questionnaire.QuestionForId(questionId);
                        if (quest == null)
                        {
                            continue;
                        }

                        currentAnswer = new UPSurveyAnswer(quest, this);
                        if (Convert.ToInt32(quest.DefaultAnswer) > 0)
                        {
                            currentAnswer.DefaultAnswer = quest.DefaultAnswer;
                        }

                        this.surveyAnswers[questionId] = currentAnswer;
                    }
                }
            }

            if (this.ForeignFields != null)
            {
                foreach (UPSurveyForeignField foreignField in this.ForeignFields.Values)
                {
                    foreach (UPQuestionnaireQuestion quest in foreignField.QuestionArray)
                    {
                        UPSurveyAnswer currentAnswer = this.surveyAnswers[quest.QuestionId];
                        if (!string.IsNullOrEmpty(foreignField.Value))
                        {
                            currentAnswer.DefaultAnswer = foreignField.Value;
                        }

                        currentAnswer.SurveyForeignField = foreignField;
                    }
                }
            }

            this.SurveyPath = new UPSurveyPath(this);
            this.TheDelegate.SurveyDidFinishWithResult(this, this);
        }
    }
}
