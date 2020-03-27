// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmFieldSetterField.cs" company="Aurea Software Gmbh">
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
//   The CRM Field Setter Field class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    /// <summary>
    /// Field Setter Field
    /// </summary>
    public class UPCRMFieldSetterField
    {
        /// <summary>
        /// Gets or sets the result position.
        /// </summary>
        /// <value>
        /// The result position.
        /// </value>
        public int ResultPosition { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is link.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is link; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsLink => false;

        /// <summary>
        /// Gets a value indicating whether this instance is field.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is field; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsField => false;

        /// <summary>
        /// Fields the with information area identifier.
        /// </summary>
        /// <param name="infoAreaId">The information area identifier.</param>
        /// <returns></returns>
        public virtual UPCRMField FieldWithInfoAreaId(string infoAreaId)
        {
            return UPCRMField.EmptyFieldWithInfoArea(infoAreaId);
        }
    }
}
