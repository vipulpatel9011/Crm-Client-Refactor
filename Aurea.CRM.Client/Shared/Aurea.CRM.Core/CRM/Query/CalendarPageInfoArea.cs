// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CalendarPageInfoArea.cs" company="Aurea Software Gmbh">
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
//   The up calendar page info area.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;

    /// <summary>
    /// The up calendar page info area.
    /// </summary>
    public class UPCalendarPageInfoArea
    {
        private const int ButtonNameStartingIndex = 7;
        private const int FilterKeyCount = 5;
        private const int MenuNameStartingIndex = 5;
        private const string Action = "Action";
        private const string ActionNone = "none";
        private const string ActionPrefixButton = "Button:";
        private const string ActionPrefixMenu = "Menu:";
        private const string AttributeCalendarEditAction = "CalendarEditAction";
        private const string AttributeCalendarShowAction = "CalendarShowAction";
        private const string ControlList = "List";
        private const string Edit = "Edit";
        private const string HeaderExpand = "Expand";
        private const string KeyConfigName = "ConfigName";
        private const string KeyAdditionalFilter = "AdditionalFilter";
        private const string KeyFilter = "Filter";
        private const string KeyFilterName = "FilterName";
        private const string KeyInfoArea = "InfoArea";
        private const string KeyInfoAreaId = "InfoAreaId";
        private const string KeyLinkRecord = "LinkRecord";
        private const string MenuEditRecord = "EDITRECORD";
        private const string SwitchToEdit = "switchToEdit";
        private const string ViewOrganizerAction = "OrganizerAction";

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCalendarPageInfoArea"/> class.
        /// </summary>
        /// <param name="viewReference">
        /// The view reference.
        /// </param>
        /// <param name="mainInfoArea">
        /// The _main info area.
        /// </param>
        public UPCalendarPageInfoArea(ViewReference viewReference, UPCalendarPageInfoArea mainInfoArea)
        {
            ViewReference = viewReference ?? throw new ArgumentNullException(nameof(viewReference));
            var configStore = ConfigurationUnitStore.DefaultStore;
            var configName = ViewReference.ContextValueForKey(KeyConfigName);
            if (!string.IsNullOrWhiteSpace(configName))
            {
                SearchAndList = configStore.SearchAndListByName(configName);
            }

            if (SearchAndList != null)
            {
                InfoAreaId = SearchAndList.InfoAreaId;
            }
            else
            {
                InfoAreaId = ViewReference.ContextValueForKey(KeyInfoArea)
                                ?? ViewReference.ContextValueForKey(KeyInfoAreaId);
                SearchAndList = configStore.SearchAndListByName(InfoAreaId);
            }

            configName = ViewReference.ContextValueForKey(KeyFilter);

            if (!string.IsNullOrWhiteSpace(configName))
            {
                Filter = configStore.FilterByName(configName);
            }

            if (Filter == null)
            {
                configName = ViewReference.ContextValueForKey(KeyFilterName);
                if (!string.IsNullOrWhiteSpace(configName))
                {
                    Filter = configStore.FilterByName(configName);
                }
            }

            configName = ViewReference.ContextValueForKey(KeyLinkRecord);
            IgnoreLink = string.IsNullOrWhiteSpace(configName);

            var filterArray = GetFilterArray(configStore);

            if (filterArray.Any())
            {
                FilterArray = filterArray;
            }

            var listFieldControl = configStore.FieldControlByNameFromGroup(
                ControlList,
                SearchAndList.FieldGroupName);

            EditRecordViewReference = GetEditRecordViewReference(listFieldControl, configStore);

            SetShowRecordViewReference(listFieldControl,configStore);
        }

        /// <summary>
        /// Gets the view reference.
        /// </summary>
        /// <value>
        /// The view reference.
        /// </value>
        public ViewReference EditRecordViewReference { get; }

        /// <summary>
        /// Gets the filter.
        /// </summary>
        public UPConfigFilter Filter { get; }

        /// <summary>
        /// Gets the filter array.
        /// </summary>
        public IList<UPConfigFilter> FilterArray { get; }

        /// <summary>
        /// Gets a value indicating whether ignore link.
        /// </summary>
        public bool IgnoreLink { get; }

        /// <summary>
        /// Gets the info area id.
        /// </summary>
        public string InfoAreaId { get; }

        /// <summary>
        /// Gets the search and list.
        /// </summary>
        public SearchAndList SearchAndList { get; }

        /// <summary>
        /// Gets or sets the show record view reference.
        /// </summary>
        /// <value>
        /// The show record view reference.
        /// </value>
        public ViewReference ShowRecordViewReference { get; set; }

        /// <summary>
        /// Gets the view reference.
        /// </summary>
        /// <value>
        /// The view reference.
        /// </value>
        public ViewReference ViewReference { get; }

        /// <summary>
        /// Returns collection of filter
        /// </summary>
        /// <param name="configStore">
        /// <see cref="IConfigurationUnitStore"/>
        /// </param>
        /// <returns>
        /// List of <see cref="UPConfigFilter"/>
        /// </returns>
        private List<UPConfigFilter> GetFilterArray(IConfigurationUnitStore configStore)
        {
            var filterArray = new List<UPConfigFilter>();
            for (var number = 1; number <= FilterKeyCount; number++)
            {
                var filterName = ViewReference.ContextValueForKey($"Filter{number}");

                if (!string.IsNullOrWhiteSpace(filterName))
                {
                    var filter = configStore.FilterByName(filterName);
                    filterArray.Add(filter);
                }
                else
                {
                    filterArray.Add(null);
                }
            }

            var addFilters = ViewReference.ContextValueForKey(KeyAdditionalFilter);
            if (!string.IsNullOrWhiteSpace(addFilters))
            {
                var filterParts = addFilters.Split(';');
                foreach (var filterPart in filterParts)
                {
                    var filter = !string.IsNullOrWhiteSpace(filterPart) ? configStore.FilterByName(filterPart) : null;
                    filterArray.Add(filter);
                }
            }

            return filterArray;
        }

        /// <summary>
        /// Computes EditRecordViewReference from <see cref="FieldControl"/>
        /// </summary>
        /// <param name="listFieldControl">
        /// <see cref="FieldControl"/> object
        /// </param>
        /// <param name="configStore">
        /// <see cref="IConfigurationUnitStore"/>
        /// </param>
        /// <returns>
        /// EditRecordViewReference value to be set
        /// </returns>
        private ViewReference GetEditRecordViewReference(FieldControl listFieldControl, IConfigurationUnitStore configStore)
        {
            var viewReference = (ViewReference)null;
            var editAction = listFieldControl.ValueForAttribute(AttributeCalendarEditAction);

            if (editAction == ActionNone)
            {
                viewReference = null;
            }
            else if (!string.IsNullOrWhiteSpace(editAction))
            {
                if (editAction.StartsWith(ActionPrefixButton))
                {
                    viewReference = configStore.ButtonByName(editAction.Substring(ButtonNameStartingIndex)).ViewReference;
                }
                else if (editAction.StartsWith(ActionPrefixMenu))
                {
                    viewReference = configStore.MenuByName(editAction.Substring(MenuNameStartingIndex)).ViewReference;
                }
                else
                {
                    viewReference = configStore.ButtonByName(editAction).ViewReference;
                }
            }
            else
            {
                var expand = configStore.ExpandByName(InfoAreaId);
                var header = configStore.HeaderByNameFromGroup(HeaderExpand, expand.HeaderGroupName);

                if (header?.ButtonNames != null)
                {
                    foreach (var button in header.ButtonNames)
                    {
                        if (button.StartsWith(Edit))
                        {
                            viewReference = configStore.ButtonByName(button).ViewReference;
                            var isSwitchToEdit = false;
                            if (viewReference.ParameterDictionary().ContainsKey(Action))
                            {
                                isSwitchToEdit = ((string)viewReference.ParameterDictionary()[Action]).StartsWith(SwitchToEdit);
                            }

                            if (viewReference.ViewName == ViewOrganizerAction && isSwitchToEdit)
                            {
                                viewReference = configStore.MenuByName(MenuEditRecord).ViewReference;
                            }

                            if (viewReference != null)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return viewReference;
        }

        /// <summary>
        /// Sets ShowRecordViewReferece
        /// </summary>
        /// <param name="listFieldControl">
        /// <see cref="FieldControl"/>
        /// </param>
        /// <param name="configStore">
        /// <see cref="IConfigurationUnitStore"/>
        /// </param>
        private void SetShowRecordViewReference(FieldControl listFieldControl, IConfigurationUnitStore configStore)
        {
            var showAction = listFieldControl.ValueForAttribute(AttributeCalendarShowAction);
            if (showAction == ActionNone)
            {
                ShowRecordViewReference = null;
            }
            else if (!string.IsNullOrWhiteSpace(showAction))
            {
                if (showAction.StartsWith(ActionPrefixButton))
                {
                    ShowRecordViewReference = configStore.ButtonByName(showAction.Substring(7)).ViewReference;
                }
                else if (showAction.StartsWith(ActionPrefixMenu))
                {
                    ShowRecordViewReference = configStore.MenuByName(showAction.Substring(5)).ViewReference;
                }
                else
                {
                    ShowRecordViewReference = configStore.ButtonByName(showAction).ViewReference;
                }
            }
        }
    }
}
