// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMCoIEdge.cs" company="Aurea Software Gmbh">
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
//   The COI Edge.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.CircleOfInfluence
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;

    /// <summary>
    /// COI Edge
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.UPMElement" />
    public class UPMCoIEdge : UPMElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMElement"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMCoIEdge(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets the name of the relation.
        /// </summary>
        /// <value>
        /// The name of the relation.
        /// </value>
        public UPMStringField RelationName { get; set; }

        /// <summary>
        /// Gets or sets the relation intensity.
        /// </summary>
        /// <value>
        /// The relation intensity.
        /// </value>
        public int RelationIntensity { get; set; }

        /// <summary>
        /// Gets or sets the list fields.
        /// </summary>
        /// <value>
        /// The list fields.
        /// </value>
        public List<UPMStringField> ListFields { get; set; }

        /// <summary>
        /// Gets or sets the target node identifier.
        /// </summary>
        /// <value>
        /// The target node identifier.
        /// </value>
        public IIdentifier TargetNodeIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the source node identifier.
        /// </summary>
        /// <value>
        /// The source node identifier.
        /// </value>
        public IIdentifier SrcNodeIdentifier { get; set; }
    }
}
