// <copyright file="UPMGridColumnHeaderStringField.cs" company="Aurea Software Gmbh">
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

namespace Aurea.CRM.UIModel.Pages
{
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;

    /// <summary>
    /// Implementation of grid column header string field
    /// </summary>
    public class UPMGridColumnHeaderStringField : UPMStringField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMGridColumnHeaderStringField"/> class.
        /// </summary>
        /// <param name="identifier">Identifier</param>
        public UPMGridColumnHeaderStringField(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether is sub field
        /// </summary>
        public bool IsSubField { get; set; }

        /// <summary>
        /// Gets or sets number of child columns
        /// </summary>
        public int NumberOfChildColumns { get; set; }
    }
}
