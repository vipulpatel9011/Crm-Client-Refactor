// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPQuestionnaireStringEditField.cs" company="Aurea Software Gmbh">
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
//   The Questionnaire String Edit Field
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Questionnaire
{
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields.Edit;
    using Core.Questionnaire;

    /// <summary>
    /// Questionnaire String Edit Field
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.Fields.Edit.UPMStringEditField" />
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Questionnaire.IQuestionnaireEditFieldContext" />
    public class UPQuestionnaireStringEditField : UPMStringEditField, IQuestionnaireEditFieldContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMStringEditField"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPQuestionnaireStringEditField(IIdentifier identifier) 
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets the question.
        /// </summary>
        /// <value>
        /// The question.
        /// </value>
        public UPQuestionnaireQuestion Question { get; set; }
    }
}
