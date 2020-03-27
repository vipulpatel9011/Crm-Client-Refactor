// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSurveyActionResult.cs" company="Aurea Software Gmbh">
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
//   The Survey Action Result
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Questionnaire
{
    /// <summary>
    /// Survey Action Result
    /// </summary>
    public class UPSurveyActionResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSurveyActionResult"/> class.
        /// </summary>
        public UPSurveyActionResult()
        {
            this.Ok = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UPSurveyActionResult"/> is ok.
        /// </summary>
        /// <value>
        ///   <c>true</c> if ok; otherwise, <c>false</c>.
        /// </value>
        public bool Ok { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [question order changed].
        /// </summary>
        /// <value>
        /// <c>true</c> if [question order changed]; otherwise, <c>false</c>.
        /// </value>
        public bool QuestionOrderChanged { get; set; }
    }
}
