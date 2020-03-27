// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParticipantsEditFieldContext.cs" company="Aurea Software Gmbh">
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
//   The Participants Edit Field Context
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Edit
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Services.ModelControllers.Group;
    using Aurea.CRM.UIModel.Contexts;

    /// <summary>
    /// Participants Edit Field Context
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.Contexts.UPEditFieldContext" />
    public class UPParticipantsEditFieldContext : UPEditFieldContext
    {
        /// <summary>
        /// Gets or sets the group model controller.
        /// </summary>
        /// <value>
        /// The group model controller.
        /// </value>
        public UPEditRepParticipantsGroupModelController GroupModelController { get; set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public override string Value => this.GroupModelController.ParticipantsControl.ParticipantString;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPParticipantsEditFieldContext" /> class.
        /// </summary>
        /// <param name="fieldConfig">The field configuration.</param>
        /// <param name="fieldIdentifier">The field identifier.</param>
        /// <param name="value">The value.</param>
        /// <param name="childFields">The child fields.</param>
        public UPParticipantsEditFieldContext(UPConfigFieldControlField fieldConfig, IIdentifier fieldIdentifier, string value, List<UPEditFieldContext> childFields)
            : base(fieldConfig, fieldIdentifier, value, childFields)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPParticipantsEditFieldContext"/> class.
        /// </summary>
        /// <param name="fieldConfig">The field configuration.</param>
        /// <param name="fieldIdentifier">The field identifier.</param>
        /// <param name="value">The value.</param>
        public UPParticipantsEditFieldContext(WebConfigLayoutField fieldConfig, IIdentifier fieldIdentifier, string value)
            : base(fieldConfig, fieldIdentifier, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPParticipantsEditFieldContext"/> class.
        /// </summary>
        /// <param name="fieldId">The field identifier.</param>
        /// <param name="value">The value.</param>
        public UPParticipantsEditFieldContext(int fieldId, string value)
            : base(fieldId, value)
        {
        }

        /// <summary>
        /// Wases the changed.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Boolean" />.
        /// </returns>
        public override bool WasChanged()
        {
            return this.OriginalValue != this.GroupModelController.ParticipantsControl.ParticipantString;
        }
    }
}
