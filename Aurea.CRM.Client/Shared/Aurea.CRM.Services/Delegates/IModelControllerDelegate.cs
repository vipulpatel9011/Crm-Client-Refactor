// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelControllerDelegate.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   Delegate interface for a model controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.Delegates
{
    using System;
    using System.Collections.Generic;

    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.UIControlInterfaces;
    using Aurea.CRM.Services.ModelControllers;
    using Aurea.CRM.UIModel;

    /// <summary>
    /// Delegate interface for a model controller
    /// </summary>
    public interface IModelControllerDelegate: IModelControllerUIDelegate
    {
        /// <summary>
        /// The model controller did change.
        /// </summary>
        /// <param name="modelController">
        /// The model controller.
        /// </param>
        /// <param name="oldTopLevelElement">
        /// The old top level element.
        /// </param>
        /// <param name="newTopLevelElement">
        /// The new top level element.
        /// </param>
        /// <param name="changedIdentifiers">
        /// The changed identifiers.
        /// </param>
        /// <param name="changeHints">
        /// The change hints.
        /// </param>
        void ModelControllerDidChange(
            UPMModelController modelController,
            ITopLevelElement oldTopLevelElement,
            ITopLevelElement newTopLevelElement,
            List<IIdentifier> changedIdentifiers,
            UPChangeHints changeHints);

        /// <summary>
        /// The model controller did fail.
        /// </summary>
        /// <param name="modelController">
        /// The model controller.
        /// </param>
        /// <param name="failedTopLevelElement">
        /// The failed top level element.
        /// </param>
        void ModelControllerDidFail(UPMModelController modelController, ITopLevelElement failedTopLevelElement);

        /// <summary>
        /// The model controller did update.
        /// </summary>
        /// <param name="modelController">
        /// The model controller.
        /// </param>
        /// <param name="errors">
        /// The errors.
        /// </param>
        void ModelControllerDidUpdate(UPMModelController modelController, List<Exception> errors);
    }
}
