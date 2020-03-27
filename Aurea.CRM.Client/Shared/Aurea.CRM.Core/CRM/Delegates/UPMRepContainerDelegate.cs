// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMRepContainerDelegate.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   Rep Container Delegate
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.CRM.Delegates
{
    using System.Collections.Generic;

    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    /// Rep Container Delegate
    /// </summary>
    public interface UPMRepContainerDelegate
    {
        /// <summary>
        /// Notifies the delegate some rep keys has been selected.
        /// </summary>
        /// <param name="repContainer">
        /// The RepContainer
        /// </param>
        /// <param name="selectedRepkeys">
        /// Selected Rep Keys
        /// </param>
        void DidSelectRepKeys(IUPMRepContainer repContainer, List<string> selectedRepkeys);
    }
}
