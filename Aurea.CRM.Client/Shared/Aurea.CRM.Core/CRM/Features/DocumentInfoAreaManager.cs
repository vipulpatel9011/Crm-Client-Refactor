// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentInfoAreaManager.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   Document info area manager
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.Features
{
    using System;
    using System.Collections.Generic;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// The up document info area manager.
    /// </summary>
    public class DocumentInfoAreaManager
    {
        /// <summary>
        /// The crm query.
        /// </summary>
        private UPContainerMetaInfo crmQuery;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentInfoAreaManager"/> class.
        /// </summary>
        /// <param name="searchAndList">
        /// The search and list.
        /// </param>
        public DocumentInfoAreaManager(SearchAndList searchAndList)
            : this(
                CreateFieldControlForConstructor(searchAndList).Item1,
                CreateFieldControlForConstructor(searchAndList).Item2,
                0,
                null)
        {
            this.SearchAndList = searchAndList;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentInfoAreaManager"/> class.
        /// Creates a new instance of <see cref="DocumentInfoAreaManager"/>
        /// </summary>
        /// <param name="infoAreaId">
        /// InfoArea Id
        /// </param>
        /// <param name="fieldControl">
        /// Field Control object
        /// </param>
        /// <param name="recordIndex">
        /// Index of record
        /// </param>
        /// <param name="documentManager">
        /// Document manager
        /// </param>
        public DocumentInfoAreaManager(
            string infoAreaId,
            FieldControl fieldControl,
            int recordIndex,
            DocumentManager documentManager)
        {
            this.InfoAreaId = infoAreaId;
            this.DocumentManager = documentManager;
            this.FieldControl = fieldControl;
            var functionForFields = this.FieldControl.FunctionNames();
            this.ResultIndexOfDate = this.ResultPositionOfField(functionForFields.ValueOrDefault("Date"), 3, fieldControl);
            this.ResultIndexOfLength = this.ResultPositionOfField(functionForFields.ValueOrDefault("Length"), 2, fieldControl);
            this.ResultIndexOfMimeType = this.ResultPositionOfField(functionForFields.ValueOrDefault("MIMEType"), 0, fieldControl);
            this.ResultIndexOfTitle = this.ResultPositionOfField(functionForFields.ValueOrDefault("Title"), 1, fieldControl);
            this.ResultIndexOfDisplayText = this.ResultPositionOfField(
                functionForFields.ValueOrDefault("DisplayText"),
                -1,
                fieldControl);
            this.ResultIndexOfDisplayDate = this.ResultPositionOfField(
                functionForFields.ValueOrDefault("DisplayDate"),
                -1,
                fieldControl);
            this.ResultIndexOfUpdDate = this.ResultPositionOfField(functionForFields.ValueOrDefault("UpdDate"), 5, fieldControl);
            this.ResultIndexOfUpdTime = this.ResultPositionOfField(functionForFields.ValueOrDefault("UpdTime"), 6, fieldControl);
            this.RecordIndex = recordIndex;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentInfoAreaManager"/> class.
        /// Creates a new instance of <see cref="DocumentInfoAreaManager"/>
        /// </summary>
        /// <param name="infoAreaId">
        /// InfoArea Id
        /// </param>
        /// <param name="fieldControl">
        /// Field Control object
        /// </param>
        /// <param name="documentManager">
        /// Document manager
        /// </param>
        public DocumentInfoAreaManager(
            string infoAreaId,
            FieldControl fieldControl,
            DocumentManager documentManager)
            : this(infoAreaId, fieldControl, 0, documentManager)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentInfoAreaManager"/> class.
        /// Creates a new instance of <see cref="DocumentInfoAreaManager"/>
        /// </summary>
        /// <param name="infoAreaId">
        /// InfoArea Id
        /// </param>
        /// <param name="documentManager">
        /// Document manager
        /// </param>
        public DocumentInfoAreaManager(string infoAreaId, DocumentManager documentManager)
            : this(infoAreaId, GetFieldControlForConstructor(infoAreaId), documentManager)
        {
        }

        /// <summary>
        /// Gets the crm query.
        /// </summary>
        public UPContainerMetaInfo CrmQuery
        {
            get
            {
                if (this.crmQuery != null || this.FieldControl == null)
                {
                    return this.crmQuery;
                }

                if (!string.IsNullOrEmpty(this.SearchAndList.FilterName))
                {
                    UPConfigFilter filter = ConfigurationUnitStore.DefaultStore.FilterByName(this.SearchAndList.FilterName);
                    this.crmQuery = new UPContainerMetaInfo(this.FieldControl, filter, null);
                }
                else
                {
                    this.crmQuery = new UPContainerMetaInfo(this.FieldControl);
                }

                return this.crmQuery;
            }
        }

        /// <summary>
        /// Gets the document manager.
        /// </summary>
        public DocumentManager DocumentManager { get; private set; }

        /// <summary>
        /// Gets the field control.
        /// </summary>
        public FieldControl FieldControl { get; private set; }

        /// <summary>
        /// Gets the info area id.
        /// </summary>
        public string InfoAreaId { get; private set; }

        /// <summary>
        /// Gets the record index.
        /// </summary>
        public int RecordIndex { get; private set; }

        /// <summary>
        /// Gets the result index of date.
        /// </summary>
        public int ResultIndexOfDate { get; private set; }

        /// <summary>
        /// Gets the result index of display date.
        /// </summary>
        public int ResultIndexOfDisplayDate { get; private set; }

        /// <summary>
        /// Gets the result index of display text.
        /// </summary>
        public int ResultIndexOfDisplayText { get; private set; }

        /// <summary>
        /// Gets the result index of length.
        /// </summary>
        public int ResultIndexOfLength { get; private set; }

        /// <summary>
        /// Gets the result index of mime type.
        /// </summary>
        public int ResultIndexOfMimeType { get; private set; }

        /// <summary>
        /// Gets the result index of title.
        /// </summary>
        public int ResultIndexOfTitle { get; private set; }

        /// <summary>
        /// Gets the result index of upd date.
        /// </summary>
        public int ResultIndexOfUpdDate { get; private set; }

        /// <summary>
        /// Gets the result index of upd time.
        /// </summary>
        public int ResultIndexOfUpdTime { get; private set; }

        /// <summary>
        /// Gets the search and list.
        /// </summary>
        public SearchAndList SearchAndList { get; private set; }

        /// <summary>
        /// The crm query with filter parameter.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="UPContainerMetaInfo"/>.
        /// </returns>
        public UPContainerMetaInfo CrmQueryWithFilterParameter(Dictionary<string, object> parameter)
        {
            if (this.FieldControl == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(this.SearchAndList.FilterName))
            {
                UPConfigFilter filter = ConfigurationUnitStore.DefaultStore.FilterByName(this.SearchAndList.FilterName);
                this.crmQuery = new UPContainerMetaInfo(this.FieldControl, filter, parameter);
            }
            else
            {
                this.crmQuery = new UPContainerMetaInfo(this.FieldControl);
            }

            return this.crmQuery;
        }

        /// <summary>
        /// The document data for record id.
        /// </summary>
        /// <param name="recordId">
        /// The record id.
        /// </param>
        /// <returns>
        /// The <see cref="DocumentData"/>.
        /// </returns>
        public DocumentData DocumentDataForRecordId(string recordId)
        {
            UPContainerMetaInfo query = new UPContainerMetaInfo(this.FieldControl);
            query.SetLinkRecordIdentification(this.InfoAreaId.InfoAreaIdRecordId(recordId));
            UPCRMResult result = query.Find();
            return result.RowCount > 0 ? this.DocumentDataForResultRow(result.ResultRowAtIndex(0) as UPCRMResultRow) : null;
        }

        /// <summary>
        /// The create field control for constructor.
        /// </summary>
        /// <param name="searchAndList">
        /// The search and list.
        /// </param>
        /// <returns>
        /// The <see cref="Tuple"/>.
        /// </returns>
        private static Tuple<string, FieldControl> CreateFieldControlForConstructor(SearchAndList searchAndList)
        {
            var listFieldControl = ConfigurationUnitStore.DefaultStore.FieldControlByNameFromGroup(
                "List",
                searchAndList.FieldGroupName);
            return Tuple.Create(listFieldControl.InfoAreaId, listFieldControl);
        }

        /// <summary>
        /// The get field control for constructor.
        /// </summary>
        /// <param name="infoAreaId">
        /// The info area id.
        /// </param>
        /// <returns>
        /// The <see cref="FieldControl"/>.
        /// </returns>
        private static FieldControl GetFieldControlForConstructor(string infoAreaId)
        {
            string fieldGroupName = $"{infoAreaId}DocData";
            FieldControl fieldControl = ConfigurationUnitStore.DefaultStore.FieldControlByNameFromGroup(
                "List",
                fieldGroupName);
            return fieldControl;
        }

        /// <summary>
        /// The document data for result row.
        /// </summary>
        /// <param name="resultRow">
        /// The result row.
        /// </param>
        /// <returns>
        /// The <see cref="DocumentData"/>.
        /// </returns>
        public DocumentData DocumentDataForResultRow(UPCRMResultRow resultRow)
        {
            var tab = this.FieldControl.TabAtIndex(0);
            var title = this.ResultIndexOfTitle >= 0 ? resultRow.RawValueAtIndex(this.ResultIndexOfTitle) : null;
            var length = this.ResultIndexOfLength >= 0
                             ? resultRow.RawValueAtIndex(this.ResultIndexOfLength).ToUInt64()
                             : 0;
            var dateString = this.ResultIndexOfDate >= 0 ? resultRow.RawValueAtIndex(this.ResultIndexOfDate) : null;
            var mimeType = this.ResultIndexOfMimeType >= 0
                                  ? resultRow.RawValueAtIndex(this.ResultIndexOfMimeType)
                                  : null;
            var updDateString = this.ResultIndexOfUpdDate >= 0
                                       ? resultRow.RawValueAtIndex(this.ResultIndexOfUpdDate)
                                       : null;
            var updTimeString = this.ResultIndexOfUpdTime >= 0
                                       ? resultRow.RawValueAtIndex(this.ResultIndexOfUpdTime)
                                       : null;
            var displayText = this.ResultIndexOfDisplayText >= 0
                                     ? resultRow.FormattedFieldValueAtIndex(this.ResultIndexOfDisplayText, null, tab)
                                     : null;
            var displayDateString = this.ResultIndexOfDisplayDate >= 0
                                           ? resultRow.FormattedFieldValueAtIndex(this.ResultIndexOfDisplayDate, null, tab)
                                           : null;
            DateTime? documentDate = null;
            if (!string.IsNullOrEmpty(dateString))
            {
                documentDate = dateString.DateFromCrmValue();
                displayDateString = DateExtensions.LocalizedFormattedDate(documentDate);
            }

            DateTime? updateDate = null;
            if (!string.IsNullOrEmpty(updDateString))
            {
                updateDate = updDateString.DateTimeFromCrmValue();
            }

            string recordIdentification;
            if (resultRow.NumberOfRecordIds() > this.RecordIndex)
            {
                recordIdentification = resultRow.RecordIdentificationAtIndex(this.RecordIndex);
            }
            else
            {
                recordIdentification = resultRow.RootRecordIdentification;
            }

            string d1RecordId = resultRow.RecordIdentificationForLinkInfoAreaIdLinkId("D1", -1);
            return new DocumentData(
                recordIdentification,
                title,
                mimeType,
                documentDate,
                length,
                updateDate,
                displayText,
                displayDateString,
                d1RecordId);
        }

        /// <summary>
        /// The result position of field.
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <param name="defaultResultFieldIndex">
        /// The default result field index.
        /// </param>
        /// <param name="fieldControl">
        /// The field control.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int ResultPositionOfField(
            UPConfigFieldControlField field,
            int defaultResultFieldIndex,
            FieldControl fieldControl)
        {
            if (field != null)
            {
                return field.TabIndependentFieldIndex;
            }

            if (defaultResultFieldIndex < 0)
            {
                return defaultResultFieldIndex;
            }

            if (defaultResultFieldIndex < fieldControl.NumberOfFields && string.IsNullOrEmpty(fieldControl.FieldAtIndex(defaultResultFieldIndex).Function))
            {
                return defaultResultFieldIndex;
            }

            return -1;
        }
    }
}
