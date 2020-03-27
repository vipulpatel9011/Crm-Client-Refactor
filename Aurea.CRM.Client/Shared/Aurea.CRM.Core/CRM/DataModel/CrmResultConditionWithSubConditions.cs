// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmResultConditionWithSubConditions.cs" company="Aurea Software Gmbh">
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
    using System.Collections.Generic;

    /// <summary>
    /// CRM result condition with sub-conditions
    /// </summary>
    /// <seealso cref="UPCRMResultCondition" />
    public class UPCRMResultConditionWithSubConditions : UPCRMResultCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMResultConditionWithSubConditions"/> class.
        /// </summary>
        /// <param name="conditions">
        /// The conditions.
        /// </param>
        public UPCRMResultConditionWithSubConditions(List<UPCRMResultCondition> conditions)
        {
            this.Conditions = conditions;
        }

        /// <summary>
        /// Gets the conditions.
        /// </summary>
        /// <value>
        /// The conditions.
        /// </value>
        public List<UPCRMResultCondition> Conditions { get; private set; }
    }
}
