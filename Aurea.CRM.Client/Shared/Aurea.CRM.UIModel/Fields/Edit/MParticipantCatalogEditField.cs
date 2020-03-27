// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MParticipantCatalogEditField.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The upm participant catalog edit field.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel.Fields.Edit
{
    using System.Collections.Generic;

    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields.Edit.Delegates;
    using Aurea.CRM.UIModel.Groups;

    /// <summary>
    /// The upm participant catalog edit field.
    /// </summary>
    public class UPMParticipantCatalogEditField : UPMCatalogEditField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMParticipantCatalogEditField"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMParticipantCatalogEditField(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMParticipantCatalogEditField"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        /// <param name="multiSelectMode">
        /// The multi select mode.
        /// </param>
        public UPMParticipantCatalogEditField(IIdentifier identifier, bool multiSelectMode)
            : base(identifier, multiSelectMode)
        {
        }

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        public UPMGroup Group { get; set; }

        /// <summary>
        /// Gets or sets the group model controller.
        /// </summary>
        public UPParticipantsGroupModelControllerDelegate GroupModelController { get; set; }

        /// <summary>
        /// The user did change field with context.
        /// </summary>
        /// <param name="pageModelController">
        /// The page model controller.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<IIdentifier> UserDidChangeField(object pageModelController)
        {
            return this.GroupModelController.UserDidChangeField(this, pageModelController);
        }
    }
}
