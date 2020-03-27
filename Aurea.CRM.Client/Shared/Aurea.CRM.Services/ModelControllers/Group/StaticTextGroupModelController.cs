// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StaticTextGroupModelController.cs" company="Aurea Software Gmbh">
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
//   The UPStaticTextGroupModelController
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
    /// UPStaticTextGroupModelController
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Group.UPGroupModelController" />
    public class UPStaticTextGroupModelController : UPGroupModelController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPStaticTextGroupModelController"/> class.
        /// </summary>
        /// <param name="formItem">The form item.</param>
        /// <param name="identifier">The identifier.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPStaticTextGroupModelController(FormItem formItem, IIdentifier identifier, IGroupModelControllerDelegate theDelegate)
            : base(theDelegate)
        {
            this.ExplicitTabIdentifier = identifier;
            this.ExplicitLabel = formItem.Label;
            this.FormItem = formItem;
        }

        /// <summary>
        /// Applies the context.
        /// </summary>
        /// <param name="contextDictionary">The context dictionary.</param>
        /// <returns></returns>
        public override UPMGroup ApplyContext(Dictionary<string, object> contextDictionary)
        {
            base.ApplyContext(contextDictionary);
            UPMMultilineGroup multiLineGroup = new UPMMultilineGroup(this.ExplicitTabIdentifier);
            var stringParts = this.TabLabel.Split(';');
            string labelText;
            string fieldText;
            if (stringParts.Length > 1)
            {
                labelText = stringParts[0];
                fieldText = stringParts[1];
            }
            else
            {
                UPGroupModelController root = this.RootGroupModelController;
                if (root != null)
                {
                    fieldText = this.TabLabel;
                    labelText = root.TabLabel;
                }
                else
                {
                    labelText = this.TabLabel;
                    fieldText = null;
                }
            }

            UPMStringField field = new UPMStringField(this.ExplicitTabIdentifier);
            multiLineGroup.LabelText = labelText;
            multiLineGroup.MultilineStringField = field;
            field.StringValue = fieldText;
            this.Group = multiLineGroup;
            this.ControllerState = GroupModelControllerState.Finished;
            return multiLineGroup;
        }
    }
}
