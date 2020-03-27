// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUnitTypeStore.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Jakub Majewski
// </author>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Configuration
{
    using System.Collections.Generic;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Single configuration unit storage
    /// </summary>
    public interface IUnitTypeStore
    {
        /// <summary>
        /// Gets the configuration store.
        /// </summary>
        /// <value>
        /// The configuration store.
        /// </value>
        ConfigurationUnitStore ConfigStore { get; }

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <value>
        /// The database.
        /// </value>
        ConfigDatabase Database { get; }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <value>
        /// The name of the table.
        /// </value>
        string TableName { get; }

        /// <summary>
        /// Gets the type of the unit.
        /// </summary>
        /// <value>
        /// The type of the unit.
        /// </value>
        string UnitType { get; }

        /// <summary>
        /// Alls the unit names.
        /// </summary>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        List<string> AllUnitNames();

        /// <summary>
        /// Alls the unit names sorted.
        /// </summary>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        List<string> AllUnitNamesSorted();

        /// <summary>
        /// Alls the units.
        /// </summary>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        List<ConfigUnit> AllUnits();

        /// <summary>
        /// Loads this instance.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool Load();

        /// <summary>
        /// Loads the table.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool LoadTable();

        /// <summary>
        /// Loads the unit.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        object LoadUnit(string unitName);

        /// <summary>
        /// Resets this instance.
        /// </summary>
        void Reset();

        /// <summary>
        /// Synchronizes the elements.
        /// </summary>
        /// <param name="elements">
        /// The elements.
        /// </param>
        /// <param name="emptyTable">
        /// if set to <c>true</c> [empty table].
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        int SyncElements(JArray elements, bool emptyTable);

        /// <summary>
        /// To the json.
        /// </summary>
        /// <param name="theObject">
        /// The object.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string ToJSON(object theObject);

        /// <summary>
        /// Units the name of the with.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// The <see cref="ConfigUnit"/>.
        /// </returns>
        ConfigUnit UnitWithName(string unitName);
    }
}