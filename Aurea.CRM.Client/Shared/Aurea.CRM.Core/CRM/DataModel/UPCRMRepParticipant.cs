// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPCRMRep.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   Defines the Rep
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    using System.Collections.Generic;
    using System.Linq;

    using Extensions;

    /// <summary>
    /// Defines the Rep
    /// </summary>
    public class UPCRMRep
    {
        /// <summary>
        /// The rep children
        /// </summary>
        private Dictionary<string, UPCRMRep> repChildren;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMRep"/> class.
        /// </summary>
        /// <param name="repId">
        /// The rep identifier.
        /// </param>
        /// <param name="orgGroupId">
        /// The org group identifier.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <param name="repType">
        /// Type of the rep.
        /// </param>
        public UPCRMRep(string repId, string orgGroupId, string name, string recordIdentification, UPCRMRepType repType)
        {
            this.RepId = repId.RepIdString();
            this.RepOrgGroupId = orgGroupId.RepIdString();
            this.RepName = name;
            this.RecordIdentification = recordIdentification;
            this.RepType = repType;
        }

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get; private set; }

        /// <summary>
        /// Gets the rep children.
        /// </summary>
        /// <value>
        /// The rep children.
        /// </value>
        public List<UPCRMRep> RepChildren => this.repChildren?.Values?.ToList();

        /// <summary>
        /// Gets the rep identifier.
        /// </summary>
        /// <value>
        /// The rep identifier.
        /// </value>
        public string RepId { get; private set; }

        /// <summary>
        /// Gets the name of the rep.
        /// </summary>
        /// <value>
        /// The name of the rep.
        /// </value>
        public string RepName { get; private set; }

        /// <summary>
        /// Gets the rep org group identifier.
        /// </summary>
        /// <value>
        /// The rep org group identifier.
        /// </value>
        public string RepOrgGroupId { get; private set; }

        /// <summary>
        /// Gets the type of the rep.
        /// </summary>
        /// <value>
        /// The type of the rep.
        /// </value>
        public UPCRMRepType RepType { get; private set; }

        /// <summary>
        /// Adds the child rep.
        /// </summary>
        /// <param name="rep">
        /// The rep.
        /// </param>
        public void AddChildRep(UPCRMRep rep)
        {
            if (this.repChildren != null)
            {
                this.repChildren[rep.RepId] = rep;
            }
            else
            {
                this.repChildren = new Dictionary<string, UPCRMRep> { { rep.RepId, rep } };
            }
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"RepId: {this.RepId}; RepName: {this.RepName}";
        }
    }
}
