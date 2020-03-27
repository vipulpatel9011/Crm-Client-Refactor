// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStorageProvider.cs" company="Aurea Software Gmbh">
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
//   Expose the platform specific local storage system
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Platform
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Expose the platform specific local storage system
    /// </summary>
    public interface IStorageProvider
    {
        /// <summary>
        /// Gets the name of the bundle.
        /// </summary>
        /// <value>
        /// The name of the bundle.
        /// </value>
        string BundleName { get; }

        /// <summary>
        /// Gets the caches folder path.
        /// </summary>
        /// <value>
        /// The caches folder path.
        /// </value>
        string CachesFolderPath { get; }

        /// <summary>
        /// Gets the documents folder path.
        /// </summary>
        /// <value>
        /// The documents folder path.
        /// </value>
        string DocumentsFolderPath { get; }

        /// <summary>
        /// Appends the text asynchronous to the given file.
        /// </summary>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="maxSize">
        /// The maximum size (lines) of the file.
        /// </param>
        /// <param name="appendLimit">
        /// The append limit (how many lines should keep after pruning).
        /// </param>
        void AppendTextAsync(string filePath, string message, int maxSize = 500, int appendLimit = 200);

        /// <summary>
        /// Gets the config data
        /// </summary>
        /// <value>
        /// The config data
        /// </value>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<string> GetConfigdataAsync();

        /// <summary>
        /// Creates the directory at given path.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        void CreateDirectory(string path);

        /// <summary>
        /// Check if the gven folder exists.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// true if exists;else false
        /// </returns>
        bool DirectoryExists(string path);

        /// <summary>
        /// Contentses the of directory.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>List of files in the folder</returns>
        List<string> ContentsOfDirectory(string path);

        /// <summary>
        /// Gets the list of directories in the directory.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>List of subdirectories</returns>
        IList<string> GetSubDirectoryNames(string path);

        /// <summary>
        /// Gets the contents of a given file.
        /// </summary>
        /// <param name="fileName">
        /// Name of the file.
        /// </param>
        /// <returns>
        /// the contents of a file
        /// </returns>
        Task<byte[]> FileContents(string fileName);

        /// <summary>
        /// Check if the gven file exists.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// true if exists;else false
        /// </returns>
        bool FileExists(string path);

        /// <summary>
        /// Exposes the file storage at given base path.
        /// </summary>
        /// <param name="basePath">
        /// The base path.
        /// </param>
        /// <returns>
        /// an instance of the file storage
        /// </returns>
        IFileStorage FileStoreAtPath(string basePath);

        /// <summary>
        /// Loads the object.
        /// </summary>
        /// <typeparam name="TInstance">
        /// The type of the instance.
        /// </typeparam>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        /// <returns>
        /// async task
        /// </returns>
        Task<TInstance> LoadObject<TInstance>(string filePath);

        /// <summary>
        /// Saves the given contents as the given file.
        /// </summary>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        /// <param name="contents">
        /// The contents.
        /// </param>
        /// <returns>
        /// true if success;else false
        /// </returns>
        Task<bool> SaveFile(string filePath, byte[] contents);

        /// <summary>
        /// Saves the object.
        /// </summary>
        /// <typeparam name="TInstance">
        /// The type of the instance.
        /// </typeparam>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        /// <returns>
        /// async result
        /// </returns>
        Task SaveObject<TInstance>(TInstance instance, string filePath);

        /// <summary>
        /// Returns file stream
        /// </summary>
        /// <param name="filePath">File Path</param>
        /// <returns>Stream instance</returns>
        Task<bool> CreateFile(string filePath);

        /// <summary>
        /// Creates the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="data">The data.</param>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        /// <returns>True is success, else false</returns>
        Task<bool> CreateFile(string filePath, byte[] data, bool overwrite = true);

        /// <summary>
        /// Returns file stream for reading
        /// </summary>
        /// <param name="filePath">File Path</param>
        /// <returns>Stream instance</returns>
        Task<Stream> GetFileStreamForRead(string filePath);

        /// <summary>
        /// Returns file stream for writing
        /// </summary>
        /// <param name="filePath">File Path</param>
        /// <returns>Stream instance</returns>
        Task<Stream> GetFileStreamForWrite(string filePath);

        /// <summary>
        /// Returns files mime type
        /// </summary>
        /// <param name="filePath">File Path</param>
        /// <returns>Mime Type</returns>
        Task<string> GetFileContentType(string filePath);

        /// <summary>
        /// Returns files size
        /// </summary>
        /// <param name="filePath">File Path</param>
        /// <returns>File size <see cref="ulong"/></returns>
        Task<ulong> GetFileSize(string filePath);

        /// <summary>
        /// Returns files created date
        /// </summary>
        /// <param name="filePath">File Path</param>
        /// <returns>Mime Type</returns>
        Task<DateTime> GetFileCreatedDate(string filePath);

        /// <summary>
        /// Returns files last modified date
        /// </summary>
        /// <param name="filePath">File Path</param>
        /// <returns>Mime Type</returns>
        Task<DateTime> GetFileLastModified(string filePath);

        /// <summary>
        /// Tries to copy a file to a given destination.
        /// </summary>
        /// <param name="from">
        /// From.
        /// </param>
        /// <param name="to">
        /// To.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// true if success;else false
        /// </returns>
        bool TryCopy(string from, string to, out Exception error);

        /// <summary>
        /// Tries the delete of a file.
        /// </summary>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// true if success;else false
        /// </returns>
        bool TryDelete(string filePath, out Exception error);

        /// <summary>
        /// Tries to move a file to a given destination.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="dest">
        /// The dest.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// true if success;else false
        /// </returns>
        bool TryMove(string source, string dest, out Exception error);
    }
}
