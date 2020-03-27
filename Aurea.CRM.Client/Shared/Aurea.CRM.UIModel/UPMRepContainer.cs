// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMRepContainer.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The upm rep container.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.UIModel.Filters;

    /// <summary>
    /// The upm rep container.
    /// </summary>
    public class UPMRepContainer : IUPMRepContainer
    {
        /// <summary>
        /// The _last used rep keys.
        /// </summary>
        private List<string> lastUsedRepKeys;

        /// <summary>
        /// The _possible groups.
        /// </summary>
        private Dictionary<string, UPMRepPossibleValue> possibleGroups;

        /// <summary>
        /// The _possible values.
        /// </summary>
        private Dictionary<string, UPMRepPossibleValue> possibleValues;

        /// <summary>
        /// The _recently used rep keys.
        /// </summary>
        private List<string> recentlyUsedRepKeys;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMRepContainer"/> class.
        /// </summary>
        public UPMRepContainer()
        {
            this.SelectedRepKeys = new List<string>();
        }

        /// <summary>
        /// Returns list of keys from PossibleGroupValues.
        /// </summary>
        public List<string> AllKeysFromPossibleGroupValues => new List<string>(this.possibleGroups.Keys);

        /// <summary>
        /// Gets or sets the all keys from possible values.
        /// </summary>
        public List<string> AllKeysFromPossibleValues => new List<string>(this.possibleValues.Keys);

        /// <summary>
        /// Gets or sets the all possible groups.
        /// </summary>
        public List<string> AllPossibleGroups { get; set; }

        /// <summary>
        /// Gets or sets the all possible values.
        /// </summary>
        public ObservableCollection<UPMRepPossibleValue> AllPossibleValues => new ObservableCollection<UPMRepPossibleValue>(this.possibleValues.Values);

        /// <summary>
        /// Gets or sets the delegate.
        /// </summary>
        public UPMRepContainerDelegate Delegate { get; set; }

        /// <summary>
        /// Gets or sets the exclude rep keys.
        /// </summary>
        public List<string> ExcludeRepKeys { get; set; }

        /// <summary>
        /// Gets or sets the explicit key order.
        /// </summary>
        public List<string> ExplicitKeyOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether multi select mode.
        /// </summary>
        public bool MultiSelectMode { get; set; }

        /// <summary>
        /// Gets or sets the null value key.
        /// </summary>
        public string NullValueKey { get; set; }

        /// <summary>
        /// Gets or sets the rep edit field filters.
        /// </summary>
        public List<UPMRepEditFieldFilter> RepEditFieldFilters { get; set; }

        /// <summary>
        /// Gets or sets the selected rep keys.
        /// </summary>
        public List<string> SelectedRepKeys { get; set; }

        /// <summary>
        /// The add possible rep group value.
        /// </summary>
        /// <param name="possibleValue">
        /// The possible value.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        public void AddPossibleRepGroupValue(UPMRepPossibleValue possibleValue, string key)
        {
            if (this.possibleGroups == null)
            {
                this.possibleGroups = new Dictionary<string, UPMRepPossibleValue>();
            }

            this.possibleGroups[key] = possibleValue;
        }

        /// <summary>
        /// The add possible value.
        /// </summary>
        /// <param name="possibleValue">
        /// The possible value.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        public void AddPossibleValue(UPMRepPossibleValue possibleValue, string key)
        {
            if (this.possibleValues == null)
            {
                this.possibleValues = new Dictionary<string, UPMRepPossibleValue>();
            }

            string groupKey = possibleValue.RepOrgGroupId;
            if (!string.IsNullOrEmpty(groupKey))
            {
                UPMRepPossibleValue groupValue = this.possibleGroups.ValueOrDefault(groupKey);
                if (groupValue != null && groupValue.GroupReps == null)
                {
                    groupValue.GroupReps = new List<UPMRepPossibleValue>();
                }

                if (groupValue != null)
                {
                    groupValue.GroupReps.Add(possibleValue);
                    this.possibleGroups.SetObjectForKey(groupValue, groupKey);
                }
            }

            this.possibleValues.SetObjectForKey(possibleValue, key);
        }

        /// <summary>
        /// The add root rep group.
        /// </summary>
        /// <param name="repOrgGroup">
        /// The rep org group.
        /// </param>
        /// <returns>
        /// The <see cref="UPMRepEditFieldFilter"/>.
        /// </returns>
        public UPMRepEditFieldFilter AddRootRepGroup(UPMRepPossibleValue repOrgGroup)
        {
            if (repOrgGroup == null)
            {
                return null;
            }

            if (this.RepEditFieldFilters == null)
            {
                this.RepEditFieldFilters = new List<UPMRepEditFieldFilter>();
            }

            UPMRepEditFieldFilter repEditFieldFilter = new UPMRepEditFieldFilter(repOrgGroup);
            if (this.RepEditFieldFilters.Contains(repEditFieldFilter) == false)
            {
                this.RepEditFieldFilters.Add(repEditFieldFilter);
            }
            else
            {
                repEditFieldFilter = this.RepEditFieldFilters[this.RepEditFieldFilters.IndexOf(repEditFieldFilter)];
            }

            return repEditFieldFilter;
        }

        /// <summary>
        /// Returns PossibleGroup for given key.
        /// </summary>
        /// <param name="key">
        /// </param>
        /// <returns>
        /// The <see cref="UPMRepPossibleValue"/>.
        /// </returns>
        public UPMRepPossibleValue PossibleGroupForKey(string key)
        {
            return this.possibleGroups[key];
        }

        /// <summary>
        /// Returns PossibleValue for given key.
        /// </summary>
        /// <param name="key">
        /// </param>
        /// <returns>
        /// The <see cref="UPMRepPossibleValue"/>.
        /// </returns>
        public UPMRepPossibleValue PossibleValueForKey(string key)
        {
            if (this.possibleValues.ContainsKey(key))
            {
                return this.possibleValues[key];
            }
            return null;
        }

        /// <summary>
        /// The rep key deselected.
        /// </summary>
        /// <param name="deselectedRepKey">
        /// The deselected rep key.
        /// </param>
        public void RepKeyDeselected(string deselectedRepKey)
        {
            if (deselectedRepKey == null)
            {
                return;
            }

            this.SelectedRepKeys.Remove(deselectedRepKey);
            if (this.MultiSelectMode == false)
            {
                this.Delegate?.DidSelectRepKeys(this, this.SelectedRepKeys);
            }

#if PORTING
            NSNotificationCenter.DefaultCenter().PostNotificationNameTheObject(Constants.UPMRepContanerChangeNotification, this);
#endif
        }

        /// <summary>
        /// The rep key selected.
        /// </summary>
        /// <param name="selectedRepKey">
        /// The selected rep key.
        /// </param>
        public void RepKeySelected(string selectedRepKey)
        {
            if (selectedRepKey == null)
            {
                return;
            }

            if (this.MultiSelectMode == false)
            {
                this.SelectedRepKeys.Clear();
                this.SelectedRepKeys.Add(selectedRepKey);
                this.Delegate?.DidSelectRepKeys(this, this.SelectedRepKeys);
            }
            else
            {
                if (this.SelectedRepKeys.Contains(selectedRepKey) == false)
                {
                    this.SelectedRepKeys.Add(selectedRepKey);
                }
            }

            // TODO: What to do with these?
#if PORTING
            NSNotificationCenter.DefaultCenter().PostNotificationNameTheObject(Constants.UPMRepContanerChangeNotification, this);
#endif
        }

        /// <summary>
        /// Clears possible value dictionary.
        /// </summary>
        public void ResetPossibleValues()
        {
            this.possibleValues = null;
        }

        /// <summary>
        /// Resets rep edit field filters.
        /// </summary>
        public void ResetRepEditFieldFilters()
        {
            foreach (UPMRepEditFieldFilter repEditFieldFilter in this.RepEditFieldFilters)
            {
                repEditFieldFilter.Active = false;
            }
        }

        /// <summary>
        /// The reset selected keys.
        /// </summary>
        public void ResetSelectedKeys()
        {
            this.SelectedRepKeys.Clear();
        }

        /// <summary>
        /// The set last used rep keys.
        /// </summary>
        /// <param name="lastUsedRepKeys">
        /// The last used rep keys.
        /// </param>
        public void SetLastUsedRepKeys(List<string> lastUsedRepKeys)
        {
            this.lastUsedRepKeys = lastUsedRepKeys;
            if (this.recentlyUsedRepKeys == null)
            {
                this.recentlyUsedRepKeys = new List<string>();
            }

            foreach (string repKey in this.lastUsedRepKeys)
            {
                if (this.recentlyUsedRepKeys.Contains(repKey))
                {
                    this.recentlyUsedRepKeys.Remove(repKey);
                }

                this.recentlyUsedRepKeys.Insert(0, repKey);
            }

            while (this.recentlyUsedRepKeys.Count > 100)
            {
                this.recentlyUsedRepKeys.RemoveAt(this.recentlyUsedRepKeys.Count - 1);
            }
        }
    }
}
