// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MParticipantsRecordSelectorEditField.cs" company="Aurea Software Gmbh">
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
//   The UPMParticipantsRecordSelectorEditField.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Fields.Edit
{
    using System.Collections.Generic;

    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields.Edit.Delegates;
    using Aurea.CRM.UIModel.Groups;

    /// <summary>
    /// UPMParticipantsRecordSelectorEditField
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.Fields.Edit.UPMRecordSelectorEditField" />
    public class UPMParticipantsRecordSelectorEditField : UPMRecordSelectorEditField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMParticipantsRecordSelectorEditField"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMParticipantsRecordSelectorEditField(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets the group model controller.
        /// </summary>
        /// <value>
        /// The group model controller.
        /// </value>
        public UPParticipantsGroupModelControllerDelegate GroupModelController { get; set; }

        /// <summary>
        /// Gets or sets the participant.
        /// </summary>
        /// <value>
        /// The participant.
        /// </value>
        public UPCRMLinkParticipant Participant { get; set; }

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        /// <value>
        /// The group.
        /// </value>
        public UPMGroup Group { get; set; }

        /// <summary>
        /// Gets the display value.
        /// </summary>
        /// <value>
        /// The display value.
        /// </value>
        public string DisplayValue => (string)this.FieldValue;

        /// <summary>
        /// Gets the link record identification.
        /// </summary>
        /// <value>
        /// The link record identification.
        /// </value>
        public override string LinkRecordIdentification => this.Participant.LinkRecordIdentification;

        /// <summary>
        /// Users the did change field with context.
        /// </summary>
        /// <param name="pageModelController">The page model controller.</param>
        /// <returns></returns>
        public List<IIdentifier> UserDidChangeFieldWithContext(object pageModelController)
        {
            return this.GroupModelController?.UserDidChangeField(this, pageModelController);
        }
    }
}
