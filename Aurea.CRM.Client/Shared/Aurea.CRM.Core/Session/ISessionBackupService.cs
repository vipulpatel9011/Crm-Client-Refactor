// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerSessionSyncHandler.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//   Jakub Malczak
// </author>
// <summary>
//   Session backup service interface
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Session
{
    /// <summary>
    /// Interface for backing up / restoring state of current session
    /// </summary>
    public interface ISessionBackupService
    {
        /// <summary>
        /// Backup database
        /// </summary>
        /// <param name="databaseFileName">database file name</param>
        void BackupDatabase(string databaseFileName);

        /// <summary>
        /// Restore database
        /// </summary>
        /// <param name="databaseFileName">database file name</param>
        void RestoreDatabase(string databaseFileName);

        /// <summary>
        /// Backup resources
        /// </summary>
        /// <param name="resourcesFolderName">resource folder name</param>
        void BackupResources(string resourcesFolderName);

        /// <summary>
        /// Restore resources
        /// </summary>
        /// <param name="resourcesFolderName">resource folder name</param>
        void RestoreResources(string resourcesFolderName);

        /// <summary>
        /// Remove backup folder and all files inside
        /// </summary>
        void RemoveBackupFolder();
    }
}
