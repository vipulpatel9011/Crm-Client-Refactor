// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContainer.cs" company="Aurea Software Gmbh">
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
//   An interface to define container registration functionality
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Platform
{
    /// <summary>
    /// An interface to define container registration functionality
    /// </summary>
    public interface IContainer
    {
        /// <summary>
        /// Registers the specified instance.
        /// </summary>
        /// <typeparam name="TType">
        /// The type of the type.
        /// </typeparam>
        /// <param name="instance">
        /// The instance.
        /// </param>
        void Register<TType>(TType instance) where TType : class;

        /// <summary>
        /// Registers the specified instance.
        /// </summary>
        /// <typeparam name="TType">
        /// The type of the type.
        /// </typeparam>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        void Register<TType>(TType instance, string key) where TType : class;
    }
}
