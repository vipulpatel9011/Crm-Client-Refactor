// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmFieldSetterSourceField.cs" company="Aurea Software Gmbh">
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
//   The CRM Field Setter Source Field class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    /// <summary>
    /// Field Setter Source Field
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.DataModel.UPCRMFieldSetterField" />
    public class UPCRMFieldSetterSourceField : UPCRMFieldSetterField
    {
        /// <summary>
        /// Gets or sets the field identifier.
        /// </summary>
        /// <value>
        /// The field identifier.
        /// </value>
        public int FieldId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMFieldSetterSourceField"/> class.
        /// </summary>
        /// <param name="fieldId">The field identifier.</param>
        public UPCRMFieldSetterSourceField(int fieldId)
        {
            this.FieldId = fieldId;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is field.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is field; otherwise, <c>false</c>.
        /// </value>
        public override bool IsField => true;

        /// <summary>
        /// Fields the with information area identifier.
        /// </summary>
        /// <param name="infoAreaId">The information area identifier.</param>
        /// <returns></returns>
        public override UPCRMField FieldWithInfoAreaId(string infoAreaId)
        {
            return new UPCRMField(this.FieldId, infoAreaId);
        }
    }
}
