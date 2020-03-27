// <copyright file="IRecordSelectorEditFieldDelegate.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>

namespace Aurea.CRM.UIModel.Fields.Edit.Delegates
{
    /// <summary>
    /// Delegate interface for a record selector editor
    /// </summary>
    public interface IRecordSelectorEditFieldDelegate
    {
        /// <summary>
        /// Contexts the record for edit field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns><see cref="string"/></returns>
        string ContextRecordForEditField(UPMEditField field);

        /// <summary>
        /// Currents the record for edit field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns><see cref="string"/></returns>
        string CurrentRecordForEditField(UPMEditField field);
    }
}
