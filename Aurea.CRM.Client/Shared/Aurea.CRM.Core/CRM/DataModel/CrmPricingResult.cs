// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmPricingResult.cs" company="Aurea Software Gmbh">
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
//   Pricing Result
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    using Aurea.CRM.Core.Configuration;

    /// <summary>
    /// Pricing Result
    /// </summary>
    public class UPCRMPricingResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMPricingResult"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="fieldControl">The field control.</param>
        public UPCRMPricingResult(UPCRMResult result, FieldControl fieldControl)
        {
            this.Result = result;
            this.FieldControl = fieldControl;
        }

        /// <summary>
        /// Gets or sets the field control.
        /// </summary>
        /// <value>
        /// The field control.
        /// </value>
        public FieldControl FieldControl { get; set; }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public UPCRMResult Result { get; set; }
    }
}
