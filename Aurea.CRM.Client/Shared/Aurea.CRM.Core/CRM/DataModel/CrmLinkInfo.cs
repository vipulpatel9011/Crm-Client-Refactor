// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmLinkInfo.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Rashan Anushka
// </author>
// <summary>
//   CRM link infomation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using DAL;
    using Extensions;

    /// <summary>
    /// CRM link infomation
    /// </summary>
    public class UPCRMLinkInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMLinkInfo"/> class.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="linkInfoPtr">
        /// The link information PTR.
        /// </param>
        public UPCRMLinkInfo(string infoAreaId, LinkInfo linkInfoPtr)
        {
            if (linkInfoPtr == null)
            {
                return;
            }

            this.LinkInfo = linkInfoPtr;
            this.InfoAreaId = infoAreaId;
            this.TargetInfoAreaId = this.LinkInfo.TargetInfoAreaId;
            this.LinkId = this.LinkInfo.LinkId;
            var count = this.LinkInfo.LinkFieldCount;
            if (count > 0)
            {
                var fieldArray = new List<UPCRMLinkInfoField>(count);
                for (var i = 0; i < count; i++)
                {
                    var sourceValue = this.LinkInfo.GetSourceValueWithIndex(i);
                    var destinationValue = this.LinkInfo.GetDestinationValueWithIndex(i);
                    var sourceValueString = sourceValue;
                    var destinationValueString = destinationValue;

                    fieldArray.Add(
                        new UPCRMLinkInfoField(
                            this.LinkInfo.GetSourceFieldIdWithIndex(i),
                            this.LinkInfo.GetDestinationFieldIdWithIndex(i),
                            sourceValueString,
                            destinationValueString));
                }

                this.LinkFieldArray = fieldArray;
            }
            else
            {
                this.LinkFieldArray = null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMLinkInfo"/> class.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="targetInfoAreaId">
        /// The _target information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The _link identifier.
        /// </param>
        /// <param name="dataStore">
        /// The data store.
        /// </param>
        public UPCRMLinkInfo(string infoAreaId, string targetInfoAreaId, int linkId, UPCRMDataStore dataStore)
            : this(infoAreaId, dataStore?.DatabaseInstance?.GetTableInfoByInfoArea(infoAreaId)?.GetLink(targetInfoAreaId, linkId))
        {
        }

        /// <summary>
        /// Gets a value indicating whether this instance has column.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has column; otherwise, <c>false</c>.
        /// </value>
        public bool HasColumn => this.LinkInfo?.HasColumn ?? false;

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; private set; }

        /// <summary>
        /// Gets the name of the information area link field.
        /// </summary>
        /// <value>
        /// The name of the information area link field.
        /// </value>
        public string InfoAreaLinkFieldName => this.LinkInfo?.InfoAreaColumnName;

        /// <summary>
        /// Gets a value indicating whether this instance is field link.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is field link; otherwise, <c>false</c>.
        /// </value>
        public bool IsFieldLink => this.LinkInfo?.IsFieldLink ?? false;

        /// <summary>
        /// Gets a value indicating whether this instance is generic.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is generic; otherwise, <c>false</c>.
        /// </value>
        public bool IsGeneric => this.LinkInfo?.IsGeneric ?? false;

        /// <summary>
        /// Gets a value indicating whether this instance is parent link.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is parent link; otherwise, <c>false</c>.
        /// </value>
        public bool IsParentLink => this.LinkInfo?.RelationType == LinkType.ONETOMANY;

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Key => $"{this.InfoAreaId}_{this.TargetInfoAreaId}_{this.LinkId}";

        /// <summary>
        /// Gets the link field array.
        /// </summary>
        /// <value>
        /// The link field array.
        /// </value>
        public List<UPCRMLinkInfoField> LinkFieldArray { get; private set; }

        /// <summary>
        /// Gets the name of the link field.
        /// </summary>
        /// <value>
        /// The name of the link field.
        /// </value>
        public string LinkFieldName => this.LinkInfo?.ColumnName;

        /// <summary>
        /// Gets the link identifier.
        /// </summary>
        /// <value>
        /// The link identifier.
        /// </value>
        public int LinkId { get; private set; }

        /// <summary>
        /// Gets or sets the link information.
        /// </summary>
        /// <value>
        /// The link information.
        /// </value>
        public LinkInfo LinkInfo { get; set; }

        /// <summary>
        /// Gets the reverse link identifier.
        /// </summary>
        /// <value>
        /// The reverse link identifier.
        /// </value>
        public int ReverseLinkId => this.LinkInfo?.ReverseLinkId ?? 0;

        /// <summary>
        /// Gets the target information area identifier.
        /// </summary>
        /// <value>
        /// The target information area identifier.
        /// </value>
        public string TargetInfoAreaId { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [use link fields].
        /// </summary>
        /// <value>
        /// <c>true</c> if [use link fields]; otherwise, <c>false</c>.
        /// </value>
        public bool UseLinkFields => this.LinkInfo?.UseLinkFields ?? false;

        /// <summary>
        /// Gets the first field.
        /// </summary>
        /// <returns>
        /// The <see cref="UPCRMLinkInfoField"/>.
        /// </returns>
        public UPCRMLinkInfoField FirstField => this.LinkFieldArray?.Count > 0 ? this.LinkFieldArray[0] : null;

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is UPCRMLinkInfo))
            {
                return false;
            }

            var compareLinkInfo = (UPCRMLinkInfo)obj;
            return compareLinkInfo.LinkId == this.LinkId
                   && compareLinkInfo.TargetInfoAreaId.Equals(this.TargetInfoAreaId);
        }

        /// <summary>
        /// Links the index of the information field with target field.
        /// </summary>
        /// <param name="targetFieldIndex">
        /// Index of the target field.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMLinkInfoField"/>.
        /// </returns>
        public UPCRMLinkInfoField LinkInfoFieldWithTargetFieldIndex(int targetFieldIndex)
        {
            return this.LinkFieldArray?.FirstOrDefault(field => field?.TargetFieldId == targetFieldIndex);
        }

        /// <summary>
        /// Gets the second field.
        /// </summary>
        /// <returns>
        /// The <see cref="UPCRMLinkInfoField"/>.
        /// </returns>
        public UPCRMLinkInfoField SecondField => this.LinkFieldArray?.Count > 1 ? this.LinkFieldArray[1] : null;

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var desc = new StringBuilder();
            desc.Append(
                this.LinkInfo == null
                    ? $"source={this.InfoAreaId}, target={this.TargetInfoAreaId}, linkid={this.LinkId}"
                    : $"source={this.InfoAreaId}, target={this.TargetInfoAreaId}, linkid={this.LinkId} (fieldName={this.LinkInfo.ColumnName}/{(this.LinkInfo.HasColumn ? "Column" : "NoColumn")}");

            var count = this.LinkFieldArray?.Count ?? 0;
            if (count == 0)
            {
                return desc.ToString();
            }

            for (var i = 0; i < count; i++)
            {
                var linkInfoField = this.LinkFieldArray[i];
                desc =
                    desc.Append(
                        $"{Environment.NewLine} {this.TargetInfoAreaId}.{linkInfoField.TargetFieldId} -> {this.InfoAreaId}.{linkInfoField.FieldId}");
            }

            return desc.ToString();
        }
    }
}
