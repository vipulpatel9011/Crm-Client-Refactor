// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMQuestionnaireQuestion.cs" company="Aurea Software Gmbh">
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
//   Questionnaire Question
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Questionnaire
{
    using Aurea.CRM.UIModel.Fields;

    /// <summary>
    /// Questionnaire Question
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.UPMContainer" />
    public class UPMQuestionnaireQuestion : UPMContainer
    {
        private UPMField field;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMQuestionnaireQuestion"/> class.
        /// </summary>
        /// <param name="_field">The field.</param>
        public UPMQuestionnaireQuestion(UPMField _field)
            : base(_field.Identifier)
        {
            this.field = _field;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="UPMQuestionnaireQuestion"/> is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if empty; otherwise, <c>false</c>.
        /// </value>
        public bool Empty => this.field.Empty;

        /// <summary>
        /// Gets the field.
        /// </summary>
        /// <value>
        /// The field.
        /// </value>
        public UPMField Field => this.field;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UPMQuestionnaireQuestion"/> is mandatory.
        /// </summary>
        /// <value>
        ///   <c>true</c> if mandatory; otherwise, <c>false</c>.
        /// </value>
        public bool Mandatory { get; set; }
    }
}
