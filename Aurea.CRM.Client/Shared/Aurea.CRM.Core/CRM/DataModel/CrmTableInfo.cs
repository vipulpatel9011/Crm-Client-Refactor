// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmTableInfo.cs" company="Aurea Software Gmbh">
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
//   CRM table information
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    using System.Collections.Generic;
    using DAL;

    /// <summary>
    /// CRM table information
    /// </summary>
    public class UPCRMTableInfo
    {
        /// <summary>
        /// The table information
        /// </summary>
        private readonly TableInfo tableInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMTableInfo"/> class.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="dataStore">
        /// The data store.
        /// </param>
        public UPCRMTableInfo(string infoAreaId, ICRMDataStore dataStore)
        {
            this.InfoAreaId = infoAreaId;
            this.DataStore = dataStore;
            var database = this.DataStore?.DatabaseInstance;
            this.tableInfo = database?.GetTableInfoByInfoArea(this.InfoAreaId);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMTableInfo"/> class.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        public UPCRMTableInfo(string infoAreaId)
            : this(infoAreaId, UPCRMDataStore.DefaultStore)
        {
        }

        /// <summary>
        /// Gets the name of the database table.
        /// </summary>
        /// <value>
        /// The name of the database table.
        /// </value>
        public string DatabaseTableName => this.tableInfo?.DatabaseTableName;

        /// <summary>
        /// Gets the data store.
        /// </summary>
        /// <value>
        /// The data store.
        /// </value>
        public ICRMDataStore DataStore { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has virtual information areas.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has virtual information areas; otherwise, <c>false</c>.
        /// </value>
        public bool HasVirtualInfoAreas => this.tableInfo != null && this.tableInfo.HasVirtualInfoAreas;

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; private set; }

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label => this.tableInfo?.Name;

        /// <summary>
        /// Gets the number of virtual information areas.
        /// </summary>
        /// <value>
        /// The number of virtual information areas.
        /// </value>
        public int NumberOfVirtualInfoAreas => this.tableInfo?.VirtualInfoAreas?.Count ?? 0;

        /// <summary>
        /// Alls the field column names.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<string> AllFieldColumnNames()
        {
            var fieldArray = new List<string>();
            int count = this.tableInfo?.FieldCount ?? 0;

            for (var i = 0; i < count; i++)
            {
                var cppFieldInfo = this.tableInfo?.GetFieldInfoByIndex(i);
                fieldArray.Add(cppFieldInfo?.DatabaseFieldName);
            }

            return fieldArray;
        }

        /// <summary>
        /// Alls the fields.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<UPCRMFieldInfo> AllFields()
        {
            var fieldArray = new List<UPCRMFieldInfo>();
            int count = this.tableInfo?.FieldCount ?? 0;

            for (var i = 0; i < count; i++)
            {
                var cppFieldInfo = this.tableInfo?.GetFieldInfoByIndex(i);
                var fieldInfo = UPCRMFieldInfo.Create(this.InfoAreaId, cppFieldInfo);
                fieldArray.Add(fieldInfo);
            }

            return fieldArray;
        }

        /// <summary>
        /// Alls the link column names.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<string> AllLinkColumnNames()
        {
            var hasGeneric = false;
            var linkArray = new List<string>();
            int count = this.tableInfo?.LinkCount ?? 0;

            for (var i = 0; i < count; i++)
            {
                var cppLinkInfo = this.tableInfo?.GetLink(i);
                if (!cppLinkInfo.HasColumn)
                {
                    continue;
                }

                if (cppLinkInfo.IsGeneric)
                {
                    if (hasGeneric)
                    {
                        continue;
                    }

                    hasGeneric = true;
                    linkArray.Add(cppLinkInfo.InfoAreaColumnName);
                }

                linkArray.Add(cppLinkInfo.ColumnName);
            }

            return linkArray;
        }

        /// <summary>
        /// Alls the links.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<UPCRMLinkInfo> AllLinks()
        {
            var linkArray = new List<UPCRMLinkInfo>();
            int count = this.tableInfo?.LinkCount ?? 0;

            for (var i = 0; i < count; i++)
            {
                var cppLinkInfo = this.tableInfo?.GetLink(i);
                var linkInfo = new UPCRMLinkInfo(this.InfoAreaId, cppLinkInfo);
                linkArray.Add(linkInfo);
            }

            return linkArray;
        }

        /// <summary>
        /// Alls the names.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<string> AllNames()
        {
            var a = new List<string>(AllSysColumnNames);
            a.AddRange(this.AllFieldColumnNames());
            a.AddRange(this.AllLinkColumnNames());
            return a;
        }

        /// <summary>
        /// All the system column names.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private static readonly List<string> AllSysColumnNames = new List<string> { "recid", "title", "upd", "sync" };

        /// <summary>
        /// Fields the information for field identifier.
        /// </summary>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMFieldInfo"/>.
        /// </returns>
        public UPCRMFieldInfo FieldInfoForFieldId(int fieldId)
        {
            return UPCRMFieldInfo.Create(fieldId, this);
        }

        /// <summary>
        /// Determines whether [has offline data].
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HasOfflineData => this.tableInfo != null && this.tableInfo.HasOfflineData(this.DataStore.DatabaseInstance);

        /// <summary>
        /// Idents the link.
        /// </summary>
        /// <returns>
        /// The <see cref="UPCRMLinkInfo"/>.
        /// </returns>
        public UPCRMLinkInfo IdentLink => this.LinkInfoForTargetInfoAreaIdLinkId(this.InfoAreaId, -1);

        /// <summary>
        /// Keys the fields.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<UPCRMLinkInfoField> KeyFields()
        {
            return this.IdentLink.LinkFieldArray;
        }

        /// <summary>
        /// Links the information for target information area identifier link identifier.
        /// </summary>
        /// <param name="targetInfoAreaId">
        /// The target information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMLinkInfo"/>.
        /// </returns>
        public UPCRMLinkInfo LinkInfoForTargetInfoAreaIdLinkId(string targetInfoAreaId, int linkId)
        {
            if (targetInfoAreaId == null || this.tableInfo == null)
            {
                return null;
            }

            var linkInfo = this.tableInfo.GetLink(targetInfoAreaId, linkId);
            if (linkInfo == null || linkInfo.LinkFieldCount == 0)
            {
                return null;
            }

            return new UPCRMLinkInfo(this.InfoAreaId, linkInfo);
        }

        /// <summary>
        /// Linkses the with field.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<UPCRMLinkInfo> LinksWithField()
        {
            var arr = new List<UPCRMLinkInfo>();
            int count = this.tableInfo?.LinkCount ?? 0;

            for (int i = 0; i < count; i++)
            {
                var linkInfo = this.tableInfo?.GetLink(i);
                if (linkInfo.HasColumn)
                {
                    arr.Add(new UPCRMLinkInfo(this.InfoAreaId, linkInfo));
                }
            }

            return arr;
        }

        /// <summary>
        /// Parents the of information area identifier link identifier.
        /// </summary>
        /// <param name="childInfoAreaId">
        /// The child information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ParentOfInfoAreaIdLinkId(string childInfoAreaId, int linkId)
        {
            var linkInfo = this.LinkInfoForTargetInfoAreaIdLinkId(childInfoAreaId, linkId);
            return !linkInfo.IsParentLink;
        }

        /// <summary>
        /// Roots the information area identifier.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string RootInfoAreaId()
        {
            return this.tableInfo?.RootInfoAreaId;
        }

        /// <summary>
        /// Roots the physical information area identifier.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string RootPhysicalInfoAreaId()
        {
            return this.tableInfo?.RootPhysicalInfoAreaId;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"infoAreaId={this.InfoAreaId}, fields={this.AllFields()}, links={this.AllLinks()}";
        }

        /// <summary>
        /// Virtuals the index of the information area at.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string VirtualInfoAreaAtIndex(int index)
        {
            var infoAreaId = this.tableInfo?.GetVirtualInfoAreaAtIndex(index);
            return infoAreaId;
        }

        /// <summary>
        /// Virtuals the information area identifier for record identifier.
        /// </summary>
        /// <param name="recordId">
        /// The record identifier.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string VirtualInfoAreaIdForRecordId(string recordId)
        {
            return this.tableInfo?.GetVirtualInfoAreaId(recordId, this.DataStore.DatabaseInstance);
        }
    }
}
