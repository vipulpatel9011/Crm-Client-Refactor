// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseBase.cs" company="Aurea Software Gmbh">
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
//   Provide access to SQLite base data base
// </summary>
// --------------------------------------------------------------------------------------------------------------------
#if SqlitePCL
using SQLite.Net;
using SQLite.Net.Interop;
#else
using SQLite;

#endif

namespace Aurea.CRM.Core.DAL
{
    using System;
    using Aurea.CRM.Core.Configuration;

    /// <summary>
    /// Provide access to SQLite base data base
    /// </summary>
    public abstract class DatabaseBase : SQLiteConnection, IDatabase
    {
        private object syncroot = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseBase"/> class.
        /// </summary>
        /// <param name="databaseName">
        /// Name of the database.
        /// </param>
        /// <param name="recreate">
        /// if set to <c>true</c> recreate the target by delete existing.
        /// </param>
        protected DatabaseBase(string databaseName, bool recreate = false)
#if SqlitePCL
            : base ( SimpleIoc.Default.GetInstance<ISQLitePlatform>(), 
                  PlatformPath(databaseName, recreate), SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create)
#else
            : base(PlatformPath(databaseName, recreate), SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create)
#endif
        {
            // Create (if not exists) the tables
            this.Init();
        }

        /// <summary>
        /// Get Platform specific path for data store.
        /// </summary>
        /// <param name="databaseName">
        /// Name of the database.
        /// </param>
        /// <param name="deleteExisting">
        /// The delete Existing.
        /// </param>
        /// <returns>
        /// Platform specific database path
        /// </returns>
        public static string PlatformPath(string databaseName, bool deleteExisting = false)
        {
            var filename = $"{databaseName}";
#if __ANDROID__
            string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); 
#elif __IOS__

    // we need to put in /Library/ on iOS5.1 to meet Apple's iCloud terms
    // (they don't want non-user-generated data in Documents)
        string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
        string libraryPath = Path.Combine (documentsPath, "..", "Library");
#else
            // var libraryPath = string.Empty; // Windows.Storage.ApplicationData.Current.LocalFolder.Path;
            string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
#endif
            var path = System.IO.Path.Combine(libraryPath, filename);

            if (deleteExisting)
            {
                var fileManager = ConfigurationUnitStore.DefaultStore.Platform.StorageProvider;

                Exception ex;
                fileManager.TryDelete(path, out ex);
            }

            return path;
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        public override void BeginTransaction()
        {
            lock (this.syncroot)
            {
                if (this.IsInTransaction)
                {
                    this.Commit();
                }

                base.BeginTransaction();
            }
        }

        /// <summary>
        /// Creates the table meta information.
        /// </summary>
        /// <param name="tableName">
        /// Name of the table.
        /// </param>
        /// <returns>
        /// The <see cref="DatabaseMetaInfoTable"/>.
        /// </returns>
        public virtual DatabaseMetaInfoTable CreateTableMetaInfo(string tableName)
        {
            var tableInfo = this.GetTableInfoExtended(tableName);
            var metainfo = new DatabaseMetaInfoTable(tableName);

            foreach (var row in tableInfo)
            {
                metainfo.AddField(row.Name, row.ColumnType);
            }

            metainfo.Sort();
            return metainfo;
        }

        /// <summary>
        /// Empties the table.
        /// </summary>
        /// <param name="tableName">
        /// Name of the table.
        /// </param>
        /// <returns>
        /// the number of records deleted
        /// </returns>
        public int EmptyTable(string tableName)
        {
            return this.ExistsTable(tableName) ? this.Execute($"DELETE FROM {tableName}") : 0;
        }

        /// <summary>
        /// Ensures the existance of the database.
        /// </summary>
        /// <returns>true if exists;else false</returns>
        public abstract bool EnsureDDL();

        /// <summary>
        /// Executes the specified query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="args">
        /// The arguments.
        /// </param>
        /// <returns>
        /// 1 if an error occured
        /// </returns>
        public new int Execute(string query, params object[] args)
        {
            try
            {
                base.Execute(query, args);
                return 0;
            }
            catch (Exception)
            {
                return 1;
            }
        }

        /// <summary>
        /// Check the existance of a given index.
        /// </summary>
        /// <param name="indexName">
        /// Name of the index.
        /// </param>
        /// <returns>
        /// true if exists;else false
        /// </returns>
        public bool ExistsIndex(string indexName)
        {
            var result = this.ExecuteScalar<string>($"SELECT * FROM sqlite_master WHERE type = 'index' AND name = '{indexName}'");
            return result != null;
        }

        /// <summary>
        /// The exists row.
        /// </summary>
        /// <param name="statement">
        /// The statement.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ExistsRow(string statement)
        {
            var result = this.ExecuteScalar<string>(statement);
            return result != null;
        }

        /// <summary>
        /// Check the existance of a given table.
        /// </summary>
        /// <param name="tableName">
        /// Name of the table.
        /// </param>
        /// <returns>
        /// true if exists;else false
        /// </returns>
        public bool ExistsTable(string tableName)
        {
            var result = this.ExecuteScalar<string>($"SELECT * FROM sqlite_master WHERE type = 'table' AND name = '{tableName}'");

            return result != null;
        }

        /// <summary>
        /// Gets the row count.
        /// </summary>
        /// <param name="tableName">
        /// Name of the table.
        /// </param>
        /// <returns>
        /// the row count
        /// </returns>
        public int GetRowCount(string tableName)
        {
            var result = this.ExecuteScalar<int>($"SELECT COUNT(*) FROM {tableName}");
            return result;
        }

        /// <summary>
        /// Perform an action with a database lock.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <param name="lockObject">
        /// The lock object.
        /// </param>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <returns>
        /// the operation result
        /// </returns>
        public TResult WithDatabaseLock<TResult>(object lockObject, Func<TResult> operation)
        {
            lock (lockObject)
            {
                return operation != null ? operation() : default(TResult);
            }
        }

        /// <summary>
        /// Perform an action with a database lock.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <returns>
        /// the operation result
        /// </returns>
        public TResult WithDatabaseLock<TResult>(Func<TResult> operation)
        {
            lock (this.LockObject())
            {
                return operation != null ? operation() : default(TResult);
            }
        }

        public SQLiteConnection GetConnection()
        {
            return this;
        }

        /// <summary>
        /// Initializes this database.
        /// typically used to create the database tables, etc...
        /// </summary>
        protected abstract void Init();

        /// <summary>
        /// Provide the lock object.
        /// </summary>
        /// <returns>the lock object</returns>
        protected abstract object LockObject();
    }
}
