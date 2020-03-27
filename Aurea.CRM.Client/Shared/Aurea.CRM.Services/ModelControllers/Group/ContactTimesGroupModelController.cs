// <copyright file="ContactTimesGroupModelController.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//   Serdar Tepeyurt
// </author>

namespace Aurea.CRM.Services.ModelControllers.Group
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;
    using Core.CRM.UIModel;
    using UIModel.Fields.Edit;

    /// <summary>
    /// Contact times group model controller implementation
    /// </summary>
    public class ContactTimesGroupModelController : UPFieldControlBasedGroupModelController, ISearchOperationHandler
    {
        private UPMContactTimesGroup contactTimesGroup;
        private string searchAndListConfigurationName;
        private string linkRecordIdentification;
        private FieldControl fieldControl;
        private SearchAndList searchAndList;
        private int linkId;
        private string infoAreaid;
        private UPContainerMetaInfo crmQuery;
        private int maxResults = 0;
        private UPCRMResult contextResult;
        private Exception error;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactTimesGroupModelController"/> class.
        /// </summary>
        /// <param name="fieldControl">Field Control</param>
        /// <param name="tabIndex">Tab Index</param>
        /// <param name="controllerDelegate">Controller Delegate</param>
        public ContactTimesGroupModelController(FieldControl fieldControl, int tabIndex, IGroupModelControllerDelegate controllerDelegate)
            : base(fieldControl, tabIndex, controllerDelegate)
        {
            var typeParts = this.TabConfig.Type.Split('_');
            this.linkId = -1;
            if (typeParts?.Length > 1)
            {
                this.searchAndListConfigurationName = typeParts[1];
            }

            if (this.searchAndListConfigurationName.Length == 0)
            {
                this.searchAndListConfigurationName = "U001";
            }

            var configStore = ConfigurationUnitStore.DefaultStore;
            this.searchAndList = configStore.SearchAndListByName(this.searchAndListConfigurationName);
            if (this.searchAndList != null)
            {
                this.infoAreaid = this.searchAndList.InfoAreaId;
                this.fieldControl = configStore.FieldControlByNameFromGroup("List", this.searchAndList.FieldGroupName);
            }
            else
            {
                this.fieldControl = configStore.FieldControlByNameFromGroup("List", this.searchAndListConfigurationName);
            }
        }

        /// <inheritdoc />
        public override Exception Error
        {
            get
            {
                return this.error;
            }
        }

        /// <inheritdoc />
        public override UPMGroup Group => this.contactTimesGroup;

        /// <inheritdoc />
        public override UPMGroup ApplyResultRow(UPCRMResultRow row)
        {
            string recordIdentification = row.RootRecordIdentification;
            UPMGroup group = this.ApplyLinkRecordIdentification(recordIdentification);
            group?.Actions.AddRange(this.BuildAdditionalActionsForRecordIdentification(recordIdentification));
            return group;
        }

        /// <inheritdoc />
        public void SearchOperationDidFailWithError(Operation operation, Exception exception)
        {
            if ((this.RequestOption == UPRequestOption.BestAvailable || this.RequestOption == UPRequestOption.FastestAvailable) && this.error.IsConnectionOfflineError())
            {
                UPCRMResult result = this.crmQuery.Find();
                if (result?.RowCount > 0)
                {
                    this.controllerState = GroupModelControllerState.Finished;
                    this.contactTimesGroup = this.GroupFromResult(result);
                }
                else
                {
                    this.contactTimesGroup = null;
                    return;
                }
            }
            else
            {
                this.controllerState = GroupModelControllerState.Error;
                this.error = exception;
                this.contactTimesGroup = null;
            }

            this.Delegate.GroupModelControllerFinished(this);
        }

        /// <inheritdoc />
        public void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            if (result.RowCount > 0)
            {
                this.controllerState = GroupModelControllerState.Finished;
                this.contactTimesGroup = this.GroupFromResult(result);
                this.controllerState = base.Group != null ? GroupModelControllerState.Finished : GroupModelControllerState.Empty;
                this.Delegate.GroupModelControllerFinished(this);
            }
            else
            {
                this.Delegate.GroupModelControllerFinished(this);
            }
        }

        /// <inheritdoc />
        public void SearchOperationDidFinishWithResults(Operation operation, List<UPCRMResult> results)
        {
        }

        /// <inheritdoc />
        public void SearchOperationDidFinishWithCount(Operation operation, int count)
        {
        }

        /// <inheritdoc />
        public void SearchOperationDidFinishWithCounts(Operation operation, List<int?> counts)
        {
        }

        private static UPMContactTime CreateContactTime(string fromDateString, string toDateString, int weekDay, string timeType)
        {
            var today = DateTime.Now;
            var hour = fromDateString.HourFromCrmTime();
            var minute = fromDateString.MinuteFromCrmTime();
            var fromDate = new DateTime(today.Year, today.Month, today.Day, hour, minute, 0);

            hour = toDateString.HourFromCrmTime();
            minute = toDateString.MinuteFromCrmTime();
            var toDate = new DateTime(today.Year, today.Month, today.Day, hour, minute, 0);
            var fromTime = new UPMDateTimeEditField(StringIdentifier.IdentifierWithStringId("fromTime"))
            {
                Type = DateTimeType.Time,
                DateValue = fromDate
            };
            var toTime = new UPMDateTimeEditField(StringIdentifier.IdentifierWithStringId("toTime"))
            {
                Type = DateTimeType.Time,
                DateValue = toDate
            };

            return new UPMContactTime(weekDay, timeType, fromTime, toTime);
        }

        private UPMGroup ApplyLinkRecordIdentification(string recordIdentification)
        {
            this.crmQuery = this.BuildCrmQuery();
            this.controllerState = GroupModelControllerState.Pending;
            if (this.crmQuery == null)
            {
                this.controllerState = GroupModelControllerState.Error;
                return null;
            }

            this.linkRecordIdentification = recordIdentification;
            if (this.linkRecordIdentification?.Length > 0)
            {
                this.crmQuery.SetLinkRecordIdentification(this.linkRecordIdentification, this.linkId);
            }

            if (this.maxResults > 0)
            {
                this.crmQuery.MaxResults = this.maxResults;
            }

            if (this.RequestOption == UPRequestOption.Offline || this.RequestOption == UPRequestOption.FastestAvailable)
            {
                UPCRMResult result = this.crmQuery.Find();
                if (result?.RowCount > 0)
                {
                    this.controllerState = GroupModelControllerState.Finished;
                    return this.GroupFromResult(result);
                }
            }

            if (this.RequestOption != UPRequestOption.Offline)
            {
                this.controllerState = GroupModelControllerState.Pending;
                var remoteOperation = this.crmQuery.Find(this);
                if (remoteOperation == null)
                {
                    this.controllerState = GroupModelControllerState.Error;
                    return null;
                }

                return null;
            }
            else
            {
                this.controllerState = GroupModelControllerState.Finished;
                return this.EmptyGroup();
            }
        }

        private UPContainerMetaInfo BuildCrmQuery()
        {
            if (this.fieldControl != null)
            {
                this.crmQuery = new UPContainerMetaInfo(this.fieldControl);
                if (this.searchAndList?.FilterName?.Length > 0)
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

        private UPMContactTimesGroup EmptyGroup()
        {
            this.contextResult = null;
            this.contactTimesGroup = new UPMContactTimesGroup(StringIdentifier.IdentifierWithStringId("ContactTimesGroup"));
            this.contactTimesGroup.LabelText = this.TabConfig.Label;
            var functionNames = this.fieldControl.FunctionNames();
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
                var field = this.fieldControl.FieldWithFunction("TYPE");
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

        private UPMContactTimesGroup GroupFromResult(UPCRMResult result)
        {
            var functionNames = this.fieldControl.FunctionNames();
            this.contextResult = result;
            if (!functionNames.Keys.Contains("TYPE"))
            {
                this.FillGroupVrianteWithoutType(result);
            }
            else
            {
                this.FillGroupVariantWithType(result);
            }

            return this.contactTimesGroup;
        }

        private UPMContactTimesGroup FillGroupVrianteWithoutType(UPCRMResult result)
        {
            var functionNames = this.fieldControl.FunctionNames();
            this.contactTimesGroup = this.EmptyGroup();
            for (int row = 0; row < result.RowCount; row++)
            {
                var resultRow = result.ResultRowAtIndex(row) as UPCRMResultRow;
                if (functionNames.Keys.Contains("OPENFROM") && functionNames.Keys.Contains("OPENTO"))
                {
                    this.FillTimesVariantWithoutType(resultRow, "0", "OPEN");
                }

                if (functionNames.Keys.Contains("VISITFROM") && functionNames.Keys.Contains("VISITTO"))
                {
                    this.FillTimesVariantWithoutType(resultRow, "1", "VISIT");
                }

                if (functionNames.Keys.Contains("PHONEFROM") && functionNames.Keys.Contains("PHONETO"))
                {
                    this.FillTimesVariantWithoutType(resultRow, "2", "PHONE");
                }
            }

            return this.contactTimesGroup;
        }

        private void FillTimesVariantWithoutType(UPCRMResultRow resultRow, string timeType, string prefix)
        {
            var valuesWithFunctions = resultRow.ValuesWithFunctions();
            UPMContactTime contactTime;
            string from = $"{prefix}FROM";
            string to = $"{prefix}TO";
            string afternoonFrom = $"{prefix}AFTERNOONFROM";
            string afternoonTo = $"{prefix}AFTERNOONTO";
            int weekDay = ((string)valuesWithFunctions.ValueOrDefault("DAYOFWEEK")).ToInt();
            if (weekDay < 7)
            {
                weekDay++;
            }
            else
            {
                weekDay = 1;
            }

            if (valuesWithFunctions.ValueOrDefault(from) != null && valuesWithFunctions.ValueOrDefault(to) != null)
            {
                contactTime = CreateContactTime((string)valuesWithFunctions.ValueOrDefault(from), (string)valuesWithFunctions.ValueOrDefault(to), weekDay, timeType);
                this.contactTimesGroup.AddChild(contactTime);
            }

            if (valuesWithFunctions.ValueOrDefault(afternoonFrom) != null && valuesWithFunctions.ValueOrDefault(afternoonTo) != null)
            {
                contactTime = CreateContactTime((string)valuesWithFunctions.ValueOrDefault(afternoonFrom), (string)valuesWithFunctions.ValueOrDefault(afternoonTo), weekDay, timeType);
                this.contactTimesGroup.AddChild(contactTime);
            }
        }

        private UPMContactTimesGroup FillGroupVariantWithType(UPCRMResult result)
        {
            this.contactTimesGroup = this.EmptyGroup();
            var functionNames = this.fieldControl.FunctionNames();
            for (int row = 0; row < result.RowCount; row++)
            {
                UPCRMResultRow resultRow = result.ResultRowAtIndex(row) as UPCRMResultRow;
                var valuesWithFunctions = resultRow.ValuesWithFunctions();
                this.FillTimesVariantWithType(resultRow, valuesWithFunctions.ValueOrDefault("TYPE") as string);
            }

            return this.contactTimesGroup;
        }

        private void FillTimesVariantWithType(UPCRMResultRow resultRow, string timeType)
        {
            if (this.contactTimesGroup.TimeTypeTitleForTimeType(timeType)?.Length == 0)
            {
                return;
            }

            var valuesWithFunctions = resultRow.ValuesWithFunctions();
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

            if (valuesWithFunctions.ValueOrDefault(from) != null && valuesWithFunctions.ValueOrDefault(to) != null)
            {
                contactTime = CreateContactTime((string)valuesWithFunctions.ValueOrDefault(from), (string)valuesWithFunctions.ValueOrDefault(to), weekDay, timeType);
                this.contactTimesGroup.AddChild(contactTime);
            }

            if (valuesWithFunctions.ValueOrDefault(afternoonFrom) != null && valuesWithFunctions.ValueOrDefault(afternoonTo) != null)
            {
                contactTime = CreateContactTime((string)valuesWithFunctions.ValueOrDefault(afternoonFrom), (string)valuesWithFunctions.ValueOrDefault(afternoonTo), weekDay, timeType);
                this.contactTimesGroup.AddChild(contactTime);
            }
        }
    }
}
