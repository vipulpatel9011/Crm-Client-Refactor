// <copyright file="ConditionCheckOperator.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//   Serdar Tepeyurt
// </author>

namespace Aurea.CRM.Core.Utilities
{
    /// <summary>
    /// Condition check operator
    /// </summary>
    public enum ConditionCheckOperator
    {
        /// <summary>
        /// Equal
        /// </summary>
        Equal = 0,

        /// <summary>
        /// Not equal
        /// </summary>
        NotEqual,

        /// <summary>
        /// Like
        /// </summary>
        Like,

        /// <summary>
        /// Greater equal
        /// </summary>
        GreaterEqual,

        /// <summary>
        /// Greater
        /// </summary>
        Greater,

        /// <summary>
        /// Less
        /// </summary>
        Less,

        /// <summary>
        /// Less equal
        /// </summary>
        LessEqual,

        /// <summary>
        /// Between
        /// </summary>
        Between
    }
}
