// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmResultFieldCondition.cs" company="Aurea Software Gmbh">
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
    /// CRM result field condition
    /// </summary>
    /// <seealso cref="UPCRMResultCondition" />
    public class UPCRMResultFieldCondition : UPCRMResultCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMResultFieldCondition"/> class.
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <param name="fieldValue">
        /// The field value.
        /// </param>
        /// <param name="resultPosition">
        /// The result position.
        /// </param>
        public UPCRMResultFieldCondition(
            UPCRMField field,
            UPConditionOperator condition,
            string fieldValue,
            int resultPosition)
        {
            this.Field = field;
            this.ResultPosition = resultPosition;
            this.FieldValue = fieldValue;
            this.Condition = condition;
        }

        /// <summary>
        /// Gets the condition.
        /// </summary>
        /// <value>
        /// The condition.
        /// </value>
        public UPConditionOperator Condition { get; private set; }

        /// <summary>
        /// Gets the field.
        /// </summary>
        /// <value>
        /// The field.
        /// </value>
        public UPCRMField Field { get; private set; }

        /// <summary>
        /// Gets the field value.
        /// </summary>
        /// <value>
        /// The field value.
        /// </value>
        public string FieldValue { get; private set; }

        /// <summary>
        /// Gets the result position.
        /// </summary>
        /// <value>
        /// The result position.
        /// </value>
        public int ResultPosition { get; private set; }

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
            return this.Field.CheckValueMatchesValueCondition(
                row.RawValueAtIndex(this.ResultPosition),
                this.FieldValue,
                this.Condition);
        }
    }
}
