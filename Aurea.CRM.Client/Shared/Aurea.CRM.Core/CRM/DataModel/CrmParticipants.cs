// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmParticipants.cs" company="Aurea Software Gmbh">
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
//   The CRM Participants class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Catalogs;
    using Configuration;
    using Delegates;
    using Extensions;
    using OperationHandling;
    using Query;

    /// <summary>
    /// Participant class
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.ISearchOperationHandler" />
    public class UPCRMParticipants : ISearchOperationHandler
    {
        private readonly string linkParticipantsInfoAreaId;
        private readonly int linkParticipantsLinkId;
        private readonly bool initializedWithString;
        private List<UPCRMRepParticipant> repParticipants;
        private UPCRMField acceptanceField;
        private UPCRMField requirementField;
        private UPContainerMetaInfo repAcceptanceCrmQuery;

        /// <summary>
        /// Gets the link participants.
        /// </summary>
        /// <value>
        /// The link participants.
        /// </value>
        public virtual List<UPCRMLinkParticipant> LinkParticipants { get; private set; }

        /// <summary>
        /// Gets the rep participants.
        /// </summary>
        /// <value>
        /// The rep participants.
        /// </value>
        public virtual List<UPCRMRepParticipant> RepParticipants => this.repParticipants ??
                                                            (this.repParticipants = this.ParticipantsFromString(this.OriginalRepParticipantString));

        /// <summary>
        /// Gets the participants.
        /// </summary>
        /// <value>
        /// The participants.
        /// </value>
        public List<UPCRMParticipant> Participants
        {
            get
            {
                if (this.RepParticipants?.Count == 0)
                {
                    return this.LinkParticipants.Select(x => x as UPCRMParticipant).ToList();
                }

                if (this.LinkParticipants?.Count == 0)
                {
                    return this.RepParticipants.Select(x => x as UPCRMParticipant).ToList();
                }

                var combined = new List<UPCRMParticipant>();

                if (this.repParticipants != null)
                {
                    combined.AddRange(this.repParticipants);
                }

                if (this.LinkParticipants != null)
                {
                    combined.AddRange(this.LinkParticipants);
                }

                return combined;
            }
        }

        /// <summary>
        /// Gets the original rep participant string.
        /// </summary>
        /// <value>
        /// The original rep participant string.
        /// </value>
        public string OriginalRepParticipantString { get; private set; }

        /// <summary>
        /// Gets the participant string.
        /// </summary>
        /// <value>
        /// The participant string.
        /// </value>
        public virtual string ParticipantString
        {
            get
            {
                if (this.initializedWithString || this.OriginalRepParticipantString != null)
                {
                    return this.OriginalRepParticipantString;
                }

                this.OriginalRepParticipantString = this.StringFromParticipants(this.RepParticipants);

                return this.OriginalRepParticipantString;
            }
        }

        /// <summary>
        /// Gets or sets the acceptance field.
        /// </summary>
        /// <value>
        /// The acceptance field.
        /// </value>
        public UPCRMField AcceptanceField
        {
            get { return this.acceptanceField; }

            set
            {
                this.acceptanceField = value;
                this.AcceptanceCatalog = value != null ? UPCRMDataStore.DefaultStore.CatalogForCrmField(value) : null;
            }
        }

        /// <summary>
        /// Gets or sets the requirement field.
        /// </summary>
        /// <value>
        /// The requirement field.
        /// </value>
        public UPCRMField RequirementField
        {
            get { return this.requirementField; }

            set
            {
                this.requirementField = value;
                this.RequirementCatalog = value != null ? UPCRMDataStore.DefaultStore.CatalogForCrmField(value) : null;
            }
        }

        /// <summary>
        /// Gets the view reference.
        /// </summary>
        /// <value>
        /// The view reference.
        /// </value>
        public ViewReference ViewReference { get; private set; }

        /// <summary>
        /// Gets the acceptance catalog.
        /// </summary>
        /// <value>
        /// The acceptance catalog.
        /// </value>
        public UPCatalog AcceptanceCatalog { get; private set; }

        /// <summary>
        /// Gets the requirement catalog.
        /// </summary>
        /// <value>
        /// The requirement catalog.
        /// </value>
        public UPCatalog RequirementCatalog { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has rep acceptance.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has rep acceptance; otherwise, <c>false</c>.
        /// </value>
        public bool HasRepAcceptance => !string.IsNullOrEmpty(this.RepAcceptanceSearchAndListName);

        /// <summary>
        /// Gets the name of the rep acceptance search and list.
        /// </summary>
        /// <value>
        /// The name of the rep acceptance search and list.
        /// </value>
        public string RepAcceptanceSearchAndListName => this.ViewReference?.ContextValueForKey(@"RepAcceptanceConfigName");

        /// <summary>
        /// Gets the rep acceptance information area identifier.
        /// </summary>
        /// <value>
        /// The rep acceptance information area identifier.
        /// </value>
        public string RepAcceptanceInfoAreaId { get; private set; }

        /// <summary>
        /// Gets the rep acceptance link identifier.
        /// </summary>
        /// <value>
        /// The rep acceptance link identifier.
        /// </value>
        public int RepAcceptanceLinkId
        {
            get
            {
                if (this.ViewReference != null)
                {
                    int linkId = Convert.ToInt32(this.ViewReference.ContextValueForKey("RepAcceptanceLinkId"));
                    if (linkId > 0)
                    {
                        return linkId;
                    }
                }

                return -1;
            }
        }

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get; private set; }

        /// <summary>
        /// Gets the root information area identifier.
        /// </summary>
        /// <value>
        /// The root information area identifier.
        /// </value>
        public string RootInfoAreaId { get; private set; }

        /// <summary>
        /// Gets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public UPCRMParticipantsDelegate TheDelegate { get; private set; }

        /// <summary>
        /// Gets or sets the rep participants request option.
        /// </summary>
        /// <value>
        /// The rep participants request option.
        /// </value>
        public UPRequestOption RepParticipantsRequestOption { get; set; }

        /// <summary>
        /// Gets the link participants information area identifier.
        /// </summary>
        /// <value>
        /// The link participants information area identifier.
        /// </value>
        public string LinkParticipantsInfoAreaId
        {
            get
            {
                if (!string.IsNullOrEmpty(this.linkParticipantsInfoAreaId))
                {
                    return this.linkParticipantsInfoAreaId;
                }

                string linkParticipantsInfoAreaId = this.ViewReference?.ContextValueForKey("LinkParticipantsInfoAreaId");

                if (!string.IsNullOrEmpty(linkParticipantsInfoAreaId))
                {
                    return linkParticipantsInfoAreaId;
                }

                string searchAndListName = this.ViewReference?.ContextValueForKey("LinkParticipantsSearchAndListName");

                if (!string.IsNullOrEmpty(searchAndListName))
                {
                    SearchAndList searchAndList = ConfigurationUnitStore.DefaultStore.SearchAndListByName(searchAndListName);
                    return searchAndList.InfoAreaId;
                }

                return "MB";
            }
        }

        /// <summary>
        /// Gets the link participants link identifier.
        /// </summary>
        /// <value>
        /// The link participants link identifier.
        /// </value>
        public int LinkParticipantsLinkId
        {
            get
            {
                if (!string.IsNullOrEmpty(this.LinkParticipantsInfoAreaId))
                {
                    return this.linkParticipantsLinkId;
                }

                string linkParticipantsLinkId = this.ViewReference.ContextValueForKey("LinkParticipantsLinkId");

                if (!string.IsNullOrEmpty(linkParticipantsLinkId))
                {
                    return Convert.ToInt32(linkParticipantsLinkId);
                }

                return -1;
            }
        }

        /// <summary>
        /// Gets the acceptance field identifier.
        /// </summary>
        /// <value>
        /// The acceptance field identifier.
        /// </value>
        public int AcceptanceFieldId { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMParticipants"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="rootInfoAreaId">The root information area identifier.</param>
        /// <param name="linkParticipantsInfoAreaId">The link participants information area identifier.</param>
        /// <param name="linkParticipantsLinkId">The link participants link identifier.</param>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPCRMParticipants(ViewReference viewReference, string rootInfoAreaId, string linkParticipantsInfoAreaId, int linkParticipantsLinkId, string recordIdentification, UPCRMParticipantsDelegate theDelegate)
        {
            this.RecordIdentification = recordIdentification;
            this.TheDelegate = theDelegate;

            this.RootInfoAreaId = rootInfoAreaId ?? recordIdentification.InfoAreaId();

            if (string.IsNullOrEmpty(this.RootInfoAreaId))
            {
                this.RootInfoAreaId = "MA";
            }

            this.linkParticipantsInfoAreaId = linkParticipantsInfoAreaId;
            this.linkParticipantsLinkId = linkParticipantsLinkId;

            if (viewReference == null)
            {
                Menu menu = ConfigurationUnitStore.DefaultStore.MenuByName($"Configuration:{this.RootInfoAreaId}Participants");
                this.ViewReference = menu?.ViewReference;
            }
            else
            {
                this.ViewReference = viewReference;
            }

            this.RepParticipantsRequestOption = UPCRMDataStore.RequestOptionFromString(this.ViewReference?.ContextValueForKey("RepAcceptanceRequestOption"), UPRequestOption.FastestAvailable);
            this.AcceptanceFieldId = -1;
            string configName = this.ViewReference?.ContextValueForKey("RepAcceptanceConfigName");

            if (!string.IsNullOrEmpty(configName))
            {
                IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
                SearchAndList searchAndList = configStore.SearchAndListByName(configName);

                if (!string.IsNullOrEmpty(searchAndList.FieldGroupName))
                {
                    configName = searchAndList.FieldGroupName;
                }

                FieldControl fieldControl = configStore.FieldControlByNameFromGroup("Edit", configName) ??
                                            configStore.FieldControlByNameFromGroup("List", configName);

                if (fieldControl != null)
                {
                    UPConfigFieldControlField field = fieldControl.FieldWithFunction(Constants.UPRepAcceptanceFunctionName_Acceptance);
                    if (field != null)
                    {
                        this.AcceptanceFieldId = field.FieldId;
                    }

                    this.RepAcceptanceInfoAreaId = fieldControl.InfoAreaId;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMParticipants"/> class.
        /// </summary>
        /// <param name="participantString">The participant string.</param>
        public UPCRMParticipants(string participantString)
        {
            this.OriginalRepParticipantString = participantString;
            this.initializedWithString = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMParticipants"/> class.
        /// </summary>
        public UPCRMParticipants()
            : this(null, null, null, -1, null, null)
        {
        }

        /// <summary>
        /// Texts for acceptance code string.
        /// </summary>
        /// <param name="acceptanceCode">The acceptance code.</param>
        /// <returns></returns>
        public string TextForAcceptanceCodeString(string acceptanceCode)
        {
            return this.AcceptanceCatalog?.TextValueForKey(acceptanceCode);
        }

        /// <summary>
        /// Texts for requirement code string.
        /// </summary>
        /// <param name="requirementCode">The requirement code.</param>
        /// <returns></returns>
        public string TextForRequirementCodeString(string requirementCode)
        {
            return this.RequirementCatalog?.TextValueForKey(requirementCode);
        }

        /// <summary>
        /// Sets the name of the fields from search and list configuration.
        /// </summary>
        /// <param name="searchAndListConfigurationName">Name of the search and list configuration.</param>
        /// <returns></returns>
        public bool SetFieldsFromSearchAndListConfigurationName(string searchAndListConfigurationName)
        {
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            SearchAndList searchAndList = configStore.SearchAndListByName(searchAndListConfigurationName);
            if (searchAndList != null)
            {
                FieldControl fieldControl = configStore.FieldControlByNameFromGroup("List", searchAndList.FieldGroupName);
                if (fieldControl != null)
                {
                    this.SetFieldsFromFieldControl(fieldControl);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Sets the fields from field control.
        /// </summary>
        /// <param name="fieldControl">The field control.</param>
        public void SetFieldsFromFieldControl(FieldControl fieldControl)
        {
            UPConfigFieldControlField field = fieldControl.FieldWithFunction("Requirement");
            if (field != null)
            {
                this.RequirementField = field.Field;
            }

            field = fieldControl.FieldWithFunction("Acceptance");

            if (field != null)
            {
                this.AcceptanceField = field.Field;
            }
        }

        /// <summary>
        /// Participants the with key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public UPCRMParticipant ParticipantWithKey(string key)
        {
            foreach (var participant in this.RepParticipants)
            {
                if (participant.Key == key)
                {
                    return participant;
                }
            }

            return this.LinkParticipants.FirstOrDefault(participant => participant.Key == key);
        }

        /// <summary>
        /// Firsts the link participant.
        /// </summary>
        /// <returns></returns>
        public UPCRMLinkParticipant FirstLinkParticipant()
        {
            return this.LinkParticipants.Count > 0 ? this.LinkParticipants[0] : null;
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        public void Load()
        {
            if (this.RepParticipants.Count > 0 && !string.IsNullOrEmpty(this.RepAcceptanceSearchAndListName) && !string.IsNullOrEmpty(this.RecordIdentification))
            {
                this.repAcceptanceCrmQuery = new UPContainerMetaInfo(this.RepAcceptanceSearchAndListName, null, new List<object> { "List", "Edit", null });
                this.repAcceptanceCrmQuery.SetLinkRecordIdentification(this.RecordIdentification, this.RepAcceptanceLinkId);
                this.repAcceptanceCrmQuery.Find(this.RepParticipantsRequestOption, this);
            }
            else
            {
                this.Loaded();
            }
        }

        private void Loaded()
        {
            if (!this.AdditionalLoadSteps())
            {
                this.Finished(null);
            }
        }

        /// <summary>
        /// Additionals the load steps.
        /// </summary>
        /// <returns></returns>
        protected virtual bool AdditionalLoadSteps()
        {
            return false;
        }

        /// <summary>
        /// Finisheds the specified error.
        /// </summary>
        /// <param name="error">The error.</param>
        protected void Finished(Exception error)
        {
            if (error != null)
            {
                this.TheDelegate.CrmParticipantsDidFailWithError(this, error);
            }
            else
            {
                this.TheDelegate.CrmParticipantsDidFinishWithResult(this, this);
            }
        }

        /// <summary>
        /// Strings from participants.
        /// </summary>
        /// <param name="participants">The participants.</param>
        /// <returns></returns>
        protected string StringFromParticipants(List<UPCRMRepParticipant> participants)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var participant in participants)
            {
                if (participant == null || participant.MarkAsDeleted)
                {
                    continue;
                }

                sb.AppendFormat("{0};", participant.ParticipantString);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Participantses from string.
        /// </summary>
        /// <param name="dataString">The data string.</param>
        /// <returns></returns>
        protected List<UPCRMRepParticipant> ParticipantsFromString(string dataString)
        {
            if (dataString == null)
            {
                return new List<UPCRMRepParticipant>();
            }

            var participantStrings = dataString.Split(';');
            var participantArray = new List<UPCRMRepParticipant>(participantStrings.Length);

            foreach (var participantString in participantStrings)
            {
                if (!string.IsNullOrEmpty(participantString))
                {
                    participantArray.Add(new UPCRMRepParticipant(participantString) { Context = this });
                }
            }

            return participantArray;
        }

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public virtual void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.repAcceptanceCrmQuery = null;
            this.Finished(error);
        }

        /// <summary>
        /// Searches the operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="result">The result.</param>
        public virtual void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            int count = result.RowCount;

            for (int i = 0; i < count; i++)
            {
                UPCRMResultRow resultRow = (UPCRMResultRow)result.ResultRowAtIndex(i);
                var rowValues = resultRow.ValuesWithFunctions();

                string rep = (string)rowValues.ValueOrDefault(Constants.UPRepAcceptanceFunctionName_RepId);
                int repId = rep.ToInt();

                if (repId > 0)
                {
                    rep = StringExtensions.NineDigitStringFromRep(Convert.ToInt32(rep));

                    var repParticipant = this.ParticipantWithKey(rep);

                    if (repParticipant is UPCRMRepParticipant)
                    {
                        string acceptance = (string)rowValues[Constants.UPRepAcceptanceFunctionName_Acceptance];
                        ((UPCRMRepParticipant)repParticipant).AcceptanceFromRecordIdentification(acceptance, resultRow.RootRecordIdentification);
                    }
                }
            }

            this.repAcceptanceCrmQuery = null;
            this.Loaded();
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
    }
}
