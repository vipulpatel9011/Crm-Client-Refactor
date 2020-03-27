// <copyright file="CrmRecordParticipants.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The UPCRMRecordParticipants class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    using System;
    using Configuration;
    using Delegates;
    using OperationHandling;
    using Query;

    public partial class Constants
    {
        /// <summary>
        /// Rep acceptance function name rep identifier
        /// </summary>
        public const string UPRepAcceptanceFunctionName_RepId = "RepId";

        /// <summary>
        /// Rep acceptance function name acceptance
        /// </summary>
        public const string UPRepAcceptanceFunctionName_Acceptance = "Acceptance";
    }

    /// <summary>
    /// The upcrm record participants.
    /// </summary>
    public class UPCRMRecordParticipants : UPCRMMutableParticipants
    {
        /// <summary>
        /// The CRM query
        /// </summary>
        protected UPContainerMetaInfo crmQuery;

        /// <summary>
        /// Gets or sets the link participants request option.
        /// </summary>
        /// <value>
        /// The link participants request option.
        /// </value>
        public UPRequestOption LinkParticipantsRequestOption { get; set; }

        /// <summary>
        /// Gets the name of the link participants search and list.
        /// </summary>
        /// <value>
        /// The name of the link participants search and list.
        /// </value>
        public string LinkParticipantsSearchAndListName { get; private set; }

        /// <summary>
        /// Gets or sets the protected link record identification.
        /// </summary>
        /// <value>
        /// The protected link record identification.
        /// </value>
        public string ProtectedLinkRecordIdentification { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMRecordParticipants"/> class.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPCRMRecordParticipants(string recordIdentification, UPCRMParticipantsDelegate theDelegate)
            : this(null, null, null, -1, recordIdentification, theDelegate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMRecordParticipants" /> class.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="linkParticipantsSearchAndListName">Name of the link participants search and list.</param>
        /// <param name="linkParticipantsLinkId">The link participants link identifier.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPCRMRecordParticipants(string recordIdentification, string linkParticipantsSearchAndListName,
            int linkParticipantsLinkId, UPCRMParticipantsDelegate theDelegate)
            : this(null, null, linkParticipantsSearchAndListName, linkParticipantsLinkId, recordIdentification, theDelegate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMRecordParticipants"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="rootInfoAreaId">The root information area identifier.</param>
        /// <param name="linkParticipantsSearchAndListName">Name of the link participants search and list.</param>
        /// <param name="linkParticipantsLinkId">The link participants link identifier.</param>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPCRMRecordParticipants(ViewReference viewReference, string rootInfoAreaId, string linkParticipantsSearchAndListName,
            int linkParticipantsLinkId, string recordIdentification, UPCRMParticipantsDelegate theDelegate)
            : base(viewReference, rootInfoAreaId, recordIdentification, null, linkParticipantsLinkId, theDelegate)
        {

            this.LinkParticipantsRequestOption = UPCRMDataStore.RequestOptionFromString(
                    this.ViewReference?.ContextValueForKey("LinkParticipantsRequestOption"), UPRequestOption.FastestAvailable);

            this.LinkParticipantsSearchAndListName = linkParticipantsSearchAndListName;
        }

        /// <summary>
        /// Additionals the load steps.
        /// </summary>
        /// <returns></returns>
        protected override bool AdditionalLoadSteps()
        {
            if (string.IsNullOrEmpty(this.LinkParticipantsSearchAndListName))
            {
                return false;
            }

            if (this.AcceptanceField == null && this.RequirementField == null)
            {
                this.SetFieldsFromSearchAndListConfigurationName(this.LinkParticipantsSearchAndListName);
            }

            this.crmQuery = new UPContainerMetaInfo(this.LinkParticipantsSearchAndListName);
            if (this.crmQuery == null)
            {
                return false;
            }

            this.crmQuery.SetLinkRecordIdentification(this.RecordIdentification, this.LinkParticipantsLinkId);
            this.crmQuery.Find(this.LinkParticipantsRequestOption, this);

            return true;
        }

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public override void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            if (this.crmQuery == null)
            {
                this.Finished(error);
            }
            else
            {
                base.SearchOperationDidFailWithError(operation, error);
            }
        }

        /// <summary>
        /// Searches the operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="result">The result.</param>
        public override void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            if (this.crmQuery != null)
            {
                int count = result.RowCount;
                int infoAreaCount = result.MetaInfo.NumberOfResultInfoAreaMetaInfos();

                for (int i = 0; i < count; i++)
                {
                    UPCRMResultRow resultRow = (UPCRMResultRow)result.ResultRowAtIndex(i);

                    for (int j = 1; j < infoAreaCount; j++)
                    {
                        string linkRecordIdentification = resultRow.RecordIdentificationAtIndex(j);
                        if (linkRecordIdentification?.Length > 6)
                        {
                            UPCRMLinkParticipant linkParticipant = new UPCRMLinkParticipant(resultRow, j);
                            this.AddLinkParticipant(linkParticipant);

                            if (!string.IsNullOrEmpty(this.ProtectedLinkRecordIdentification) && this.ProtectedLinkRecordIdentification == linkParticipant.LinkRecordIdentification)
                            {
                                linkParticipant.MayNotBeDeleted = true;
                            }

                            break;
                        }
                        else
                        {
                            UPContainerInfoAreaMetaInfo iaMeta = result.MetaInfo.ResultInfoAreaMetaInfoAtIndex(j);
                            UPCRMLinkReader _linkReader = new UPCRMLinkReader(resultRow.RootRecordIdentification, iaMeta.InfoAreaIdWithLink, null);
                            string linkRecordId = _linkReader.RequestLinkRecordOffline();

                            if (!string.IsNullOrEmpty(linkRecordId) && UPCRMDataStore.DefaultStore.RecordExistsOffline(linkRecordId))
                            {
                                UPCRMLinkParticipant linkParticipant = new UPCRMLinkParticipant(resultRow, linkRecordId);
                                this.AddLinkParticipant(linkParticipant);
                                break;
                            }
                        }
                    }
                }

                this.Finished(null);
            }
            else
            {
                base.SearchOperationDidFinishWithResult(operation, result);
            }
        }
    }
}
