// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InsightBoardItemGroupModelController.cs" company="Aurea Software Gmbh">
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
//   The Insightboard Item Group Model Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Group
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// The Insightboard Item Group Model Controller
    /// </summary>
    /// <seealso cref="UPGroupModelController" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.ISearchOperationHandler" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.UPCRMLinkReaderDelegate" />
    public class UPInsightBoardItemGroupModelController : UPGroupModelController, ISearchOperationHandler, UPCRMLinkReaderDelegate
    {
        private UPRequestOption requestOption;
        private bool isAlternativeContextMenu;
        private bool countable;
        private bool forcedRecordListView;
        private string searchAndListConfigurationName;
        private string infoAreaid;
        private string linkRecordIdentification;
        private int linkId;
        private string filterName;
        private Dictionary<string, object> parameters;
        private bool detailRecordSwipeEnabled;
        private ViewReference detailActionViewReference;
        private string detailActionRecordIdentifier;
        private ViewReference relevantViewReference;
        private UPContainerMetaInfo crmQuery;
        private string recordIdentification;
        private UPCRMLinkReader linkReader;

        /// <summary>
        /// Gets or sets the menu.
        /// </summary>
        /// <value>
        /// The menu.
        /// </value>
        public Menu Menu { get; set; }

        /// <summary>
        /// Gets or sets the index of the sort.
        /// </summary>
        /// <value>
        /// The index of the sort.
        /// </value>
        public int SortIndex { get; set; }

        /// <summary>
        /// Gets or sets the insight board item view reference.
        /// </summary>
        /// <value>
        /// The insight board item view reference.
        /// </value>
        public ViewReference InsightBoardItemViewReference { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPInsightBoardItemGroupModelController"/> class.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <param name="theDelegate">The delegate.</param>
        /// <param name="sortIndex">Index of the sort.</param>
        /// <param name="insightBoardItemViewReference">The insight board item view reference.</param>
        /// <param name="recordIdentification">The record identification.</param>
        public UPInsightBoardItemGroupModelController(Menu menu, IGroupModelControllerDelegate theDelegate, int sortIndex,
            ViewReference insightBoardItemViewReference, string recordIdentification)
            : base(theDelegate)
        {
            this.Menu = menu;
            this.SortIndex = sortIndex;
            this.InsightBoardItemViewReference = insightBoardItemViewReference;
            this.recordIdentification = recordIdentification;
            bool showImage = false;
            UPMInsightBoardItemType insightBoardType;

            if (menu?.ViewReference?.ViewName == "InsightBoardItem")
            {
                string contextMenuName = this.Menu.ViewReference.ContextValueForKey("ContextMenu");
                this.isAlternativeContextMenu = false;
                if (!string.IsNullOrEmpty(contextMenuName))
                {
                    IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
                    Menu configMenu = configStore.MenuByName(contextMenuName);
                    if (configMenu != null)
                    {
                        this.relevantViewReference = configMenu.ViewReference;
                        this.countable = this.relevantViewReference.ViewName == "RecordListView";
                        if (this.relevantViewReference.ContextValueForKey("Modus") == "GeoSearch")
                        {
                            this.countable = false;
                        }

                        if (menu.ViewReference.ContextValueIsSet("ForceActionStyle"))
                        {
                            this.forcedRecordListView = this.countable;
                            this.countable = false;
                        }
                        else
                        {
                            this.forcedRecordListView = false;
                        }

                        this.isAlternativeContextMenu = true;
                        if (this.countable)
                        {
                            this.searchAndListConfigurationName = this.relevantViewReference.ContextValueForKey("ConfigName");
                            this.infoAreaid = this.relevantViewReference.ContextValueForKey("InfoArea");
                            if (string.IsNullOrEmpty(this.searchAndListConfigurationName))
                            {
                                this.searchAndListConfigurationName = this.infoAreaid;
                            }
                        }
                    }
                }

                if (!this.isAlternativeContextMenu)
                {
                    this.relevantViewReference = this.Menu.ViewReference;
                    this.countable = true;
                    if (menu.ViewReference.ContextValueIsSet("ForceActionStyle"))
                    {
                        this.forcedRecordListView = true;
                        this.countable = false;
                    }

                    this.searchAndListConfigurationName = this.relevantViewReference.ContextValueForKey("ConfigName");
                    if (this.forcedRecordListView)
                    {
                        IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
                        SearchAndList searchAndList = configStore.SearchAndListByName(this.searchAndListConfigurationName);
                        this.infoAreaid = searchAndList.InfoAreaId;
                    }
                }

                string requestOptionString = this.Menu.ViewReference.ContextValueForKey("RequestOption");
                this.relevantViewReference = new ViewReference(this.relevantViewReference, requestOptionString, "InitialRequestOption", null);
                this.requestOption = UPCRMDataStore.RequestOptionFromString(requestOptionString, UPRequestOption.Offline);
                insightBoardType = this.countable ? UPMInsightBoardItemType.Count : UPMInsightBoardItemType.Action;
            }
            else
            {
                this.relevantViewReference = this.Menu.ViewReference;
                showImage = true;
                insightBoardType = UPMInsightBoardItemType.ExMenu;
            }

            UPMInsightBoardGroup insightBoardGroup = new UPMInsightBoardGroup(StringIdentifier.IdentifierWithStringId("temp"));
            insightBoardGroup.LabelText = "InsightBoard";
            this.Group = insightBoardGroup;
            this.ControllerState = GroupModelControllerState.Pending;
            IIdentifier identifier;
            if (!string.IsNullOrEmpty(this.infoAreaid))
            {
                identifier = new RecordIdentifier(this.infoAreaid, null);
            }
            else
            {
                identifier = StringIdentifier.IdentifierWithStringId("InsightBoardItem");
            }

            UPMInsightBoardItem item = new UPMInsightBoardItem(identifier)
            {
                ShowImage = showImage,
                Type = insightBoardType
            };

            this.Group.AddChild(item);
        }

        /// <summary>
        /// Applies the record identification.
        /// </summary>
        /// <param name="_recordIdentification">The record identification.</param>
        public void ApplyRecordIdentification(string _recordIdentification)
        {
            this.ControllerState = GroupModelControllerState.Pending;
            this.recordIdentification = _recordIdentification;
            if (!string.IsNullOrEmpty(this.recordIdentification))
            {
                this.relevantViewReference = this.relevantViewReference.ViewReferenceWith(this.recordIdentification);
            }

            string _linkRecordIdentification;
            if (this.isAlternativeContextMenu && this.countable)
            {
                _linkRecordIdentification = this.relevantViewReference.ContextValueForKey("LinkRecord");
                if (!string.IsNullOrEmpty(_linkRecordIdentification) && _linkRecordIdentification.IsRecordIdentification())
                {
                    _linkRecordIdentification = UPCRMDataStore.DefaultStore.ReplaceRecordIdentification(_linkRecordIdentification);
                    object linkIdObject = this.relevantViewReference.ContextValueForKey("LinkId");
                    if (linkIdObject != null)
                    {
                        this.linkId = Convert.ToInt32(linkIdObject);
                    }
                    else
                    {
                        this.linkId = -1;
                    }
                }
                else
                {
                    _linkRecordIdentification = null;
                    this.linkId = -1;
                }

                this.filterName = this.relevantViewReference.ContextValueForKey("FilterName");
            }
            else
            {
                _linkRecordIdentification = this.recordIdentification;
                this.linkId = 0;
                this.filterName = null;
            }

            string parentLink = this.relevantViewReference?.ContextValueForKey("ParentLink");
            if (!string.IsNullOrEmpty(_linkRecordIdentification) && !string.IsNullOrEmpty(parentLink))
            {
                this.linkReader = new UPCRMLinkReader(_linkRecordIdentification, parentLink, this.requestOption, this);
                this.linkReader.Start();
            }
            else
            {
                this.ContinueWithRecordIdentification(_linkRecordIdentification);
            }
        }

        /// <summary>
        /// Continues the with record identification.
        /// </summary>
        /// <param name="_linkRecordIdentification">The link record identification.</param>
        private void ContinueWithRecordIdentification(string _linkRecordIdentification)
        {
            this.linkRecordIdentification = _linkRecordIdentification;
            if (this.countable)
            {
                IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
                this.crmQuery = !string.IsNullOrEmpty(this.searchAndListConfigurationName)
                    ? new UPContainerMetaInfo(this.searchAndListConfigurationName, this.parameters)
                    : new UPContainerMetaInfo(configStore.FieldControlByNameFromGroup("List", this.infoAreaid));

                if (!string.IsNullOrEmpty(this.linkRecordIdentification))
                {
                    this.crmQuery.SetLinkRecordIdentification(this.linkRecordIdentification);
                }

                if (!string.IsNullOrEmpty(this.filterName))
                {
                    if (this.crmQuery != null)
                    {
                        UPConfigFilter filter = configStore.FilterByName(this.filterName);
                        if (filter != null)
                        {
                            if (this.parameters?.Count > 0)
                            {
                                filter = filter.FilterByApplyingValueDictionaryDefaults(this.parameters, true);
                            }

                            this.crmQuery.ApplyFilter(filter);
                        }
                    }
                }

                Operation operation = this.crmQuery.CountTheDelegate(this.requestOption, this);
                if (operation == null)
                {
                    this.SearchOperationDidFinishWithCount(null, 0);
                }
            }
            else
            {
                UPMInsightBoardItem item = (UPMInsightBoardItem)this.Group.Children[0];
                item.Countable = false;
                item.Count = 1;
                item.Title = !string.IsNullOrEmpty(this.Menu.DisplayName) ? this.Menu.DisplayName : string.Empty;
                item.ImageName = this.Menu.ImageName;
                item.SortIndex = this.SortIndex;

                this.detailActionViewReference = this.relevantViewReference;
                this.detailActionRecordIdentifier = this.linkRecordIdentification;

                UPMAction action = new UPMAction(null);
                if (this.forcedRecordListView)
                {
                    action.SetTargetAction(this, this.SwitchToListOrganizer);
                }
                else
                {
                    action.SetTargetAction(this, this.SwitchToDetailOrganizer);
                }

                item.Action = action;
                this.ControllerState = GroupModelControllerState.Finished;
                this.Delegate.GroupModelControllerFinished(this);
            }
        }

        /// <summary>
        /// Switches to detail organizer.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void SwitchToDetailOrganizer(object sender)
        {
            if (this.detailActionViewReference != null)
            {
                this.Delegate.PerformOrganizerAction(this,
                    this.detailActionViewReference.ViewReferenceWith(this.detailActionRecordIdentifier));
            }
        }

        /// <summary>
        /// Switches to list organizer.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void SwitchToListOrganizer(object sender)
        {
            ViewReference recordListView = (this.countable && this.isAlternativeContextMenu) || (this.forcedRecordListView && this.isAlternativeContextMenu)
                ? this.relevantViewReference
                : this.ConfigRecordListView(this.searchAndListConfigurationName, this.infoAreaid, this.linkRecordIdentification, 0, null, false);
            recordListView = recordListView.ViewReferenceWith(this.parameters);
            this.Delegate.PerformOrganizerAction(this, recordListView);
        }

        /// <summary>
        /// Configurations the record ListView.
        /// </summary>
        /// <param name="configName">Name of the configuration.</param>
        /// <param name="infoAreaId">The information area identifier.</param>
        /// <param name="_linkRecordIdentification">The link record identification.</param>
        /// <param name="_linkId">The link identifier.</param>
        /// <param name="_filterName">Name of the filter.</param>
        /// <param name="_detailRecordSwipeEnabled">if set to <c>true</c> [detail record swipe enabled].</param>
        /// <returns></returns>
        private ViewReference ConfigRecordListView(string configName, string infoAreaId, string _linkRecordIdentification, int _linkId, string _filterName, bool _detailRecordSwipeEnabled)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>
            {
                ["ConfigName"] = configName,
                ["InfoArea"] = infoAreaId
            };

            if (!string.IsNullOrEmpty(_linkRecordIdentification))
            {
                dictionary["LinkRecord"] = _linkRecordIdentification;
            }

            if (_linkId > 0)
            {
                dictionary["LinkId"] = _linkId.ToString();
            }

            if (!string.IsNullOrEmpty(_filterName))
            {
                dictionary["FilterName"] = _filterName;
            }

            if (_detailRecordSwipeEnabled)
            {
                dictionary[Search.Constants.SwipeDetailRecordsConfigName] = "true";
            }

            return new ViewReference(dictionary, "RecordListView");
        }

        /// <summary>
        /// Defaults the action for search and list.
        /// </summary>
        /// <param name="searchAndList">The search and list.</param>
        /// <returns></returns>
        public Menu DefaultActionForSearchAndList(SearchAndList searchAndList)
        {
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            Menu tmpDetailAction = null;
            string infoAreaId = searchAndList.InfoAreaId;
            if (!string.IsNullOrEmpty(searchAndList.DefaultAction))
            {
                tmpDetailAction = configStore.MenuByName(searchAndList.DefaultAction);
            }

            if (tmpDetailAction == null)
            {
                InfoArea infoAreaConfig = configStore.InfoAreaConfigById(infoAreaId);
                if (!string.IsNullOrEmpty(infoAreaConfig.DefaultAction))
                {
                    tmpDetailAction = configStore.MenuByName(infoAreaConfig.DefaultAction);
                }
            }

            return tmpDetailAction ?? configStore.MenuByName("SHOWRECORD");
        }

        /// <summary>
        /// Applies the context.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns></returns>
        public override UPMGroup ApplyContext(Dictionary<string, object> dictionary)
        {
            base.ApplyContext(dictionary);
            ViewReference tmpInsightBoardItemViewReference = new ViewReference(this.InsightBoardItemViewReference, dictionary, true);
            this.parameters = tmpInsightBoardItemViewReference.ParamsDictionary();
            this.ApplyRecordIdentification(this.recordIdentification);
            return this.Group;
        }

        /// <summary>
        /// Links the reader did finish with result.
        /// </summary>
        /// <param name="_linkReader">The link reader.</param>
        /// <param name="result">The result.</param>
        public void LinkReaderDidFinishWithResult(UPCRMLinkReader _linkReader, object result)
        {
            this.linkReader = null;
            this.ContinueWithRecordIdentification(_linkReader.DestinationRecordIdentification);
        }

        /// <summary>
        /// Links the reader did finish with error.
        /// </summary>
        /// <param name="_linkReader">The link reader.</param>
        /// <param name="error">The error.</param>
        public void LinkReaderDidFinishWithError(UPCRMLinkReader _linkReader, Exception error)
        {
            this.ControllerState = GroupModelControllerState.Error;
            this.Delegate.GroupModelControllerFinished(this);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"[{base.ToString()}, {this.relevantViewReference}]";
        }

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.ControllerState = GroupModelControllerState.Error;
            this.Delegate.GroupModelControllerFinished(this);
        }

        /// <summary>
        /// Searches the operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="result">The result.</param>
        public void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            SearchAndList searchAndList = configStore.SearchAndListByName(this.searchAndListConfigurationName);
            this.detailActionViewReference = this.DefaultActionForSearchAndList(searchAndList).ViewReference;
            if (result.RowCount > 0)
            {
                this.detailActionRecordIdentifier = result.ResultRowAtIndex(0).RootRecordIdentification;
            }

            this.Delegate.GroupModelControllerFinished(this);
        }

        /// <summary>
        /// Searches the operation did finish with results.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="results">The results.</param>
        public void SearchOperationDidFinishWithResults(Operation operation, List<UPCRMResult> results)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches the operation did finish with count.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="count">The count.</param>
        public void SearchOperationDidFinishWithCount(Operation operation, int count)
        {
            this.ControllerState = GroupModelControllerState.Finished;
            UPMInsightBoardItem item = (UPMInsightBoardItem)this.Group.Children[0];
            item.Countable = true;
            item.Count = count;
            item.SortIndex = this.SortIndex;
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            SearchAndList searchAndList = configStore.SearchAndListByName(this.searchAndListConfigurationName);
            this.infoAreaid = searchAndList.InfoAreaId;
            InfoArea infoAreaConfig = configStore.InfoAreaConfigById(this.infoAreaid);
            if (!string.IsNullOrEmpty(infoAreaConfig.ColorKey))
            {
                item.Color = AureaColor.ColorWithString(infoAreaConfig.ColorKey);
            }

            if (!string.IsNullOrEmpty(this.Menu.DisplayName))
            {
                item.Title = this.Menu.DisplayName;
            }
            else
            {
                item.Title = item.Count > 1 ? infoAreaConfig.PluralName : infoAreaConfig.SingularName;
            }

            item.ImageName = this.Menu.ImageName ?? infoAreaConfig.ImageName;
            UPMAction action = new UPMAction(null);
            if (count == 1)
            {
                this.crmQuery.Find(this.requestOption, this);
                action.SetTargetAction(this, this.SwitchToDetailOrganizer);
            }
            else
            {
                action.SetTargetAction(this, this.SwitchToListOrganizer);
            }

            item.Action = action;
            if (count != 1)
            {
                this.Delegate.GroupModelControllerFinished(this);
            }
        }

        /// <summary>
        /// Searches the operation did finish with counts.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="counts">The counts.</param>
        public void SearchOperationDidFinishWithCounts(Operation operation, List<int?> counts)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }
    }
}
