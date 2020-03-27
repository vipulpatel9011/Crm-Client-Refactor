// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPChildEditContext.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The child edit fiels context
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Contexts
{
    using System.Collections.Generic;

    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.UIModel.Fields.Edit;
    using Aurea.CRM.UIModel.Groups;

    /// <summary>
    /// The child edit fiels context
    /// </summary>
    public class UPChildEditContext
    {
        /// <summary>
        /// The changed links.
        /// </summary>
        private Dictionary<string, UPCRMLink> changedLinks;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPChildEditContext"/> class.
        /// </summary>
        /// <param name="resultRow">
        /// The result row.
        /// </param>
        /// <param name="fieldLabelPostfix">
        /// The field label postfix.
        /// </param>
        public UPChildEditContext(UPCRMResultRow resultRow, string fieldLabelPostfix)
        {
            this.ResultRow = resultRow;
            this.RecordIdentification = resultRow?.RootRecordIdentification;
            this.EditFieldContext = new Dictionary<string, UPEditFieldContext>();
            this.DeleteRecord = false;
            this.FieldLabelPostfix = fieldLabelPostfix;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPChildEditContext"/> class.
        /// </summary>
        /// <param name="fieldLabelPostfix">
        /// The field label postfix.
        /// </param>
        public UPChildEditContext(string fieldLabelPostfix)
            : this(null, fieldLabelPostfix)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPChildEditContext"/> class.
        /// </summary>
        public UPChildEditContext()
            : this(null, null)
        {
        }

        /// <summary>
        /// Gets the changed link array.
        /// </summary>
        /// <value>
        /// The changed link array.
        /// </value>
        public Dictionary<string, UPCRMLink>.ValueCollection ChangedLinkArray => this.changedLinks?.Values;

        /// <summary>
        /// Gets or sets a value indicating whether [delete record].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [delete record]; otherwise, <c>false</c>.
        /// </value>
        public bool DeleteRecord { get; set; }

        /// <summary>
        /// Gets the edit field context.
        /// </summary>
        /// <value>
        /// The edit field context.
        /// </value>
        public Dictionary<string, UPEditFieldContext> EditFieldContext { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [explicit new].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [explicit new]; otherwise, <c>false</c>.
        /// </value>
        public bool ExplicitNew { get; private set; }

        /// <summary>
        /// Gets or sets the field label postfix.
        /// </summary>
        /// <value>
        /// The field label postfix.
        /// </value>
        public string FieldLabelPostfix { get; set; }

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        /// <value>
        /// The group.
        /// </value>
        public UPMGroup Group { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is new.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is new; otherwise, <c>false</c>.
        /// </value>
        public bool IsNew
            =>
                this.ExplicitNew
                || (!string.IsNullOrEmpty(this.RecordIdentification) && this.RecordIdentification.StartsWith("new"));

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get; private set; }

        /// <summary>
        /// Gets the result row.
        /// </summary>
        /// <value>
        /// The result row.
        /// </value>
        public UPCRMResultRow ResultRow { get; private set; }

        /// <summary>
        /// Adds the changed links from record parent link.
        /// </summary>
        /// <param name="record">
        /// The record.
        /// </param>
        /// <param name="parentLink">
        /// The parent link.
        /// </param>
        public void AddChangedLinksFromRecordParentLink(UPCRMRecord record, UPCRMLink parentLink)
        {
            // TODO
        }

        /// <summary>
        /// Contexts the record for sender information area identifier link identifier.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ContextRecordForSenderInfoAreaIdLinkId(object sender, string infoAreaId, int linkId)
        {
            var key = this.LinkKeyForInfoAreaIdLinkId(infoAreaId, linkId);
            var link = this.changedLinks.ValueOrDefault(key);
            if (link != null)
            {
                return link.RecordIdentification;
            }

            var linkRecordIdentification = this.ResultRow?.RecordIdentificationForLinkInfoAreaIdLinkId(
                infoAreaId,
                linkId);
            if (!string.IsNullOrEmpty(linkRecordIdentification) && linkRecordIdentification.Length > 10)
            {
                return linkRecordIdentification;
            }

            return UPSelector.StaticSelectorContextDelegate.SenderLinkForInfoAreaIdLinkId(this, infoAreaId, linkId);
        }

        /// <summary>
        /// Contexts the record for sender selector.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="selector">
        /// The selector.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ContextRecordForSenderSelector(object sender, UPRecordSelector selector)
        {
            for (var i = 0; i < selector.RecordLinkInfoAreaIds.Count; i++)
            {
                var linkInfoAreaId = selector.RecordLinkInfoAreaIds[i];
                var linkId = selector.RecordLinkLinkIds[i];
                var rid = this.ContextRecordForSenderInfoAreaIdLinkId(sender, linkInfoAreaId, linkId);
                if (!string.IsNullOrEmpty(rid) && rid.Length > 8)
                {
                    return rid;
                }
            }

            return null;
        }

        /// <summary>
        /// Currents the record for sender selector.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="selector">
        /// The selector.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string CurrentRecordForSenderSelector(object sender, UPRecordSelector selector)
        {
            return this.ContextRecordForSenderInfoAreaIdLinkId(
                sender,
                selector.LinkTargetInfoAreaId,
                selector.LinklinkId);
        }

        /// <summary>
        /// Handles the dependent fields.
        /// </summary>
        public void HandleDependentFields()
        {
            var parentFieldContextArray = new List<UPEditFieldContext>();
            foreach (var fieldContext in this.EditFieldContext.Values)
            {
                var parentField = fieldContext.ParentField;
                if (parentField == null)
                {
                    continue;
                }

                var parentFieldContext = this.EditFieldContext.ValueOrDefault(parentField.FieldIdentification);
                if (parentFieldContext == null)
                {
                    continue;
                }

                parentFieldContext.AddDependentFieldContext(fieldContext);
                if (parentFieldContextArray.Contains(parentFieldContext) == false)
                {
                    parentFieldContextArray.Add(parentFieldContext);
                }
            }

            foreach (var parentFieldContext in parentFieldContextArray)
            {
                parentFieldContext.NotifyDependentFields();
            }
        }

        /// <summary>
        /// Links the key for information area identifier link identifier.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string LinkKeyForInfoAreaIdLinkId(string infoAreaId, int linkId)
            => $"{infoAreaId}_{(linkId < 0 ? 0 : linkId)}";

        /// <summary>
        /// Sets as new.
        /// </summary>
        public void SetAsNew()
        {
            this.ExplicitNew = true;
        }

        /// <summary>
        /// Users the did change record selector edit field call depth.
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <param name="callDepth">
        /// The call depth.
        /// </param>
        public void UserDidChangeRecordSelectorEditFieldCallDepth(UPMRecordSelectorEditField field, int callDepth)
        {
            var selector = field.CurrentSelector;
            var crmLinkInfo = selector?.LinkInfo;
            if (crmLinkInfo == null)
            {
                return;
            }

            var link = !string.IsNullOrEmpty(field.ResultRows.RootRecordIdentification)
                           ? new UPCRMLink(field.ResultRows.RootRecordIdentification, crmLinkInfo.LinkId)
                           : new UPCRMLink(crmLinkInfo.TargetInfoAreaId, crmLinkInfo.LinkId, false);

            var key = this.LinkKeyForInfoAreaIdLinkId(crmLinkInfo.TargetInfoAreaId, crmLinkInfo.LinkId);
            if (this.changedLinks != null)
            {
                this.changedLinks.SetObjectForKey(link, key);
            }
            else
            {
                this.changedLinks = new Dictionary<string, UPCRMLink> { { key, link } };
            }
        }
    }
}
