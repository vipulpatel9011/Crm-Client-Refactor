// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownControlGroupModelController.cs" company="Aurea Software Gmbh">
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
//   The UnknownControlGroupModelController
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Group
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Groups;

    /// <summary>
    /// UnknownControlGroupModelController
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Group.UPGroupModelController" />
    public class UnknownControlGroupModelController : UPGroupModelController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownControlGroupModelController"/> class.
        /// </summary>
        /// <param name="formItem">The form item.</param>
        /// <param name="identifier">The identifier.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UnknownControlGroupModelController(FormItem formItem, IIdentifier identifier, IGroupModelControllerDelegate theDelegate)
            : base(theDelegate)
        {
            this.FormItem = formItem;
            this.ViewReference = formItem.ViewReference;
            this.ExplicitTabIdentifier = identifier;
            this.ExplicitLabel = this.FormItem.Label;
        }

        /// <summary>
        /// Gets the view reference.
        /// </summary>
        /// <value>
        /// The view reference.
        /// </value>
        public ViewReference ViewReference { get; private set; }

        /// <summary>
        /// Applies the context.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns></returns>
        public override UPMGroup ApplyContext(Dictionary<string, object> dictionary)
        {
            UPMStandardGroup detailGroup = new UPMStandardGroup(this.ExplicitTabIdentifier);
            detailGroup.LabelText = this.FormItem.Label;
            UPMStringField field = new UPMStringField(this.ExplicitTabIdentifier)
            {
                LabelText = "ViewName",
                StringValue = this.ViewReference.ViewName
            };
            detailGroup.AddChild(field);
            foreach (ReferenceArgument arg in this.FormItem.ViewReference.Arguments.Values)
            {
                field = new UPMStringField(this.ExplicitTabIdentifier)
                {
                    LabelText = arg.Name,
                    StringValue = arg.Value
                };
                detailGroup.AddChild(field);
            }

            this.ControllerState = GroupModelControllerState.Finished;
            this.Group = detailGroup;
            return detailGroup;
        }
    }
}
