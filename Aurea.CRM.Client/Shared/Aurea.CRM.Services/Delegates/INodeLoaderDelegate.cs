// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INodeLoaderDelegate.cs" company="Aurea Software Gmbh">
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
//   The INodeLoaderDelegate
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.Delegates
{
    using System;
    using Aurea.CRM.Services.ModelControllers.CircleOfInfluence;

    /// <summary>
    /// INodeLoaderDelegate
    /// </summary>
    public interface INodeLoaderDelegate
    {
        /// <summary>
        /// Nodes loaded succesfull.
        /// </summary>
        /// <param name="nodeloader">The nodeloader.</param>
        void NodesLoadedSuccesfull(CoIBaseNodeLoader nodeloader);

        /// <summary>
        /// Nodes load failed error.
        /// </summary>
        /// <param name="nodeloader">The nodeloader.</param>
        /// <param name="error">The error.</param>
        void NodesLoadFailedError(CoIBaseNodeLoader nodeloader, Exception error);
    }
}
