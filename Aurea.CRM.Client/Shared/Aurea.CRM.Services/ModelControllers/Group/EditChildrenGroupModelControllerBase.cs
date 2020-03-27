// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditChildrenGroupModelControllerBase.cs" company="Aurea Software Gmbh">
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
//   The Edit Children Group Model Controller Base class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Group
{
    using System;
    using System.Collections.Generic;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Contexts;
    using Aurea.CRM.UIModel.Groups;
    using Core.CRM.DataModel;

    /// <summary>
    /// The Edit Children Group Model Controller Base class
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Group.UPFieldControlBasedEditGroupModelController" />
    public abstract class UPEditChildrenGroupModelControllerBase : UPFieldControlBasedEditGroupModelController
    {
        private int _displayIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPEditChildrenGroupModelControllerBase"/> class.
        /// </summary>
        /// <param name="fieldControl">The field control.</param>
        /// <param name="tabIndex">Index of the tab.</param>
        /// <param name="editPageContext">The edit page context.</param>
        /// <param name="theDelegate">The delegate.</param>
        protected UPEditChildrenGroupModelControllerBase(FieldControl fieldControl, int tabIndex,
            UPEditPageContext editPageContext, IGroupModelControllerDelegate theDelegate)
            : base(fieldControl, tabIndex, editPageContext, theDelegate)
        {
            this.AddRecordEnabled = this.AddRecordEnabledDefault(this.EditPageContext.IsNewRecord);
            this.DeleteRecordEnabled = this.DeleteRecordEnabledDefault;

            string attrValue = fieldControl.ValueForAttribute($"AddRecords_{tabIndex + 1}");

            if (!string.IsNullOrEmpty(attrValue))
            {
                attrValue = fieldControl.ValueForAttribute($"{(editPageContext.IsNewRecord ? @"New" : @"Update")}.AddRecords_{tabIndex + 1}");
            }

            if (!string.IsNullOrEmpty(attrValue))
            {
                this.MaxChildren = Convert.ToInt32(attrValue);
                this.AddRecordEnabled = this.MaxChildren > 0;
            }

            attrValue = fieldControl.ValueForAttribute($"MaxRecords_{tabIndex + 1}");
            if (!string.IsNullOrEmpty(attrValue))
            {
                attrValue = fieldControl.ValueForAttribute($"{(editPageContext.IsNewRecord ? @"New" : @"Update")}.MaxRecords_{tabIndex + 1}");
            }

            if (!string.IsNullOrEmpty(attrValue))
            {
                this.MinChildren = Convert.ToInt32(attrValue);
            }

            attrValue = fieldControl.ValueForAttribute($"DeleteRecord_{tabIndex + 1}");

            if (!string.IsNullOrEmpty(attrValue))
            {
                this.DeleteRecordEnabled = Convert.ToBoolean(attrValue);
            }

            this._displayIndex = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPEditChildrenGroupModelControllerBase"/> class.
        /// </summary>
        /// <param name="fieldControl">The field control.</param>
        /// <param name="tabIndex">Index of the tab.</param>
        /// <param name="theDelegate">The delegate.</param>
        protected UPEditChildrenGroupModelControllerBase(FieldControl fieldControl, int tabIndex, IGroupModelControllerDelegate theDelegate)
            : base(fieldControl, tabIndex, theDelegate)
        {
        }

        /// <summary>
        /// Gets the child group key.
        /// </summary>
        /// <value>
        /// The child group key.
        /// </value>
        public virtual string ChildGroupKey => null;

        /// <summary>
        /// Gets the target information area identifier.
        /// </summary>
        /// <value>
        /// The target information area identifier.
        /// </value>
        public virtual string TargetInfoAreaId => null;

        /// <summary>
        /// Gets the target link identifier.
        /// </summary>
        /// <value>
        /// The target link identifier.
        /// </value>
        public virtual int TargetLinkId => -1;

        /// <summary>
        /// Gets a value indicating whether [add record enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [add record enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool AddRecordEnabled { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [delete record enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [delete record enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool DeleteRecordEnabled { get; private set; }

        /// <summary>
        /// Gets the minimum children.
        /// </summary>
        /// <value>
        /// The minimum children.
        /// </value>
        public int MinChildren { get; private set; }

        /// <summary>
        /// Gets the maximum children.
        /// </summary>
        /// <value>
        /// The maximum children.
        /// </value>
        public int MaxChildren { get; private set; }

        /// <summary>
        /// Gets the display index.
        /// </summary>
        /// <value>
        /// The display index.
        /// </value>
        public int DisplayIndex => this._displayIndex;

        /// <summary>
        /// Gets or sets the link record identification.
        /// </summary>
        /// <value>
        /// The link record identification.
        /// </value>
        public string LinkRecordIdentification { get; set; }

        /// <summary>
        /// Gets the next postfix.
        /// </summary>
        /// <value>
        /// The next postfix.
        /// </value>
        public string NextPostfix => $"{++this._displayIndex}";

        /// <summary>
        /// Gets a value indicating whether [delete record enabled default].
        /// </summary>
        /// <value>
        /// <c>true</c> if [delete record enabled default]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool DeleteRecordEnabledDefault { get; } = true;

        /// <summary>
        /// Childs the group key for information area identifier.
        /// </summary>
        /// <param name="infoAreaId">The information area identifier.</param>
        /// <param name="linkId">The link identifier.</param>
        /// <returns></returns>
        public static string ChildGroupKeyForInfoAreaIdLinkId(string infoAreaId, int linkId)
        {
            return $".{infoAreaId}.{(linkId < 0 ? 0 : linkId)}";
        }

        /// <summary>
        /// Adds the record enabled default.
        /// </summary>
        /// <param name="isNewRecord">if set to <c>true</c> [is new record].</param>
        /// <returns></returns>
        public virtual bool AddRecordEnabledDefault(bool isNewRecord)
        {
            return true;
        }

        /// <summary>
        /// Child group key for information area identifier.
        /// </summary>
        /// <param name="initialValues">The initial values.</param>
        public virtual void AddGroupsWithInitialValues(List<Dictionary<string, object>> initialValues)
        {
        }

        /// <summary>
        /// Options from string array.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns></returns>
        public static Dictionary<string, object> OptionsFromStringArray(List<string> array)
        {
            if (array.Count == 0)
            {
                return null;
            }

            Dictionary<string, object> dict;
            Dictionary<string, object> par0Dict = null;

            string string0 = array[0];

            if (string0.StartsWith(@"{"))
            {
                par0Dict = string0.JsonDictionaryFromString();
            }

            if (par0Dict != null && par0Dict.Count > 0)
            {
                dict = new Dictionary<string, object>(par0Dict);
            }
            else
            {
                dict = new Dictionary<string, object>(array.Count);
            }

            foreach (string part in array)
            {
                if (par0Dict != null)
                {
                    par0Dict = null;
                    continue;
                }

                dict.Add(part, true);
            }

            return dict;
        }

        /// <summary>
        /// Changeds the links for information area identifier.
        /// </summary>
        /// <param name="infoAreaId">The information area identifier.</param>
        /// <param name="userChangedOnly">if set to <c>true</c> [user changed only].</param>
        /// <returns></returns>
        public virtual Dictionary<string, UPCRMLink> ChangedLinksForInfoAreaId(string infoAreaId, bool userChangedOnly)
        {
            return null;
        }

        /// <summary>
        /// Handles removing an item from repeatable edit group
        /// </summary>
        /// <param name="group">Group to remove</param>
        /// <param name="repeatableEditGroup">Repeatable edit group to remove from</param>
        public virtual void UserWillDeleteGroupInRepeatableEditGroup(UPMGroup group, UPMRepeatableEditGroup repeatableEditGroup)
        {
        }

        /// <summary>
        /// Handles adding new item to repeatable edit group
        /// </summary>
        /// <param name="repeatableEditGroup">Repeatable edit group for adding new item</param>
        public virtual void UserWillInsertNewGroupInRepeatableEditGroup(UPMRepeatableEditGroup repeatableEditGroup)
        {
        }

        /// <summary>
        /// Returns changed child records list for given parent record
        /// </summary>
        /// <param name="parentRecord">Parent record</param>
        /// <param name="userChangesOnly">User changes only</param>
        /// <returns><see cref="List{UPCRMRecord}"/></returns>
        public virtual List<UPCRMRecord> ChangedChildRecordsForParentRecord(UPCRMRecord parentRecord, bool userChangesOnly)
        {
            return null;
        }

        /// <summary>
        /// Constraints the violations with page context.
        /// </summary>
        /// <param name="editPageContext">The edit page context.</param>
        /// <returns></returns>
        public virtual List<UPEditConstraintViolation> ConstraintViolationsWithPageContext(UPEditPageContext editPageContext)
        {
            return null;
        }
    }
}
