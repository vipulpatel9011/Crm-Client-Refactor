// <copyright file="IEditTableViewDataProvider.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>

namespace Aurea.CRM.Services.Delegates
{
    using Aurea.CRM.Services.ModelControllers.Search;
    using Aurea.CRM.UIModel.Fields.Edit;
    using Aurea.CRM.UIModel.Groups;

    /// <summary>
    /// The EditTableViewDataProvider interface.
    /// </summary>
    public interface IEditTableViewDataProvider
    {
        /// <summary>
        /// Gets the group count.
        /// </summary>
        int GroupCount { get; }

        /// <summary>
        /// The group at index.
        /// </summary>
        /// <param name="index">
        /// The _index.
        /// </param>
        /// <returns>
        /// The <see cref="UPMGroup"/>.
        /// </returns>
        UPMGroup GroupAtIndex(int index);

        /// <summary>
        /// The user did change field.
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        void UserDidChangeField(UPMEditField field);

        /// <summary>
        /// Will insert new group in repeatable edit group.
        /// </summary>
        /// <param name="repeatableEditGroup">
        /// The repeatable edit group.
        /// </param>
        void UserWillInsertNewGroupInRepeatableEditGroup(UPMRepeatableEditGroup repeatableEditGroup);

        /// <summary>
        /// The user will delete group in repeatable edit group.
        /// </summary>
        /// <param name="group">
        /// The group.
        /// </param>
        /// <param name="repeatableEditGroup">
        /// The repeatable edit group.
        /// </param>
        void UserWillDeleteGroupInRepeatableEditGroup(UPMGroup group, UPMRepeatableEditGroup repeatableEditGroup);

        /// <summary>
        /// Creates and returns an instance of <see cref="UPRecordSelectorPageModelController"/> based on given <see cref="UPMRecordSelectorEditField"/> object.
        /// </summary>
        /// <param name="editField">Edit field</param>
        /// <returns><see cref="UPRecordSelectorPageModelController"/></returns>
        UPRecordSelectorPageModelController RecordSelectorPageModelControllerForField(UPMRecordSelectorEditField editField);

    }
}
