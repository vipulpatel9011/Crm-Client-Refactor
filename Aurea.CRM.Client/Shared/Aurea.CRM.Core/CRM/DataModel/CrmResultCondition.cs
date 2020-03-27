﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmResultCondition.cs" company="Aurea Software Gmbh">
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
    using System.Linq;

    /// <summary>
    /// CRM result condition
    /// </summary>
    public class UPCRMResultCondition
    {
        /// <summary>
        /// Checks the specified row.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public virtual bool Check(UPCRMResultRow row) => true;

        /// <summary>
        /// Conditions the by appending and condition.
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMResultCondition"/>.
        /// </returns>
        public UPCRMResultCondition ConditionByAppendingANDCondition(UPCRMResultCondition condition)
        {
            return new UPCRMResultANDCondition(new List<UPCRMResultCondition> { this, condition });
        }

        /// <summary>
        /// Conditions the by appending or condition.
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMResultCondition"/>.
        /// </returns>
        public UPCRMResultCondition ConditionByAppendingORCondition(UPCRMResultCondition condition)
        {
            return new UPCRMResultORCondition(new List<UPCRMResultCondition> { this, condition });
        }

        /// <summary>
        /// Conditions the by negating condition.
        /// </summary>
        /// <returns>
        /// The <see cref="UPCRMResultCondition"/>.
        /// </returns>
        public UPCRMResultCondition ConditionByNegatingCondition()
        {
            return new UPCRMResultNOTCondition(this);
        }
    }
}
