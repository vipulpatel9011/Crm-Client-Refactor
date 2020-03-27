// <copyright file="ContactTimesEditPageModelController.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>

namespace Aurea.CRM.Services.ModelControllers.ContactTimes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.Services.ModelControllers.Edit;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;
    using Aurea.CRM.UIModel.Pages;
    using Aurea.CRM.UIModel.Status;
    using Core.CRM.UIModel;
    using Core.Extensions;
    using UIModel.Fields.Edit;

    /// <summary>
    /// Contact times edit page model controller implementation
    /// </summary>
    public class ContactTimesEditPageModelController : EditPageModelController, ISearchOperationHandler
    {
        private const string AfternoonFrom = "AFTERNOONFROM";
        private const string AfternoonTo = "AFTERNOONTO";
        private const string From = "FROM";
        private const string PrefixOpen = "OPEN";
        private const string PrefixVisit = "VISIT";
        private const string PrefixPhone = "PHONE";
        private const string TimeType0 = "0";
        private const string TimeType1 = "1";
        private const string TimeType2 = "2";
        private const string TimeStampDayStart = "0000";
        private const string TimeStampDayEnd = "23:59";
        private const string To = "TO";
        private const string Delete = "Delete";
        private UPMContactTimesGroup contactTimesGroup;
        private UPCRMResult contextResult;
        private UPContainerMetaInfo crmQuery;
        private string infoAreaid;
        private string linkRecordIdentification;
        private SearchAndList searchAndList;
        private string searchAndListConfigurationName;
        private int[] weekDayArray = new int[7];

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactTimesEditPageModelController"/> class.
        /// </summary>
        /// <param name="viewReference">View reference</param>
        public ContactTimesEditPageModelController(ViewReference viewReference)
            : base(viewReference)
        {
            this.RecordIdentification = this.ViewReference.ContextValueForKey("RecordId");
            this.searchAndListConfigurationName = this.ViewReference.ContextValueForKey("SearchList");
            if (string.IsNullOrEmpty(this.searchAndListConfigurationName))
            {
                this.searchAndListConfigurationName = "U001";
            }

            var configStore = ConfigurationUnitStore.DefaultStore;
            this.searchAndList = configStore.SearchAndListByName(this.searchAndListConfigurationName);
            if (this.searchAndList != null)
            {
                this.infoAreaid = this.searchAndList.InfoAreaId;
                this.FieldControl = configStore.FieldControlByNameFromGroup("Edit", this.searchAndList.FieldGroupName);
            }
            else
            {
                this.FieldControl = configStore.FieldControlByNameFromGroup("Edit", this.searchAndListConfigurationName);
            }

            for (int weekDay = 0; weekDay < 7; weekDay++)
            {
                var day = (int)DayOfWeek.Sunday + (weekDay + 1);
                this.weekDayArray[weekDay] = day > 7 ? (day % 7) : day;
            }

            this.BuildPage();
        }

        /// <summary>
        /// Gets record identification
        /// </summary>
        public string RecordIdentification { get; private set; }

        /// <inheritdoc/>
        public override void ApplyLoadingStatusOnPage(Page page)
        {
            UPMProgressStatus loadinfStatus = new UPMProgressStatus(StringIdentifier.IdentifierWithStringId("loadingIdentifier"));
            UPMStringField statusField = new UPMStringField(StringIdentifier.IdentifierWithStringId("statusFieldIdentifier"))
            {
                FieldValue = LocalizedString.TextLoadingData
            };
            loadinfStatus.StatusMessageField = statusField;
            page.Status = loadinfStatus;
        }

        /// <inheritdoc/>
        public sealed override void BuildPage()
        {
            var page = this.InstantiatePage();
            page.Invalid = true;
            this.TopLevelElement = page;
            this.ApplyLoadingStatusOnPage(page);
        }

        /// <inheritdoc/>
        public override List<UPCRMRecord> ChangedChildRecordsForParentRecord(UPCRMRecord parentRecord, bool userChangesOnly)
        {
            return !this.FieldControl.FunctionNames().Keys.Contains("TYPE") ? this.ChangedRecordsVrianteWithoutType() : this.ChangedRecordsVrianteWithType();
        }

        /// <inheritdoc/>
        public override Page InstantiatePage()
        {
            return new EditPage(StringIdentifier.IdentifierWithStringId("ContactTimes"));
        }

        /// <inheritdoc/>
        public override void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            if (this.RequestOption == UPRequestOption.BestAvailable || this.RequestOption == UPRequestOption.FastestAvailable)
            {
                UPCRMResult result = this.crmQuery.Find();
                this.Page.AddGroup(this.GroupFromResult(result));
                this.Page.Status = null;
                this.Page.Invalid = false;
                this.InformAboutDidChangeTopLevelElement(this.Page, this.Page, null, null);
            }
            else
            {
                this.ShowReadError(error);
            }
        }

        /// <inheritdoc/>
        public override void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            this.Page.AddGroup(this.GroupFromResult(result));
            this.Page.Status = null;
            this.Page.Invalid = false;
            this.InformAboutDidChangeTopLevelElement(this.Page, this.Page, null, null);
        }

        /// <inheritdoc/>
        public override UPMElement UpdatedElementForPage(Page page)
        {
            Page newPage = this.InstantiatePage();
            newPage.Invalid = true;
            this.TopLevelElement = newPage;
            this.ApplyLoadingStatusOnPage(newPage);
            this.crmQuery = this.BuildCrmQuery();
            if (this.crmQuery == null)
            {
                return null;
            }

            this.linkRecordIdentification = this.RecordIdentification;
            if (!string.IsNullOrEmpty(this.linkRecordIdentification))
            {
                this.crmQuery.SetLinkRecordIdentification(this.linkRecordIdentification, 0);
            }

            if (this.RequestOption == UPRequestOption.Offline || this.RequestOption == UPRequestOption.FastestAvailable)
            {
                UPCRMResult result = this.crmQuery.Find();
                newPage.Status = null;
                if (result.RowCount > 0)
                {
                    newPage.AddGroup(this.GroupFromResult(result));
                    return newPage;
                }
            }

            if (this.RequestOption != UPRequestOption.Offline)
            {
                var remoteOperation = this.crmQuery.Find(this);
                if (remoteOperation == null)
                {
                    newPage.Status = null;
                }
            }
            else
            {
                newPage.AddGroup(this.EmptyGroup());
            }

            return newPage;
        }

        private void AddValuesToRecordContextRecordId(Dictionary<string, object> functionValueMapping, UPCRMRecord changedRecord, string contectRecordIdentification)
        {
            Dictionary<string, object> contextValuesWithFunctions = null;
            for (int row = 0; row < this.contextResult.RowCount; row++)
            {
                var resultRow = (UPCRMResultRow)this.contextResult.ResultRowAtIndex(row);
                if (resultRow.RecordIdentificationAtFieldIndex(0) == contectRecordIdentification)
                {
                    contextValuesWithFunctions = resultRow.ValuesWithFunctions();
                    break;
                }
            }

            foreach (string function in functionValueMapping.Keys)
            {
                if (contextValuesWithFunctions == null)
                {
                    if (functionValueMapping.ValueOrDefault(function) != null)
                    {
                        changedRecord.NewValueFieldId(functionValueMapping.ValueOrDefault(function) as string,
                            this.FieldControl.FunctionNames().ValueOrDefault(function).FieldId);
                    }
                    else
                    {
                        changedRecord.NewValueFieldId(null, this.FieldControl.FunctionNames().ValueOrDefault(function).FieldId);
                    }
                }
                else
                {
                    if (functionValueMapping.ValueOrDefault(function) != null)
                    {
                        changedRecord.NewValueFromValueFieldId(
                            functionValueMapping.ValueOrDefault(function) as string,
                            contextValuesWithFunctions.ValueOrDefault(function) as string,
                            this.FieldControl.FunctionNames().ValueOrDefault(function).FieldId);
                    }
                    else
                    {
                        changedRecord.NewValueFromValueFieldId(null, contextValuesWithFunctions.ValueOrDefault(function) as string,
                            this.FieldControl.FunctionNames().ValueOrDefault(function).FieldId);
                    }
                }
            }
        }

        private UPContainerMetaInfo BuildCrmQuery()
        {
            if (this.FieldControl != null)
            {
                this.crmQuery = new UPContainerMetaInfo(this.FieldControl);
                if (!string.IsNullOrEmpty(this.searchAndList.FilterName))
                {
                    UPConfigFilter filter = ConfigurationUnitStore.DefaultStore.FilterByName(this.searchAndList.FilterName);
                    if (filter != null)
                    {
                        this.crmQuery.ApplyFilter(filter);
                    }
                }
            }

            return this.crmQuery;
        }

        /// <summary>
        /// Method changes record variante, not having TYPE key.
        /// </summary>
        /// <returns>
        /// List of <see cref="UPCRMRecords" />.
        /// </returns>
        private List<UPCRMRecord> ChangedRecordsVrianteWithoutType()
        {
            var changedRecords = new List<UPCRMRecord>();
            for (var weekDay = 0; weekDay < weekDayArray.Length; weekDay++)
            {
                var functionValueMapping = new Dictionary<string, object>();
                var functionValueMappingNullValue = new Dictionary<string, object>();
                var changedRecord = (UPCRMRecord)null;
                var weekDayU7 = weekDayArray[weekDay];
                if (weekDayU7 > 1)
                {
                    weekDayU7--;
                }
                else
                {
                    weekDayU7 = 7;
                }

                var contactRecordIdentification = SetFunctionValueMapping(weekDay, functionValueMapping, functionValueMappingNullValue);
                functionValueMapping["DAYOFWEEK"] = $"{weekDayU7}";
                if (contactRecordIdentification != null && functionValueMapping.Keys.Count == 1)
                {
                    changedRecord = new UPCRMRecord(contactRecordIdentification, "Delete");
                }
                else if (contactRecordIdentification == null && functionValueMapping.Keys.Count > 1)
                {
                    changedRecord = new UPCRMRecord(infoAreaid);
                    changedRecord.AddLink(new UPCRMLink(RecordIdentification));
                    functionValueMapping.Append(functionValueMappingNullValue);
                    AddValuesToRecordContextRecordId(functionValueMapping, changedRecord, contactRecordIdentification);
                }
                else if (contactRecordIdentification != null && !IsRecordEqual(contactRecordIdentification, functionValueMapping))
                {
                    changedRecord = new UPCRMRecord(contactRecordIdentification);
                    functionValueMapping.Append(functionValueMappingNullValue);
                    AddValuesToRecordContextRecordId(functionValueMapping, changedRecord, contactRecordIdentification);
                }

                if (changedRecord != null)
                {
                    changedRecords.Add(changedRecord);
                }
            }

            return changedRecords;
        }

        /// <summary>
        /// Method caclulates and set To,From range in functionValueMapping collection
        /// </summary>
        /// <param name="weekDay">
        /// weekday to process
        /// </param>
        /// <param name="functionValueMapping">
        /// Dictionary to store To/From time range.
        /// </param>
        /// <param name="functionValueMappingNullValue">
        /// Dictionary to store To/From null values if any.
        /// </param>
        /// <returns>
        /// contactRecordIdentification string
        /// </returns>
        private string SetFunctionValueMapping(int weekDay, Dictionary<string, object> functionValueMapping, Dictionary<string, object> functionValueMappingNullValue)
        {
            var contactRecordIdentification = string.Empty;
            foreach (var timeType in contactTimesGroup.TimeTypeTitleSortedKeys)
            {
                var prefix = timeType == TimeType0
                    ? PrefixOpen
                    : timeType == TimeType1
                        ? PrefixVisit
                        : timeType == TimeType2
                            ? PrefixPhone
                            : string.Empty;

                var from = $"{prefix}FROM";
                var to = $"{prefix}TO";
                var afternoonFrom = $"{prefix}AFTERNOONFROM";
                var afternoonTo = $"{prefix}AFTERNOONTO";
                var contactTimes = contactTimesGroup.ContactTimeForWeekDayTimeType(weekDayArray[weekDay], timeType);
                var contactTime = (UPMContactTime)null;
                var afternoonContactTime = (UPMContactTime)null;
                if (contactTimes.Count >= 2)
                {
                    contactTime = contactTimes[0];
                    afternoonContactTime = contactTimes[1];
                }
                else if (contactTimes.Count == 1)
                {
                    contactTime = contactTimes[0];
                }

                if (!string.IsNullOrWhiteSpace(contactTime?.RecordIdentification))
                {
                    contactRecordIdentification = contactTime.RecordIdentification;
                }
                else if (!string.IsNullOrWhiteSpace(afternoonContactTime?.RecordIdentification))
                {
                    contactRecordIdentification = afternoonContactTime.RecordIdentification;
                }

                var fromEditTime = contactTime?.FromTime.DateValue.CrmValueFromTime();
                var toEditTime = contactTime?.ToTime.DateValue.CrmValueFromTime();
                var afternoonFromEditTime = afternoonContactTime?.FromTime.DateValue.CrmValueFromTime();
                var afternoonToEditTime = afternoonContactTime?.ToTime.DateValue.CrmValueFromTime();
                if (fromEditTime == TimeStampDayStart && toEditTime == TimeStampDayStart)
                {
                    toEditTime = TimeStampDayEnd;
                }

                if (afternoonFromEditTime == TimeStampDayStart && afternoonToEditTime == TimeStampDayStart)
                {
                    afternoonToEditTime = TimeStampDayEnd;
                }

                functionValueMapping[from] = fromEditTime;
                functionValueMapping[to] = toEditTime;
                functionValueMapping[afternoonFrom] = afternoonFromEditTime;
                functionValueMapping[afternoonTo] = afternoonToEditTime;
                if (functionValueMapping.ValueOrDefault(from) == null)
                {
                    functionValueMappingNullValue[from] = null;
                }

                if (functionValueMapping.ValueOrDefault(to) == null)
                {
                    functionValueMappingNullValue[to] = null;
                }

                if (functionValueMapping.ValueOrDefault(afternoonFrom) == null)
                {
                    functionValueMappingNullValue[afternoonFrom] = null;
                }

                if (functionValueMapping.ValueOrDefault(afternoonTo) == null)
                {
                    functionValueMappingNullValue[afternoonTo] = null;
                }
            }

            return contactRecordIdentification;
        }

        /// <summary>
        /// Method changes record variante, having TYPE key.
        /// </summary>
        /// <returns>
        /// List of <see cref="UPCRMRecord"/>.
        /// </returns>
        private List<UPCRMRecord> ChangedRecordsVrianteWithType()
        {
            var changedRecords = new List<UPCRMRecord>();
            for (var weekDay = 0; weekDay < weekDayArray.Length; weekDay++)
            {
                var weekDayU7 = weekDayArray[weekDay];
                if (weekDayU7 > 1)
                {
                    weekDayU7--;
                }
                else
                {
                    weekDayU7 = 7;
                }

                changedRecords.AddRange(PopulateChangedUPCRMRecordRecords(weekDay, weekDayU7));
            }

            return changedRecords;
        }

        private UPMContactTimesGroup EmptyGroup()
        {
            this.contactTimesGroup = new UPMContactTimesGroup(StringIdentifier.IdentifierWithStringId("ContactTimesGroup"));
            var functionNames = this.FieldControl.FunctionNames();
            if (!functionNames.Keys.Contains("TYPE"))
            {
                if (functionNames.Keys.Contains("OPENFROM") && functionNames.Keys.Contains("OPENTO"))
                {
                    this.contactTimesGroup.AddTitleForTimeType(LocalizedString.TextOpeningTimes, "0");
                }

                if (functionNames.Keys.Contains("VISITFROM") && functionNames.Keys.Contains("VISITTO"))
                {
                    this.contactTimesGroup.AddTitleForTimeType(LocalizedString.TextVisitTimes, "1");
                }

                if (functionNames.Keys.Contains("PHONEFROM") && functionNames.Keys.Contains("PHONETO"))
                {
                    this.contactTimesGroup.AddTitleForTimeType(LocalizedString.TextPhoneTimes, "2");
                }
            }
            else
            {
                var field = this.FieldControl.FieldWithFunction("TYPE");
                var catalog = field.Field.Catalog;
                var sortedValues = catalog.SortedValues;
                foreach (string key in sortedValues)
                {
                    var catalogValue = catalog.ValueForCode(key.ToInt());
                    if (catalogValue.Access == 0)
                    {
                        this.contactTimesGroup.AddTitleForTimeType(catalogValue.Text, key);
                    }
                }
            }

            return this.contactTimesGroup;
        }

        private UPMContactTimesGroup FillGroupVrianteWithoutType(UPCRMResult result)
        {
            this.contactTimesGroup = this.EmptyGroup();
            var functionNames = this.FieldControl.FunctionNames();
            for (int row = 0; row < result.RowCount; row++)
            {
                var resultRow = result.ResultRowAtIndex(row) as UPCRMResultRow;
                if (functionNames.Keys.Contains("OPENFROM") && functionNames.Keys.Contains("OPENTO"))
                {
                    this.contactTimesGroup.AddTitleForTimeType(LocalizedString.TextOpeningTimes, "0");
                    this.FillTimesVrianteWithoutTypeFromResultRowTimeTypePrefix(resultRow, "0", "OPEN");
                }

                if (functionNames.Keys.Contains("VISITFROM") && functionNames.Keys.Contains("VISITTO"))
                {
                    this.contactTimesGroup.AddTitleForTimeType(LocalizedString.TextVisitTimes, "1");
                    this.FillTimesVrianteWithoutTypeFromResultRowTimeTypePrefix(resultRow, "1", "VISIT");
                }

                if (functionNames.Keys.Contains("PHONEFROM") && functionNames.Keys.Contains("PHONETO"))
                {
                    this.contactTimesGroup.AddTitleForTimeType(LocalizedString.TextPhoneTimes, "2");
                    this.FillTimesVrianteWithoutTypeFromResultRowTimeTypePrefix(resultRow, "2", "PHONE");
                }
            }

            return this.contactTimesGroup;
        }

        private UPMContactTimesGroup FillGroupVrianteWithType(UPCRMResult result)
        {
            this.contactTimesGroup = this.EmptyGroup();
            for (int row = 0; row < result.RowCount; row++)
            {
                var resultRow = (UPCRMResultRow)result.ResultRowAtIndex(row);
                var valuesWithFunctions = resultRow.ValuesWithFunctions();
                this.FillTimesVrianteWithTypeFromResultRowTimeType(resultRow, valuesWithFunctions.ValueOrDefault("TYPE") as string);
            }

            return this.contactTimesGroup;
        }

        private void FillTimesVrianteWithoutTypeFromResultRowTimeTypePrefix(UPCRMResultRow resultRow, string timeType, string prefix)
        {
            var valuesWithFunctions = resultRow.ValuesWithFunctions();
            DateTime fromDate;
            DateTime toDate;
            UPMDateTimeEditField fromTime;
            UPMDateTimeEditField toTime;
            UPMContactTime contactTime;
            var from = $"{prefix}FROM";
            var to = $"{prefix}TO";
            var afternoonFrom = $"{prefix}AFTERNOONFROM";
            var afternoonTo = $"{prefix}AFTERNOONTO";
            int weekDay = ((string)valuesWithFunctions.ValueOrDefault("DAYOFWEEK")).ToInt();
            if (weekDay < 7)
            {
                weekDay++;
            }
            else
            {
                weekDay = 1;
            }

            if (valuesWithFunctions.ValueOrDefault(to) != null && ((string)valuesWithFunctions.ValueOrDefault(to)).ToInt() > 0)
            {
                fromDate = (valuesWithFunctions.ValueOrDefault(from) as string).TimeFromCrmValue();
                toDate = (valuesWithFunctions.ValueOrDefault(to) as string).TimeFromCrmValue();
                fromTime = new UPMDateTimeEditField(StringIdentifier.IdentifierWithStringId("fromTime"))
                {
                    Type = DateTimeType.Time,
                    DateValue = fromDate
                };

                toTime = new UPMDateTimeEditField(StringIdentifier.IdentifierWithStringId("toTime"))
                {
                    Type = DateTimeType.Time,
                    DateValue = toDate
                };

                contactTime = new UPMContactTime(weekDay, timeType, fromTime, toTime)
                {
                    RecordIdentification = resultRow.RecordIdentificationAtFieldIndex(0)
                };
                this.contactTimesGroup.AddChild(contactTime);
            }
            else
            {
                contactTime = new UPMContactTime(weekDay, timeType, null, null);
                this.contactTimesGroup.AddChild(contactTime);
            }

            if (valuesWithFunctions.ValueOrDefault(afternoonTo) != null && ((string)valuesWithFunctions.ValueOrDefault(afternoonTo)).ToInt() > 0)
            {
                fromDate = (valuesWithFunctions.ValueOrDefault(afternoonFrom) as string).TimeFromCrmValue();
                toDate = (valuesWithFunctions.ValueOrDefault(afternoonTo) as string).TimeFromCrmValue();
                fromTime = new UPMDateTimeEditField(StringIdentifier.IdentifierWithStringId("fromTime"))
                {
                    Type = DateTimeType.Time,
                    DateValue = fromDate
                };
                toTime = new UPMDateTimeEditField(StringIdentifier.IdentifierWithStringId("toTime"))
                {
                    Type = DateTimeType.Time,
                    DateValue = toDate
                };
                contactTime = new UPMContactTime(weekDay, timeType, fromTime, toTime)
                {
                    RecordIdentification = resultRow.RecordIdentificationAtFieldIndex(0)
                };
                this.contactTimesGroup.AddChild(contactTime);
            }
            else
            {
                contactTime = new UPMContactTime(weekDay, timeType, null, null)
                {
                    RecordIdentification = resultRow.RecordIdentificationAtFieldIndex(0)
                };
                this.contactTimesGroup.AddChild(contactTime);
            }
        }

        private void FillTimesVrianteWithTypeFromResultRowTimeType(UPCRMResultRow resultRow, string timeType)
        {
            if (string.IsNullOrEmpty(this.contactTimesGroup.TimeTypeTitleForTimeType(timeType)))
            {
                return;
            }

            var valuesWithFunctions = resultRow.ValuesWithFunctions();
            DateTime fromDate;
            DateTime toDate;
            UPMDateTimeEditField fromTime;
            UPMDateTimeEditField toTime;
            UPMContactTime contactTime;
            string from = "FROM";
            string to = "TO";
            string afternoonFrom = "AFTERNOONFROM";
            string afternoonTo = "AFTERNOONTO";
            int weekDay = ((string)valuesWithFunctions.ValueOrDefault("DAYOFWEEK")).ToInt();
            if (weekDay < 7)
            {
                weekDay++;
            }
            else
            {
                weekDay = 1;
            }

            if ((valuesWithFunctions.ValueOrDefault(from) != null && ((string)valuesWithFunctions.ValueOrDefault(from)).ToInt() > 0)
                || (valuesWithFunctions.ValueOrDefault(to) != null && ((string)valuesWithFunctions.ValueOrDefault(to)).ToInt() > 0))
            {
                fromDate = ((string)valuesWithFunctions.ValueOrDefault(from)).TimeFromCrmValue();
                toDate = ((string)valuesWithFunctions.ValueOrDefault(to)).TimeFromCrmValue();
                fromTime = new UPMDateTimeEditField(StringIdentifier.IdentifierWithStringId("fromTime"))
                {
                    Type = DateTimeType.Time,
                    DateValue = fromDate
                };
                toTime = new UPMDateTimeEditField(StringIdentifier.IdentifierWithStringId("toTime"))
                {
                    Type = DateTimeType.Time,
                    DateValue = toDate
                };
                contactTime = new UPMContactTime(weekDay, timeType, fromTime, toTime)
                {
                    RecordIdentification = resultRow.RecordIdentificationAtFieldIndex(0)
                };
                this.contactTimesGroup.AddChild(contactTime);
            }

            if ((valuesWithFunctions.ValueOrDefault(afternoonFrom) != null && ((string)valuesWithFunctions.ValueOrDefault(afternoonFrom)).ToInt() > 0)
                || (valuesWithFunctions.ValueOrDefault(afternoonTo) != null && ((string)valuesWithFunctions.ValueOrDefault(afternoonTo)).ToInt() > 0))
            {
                fromDate = ((string)valuesWithFunctions.ValueOrDefault(afternoonFrom)).TimeFromCrmValue();
                toDate = ((string)valuesWithFunctions.ValueOrDefault(afternoonTo)).TimeFromCrmValue();
                fromTime = new UPMDateTimeEditField(StringIdentifier.IdentifierWithStringId("fromTime"))
                {
                    Type = DateTimeType.Time,
                    DateValue = fromDate
                };
                toTime = new UPMDateTimeEditField(StringIdentifier.IdentifierWithStringId("toTime"))
                {
                    Type = DateTimeType.Time,
                    DateValue = toDate
                };
                contactTime = new UPMContactTime(weekDay, timeType, fromTime, toTime)
                {
                    RecordIdentification = resultRow.RecordIdentificationAtFieldIndex(0)
                };
                this.contactTimesGroup.AddChild(contactTime);
            }
        }

        private UPMContactTimesGroup GroupFromResult(UPCRMResult result)
        {
            this.contextResult = result;
            if (!this.FieldControl.FunctionNames().Keys.Contains("TYPE"))
            {
                this.FillGroupVrianteWithoutType(this.contextResult);
            }
            else
            {
                this.FillGroupVrianteWithType(this.contextResult);
            }

            for (int weekDay = 0; weekDay < 7; weekDay++)
            {
                int weekDayU7 = this.weekDayArray[weekDay];
                if (weekDayU7 > 1)
                {
                    weekDayU7--;
                }
                else
                {
                    weekDayU7 = 7;
                }

                foreach (string timeType in this.contactTimesGroup.TimeTypeTitleSortedKeys)
                {
                    var contactTimes = this.contactTimesGroup.ContactTimeForWeekDayTimeType(weekDayU7, timeType);
                    if (contactTimes.Count < 1)
                    {
                        UPMContactTime contactTime = new UPMContactTime(weekDayU7, timeType, null, null);
                        UPMContactTime contactTime2 = new UPMContactTime(weekDayU7, timeType, null, null);
                        this.contactTimesGroup.AddChild(contactTime);
                        this.contactTimesGroup.AddChild(contactTime2);
                    }
                    else if (contactTimes.Count < 2)
                    {
                        UPMContactTime contactTime = new UPMContactTime(weekDayU7, timeType, null, null);
                        this.contactTimesGroup.AddChild(contactTime);
                    }
                }
            }

            this.contactTimesGroup.Name = "EDIT";
            return this.contactTimesGroup;
        }

        private bool IsRecordEqual(string contactRecordIdentification, Dictionary<string, object> functionValueMapping)
        {
            var newFunctionValueMapping = new Dictionary<string, object>(functionValueMapping);
            var keys = newFunctionValueMapping.Where(a => (string)a.Value == "0000").Select(a => a.Key);
            newFunctionValueMapping = newFunctionValueMapping.RemoveObjectsForKeys(keys);
            for (int row = 0; row < this.contextResult.RowCount; row++)
            {
                var resultRow = (UPCRMResultRow)this.contextResult.ResultRowAtIndex(row);
                if (resultRow.RecordIdentificationAtFieldIndex(0) == contactRecordIdentification)
                {
                    var rowValuesWithFunctions = new Dictionary<string, object>(resultRow.ValuesWithFunctions());
                    keys = rowValuesWithFunctions.Where(a => (string)a.Value == "0000").Select(a => a.Key);
                    rowValuesWithFunctions.RemoveObjectsForKeys(keys);
                    return newFunctionValueMapping.IsEqualToDictionary(rowValuesWithFunctions);
                }
            }

            return false;
        }

        /// <summary>
        /// Populate Changed UPCRMRecords
        /// </summary>
        /// <param name="weekDay">
        /// day of week
        /// </param>
        /// <param name="weekDayU7">
        /// day of week, under 7
        /// </param>
        /// <returns>
        /// List of <see cref="UPCRMRecord"/>
        /// </returns>
        private List<UPCRMRecord> PopulateChangedUPCRMRecordRecords(int weekDay, int weekDayU7)
        {
            var changedRecords = new List<UPCRMRecord>();
            foreach (var timeType in contactTimesGroup.TimeTypeTitleSortedKeys)
            {
                var afternoonContactTime = (UPMContactTime)null;
                var contactTime = (UPMContactTime)null;
                var contactTimes = contactTimesGroup.ContactTimeForWeekDayTimeType(weekDayArray[weekDay], timeType);
                var functionValueMapping = new Dictionary<string, object>();
                var functionValueMappingNullValue = new Dictionary<string, object>();
                var contactRecordIdentification = (string)null;
                if (contactTimes.Count == 2)
                {
                    contactTime = contactTimes[0];
                    afternoonContactTime = contactTimes[1];
                }
                else if (contactTimes.Count == 1)
                {
                    contactTime = contactTimes[0];
                }

                if (!string.IsNullOrWhiteSpace(contactTime?.RecordIdentification))
                {
                    contactRecordIdentification = contactTime.RecordIdentification;
                }
                else if (!string.IsNullOrWhiteSpace(afternoonContactTime?.RecordIdentification))
                {
                    contactRecordIdentification = afternoonContactTime.RecordIdentification;
                }

                var fromEditTime = contactTime?.FromTime.DateValue.CrmValueFromTime();
                var toEditTime = contactTime?.ToTime.DateValue.CrmValueFromTime();
                var afternoonFromEditTime = afternoonContactTime?.FromTime.DateValue.CrmValueFromTime();
                var afternoonToEditTime = afternoonContactTime?.ToTime.DateValue.CrmValueFromTime();
                if (fromEditTime == TimeStampDayStart && toEditTime == TimeStampDayStart)
                {
                    toEditTime = TimeStampDayEnd;
                }

                if (afternoonFromEditTime == TimeStampDayStart && afternoonFromEditTime == TimeStampDayStart)
                {
                    afternoonToEditTime = TimeStampDayEnd;
                }

                if (!string.IsNullOrWhiteSpace(fromEditTime))
                {
                    functionValueMapping[From] = fromEditTime;
                }

                if (!string.IsNullOrWhiteSpace(toEditTime))
                {
                    functionValueMapping[To] = toEditTime;
                }

                if (!string.IsNullOrWhiteSpace(afternoonFromEditTime))
                {
                    functionValueMapping[AfternoonFrom] = afternoonFromEditTime;
                }

                if (!string.IsNullOrWhiteSpace(afternoonToEditTime))
                {
                    functionValueMapping[AfternoonTo] = afternoonToEditTime;
                }

                if (functionValueMapping.ValueOrDefault(From) == null)
                {
                    functionValueMappingNullValue.SetObjectForKey(null, From);
                }

                if (functionValueMapping.ValueOrDefault(To) == null)
                {
                    functionValueMappingNullValue.SetObjectForKey(null, To);
                }

                if (functionValueMapping.ValueOrDefault(AfternoonFrom) == null)
                {
                    functionValueMappingNullValue.SetObjectForKey(null, AfternoonFrom);
                }

                if (functionValueMapping.ValueOrDefault(AfternoonTo) == null)
                {
                    functionValueMappingNullValue.SetObjectForKey(null, AfternoonTo);
                }

                var changedRecord = CreateChangedUPCRMRecordRecord(
                    functionValueMapping,
                    functionValueMappingNullValue,
                    timeType,
                    contactRecordIdentification,
                    weekDayU7);

                if (changedRecord != null)
                {
                    changedRecords.Add(changedRecord);
                }
            }

            return changedRecords;
        }

        /// <summary>
        /// Create Changed UPCRMRecord
        /// </summary>
        /// <param name="functionValueMapping">
        /// Dictionary to store To/From time range.
        /// </param>
        /// <param name="functionValueMappingNullValue">
        /// Dictionary to store To/From null values if any.
        /// </param>
        /// <param name="timeType">
        /// timeType
        /// </param>
        /// <param name="contactRecordIdentification">
        /// contact Record Id
        /// </param>
        /// <param name="weekDayU7">
        /// weekday under 7
        /// </param>
        /// <returns>
        /// <see cref="UPCRMRecord"/>
        /// </returns>
        private UPCRMRecord CreateChangedUPCRMRecordRecord(
            Dictionary<string, object> functionValueMapping,
            Dictionary<string, object> functionValueMappingNullValue,
            string timeType,
            string contactRecordIdentification,
            int weekDayU7)
        {
            var changedRecord = (UPCRMRecord)null;
            functionValueMapping["DAYOFWEEK"] = $"{weekDayU7}";
            functionValueMapping["TYPE"] = timeType;
            if (contactRecordIdentification != null && functionValueMapping.Keys.Count == 2)
            {
                changedRecord = new UPCRMRecord(contactRecordIdentification, Delete, null);
            }
            else if (contactRecordIdentification == null && functionValueMapping.Keys.Count > 2)
            {
                changedRecord = UPCRMRecord.CreateNew(infoAreaid);
                changedRecord.AddLink(new UPCRMLink(RecordIdentification));

                functionValueMapping.Append(functionValueMappingNullValue);

                AddValuesToRecordContextRecordId(functionValueMapping, changedRecord, contactRecordIdentification);
            }
            else if (contactRecordIdentification != null && !IsRecordEqual(contactRecordIdentification, functionValueMapping))
            {
                changedRecord = new UPCRMRecord(contactRecordIdentification);

                functionValueMapping.Append(functionValueMappingNullValue);

                AddValuesToRecordContextRecordId(functionValueMapping, changedRecord, contactRecordIdentification);
            }

            return changedRecord;
        }
    }
}
