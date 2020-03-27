// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigUnit.cs" company="Aurea Software Gmbh">
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
//   Interface for a configuration unit
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Configuration
{
    /// <summary>
    /// Interface for a configuration unit
    /// </summary>
    public interface IConfigUnit
    {
        /// <summary>
        /// Gets the name of the unit.
        /// </summary>
        /// <value>
        /// The name of the unit.
        /// </value>
        string UnitName { get; }
    }

    /// <summary>
    /// Base implementation for a configuratio unit
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Configuration.IConfigUnit" />
    public abstract class ConfigUnit : IConfigUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigUnit"/> class.
        /// </summary>
        protected ConfigUnit()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigUnit"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        protected ConfigUnit(object definition)
        {
        }

        /// <summary>
        /// Gets the name of the unit.
        /// </summary>
        /// <value>
        /// The name of the unit.
        /// </value>
        public string UnitName { get; protected set; }
    }
}
