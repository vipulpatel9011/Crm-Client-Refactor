// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchPageModelController.cs" company="Aurea Software Gmbh">
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
//   Search Page Model Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.Core.Session;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.Services.ModelControllers.Organizer;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Contexts;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;
    using Aurea.CRM.UIModel.Pages;
    using Aurea.CRM.UIModel.Status;
    using Core.Messages;
    using GalaSoft.MvvmLight.Messaging;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// Constants
    /// </summary>
    public partial class Constants
    {
        /// <summary>
        /// The swipe detail records configuration name
        /// </summary>
        public const string SwipeDetailRecordsConfigName = @"SwipeDetailRecords";

        /// <summary>
        /// Default online search delay period in seconds
        /// </summary>
        public const double OnlineDefaultDelaySeconds = 0.5;

        /// <summary>
        /// Identifier string Field
        /// </summary>
        public static readonly string KeyFieldIdentifier = "Field";

        /// <summary>
        /// CrmResultRow Method name
        /// </summary>
        public static readonly string MethodCrmResultRow = "CrmResultRow";

        /// <summary>
        /// GPS Key
        /// </summary>
        public static readonly string KeyGPS = "GPS";

        /// <summary>
        /// Keyword X
        /// </summary>
        public static readonly string KeyX = "X";

        /// <summary>
        /// Keyword Y
        /// </summary>
        public static readonly string KeyY = "Y";
    }

    /// <summary>
    /// Search Page Model COntroller
    /// </summary>
    /// <seealso cref="UPPageModelController" />
    /// <seealso cref="Aurea.CRM.Services.Delegates.ISwipePageDataSourceController" />
    /// <seealso cref="ISearchViewControllerDataProvider" />
    /// <seealso cref="IFavoriteModelControllerDelegate" />
    public class SearchPageModelController : UPPageModelController,
         ISwipePageDataSourceController,
         ISearchViewControllerDataProvider,
         IFavoriteModelControllerDelegate
    {
        /// <summary>
        /// The filter name
        /// </summary>
        protected string filterName;

        /// <summary>
        /// The section contexts
        /// </summary>
        protected Dictionary<string, UPCoreMappingResultContext> SectionContexts;

        /// <summary>
        /// The current search operation
        /// </summary>
        protected Operation CurrentSearchOperation;

        /// <summary>
        /// The table caption result field map
        /// </summary>
        protected Dictionary<string, List<UPContainerFieldMetaInfo>> TableCaptionResultFieldMap;

        /// <summary>
        /// The current detail row
        /// </summary>
        protected UPMResultRow CurrentDetailRow;

        /// <summary>
        /// The current section index
        /// </summary>
        protected int CurrentSectionIndex;

        /// <summary>
        /// The swipe detail records
        /// </summary>
        protected bool SwipeDetailRecords;

        /// <summary>
        /// The filter object
        /// </summary>
        protected UPConfigFilter filterObject;

        /// <summary>
        /// The offline wait time
        /// </summary>
        protected double OfflineWaitTime;

        /// <summary>
        /// The online wait time
        /// </summary>
        protected double OnlineWaitTime;

        /// <summary>
        /// The automatic switch to offline
        /// </summary>
        protected bool AutoSwitchToOffline;

        /// <summary>
        /// The maximum results
        /// </summary>
        protected int MaxResults;

        /// <summary>
        /// The expanded row context
        /// </summary>
        protected UPExpandedRowContext ExpandedRowContext;

        /// <summary>
        /// The classic layout
        /// </summary>
        protected bool ClassicLayout;

        /// <summary>
        /// The loader
        /// </summary>
        protected UPCalendarPopoverLoader Loader;

        /// <summary>
        /// The favorite model controller
        /// </summary>
        protected UPFavoriteModelController FavoriteModelController;

        /// <summary>
        /// Gets or sets online mode override flag
        /// </summary>
        public bool ForceOnlineMode { get; set; }

        /// <summary>
        /// Gets the search page.
        /// </summary>
        public virtual UPMSearchPage SearchPage => (UPMSearchPage)this.Page;

        /// <summary>
        /// Gets the result contexts.
        /// </summary>
        /// <value>
        /// The result contexts.
        /// </value>
        public virtual List<UPCoreMappingResultContext> ResultContexts => null;

        /// <summary>
        /// Gets the search page mode.
        /// </summary>
        /// <value>
        /// The search page mode.
        /// </value>
        public SearchPageMode SearchPageMode { get; private set; }

        /// <summary>
        /// Gets the filter object.
        /// </summary>
        /// <value>
        /// The filter object.
        /// </value>
        public UPConfigFilter FilterObject => this.filterObject;

        /// <summary>
        /// Gets the initial request option.
        /// </summary>
        /// <value>
        /// The initial request option.
        /// </value>
        public UPRequestOption InitialRequestOption { get; }

        /// <summary>
        /// Gets the name of the filter.
        /// </summary>
        /// <value>
        /// The name of the filter.
        /// </value>
        public string FilterName => this.filterName;

        /// <summary>
        /// Gets the test delegate.
        /// </summary>
        /// <value>
        /// The test delegate.
        /// </value>
        public UPSearchPageModelControllerTestDelegate TestDelegate { get; private set; }

        /// <summary>
        /// Gets the details action.
        /// </summary>
        /// <value>
        /// The details action.
        /// </value>
        public virtual ViewReference DetailsAction => ConfigurationUnitStore.DefaultStore.MenuByName(@"SHOWRECORD").ViewReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchPageModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public SearchPageModelController(ViewReference viewReference)
            : base(viewReference)
        {
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;

            string searchPageModeString = viewReference.ContextValueForKey(@"SearchPageMode");

            if (!string.IsNullOrEmpty(searchPageModeString))
            {
                searchPageModeString = configStore.ConfigValue(@"Search.Mode");
            }

            if (string.IsNullOrEmpty(searchPageModeString))
            {
                searchPageModeString = null;
            }

            this.SearchPageMode = (SearchPageMode)Convert.ToInt32(searchPageModeString);

            if (!configStore.ConfigValueIsSet(@"Search.NoColorOnDefault"))
            {
                this.SearchPageMode |= SearchPageMode.ShowColorOnDefault;
            }

            if (!configStore.ConfigValueIsSet(@"Search.NoColorOnVirtualInfoAreaIds"))
            {
                this.SearchPageMode |= SearchPageMode.ShowColorOnVirtualInfoArea;
            }

            this.InitialRequestOption =
                UPCRMDataStore.RequestOptionFromString(
                    viewReference.ContextValueForKey(@"InitialRequestOption"), UPRequestOption.Offline);

            var val = configStore.ConfigValue(@"Search.OnlineDelayTime");
            this.OnlineWaitTime = !string.IsNullOrEmpty(val) ? Convert.ToDouble(val, System.Globalization.CultureInfo.InvariantCulture) : Constants.OnlineDefaultDelaySeconds;

            val = configStore.ConfigValue(@"Search.OfflineDelayTime");
            this.OfflineWaitTime = !string.IsNullOrEmpty(val) ? Convert.ToDouble(val, System.Globalization.CultureInfo.InvariantCulture) : 0.3;

            val = viewReference.ContextValueForKey(@"MaxResults");

            if (!string.IsNullOrEmpty(val))
            {
                this.MaxResults = Convert.ToInt32(val);
            }

            if (this.MaxResults == 0)
            {
                val = configStore.ConfigValue(@"Search.MaxResults");
                this.MaxResults = !string.IsNullOrEmpty(val) ? Convert.ToInt32(val) : -1;
            }

            this.AutoSwitchToOffline = configStore.ConfigValueIsSet(@"Search.AutoSwitchToOffline");

            this.BuildPage();
            this.SectionContexts = new Dictionary<string, UPCoreMappingResultContext>();

            if (this.SearchPage?.SearchType == 0)
            {
                this.SearchPage.SearchType = SearchPageSearchType.OfflineSearch;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchPageModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="testDelegate">The test delegate.</param>
        public SearchPageModelController(ViewReference viewReference, UPSearchPageModelControllerTestDelegate testDelegate)
            : this(viewReference)
        {
            this.TestDelegate = testDelegate;
        }

        /// <summary>
        /// Creates the page instance.
        /// </summary>
        /// <returns></returns>
        public virtual UPMSearchPage CreatePageInstance()
        {
            return this.CreatePageInstance(new RecordIdentifier(this.InfoAreaId, null));
        }

        /// <summary>
        /// Creates the page instance.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <returns></returns>
        public virtual UPMSearchPage CreatePageInstance(IIdentifier identifier)
        {
            UPMSearchPage searchPage = new UPMSearchPage(identifier)
            {
                AutoSwitchToOffline = this.AutoSwitchToOffline,
                OnlineWaitTime = this.OnlineWaitTime,
                OfflineWaitTime = this.OfflineWaitTime
            };

            return searchPage;
        }

        /// <summary>
        /// Resets the with reason.
        /// </summary>
        /// <param name="reason">The reason.</param>
        public override void ResetWithReason(ModelControllerResetReason reason)
        {
            base.ResetWithReason(reason);

            // In case of a memory warning remove all sections
            // but only if the pagemodellcontroller has false connection to a view
            if (this.ModelControllerDelegate == null && reason != ModelControllerResetReason.SearchCacheInvalidated)
            {
                this.SearchPage.RemoveAllResultSections();
                this.ApplyLoadingStatusOnPage(this.Page);
                this.SectionContexts.Clear();
                this.Page.Invalid = true;
            }
        }

        /// <summary>
        /// Builds the page.
        /// </summary>
        public virtual void BuildPage()
        {
            this.InfoAreaId = this.ViewReference.ContextValueForKey(@"InfoArea");
            this.ConfigName = this.ViewReference.ContextValueForKey(@"ConfigName");

            string swipeDetailRecordsString = this.ViewReference.ContextValueForKey(Constants.SwipeDetailRecordsConfigName);

            if (!string.IsNullOrEmpty(swipeDetailRecordsString))
            {
                this.SwipeDetailRecords = swipeDetailRecordsString == @"true";
            }
            else
            {
                this.SwipeDetailRecords = ConfigurationUnitStore.DefaultStore.ConfigValueIsSetDefaultValue(@"View.RecordSwipeEnabledDefault", true);
            }

            if (string.IsNullOrEmpty(this.InfoAreaId))
            {
                SearchAndList searchAndList = ConfigurationUnitStore.DefaultStore.SearchAndListByName(this.ConfigName);
                this.InfoAreaId = searchAndList?.InfoAreaId;
            }

            UPMSearchPage page = this.CreatePageInstance();
            page.Invalid = true;
            this.TopLevelElement = page;

            this.BuildPageDetails();
            UPMAction searchAction = new UPMAction(null);
            searchAction.SetTargetAction(this, this.Search);
            this.SearchPage.SearchAction = searchAction;
        }

        /// <summary>
        /// Builds the page details.
        /// </summary>
        public virtual void BuildPageDetails()
        {
            List<SearchPageViewType> types = new List<SearchPageViewType> { SearchPageViewType.List };

            if (!string.IsNullOrEmpty(this.ViewReference.ContextValueForKey(@"MapView")))
            {
                types.Add(SearchPageViewType.Map);
            }

            this.SearchPage.AvailableViewTypes = types;
            bool isGeoSearch = this.ViewReference.ContextValueForKey(@"Modus") == @"GeoSearch";

            this.ClassicLayout = false;
            string cellStyleAsString = this.ViewReference.ContextValueForKey(@"ListStyle");

            if (cellStyleAsString == @"rowOnly" || isGeoSearch)
            {
                this.ClassicLayout = false;
                this.SearchPage.CellStyle = TableCellStyle.RowOnly;
            }
            else if (cellStyleAsString == @"row")
            {
                this.ClassicLayout = false;
                this.SearchPage.CellStyle = TableCellStyle.Row;
            }
            else if (cellStyleAsString == @"card")
            {
                this.ClassicLayout = false;
                this.SearchPage.CellStyle = TableCellStyle.Card;
            }
            else if (cellStyleAsString == @"card23")
            {
                this.ClassicLayout = false;
                this.SearchPage.CellStyle = TableCellStyle.Card23;
            }
            else if (cellStyleAsString == @"card2Only")
            {
                this.ClassicLayout = false;
                this.SearchPage.CellStyle = TableCellStyle.Card2Only;
            }
            // else if (cellStyleAsString == @"classic" || (!string.IsNullOrEmpty(this.ConfigName) ? this.ConfigName.upClassicLayoutEnabled : this.InfoAreaId.upClassicLayoutEnabled))
            else if (cellStyleAsString == @"classic") // TODO
            {
                this.ClassicLayout = true;
                this.SearchPage.CellStyle = TableCellStyle.Classic;
            }
            else
            {
                this.ClassicLayout = false;
                this.SearchPage.CellStyle = TableCellStyle.Row;
            }
        }

        /// <summary>
        /// The update groups for result row.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        public void UpdateGroupsForResultRow(UPMResultRow resultRow)
        {
            foreach (UPMGroup group in resultRow.DetailGroups)
            {
                UPMGroup newGroup = (UPMGroup)this.UpdatedElement(group);
                group.ApplyUpdatesFromGroup(newGroup);
            }
        }

        /// <summary>
        /// The update result row.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        public void UpdateResultRow(UPMResultRow resultRow)
        {
            UPMResultRow newRow = (UPMResultRow)this.UpdatedElement(resultRow);
            resultRow.ApplyUpdatesFromResultRow(newRow);
        }

        /// <summary>
        /// The update expanded result row.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        public void UpdateExpandedResultRow(UPMResultRow resultRow)
        {
            this.ExpandedRowContext = resultRow != null
                ? new UPExpandedRowContext((UPMiniDetailsResultRow)resultRow)
                : null;
        }

        /// <summary>
        /// Updateds the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// The <see cref="UPMElement" />.
        /// </returns>
        public override UPMElement UpdatedElement(UPMElement element)
        {
            if (element is Page)
            {
                return this.UpdatedElementForPage((UPMSearchPage)element);
            }

            if (element is UPMCalendarPopoverGroup)
            {
                return this.UpdateElementForCalendarGroup((UPMCalendarPopoverGroup)element);
            }

            if (element is UPMGroup)
            {
                return this.UpdateElementForDropdownGroup((UPMGroup)element);
            }

            if (element is UPMResultRow)
            {
                return this.UpdatedElementForResultRow((UPMResultRow)element);
            }

            return null;
        }

        /// <summary>
        /// Updateds the element for page.
        /// </summary>
        /// <param name="origSearchPage">The original search page.</param>
        /// <returns></returns>
        public virtual Page UpdatedElementForPage(UPMSearchPage origSearchPage)
        {
            if (!origSearchPage.Invalid)
            {
                return origSearchPage;
            }

            this.Search(origSearchPage);
            return origSearchPage;
        }

        /// <summary>
        /// Updateds the element for result row.
        /// </summary>
        /// <param name="origResultRow">The original result row.</param>
        /// <returns><see cref="UPMResultRow"/> resulting row.</returns>
        public virtual UPMResultRow UpdatedElementForResultRow(UPMResultRow origResultRow)
        {
            var resultRow = CopyResultRow(origResultRow);
            var resultRowKey = origResultRow.Key;
            var resultContext = ResultContextForRow(origResultRow);
            var rowContext = resultContext.RowDictionary.ValueOrDefault(resultRowKey);
            var row = (UPCRMResultRow)null;

            if (rowContext == null)
            {
                row = origResultRow.FindAndInvokeMethod(Constants.MethodCrmResultRow, null) as UPCRMResultRow;
                if (row != null)
                {
                    rowContext = new UPCoreMappingResultRowContext(row, resultContext);
                    resultContext.RowDictionary.Add(resultRowKey, rowContext);
                }
            }
            else
            {
                resultContext.RowDictionary[resultRowKey] = rowContext;
                row = rowContext.Row;
            }

            if (!origResultRow.DataValid && row != null)
            {
                var newResult = row.ReloadRow();
                if (newResult != null && newResult.RowCount == 1)
                {
                    row = newResult.ResultRowAtIndex(0) as UPCRMResultRow;
                    rowContext = new UPCoreMappingResultRowContext(newResult, resultContext);
                    resultContext.RowDictionary.Add(resultRowKey, rowContext);
                }

                resultRow.DataValid = true;
            }
            else if (origResultRow.DataValid)
            {
                resultRow.DataValid = true;
            }

            if (row == null)
            {
                return resultRow;
            }

            var recordId = row.RecordIdentificationAtIndex(0);
            var virtualInfoAreaId = (string)null;
            if (SearchPageMode != SearchPageMode.IgnoreVirtual)
            {
                virtualInfoAreaId = row.VirtualInfoAreaIdAtIndex(0);
                var physicalInfoAreaId = row.PhysicalInfoAreaIdAtIndex(0);
                if (virtualInfoAreaId == physicalInfoAreaId)
                {
                    virtualInfoAreaId = null;
                }
            }

            SetUPMResultRowFields(resultRow, row, resultContext);
            SetUPMResultRowColorAndImage(resultContext, row, origResultRow, resultRow, virtualInfoAreaId);

            if (!row.HasLocalCopy)
            {
                // resultRow.statusIndicatorIcon = [UIImage upXXImageNamed: @"crmpad-List-Cloud"];      // CRM-5007
            }

            resultRow.OnlineData = !row.HasLocalCopy;

            // TODO: This guy here has some performance issues, check this again on CRM-5413
            // AddDropdownGroupsForResultRow(resultRow, rowContext, expand);
            resultRow.Invalid = false;
            return resultRow;
        }

        /// <summary>
        /// Updates the element for dropdown group.
        /// </summary>
        /// <param name="origDetailGroup">The original detail group.</param>
        /// <param name="seperate">if set to <c>true</c> [seperate].</param>
        /// <returns></returns>
        public UPMGroup UpdateElementForDropdownGroup(UPMGroup origDetailGroup, bool seperate = false)
        {
            UPMGroup group = new UPMGroup(origDetailGroup.Identifier);
            int detailFieldCount, detailFieldOffset;

            bool leftCol = true;
            UPMResultRow resultRow = (UPMResultRow)origDetailGroup.Parent;

            UPCoreMappingResultContext resultContext;

            // if ([origDetailGroup respondsToSelector: @selector(resultContext)])
            // {
            //    resultContext = [origDetailGroup performSelector: @selector(resultContext)];
            // }
            // else
            {
                resultContext = this.ResultContextForRow(resultRow);
            }

            UPCoreMappingResultRowContext rowContext = resultContext.RowDictionary[resultRow.Key];
            UPCRMResultRow row = rowContext.Row;
            UPCoreMappingResultContext context = rowContext.Context;
            detailFieldCount = context.DropdownFields.Count;
            detailFieldOffset = context.NumberOfListFields;

            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            bool hideEmptyFields = configStore.ConfigValueIsSet(@"ViewHideEmptyFields");

            FieldIdentifier fieldIdentifier = (FieldIdentifier)origDetailGroup.Identifier;
            leftCol = fieldIdentifier.FieldId == @"DetailField_Left";

            bool fieldOnleftCol = true;
            bool hasFieldValue = false;

            for (int i = 0; i < detailFieldCount; i++)
            {
                UPConfigFieldControlField configField = context.DropdownFields[i];
                FieldAttributes attributes = configField.Attributes;

                string fieldValue;
                if (attributes.FieldCount == 1)
                {
                    fieldValue = row.ValueAtIndex(i + detailFieldOffset);
                }
                else
                {
                    List<string> fieldValues = new List<string>(attributes.FieldCount);
                    for (int j = 0; j < attributes.FieldCount; j++)
                    {
                        string v = row.ValueAtIndex(i + detailFieldOffset + j);
                        if (v != null)
                        {
                            fieldValues.Add(v);
                        }
                    }

                    fieldValue = attributes.FormatValues(fieldValues);
                    i += attributes.FieldCount - 1;
                }

                if (hideEmptyFields && !string.IsNullOrEmpty(fieldValue))
                {
                    continue;
                }

                fieldOnleftCol = !fieldOnleftCol;
                if (fieldOnleftCol == leftCol && seperate)
                {
                    continue;
                }

                hasFieldValue = true;

                RecordIdentifier identifier = (RecordIdentifier)resultRow.Identifier;

                UPMField field;
                if (attributes.ExtendedOptionForKey(@"GPS") == @"X")
                {
                    field = new UPMGpsXField(new FieldIdentifier(identifier.RecordIdentification, configField.Field.FieldIdentification));
                }
                else if (attributes.ExtendedOptionForKey(@"GPS") == @"Y")
                {
                    field = new UPMGpsYField(new FieldIdentifier(identifier.RecordIdentification, configField.Field.FieldIdentification));
                }
                else
                {
                    field = new UPMStringField(new FieldIdentifier(
                            identifier.RecordIdentification,
                            configField.Field.FieldIdentification));
                }

                ((UPMStringField)field).StringValue = fieldValue;
                if (!attributes.NoLabel)
                {
                    field.LabelText = configField.Label;
                }

                ((UPMStringField)field).MulitLine = attributes.MultiLine;
                field.SetAttributes(attributes);
                field.Hidden = attributes.Hide;
                this.AddFieldToDropDownGroup(field, group, configField);
            }

            if (leftCol && !hasFieldValue)
            {
                UPMStringField stringField = new UPMStringField(new StringIdentifier($"{fieldIdentifier.RecordId}:emptyDetails"));
                stringField.LabelText = configStore.BasicTextByIndexDefaultText(1, @" false details ");
                group.AddField(stringField);
            }

            group.Invalid = false;
            return group;
        }

        /// <summary>
        /// Applies the loading status on page.
        /// </summary>
        /// <param name="page">The page.</param>
        public void ApplyLoadingStatusOnPage(Page page)
        {
            UPMProgressStatus stillLoadingError = new UPMProgressStatus(StringIdentifier.IdentifierWithStringId(@"loadingIdentifier"));
            UPMStringField statusField = new UPMStringField(StringIdentifier.IdentifierWithStringId(@"statusFieldIdentifier"));
            statusField.FieldValue = LocalizedString.TextLoadingData;
            stillLoadingError.StatusMessageField = statusField;
            page.Status = stillLoadingError;
        }

        /// <summary>
        /// Switches to detail.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public virtual void SwitchToDetail(object sender)
        {
            UPMResultRow resultRow = (UPMResultRow)sender;
            this.CurrentDetailRow = resultRow;

            this.CurrentSectionIndex = this.IndexOfSectionForRow(resultRow);

            if (resultRow.OnlineData)
            {
                bool connectedToServer = ServerSession.CurrentSession.ConnectedToServer;
                if (!connectedToServer)
                {
                    SimpleIoc.Default.GetInstance<IMessenger>().Send(new ToastrMessage
                    {
                        MessageText = LocalizedString.TextErrorTitleNoInternet,
                        DetailedMessage = LocalizedString.TextErrorMessageNoInternet
                    });

                    SimpleIoc.Default.GetInstance<ILogger>().LogError(string.Concat(LocalizedString.TextErrorTitleNoInternet, " - ", LocalizedString.TextErrorMessageNoInternet));

                    return;
                }
            }

            var organizerModelController = this.DetailOrganizerForResultRow(resultRow);
            if (organizerModelController != null)
            {
                organizerModelController.ShouldShowTabsForSingleTab = true;
                if (resultRow.Identifier is RecordIdentifier identifier)
                {
                    this.EnableSwipingForOrganizerInfoAreaId(organizerModelController, identifier);
                }

                this.ModelControllerDelegate.TransitionToContentModelController(organizerModelController);
            }
        }

        /// <summary>
        /// Enables the swiping for organizer information area identifier.
        /// </summary>
        /// <param name="organizerModelController">The organizer model controller.</param>
        /// <param name="identifier">The information identifier.</param>
        public void EnableSwipingForOrganizerInfoAreaId(UPOrganizerModelController organizerModelController, RecordIdentifier identifier)
        {
            if (this.TableCaptionResultFieldMap == null)
            {
                this.TableCaptionResultFieldMap = new Dictionary<string, List<UPContainerFieldMetaInfo>>();
                this.TableCaptionResultFieldMapInDictionary(this.TableCaptionResultFieldMap);
            }

            bool moreThanRecord = this.HasMoreThanOneRecord;
            if (this.TableCaptionResultFieldMap.ContainsKey(identifier.InfoAreaId) && this.SwipeDetailRecords && moreThanRecord)
            {
                UPSearchResultCachingSwipeRecordController cachingRecordController = new UPSearchResultCachingSwipeRecordController(this);
                cachingRecordController.BuildCache(identifier);
                organizerModelController.ParentSwipePageRecordController = cachingRecordController;
            }
            else
            {
                if (this.SwipeDetailRecords && moreThanRecord)
                {
                    SimpleIoc.Default.GetInstance<ILogger>().LogWarn($"HeaderSwipe for unit {this.ViewReference.Name} disabled Table Caption could not be determined");
                }
            }
        }

        /// <summary>
        /// Switches to edit.
        /// </summary>
        /// <param name="sender">The sender.</param>
        protected override void SwitchToEdit(object sender)
        {
            UPMResultRow resultRow = (UPMResultRow)sender;
            RecordIdentifier identifier = (RecordIdentifier)resultRow.Identifier;

            UPOrganizerModelController organizerModelController = new UPOrganizerModelController(ConfigurationUnitStore.DefaultStore.MenuByName(@"EDITRECORD").ViewReference.ViewReferenceWith(identifier.RecordIdentification));
            this.modelControllerDelegate.TransitionToContentModelController(organizerModelController);
        }

        /// <summary>
        /// Gets a value indicating whether this instance has more than one record.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has more than one record; otherwise, <c>false</c>.
        /// </value>
        public bool HasMoreThanOneRecord
        {
            get
            {
                if (this.SearchPage.NumberOfResultSections > 1)
                {
                    return true;
                }

                if (this.SearchPage.NumberOfResultSections == 1)
                {
                    return this.SearchPage.ResultSectionAtIndex(0).NumberOfResultRows > 1;
                }

                return false;
            }
        }

        /// <summary>
        /// Tables the caption result field map in dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        public void TableCaptionResultFieldMapInDictionary(Dictionary<string, List<UPContainerFieldMetaInfo>> dictionary)
        {
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            List<string> infoAreaIds = this.SectionContexts.Keys.Select(x => x).ToList();

            foreach (string infoAreaId in infoAreaIds)
            {
                UPConfigExpand expandConfig = configStore.ExpandByName(infoAreaId);

                string tableCaptionName = expandConfig?.TableCaptionName;
                if (string.IsNullOrEmpty(tableCaptionName))
                {
                    tableCaptionName = infoAreaId;
                }

                UPConfigTableCaption tableCaption = configStore.TableCaptionByName(tableCaptionName);
                if (tableCaption != null)
                {
                    UPCoreMappingResultContext resultContext = this.SectionContexts[infoAreaId];
                    List<UPContainerFieldMetaInfo> resultFieldMap = tableCaption.ResultFieldMapFromMetaInfo(resultContext.Result.MetaInfo);

                    if (resultFieldMap != null && resultFieldMap.Any())
                    {
                        dictionary.Add(infoAreaId, resultFieldMap);
                    }
                }
            }
        }

        /// <summary>
        /// Searches the specified sender.
        /// </summary>
        /// <param name="searchPage">The search page.</param>
        public virtual void Search(object searchPage)
        {
            // Do nothing, implement in derived classes
        }

        /// <summary>
        /// Details the organizer for result row.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        /// <returns></returns>
        public virtual UPOrganizerModelController DetailOrganizerForResultRow(UPMResultRow resultRow)
        {
            RecordIdentifier identifier = (RecordIdentifier)resultRow.Identifier;
            return this.DetailOrganizerForRecordIdentification(identifier.RecordIdentification, resultRow.OnlineData);
        }

        /// <summary>
        /// The detail organizer for record identification.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="onlineData">The online data.</param>
        /// <returns>
        /// The <see cref="UPOrganizerModelController" />.
        /// </returns>
        public UPOrganizerModelController DetailOrganizerForRecordIdentification(string recordIdentification, bool onlineData)
        {
            var organizerController = UPOrganizerModelController.OrganizerFromViewReference(this.DetailsAction.ViewReferenceWith(recordIdentification));

            if (onlineData)
            {
                organizerController.OnlineData = onlineData;
            }

            return organizerController;
        }

        /// <summary>
        /// The find quick actions for row check details.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        /// <param name="checkDetails">The check details.</param>
        /// <returns>
        /// The <see cref="List" />.
        /// </returns>
        public virtual List<UPMAction> FindQuickActionsForRowCheckDetails(UPMResultRow resultRow, bool checkDetails)
        {
            var actions = resultRow.DetailActions;

            if (actions != null)
            {
                foreach (UPMAction action in actions)
                {
                    action.Invalid = false;

                    if (checkDetails && action.Identifier.MatchesIdentifier(StringIdentifier.IdentifierWithStringId("action.ToggleFavorite")))
                    {
                        this.FavoriteModelController = new UPFavoriteModelController();
                        this.FavoriteModelController.TheDelegate = this;
                        this.FavoriteModelController.IsFavorite(((RecordIdentifier)resultRow.Identifier).RecordIdentification);
                    }

                    if (!action.Identifier.MatchesIdentifier(StringIdentifier.IdentifierWithStringId(@"startNavigation")))
                    {
                        action.SetTargetAction(this, this.PerformAction);
                    }
                }
            }

            return actions;
        }

        /// <summary>
        /// Toggles the favorite.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        protected override void ToggleFavorite(ViewReference viewReference)
        {
            List<UPMAction> actions = this.ExpandedRowContext?.ResultRow.DetailActionsForMiniDetails;
            if (actions != null)
            {
                foreach (UPMAction action in actions)
                {
                    if (action.Identifier.MatchesIdentifier(StringIdentifier.IdentifierWithStringId(Core.Constants.ActionIdToggleFavorite)))
                    {
                        if (!string.IsNullOrEmpty(this.ExpandedRowContext.FavoriteRecordIdentification))
                        {
                            this.FavoriteModelController.ChangeFavoriteValue(this.ExpandedRowContext.FavoriteRecordIdentification, false);
                        }
                        else
                        {
                            this.FavoriteModelController.ChangeFavoriteValue(((RecordIdentifier)this.ExpandedRowContext.ResultRow.IdentifierForMiniDetails).RecordIdentification, true);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Favorites the model controller did change favorite.
        /// </summary>
        /// <param name="_favoriteModelController">The favorite model controller.</param>
        /// <param name="favoriteRecordIdentification">The favorite record identification.</param>
        public void FavoriteModelControllerDidChangeFavorite(UPFavoriteModelController _favoriteModelController, string favoriteRecordIdentification)
        {
            if (this.ExpandedRowContext == null || _favoriteModelController != this.FavoriteModelController)
            {
                return;
            }

            this.ExpandedRowContext.FavoriteRecordIdentification = favoriteRecordIdentification;
            List<UPMAction> actions = this.ExpandedRowContext.ResultRow.DetailActionsForMiniDetails;
            if (actions != null)
            {
                foreach (UPMOrganizerAction action in actions)
                {
                    if (action.Identifier.MatchesIdentifier(StringIdentifier.IdentifierWithStringId(Core.Constants.ActionIdToggleFavorite)))
                    {
                        action.Aktive = !string.IsNullOrEmpty(this.ExpandedRowContext.FavoriteRecordIdentification);
                        action.LabelText = action.Aktive == false ? LocalizedString.TextProcessAddToFavorites : LocalizedString.TextProcessDeleteFromFavorites;
                    }
                }
            }

            this.InformAboutDidChangeTopLevelElement(this.SearchPage, this.SearchPage,
                new List<IIdentifier> { this.ExpandedRowContext.ResultRow.IdentifierForMiniDetails },
                UPChangeHints.ChangeHintsWithHint("FavoritestateChanged"));
        }

        /// <summary>
        /// Favorites the model controller favorite record identification.
        /// </summary>
        /// <param name="favoriteModelController">The favorite model controller.</param>
        /// <param name="favoriteRecordIdentification">The favorite record identification.</param>
        public void FavoriteModelControllerFavoriteRecordIdentification(UPFavoriteModelController favoriteModelController, string favoriteRecordIdentification)
        {
            if (this.ExpandedRowContext == null || favoriteModelController != this.FavoriteModelController)
            {
                return;
            }

            this.ExpandedRowContext.FavoriteRecordIdentification = favoriteRecordIdentification;
            List<UPMAction> actions = this.ExpandedRowContext.ResultRow.DetailActionsForMiniDetails;
            if (actions != null)
            {
                foreach (UPMOrganizerAction action in actions)
                {
                    if (action.Identifier.MatchesIdentifier(StringIdentifier.IdentifierWithStringId(Core.Constants.ActionIdToggleFavorite)))
                    {
                        action.Aktive = !string.IsNullOrEmpty(this.ExpandedRowContext.FavoriteRecordIdentification);
                        action.LabelText = action.Aktive == false ? LocalizedString.TextProcessAddToFavorites : LocalizedString.TextProcessDeleteFromFavorites;
                    }
                }
            }

            this.InformAboutDidChangeTopLevelElement(
                this.SearchPage,
                this.SearchPage,
                new List<IIdentifier> { this.ExpandedRowContext.ResultRow.IdentifierForMiniDetails },
                UPChangeHints.ChangeHintsWithHint("FavoritestateChanged"));
        }

        /// <summary>
        /// The favorite model controller did fail with error.
        /// </summary>
        /// <param name="favoriteModelController">The favorite model controller.</param>
        /// <param name="error">The error.</param>
        public void FavoriteModelControllerDidFailWithError(UPFavoriteModelController favoriteModelController, Exception error)
        {
            this.ReportError(error, true);
        }

        /// <summary>
        /// Performs the action.
        /// </summary>
        /// <param name="actionParam">The action parameter.</param>
        public void PerformAction(object actionParam)
        {
            Dictionary<string, object> actionDictionary = (Dictionary<string, object>)actionParam;
            UPMOrganizerAction action = (UPMOrganizerAction)actionDictionary["UPMOrganizerAction"];
            Dictionary<string, object> addParams = new Dictionary<string, object>();

            ViewReference viewReference = action != null
                ? action.ViewReference
                : (ViewReference)actionDictionary[@"viewReference"];

            if (viewReference != null)
            {
                if (viewReference.ViewName == @"RecordListView" && viewReference.ContextValueForKey(@"Modus") == @"GeoSearch")
                {
                    string additionalParamString = viewReference.ContextValueForKey(@"AdditionalParameters");
                    if (!string.IsNullOrEmpty(additionalParamString))
                    {
                        UPMResultRow resultRow = this.ExpandedRowContext.ResultRow as UPMResultRow;
                        if (resultRow != null && this.HasGPSFieldValues(resultRow))
                        {
                            string resultRowKey = resultRow.Key;
                            UPCoreMappingResultContext resultContext = this.ResultContextForRow(resultRow);
                            UPCoreMappingResultRowContext rowContext = resultContext.RowDictionary[resultRowKey];
                            UPCRMResultRow row = rowContext.Row;

                            int fieldCount = resultContext.FieldControl.NumberOfFields;
                            for (int i = 0; i < fieldCount; i++)
                            {
                                UPConfigFieldControlField configField = resultContext.FieldControl.FieldAtIndex(i);
                                FieldAttributes attributes = configField.Attributes;
                                string fieldValue;

                                switch (attributes.ExtendedOptionForKey(@"GPS"))
                                {
                                    case "X":
                                        fieldValue = row.RawValueAtIndex(i);
                                        addParams.Add(@"$GpsX$", fieldValue);
                                        break;

                                    case "Y":
                                        fieldValue = row.RawValueAtIndex(i);
                                        addParams.Add(@"$GpsY$", fieldValue);
                                        break;

                                    case "City":
                                        fieldValue = row.RawValueAtIndex(i);
                                        addParams.Add(@"$GpsCity$", fieldValue);
                                        break;

                                    case "Street":
                                        fieldValue = row.RawValueAtIndex(i);
                                        addParams.Add(@"$GpsStreet$", fieldValue);
                                        break;

                                    case "Country":
                                        fieldValue = row.RawValueAtIndex(i);
                                        addParams.Add(@"$GpsCountry$", fieldValue);
                                        break;
                                }
                            }
                        }
                    }

                    base.PerformAction(actionDictionary, addParams);
                }
            }
        }

        /// <summary>
        /// Results the context for row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public virtual UPCoreMappingResultContext ResultContextForRow(UPMResultRow row)
        {
            return this.SectionContexts[this.InfoAreaId];
        }

        /// <summary>
        /// CRMs the row for row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public virtual UPCRMResultRow CrmRowForRow(UPMResultRow row)
        {
            string resultRowKey = row.Key;
            UPCoreMappingResultContext resultContext = this.ResultContextForRow(row);
            UPCoreMappingResultRowContext rowContext = resultContext.RowDictionary[resultRowKey];
            return rowContext.Row;
        }

        /// <summary>
        /// Copies the result row.
        /// </summary>
        /// <param name="originalResultRow">The original result row.</param>
        /// <returns></returns>
        public UPMResultRow CopyResultRow(UPMResultRow originalResultRow)
        {
            UPMResultRow copiedResultRow = new UPMResultRow(originalResultRow.Identifier)
            {
                Icon = originalResultRow.Icon,
                StatusIndicatorIcon = originalResultRow.StatusIndicatorIcon,
                OnlineData = originalResultRow.OnlineData,
                RowColor = originalResultRow.RowColor,
                RecordImageDocument = originalResultRow.RecordImageDocument,
                Parent = originalResultRow.Parent
            };

            return copiedResultRow;
        }

        /// <summary>
        /// The has gps field values.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        public bool HasGPSFieldValues(UPMResultRow resultRow)
        {
            string resultRowKey = resultRow.Key;
            UPCoreMappingResultContext resultContext = this.ResultContextForRow(resultRow);
            UPCoreMappingResultRowContext rowContext = resultContext.RowDictionary[resultRowKey];
            UPCRMResultRow row = rowContext.Row;

            int fieldCount = resultContext.FieldControl.NumberOfFields;
            bool hasGpsX = false;
            bool hasGpsY = false;

            for (int i = 0; i < fieldCount; i++)
            {
                UPConfigFieldControlField configField = resultContext.FieldControl.FieldAtIndex(i);
                FieldAttributes attributes = configField.Attributes;
                string fieldValue;
                if (attributes.ExtendedOptionForKey(@"GPS") == @"X")
                {
                    fieldValue = row.RawValueAtIndex(i);

                    if (Convert.ToInt32(fieldValue) != 0)
                    {
                        hasGpsX = true;
                    }
                }
                else if (attributes.ExtendedOptionForKey(@"GPS") == @"Y")
                {
                    fieldValue = row.RawValueAtIndex(i);
                    if (Convert.ToInt32(fieldValue) != 0)
                    {
                        hasGpsY = true;
                    }
                }
            }

            return hasGpsX && hasGpsY;
        }

        /// <summary>
        /// Adds the dropdown groups for result row.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        /// <param name="rowContext">The row context.</param>
        /// <param name="expand">The expand.</param>
        public virtual void AddDropdownGroupsForResultRow(UPMResultRow resultRow, UPCoreMappingResultRowContext rowContext, UPConfigExpand expand)
        {
            UPCoreMappingResultContext resultContext = rowContext.Context;
            if (resultContext.DropdownFields?.Count > 0)
            {
                int detailFieldCount = resultContext.DropdownFields.Count;

                UPCRMResultRow row = rowContext.Row;
                string recordId = row.RecordIdentificationAtIndex(0);
                if (detailFieldCount > 0)
                {
                    UPMGroup detailGroupCol1 = new UPMGroup(new FieldIdentifier(this.InfoAreaId, recordId, @"DetailField_Left"));
                    detailGroupCol1.Invalid = true;
                    resultRow.AddDetailGroup(detailGroupCol1);
                }

                this.AddRowActions(expand, resultRow, recordId);
            }
        }

        /// <summary>
        /// Adds the row actions.
        /// </summary>
        /// <param name="expand">The expand.</param>
        /// <param name="resultRow">The result row.</param>
        /// <param name="recordId">The record identifier.</param>
        public virtual void AddRowActions(UPConfigExpand expand, UPMResultRow resultRow, string recordId)
        {
            // RowActions
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            UPConfigHeader actionHeader = ConfigurationUnitStore.DefaultStore.HeaderByNameFromGroup(@"ExpandOptions", expand.HeaderGroupName);

            if (actionHeader != null)
            {
                UPOrganizerModelController targetController = this.DetailOrganizerForResultRow(resultRow);
                foreach (string buttonName in actionHeader.ButtonNames)
                {
                    UPMOrganizerAction action = null;
                    UPConfigButton buttonDef = configStore.ButtonByName(buttonName);

                    if (buttonDef.IsHidden)
                    {
                        continue;
                    }

                    string iconName = string.Empty;
                    if (!string.IsNullOrEmpty(buttonDef.ImageName))
                    {
                        iconName = configStore.FileNameForResourceName(buttonDef.ImageName);
                    }

                    if (buttonName.StartsWith(@"GroupStart") || buttonName.StartsWith(@"GroupEnd"))
                    {
                        continue;
                    }

                    if (buttonDef.ViewReference != null)
                    {
                        action = new UPMOrganizerAction(StringIdentifier.IdentifierWithStringId($"action.{buttonName}"));
                        action.SetTargetAction(targetController, this.PerformAction);

                        ViewReference viewReference = buttonDef.ViewReference.ViewReferenceWith(recordId);
                        viewReference = viewReference.ViewReferenceWith(new Dictionary<string, string> { { @".fromPopup", "1" } });
                        action.ViewReference = viewReference;
                        if (!string.IsNullOrEmpty(iconName))
                        {
                            action.IconName = iconName;
                        }

                        if (action.Identifier.MatchesIdentifier(StringIdentifier.IdentifierWithStringId(Core.Constants.ActionIdToggleFavorite)))
                        {
                            action.IconName = @"Icon:StarEmpty";
                            action.ActiveIconName = @"Icon:Star";
                            action.LabelText = LocalizedString.TextProcessAddToFavorites;
                        }
                        else
                        {
                            action.LabelText = buttonDef.Label;
                        }
                    }

                    if (action != null)
                    {
                        action.Invalid = true;
                        resultRow.AddDetailAction(action);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the element for calendar group.
        /// </summary>
        /// <param name="origDetailGroup">The original detail group.</param>
        /// <returns></returns>
        public virtual UPMCalendarPopoverGroup UpdateElementForCalendarGroup(UPMCalendarPopoverGroup origDetailGroup)
        {
            if (origDetailGroup.Invalid)
            {
                string sourceCopyFieldGroupName = this.ViewReference.ContextValueForKey("CalendarPopOverConfig");
                IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
                FieldControl sourceFieldControl = configStore.FieldControlByNameFromGroup("Details", sourceCopyFieldGroupName);
                if (sourceFieldControl == null)
                {
                    SearchAndList searchAndList = configStore.SearchAndListByName(this.ConfigName);
                    if (searchAndList != null)
                    {
                        sourceFieldControl = configStore.FieldControlByNameFromGroup("Details", searchAndList.FieldGroupName) ??
                                             configStore.FieldControlByNameFromGroup("List", searchAndList.FieldGroupName);
                    }
                }

                if (sourceFieldControl == null)
                {
                    sourceFieldControl = configStore.FieldControlByNameFromGroup("Details", this.ConfigName);
                }

                if (sourceFieldControl == null)
                {
                    sourceFieldControl = configStore.FieldControlByNameFromGroup("List", this.ConfigName);
                }

                this.Loader = new UPCalendarPopoverLoader(this);
                this.Loader.LoadElementForCalendarGroupFieldControl(origDetailGroup, sourceFieldControl);
            }

            return origDetailGroup;
        }

        // Overriden in Calendar
        /// <summary>
        /// Adds the field to drop down group.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="group">The group.</param>
        /// <param name="configField">The configuration field.</param>
        public virtual void AddFieldToDropDownGroup(UPMField field, UPMGroup group, UPConfigFieldControlField configField)
        {
            if (!field.Hidden)
            {
                group.AddField(field);
            }
        }

        /// <summary>
        /// Reports the error continue operation.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="continueOperation">if set to <c>true</c> [continue operation].</param>
        public override void ReportError(Exception error, bool continueOperation)
        {
            if (error == null)
            {
                this.InformAboutDidUpdateListOfErrors(null);
            }
            else if (continueOperation)
            {
                if (error.IsConnectionOfflineError())
                {
                    this.SearchPage.Invalid = false;
                    this.SearchPage.ResultState = SearchPageResultState.Ok;
                    this.SearchPage.SearchType = SearchPageSearchType.OfflineSearch;
                    this.SearchPage.Status = null;
                    this.InformAboutDidUpdateListOfErrors(new List<Exception> { error });
                    this.InformAboutDidChangeTopLevelElement(this.SearchPage, this.SearchPage, null, null);
                }
                else
                {
                    this.InformAboutDidUpdateListOfErrors(new List<Exception> { error });
                    this.InformAboutDidChangeTopLevelElement(this.Page, this.Page, null, null);
                }
            }
            else
            {
                base.ReportError(error, continueOperation);
            }
        }

        /// <summary>
        /// Loads the index of the table captions from index to.
        /// </summary>
        /// <param name="fromIndex">From index.</param>
        /// <param name="toIndex">To index.</param>
        /// <returns></returns>
        public List<UPSwipePageRecordItem> LoadTableCaptionsFromIndexToIndex(int fromIndex, int toIndex)
        {
            List<UPSwipePageRecordItem> result = new List<UPSwipePageRecordItem>();
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            var crmStore = UPCRMDataStore.DefaultStore;
            for (int index = fromIndex; index < toIndex; index++)
            {
                UPMResultRow row = this.ResultRowAtIndexOffset(index);
                UPCRMResultRow crmRow = row != null ? this.CrmRowForRow(row) : null;
                if (row != null && crmRow != null)
                {
                    RecordIdentifier recordIdentifier = (RecordIdentifier)row.Identifier;
                    UPConfigExpand expandConfig = configStore.ExpandByName(recordIdentifier.InfoAreaId);
                    string tableCaptionName = expandConfig.TableCaptionName;
                    if (string.IsNullOrEmpty(tableCaptionName))
                    {
                        tableCaptionName = recordIdentifier.InfoAreaId;
                    }

                    UPConfigTableCaption tableCaption = configStore.TableCaptionByName(tableCaptionName);
                    if (tableCaption != null && this.TableCaptionResultFieldMap != null)
                    {
                        var tableCaptionResultMap = this.TableCaptionResultFieldMap[recordIdentifier.InfoAreaId];
                        var recordTableCaption = tableCaption.TableCaptionForResultRow(crmRow, tableCaptionResultMap);
                        UPSwipePageRecordItem item = new UPSwipePageRecordItem(recordTableCaption, crmStore.TableInfoForInfoArea(recordIdentifier.InfoAreaId).Label, recordIdentifier.RecordIdentification, row.OnlineData);
                        result.Add(item);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Results the row at index offset.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public virtual UPMResultRow ResultRowAtIndexOffset(int index)
        {
            if (this.CurrentDetailRow != null)
            {
                if (this.CurrentSectionIndex < this.SearchPage.Children.Count)
                {
                    UPMResultSection section = this.SearchPage.ResultSectionAtIndex(this.CurrentSectionIndex);
                    UPMResultRow row = this.CurrentDetailRow;

                    if (index == 0)
                    {
                        return row; // current DetailRow
                    }

                    int currentRowIndex = this.IndexOfRowInSection(row, section);
                    int sectionIndex = this.CurrentSectionIndex;

                    currentRowIndex += index;

                    while (currentRowIndex < 0)
                    {
                        sectionIndex--;

                        if (sectionIndex >= 0)
                        {
                            currentRowIndex += this.SearchPage.ResultSectionAtIndex(sectionIndex).NumberOfResultRows;
                        }
                        else
                        {
                            return null;
                        }
                    }

                    while (currentRowIndex >= this.SearchPage.ResultSectionAtIndex(sectionIndex).NumberOfResultRows)
                    {
                        if (sectionIndex + 1 < this.SearchPage.NumberOfResultSections)
                        {
                            currentRowIndex -= this.SearchPage.ResultSectionAtIndex(sectionIndex).NumberOfResultRows;
                            sectionIndex++;
                        }
                        else
                        {
                            return null;
                        }
                    }

                    return this.SearchPage.ResultSectionAtIndex(sectionIndex).ResultRowAtIndex(currentRowIndex);
                }
            }

            return null;
        }

        /// <summary>
        /// Indexes the of section for row.
        /// </summary>
        /// <param name="_row">The row.</param>
        /// <returns></returns>
        public int IndexOfSectionForRow(UPMResultRow _row)
        {
            var sections = this.SearchPage.Children;
            for (int sectionIndex = 0; sectionIndex <= sections.Count - 1; sectionIndex++)
            {
                UPMResultSection section = sections[sectionIndex] as UPMResultSection;

                for (int rowIndex = 0; rowIndex <= section.NumberOfResultRows - 1; rowIndex++)
                {
                    UPMResultRow row = section.ResultRowAtIndex(rowIndex);

                    if (row.Identifier.IdentifierAsString == _row.Identifier.IdentifierAsString)
                    {
                        return sectionIndex;
                    }
                }
            }

            return -1; // NSNotFound;
        }

        /// <summary>
        /// Indexes the of row in section.
        /// </summary>
        /// <param name="_row">The row.</param>
        /// <param name="section">The section.</param>
        /// <returns></returns>
        public int IndexOfRowInSection(UPMResultRow _row, UPMResultSection section)
        {
            for (int rowIndex = 0; rowIndex <= section.NumberOfResultRows - 1; rowIndex++)
            {
                UPMResultRow row = section.ResultRowAtIndex(rowIndex);

                if (row.Identifier.IdentifierAsString == _row.Identifier.IdentifierAsString)
                {
                    return rowIndex;
                }
            }

            return -1; // NSNotFound;
        }

        /// <summary>
        /// Counts the state of the result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public SearchPageResultState CountResultState(UPCRMResult result)
        {
            bool hasOfflineData = UPCRMDataStore.DefaultStore.HasOfflineData(this.InfoAreaId);
            bool connectedToServer = ServerSession.CurrentSession.ConnectedToServer;

            if (result == null && !connectedToServer && !hasOfflineData)
            {
                return SearchPageResultState.OnlyOnline;
            }

            if (result == null)
            {
                return SearchPageResultState.NoSearchCharacter;
            }

            if (result.RowCount > 0)
            {
                return SearchPageResultState.Ok;
            }

            if (result.IsServerResult && result.RowCount == 0)
            {
                return SearchPageResultState.EmptyOnline;
            }

            if (connectedToServer && !hasOfflineData)
            {
                return SearchPageResultState.OnlyOnline; // .OnlineNoLokalData;
            }

            if (!connectedToServer && !hasOfflineData)
            {
                return SearchPageResultState.OfflineNoLokalData;
            }

            return SearchPageResultState.OfflineNoLokalData;
        }

        /// <summary>
        /// Invokes an online or offline search
        /// </summary>
        /// <param name="searchText">Text to be found</param>
        public void PerformSearch(string searchText)
        {
            if (this.SearchPage == null)
            {
                return;
            }

            this.SearchPage.SearchText = searchText;
            this.SearchPage.SearchType = this.GetSearchType();
            this.SearchPage.SearchAction?.PerformAction(this.SearchPage);
        }

        private SearchPageSearchType GetSearchType()
        {
            return this.ForceOnlineMode
                ? SearchPageSearchType.OnlineSearch
                : SearchPageSearchType.OfflineSearch;
        }

        /// <summary>
        /// Sets UPMResultRow result row's Fields.
        /// </summary>
        /// <param name="resultRow">
        /// resulting row
        /// </param>
        /// <param name="row">
        /// <see cref="UPCRMResultRow"/> row
        /// </param>
        /// <param name="resultContext">
        /// <see cref="UPCoreMappingResultContext"/> context
        /// </param>
        private void SetUPMResultRowFields(UPMResultRow resultRow, UPCRMResultRow row, UPCoreMappingResultContext resultContext)
        {
            var fieldCount = ClassicLayout
                ? Math.Max(6, resultContext.ListFormatter.PositionCount)
                : resultContext.ListFormatter.PositionCount;
            var recordId = row.RecordIdentificationAtIndex(0);
            var recordIdentifier = new RecordIdentifier(recordId.InfoAreaId(), recordId.RecordId());
            var listFields = new List<UPMField>(fieldCount);

            for (var i = 0; i < fieldCount; i++)
            {
                var configField = resultContext.ListFormatter.FirstFieldForPosition(i);
                if (configField == null)
                {
                    // Dont show helper fields for normal Layout
                    if (SearchPage.CellStyle == TableCellStyle.Classic)
                    {
                        var stringField = new UPMStringField(recordIdentifier.IdentifierWithFieldId(Constants.KeyFieldIdentifier))
                        {
                            StringValue = string.Empty
                        };
                        listFields.Add(stringField);
                    }

                    continue;
                }

                var field = (UPMField)null;
                var attributes = configField.Attributes;
                var fieldValue = string.Empty;
                if (attributes.ExtendedOptionForKey(Constants.KeyGPS) == Constants.KeyX)
                {
                    field = new UPMGpsXField(recordIdentifier.IdentifierWithFieldId(Constants.KeyFieldIdentifier));
                    fieldValue = row.RawValueForFieldIdInfoAreaIdLinkId(configField.FieldId, configField.InfoAreaId, configField.LinkId);
                }
                else if (attributes.ExtendedOptionForKey(Constants.KeyGPS) == Constants.KeyY)
                {
                    field = new UPMGpsYField(recordIdentifier.IdentifierWithFieldId(Constants.KeyFieldIdentifier));
                    fieldValue = row.RawValueForFieldIdInfoAreaIdLinkId(configField.FieldId, configField.InfoAreaId, configField.LinkId);
                }
                else if (attributes.Image)
                {
                    var documentManager = new DocumentManager();
                    var documentKey = resultContext.ListFormatter.StringFromRowForPosition(row, i);
                    var documentData = documentManager.DocumentForKey(documentKey);

                    resultRow.RecordImageDocument = documentData != null
                        ? new UPMDocument(documentData)
                        : new UPMDocument(recordIdentifier, ServerSession.CurrentSession.DocumentRequestUrlForDocumentKey(documentKey));
                    continue;
                }
                else
                {
                    field = new UPMStringField(recordIdentifier.IdentifierWithFieldId(Constants.KeyFieldIdentifier));
                    fieldValue = resultContext.ListFormatter.StringFromRowForPosition(row, i);
                }

                field.Hidden = attributes.Hide;
                field.LabelText = configField.Label;

                if (field is UPMStringField)
                {
                    if (!string.IsNullOrWhiteSpace(fieldValue) && attributes.NoLabel)
                    {
                        fieldValue = $"{configField.Label} {fieldValue}";
                    }

                    ((UPMStringField)field).StringValue = fieldValue;
                }

                field.SetAttributes(attributes);
                listFields.Add(field);
            }

            resultRow.Fields = listFields;
        }

        /// <summary>
        /// Sets resulting UPMResultRow's color and image
        /// </summary>
        /// <param name="resultContext">
        /// Result Context <see cref="UPCoreMappingResultContext"/>
        /// </param>
        /// <param name="row">
        /// <see cref="UPCRMResultRow"/>
        /// </param>
        /// <param name="origResultRow">
        /// <see cref="UPMResultRow"/> original result row.
        /// </param>
        /// <param name="resultRow">
        /// <see cref="UPMResultRow"/> resulting row.
        /// </param>
        /// <param name="virtualInfoAreaId">
        /// string virtual Info AreaId
        /// </param>
        private void SetUPMResultRowColorAndImage(
            UPCoreMappingResultContext resultContext,
            UPCRMResultRow row,
            UPMResultRow origResultRow,
            UPMResultRow resultRow,
            string virtualInfoAreaId)
        {
            var imageName = (string)null;
            var colorString = (string)null;
            var expand = (UPConfigExpand)null;
            if (!string.IsNullOrWhiteSpace(virtualInfoAreaId))
            {
                expand = ConfigurationUnitStore.DefaultStore.ExpandByName(virtualInfoAreaId);
                if (expand != null && SearchPageMode.HasFlag(SearchPageMode.ShowColorOnVirtualInfoArea))
                {
                    colorString = expand.ColorKey;
                }
            }

            if (expand == null)
            {
                if (resultContext.ExpandMapper != null)
                {
                    expand = resultContext.ExpandMapper.ExpandForResultRow(row);
                    if (expand.UnitName == InfoAreaId)
                    {
                        if (SearchPageMode.HasFlag(SearchPageMode.ShowColorOnDefault))
                        {
                            colorString = expand.ColorKey;
                        }
                    }
                    else if (!SearchPageMode.HasFlag(SearchPageMode.IgnoreColors))
                    {
                        colorString = expand.ColorKey;
                    }
                }
                else if (string.IsNullOrWhiteSpace(colorString) && !SearchPageMode.HasFlag(SearchPageMode.ShowColorOnDefault))
                {
                    colorString = resultContext.InfoAreaConfig?.ColorKey;
                }
            }

            if (expand != null)
            {
                imageName = expand.ImageName;
            }

            if (imageName == null)
            {
                imageName = resultContext.InfoAreaConfig?.ImageName;
            }

            if (origResultRow.Icon != null)
            {
                resultRow.Icon = origResultRow.Icon;
            }
            else if (!string.IsNullOrWhiteSpace(imageName))
            {
                // resultRow.icon = [UIImage upImageWithFileName: imageName];   // CRM-5007
            }

            if (origResultRow.RowColor != null)
            {
                resultRow.RowColor = origResultRow.RowColor;
            }
            else if (!string.IsNullOrWhiteSpace(colorString))
            {
                resultRow.RowColor = AureaColor.ColorWithString(colorString);
            }
        }
    }
}
