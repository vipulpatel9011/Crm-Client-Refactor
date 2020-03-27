// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPInboxFileManager.cs" company="Aurea Software Gmbh">
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
//   The InBox File Manager
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Inbox
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Messages;
    using GalaSoft.MvvmLight.Messaging;

    /// <summary>
    /// Inbox File Manager
    /// </summary>
    public class UPInboxFileManager
    {
        private static UPInboxFileManager defaultManager;

        /// <summary>
        /// Gets or sets the real inbox path.
        /// </summary>
        /// <value>
        /// The real inbox path.
        /// </value>
        public string RealInboxPath { get; set; }

        /// <summary>
        /// Gets or sets the inbox path.
        /// </summary>
        /// <value>
        /// The inbox path.
        /// </value>
        public string InboxPath { get; set; }

        /// <summary>
        /// Currents the inbox file manager.
        /// </summary>
        /// <returns></returns>
        public static UPInboxFileManager CurrentInboxFileManager()
        {
            if (defaultManager == null)
            {
                defaultManager = new UPInboxFileManager();
#if PORTING
                defaultManager.InboxPath = UPPathService.DocumentPath().StringByAppendingPathComponent("writeableInbox");
                defaultManager.RealInboxPath = UPPathService.DocumentPath().StringByAppendingPathComponent("Inbox");
                if (!NSFileManager.DefaultManager().FileExistsAtPath(defaultManager.InboxPath))
                {
                    NSError error = null;
                    NSFileManager.DefaultManager().CreateDirectoryAtPathWithIntermediateDirectoriesAttributesError(defaultManager.InboxPath, false, null, error);
                }
#endif
            }

            return defaultManager;
        }

        /// <summary>
        /// Moves from URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public Uri MoveFromUrl(Uri url)
        {
            return null;
#if PORTING
            string oldPath = url.Path;
            string newName = oldPath.LastPathComponent.StringByDeletingPathExtension();
            string pathExtension = oldPath.LastPathComponent.PathExtension;
            string newPath = this.InboxPath.StringByAppendingPathComponent(oldPath.LastPathComponent);
            Exception error = null;

            if (!NSFileManager.DefaultManager().FileExistsAtPath(newPath))
            {
                NSFileManager.DefaultManager().MoveItemAtPathToPathError(oldPath, newPath, error);
            }
            else
            {
                bool moved = false;
                for (int i = 1; (!moved && i < 100); i++)
                {
                    string newName1 = $"{newName}-{i}.{pathExtension}";
                    newPath = this.InboxPath.StringByAppendingPathComponent(newName1);
                    if (!NSFileManager.DefaultManager().FileExistsAtPath(newPath))
                    {
                        moved = NSFileManager.DefaultManager().MoveItemAtPathToPathError(oldPath, newPath, error);
                    }
                }
            }

            return NSURL.FileURLWithPath(newPath);
#endif
        }

        /// <summary>
        /// Alls the inbox files.
        /// </summary>
        /// <returns></returns>
        public List<UPInboxFile> AllInboxFiles()
        {
            List<UPInboxFile> files = new List<UPInboxFile>();
#if PORTING
            Exception error;
            List<string> pathNames = NSFileManager.DefaultManager().ContentsOfDirectoryAtPathError(this.InboxPath, error);
            if (error)
            {
                if (error.Code() == NSFileReadNoSuchFileError)
                {
                    DDLogInfo("Inbox directory %@ does not exist.", this.InboxPath);
                }
                else
                {
                    DDLogError("Could not load files from inbox: %@", error);
                }
            }

            foreach (string path in pathNames)
            {
                UPInboxFile file = new UPInboxFile(this.InboxPath.StringByAppendingPathComponent(path), NSURL.FileURLWithPathIsDirectory(this.InboxPath.StringByAppendingPathComponent(path), false));
                files.Add(file);
            }
#endif
            return files;
        }

        /// <summary>
        /// Inboxes the file from URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public static UPInboxFile InboxFileFromURL(Uri url)
        {
            return null;
#if PORTING
            UPInboxFile file = new UPInboxFile(url.Path(), url);
            return file;
#endif
        }

        /// <summary>
        /// Deletes the file with URL.
        /// </summary>
        /// <param name="inboxFileURL">The inbox file URL.</param>
        public static void DeleteFileWithURL(Uri inboxFileURL)
        {
#if PORTING
            Exception deleteError;
            NSFileManager.DefaultManager().RemoveItemAtURLError(inboxFileURL, deleteError);
            if (deleteError == null)
            {
                DDLogError("Error deleting inbox file %@", deleteError);
            }
            else
            {
                Messenger.Default.Send(InboxFileManagerMessage.FileRemovedFromInbox(inboxFileURL.ToString()));
            }
#endif
        }

        /// <summary>
        /// Files the added to inbox with URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        public static void FileAddedToInboxWithURL(Uri url)
        {
            Messenger.Default.Send(InboxFileManagerMessage.FileAddedToInbox(url.ToString()));
        }

        /// <summary>
        /// MIME type for path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static string MimeTypeForPath(string path)
        {
            return null;
#if PORTING
            // Get the UTI from the file's extension:
            string pathExtension = path.PathExtension();
            string type = UTTypeCreatePreferredIdentifierForTag(kUTTagClassFilenameExtension, pathExtension, NULL);

            // The UTI can be converted to a mime type:
            string mimeType = UTTypeCopyPreferredTagWithClass(type, kUTTagClassMIMEType);

            if (!mimeType)
            {
                return "application/octet-stream";
            }
            else
            {
                return mimeType;
            }
#endif
        }
    }
}
