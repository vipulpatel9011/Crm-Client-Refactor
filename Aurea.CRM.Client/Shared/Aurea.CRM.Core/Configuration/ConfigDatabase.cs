// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigDatabase.cs" company="Aurea Software Gmbh">
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
//   Configuration database
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Configuration
{
    using System;
#if SqlitePCL
    using SQLite.Net;
    using SQLite.Net.Interop;
#else
    using SQLite;
#endif
    using Aurea.CRM.Core.DAL;
    using Aurea.CRM.Core.Logging;
    //using Microsoft.Practices.ServiceLocation;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// Configuration database
    /// </summary>
    public class ConfigDatabase : DatabaseBase
    {
        /// <summary>
        /// The lock object used when accesing the data base
        /// </summary>
        private static readonly object Locker = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigDatabase"/> class.
        /// </summary>
        /// <param name="databaseName">
        /// Name of the database.
        /// </param>
        private ConfigDatabase(string databaseName)
            : base(databaseName, false)
        {
        }

        /// <summary>
        /// Creates the specified file name.
        /// </summary>
        /// <param name="fileName">
        /// Name of the file.
        /// </param>
        /// <returns>
        /// The <see cref="ConfigDatabase"/>.
        /// </returns>
        public static ConfigDatabase Create(string fileName)
        {
            return new ConfigDatabase(fileName);
        }

        /// <summary>
        /// Creates the exists unit statement.
        /// </summary>
        /// <param name="tablename">
        /// The tablename.
        /// </param>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// database command
        /// </returns>
        public SQLiteCommand CreateExistsUnitStatement(string tablename, string unitName)
        {
            return this.CreateCommand($"SELECT unitName FROM {tablename} WHERE unitName = ?", unitName);
        }

        /// <summary>
        /// Creates the insert unit statement.
        /// </summary>
        /// <param name="tableName">
        /// Name of the table.
        /// </param>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <param name="definition">
        /// The definition.
        /// </param>
        /// <returns>
        /// insert sqlite command
        /// </returns>
        public SQLiteCommand CreateInsertUnitStatement(string tableName, string unitName, string definition)
        {
            return this.CreateCommand($"INSERT INTO {tableName} (unitDef, unitName) VALUES(?,?)", definition, unitName);
        }

        /// <summary>
        /// Creates the update unit statement.
        /// </summary>
        /// <param name="tablename">
        /// The tablename.
        /// </param>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <param name="definition">
        /// The definition.
        /// </param>
        /// <returns>
        /// database command
        /// </returns>
        public SQLiteCommand CreateUpdateUnitStatement(string tablename, string unitName, string definition)
        {
            return this.CreateCommand($"UPDATE {tablename} SET unitDef = ? WHERE unitName = ?", definition, unitName);
        }

        /// <summary>
        /// Ensures the existance of the database.
        /// </summary>
        /// <returns>
        /// true if exists;else false
        /// </returns>
        public override bool EnsureDDL()
        {
            if (this.ExistsTable("configuration"))
            {
                return true;
            }

            var result = this.ExecuteScalar<int>("CREATE TABLE configuration (version TEXT)");
            return result == 0;
        }

        /// <summary>
        /// Ensures the existance of a given table.
        /// </summary>
        /// <param name="tableName">
        /// Name of the table.
        /// </param>
        /// <returns>
        /// true if success;else false
        /// </returns>
        public bool EnsureTableDDL(string tableName)
        {
            if (this.ExistsTable(tableName))
            {
                return false;
            }

            var result = this.Execute($"CREATE TABLE {tableName} (unitName TEXT, unitDef TEXT)");
            return result == 0;
        }

        /// <summary>
        /// Writes the unit to table.
        /// </summary>
        /// <param name="tableName">
        /// Name of the table.
        /// </param>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <param name="definition">
        /// The definition.
        /// </param>
        /// <returns>
        /// 1 if error, else 0
        /// </returns>
        public int WriteUnitToTable(string tableName, string unitName, string definition)
        {
            try
            {
                var command = this.CreateInsertUnitStatement(tableName, unitName, definition);
                command.ExecuteNonQuery();
                return 0;
            }
            catch (Exception)
            {
                return 1;
            }
        }

        /// <summary>
        /// Initializes this database.
        /// typically used to create the database tables, etc...
        /// </summary>
        protected override void Init()
        {
            // Create the tables
            // CreateTable<SessionConfiguration>();
        }

        /// <summary>
        /// Provide the lock object.
        /// </summary>
        /// <returns>
        /// the lock object
        /// </returns>
        protected override object LockObject()
        {
            return Locker;
        }
    }
}
