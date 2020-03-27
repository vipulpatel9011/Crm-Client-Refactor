// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPParticipantsGroupModelControllerDelegate.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The PParticipantsGroupModelControllerDelegate interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel.Fields.Edit.Delegates
{
    using System.Collections.Generic;

    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    /// The UPParticipantsGroupModelControllerDelegate interface.
    /// </summary>
    public interface UPParticipantsGroupModelControllerDelegate
    {
        /// <summary>
        /// Notifies the delegate about fields value has been changed.
        /// </summary>
        /// <param name="field">
        /// Field info
        /// </param>
        /// <param name="pageModelController">
        /// Page Model Controller data
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        List<IIdentifier> UserDidChangeField(UPMEditField field, object pageModelController);
    }
}
