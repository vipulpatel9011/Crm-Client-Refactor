// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOperationManagerDelegate.cs" company="Aurea Software Gmbh">
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
//   Result of a password change request
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.OperationHandling
{
    using System;
    using System.Collections.Generic;

    using Aurea.CRM.Core.Session;

    /// <summary>
    /// Result of a password change request
    /// </summary>
    public enum PasswordChangeResult
    {
        /// <summary>
        /// The no password change requested.
        /// </summary>
        NoPasswordChangeRequested = 0,

        /// <summary>
        /// The password change not supported.
        /// </summary>
        PasswordChangeNotSupported,

        /// <summary>
        /// The password changed.
        /// </summary>
        PasswordChanged,

        /// <summary>
        /// The password not changed.
        /// </summary>
        PasswordNotChanged
    }

    /// <summary>
    /// Operation manager delegate interface
    /// </summary>
    public interface IOperationManagerDelegate
    {
        /// <summary>
        /// Servers the operation manager did fail server login.
        /// </summary>
        /// <param name="serverOperationManager">
        /// The server operation manager.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="passwordChanged">
        /// The password changed.
        /// </param>
        void ServerOperationManagerDidFailServerLogin(
            ServerOperationManager serverOperationManager,
            Exception error,
            PasswordChangeResult passwordChanged);

        /// <summary>
        /// Servers the operation manager did fail with internet connection error.
        /// </summary>
        /// <param name="serverOperationManager">
        /// The server operation manager.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="passwordChanged">
        /// The password changed.
        /// </param>
        void ServerOperationManagerDidFailWithInternetConnectionError(
            ServerOperationManager serverOperationManager,
            Exception error,
            PasswordChangeResult passwordChanged);

        /// <summary>
        /// Servers the operation manager did perform server login.
        /// </summary>
        /// <param name="serverOperationManager">
        /// The server operation manager.
        /// </param>
        /// <param name="availableLanguages">
        /// The available languages.
        /// </param>
        /// <param name="sessionAttributes">
        /// The session attributes.
        /// </param>
        /// <param name="serverInfo">
        /// The server information.
        /// </param>
        /// <param name="passwordChanged">
        /// The password changed.
        /// </param>
        void ServerOperationManagerDidPerformServerLogin(
            ServerOperationManager serverOperationManager,
            List<ServerLanguage> availableLanguages,
            Dictionary<string, object> sessionAttributes,
            List<object> serverInfo,
            PasswordChangeResult passwordChanged);

        /// <summary>
        /// Servers the operation manager password change requested.
        /// </summary>
        /// <param name="serverOperationManager">
        /// The server operation manager.
        /// </param>
        /// <param name="isRequested">
        /// if set to <c>true</c> [is requested].
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool ServerOperationManagerPasswordChangeRequested(
            ServerOperationManager serverOperationManager,
            bool isRequested);

        /// <summary>
        /// Servers the operation manager requires language for session.
        /// </summary>
        /// <param name="serverOperationManager">
        /// The server operation manager.
        /// </param>
        /// <param name="availableLanguages">
        /// The available languages.
        /// </param>
        /// <param name="sessionAttributes">
        /// The session attributes.
        /// </param>
        /// <param name="serverInfo">
        /// The server information.
        /// </param>
        /// <param name="passwordChanged">
        /// The password changed.
        /// </param>
        void ServerOperationManagerRequiresLanguageForSession(
            ServerOperationManager serverOperationManager,
            List<ServerLanguage> availableLanguages,
            Dictionary<string, object> sessionAttributes,
            List<object> serverInfo,
            PasswordChangeResult passwordChanged);
    }
}
