// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IQuestionnaireEditFieldContext.cs" company="Aurea Software Gmbh">
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
//   The Questionnaire Edit Field Context
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Aurea.CRM.Core.Questionnaire;

namespace Aurea.CRM.Services.ModelControllers.Questionnaire
{
    /// <summary>
    /// Questionnaire Edit Field Context
    /// </summary>
    public interface IQuestionnaireEditFieldContext
    {
        /// <summary>
        /// Gets or sets the question.
        /// </summary>
        /// <value>
        /// The question.
        /// </value>
        UPQuestionnaireQuestion Question { get; set; }
    }
}
