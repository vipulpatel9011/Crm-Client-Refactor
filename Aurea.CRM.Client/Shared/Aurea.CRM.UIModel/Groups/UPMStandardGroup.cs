// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MStandardGroup.cs" company="Aurea Software Gmbh">
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
//   Implements the standard group UI control
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Groups
{
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Structs;

    /// <summary>
    /// Implements the standard group UI control
    /// </summary>
    /// <seealso cref="UPMGroup" />
    public class UPMStandardGroup : UPMGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMStandardGroup"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMStandardGroup(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether [column style].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [column style]; otherwise, <c>false</c>.
        /// </value>
        public bool ColumnStyle { get; set; }

        /// <summary>
        /// Gets or sets the image document.
        /// </summary>
        /// <value>
        /// The image document.
        /// </value>
        public UPMDocument ImageDocument { get; set; }

        /// <summary>
        /// Gets or sets the maximum size of the image document.
        /// </summary>
        /// <value>
        /// The maximum size of the image document.
        /// </value>
        public Size ImageDocumentMaxSize { get; set; }
    }
}
