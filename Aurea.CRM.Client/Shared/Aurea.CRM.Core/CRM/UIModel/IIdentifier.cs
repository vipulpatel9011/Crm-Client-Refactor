// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIdentifier.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   Interface of control identifiers
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.CRM.UIModel
{
    /// <summary>
    /// Interface of control identifiers
    /// </summary>
    public interface IIdentifier
    {
        /// <summary>
        /// Gets a value indicating whether this instance has wildcard suffix.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has wildcard suffix; otherwise, <c>false</c>.
        /// </value>
        bool HasWildcardSuffix { get; }

        /// <summary>
        /// Gets the identifier as string.
        /// </summary>
        /// <value>
        /// The identifier as string.
        /// </value>
        string IdentifierAsString { get; }

        /// <summary>
        /// Matcheses the identifier.
        /// </summary>
        /// <param name="otherIdentifier">
        /// The other identifier.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool MatchesIdentifier(IIdentifier otherIdentifier);
    }
}
