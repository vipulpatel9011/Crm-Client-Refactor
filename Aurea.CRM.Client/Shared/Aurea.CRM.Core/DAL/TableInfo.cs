// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableInfo.cs" company="Aurea Software Gmbh">
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
//   Defines the table infomation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.DAL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Aurea.CRM.Core.CRM;

    /// <summary>
    /// Defines the table infomation
    /// </summary>
    public class TableInfo
    {
        /// <summary>
        /// The maximum linkfield count
        /// </summary>
        public const int MaxLinkfieldCount = 32;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableInfo"/> class.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="rootInfoAreaId">
        /// The root information area identifier.
        /// </param>
        /// <param name="rootPhysicalInfoAreaId">
        /// The root physical information area identifier.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="hasLookup">
        /// The has lookup.
        /// </param>
        public TableInfo(
            string infoAreaId,
            string rootInfoAreaId,
            string rootPhysicalInfoAreaId,
            string name,
            int hasLookup)
        {
            // trim at MAX_INFOAREAID_LEN
            this.InfoAreaId = infoAreaId;
            this.RootInfoAreaId = rootInfoAreaId;
            this.RootPhysicalInfoAreaId = rootPhysicalInfoAreaId;

            this.Name = name;
            this.HasLookup = hasLookup > 0;
            this.DatabaseTableName = $"CRM_{infoAreaId}";
            this.FieldInfos = new List<FieldInfo>();
        }

        /// <summary>
        /// Gets the participants field infos.
        /// </summary>
        /// <value>
        /// The participants field infos.
        /// </value>
        public List<FieldInfo> ParticipantsFieldInfos { get; private set; }

        /// <summary>
        /// Gets the participants table names.
        /// </summary>
        /// <value>
        /// The participants table names.
        /// </value>
        public List<string> ParticipantsTableNames { get; private set; }

        /// <summary>
        /// Gets the name of the database table.
        /// </summary>
        /// <value>
        /// The name of the database table.
        /// </value>
        public string DatabaseTableName { get; private set; }

        /// <summary>
        /// Gets the field count.
        /// </summary>
        /// <value>
        /// The field count.
        /// </value>
        public int FieldCount => this.FieldInfos?.Count ?? 0;

        /// <summary>
        /// Gets or sets the field infos.
        /// </summary>
        /// <value>
        /// The field infos.
        /// </value>
        public List<FieldInfo> FieldInfos { get; set; }

        /// <summary>
        /// Gets the name of the field infos by XML.
        /// </summary>
        /// <value>
        /// The name of the field infos by XML.
        /// </value>
        public FieldInfo[] FieldInfosByXmlName { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has lookup.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has lookup; otherwise, <c>false</c>.
        /// </value>
        public bool HasLookup { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has virtual information areas.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has virtual information areas; otherwise, <c>false</c>.
        /// </value>
        public bool HasVirtualInfoAreas => this.VirtualInfoAreas?.Count > 0;

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; private set; }

        /// <summary>
        /// Gets the name of the information area identifier field.
        /// </summary>
        /// <value>
        /// The name of the information area identifier field.
        /// </value>
        public string InfoAreaIdFieldName => "title";

        /// <summary>
        /// Gets or sets the link count.
        /// </summary>
        /// <value>
        /// The link count.
        /// </value>
        public int LinkCount => this.LinkInfos?.Count ?? 0;

        /// <summary>
        /// Gets or sets the link infos.
        /// </summary>
        /// <value>
        /// The link infos.
        /// </value>
        public List<LinkInfo> LinkInfos { get; set; }

        /// <summary>
        /// Gets the name of the link infos by.
        /// </summary>
        /// <value>
        /// The name of the link infos by.
        /// </value>
        public LinkInfo[] LinkInfosByName { get; private set; }

        /// <summary>
        /// Gets the name of the lookup field.
        /// </summary>
        /// <value>
        /// The name of the lookup field.
        /// </value>
        public string LookupFieldName => "lookup";

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets the name of the record identifier field.
        /// </summary>
        /// <value>
        /// The name of the record identifier field.
        /// </value>
        public string RecordIdFieldName => "recid";

        /// <summary>
        /// Gets the root information area identifier.
        /// </summary>
        /// <value>
        /// The root information area identifier.
        /// </value>
        public string RootInfoAreaId { get; }

        /// <summary>
        /// Gets the root physical information area identifier.
        /// </summary>
        /// <value>
        /// The root physical information area identifier.
        /// </value>
        public string RootPhysicalInfoAreaId { get; }

        /// <summary>
        /// Gets the name of the synchronize date field.
        /// </summary>
        /// <value>
        /// The name of the synchronize date field.
        /// </value>
        public string SyncDateFieldName => "sync";

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TableInfo"/> is unsorted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if unsorted; otherwise, <c>false</c>.
        /// </value>
        public bool Unsorted { get; set; }

        /// <summary>
        /// Gets the name of the upd date field.
        /// </summary>
        /// <value>
        /// The name of the upd date field.
        /// </value>
        public string UpdDateFieldName => "upd";

        /// <summary>
        /// Gets or sets the virtual information areas.
        /// </summary>
        /// <value>
        /// The virtual information areas.
        /// </value>
        public List<TableInfo> VirtualInfoAreas { get; set; }

        /// <summary>
        /// Gets or sets the virtual link infos.
        /// </summary>
        /// <value>
        /// The virtual link infos.
        /// </value>
        public List<VirtualLinkInfo> VirtualLinkInfos { get; set; }

        /// <summary>
        /// Adds the field information.
        /// </summary>
        /// <param name="fieldInfo">
        /// The field information.
        /// </param>
        /// <returns>
        /// The <see cref="FieldInfo"/>.
        /// </returns>
        public FieldInfo AddFieldInfo(FieldInfo fieldInfo)
        {
            return this.AddFieldInfoWithOwnership(fieldInfo.CreateCopy());
        }

        /// <summary>
        /// Adds the field information.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <param name="xmlName">
        /// Name of the XML.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="fieldType">
        /// Type of the field.
        /// </param>
        /// <param name="fieldLen">
        /// Length of the field.
        /// </param>
        /// <param name="cat">
        /// The cat.
        /// </param>
        /// <param name="ucat">
        /// The ucat.
        /// </param>
        /// <param name="attributes">
        /// The attributes.
        /// </param>
        /// <param name="repMode">
        /// The rep mode.
        /// </param>
        /// <param name="rights">
        /// The rights.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arrayFieldString">
        /// The array field string.
        /// </param>
        /// <returns>
        /// The <see cref="FieldInfo"/>.
        /// </returns>
        public FieldInfo AddFieldInfo(
            string infoAreaId,
            int fieldId,
            string xmlName,
            string name,
            char fieldType,
            int fieldLen,
            int cat,
            int ucat,
            int attributes,
            string repMode,
            int rights,
            int format,
            string[] arrayFieldString)
        {
            if (string.IsNullOrEmpty(arrayFieldString?[0]))
            {
                return
                    this.AddFieldInfoWithOwnership(
                        new FieldInfo(
                            infoAreaId,
                            fieldId,
                            xmlName,
                            name,
                            fieldType,
                            fieldLen,
                            cat,
                            ucat,
                            attributes,
                            repMode,
                            rights,
                            format,
                            0,
                            null));
            }

            var arrayFieldCount = 0;
            var curPos = arrayFieldString;
            arrayFieldCount = 0;
            var arrayFieldIndices = new int[arrayFieldString.Length / 2 + 1];
#if PORTING
            while (curPos != null)
            {
                arrayFieldIndices[arrayFieldCount++] = atoi(curPos);
                curPos = strchr(curPos, ',');
                if (curPos) ++curPos;
            }
#endif
            var fieldInfo =
                this.AddFieldInfoWithOwnership(
                    new FieldInfo(
                        infoAreaId,
                        fieldId,
                        xmlName,
                        name,
                        fieldType,
                        fieldLen,
                        cat,
                        ucat,
                        attributes,
                        repMode,
                        rights,
                        format,
                        arrayFieldCount,
                        arrayFieldIndices));
            return fieldInfo;
        }

        /// <summary>
        /// Adds the field information with ownership.
        /// </summary>
        /// <param name="fieldInfo">
        /// The field information.
        /// </param>
        /// <returns>
        /// The <see cref="FieldInfo"/>.
        /// </returns>
        public FieldInfo AddFieldInfoWithOwnership(FieldInfo fieldInfo)
        {
            this.FieldInfos.Add(fieldInfo);

            if (fieldInfo.IsParticipantsField)
            {
                if (this.ParticipantsFieldInfos == null)
                {
                    this.ParticipantsFieldInfos = new List<FieldInfo>();
                    this.ParticipantsTableNames = new List<string>();
                }

                string tableName = this.GetDatabaseTableNameForParticipantsField(fieldInfo);
                this.ParticipantsTableNames.Add(tableName);
                this.ParticipantsFieldInfos.Add(fieldInfo);
            }

            this.Unsorted = true;
            this.UpdateDictionaries();
            return fieldInfo;
        }

        /// <summary>
        /// Adds the link information.
        /// </summary>
        /// <param name="relatedInfoAreaId">
        /// The related information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <param name="reverseLinkId">
        /// The reverse link identifier.
        /// </param>
        /// <param name="relationType">
        /// Type of the relation.
        /// </param>
        /// <param name="sourceFieldId">
        /// The source field identifier.
        /// </param>
        /// <param name="destFieldId">
        /// The dest field identifier.
        /// </param>
        /// <param name="linkFieldCount">
        /// The link field count.
        /// </param>
        /// <param name="sourceLinkFieldIds">
        /// The source link field ids.
        /// </param>
        /// <param name="destLinkFieldIds">
        /// The dest link field ids.
        /// </param>
        /// <param name="linkFlag">
        /// The link flag.
        /// </param>
        /// <param name="sourceValues">
        /// The source values.
        /// </param>
        /// <param name="destValues">
        /// The dest values.
        /// </param>
        /// <returns>
        /// The <see cref="LinkInfo"/>.
        /// </returns>
        public LinkInfo AddLinkInfo(
            string relatedInfoAreaId,
            int linkId,
            int reverseLinkId,
            LinkType relationType,
            int sourceFieldId,
            int destFieldId,
            int linkFieldCount,
            int[] sourceLinkFieldIds,
            int[] destLinkFieldIds,
            int linkFlag,
            string[] sourceValues,
            string[] destValues)
        {
            return
                this.AddLinkInfoWithOwnership(
                    new LinkInfo(
                        this.InfoAreaId,
                        relatedInfoAreaId,
                        linkId,
                        reverseLinkId,
                        relationType,
                        sourceFieldId,
                        destFieldId,
                        linkFieldCount,
                        sourceLinkFieldIds,
                        destLinkFieldIds,
                        linkFlag,
                        sourceValues,
                        destValues));
        }

        /// <summary>
        /// Adds the link information.
        /// </summary>
        /// <param name="linkInfo">
        /// The link information.
        /// </param>
        /// <returns>
        /// The <see cref="LinkInfo"/>.
        /// </returns>
        public LinkInfo AddLinkInfo(LinkInfo linkInfo)
        {
            return this.AddLinkInfoWithOwnership(linkInfo.CreateCopy());
        }

        /// <summary>
        /// Adds the link information with ownership.
        /// </summary>
        /// <param name="linkInfo">
        /// The link information.
        /// </param>
        /// <returns>
        /// The <see cref="LinkInfo"/>.
        /// </returns>
        public LinkInfo AddLinkInfoWithOwnership(LinkInfo linkInfo)
        {
            if (this.LinkInfos == null)
            {
                this.LinkInfos = new List<LinkInfo>();
            }

            this.LinkInfos.Add(linkInfo);
            this.Unsorted = true;
            this.UpdateDictionaries();
            return linkInfo;
        }

        /// <summary>
        /// Adds the special information area.
        /// </summary>
        /// <param name="tableInfo">
        /// The table information.
        /// </param>
        public void AddSpecialInfoArea(TableInfo tableInfo)
        {
            if (this.VirtualInfoAreas == null)
            {
                this.VirtualInfoAreas = new List<TableInfo>();
            }

            this.VirtualInfoAreas.Add(tableInfo);
        }

        /// <summary>
        /// Adds the virtual link with ownership.
        /// </summary>
        /// <param name="virtualLinkInfo">
        /// The virtual link information.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool AddVirtualLinkWithOwnership(VirtualLinkInfo virtualLinkInfo)
        {
            if (this.VirtualLinkInfos == null)
            {
                this.VirtualLinkInfos = new List<VirtualLinkInfo>();
            }

            this.VirtualLinkInfos.Add(virtualLinkInfo);
            return true;
        }

        /// <summary>
        /// Gets the participants field count.
        /// </summary>
        /// <returns></returns>
        public int GetParticipantsFieldCount()
        {
            return this.ParticipantsFieldInfos?.Count ?? 0;
        }

        /// <summary>
        /// Creates the alter table statements.
        /// </summary>
        /// <param name="metainfoTable">
        /// The metainfo table.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<string> CreateAlterTableStatements(DatabaseMetaInfoTable metainfoTable)
        {
            var first = true;
            var prefix = string.Empty;
            List<string> stringList = null;
            string statement;
            LinkInfo genLink = null;
            DatabaseMetaInfoField field;

            if (this.FieldCount <= 0)
            {
                return null;
            }

            for (var i = 0; i < this.FieldCount; i++)
            {
                field = metainfoTable.GetField(this.FieldInfos[i].DatabaseFieldName);
                if (field != null)
                {
                    continue;
                }

                if (first)
                {
                    prefix = $"ALTER TABLE {this.DatabaseTableName} ADD COLUMN";
                    stringList = new List<string>();
                    first = false;
                }

                statement = $"{prefix} {this.FieldInfos[i].DatabaseFieldName} {FieldTypeToDatabaseType(this.FieldInfos[i].FieldType)}";
                stringList.Add(statement);
            }

            for (var i = 0; i < this.LinkCount; i++)
            {
                if (!this.LinkInfos[i].HasColumn)
                {
                    continue;
                }

                if (this.LinkInfos[i].IsGeneric)
                {
                    if (genLink == null)
                    {
                        genLink = this.LinkInfos[i];
                    }

                    continue;
                }

                field = metainfoTable.GetField(this.LinkInfos[i].ColumnName);
                if (field != null)
                {
                    continue;
                }

                if (first)
                {
                    prefix = $"ALTER TABLE {this.DatabaseTableName} ADD COLUMN";
                    stringList = new List<string>();
                    first = false;
                }

                statement = $"{prefix} {this.LinkInfos[i].ColumnName} TEXT";
                stringList.Add(statement);
            }

            if (genLink != null)
            {
                field = metainfoTable.GetField(genLink.ColumnName);
                if (field == null)
                {
                    if (first)
                    {
                        prefix = $"ALTER TABLE {this.DatabaseTableName} ADD COLUMN";
                        stringList = new List<string>();
                    }

                    statement = $"{prefix} {genLink.InfoAreaColumnName} TEXT";
                    stringList.Add(statement);

                    statement = $"{prefix} {genLink.ColumnName} TEXT";
                    stringList.Add(statement);
                }
            }

            return stringList;
        }

        /// <summary>
        /// Creates the copy.
        /// </summary>
        /// <returns>
        /// The <see cref="TableInfo"/>.
        /// </returns>
        public TableInfo CreateCopy()
        {
            var dest = new TableInfo(
                this.InfoAreaId,
                this.RootInfoAreaId,
                this.RootPhysicalInfoAreaId,
                this.Name,
                this.HasLookup ? 1 : 0);

            for (var i = 0; i < this.FieldCount; i++)
            {
                dest.AddFieldInfo(this.FieldInfos[i]);
            }

            for (var i = 0; i < this.LinkCount; i++)
            {
                dest.AddLinkInfo(this.LinkInfos[i]);
            }

            if (!this.Unsorted)
            {
                dest.Sort();
            }

            return dest;
        }

        /// <summary>
        /// Creates the create table statement.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string CreateCreateTableStatement()
        {
            LinkInfo genLink = null;

            var memorybuffer = new StringBuilder();
            memorybuffer.AppendFormat($"CREATE TABLE {this.DatabaseTableName}");
            memorybuffer.Append("(recid TEXT PRIMARY KEY, title TEXT, sync TEXT, upd TEXT, lookup INTEGER");

            for (var i = 0; i < this.FieldCount; i++)
            {
                memorybuffer.AppendFormat(
                    $", {this.FieldInfos[i].DatabaseFieldName} {FieldTypeToDatabaseType(this.FieldInfos[i].FieldType)}");
            }

            for (var i = 0; i < this.LinkCount; i++)
            {
                if (!this.LinkInfos[i].HasColumn)
                {
                    continue;
                }

                if (this.LinkInfos[i].IsGeneric)
                {
                    if (genLink == null)
                    {
                        genLink = this.LinkInfos[i];
                    }

                    continue;
                }

                memorybuffer.AppendFormat($", {this.LinkInfos[i].ColumnName} TEXT");
            }

            if (genLink != null)
            {
                memorybuffer.AppendFormat($", {genLink.InfoAreaColumnName} TEXT");
                memorybuffer.AppendFormat($", {genLink.ColumnName} TEXT");
            }

            memorybuffer.Append(")");

            for (var i = 0; i < this.ParticipantsFieldInfos?.Count; i++)
            {
                var participantsCreateString = this.CreateCreateTableStatementForParticipantsField(i);
                if (!string.IsNullOrEmpty(participantsCreateString))
                {
                    memorybuffer.Append(";");
                    memorybuffer.Append(participantsCreateString);
                }
            }

            return memorybuffer.ToString();
        }

        /// <summary>
        /// Creates the create table statement for participants field.
        /// </summary>
        /// <param name="participantsFieldIndex">
        /// Index of the participants field.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string CreateCreateTableStatementForParticipantsField(int participantsFieldIndex)
        {
            var memorybuffer = new StringBuilder();
            memorybuffer.Append("CREATE TABLE ");
            memorybuffer.Append(this.GetDatabaseTableNameForParticipantsField(participantsFieldIndex));
            memorybuffer.Append(
                "(recid TEXT, nr INTEGER, repId INT, repOrgGroupId INT, attendance TEXT, additional TEXT, PRIMARY KEY (recid,nr))");

            return memorybuffer.ToString();
        }

        /// <summary>
        /// Gets the database table name for participants field.
        /// </summary>
        /// <param name="fieldInfo">
        /// The field information.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetDatabaseTableNameForParticipantsField(FieldInfo fieldInfo)
        {
            return $"{this.DatabaseTableName}_PART_{fieldInfo.DatabaseFieldName}";
        }

        /// <summary>
        /// Gets the database table name for participants field.
        /// </summary>
        /// <param name="participantsFieldIndex">
        /// Index of the participants field.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetDatabaseTableNameForParticipantsField(int participantsFieldIndex)
        {
            return this.ParticipantsTableNames[participantsFieldIndex];
        }

        /// <summary>
        /// Gets the default link.
        /// </summary>
        /// <param name="targetInfoAreaid">
        /// The target information areaid.
        /// </param>
        /// <returns>
        /// The <see cref="LinkInfo"/>.
        /// </returns>
        public LinkInfo GetDefaultLink(string targetInfoAreaid)
        {
            return this.GetLink(targetInfoAreaid, -1);
        }

        /// <summary>
        /// Gets the field information.
        /// </summary>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <returns>
        /// The <see cref="FieldInfo"/>.
        /// </returns>
        public FieldInfo GetFieldInfo(int fieldId)
        {
            return this.FieldInfos?.FirstOrDefault(f => f.FieldId == fieldId);
        }

        /// <summary>
        /// Gets the field information.
        /// </summary>
        /// <param name="xmlName">
        /// Name of the XML.
        /// </param>
        /// <returns>
        /// The <see cref="FieldInfo"/>.
        /// </returns>
        public FieldInfo GetFieldInfo(string xmlName)
        {
            if (this.Unsorted || this.FieldInfosByXmlName == null)
            {
                return this.FieldInfos.FirstOrDefault(f => f.XmlName == xmlName);
            }

            return this.FieldInfosByXmlName.FirstOrDefault(f => f.XmlName == xmlName);
        }

        /// <summary>
        /// Gets the index of the field information by.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="FieldInfo"/>.
        /// </returns>
        public FieldInfo GetFieldInfoByIndex(int index)
        {
            return index < this.FieldCount ? this.FieldInfos[index] : null;
        }

        /// <summary>
        /// Gets the name of the field.
        /// </summary>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetFieldName(int fieldId)
        {
            return this.GetFieldName((FieldIdType)fieldId);
        }

        /// <summary>
        /// Gets the name of the field.
        /// </summary>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetFieldName(FieldIdType fieldId)
        {
            switch (fieldId)
            {
                case FieldIdType.INFOAREAID:
                    return this.InfoAreaIdFieldName;
                case FieldIdType.RECORDID:
                    return this.RecordIdFieldName;
                case FieldIdType.UPDDATE:
                    return this.UpdDateFieldName;
                case FieldIdType.SYNCDATE:
                    return this.SyncDateFieldName;
                case FieldIdType.LOOKUP:
                    return this.LookupFieldName;
                case FieldIdType.EMPTY:
                    return "null";
                default:
                    return $"F{fieldId}";
            }
        }

        /// <summary>
        /// Gets the link.
        /// </summary>
        /// <param name="linkIndex">
        /// Index of the link.
        /// </param>
        /// <returns>
        /// The <see cref="LinkInfo"/>.
        /// </returns>
        public LinkInfo GetLink(int linkIndex)
        {
            return linkIndex < this.LinkCount ? this.LinkInfos[linkIndex] : null;
        }

        /// <summary>
        /// Gets the link.
        /// </summary>
        /// <param name="linkFieldName">
        /// Name of the link field.
        /// </param>
        /// <returns>
        /// The <see cref="LinkInfo"/>.
        /// </returns>
        public LinkInfo GetLink(string linkFieldName)
        {
            if (linkFieldName == "LINK_RECORDID")
            {
                return this.LinkInfos.FirstOrDefault(x => x.LinkId == 126);
            }

            if (this.Unsorted && this.LinkInfos != null)
            {
                return this.LinkInfos.FirstOrDefault(x => x.IdentName == linkFieldName);
            }

            return this.LinkInfosByName?.FirstOrDefault(f => f.IdentName == linkFieldName);
        }

        /// <summary>
        /// Gets the link.
        /// </summary>
        /// <param name="targetInfoAreaId">
        /// The target information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <returns>
        /// The <see cref="LinkInfo"/>.
        /// </returns>
        public LinkInfo GetLink(string targetInfoAreaId, int linkId)
        {
            LinkInfo infoAreaDefaultLink = null;
            LinkInfo reverseLinkIdLink = null;

            for (var i = 0; i < this.LinkCount; i++)
            {
                if (!Equals(targetInfoAreaId, this.LinkInfos[i].TargetInfoAreaId))
                {
                    continue;
                }

                var currentLinkId = this.LinkInfos[i].LinkId;
                if (currentLinkId == linkId)
                {
                    return this.LinkInfos[i];
                }

                if (linkId > 0 && linkId == this.LinkInfos[i].ReverseLinkId)
                {
                    reverseLinkIdLink = this.LinkInfos[i];
                }

                if (linkId > 0 && currentLinkId != linkId)
                {
                    continue;
                }

                if (linkId <= 0)
                {
                    if (currentLinkId <= 0)
                    {
                        return this.LinkInfos[i];
                    }

                    if (infoAreaDefaultLink == null || (infoAreaDefaultLink.IsGeneric && !this.LinkInfos[i].IsGeneric))
                    {
                        infoAreaDefaultLink = this.LinkInfos[i];
                    }
                }
            }

            return reverseLinkIdLink ?? infoAreaDefaultLink;
        }

        /// <summary>
        /// Gets the l nr field identifier.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetLNrFieldId()
        {
            var identLink = this.GetLink(this.InfoAreaId, 0);
            if (identLink == null)
            {
                return -1;
            }

            if (identLink.LinkFieldCount == 2)
            {
                return identLink.GetSourceFieldIdWithIndex(1);
            }

            if (identLink.LinkFieldCount == 4 && !Equals(this.InfoAreaId, "KP"))
            {
                return identLink.GetSourceFieldIdWithIndex(3);
            }

            return -1;
        }

        /// <summary>
        /// Gets the stat no field identifier.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetStatNoFieldId()
        {
            var identLink = this.GetLink(this.InfoAreaId, 0);
            if (identLink == null)
            {
                return -1;
            }

            if (identLink.LinkFieldCount == 2)
            {
                return identLink.SourceFieldId;
            }

            if (identLink.LinkFieldCount == 4 && !Equals(this.InfoAreaId, "KP"))
            {
                return identLink.GetSourceFieldIdWithIndex(2);
            }

            return -1;
        }

        /// <summary>
        /// Gets the index of the virtual information area at.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetVirtualInfoAreaAtIndex(int index)
        {
            if (this.VirtualInfoAreas == null || index >= this.VirtualInfoAreas.Count)
            {
                return null;
            }

            return this.VirtualInfoAreas[index].InfoAreaId;
        }

        /// <summary>
        /// Gets the virtual information area identifier.
        /// </summary>
        /// <param name="recordId">
        /// The record identifier.
        /// </param>
        /// <param name="database">
        /// The database.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetVirtualInfoAreaId(string recordId, CRMDatabase database)
        {
            var tableInfo = this.GetVirtualTableInfo(recordId, database);
            return tableInfo?.InfoAreaId;
        }

        /// <summary>
        /// Gets the virtual information area table information.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <returns>
        /// The <see cref="TableInfo"/>.
        /// </returns>
        public TableInfo GetVirtualInfoAreaTableInfo(string infoAreaId)
        {
            return this.VirtualInfoAreas?.FirstOrDefault(v => v.InfoAreaId == infoAreaId);
        }

        /// <summary>
        /// Gets the virtual link information.
        /// </summary>
        /// <param name="targetInfoAreaId">
        /// The target information area identifier.
        /// </param>
        /// <param name="targetLinkId">
        /// The target link identifier.
        /// </param>
        /// <returns>
        /// The <see cref="VirtualLinkInfo"/>.
        /// </returns>
        public VirtualLinkInfo GetVirtualLinkInfo(string targetInfoAreaId, int targetLinkId)
        {
            return this.VirtualLinkInfos?.FirstOrDefault(
                    v => v.TargetInfoAreaId == targetInfoAreaId
                         && (targetLinkId > 0 || targetLinkId == v.LinkId)
                         && (targetLinkId <= 0 || v.LinkId == 0));
        }

        /// <summary>
        /// Gets the virtual table information.
        /// </summary>
        /// <param name="recordId">
        /// The record identifier.
        /// </param>
        /// <param name="database">
        /// The database.
        /// </param>
        /// <returns>
        /// The <see cref="TableInfo"/>.
        /// </returns>
        public TableInfo GetVirtualTableInfo(string recordId, CRMDatabase database)
        {
            if (this.VirtualInfoAreas?.Count == 0)
            {
                return this;
            }

            TableInfo returnTableInfo = null;

            var statementBuffer =
                $"SELECT {this.GetFieldName(FieldIdType.INFOAREAID)} FROM {this.DatabaseTableName} WHERE {this.GetFieldName(FieldIdType.RECORDID)} = ?";

            var query = new DatabaseRecordSet(database);
            var ret = query.Execute(statementBuffer, new[] { recordId }, 0);

            if (ret > 0 && query.GetRowCount() == 1)
            {
                var row = query.GetRow(0);
                var infoAreaId = row.GetColumn(0);
                returnTableInfo = !string.IsNullOrEmpty(infoAreaId) && Equals(infoAreaId, this.InfoAreaId)
                                      ? this.GetVirtualInfoAreaTableInfo(infoAreaId)
                                      : this;
            }

            return returnTableInfo;
        }

        /// <summary>
        /// Determines whether [has offline data] [the specified database].
        /// </summary>
        /// <param name="database">
        /// The database.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HasOfflineData(CRMDatabase database)
        {
            var query = new DatabaseRecordSet(database);
            query.Query.Prepare("SELECT datasetname FROM syncinfo WHERE infoareaid = ?");
            query.Query.Bind(1, this.InfoAreaId);

            var ret = query.Execute(1);
            return ret == 0 && query.GetRowCount() > 0;
        }

        /// <summary>
        /// Loads the specified database.
        /// </summary>
        /// <param name="database">
        /// The database.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int Load(CRMDatabase database)
        {
            if (database == null)
            {
                throw new ArgumentNullException(nameof(database));
            }

            var fieldRecordSet = new DatabaseRecordSet(database);
            fieldRecordSet
               .Query.Prepare("SELECT fieldid, xmlname, name, fieldtype, fieldlen, cat, ucat, attributes, repMode, rights, format, arrayfieldindices FROM fieldinfo WHERE infoareaid = ?");
            fieldRecordSet.Query.Bind(1, InfoAreaId);
            fieldRecordSet.Execute();

            ProcessFieldInfos(fieldRecordSet);

            var linkRecordSet = GetDatabaseRecordSet(database);

            int result;
            DatabaseRecordSet linkFieldRecordSet = null;

            if (ExecuteQuery(database, ref linkFieldRecordSet, linkRecordSet, out result))
            {
                return result;
            }

            ProcessLinkInfos(linkRecordSet, linkFieldRecordSet);
            Sort();
            SetZFields();

            return result;
        }

        private static DatabaseRow SetupNextField(
            DatabaseRecordSet linkFieldRecordSet,
            DatabaseRow nextFieldRow,
            int currentLinkId,
            string currentInfoAreaId,
            int[] sourceFieldIds,
            int[] destFieldIds,
            string[] sourceValues,
            string[] destValues,
            int linkFieldInfoCount,
            ref int nextLinkFieldLinkId,
            ref string nextLinkFieldInfoAreaId,
            ref int linkFieldCount,
            ref bool hasSourceValues,
            ref bool hasDestValues,
            ref int nextLinkFieldInfo)
        {
            if (nextFieldRow.ColumnCount > 6)
            {
                while (nextFieldRow != null
                    && nextLinkFieldLinkId == currentLinkId
                    && Equals(currentInfoAreaId, nextLinkFieldInfoAreaId)
                    && linkFieldCount < MaxLinkfieldCount)
                {
                    sourceFieldIds[linkFieldCount] = nextFieldRow.GetColumnInt(3);
                    destFieldIds[linkFieldCount] = nextFieldRow.GetColumnInt(4);
                    sourceValues[linkFieldCount] = nextFieldRow.GetColumn(5);
                    destValues[linkFieldCount] = nextFieldRow.GetColumn(6);

                    if (!string.IsNullOrWhiteSpace(sourceValues[linkFieldCount]))
                    {
                        hasSourceValues = true;
                    }

                    if (!string.IsNullOrWhiteSpace(destValues[linkFieldCount]))
                    {
                        hasDestValues = true;
                    }

                    ++linkFieldCount;

                    if (nextLinkFieldInfo < linkFieldInfoCount)
                    {
                        nextFieldRow = linkFieldRecordSet.GetRow(nextLinkFieldInfo++);
                        nextLinkFieldInfoAreaId = nextFieldRow.GetColumn(0);
                        nextLinkFieldLinkId = nextFieldRow.GetColumnInt(1, 0);
                    }
                    else
                    {
                        nextFieldRow = null;
                    }
                }
            }

            return nextFieldRow;
        }

        /// <summary>
        /// Sorts this instance.
        /// </summary>
        public void Sort()
        {
            if (!this.Unsorted)
            {
                return;
            }

            if (this.FieldCount > 0)
            {
                this.FieldInfos.Sort((f1, f2) => CompareTo(f1, f2, (a, b) => a.FieldId - b.FieldId));
            }

            if (this.LinkCount > 0)
            {
                this.LinkInfos.Sort(CompareLinkinfo);
            }

            this.Unsorted = false;
            if (this.FieldCount > 0)
            {
                this.UpdateDictionaries();
            }
        }

        /// <summary>
        /// Updates the dictionaries.
        /// </summary>
        public void UpdateDictionaries()
        {
            this.FieldInfosByXmlName = null;
            this.LinkInfosByName = null;
            if (this.Unsorted)
            {
                return;
            }

            if (this.FieldCount > 0)
            {
                this.FieldInfosByXmlName = this.FieldInfos.ToArray();
                Array.Sort(
                    this.FieldInfosByXmlName,
                    (f1, f2) =>
                    CompareTo(f1, f2, (a, b) => string.Compare(f1.XmlName, f2.XmlName, StringComparison.Ordinal)));
            }

            if (this.LinkCount > 0)
            {
                this.LinkInfosByName = this.LinkInfos.ToArray();
                Array.Sort(
                    this.LinkInfosByName,
                    (l1, l2) => string.Compare(l1.IdentName, l2.IdentName, StringComparison.OrdinalIgnoreCase));
            }
        }

        /// <summary>
        /// Compares the linkinfo.
        /// </summary>
        /// <param name="l1">
        /// The first link instance.
        /// </param>
        /// <param name="l2">
        /// The other link instance.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int CompareLinkinfo(LinkInfo l1, LinkInfo l2)
        {
            var cmp = string.Compare(l1.TargetInfoAreaId, l2.TargetInfoAreaId, StringComparison.OrdinalIgnoreCase);
            if (cmp != 0)
            {
                return cmp;
            }

            return l1.LinkId - l2.LinkId;
        }

        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="f1">
        /// The f1.
        /// </param>
        /// <param name="f2">
        /// The f2.
        /// </param>
        /// <param name="comparer">
        /// The comparer.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int CompareTo(FieldInfo f1, FieldInfo f2, Func<FieldInfo, FieldInfo, int> comparer)
        {
            if (f1 == null)
            {
                return -1;
            }

            if (f2 == null)
            {
                return -1;
            }

            return comparer(f1, f2);
        }

        /// <summary>
        /// Fields the type of the type to database.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string FieldTypeToDatabaseType(char type)
        {
            switch (type)
            {
                case 'C':
                case 'Z':
                    return "TEXT COLLATE NOCASE";
                case 'D':
                case 'T':
                    return "TEXT";
                case 'F':
                    return "REAL";
                case 'L':
                case 'N':
                case 'K':
                case 'X':
                case 'S':
                case 'B':
                    return "INTEGER";
                default:
                    return "TEXT";
            }
        }

        private DatabaseRecordSet GetDatabaseRecordSet(CRMDatabase database)
        {
            if (database == null)
            {
                throw new ArgumentNullException(nameof(database));
            }

            var linkRecordSet = new DatabaseRecordSet(database);
            linkRecordSet.Query
                         .Prepare("SELECT targetinfoareaid, linkid, reverseLinkId, relationtype, sourcefieldid, destfieldid, useLinkFields FROM linkinfo WHERE infoareaid = ? ORDER BY linkid, targetinfoareaid");
            linkRecordSet.Query.Bind(1, this.InfoAreaId);
            return linkRecordSet;
        }

        private void ProcessFieldInfos(DatabaseRecordSet fieldRecordSet)
        {
            if (fieldRecordSet == null)
            {
                throw new ArgumentNullException(nameof(fieldRecordSet));
            }

            var fieldInfoCount = fieldRecordSet.GetRowCount();

            for (var i = 0; i < fieldInfoCount; i++)
            {
                var row = fieldRecordSet.GetRow(i);
                if (row.ColumnCount > 11)
                {
                    var fieldType = row.GetColumn(3)?[0] ?? 'A';

                    this.AddFieldInfo(
                        this.InfoAreaId,
                        row.GetColumnInt(0),
                        row.GetColumn(1),
                        row.GetColumn(2),
                        fieldType,
                        row.GetColumnInt(4),
                        row.GetColumnInt(5),
                        row.GetColumnInt(6),
                        row.GetColumnInt(7),
                        row.GetColumn(8),
                        row.GetColumnInt(9),
                        row.GetColumnInt(10, 0),
                        new[] { row.GetColumn(11) });
                }
            }
        }

        private bool ExecuteQuery(
            CRMDatabase database,
            ref DatabaseRecordSet linkFieldRecordSet,
            DatabaseRecordSet linkRecordSet,
            out int result)
        {
            if (database == null)
            {
                throw new ArgumentNullException(nameof(database));
            }

            if (linkRecordSet == null)
            {
                throw new ArgumentNullException(nameof(linkRecordSet));
            }

            result = linkRecordSet.Execute();

            if (result == 0)
            {
                linkFieldRecordSet = new DatabaseRecordSet(database);
                linkFieldRecordSet
                   .Query.Prepare("SELECT targetinfoareaid, linkid, nr, sourceFieldId, destFieldId, sourcevalue, destvalue FROM linkfields WHERE infoareaid = ? ORDER BY linkid, targetinfoareaid, nr");
                linkFieldRecordSet.Query.Bind(1, InfoAreaId);
                result = linkFieldRecordSet.Execute();
                return false;
            }

            return true;
        }

        private void SetZFields()
        {
            if (FieldInfos != null
                && FieldInfos.Count >= FieldCount)
            {
                for (var i = 0;
                     i < FieldCount;
                     i++)
                {
                    const char fieldTypeZ = 'Z';
                    if (FieldInfos[i].FieldType == fieldTypeZ)
                    {
                        var otherFieldId = FieldInfos[i]
                            .UCat;
                        if (otherFieldId >= 0)
                        {
                            var otherFieldInfo = FieldInfos.FirstOrDefault(f => otherFieldId == f.FieldId);

                            if (otherFieldInfo != null)
                            {
                                otherFieldInfo.ZField = FieldInfos[i]
                                    .FieldId;
                            }
                        }
                    }
                }
            }
        }

        private void ProcessLinkInfos(DatabaseRecordSet linkRecordSet, DatabaseRecordSet linkFieldRecordSet)
        {
            if (linkRecordSet == null)
            {
                throw new ArgumentNullException(nameof(linkRecordSet));
            }

            if (linkFieldRecordSet == null)
            {
                throw new ArgumentNullException(nameof(linkFieldRecordSet));
            }

            var linkInfoCount = linkRecordSet.GetRowCount();
            var linkFieldInfoCount = linkFieldRecordSet.GetRowCount();
            var nextLinkFieldInfo = 0;

            var sourceFieldIds = new int[MaxLinkfieldCount];
            var destFieldIds = new int[MaxLinkfieldCount];
            var sourceValues = new string[MaxLinkfieldCount];
            var destValues = new string[MaxLinkfieldCount];

            DatabaseRow nextFieldRow = null;
            string nextLinkFieldInfoAreaId = null;
            var nextLinkFieldLinkId = 0;

            if (linkFieldInfoCount > 0)
            {
                nextFieldRow = linkFieldRecordSet.GetRow(nextLinkFieldInfo++);
                nextLinkFieldInfoAreaId = nextFieldRow.GetColumn(0);
                nextLinkFieldLinkId = nextFieldRow.GetColumnInt(1, 0);
            }

            for (var i = 0; i < linkInfoCount; i++)
            {
                var row = linkRecordSet.GetRow(i);

                var currentInfoAreaId = row.GetColumn(0);
                var currentLinkId = row.GetColumnInt(1);

                int sourceFieldId;
                int destFieldId;
                if (row.IsNull(4))
                {
                    sourceFieldId = -1;
                    destFieldId = -1;
                }
                else
                {
                    sourceFieldId = row.GetColumnInt(4);
                    destFieldId = row.GetColumnInt(5);
                }

                var linkFlag = row.GetColumnInt(6);
                var linkFieldCount = 0;
                bool hasDestValues;
                var hasSourceValues = hasDestValues = false;
                if (nextFieldRow != null)
                {
                    while (nextFieldRow != null
                        && (nextLinkFieldLinkId <= currentLinkId
                         && string.Compare(nextLinkFieldInfoAreaId, currentInfoAreaId) < 0))
                    {
                        if (nextLinkFieldInfo < linkFieldInfoCount)
                        {
                            nextFieldRow = linkFieldRecordSet.GetRow(nextLinkFieldInfo++);
                            nextLinkFieldInfoAreaId = nextFieldRow.GetColumn(0);
                            nextLinkFieldLinkId = nextFieldRow.GetColumnInt(1, 0);
                        }
                        else
                        {
                            nextFieldRow = null;
                        }
                    }

                    nextFieldRow = SetupNextField(
                        linkFieldRecordSet,
                        nextFieldRow,
                        currentLinkId,
                        currentInfoAreaId,
                        sourceFieldIds,
                        destFieldIds,
                        sourceValues,
                        destValues,
                        linkFieldInfoCount,
                        ref nextLinkFieldLinkId,
                        ref nextLinkFieldInfoAreaId,
                        ref linkFieldCount,
                        ref hasSourceValues,
                        ref hasDestValues,
                        ref nextLinkFieldInfo);
                }

                this.AddLinkInfo(
                    currentInfoAreaId,
                    currentLinkId,
                    row.GetColumnInt(2, -1),
                    (LinkType)row.GetColumnInt(3),
                    sourceFieldId,
                    destFieldId,
                    linkFieldCount,
                    sourceFieldIds,
                    destFieldIds,
                    linkFlag,
                    hasSourceValues ? sourceValues : null,
                    hasDestValues ? destValues : null);
            }
        }
    }
}
