// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDatabase.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2017 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.DAL
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using SQLite;

    /// <summary>
    /// Interface for interacting with database.
    /// </summary>
    public interface IDatabase
    {
        /// <summary>
        /// Begins the transaction.
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Creates the table meta information.
        /// </summary>
        /// <param name="tableName">
        /// Name of the table.
        /// </param>
        /// <returns>
        /// The <see cref="DatabaseMetaInfoTable"/>.
        /// </returns>
        DatabaseMetaInfoTable CreateTableMetaInfo(string tableName);

        /// <summary>
        /// Empties the table.
        /// </summary>
        /// <param name="tableName">
        /// Name of the table.
        /// </param>
        /// <returns>
        /// the number of records deleted
        /// </returns>
        int EmptyTable(string tableName);

        /// <summary>
        /// Ensures the existance of the database.
        /// </summary>
        /// <returns>true if exists;else false</returns>
        bool EnsureDDL();

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
        new int Execute(string query, params object[] args);

        /// <summary>
        /// Check the existance of a given index.
        /// </summary>
        /// <param name="indexName">
        /// Name of the index.
        /// </param>
        /// <returns>
        /// true if exists;else false
        /// </returns>
        bool ExistsIndex(string indexName);

        /// <summary>
        /// The exists row.
        /// </summary>
        /// <param name="statement">
        /// The statement.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool ExistsRow(string statement);

        /// <summary>
        /// Check the existance of a given table.
        /// </summary>
        /// <param name="tableName">
        /// Name of the table.
        /// </param>
        /// <returns>
        /// true if exists;else false
        /// </returns>
        bool ExistsTable(string tableName);

        /// <summary>
        /// Gets the row count.
        /// </summary>
        /// <param name="tableName">
        /// Name of the table.
        /// </param>
        /// <returns>
        /// the row count
        /// </returns>
        int GetRowCount(string tableName);

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
        TResult WithDatabaseLock<TResult>(object lockObject, Func<TResult> operation);

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
        TResult WithDatabaseLock<TResult>(Func<TResult> operation);

        /// <summary>
        /// Gets the last row identifier.
        /// </summary>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        long GetLastRowId();

        /// <summary>
        /// Gets the table information extended.
        /// </summary>
        /// <param name="tableName">
        /// Name of the table.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        List<ColumnInfoExt> GetTableInfoExtended(string tableName);

        /// <summary>
        /// Gets the handle.
        /// </summary>
        IntPtr Handle { get; }

        /// <summary>
        /// Gets the database path.
        /// </summary>
        string DatabasePath { get; }

        /// <summary>
        /// Gets or sets a value indicating whether time execution.
        /// </summary>
        bool TimeExecution { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether trace.
        /// </summary>
        bool Trace { get; set; }

        /// <summary>
        /// Gets a value indicating whether store date time as ticks.
        /// </summary>
        bool StoreDateTimeAsTicks { get; }

        /// <summary>
        /// Sets a busy handler to sleep the specified amount of time when a table is locked.
        /// The handler will sleep multiple times until a total time of <see cref="BusyTimeout"/> has accumulated.
        /// </summary>
        TimeSpan BusyTimeout { get; set; }

        /// <summary>
        /// Returns the mappings from types to tables that the connection
        /// currently understands.
        /// </summary>
        IEnumerable<TableMapping> TableMappings { get; }

        /// <summary>
        /// Whether <see cref="BeginTransaction"/> has been called and the database is waiting for a <see cref="Commit"/>.
        /// </summary>
        bool IsInTransaction { get; }

        /// <summary>
        /// The enable load extension.
        /// </summary>
        /// <param name="onoff">
        /// The onoff.
        /// </param>
        /// <exception cref="SQLiteException">
        /// </exception>
        void EnableLoadExtension(int onoff);

        /// <summary>
        /// Retrieves the mapping that is automatically generated for the given type.
        /// </summary>
        /// <param name="type">
        /// The type whose mapping to the database is returned.
        /// </param>
        /// <param name="createFlags">
        /// Optional flags allowing implicit PK and indexes based on naming conventions
        /// </param>
        /// <returns>
        /// The mapping represents the schema of the columns of the database and contains
        /// methods to set and get properties of objects.
        /// </returns>
        TableMapping GetMapping(Type type, CreateFlags createFlags = CreateFlags.None);

        /// <summary>
        /// Retrieves the mapping that is automatically generated for the given type.
        /// </summary>
        /// <returns>
        /// The mapping represents the schema of the columns of the database and contains
        /// methods to set and get properties of objects.
        /// </returns>
        TableMapping GetMapping<T>();

        /// <summary>
        /// Executes a "drop table" on the database.  This is non-recoverable.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        int DropTable<T>();

        /// <summary>
        /// Executes a "create table if not exists" on the database. It also
        /// creates any specified indexes on the columns of the table. It uses
        /// a schema automatically generated from the specified type. You can
        /// later access this schema by calling GetMapping.
        /// </summary>
        /// <param name="createFlags">
        /// The create Flags.
        /// </param>
        /// <returns>
        /// The number of entries added to the database schema.
        /// </returns>
        int CreateTable<T>(CreateFlags createFlags = CreateFlags.None);

        /// <summary>
        /// Executes a "create table if not exists" on the database. It also
        /// creates any specified indexes on the columns of the table. It uses
        /// a schema automatically generated from the specified type. You can
        /// later access this schema by calling GetMapping.
        /// </summary>
        /// <param name="ty">
        /// Type to reflect to a database table.
        /// </param>
        /// <param name="createFlags">
        /// Optional flags allowing implicit PK and indexes based on naming conventions.
        /// </param>
        /// <returns>
        /// The number of entries added to the database schema.
        /// </returns>
        int CreateTable(Type ty, CreateFlags createFlags = CreateFlags.None);

        /// <summary>
        /// Creates an index for the specified table and columns.
        /// </summary>
        /// <param name="indexName">
        /// Name of the index to create
        /// </param>
        /// <param name="tableName">
        /// Name of the database table
        /// </param>
        /// <param name="columnNames">
        /// An array of column names to index
        /// </param>
        /// <param name="unique">
        /// Whether the index should be unique
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        int CreateIndex(string indexName, string tableName, string[] columnNames, bool unique = false);

        /// <summary>
        /// Creates an index for the specified table and column.
        /// </summary>
        /// <param name="indexName">
        /// Name of the index to create
        /// </param>
        /// <param name="tableName">
        /// Name of the database table
        /// </param>
        /// <param name="columnName">
        /// Name of the column to index
        /// </param>
        /// <param name="unique">
        /// Whether the index should be unique
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        int CreateIndex(string indexName, string tableName, string columnName, bool unique = false);

        /// <summary>
        /// Creates an index for the specified table and column.
        /// </summary>
        /// <param name="tableName">
        /// Name of the database table
        /// </param>
        /// <param name="columnName">
        /// Name of the column to index
        /// </param>
        /// <param name="unique">
        /// Whether the index should be unique
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        int CreateIndex(string tableName, string columnName, bool unique = false);

        /// <summary>
        /// Creates an index for the specified table and columns.
        /// </summary>
        /// <param name="tableName">
        /// Name of the database table
        /// </param>
        /// <param name="columnNames">
        /// An array of column names to index
        /// </param>
        /// <param name="unique">
        /// Whether the index should be unique
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        int CreateIndex(string tableName, string[] columnNames, bool unique = false);

        /// <summary>
        /// The create index.
        /// </summary>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <param name="unique">
        /// The unique.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <exception cref="ArgumentException">
        /// </exception>
        void CreateIndex<T>(Expression<Func<T, object>> property, bool unique = false);

        /// <summary>
        /// The get table info.
        /// </summary>
        /// <param name="tableName">
        /// The table name.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        List<SQLiteConnection.ColumnInfo> GetTableInfo(string tableName);

        /// <summary>
        /// Creates a new SQLiteCommand given the command text with arguments. Place a '?'
        /// in the command text for each of the arguments.
        /// </summary>
        /// <param name="cmdText">
        /// The fully escaped SQL.
        /// </param>
        /// <param name="ps">
        /// The ps.
        /// </param>
        /// <returns>
        /// A <see cref="SQLiteCommand"/>
        /// </returns>
        SQLiteCommand CreateCommand(string cmdText, params object[] ps);


        /// <summary>
        /// The execute scalar.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        T ExecuteScalar<T>(string query, params object[] args);

        /// <summary>
        /// Creates a SQLiteCommand given the command text (SQL) with arguments. Place a '?'
        /// in the command text for each of the arguments and then executes that command.
        /// It returns each row of the result using the mapping automatically generated for
        /// the given type.
        /// </summary>
        /// <param name="query">
        /// The fully escaped SQL.
        /// </param>
        /// <param name="args">
        /// Arguments to substitute for the occurences of '?' in the query.
        /// </param>
        /// <returns>
        /// An enumerable with one result for each row returned by the query.
        /// </returns>
        List<T> Query<T>(string query, params object[] args) where T : new();

        /// <summary>
        /// Creates a SQLiteCommand given the command text (SQL) with arguments. Place a '?'
        /// in the command text for each of the arguments and then executes that command.
        /// It returns each row of the result using the mapping automatically generated for
        /// the given type.
        /// </summary>
        /// <param name="query">
        /// The fully escaped SQL.
        /// </param>
        /// <param name="args">
        /// Arguments to substitute for the occurences of '?' in the query.
        /// </param>
        /// <returns>
        /// An enumerable with one result for each row returned by the query.
        /// The enumerator will call sqlite3_step on each call to MoveNext, so the database
        /// connection must remain open for the lifetime of the enumerator.
        /// </returns>
        IEnumerable<T> DeferredQuery<T>(string query, params object[] args) where T : new();

        /// <summary>
        /// Creates a SQLiteCommand given the command text (SQL) with arguments. Place a '?'
        /// in the command text for each of the arguments and then executes that command.
        /// It returns each row of the result using the specified mapping. This function is
        /// only used by libraries in order to query the database via introspection. It is
        /// normally not used.
        /// </summary>
        /// <param name="map">
        /// A <see cref="TableMapping"/> to use to convert the resulting rows
        /// into objects.
        /// </param>
        /// <param name="query">
        /// The fully escaped SQL.
        /// </param>
        /// <param name="args">
        /// Arguments to substitute for the occurences of '?' in the query.
        /// </param>
        /// <returns>
        /// An enumerable with one result for each row returned by the query.
        /// </returns>
        List<object> Query(TableMapping map, string query, params object[] args);

        /// <summary>
        /// Creates a SQLiteCommand given the command text (SQL) with arguments. Place a '?'
        /// in the command text for each of the arguments and then executes that command.
        /// It returns each row of the result using the specified mapping. This function is
        /// only used by libraries in order to query the database via introspection. It is
        /// normally not used.
        /// </summary>
        /// <param name="map">
        /// A <see cref="TableMapping"/> to use to convert the resulting rows
        /// into objects.
        /// </param>
        /// <param name="query">
        /// The fully escaped SQL.
        /// </param>
        /// <param name="args">
        /// Arguments to substitute for the occurences of '?' in the query.
        /// </param>
        /// <returns>
        /// An enumerable with one result for each row returned by the query.
        /// The enumerator will call sqlite3_step on each call to MoveNext, so the database
        /// connection must remain open for the lifetime of the enumerator.
        /// </returns>
        IEnumerable<object> DeferredQuery(TableMapping map, string query, params object[] args);

        /// <summary>
        /// Returns a queryable interface to the table represented by the given type.
        /// </summary>
        /// <returns>
        /// A queryable object that is able to translate Where, OrderBy, and Take
        /// queries into native SQL.
        /// </returns>
        TableQuery<T> Table<T>() where T : new();

        /// <summary>
        /// Attempts to retrieve an object with the given primary key from the table
        /// associated with the specified type. Use of this method requires that
        /// the given type have a designated PrimaryKey (using the PrimaryKeyAttribute).
        /// </summary>
        /// <param name="pk">
        /// The primary key.
        /// </param>
        /// <returns>
        /// The object with the given primary key. Throws a not found exception
        /// if the object is not found.
        /// </returns>
        T Get<T>(object pk) where T : new();

        /// <summary>
        /// Attempts to retrieve the first object that matches the predicate from the table
        /// associated with the specified type.
        /// </summary>
        /// <param name="predicate">
        /// A predicate for which object to find.
        /// </param>
        /// <returns>
        /// The object that matches the given predicate. Throws a not found exception
        /// if the object is not found.
        /// </returns>
        T Get<T>(Expression<Func<T, bool>> predicate) where T : new();

        /// <summary>
        /// Attempts to retrieve an object with the given primary key from the table
        /// associated with the specified type. Use of this method requires that
        /// the given type have a designated PrimaryKey (using the PrimaryKeyAttribute).
        /// </summary>
        /// <param name="pk">
        /// The primary key.
        /// </param>
        /// <returns>
        /// The object with the given primary key or null
        /// if the object is not found.
        /// </returns>
        T Find<T>(object pk) where T : new();

        /// <summary>
        /// Attempts to retrieve an object with the given primary key from the table
        /// associated with the specified type. Use of this method requires that
        /// the given type have a designated PrimaryKey (using the PrimaryKeyAttribute).
        /// </summary>
        /// <param name="pk">
        /// The primary key.
        /// </param>
        /// <param name="map">
        /// The TableMapping used to identify the object type.
        /// </param>
        /// <returns>
        /// The object with the given primary key or null
        /// if the object is not found.
        /// </returns>
        object Find(object pk, TableMapping map);

        /// <summary>
        /// Attempts to retrieve the first object that matches the predicate from the table
        /// associated with the specified type.
        /// </summary>
        /// <param name="predicate">
        /// A predicate for which object to find.
        /// </param>
        /// <returns>
        /// The object that matches the given predicate or null
        /// if the object is not found.
        /// </returns>
        T Find<T>(Expression<Func<T, bool>> predicate) where T : new();

        /// <summary>
        /// Attempts to retrieve the first object that matches the query from the table
        /// associated with the specified type.
        /// </summary>
        /// <param name="query">
        /// The fully escaped SQL.
        /// </param>
        /// <param name="args">
        /// Arguments to substitute for the occurences of '?' in the query.
        /// </param>
        /// <returns>
        /// The object that matches the given predicate or null
        /// if the object is not found.
        /// </returns>
        T FindWithQuery<T>(string query, params object[] args) where T : new();

        /// <summary>
        /// Creates a savepoint in the database at the current point in the transaction timeline.
        /// Begins a new transaction if one is not in progress.
        ///
        /// Call <see cref="SQLiteConnection.RollbackTo"/> to undo transactions since the returned savepoint.
        /// Call <see cref="SQLiteConnection.Release"/> to commit transactions after the savepoint returned here.
        /// Call <see cref="SQLiteConnection.Commit"/> to end the transaction, committing all changes.
        /// </summary>
        /// <returns>A string naming the savepoint.</returns>
        string SaveTransactionPoint();

        /// <summary>
        /// Rolls back the transaction that was begun by <see cref="SQLiteConnection.BeginTransaction"/> or <see cref="SQLiteConnection.SaveTransactionPoint"/>.
        /// </summary>
        void Rollback();

        /// <summary>
        /// Rolls back the savepoint created by <see cref="SQLiteConnection.BeginTransaction"/> or SaveTransactionPoint.
        /// </summary>
        /// <param name="savepoint">
        /// The name of the savepoint to roll back to, as returned by <see cref="SQLiteConnection.SaveTransactionPoint"/>.  If savepoint is null or empty, this method is equivalent to a call to <see cref="SQLiteConnection.Rollback"/>
        /// </param>
        void RollbackTo(string savepoint);

        /// <summary>
        /// Releases a savepoint returned from <see cref="SQLiteConnection.SaveTransactionPoint"/>.  Releasing a savepoint
        ///    makes changes since that savepoint permanent if the savepoint began the transaction,
        ///    or otherwise the changes are permanent pending a call to <see cref="SQLiteConnection.Commit"/>.
        ///
        /// The RELEASE command is like a COMMIT for a SAVEPOINT.
        /// </summary>
        /// <param name="savepoint">
        /// The name of the savepoint to release.  The string should be the result of a call to <see cref="SQLiteConnection.SaveTransactionPoint"/>
        /// </param>
        void Release(string savepoint);

        /// <summary>
        /// Commits the transaction that was begun by <see cref="SQLiteConnection.BeginTransaction"/>.
        /// </summary>
        void Commit();

        /// <summary>
        /// The run in transaction.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        void RunInTransaction(Action action);

        /// <summary>
        /// Inserts all specified objects.
        /// </summary>
        /// <param name="objects">
        /// An <see cref="IEnumerable"/> of the objects to insert.
        /// A boolean indicating if the inserts should be wrapped in a transaction.
        /// </param>
        /// <param name="runInTransaction">
        /// </param>
        /// <returns>
        /// The number of rows added to the table.
        /// </returns>
        int InsertAll(IEnumerable objects, bool runInTransaction = true);

        /// <summary>
        /// The insert all.
        /// </summary>
        /// <param name="objects">
        /// The objects.
        /// </param>
        /// <param name="extra">
        /// The extra.
        /// </param>
        /// <param name="runInTransaction">
        /// The run in transaction.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        int InsertAll(IEnumerable objects, string extra, bool runInTransaction = true);

        /// <summary>
        /// The insert all.
        /// </summary>
        /// <param name="objects">
        /// The objects.
        /// </param>
        /// <param name="objType">
        /// The obj type.
        /// </param>
        /// <param name="runInTransaction">
        /// The run in transaction.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        int InsertAll(IEnumerable objects, Type objType, bool runInTransaction = true);

        /// <summary>
        /// Inserts the given object and retrieves its
        /// auto incremented primary key if it has one.
        /// </summary>
        /// <param name="obj">
        /// The object to insert.
        /// </param>
        /// <returns>
        /// The number of rows added to the table.
        /// </returns>
        int Insert(object obj);

        /// <summary>
        /// Inserts the given object and retrieves its
        /// auto incremented primary key if it has one.
        /// If a UNIQUE constraint violation occurs with
        /// some pre-existing object, this function deletes
        /// the old object.
        /// </summary>
        /// <param name="obj">
        /// The object to insert.
        /// </param>
        /// <returns>
        /// The number of rows modified.
        /// </returns>
        int InsertOrReplace(object obj);

        /// <summary>
        /// Inserts the given object and retrieves its
        /// auto incremented primary key if it has one.
        /// </summary>
        /// <param name="obj">
        /// The object to insert.
        /// </param>
        /// <param name="objType">
        /// The type of object to insert.
        /// </param>
        /// <returns>
        /// The number of rows added to the table.
        /// </returns>
        int Insert(object obj, Type objType);

        /// <summary>
        /// Inserts the given object and retrieves its
        /// auto incremented primary key if it has one.
        /// If a UNIQUE constraint violation occurs with
        /// some pre-existing object, this function deletes
        /// the old object.
        /// </summary>
        /// <param name="obj">
        /// The object to insert.
        /// </param>
        /// <param name="objType">
        /// The type of object to insert.
        /// </param>
        /// <returns>
        /// The number of rows modified.
        /// </returns>
        int InsertOrReplace(object obj, Type objType);

        /// <summary>
        /// Inserts the given object and retrieves its
        /// auto incremented primary key if it has one.
        /// </summary>
        /// <param name="obj">
        /// The object to insert.
        /// </param>
        /// <param name="extra">
        /// Literal SQL code that gets placed into the command. INSERT {extra} INTO ...
        /// </param>
        /// <returns>
        /// The number of rows added to the table.
        /// </returns>
        int Insert(object obj, string extra);

        /// <summary>
        /// Inserts the given object and retrieves its
        /// auto incremented primary key if it has one.
        /// </summary>
        /// <param name="obj">
        /// The object to insert.
        /// </param>
        /// <param name="extra">
        /// Literal SQL code that gets placed into the command. INSERT {extra} INTO ...
        /// </param>
        /// <param name="objType">
        /// The type of object to insert.
        /// </param>
        /// <returns>
        /// The number of rows added to the table.
        /// </returns>
        int Insert(object obj, string extra, Type objType);

        /// <summary>
        /// Updates all of the columns of a table using the specified object
        /// except for its primary key.
        /// The object is required to have a primary key.
        /// </summary>
        /// <param name="obj">
        /// The object to update. It must have a primary key designated using the PrimaryKeyAttribute.
        /// </param>
        /// <returns>
        /// The number of rows updated.
        /// </returns>
        int Update(object obj);

        /// <summary>
        /// Updates all of the columns of a table using the specified object
        /// except for its primary key.
        /// The object is required to have a primary key.
        /// </summary>
        /// <param name="obj">
        /// The object to update. It must have a primary key designated using the PrimaryKeyAttribute.
        /// </param>
        /// <param name="objType">
        /// The type of object to insert.
        /// </param>
        /// <returns>
        /// The number of rows updated.
        /// </returns>
        int Update(object obj, Type objType);

        /// <summary>
        /// The update all.
        /// </summary>
        /// <param name="objects">
        /// The objects.
        /// </param>
        /// <param name="runInTransaction">
        /// The run in transaction.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        int UpdateAll(IEnumerable objects, bool runInTransaction = true);

        /// <summary>
        /// Deletes the given object from the database using its primary key.
        /// </summary>
        /// <param name="objectToDelete">
        /// The object to delete. It must have a primary key designated using the PrimaryKeyAttribute.
        /// </param>
        /// <returns>
        /// The number of rows deleted.
        /// </returns>
        int Delete(object objectToDelete);

        /// <summary>
        /// Deletes the object with the specified primary key.
        /// </summary>
        /// <param name="primaryKey">
        /// The primary key of the object to delete.
        /// </param>
        /// <returns>
        /// The number of objects deleted.
        /// </returns>
        /// <typeparam name="T">
        /// The type of object.
        /// </typeparam>
        int Delete<T>(object primaryKey);

        /// <summary>
        /// Deletes all the objects from the specified table.
        /// WARNING WARNING: Let me repeat. It deletes ALL the objects from the
        /// specified table. Do you really want to do that?
        /// </summary>
        /// <returns>
        /// The number of objects deleted.
        /// </returns>
        /// <typeparam name='T'>
        /// The type of objects to delete.
        /// </typeparam>
        int DeleteAll<T>();

        /// <summary>
        /// The dispose.
        /// </summary>
        void Dispose();

        /// <summary>
        /// The close.
        /// </summary>
        void Close();

        /// <summary>
        /// The table changed.
        /// </summary>
        event EventHandler<NotifyTableChangedEventArgs> TableChanged;

        /// <summary>
        /// Gets SQLiteConnection object
        /// </summary>
        /// <returns>Return <see cref="SQLiteConnection"/> object </returns>
        SQLiteConnection GetConnection();
    }
}