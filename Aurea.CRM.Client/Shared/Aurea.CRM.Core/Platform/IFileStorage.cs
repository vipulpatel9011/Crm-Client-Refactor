// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFileStorage.cs" company="Aurea Software Gmbh">
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
//   Defines the AuthenticationResponse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Platform
{
    /// <summary>
    /// defines the file storage
    /// </summary>
    public interface IFileStorage
    {
        /// <summary>
        /// Gets the default name of the folder.
        /// </summary>
        /// <value>
        /// The default name of the folder.
        /// </value>
        string DefaultFolderName { get; }

        /// <summary>
        /// Downgrades the file protection for resources.
        /// </summary>
        void DowngradeFileProtectionForResources();

        /// <summary>
        /// Upgrades the file protection for resources.
        /// </summary>
        void UpgradeFileProtectionForResources();

        /// <summary>
        /// Generates the file path for the given image name
        /// </summary>
        /// <param name="imageName">Image name</param>
        /// <returns>File path</returns>
        string ImagePathForName(string imageName);
    }
}
