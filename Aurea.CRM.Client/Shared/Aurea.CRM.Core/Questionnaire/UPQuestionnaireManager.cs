// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPQuestionnaireManager.cs" company="Aurea Software Gmbh">
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
//   The Questionnaire Manager
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Questionnaire
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Session;

    /// <summary>
    /// Constants
    /// </summary>
    public partial class Constants
    {
        /// <summary>
        /// The questionnaire identifier
        /// </summary>
        public const string QuestionnaireQuestionnaireID = "QuestionnaireID";

        /// <summary>
        /// The questionnaire label
        /// </summary>
        public const string QuestionnaireLabel = "Label";

        /// <summary>
        /// The questionnaire number
        /// </summary>
        public const string QuestionnaireQuestionNumber = "QuestionNumber";

        /// <summary>
        /// The questionnaire multiple
        /// </summary>
        public const string QuestionnaireMultiple = "Multiple";

        /// <summary>
        /// The questionnaire information area identifier
        /// </summary>
        public const string QuestionnaireInfoAreaId = "InfoAreaId";

        /// <summary>
        /// The questionnaire field identifier
        /// </summary>
        public const string QuestionnaireFieldId = "FieldId";

        /// <summary>
        /// The questionnaire follow up number
        /// </summary>
        public const string QuestionnaireFollowUpNumber = "FollowUpNumber";

        /// <summary>
        /// The questionnaire new section
        /// </summary>
        public const string QuestionnaireNewSection = "NewSection";

        /// <summary>
        /// The questionnaire mandatory
        /// </summary>
        public const string QuestionnaireMandatory = "Mandatory";

        /// <summary>
        /// The questionnaire default
        /// </summary>
        public const string QuestionnaireDefault = "Default";

        /// <summary>
        /// The questionnaire read
        /// </summary>
        public const string QuestionnaireRead = "Read";

        /// <summary>
        /// The questionnaire save
        /// </summary>
        public const string QuestionnaireSave = "Save";

        /// <summary>
        /// The questionnaire answer number
        /// </summary>
        public const string QuestionnaireAnswerNumber = "AnswerNumber";

        /// <summary>
        /// The questionnaire answer
        /// </summary>
        public const string QuestionnaireAnswer = "Answer";

        /// <summary>
        /// The questionnaire additional information
        /// </summary>
        public const string QuestionnaireAdditionalInfo = "AdditionalInfo";

        /// <summary>
        /// The questionnaire hide
        /// </summary>
        public const string QuestionnaireHide = "Hide";
    }

    /// <summary>
    /// Questionnaire Manager
    /// </summary>
    public class UPQuestionnaireManager
    {
        private Dictionary<int, UPQuestionnaire> cacheByCode;
        private Dictionary<string, UPQuestionnaire> cacheByRecordIdentification;
        private List<UPQuestionnaire> pendingQuestionnaires;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPQuestionnaireManager"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public UPQuestionnaireManager(ViewReference configuration)
        {
            this.Configuration = configuration;
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            string configValue = this.Configuration.ContextValueForKey("QuestionnaireSL");
            this.QuestionnaireSearchAndList = configStore.SearchAndListByName(configValue);
            this.QuestionnaireList = configStore.FieldControlByNameFromGroup("List", this.QuestionnaireSearchAndList.FieldGroupName);
            if (this.QuestionnaireList == null)
            {
                throw new InvalidOperationException("QuestionnaireList is null");
            }

            configValue = this.Configuration.ContextValueForKey("QuestionsSL");
            this.QuestionSearchAndList = configStore.SearchAndListByName(configValue);
            this.QuestionList = configStore.FieldControlByNameFromGroup("List", this.QuestionSearchAndList.FieldGroupName);
            if (this.QuestionList == null)
            {
                throw new InvalidOperationException("QuestionList is null");
            }

            configValue = this.Configuration.ContextValueForKey("QuestionAnswersSL");
            this.QuestionAnswersSearchAndList = configStore.SearchAndListByName(configValue);
            this.QuestionAnswersList = configStore.FieldControlByNameFromGroup("List", this.QuestionAnswersSearchAndList.FieldGroupName);
            this.cacheByRecordIdentification = new Dictionary<string, UPQuestionnaire>();
            this.cacheByCode = new Dictionary<int, UPQuestionnaire>();
            this.pendingQuestionnaires = new List<UPQuestionnaire>();
            this.QuestionnaireRequestOption = UPRequestOption.FastestAvailable;
            if (ServerSession.CurrentSession.IsUpdateCrm)
            {
                this.MultipleAnswersForCatalogs = false;
                this.SaveCatalogValuesAsText = true;
                this.LinkAnswerToQuestionnaire = false;
                this.LinkAnswerToQuestion = true;
                this.LinkAnswerToQuestionAnswer = true;
                this.LinkSurveyToQuestionnaire = false;
                this.DeleteSingleAnswerOptionIfEmpty = true;
                this.DeleteAndInsertOnAnswerOptionChange = true;
            }
            else
            {
                this.MultipleAnswersForCatalogs = false;
                this.SaveCatalogValuesAsText = true;
                this.LinkAnswerToQuestionnaire = false;
                this.LinkAnswerToQuestion = false;
                this.LinkAnswerToQuestionAnswer = false;
                this.LinkSurveyToQuestionnaire = false;
                this.DeleteSingleAnswerOptionIfEmpty = false;
                this.DeleteAndInsertOnAnswerOptionChange = false;
            }

            string optionString = this.Configuration.ContextValueForKey("Options");
            Dictionary<string, object> options = optionString.JsonDictionaryFromString();
            if (options?.Count > 0)
            {
                object param = options.ValueOrDefault("MultipleCatalogAnswers");
                if (param != null)
                {
                    this.MultipleAnswersForCatalogs = Convert.ToInt32(param) != 0;
                }

                param = options.ValueOrDefault("SaveCatalogValuesAsText");
                if (param != null)
                {
                    this.SaveCatalogValuesAsText = Convert.ToInt32(param) != 0;
                }

                param = options.ValueOrDefault("LinkAnswerToRoot");
                if (param != null)
                {
                    this.LinkAnswerToQuestionnaire = Convert.ToInt32(param) != 0;
                }

                param = options.ValueOrDefault("LinkAnswerToQuestion");
                if (param != null)
                {
                    this.LinkAnswerToQuestion = Convert.ToInt32(param) != 0;
                }

                param = options.ValueOrDefault("LinkAnswerToAnswer");
                if (param != null)
                {
                    this.LinkAnswerToQuestionAnswer = Convert.ToInt32(param) != 0;
                }

                param = options.ValueOrDefault("LinkSurveyToRoot");
                if (param != null)
                {
                    this.LinkSurveyToQuestionnaire = Convert.ToInt32(param) != 0;
                }

                param = options.ValueOrDefault("DeleteSingleAnswerOptionIfEmpty");
                if (param != null)
                {
                    this.DeleteSingleAnswerOptionIfEmpty = Convert.ToInt32(param) != 0;
                }

                param = options.ValueOrDefault("DeleteAndInsertOnAnswerOptionChange");
                if (param != null)
                {
                    this.DeleteAndInsertOnAnswerOptionChange = Convert.ToInt32(param) != 0;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPQuestionnaireManager"/> class.
        /// </summary>
        public UPQuestionnaireManager()
            : this(ConfigurationUnitStore.DefaultStore.MenuByName("QuestionnaireConfiguration").ViewReference)
        {
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public ViewReference Configuration { get; }

        /// <summary>
        /// Gets the questionnaire search and list.
        /// </summary>
        /// <value>
        /// The questionnaire search and list.
        /// </value>
        public SearchAndList QuestionnaireSearchAndList { get; } // F1

        /// <summary>
        /// Gets the question answers search and list.
        /// </summary>
        /// <value>
        /// The question answers search and list.
        /// </value>
        public SearchAndList QuestionAnswersSearchAndList { get; } // F2

        /// <summary>
        /// Gets the question search and list.
        /// </summary>
        /// <value>
        /// The question search and list.
        /// </value>
        public SearchAndList QuestionSearchAndList { get; } // F3

        /// <summary>
        /// Gets the questionnaire list.
        /// </summary>
        /// <value>
        /// The questionnaire list.
        /// </value>
        public FieldControl QuestionnaireList { get; } // U1

        /// <summary>
        /// Gets the question answers list.
        /// </summary>
        /// <value>
        /// The question answers list.
        /// </value>
        public FieldControl QuestionAnswersList { get; private set; } // U2

        /// <summary>
        /// Gets the question list.
        /// </summary>
        /// <value>
        /// The question list.
        /// </value>
        public FieldControl QuestionList { get; }

        /// <summary>
        /// Gets the survey edit.
        /// </summary>
        /// <value>
        /// The survey edit.
        /// </value>
        public FieldControl SurveyEdit { get; private set; }

        /// <summary>
        /// Gets the survey result edit.
        /// </summary>
        /// <value>
        /// The survey result edit.
        /// </value>
        public FieldControl SurveyResultEdit { get; private set; }

        /// <summary>
        /// Gets the questionnaire request option.
        /// </summary>
        /// <value>
        /// The questionnaire request option.
        /// </value>
        public UPRequestOption QuestionnaireRequestOption { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [multiple answers for catalogs].
        /// </summary>
        /// <value>
        /// <c>true</c> if [multiple answers for catalogs]; otherwise, <c>false</c>.
        /// </value>
        public bool MultipleAnswersForCatalogs { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [save catalog values as text].
        /// </summary>
        /// <value>
        /// <c>true</c> if [save catalog values as text]; otherwise, <c>false</c>.
        /// </value>
        public bool SaveCatalogValuesAsText { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [automatic online save of foreign fields].
        /// </summary>
        /// <value>
        /// <c>true</c> if [automatic online save of foreign fields]; otherwise, <c>false</c>.
        /// </value>
        public bool AutomaticOnlineSaveOfForeignFields { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [link answer to questionnaire].
        /// </summary>
        /// <value>
        /// <c>true</c> if [link answer to questionnaire]; otherwise, <c>false</c>.
        /// </value>
        public bool LinkAnswerToQuestionnaire { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [link answer to question].
        /// </summary>
        /// <value>
        /// <c>true</c> if [link answer to question]; otherwise, <c>false</c>.
        /// </value>
        public bool LinkAnswerToQuestion { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [link answer to question answer].
        /// </summary>
        /// <value>
        /// <c>true</c> if [link answer to question answer]; otherwise, <c>false</c>.
        /// </value>
        public bool LinkAnswerToQuestionAnswer { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [link survey to questionnaire].
        /// </summary>
        /// <value>
        /// <c>true</c> if [link survey to questionnaire]; otherwise, <c>false</c>.
        /// </value>
        public bool LinkSurveyToQuestionnaire { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [delete single answer option if empty].
        /// </summary>
        /// <value>
        /// <c>true</c> if [delete single answer option if empty]; otherwise, <c>false</c>.
        /// </value>
        public bool DeleteSingleAnswerOptionIfEmpty { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [delete and insert on answer option change].
        /// </summary>
        /// <value>
        /// <c>true</c> if [delete and insert on answer option change]; otherwise, <c>false</c>.
        /// </value>
        public bool DeleteAndInsertOnAnswerOptionChange { get; private set; }

        /// <summary>
        /// Adds the questionnaire to cache.
        /// </summary>
        /// <param name="questionnaire">The questionnaire.</param>
        public void AddQuestionnaireToCache(UPQuestionnaire questionnaire)
        {
            if (questionnaire.CatalogCode > 0)
            {
                this.cacheByCode[questionnaire.CatalogCode] = questionnaire;
            }

            if (!string.IsNullOrEmpty(questionnaire.RecordIdentification))
            {
                this.cacheByRecordIdentification[questionnaire.RecordIdentification] = questionnaire;
            }
        }

        /// <summary>
        /// Questionnaires the loaded error.
        /// </summary>
        /// <param name="questionnaire">The questionnaire.</param>
        /// <param name="error">The error.</param>
        public void QuestionnaireLoadedError(UPQuestionnaire questionnaire, Exception error)
        {
            if (error == null)
            {
                this.AddQuestionnaireToCache(questionnaire);
            }

            this.pendingQuestionnaires.Remove(questionnaire);
        }

        /// <summary>
        /// Loads the questionaire.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="theDelegate">The delegate.</param>
        /// <returns></returns>
        public UPQuestionnaire LoadQuestionaire(int code, IQuestionnaireDelegate theDelegate)
        {
            UPQuestionnaire quest = this.cacheByCode.ValueOrDefault(code);
            if (quest != null || theDelegate == null)
            {
                return quest;
            }

            quest = new UPQuestionnaire(code, this);
            this.pendingQuestionnaires.Add(quest);
            quest.Load(theDelegate);
            return null;
        }

        /// <summary>
        /// Loads the questionaire.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="theDelegate">The delegate.</param>
        /// <returns></returns>
        public UPQuestionnaire LoadQuestionaire(string recordIdentification, IQuestionnaireDelegate theDelegate)
        {
            UPQuestionnaire quest = this.cacheByRecordIdentification.ValueOrDefault(recordIdentification);
            if (quest != null || theDelegate == null)
            {
                return quest;
            }

            quest = new UPQuestionnaire(recordIdentification, this);
            this.pendingQuestionnaires.Add(quest);
            quest.Load(theDelegate);
            return null;
        }
    }
}
