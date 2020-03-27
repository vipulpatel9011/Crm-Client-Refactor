// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSurveyAnswerSingle.cs" company="Aurea Software Gmbh">
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
//   Survey Answer Single
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Questionnaire
{
    /// <summary>
    /// UPSurveyAnswerSingle
    /// </summary>
    public class UPSurveyAnswerSingle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSurveyAnswerSingle"/> class.
        /// </summary>
        /// <param name="answerId">The answer identifier.</param>
        /// <param name="recordIdentification">The record identification.</param>
        public UPSurveyAnswerSingle(string answerId, string recordIdentification = null)
        {
            this.RecordIdentification = recordIdentification;
            this.AnswerId = answerId;
            this.Deleted = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [check delete].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [check delete]; otherwise, <c>false</c>.
        /// </value>
        public bool CheckDelete { get; set; }

        /// <summary>
        /// Gets the answer identifier.
        /// </summary>
        /// <value>
        /// The answer identifier.
        /// </value>
        public string AnswerId { get; private set; }

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UPSurveyAnswerSingle"/> is deleted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if deleted; otherwise, <c>false</c>.
        /// </value>
        public bool Deleted { get; set; }
    }
}
