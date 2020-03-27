// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmLinkParticipant.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//   Max Menezes
// </author>
// <summary>
//   Query configurations
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    using Configuration;
    using Extensions;
    using Query;

    public class UPCRMLinkParticipant : UPCRMParticipant
    {
        /// <summary>
        /// The name
        /// </summary>
        protected string _name;

        /// <summary>
        /// The create record
        /// </summary>
        protected bool _createRecord;

        /// <summary>
        /// The delete record
        /// </summary>
        protected bool _deleteRecord;

        /// <summary>
        /// Gets a value indicating whether this instance is offline empty participant.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is offline empty participant; otherwise, <c>false</c>.
        /// </value>
        public bool IsOfflineEmptyParticipant { get; private set; }

        /// <summary>
        /// Gets the link record identification.
        /// </summary>
        /// <value>
        /// The link record identification.
        /// </value>
        public string LinkRecordIdentification { get; private set; }

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is new participant.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is new participant; otherwise, <c>false</c>.
        /// </value>
        public bool IsNewParticipant { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMLinkParticipant"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        public UPCRMLinkParticipant(string key)
            : base(key)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMLinkParticipant"/> class.
        /// </summary>
        /// <param name="infoAreaId">The information area identifier.</param>
        /// <param name="linkRecordIdentification">The link record identification.</param>
        /// <param name="name">The name.</param>
        public UPCRMLinkParticipant(string infoAreaId, string linkRecordIdentification, string name)
            : base(linkRecordIdentification)
        {
            this._createRecord = true;
            this.LinkRecordIdentification = linkRecordIdentification;
            this.RecordIdentification = infoAreaId;
            this.IsNewParticipant = true;
            this._name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMLinkParticipant"/> class.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        /// <param name="index">The index.</param>
        public UPCRMLinkParticipant(UPCRMResultRow resultRow, int index)
            : base(resultRow.RootRecordIdentification)
        {
            this.RecordIdentification = resultRow.RootRecordIdentification;
            this.LinkRecordIdentification = resultRow.RecordIdentificationAtIndex(index);
            this.IsNewParticipant = false;

            var functionNameValues = resultRow.ValuesWithFunctions();
            this.RequirementText = (string)functionNameValues["Requirement"];
            this.AcceptanceText = (string)functionNameValues["Acceptance"];
            this.OriginalAcceptanceText = this.AcceptanceText;

            UPContainerInfoAreaMetaInfo infoAreaMetaInfo = resultRow.Result.MetaInfo.ResultInfoAreaMetaInfoAtIndex(index);
            this._name = TableCaptionForInfoAreaIdResultRow(this.LinkRecordIdentification.InfoAreaId(), resultRow);

            if (string.IsNullOrEmpty(this._name))
            {
                this._name = string.Empty;
                foreach (UPContainerFieldMetaInfo field in infoAreaMetaInfo.Fields)
                {
                    string val = resultRow.ValueForField(field);
                    if (!string.IsNullOrEmpty(val))
                    {
                        this._name = this._name.Length > 0 ? $"{this._name} {val}" : val;
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMLinkParticipant"/> class.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        /// <param name="linkRecordIdentification">The link record identification.</param>
        public UPCRMLinkParticipant(UPCRMResultRow resultRow, string linkRecordIdentification)
            : base(resultRow.RootRecordIdentification)
        {
            this.RecordIdentification = resultRow.RootRecordIdentification;
            this.LinkRecordIdentification = linkRecordIdentification;
            this.IsOfflineEmptyParticipant = true;

            var functionNameValues = resultRow.ValuesWithFunctions();
            this.RequirementText = (string)functionNameValues["Requirement"];
            this.AcceptanceText = (string)functionNameValues["Acceptance"];
            this.OriginalAcceptanceText = this.AcceptanceText;

            this._name = LocalizedString.TextOfflineNotAvailable;
        }

        /// <summary>
        /// Tables the caption for information area identifier result row.
        /// </summary>
        /// <param name="infoAreaId">The information area identifier.</param>
        /// <param name="resultRow">The result row.</param>
        /// <returns></returns>
        public static string TableCaptionForInfoAreaIdResultRow(string infoAreaId, UPCRMResultRow resultRow)
        {
            var store = ConfigurationUnitStore.DefaultStore;
            UPConfigTableCaption tableCaption = store.TableCaptionByName($"{infoAreaId}Part") ??
                                                store.TableCaptionByName(infoAreaId);

            return tableCaption != null ? tableCaption.TableCaptionForResultRow(resultRow) : string.Empty;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public override string Name => this._name;

        /// <summary>
        /// Changes the name of the rep.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public bool ChangeRepName(string recordIdentification, string name)
        {
            this.LinkRecordIdentification = recordIdentification;
            this._name = name;
            return true;
        }

        /// <summary>
        /// Gets a value indicating whether this instance can change acceptance state.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can change acceptance state; otherwise, <c>false</c>.
        /// </value>
        public override bool CanChangeAcceptanceState => true;

        /// <summary>
        /// Gets a value indicating whether this <see cref="UPCRMParticipant" /> is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if empty; otherwise, <c>false</c>.
        /// </value>
        public override bool Empty => string.IsNullOrEmpty(this.LinkRecordIdentification);
    }
}
