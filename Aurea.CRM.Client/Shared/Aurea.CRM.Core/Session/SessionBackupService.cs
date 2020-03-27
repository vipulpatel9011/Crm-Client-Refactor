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
//   Session backup service
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Session
{
    using System;
    using System.IO;
    using Aurea.CRM.Core.Platform;

    /// <summary>
    /// Session backup service
    /// </summary>
    public class SessionBackupService : ISessionBackupService
    {
        private readonly string sourceFolderPath;
        private readonly IStorageProvider storageProvider;
        private readonly string backupFolderPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionBackupService"/> class.
        /// </summary>
        /// <param name="sourceFolderPath">Source path for backups</param>
        /// <param name="backupFolderPath">Backup folder path</param>
        /// <param name="storageProvider">Storage provider</param>
        public SessionBackupService(string sourceFolderPath, string backupFolderPath, IStorageProvider storageProvider)
        {
            this.backupFolderPath = backupFolderPath;
            this.storageProvider = storageProvider;
            this.sourceFolderPath = sourceFolderPath;
        }

        /// <summary>
        /// Backup database
        /// </summary>
        /// <param name="databaseFileName">database file name</param>
        public void BackupDatabase(string databaseFileName)
        {
            this.CreateBackupFolderIfDoesntExist();

            Exception copyException = null;

            this.storageProvider.TryCopy(Path.Combine(this.sourceFolderPath, databaseFileName), Path.Combine(this.backupFolderPath, databaseFileName), out copyException);

            if (copyException != null)
            {
                throw copyException;
            }
        }

        /// <summary>
        /// Restore database
        /// </summary>
        /// <param name="databaseFileName">database file name</param>
        public void RestoreDatabase(string databaseFileName)
        {
            Exception copyException = null;
            this.storageProvider.TryCopy(Path.Combine(this.backupFolderPath, databaseFileName), Path.Combine(this.sourceFolderPath, databaseFileName), out copyException);

            if (copyException != null)
            {
                throw copyException;
            }
        }

        /// <summary>
        /// Backup resources located in folder
        /// </summary>
        /// <param name="resourcesFolderName">resource folder name</param>
        public void BackupResources(string resourcesFolderName)
        {
            this.CreateBackupFolderIfDoesntExist();

            var resourceFilesFolderPath = Path.Combine(this.sourceFolderPath, resourcesFolderName);
            var resourceFiles = this.storageProvider.ContentsOfDirectory(resourceFilesFolderPath);
            var resourcesBackupFolderPath = Path.Combine(this.backupFolderPath, resourcesFolderName);

            if (!this.storageProvider.DirectoryExists(resourcesBackupFolderPath))
            {
                this.storageProvider.CreateDirectory(resourcesBackupFolderPath);
            }

            foreach (var resourceFile in resourceFiles)
            {
                Exception resourceFileCopyException = null;
                this.storageProvider.TryCopy(Path.Combine(resourceFilesFolderPath, resourceFile), Path.Combine(resourcesBackupFolderPath, resourceFile), out resourceFileCopyException);

                if (resourceFileCopyException != null)
                {
                    throw resourceFileCopyException;
                }
            }
        }

        /// <summary>
        /// Restore resources located in folder
        /// </summary>
        /// <param name="resourcesFolderName">resource folder name</param>
        public void RestoreResources(string resourcesFolderName)
        {
            var resourceFilesFolderPath = Path.Combine(this.sourceFolderPath, resourcesFolderName);
            var resourcesBackupFolderPath = Path.Combine(this.backupFolderPath, resourcesFolderName);
            var resourceFiles = this.storageProvider.ContentsOfDirectory(resourcesBackupFolderPath);

            if (!this.storageProvider.DirectoryExists(resourceFilesFolderPath))
            {
                this.storageProvider.CreateDirectory(resourceFilesFolderPath);
            }

            // Remove existing resource files before restore so we won't have any leftover files
            var existingResourceFilesToRemove = this.storageProvider.ContentsOfDirectory(resourceFilesFolderPath);

            foreach (var existingResourceFileToRemove in existingResourceFilesToRemove)
            {
                Exception resourceFileRemoveException = null;
                this.storageProvider.TryDelete(Path.Combine(resourceFilesFolderPath, existingResourceFileToRemove), out resourceFileRemoveException);

                if (resourceFileRemoveException != null)
                {
                    throw resourceFileRemoveException;
                }
            }

            // Restore resources by copying back all files from backup
            foreach (var resourceFile in resourceFiles)
            {
                Exception resourceFileCopyException = null;
                this.storageProvider.TryCopy(Path.Combine(resourcesBackupFolderPath, resourceFile), Path.Combine(resourceFilesFolderPath, resourceFile), out resourceFileCopyException);

                if (resourceFileCopyException != null)
                {
                    throw resourceFileCopyException;
                }
            }
        }

        /// <summary>
        /// Remove backup folder and all files insid3
        /// </summary>
        public void RemoveBackupFolder()
        {
            Exception removeBackupFolderException = null;
            this.storageProvider.TryDelete(this.backupFolderPath, out removeBackupFolderException);

            if (removeBackupFolderException != null)
            {
                throw removeBackupFolderException;
            }
        }

        private void CreateBackupFolderIfDoesntExist()
        {
            if (!this.storageProvider.DirectoryExists(this.backupFolderPath))
            {
                this.storageProvider.CreateDirectory(this.backupFolderPath);
            }
        }
    }
}
