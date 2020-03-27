// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmFieldSetterSourceLink.cs" company="Aurea Software Gmbh">
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
//   Field Setter Source Link
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    /// <summary>
    /// Field Setter Source Link
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.DataModel.UPCRMFieldSetterField" />
    public class UPCRMFieldSetterSourceLink : UPCRMFieldSetterField
    {
        /// <summary>
        /// Gets the target information area identifier.
        /// </summary>
        /// <value>
        /// The target information area identifier.
        /// </value>
        public string TargetInfoAreaId { get; private set; }

        /// <summary>
        /// Gets the link identifier.
        /// </summary>
        /// <value>
        /// The link identifier.
        /// </value>
        public int LinkId { get; private set; }

        /// <summary>
        /// Gets the index of the field.
        /// </summary>
        /// <value>
        /// The index of the field.
        /// </value>
        public int FieldIndex { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMFieldSetterSourceLink"/> class.
        /// </summary>
        /// <param name="targetInfoAreaId">The target information area identifier.</param>
        /// <param name="linkId">The link identifier.</param>
        /// <param name="fieldIndex">Index of the field.</param>
        public UPCRMFieldSetterSourceLink(string targetInfoAreaId, int linkId, int fieldIndex)
        {
            this.TargetInfoAreaId = targetInfoAreaId;
            this.LinkId = linkId;
            this.FieldIndex = fieldIndex;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is link.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is link; otherwise, <c>false</c>.
        /// </value>
        public override bool IsLink => true;

        /// <summary>
        /// Fields the with information area identifier.
        /// </summary>
        /// <param name="infoAreaId">The information area identifier.</param>
        /// <returns></returns>
        public override UPCRMField FieldWithInfoAreaId(string infoAreaId)
        {
            return new UPCRMLinkField(this.TargetInfoAreaId, this.LinkId, infoAreaId);
        }
    }
}
