// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuestionnaireEditPageModelController.cs" company="Aurea Software Gmbh">
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
//   The Questionnaire Group
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Questionnaire
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.Services.ModelControllers.Edit;
    using Aurea.CRM.Services.ModelControllers.Group;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Fields.Edit;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;
    using Aurea.CRM.UIModel.Pages;
    using Aurea.CRM.UIModel.Questionnaire;
    using Aurea.CRM.UIModel.Status;
    using Core.Questionnaire;

    /// <summary>
    /// Questionnaire Edit Page Model Controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Edit.EditPageModelController" />
    /// <seealso cref="Aurea.CRM.Services.Delegates.ISurveyDelegate" />
    public class QuestionnaireEditPageModelController : EditPageModelController, ISurveyDelegate
    {
        private const char FieldTypeX = 'X';
        private const char FieldTypeK = 'K';
        private const char FieldTypeD = 'D';

        private UPContainerMetaInfo crmQuery;
        private bool questionnaireConfirmed;
        private UPMGroup portfolioGroup;
        private UPWebContentGroupModelController portfolioGroupController;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionnaireEditPageModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public QuestionnaireEditPageModelController(ViewReference viewReference)
            : base(viewReference)
        {
            this.RecordIdentification = this.ViewReference.ContextValueForKey("RecordId");
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            string configValue = this.ViewReference.ContextValueForKey("ConfirmButtonName");
            if (!string.IsNullOrEmpty(configValue))
            {
                this.ConfirmationButton = configStore.ButtonByName(configValue);
            }

            configValue = this.ViewReference.ContextValueForKey("ConfirmedFilterName");
            if (!string.IsNullOrEmpty(configValue))
            {
                this.ConfirmedFilter = configStore.FilterByName(configValue);
            }

            this.Readonly = this.ViewReference.ContextValueIsSet("ReadOnly") || this.ViewReference.ContextValueIsSet("QuestionnaireReadOnly");
            this.Survey = new UPSurvey(this.ViewReference, this.RecordIdentification, this);
            this.PortfolioMode = this.ViewReference.ContextValueIsSet("Portfolio");
            this.BuildPage();
        }

        /// <summary>
        /// Gets the questionnaire record identification.
        /// </summary>
        /// <value>
        /// The questionnaire record identification.
        /// </value>
        public string QuestionnaireRecordIdentification { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="QuestionnaireEditPageModelController"/> is readonly.
        /// </summary>
        /// <value>
        ///   <c>true</c> if readonly; otherwise, <c>false</c>.
        /// </value>
        public bool Readonly { get; private set; }

        /// <summary>
        /// Gets the survey.
        /// </summary>
        /// <value>
        /// The survey.
        /// </value>
        public UPSurvey Survey { get; private set; }

        /// <summary>
        /// Gets the questionnaire.
        /// </summary>
        /// <value>
        /// The questionnaire.
        /// </value>
        public UPQuestionnaire Questionnaire { get; private set; }

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get; private set; }

        /// <summary>
        /// Gets the confirmation button.
        /// </summary>
        /// <value>
        /// The confirmation button.
        /// </value>
        public UPConfigButton ConfirmationButton { get; private set; }

        /// <summary>
        /// Gets the confirmed filter.
        /// </summary>
        /// <value>
        /// The confirmed filter.
        /// </value>
        public UPConfigFilter ConfirmedFilter { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [portfolio mode].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [portfolio mode]; otherwise, <c>false</c>.
        /// </value>
        public bool PortfolioMode { get; private set; }

        /// <summary>
        /// Builds the page.
        /// </summary>
        public override void BuildPage()
        {
            Page page = this.InstantiatePage();
            page.Invalid = true;
            this.TopLevelElement = page;
            this.ApplyLoadingStatusOnPage((Page)this.TopLevelElement);
        }

        /// <summary>
        /// Instantiates the page.
        /// </summary>
        /// <returns></returns>
        public override Page InstantiatePage()
        {
            return this.Page != null
                ? new UPMQuestionnairePage(this.Page.Identifier)
                : new UPMQuestionnairePage(FieldIdentifier.IdentifierWithRecordIdentificationFieldId("test", "Page0"));
        }

        /// <summary>
        /// Updateds the element for page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        public override UPMElement UpdatedElementForPage(Page page)
        {
            UPMQuestionnairePage newPage = (UPMQuestionnairePage)this.InstantiatePage();
            newPage.Invalid = true;
            this.TopLevelElement = newPage;
            this.ApplyLoadingStatusOnPage((Page)this.TopLevelElement);
            if (this.Survey != null)
            {
                this.Survey.Load();
            }
            else
            {
                this.FillPage(this.Page);
            }

            return newPage;
        }

        /// <summary>
        /// Applies the loading status on page.
        /// </summary>
        /// <param name="page">The page.</param>
        public override void ApplyLoadingStatusOnPage(Page page)
        {
            UPMProgressStatus loadinfStatus = new UPMProgressStatus(StringIdentifier.IdentifierWithStringId("loadingIdentifier"));
            UPMStringField statusField = new UPMStringField(StringIdentifier.IdentifierWithStringId("statusFieldIdentifier"))
            {
                FieldValue = LocalizedString.TextLoadingData
            };
            loadinfStatus.StatusMessageField = statusField;
            page.Status = loadinfStatus;
        }

        private static void CreateFields(
            UPQuestionnaireQuestion currentQuestion,
            out UPMEditField editField,
            out IQuestionnaireEditFieldContext editFieldContext,
            out UPQuestionnaireCatalogEditField catalogEditField,
            IIdentifier fieldIdentifier,
            bool multiSelect,
            IList<string> explicitKeyOrder)
        {
            if (currentQuestion == null)
            {
                throw new ArgumentNullException(nameof(currentQuestion));
            }

            if (explicitKeyOrder == null)
            {
                throw new ArgumentNullException(nameof(explicitKeyOrder));
            }

            editField = null;
            editFieldContext = null;
            catalogEditField = null;

            switch (currentQuestion.FieldType[0])
            {
                case FieldTypeX:
                case FieldTypeK:
                    var answers = currentQuestion.Answers();
                    if (answers == null)
                    {
                        break;
                    }

                    if (answers.Count > 10)
                    {
                        catalogEditField = new UPQuestionnaireCatalogEditField(fieldIdentifier, multiSelect)
                        {
                            CatalogElementViewType = CatalogElementViewType.PopOver
                        };
                    }
                    else
                    {
                        catalogEditField = new UPQuestionnaireCatalogEditField(fieldIdentifier, multiSelect)
                        {
                            CatalogElementViewType = CatalogElementViewType.Table
                        };
                    }

                    catalogEditField.NullValueKey = "0";
                    if (multiSelect)
                    {
                        catalogEditField.MultiSelectMaxCount = answers.Count;
                    }

                    editField = catalogEditField;
                    editFieldContext = catalogEditField;
                    foreach (var answer in answers)
                    {
                        var catalogPossibleValue = new UPMCatalogPossibleValue
                        {
                            TitleLabelField = new UPMStringField(null)
                            {
                                StringValue = answer.AnswerText
                            }
                        };
                        explicitKeyOrder.Add(answer.AnswerId);
                        catalogEditField.AddPossibleValue(catalogPossibleValue, answer.AnswerId);
                    }

                    catalogEditField.ExplicitKeyOrder = explicitKeyOrder as List<string>;
                    break;

                case FieldTypeD:
                    editField = new UPQuestionnaireDateTimeEditField(fieldIdentifier);
                    editFieldContext = (IQuestionnaireEditFieldContext)editField;
                    break;

                default:
                    editField = new UPQuestionnaireStringEditField(fieldIdentifier);
                    editFieldContext = (IQuestionnaireEditFieldContext)editField;
                    break;
            }
        }

        private static void ProcessEditField(
            UPMQuestionnaireGroup questionnaireGroup,
            UPQuestionnaireQuestion currentQuestion,
            UPMEditField editField,
            IQuestionnaireEditFieldContext editFieldContext,
            UPQuestionnaireCatalogEditField catalogEditField,
            string surveyAnswer,
            IList<string> answerIds,
            bool multiSelect)
        {
            if (questionnaireGroup == null)
            {
                throw new ArgumentNullException(nameof(questionnaireGroup));
            }

            if (currentQuestion == null)
            {
                throw new ArgumentNullException(nameof(currentQuestion));
            }

            if (editFieldContext == null)
            {
                throw new ArgumentNullException(nameof(editFieldContext));
            }

            if (answerIds == null)
            {
                throw new ArgumentNullException(nameof(answerIds));
            }

            editField.LabelText = currentQuestion.Label;
            editField.DetailText = currentQuestion.AdditionalInfo;
            editField.RequiredField = currentQuestion.Mandatory;
            editFieldContext.Question = currentQuestion;

            if (multiSelect)
            {
                if (catalogEditField != null)
                {
                    foreach (string a in answerIds)
                    {
                        catalogEditField.AddFieldValue(a);
                    }
                }
            }
            else
            {
                editField.FieldValue = surveyAnswer;
            }

            editField.ContinuousUpdate = true;
            var questionnaireQuestion = new UPMQuestionnaireQuestion(editField)
            {
                Mandatory = currentQuestion.Mandatory
            };
            questionnaireGroup.AddQuestion(questionnaireQuestion);
        }

        private void FinalizeButtonClicked(object dict)
        {
            string recordIdentification = this.Survey.AnswerRootRecordIdentification ?? string.Empty;
            this.ParentOrganizerModelController.PerformActionWithViewReference(this.ConfirmationButton.ViewReference.ViewReferenceWith(recordIdentification));
        }

        private void FillPage(Page pageObject)
        {
            var questionGroups = this.Survey.VisibleQuestionGroups;
            UPMQuestionnairePage page;
            if (pageObject is UPMQuestionnairePage)
            {
                page = (UPMQuestionnairePage)pageObject;
            }
            else
            {
                return;
            }

            page.Readonly = this.Readonly || this.ViewReference.ViewName == "QuestionnaireView";
            if (this.questionnaireConfirmed)
            {
                page.FinalizedText = this.ConfirmedFilter.DisplayName;
                if (string.IsNullOrEmpty(page.FinalizedText))
                {
                    page.FinalizedText = LocalizedString.TextProcessQuestionnaireFinalized;
                }
            }
            else if (this.ConfirmationButton != null)
            {
                var action = new UPMAction(StringIdentifier.IdentifierWithStringId("Action.Approve"))
                {
                    LabelText = this.ConfirmationButton.Label
                };
                if (string.IsNullOrEmpty(action.LabelText))
                {
                    action.LabelText = LocalizedString.TextProcessQuestionnaireFinalize;
                }

                action.SetTargetAction(this, this.FinalizeButtonClicked);
                page.FinalizeAction = action;
            }

            page.RemoveAllChildren();
            if (this.PortfolioMode)
            {
                this.portfolioGroup = this.PortfolioGroupForSurvey();
            }

            int index = 0;
            page.LabelText = this.Survey.Questionnaire.Label;
            foreach (var currentGroup in questionGroups)
            {
                ++index;
                this.ProcessGroup(page, index, currentGroup);
            }

            page.Status = null;
        }

        private void ProcessGroup(UPMQuestionnairePage page, int index, UPQuestionnaireQuestionGroup currentGroup)
        {
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            if (currentGroup == null)
            {
                throw new ArgumentNullException(nameof(currentGroup));
            }

            bool firstQuestion = true;
            var questionnaireGroup = new UPMQuestionnaireGroup(StringIdentifier.IdentifierWithStringId($"group_{currentGroup.Label}"))
            {
                LabelText = currentGroup.Label
            };
            page.AddGroup(questionnaireGroup);
            var questions = this.Survey.VisibleQuestionsForGroup(currentGroup);
            foreach (var currentQuestion in questions)
            {
                if (currentQuestion.Hide)
                {
                    continue;
                }

                UPMEditField editField = null;
                IQuestionnaireEditFieldContext editFieldContext = null;
                UPQuestionnaireCatalogEditField catalogEditField = null;
                if (firstQuestion)
                {
                    if (string.IsNullOrEmpty(questionnaireGroup.LabelText))
                    {
                        questionnaireGroup.LabelText = currentQuestion.Label;
                    }

                    firstQuestion = false;
                }

                var fieldIdentifier = StringIdentifier.IdentifierWithStringId($"{currentQuestion.RecordIdentification}_{index}");
                var surveyAnswer = this.Survey.SurveyAnswerForQuestion(currentQuestion);
                var answerIds = surveyAnswer.AnswerIds;
                bool multiSelect = currentQuestion.Multiple;
                var explicitKeyOrder = new List<string>();

                CreateFields(
                    currentQuestion,
                    out editField,
                    out editFieldContext,
                    out catalogEditField,
                    fieldIdentifier,
                    multiSelect,
                    explicitKeyOrder);

                if (editField != null)
                {
                    ProcessEditField(
                        questionnaireGroup,
                        currentQuestion,
                        editField,
                        editFieldContext,
                        catalogEditField,
                        surveyAnswer.Answer,
                        answerIds,
                        multiSelect);
                }
            }
        }

        /// <summary>
        /// Pages for overview.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        public Page PageForOverview(Page page)
        {
            Page newPage = this.InstantiatePage();
            if (this.portfolioGroup != null)
            {
                newPage.AddGroup(this.portfolioGroup);
            }

            foreach (UPMQuestionnaireGroup currentGroup in page.Groups)
            {
                UPMQuestionnaireGroup questionnaireGroup = new UPMQuestionnaireGroup(currentGroup.Identifier)
                {
                    LabelText = currentGroup.LabelText
                };
                bool insertGroup = false;
                foreach (UPMQuestionnaireQuestion currentQuestion in currentGroup.Questions)
                {
                    if (currentQuestion.Field.Empty)
                    {
                        continue;
                    }

                    insertGroup = true;
                    UPMEditField editField = (UPMEditField)currentQuestion.Field;
                    UPMField field = new UPMStringField(editField.Identifier)
                    {
                        LabelText = editField.LabelText
                    };
                    if (!editField.Empty)
                    {
                        field.FieldValue = editField.StringDisplayValue;
                    }

                    UPMQuestionnaireQuestion questionnaireQuestion = new UPMQuestionnaireQuestion(field);
                    questionnaireGroup.AddQuestion(questionnaireQuestion);
                }

                if (insertGroup)
                {
                    newPage.AddGroup(questionnaireGroup);
                }
            }

            return newPage;
        }

        /// <summary>
        /// Pages for mandatory.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        public Page PageForMandatory(Page page)
        {
            Page newPage = this.InstantiatePage();
            foreach (UPMQuestionnaireGroup currentGroup in page.Groups)
            {
                UPMQuestionnaireGroup questionnaireGroup = new UPMQuestionnaireGroup(currentGroup.Identifier)
                {
                    LabelText = currentGroup.LabelText
                };
                bool insertGroup = false;
                foreach (UPMQuestionnaireQuestion currentQuestion in currentGroup.Questions)
                {
                    if (!currentQuestion.Mandatory)
                    {
                        continue;
                    }

                    insertGroup = true;
                    questionnaireGroup.AddQuestion(currentQuestion);
                }

                if (insertGroup)
                {
                    newPage.AddGroup(questionnaireGroup);
                }
            }

            return newPage;
        }

        /// <summary>
        /// Completes for page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        public int CompleteForPage(Page page)
        {
            int countEmptyQuestionAll = 0;
            int countQuestionAll = 0;
            foreach (UPMQuestionnaireGroup currentGroup in page.Groups)
            {
                int countEmpty = currentGroup.Questions.Count(currentQuestion => currentQuestion.Field.Empty);
                countQuestionAll += currentGroup.Fields.Count;
                countEmptyQuestionAll += countEmpty;
            }

            if (countQuestionAll == 0)
            {
                return 0;
            }

            return Convert.ToInt32((float)(countQuestionAll - countEmptyQuestionAll) / countQuestionAll * 100);
        }

        /// <summary>
        /// Completes for group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <returns></returns>
        public int CompleteForGroup(UPMQuestionnaireGroup group)
        {
            if (group.Questions.Count == 0)
            {
                return 0;
            }

            int countEmpty = 0;
            foreach (UPMQuestionnaireQuestion currentQuestion in group.Questions)
            {
                if (currentQuestion.Field.Empty)
                {
                    countEmpty++;
                }
            }

            return Convert.ToInt32((float)(group.Questions.Count - countEmpty) / group.Questions.Count * 100);
        }

        /// <summary>
        /// Progresses the type for page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        public QuestionnaireProgressType ProgressTypeForPage(Page page)
        {
            QuestionnaireProgressType progressType = QuestionnaireProgressType.NoQuestions;
            int countMandatory = 0;
            int countMandatoryAnswer = 0;
            int countAll = 0;
            int countAllAnswer = 0;

            foreach (UPMQuestionnaireGroup currentGroup in page.Groups)
            {
                foreach (UPMQuestionnaireQuestion questionnaireQuestion in currentGroup.Questions)
                {
                    countAll++;
                    if (questionnaireQuestion.Mandatory)
                    {
                        countMandatory++;
                    }

                    if (!questionnaireQuestion.Empty)
                    {
                        countAllAnswer++;
                        if (questionnaireQuestion.Mandatory)
                        {
                            countMandatoryAnswer++;
                        }
                    }
                }
            }

            if (countAllAnswer == 0)
            {
                progressType = QuestionnaireProgressType.NoQuestions;
            }
            else if (countAll == countAllAnswer)
            {
                progressType = QuestionnaireProgressType.AllQuestions;
            }
            else if (countMandatory == countMandatoryAnswer)
            {
                progressType = QuestionnaireProgressType.AllMandatory;
            }
            else if (countMandatory > countMandatoryAnswer)
            {
                progressType = QuestionnaireProgressType.NotAllMandatory;
            }

            return progressType;
        }

        /// <summary>
        /// Progresses the type for group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <returns></returns>
        public QuestionnaireProgressType ProgressTypeForGroup(UPMQuestionnaireGroup group)
        {
            QuestionnaireProgressType progressType = QuestionnaireProgressType.NoQuestions;
            int countMandatory = 0;
            int countMandatoryAnswer = 0;
            int countAll = 0;
            int countAllAnswer = 0;
            foreach (UPMQuestionnaireQuestion questionnaireQuestion in group.Questions)
            {
                countAll++;
                if (questionnaireQuestion.Mandatory)
                {
                    countMandatory++;
                }

                if (!questionnaireQuestion.Empty)
                {
                    countAllAnswer++;
                    if (questionnaireQuestion.Mandatory)
                    {
                        countMandatoryAnswer++;
                    }
                }
            }

            if (countAllAnswer == 0)
            {
                progressType = QuestionnaireProgressType.NoQuestions;
            }
            else if (countAll == countAllAnswer)
            {
                progressType = QuestionnaireProgressType.AllQuestions;
            }
            else if (countMandatory == countMandatoryAnswer)
            {
                progressType = QuestionnaireProgressType.AllMandatory;
            }
            else if (countMandatory > countMandatoryAnswer)
            {
                progressType = QuestionnaireProgressType.NotAllMandatory;
            }

            return progressType;
        }

        private UPMGroup PortfolioGroupForSurvey()
        {
            this.portfolioGroupController = new UPWebContentGroupModelController(this);
            ViewReference portfolioViewReference = new ViewReference(this.ViewReference, null, "Portfolio", "ReportType");
            portfolioViewReference.SetValueForKey("SurveyRecordId", this.ViewReference.ContextValueForKey("RecordId"));
            portfolioViewReference.SetValueForKey("InfoAreaId", this.ViewReference.ContextValueForKey("RecordId"));
            portfolioViewReference.SetValueForKey("ParentLink", string.Empty);

            UPMGroup group = this.portfolioGroupController.ApplyLinkRecordIdentification(this.Survey.AnswerRootRecordIdentification, null, portfolioViewReference);
            group.LabelText = LocalizedString.TextProcessPortfolioResult;
            return group;
        }

        public override void UserDidChangeField(UPMEditField field)
        {
            IQuestionnaireEditFieldContext fieldContext = field as IQuestionnaireEditFieldContext;
            if (fieldContext != null)
            {
                string answerId = field.StringEditValue;
                UPSurveyActionResult surveyActionResult;
                if (field is UPMCatalogEditField)
                {
                    object v = field.FieldValue;
                    if (v is List<string>)
                    {
                        surveyActionResult = this.Survey.SetAnswerArrayForQuestionKey((List<string>)v, fieldContext.Question.QuestionId);
                    }
                    else
                    {
                        surveyActionResult = this.Survey.SetAnswerForQuestionKey((string)v, fieldContext.Question.QuestionId);
                    }
                }
                else
                {
                    surveyActionResult = this.Survey.SetAnswerForQuestionKey(answerId, fieldContext.Question.QuestionId);
                }

                if (surveyActionResult.QuestionOrderChanged)
                {
                    this.FillPage(this.Page);
                    this.InformAboutDidChangeTopLevelElement(this.Page, this.Page, null, null);
                }
            }
        }

        /// <summary>
        /// Changeds the records ignore default with root template filter base template filter.
        /// </summary>
        /// <param name="rootTemplateFilter">The root template filter.</param>
        /// <param name="baseTemplateFilter">The base template filter.</param>
        /// <returns></returns>
        public List<UPCRMRecord> ChangedRecordsIgnoreDefaultWithRootTemplateFilterBaseTemplateFilter(UPConfigFilter rootTemplateFilter, UPConfigFilter baseTemplateFilter)
        {
            return this.Survey.ChangedRecords(rootTemplateFilter, baseTemplateFilter, true);
        }

        /// <summary>
        /// Changeds the records with root template filter base template filter.
        /// </summary>
        /// <param name="rootTemplateFilter">The root template filter.</param>
        /// <param name="baseTemplateFilter">The base template filter.</param>
        /// <returns></returns>
        public List<UPCRMRecord> ChangedRecordsWithRootTemplateFilterBaseTemplateFilter(UPConfigFilter rootTemplateFilter, UPConfigFilter baseTemplateFilter)
        {
            return this.Survey.ChangedRecords(rootTemplateFilter, baseTemplateFilter, false);
        }

        /// <summary>
        /// Surveys the did finish with result.
        /// </summary>
        /// <param name="survey">The survey.</param>
        /// <param name="result">The result.</param>
        public void SurveyDidFinishWithResult(UPSurvey survey, object result)
        {
            this.SurveyLoaded();
            UPMQuestionnairePage page = (UPMQuestionnairePage)this.Page;
            page.QuestionnaireNotExists = false;
        }

        /// <summary>
        /// Surveys the did fail with error.
        /// </summary>
        /// <param name="survey">The survey.</param>
        /// <param name="error">The error.</param>
        public void SurveyDidFailWithError(UPSurvey survey, Exception error)
        {
            UPMQuestionnairePage page = (UPMQuestionnairePage)this.Page;
            page.Status = null;
            page.QuestionnaireNotExists = true;
            this.InformAboutDidChangeTopLevelElement(this.Page, this.Page, null, null);
        }

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public override void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.ReportError(error, true);
        }

        /// <summary>
        /// Searches the operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="result">The result.</param>
        public override void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            this.crmQuery = null;
            this.questionnaireConfirmed = result.RowCount > 0;
            this.LoadFinished();
        }

        /// <summary>
        /// Groups the model controller finished.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public override void GroupModelControllerFinished(UPGroupModelController sender)
        {
            if (sender.ControllerState == GroupModelControllerState.Error)
            {
                this.portfolioGroup = null;
            }

            this.InformAboutDidChangeTopLevelElement(this.Page, this.Page, null, null);
        }

        private void SurveyLoaded()
        {
            var parentController = this.ParentOrganizerModelController as QuestionnaireEditOrganizerModelController;
            parentController?.SetSurveyName(this.Survey.Questionnaire.Label);

            if (this.ConfirmedFilter != null && !string.IsNullOrEmpty(this.Survey.AnswerRootRecordIdentification))
            {
                this.crmQuery = new UPContainerMetaInfo(null, this.ConfirmedFilter.InfoAreaId);
                this.crmQuery.ApplyFilter(this.ConfirmedFilter);
                this.crmQuery.SetLinkRecordIdentification(this.Survey.AnswerRootRecordIdentification);
                this.crmQuery.Find(this.Survey.DestinationRequestOption, this);
            }
            else
            {
                this.LoadFinished();
            }
        }

        private void LoadFinished()
        {
            this.FillPage(this.Page);
            this.InformAboutDidChangeTopLevelElement(this.Page, this.Page, null, null);
        }
    }
}
