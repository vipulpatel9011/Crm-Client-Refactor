// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITopLevelElement.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   Defines the interface for a top level element
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.UIControlInterfaces
{
    using System.Collections.Generic;

    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    /// Defines the interface for a top level element
    /// </summary>
    public interface ITopLevelElement
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="ITopLevelElement"/> is invalid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if invalid; otherwise, <c>false</c>.
        /// </value>
        bool Invalid { get; }

        /// <summary>
        /// Processes the changes applied identifiers.
        /// </summary>
        /// <param name="listOfIdentifiers">
        /// The list of identifiers.
        /// </param>
        /// <param name="appliedIdentifiers">
        /// The applied identifiers.
        /// </param>
        void ProcessChangesAppliedIdentifiers(List<IIdentifier> listOfIdentifiers, List<IIdentifier> appliedIdentifiers);
    }
}
