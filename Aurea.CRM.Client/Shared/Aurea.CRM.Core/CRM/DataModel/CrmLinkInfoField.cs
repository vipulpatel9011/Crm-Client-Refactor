// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmLinkInfoField.cs" company="Aurea Software Gmbh">
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
//   Link Info Field
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    /// <summary>
    /// Link field infomation field
    /// </summary>
    public class UPCRMLinkInfoField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMLinkInfoField"/> class.
        /// </summary>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <param name="targetFieldId">
        /// The target field identifier.
        /// </param>
        /// <param name="sourceValue">
        /// The source value.
        /// </param>
        /// <param name="targetValue">
        /// The target value.
        /// </param>
        public UPCRMLinkInfoField(int fieldId, int targetFieldId, string sourceValue, string targetValue)
        {
            this.FieldId = fieldId;
            this.TargetFieldId = targetFieldId;
            this.SourceValue = sourceValue;
            this.TargetValue = targetValue;
        }

        /// <summary>
        /// Gets the field identifier.
        /// </summary>
        /// <value>
        /// The field identifier.
        /// </value>
        public int FieldId { get; private set; }

        /// <summary>
        /// Gets the source value.
        /// </summary>
        /// <value>
        /// The source value.
        /// </value>
        public string SourceValue { get; private set; }

        /// <summary>
        /// Gets the target field identifier.
        /// </summary>
        /// <value>
        /// The target field identifier.
        /// </value>
        public int TargetFieldId { get; private set; }

        /// <summary>
        /// Gets the target value.
        /// </summary>
        /// <value>
        /// The target value.
        /// </value>
        public string TargetValue { get; private set; }
    }
}
