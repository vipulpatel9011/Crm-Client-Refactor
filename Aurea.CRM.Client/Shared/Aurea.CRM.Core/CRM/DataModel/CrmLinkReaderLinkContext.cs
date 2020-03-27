// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmLinkReaderLinkContext.cs" company="Aurea Software Gmbh">
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
//   The CRM Link Reader Link Context class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    using System.Collections.Generic;

    using Query;

    /// <summary>
    /// The upcrm link reader link context.
    /// </summary>
    public class UPCRMLinkReaderLinkContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMLinkReaderLinkContext"/> class.
        /// </summary>
        /// <param name="linkInfo">
        /// The link info.
        /// </param>
        /// <param name="linkReader">
        /// The link reader.
        /// </param>
        public UPCRMLinkReaderLinkContext(UPCRMLinkInfo linkInfo, UPCRMLinkReader linkReader)
        {
            this.LinkInfo = linkInfo;
            if (this.LinkInfo.IsGeneric)
            {
            }
            else if (this.LinkInfo.UseLinkFields)
            {
                List<UPCRMField> linkFields = new List<UPCRMField>();
                foreach (UPCRMLinkInfoField linkInfoField in this.LinkInfo.LinkFieldArray)
                {
                    if (linkInfoField.FieldId >= 0)
                    {
                        linkFields.Add(UPCRMField.FieldWithFieldIdInfoAreaId(linkInfoField.FieldId, linkReader.InfoAreaId));
                    }
                }

                this.FieldLinkFields = linkFields;
            }
            else
            {
                this.LinkField = UPCRMLinkField.FieldWithLinkInfoAreaIdLinkIdInfoAreaId(
                    this.LinkInfo.TargetInfoAreaId,
                    this.LinkInfo.LinkId,
                    linkReader.InfoAreaId);
            }
        }

        /// <summary>
        /// Gets the field link fields.
        /// </summary>
        public List<UPCRMField> FieldLinkFields { get; private set; }

        /// <summary>
        /// Gets the link field.
        /// </summary>
        public UPCRMLinkField LinkField { get; private set; }

        /// <summary>
        /// Gets the link info.
        /// </summary>
        public UPCRMLinkInfo LinkInfo { get; private set; }

        /// <summary>
        /// Gets or sets the starting position.
        /// </summary>
        public int StartingPosition { get; set; }

        /// <summary>
        /// The read link record identification from row.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ReadLinkRecordIdentificationFromRow(UPCRMResultRow row)
        {
            if (!this.LinkInfo.UseLinkFields)
            {
                return null;
            }

            int nextIndex = this.StartingPosition;
            string targetInfoAreaId = this.LinkInfo.TargetInfoAreaId;
            UPContainerMetaInfo query = new UPContainerMetaInfo(new List<UPCRMField>(), targetInfoAreaId);
            UPInfoAreaCondition condition = null;
            foreach (UPCRMLinkInfoField linkInfoField in this.LinkInfo.LinkFieldArray)
            {
                UPInfoAreaCondition leaf;
                if (linkInfoField.FieldId >= 0)
                {
                    string value = row.RawValueAtIndex(nextIndex++);
                    if (linkInfoField.TargetFieldId < 0)
                    {
                        if (!value.Equals(linkInfoField.TargetValue))
                        {
                            return null;
                        }
                    }

                    leaf = new UPInfoAreaConditionLeaf(targetInfoAreaId, linkInfoField.TargetFieldId, value);
                }
                else
                {
                    leaf = new UPInfoAreaConditionLeaf(
                        targetInfoAreaId,
                        linkInfoField.TargetFieldId,
                        linkInfoField.SourceValue);
                }

                condition = condition != null ? condition.InfoAreaConditionByAppendingAndCondition(leaf) : leaf;
            }

            if (condition != null)
            {
                query.RootInfoAreaMetaInfo.AddCondition(condition);
            }

            UPCRMResult result = query.Find();
            if (result.RowCount >= 1)
            {
                return ((UPCRMResultRow)result.ResultRowAtIndex(0)).RootRecordIdentification;
            }

            return null;
        }
    }
}
