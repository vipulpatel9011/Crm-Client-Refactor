// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmResultNOTCondition.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Rashan Anushka
// </author>
// <summary>
//   CRM result condition
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    /// <summary>
    /// CRM result NOT condition
    /// </summary>
    /// <seealso cref="UPCRMResultCondition" />
    public class UPCRMResultNOTCondition : UPCRMResultCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMResultNOTCondition"/> class.
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        public UPCRMResultNOTCondition(UPCRMResultCondition condition)
        {
            this.Condition = condition;
        }

        /// <summary>
        /// Gets the condition.
        /// </summary>
        /// <value>
        /// The condition.
        /// </value>
        public UPCRMResultCondition Condition { get; private set; }

        /// <summary>
        /// Checks the specified row.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Check(UPCRMResultRow row)
        {
            return !this.Condition.Check(row);
        }
    }
}
