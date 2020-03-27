// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAlternateExpandCheckerDelegate.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The AlternateExpandCheckerDelegate interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.CRM.Delegates
{
    using System;

    using Aurea.CRM.Core.Configuration;

    /// <summary>
    /// The AlternateExpandCheckerDelegate interface.
    /// </summary>
    public interface IAlternateExpandCheckerDelegate
    {
        /// <summary>
        /// The alternate expand checker did fail with error.
        /// </summary>
        /// <param name="expandChecker">
        /// The expand checker.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        void AlternateExpandCheckerDidFailWithError(UPCRMAlternateExpandChecker expandChecker, Exception error);

        /// <summary>
        /// The alternate expand checker did finish with result.
        /// </summary>
        /// <param name="expandChecker">
        /// The expand checker.
        /// </param>
        /// <param name="expand">
        /// The expand.
        /// </param>
        void AlternateExpandCheckerDidFinishWithResult(UPCRMAlternateExpandChecker expandChecker, UPConfigExpand expand);
    }
}
