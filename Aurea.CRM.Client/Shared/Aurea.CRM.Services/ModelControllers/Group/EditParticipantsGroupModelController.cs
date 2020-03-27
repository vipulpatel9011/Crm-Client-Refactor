// <copyright file="EditParticipantsGroupModelController.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>

namespace Aurea.CRM.Services.ModelControllers.Group
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.UIModel.Contexts;
    using Aurea.CRM.UIModel.Groups;

    /// <summary>
    /// EditParticipants group model controller.
    /// </summary>
    public class UPEditParticipantsGroupModelController : UPEditChildrenGroupModelControllerBase
    {
        /// <summary>
        /// The key to edit group dictionary
        /// </summary>
        protected Dictionary<string, UPMGroup> keyToEditGroupDictionary;

        /// <summary>
        /// Gets or sets the participants control.
        /// </summary>
        /// <value>
        /// The participants control.
        /// </value>
        public UPCRMMutableParticipants ParticipantsControl { get; protected set; }

        /// <summary>
        /// Gets the participants field.
        /// </summary>
        /// <value>
        /// The participants field.
        /// </value>
        public UPConfigFieldControlField ParticipantsField { get; private set; }

        /// <summary>
        /// Gets the name of the link participants.
        /// </summary>
        /// <value>
        /// The name of the link participants.
        /// </value>
        public string LinkParticipantsName { get; private set; }

        /// <summary>
        /// Gets the link identifier.
        /// </summary>
        /// <value>
        /// The link identifier.
        /// </value>
        public int LinkId { get; private set; }

        /// <summary>
        /// Gets the root information area identifier.
        /// </summary>
        /// <value>
        /// The root information area identifier.
        /// </value>
        public string RootInfoAreaId { get; private set; }

        /// <summary>
        /// Gets the participant information area identifier.
        /// </summary>
        /// <value>
        /// The participant information area identifier.
        /// </value>
        public string ParticipantInfoAreaId { get; private set; }

        /// <summary>
        /// Gets the participant link identifier.
        /// </summary>
        /// <value>
        /// The participant link identifier.
        /// </value>
        public int ParticipantLinkId { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPEditParticipantsGroupModelController"/> class.
        /// </summary>
        /// <param name="_fieldControl">The field control.</param>
        /// <param name="_tabIndex">Index of the tab.</param>
        /// <param name="_editPageContext">The edit page context.</param>
        /// <param name="_theDelegate">The delegate.</param>
        public UPEditParticipantsGroupModelController(FieldControl _fieldControl, int _tabIndex, UPEditPageContext _editPageContext, IGroupModelControllerDelegate _theDelegate)
            : base(_fieldControl, _tabIndex, _editPageContext, _theDelegate)
        {
            int fieldCount = this.TabConfig.NumberOfFields;
            for (int i = 0; i < fieldCount; i++)
            {
                UPConfigFieldControlField fieldConfig = this.TabConfig.FieldAtIndex(i);
                if (fieldConfig.Field.FieldInfo.IsParticipantsField)
                {
                    this.ParticipantsField = fieldConfig;
                    break;
                }
            }

            var tabTypeParts = this.TabConfig.Type.Split('_');
            this.LinkParticipantsName = null;
            if (tabTypeParts.Length > 1)
            {
                this.LinkParticipantsName = tabTypeParts[1];
                var configParts = this.LinkParticipantsName.Split('#');
                if (configParts.Length > 1)
                {
                    this.LinkParticipantsName = configParts[0];
                    this.LinkId = Convert.ToInt32(configParts[1]);
                }

                SearchAndList searchAndList = ConfigurationUnitStore.DefaultStore.SearchAndListByName(this.LinkParticipantsName);
                if (searchAndList != null)
                {
                    this.ParticipantInfoAreaId = searchAndList.InfoAreaId;
                }
                else
                {
                    FieldControl fieldControl = ConfigurationUnitStore.DefaultStore.FieldControlByNameFromGroup("List", this.LinkParticipantsName);
                    this.ParticipantInfoAreaId = fieldControl.InfoAreaId;
                }
            }

            this.RootInfoAreaId = _fieldControl.InfoAreaId;
        }

        /// <summary>
        /// Edits the group for key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public UPMGroup EditGroupForKey(string key)
        {
            return this.keyToEditGroupDictionary[key];
        }

        /// <summary>
        /// Keys for edit group.
        /// </summary>
        /// <param name="repeatableEditGroup">The repeatable edit group.</param>
        /// <returns></returns>
        public string KeyForEditGroup(UPMGroup repeatableEditGroup)
        {
            return this.keyToEditGroupDictionary.Keys.FirstOrDefault(key => this.keyToEditGroupDictionary[key] == repeatableEditGroup);
        }

        /// <summary>
        /// Adds the group for key.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="key">The key.</param>
        public void AddGroupForKey(UPMGroup group, string key)
        {
            if (key == null)
            {
                return;
            }

            if (this.keyToEditGroupDictionary == null)
            {
                this.keyToEditGroupDictionary = new Dictionary<string, UPMGroup>();
            }

            this.keyToEditGroupDictionary[key] = group;
        }

        /// <summary>
        /// Handles removing an item from repeatable edit group
        /// </summary>
        /// <param name="group">Group to remove</param>
        /// <param name="repeatableEditGroup">Repeatable edit group to remove from</param>
        public override void UserWillDeleteGroupInRepeatableEditGroup(UPMGroup group, UPMRepeatableEditGroup repeatableEditGroup)
        {
            string participantKey = this.KeyForEditGroup(group);
            if (!string.IsNullOrEmpty(participantKey))
            {
                UPCRMParticipant participant = this.ParticipantsControl.ParticipantWithKey(participantKey);
                if (participant != null && this.ParticipantsControl.MarkParticipantAsDeleted(participant))
                {
                    repeatableEditGroup.RemoveChild(group);
                    if (this.MaxChildren > 0)
                    {
                        repeatableEditGroup.AddingEnabled = this.AddRecordEnabled;
                    }
                }
            }
        }

        /// <summary>
        /// Creates and returns a new group with given initial values
        /// </summary>
        /// <param name="initialValues">Initial Values</param>
        /// <returns>
        ///   <see cref="UPMStandardGroup" />
        /// </returns>
        public virtual UPMStandardGroup CreateNewGroup(Dictionary<string, string> initialValues)
        {
            return null;
        }

        /// <summary>
        /// Handles adding new item to repeatable edit group
        /// </summary>
        /// <param name="repeatableEditGroup">Repeatable edit group for adding new item</param>
        /// <inheritdoc />
        public override void UserWillInsertNewGroupInRepeatableEditGroup(UPMRepeatableEditGroup repeatableEditGroup)
        {
            Dictionary<string, string> childInitialValues = null;
            string key = this.ChildGroupKey;
            if (key.StartsWith("."))
            {
                key = $"#{key.Substring(1)}";
            }

            List<Dictionary<string, string>> childInitialValueArray = this.EditPageContext.InitialValues.ValueOrDefault(key) as List<Dictionary<string, string>>;
            if (childInitialValueArray?.Count > 0)
            {
                childInitialValues = childInitialValueArray[0];
            }

            UPMStandardGroup group = this.CreateNewGroup(childInitialValues);
            if (group != null)
            {
                repeatableEditGroup.AddChild(group);
            }

            if (this.MaxChildren > 0 && repeatableEditGroup.Groups.Count >= this.MaxChildren)
            {
                repeatableEditGroup.AddingEnabled = false;
            }
        }

        /// <summary>
        /// Explicits the key order by catalog attribute code order.
        /// </summary>
        /// <param name="catalogAttributes">The catalog attributes.</param>
        /// <param name="codeOrder">The code order.</param>
        /// <returns></returns>
        public List<string> ExplicitKeyOrderByCatalogAttributeCodeOrder(UPConfigCatalogAttributes catalogAttributes, List<string> codeOrder)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>(codeOrder.Count);

            foreach (string v in codeOrder)
            {
                dict.Add(v, v);
            }

            List<string> resultArray = catalogAttributes.ValueArray.Where(v => dict.ContainsKey(v.RawValue)).Select(v => v.RawValue).ToList();

            if (resultArray.Count < codeOrder.Count)
            {
                dict = new Dictionary<string, string>(codeOrder.Count);
                foreach (string v in resultArray)
                {
                    dict[v] = v;
                }

                resultArray.AddRange(codeOrder.Where(v => dict.ContainsKey(v)));
            }

            return resultArray;
        }
    }
}
