// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMQuestionnairePage.cs" company="Aurea Software Gmbh">
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
//   The Questionnaire Edit Organizer Model Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Pages
{
    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    /// Questionnaire Page
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.Pages.Page" />
    public class UPMQuestionnairePage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Page"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMQuestionnairePage(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UPMQuestionnairePage"/> is readonly.
        /// </summary>
        /// <value>
        ///   <c>true</c> if readonly; otherwise, <c>false</c>.
        /// </value>
        public bool Readonly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [questionnaire not exists].
        /// </summary>
        /// <value>
        /// <c>true</c> if [questionnaire not exists]; otherwise, <c>false</c>.
        /// </value>
        public bool QuestionnaireNotExists { get; set; }

        /// <summary>
        /// Gets or sets the finalized text.
        /// </summary>
        /// <value>
        /// The finalized text.
        /// </value>
        public string FinalizedText { get; set; }

        /// <summary>
        /// Gets or sets the finalize action.
        /// </summary>
        /// <value>
        /// The finalize action.
        /// </value>
        public UPMAction FinalizeAction { get; set; }
    }
}
