// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="SQLiteORM.cs">
// </copyright>
// <summary>
//   The sq lite exception.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
#define USE_NEW_REFLECTION_API
#if WINDOWS_PHONE && !USE_WP8_NATIVE_SQLITE
#define USE_CSHARP_SQLITE
#endif

#if NETFX_CORE
#define USE_NEW_REFLECTION_API
#endif

#if !USE_SQLITEPCL_RAW
#endif
#if NO_CONCURRENT
using ConcurrentStringDictionary = System.Collections.Generic.Dictionary<string, object>;
using SQLite.Extensions;
#else
#endif
#if USE_CSHARP_SQLITE
using Sqlite3 = Community.CsharpSqlite.Sqlite3;
using Sqlite3DatabaseHandle = Community.CsharpSqlite.Sqlite3.sqlite3;
using Sqlite3Statement = Community.CsharpSqlite.Sqlite3.Vdbe;
#elif USE_WP8_NATIVE_SQLITE
using Sqlite3 = Sqlite.Sqlite3;
using Sqlite3DatabaseHandle = Sqlite.Database;
using Sqlite3Statement = Sqlite.Statement;
#elif USE_SQLITEPCL_RAW
using Sqlite3DatabaseHandle = SQLitePCL.sqlite3;
using Sqlite3Statement = SQLitePCL.sqlite3_stmt;
using Sqlite3 = SQLitePCL.raw;
#else
#endif

namespace SQLite
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    //using Microsoft.Practices.ServiceLocation;
    using GalaSoft.MvvmLight.Ioc;
    using Newtonsoft.Json;
    using ConcurrentStringDictionary = System.Collections.Concurrent.ConcurrentDictionary<string, object>;
    using Sqlite3DatabaseHandle = System.IntPtr;
    using Sqlite3Statement = System.IntPtr;
    using SQLitePath;

    /// <summary>
    /// The sq lite exception.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class SQLiteException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteException"/> class.
        /// </summary>
        /// <param name="r">
        /// The r.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        protected SQLiteException(SQLite3.Result r, string message)
            : base(message)
        {
            this.Result = r;
        }

        /// <summary>
        /// Gets the result.
        /// </summary>
        public SQLite3.Result Result { get; private set; }

        /// <summary>
        /// The new.
        /// </summary>
        /// <param name="r">
        /// The r.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="SQLiteException"/>.
        /// </returns>
        public static SQLiteException New(SQLite3.Result r, string message)
        {
            return new SQLiteException(r, message);
        }
    }

    /// <summary>
    /// The not null constraint violation exception.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class NotNullConstraintViolationException : SQLiteException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotNullConstraintViolationException"/> class.
        /// </summary>
        /// <param name="r">
        /// The r.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        protected NotNullConstraintViolationException(SQLite3.Result r, string message)
            : this(r, message, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotNullConstraintViolationException"/> class.
        /// </summary>
        /// <param name="r">
        /// The r.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="mapping">
        /// The mapping.
        /// </param>
        /// <param name="obj">
        /// The obj.
        /// </param>
        protected NotNullConstraintViolationException(
            SQLite3.Result r,
            string message,
            TableMapping mapping,
            object obj)
            : base(r, message)
        {
            if (mapping != null && obj != null)
            {
                this.Columns = from c in mapping.Columns where c.IsNullable == false && c.GetValue(obj) == null select c;
            }
        }

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        public IEnumerable<TableMapping.Column> Columns { get; protected set; }

        /// <summary>
        /// The new.
        /// </summary>
        /// <param name="r">
        /// The r.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="NotNullConstraintViolationException"/>.
        /// </returns>
        public static new NotNullConstraintViolationException New(SQLite3.Result r, string message)
        {
            return new NotNullConstraintViolationException(r, message);
        }

        /// <summary>
        /// The new.
        /// </summary>
        /// <param name="r">
        /// The r.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="mapping">
        /// The mapping.
        /// </param>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="NotNullConstraintViolationException"/>.
        /// </returns>
        public static NotNullConstraintViolationException New(
            SQLite3.Result r,
            string message,
            TableMapping mapping,
            object obj)
        {
            return new NotNullConstraintViolationException(r, message, mapping, obj);
        }

        /// <summary>
        /// The new.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="mapping">
        /// The mapping.
        /// </param>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="NotNullConstraintViolationException"/>.
        /// </returns>
        public static NotNullConstraintViolationException New(
            SQLiteException exception,
            TableMapping mapping,
            object obj)
        {
            return new NotNullConstraintViolationException(exception.Result, exception.Message, mapping, obj);
        }
    }

    /// <summary>
    /// The sq lite open flags.
    /// </summary>
    [Flags]
    public enum SQLiteOpenFlags
    {
        /// <summary>
        /// The read only.
        /// </summary>
        ReadOnly = 1,

        /// <summary>
        /// The read write.
        /// </summary>
        ReadWrite = 2,

        /// <summary>
        /// The create.
        /// </summary>
        Create = 4,

        /// <summary>
        /// The no mutex.
        /// </summary>
        NoMutex = 0x8000,

        /// <summary>
        /// The full mutex.
        /// </summary>
        FullMutex = 0x10000,

        /// <summary>
        /// The shared cache.
        /// </summary>
        SharedCache = 0x20000,

        /// <summary>
        /// The private cache.
        /// </summary>
        PrivateCache = 0x40000,

        /// <summary>
        /// The protection complete.
        /// </summary>
        ProtectionComplete = 0x00100000,

        /// <summary>
        /// The protection complete unless open.
        /// </summary>
        ProtectionCompleteUnlessOpen = 0x00200000,

        /// <summary>
        /// The protection complete until first user authentication.
        /// </summary>
        ProtectionCompleteUntilFirstUserAuthentication = 0x00300000,

        /// <summary>
        /// The protection none.
        /// </summary>
        ProtectionNone = 0x00400000
    }

    /// <summary>
    /// The create flags.
    /// </summary>
    [Flags]
    public enum CreateFlags
    {
        /// <summary>
        /// The none.
        /// </summary>
        None = 0x000,

        /// <summary>
        /// The implicit pk.
        /// </summary>
        ImplicitPK = 0x001, // create a primary key for field called 'Id' (Orm.ImplicitPkName)

        /// <summary>
        /// The implicit index.
        /// </summary>
        ImplicitIndex = 0x002, // create an index for fields ending in 'Id' (Orm.ImplicitIndexSuffix)

        /// <summary>
        /// The all implicit.
        /// </summary>
        AllImplicit = 0x003, // do both above

        /// <summary>
        /// The auto inc pk.
        /// </summary>
        AutoIncPK = 0x004, // force PK field to be auto inc

        /// <summary>
        /// The full text search 3.
        /// </summary>
        FullTextSearch3 = 0x100, // create virtual table using FTS3

        /// <summary>
        /// The full text search 4.
        /// </summary>
        FullTextSearch4 = 0x200 // create virtual table using FTS4
    }

    /// <summary>
    /// Represents an open connection to a SQLite database.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class SQLiteConnection : IDisposable
    {
        /// <summary>
        /// The _open.
        /// </summary>
        private bool _open;

        /// <summary>
        /// The _busy timeout.
        /// </summary>
        private TimeSpan _busyTimeout;

        /// <summary>
        /// The _mappings.
        /// </summary>
        private Dictionary<string, TableMapping> _mappings = null;

        /// <summary>
        /// The _tables.
        /// </summary>
        private Dictionary<string, TableMapping> _tables = null;

        /// <summary>
        /// The _sw.
        /// </summary>
        private Stopwatch _sw;

        /// <summary>
        /// The _elapsed milliseconds.
        /// </summary>
        private long _elapsedMilliseconds = 0;

        /// <summary>
        /// The _transaction depth.
        /// </summary>
        private int _transactionDepth = 0;

        /// <summary>
        /// The _rand.
        /// </summary>
        private readonly Random _rand = new Random();

        /// <summary>
        /// Gets the handle.
        /// </summary>
        public IntPtr Handle { get; private set; }

        /// <summary>
        /// The null handle.
        /// </summary>
        internal static readonly IntPtr NullHandle = default(Sqlite3DatabaseHandle);

        /// <summary>
        /// Gets the database path.
        /// </summary>
        public string DatabasePath { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether time execution.
        /// </summary>
        public bool TimeExecution { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether trace.
        /// </summary>
        public bool Trace { get; set; }

        /// <summary>
        /// Gets a value indicating whether store date time as ticks.
        /// </summary>
        public bool StoreDateTimeAsTicks { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteConnection"/> class.
        /// Constructs a new SQLiteConnection and opens a SQLite database specified by databasePath.
        /// </summary>
        /// <param name="databasePath">
        /// Specifies the path to the database file.
        /// </param>
        /// <param name="storeDateTimeAsTicks">
        /// Specifies whether to store DateTime properties as ticks (true) or strings (false). You
        /// absolutely do want to store them as Ticks in all new projects. The value of false is
        /// only here for backwards compatibility. There is a *significant* speed advantage, with no
        /// down sides, when setting storeDateTimeAsTicks = true.
        /// If you use DateTimeOffset properties, it will be always stored as ticks regardingless
        /// the storeDateTimeAsTicks parameter.
        /// </param>
        public SQLiteConnection(string databasePath, bool storeDateTimeAsTicks = true)
            : this(databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, storeDateTimeAsTicks)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteConnection"/> class.
        /// Constructs a new SQLiteConnection and opens a SQLite database specified by databasePath.
        /// </summary>
        /// <param name="databasePath">
        /// Specifies the path to the database file.
        /// </param>
        /// <param name="openFlags">
        /// The open Flags.
        /// </param>
        /// <param name="storeDateTimeAsTicks">
        /// Specifies whether to store DateTime properties as ticks (true) or strings (false). You
        /// absolutely do want to store them as Ticks in all new projects. The value of false is
        /// only here for backwards compatibility. There is a *significant* speed advantage, with no
        /// down sides, when setting storeDateTimeAsTicks = true.
        /// If you use DateTimeOffset properties, it will be always stored as ticks regardingless
        /// the storeDateTimeAsTicks parameter.
        /// </param>
        public SQLiteConnection(string databasePath, SQLiteOpenFlags openFlags, bool storeDateTimeAsTicks = true)
        {
            if (string.IsNullOrEmpty(databasePath))
            {
                throw new ArgumentException("Must be specified", "databasePath");
            }

            this.DatabasePath = databasePath;

#if NETFX_CORE
            SQLite3.SetDirectory(/*temp directory type*/2, Windows.Storage.ApplicationData.Current.TemporaryFolder.Path);
#endif

            IntPtr handle;

#if SILVERLIGHT || USE_CSHARP_SQLITE || USE_SQLITEPCL_RAW
            SQLite3.Result r;
            if (databasePath.ContainsInternationalCharacter())
            {
                var databasePathAsBytes = GetNullTerminatedUtf8(databasePath);

                r = SQLite3.Open(databasePathAsBytes, out handle, (int)(openFlags | SQLiteOpenFlags.FullMutex), IntPtr.Zero);
            }
            else
            {
                r = SQLite3.Open(databasePath, out handle, (int)(openFlags | SQLiteOpenFlags.FullMutex), IntPtr.Zero);
            }
#else

    // open using the byte[]
    // in the case where the path may include Unicode
    // force open to using UTF-8 using sqlite3_open_v2
            var databasePathAsBytes = GetNullTerminatedUtf8(DatabasePath);
            var r = SQLite3.Open(databasePathAsBytes, out handle, (int)openFlags, IntPtr.Zero);
#endif

            this.Handle = handle;
            if (r != SQLite3.Result.OK)
            {
                throw SQLiteException.New(
                    r,
                    string.Format("Could not open database file: {0} ({1})", this.DatabasePath, r));
            }

            this._open = true;

            this.StoreDateTimeAsTicks = storeDateTimeAsTicks;

            this.BusyTimeout = TimeSpan.FromSeconds(0.1);
        }

#if __IOS__
		static SQLiteConnection ()
		{
			if (_preserveDuringLinkMagic) {
				var ti = new ColumnInfo ();
				ti.Name = "magic";
			}
		}

   		// <summary>
		/// Used to list some code that we want the MonoTouch linker
		/// to see, but that we never want to actually execute.
		/// </summary>
		static bool _preserveDuringLinkMagic;
#endif

#if !USE_SQLITEPCL_RAW

        /// <summary>
        /// The enable load extension.
        /// </summary>
        /// <param name="onoff">
        /// The onoff.
        /// </param>
        /// <exception cref="SQLiteException">
        /// </exception>
        public void EnableLoadExtension(int onoff)
        {
            SQLite3.Result r = SQLite3.EnableLoadExtension(this.Handle, onoff);
            if (r != SQLite3.Result.OK)
            {
                string msg = SQLite3.GetErrmsg(this.Handle);
                throw SQLiteException.New(r, msg);
            }
        }

#endif

#if !USE_SQLITEPCL_RAW

        /// <summary>
        /// The get null terminated utf 8.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The <see cref="T:byte[]"/>.
        /// </returns>
        static byte[] GetNullTerminatedUtf8(string s)
        {
            var utf8Length = System.Text.Encoding.UTF8.GetByteCount(s);
            var bytes = new byte[utf8Length + 1];
            utf8Length = System.Text.Encoding.UTF8.GetBytes(s, 0, s.Length, bytes, 0);
            return bytes;
        }

#endif

        /// <summary>
        /// Sets a busy handler to sleep the specified amount of time when a table is locked.
        /// The handler will sleep multiple times until a total time of <see cref="BusyTimeout"/> has accumulated.
        /// </summary>
        public TimeSpan BusyTimeout
        {
            get
            {
                return this._busyTimeout;
            }

            set
            {
                this._busyTimeout = value;
                if (this.Handle != NullHandle)
                {
                    SQLite3.BusyTimeout(this.Handle, (int)this._busyTimeout.TotalMilliseconds);
                }
            }
        }

        /// <summary>
        /// Returns the mappings from types to tables that the connection
        /// currently understands.
        /// </summary>
        public IEnumerable<TableMapping> TableMappings
        {
            get
            {
                return this._tables != null ? this._tables.Values : Enumerable.Empty<TableMapping>();
            }
        }

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
        public TableMapping GetMapping(Type type, CreateFlags createFlags = CreateFlags.None)
        {
            if (this._mappings == null)
            {
                this._mappings = new Dictionary<string, TableMapping>();
            }

            TableMapping map;
            if (!this._mappings.TryGetValue(type.FullName, out map))
            {
                map = new TableMapping(type, createFlags);
                this._mappings[type.FullName] = map;
            }

            return map;
        }

        /// <summary>
        /// Retrieves the mapping that is automatically generated for the given type.
        /// </summary>
        /// <returns>
        /// The mapping represents the schema of the columns of the database and contains
        /// methods to set and get properties of objects.
        /// </returns>
        public TableMapping GetMapping<T>()
        {
            return this.GetMapping(typeof(T));
        }

        /// <summary>
        /// The indexed column.
        /// </summary>
        private struct IndexedColumn
        {
            /// <summary>
            /// The order.
            /// </summary>
            public int Order;

            /// <summary>
            /// The column name.
            /// </summary>
            public string ColumnName;
        }

        /// <summary>
        /// The index info.
        /// </summary>
        private struct IndexInfo
        {
            /// <summary>
            /// The index name.
            /// </summary>
            public string IndexName;

            /// <summary>
            /// The table name.
            /// </summary>
            public string TableName;

            /// <summary>
            /// The unique.
            /// </summary>
            public bool Unique;

            /// <summary>
            /// The columns.
            /// </summary>
            public List<IndexedColumn> Columns;
        }

        /// <summary>
        /// Executes a "drop table" on the database.  This is non-recoverable.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int DropTable<T>()
        {
            var map = this.GetMapping(typeof(T));

            var query = string.Format("drop table if exists \"{0}\"", map.TableName);

            return this.Execute(query);
        }

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
        public int CreateTable<T>(CreateFlags createFlags = CreateFlags.None)
        {
            return this.CreateTable(typeof(T), createFlags);
        }

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
        public int CreateTable(Type ty, CreateFlags createFlags = CreateFlags.None)
        {
            if (this._tables == null)
            {
                this._tables = new Dictionary<string, TableMapping>();
            }

            TableMapping map;
            if (!this._tables.TryGetValue(ty.FullName, out map))
            {
                map = this.GetMapping(ty, createFlags);
                this._tables.Add(ty.FullName, map);
            }

            // Present a nice error if no columns specified
            if (map.Columns == null || map.Columns.Length == 0)
            {
                throw new Exception(
                    $"Cannot create a table with zero columns (does '{ty.FullName}' have public properties?)");
            }

            // Facilitate virtual tables a.k.a. full-text search.
            bool fts3 = (createFlags & CreateFlags.FullTextSearch3) != 0;
            bool fts4 = (createFlags & CreateFlags.FullTextSearch4) != 0;
            bool fts = fts3 || fts4;
            var @virtual = fts ? "virtual " : string.Empty;
            var @using = fts3 ? "using fts3 " : fts4 ? "using fts4 " : string.Empty;

            // Build query.
            var query = "create " + @virtual + "table if not exists \"" + map.TableName + "\" " + @using
                        + $"({Environment.NewLine}";
            var decls = map.Columns.Select(p => Orm.SqlDecl(p, this.StoreDateTimeAsTicks));
            var decl = string.Join($",{Environment.NewLine}", decls.ToArray());
            query += decl;
            query += ")";

            var count = this.Execute(query);

            if (count == 0)
            {
                // Possible bug: This always seems to return 0?
                // Table already exists, migrate it
                this.MigrateTable(map);
            }

            var indexes = new Dictionary<string, IndexInfo>();
            foreach (var c in map.Columns)
            {
                foreach (var i in c.Indices)
                {
                    var iname = i.Name ?? map.TableName + "_" + c.Name;
                    IndexInfo iinfo;
                    if (!indexes.TryGetValue(iname, out iinfo))
                    {
                        iinfo = new IndexInfo
                        {
                            IndexName = iname,
                            TableName = map.TableName,
                            Unique = i.Unique,
                            Columns = new List<IndexedColumn>()
                        };
                        indexes.Add(iname, iinfo);
                    }

                    if (i.Unique != iinfo.Unique)
                        throw new Exception("All the columns in an index must have the same value for their Unique property");

                    iinfo.Columns.Add(new IndexedColumn { Order = i.Order, ColumnName = c.Name });
                }
            }

            foreach (var indexName in indexes.Keys)
            {
                var index = indexes[indexName];
                var columns = index.Columns.OrderBy(i => i.Order).Select(i => i.ColumnName).ToArray();
                count += this.CreateIndex(indexName, index.TableName, columns, index.Unique);
            }

            return count;
        }

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
        public int CreateIndex(string indexName, string tableName, string[] columnNames, bool unique = false)
        {
            const string sqlFormat = "create {2} index if not exists \"{3}\" on \"{0}\"(\"{1}\")";
            var sql = string.Format(
                sqlFormat,
                tableName,
                string.Join("\", \"", columnNames),
                unique ? "unique" : string.Empty,
                indexName);
            return this.Execute(sql);
        }

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
        public int CreateIndex(string indexName, string tableName, string columnName, bool unique = false)
        {
            return this.CreateIndex(indexName, tableName, new[] { columnName }, unique);
        }

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
        public int CreateIndex(string tableName, string columnName, bool unique = false)
        {
            return this.CreateIndex(tableName + "_" + columnName, tableName, columnName, unique);
        }

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
        public int CreateIndex(string tableName, string[] columnNames, bool unique = false)
        {
            return this.CreateIndex(tableName + "_" + string.Join("_", columnNames), tableName, columnNames, unique);
        }

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
        public void CreateIndex<T>(Expression<Func<T, object>> property, bool unique = false)
        {
            MemberExpression mx;
            if (property.Body.NodeType == ExpressionType.Convert)
            {
                mx = ((UnaryExpression)property.Body).Operand as MemberExpression;
            }
            else
            {
                mx = property.Body as MemberExpression;
            }

            var propertyInfo = mx.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException("The lambda expression 'property' should point to a valid Property");
            }

            var propName = propertyInfo.Name;

            var map = this.GetMapping<T>();
            var colName = map.FindColumnWithPropertyName(propName).Name;

            this.CreateIndex(map.TableName, colName, unique);
        }

        /// <summary>
        /// The column info.
        /// </summary>
        public class ColumnInfo
        {
            // 			public int cid { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            [Column("name")]
            public string Name { get; set; }

            // 			[Column ("type")]
            // 			public string ColumnType { get; set; }

            /// <summary>
            /// Gets or sets the notnull.
            /// </summary>
            public int notnull { get; set; }

            // 			public string dflt_value { get; set; }

            // 			public int pk { get; set; }

            /// <summary>
            /// The to string.
            /// </summary>
            /// <returns>
            /// The <see cref="string"/>.
            /// </returns>
            public override string ToString()
            {
                return this.Name;
            }
        }

        /// <summary>
        /// The get table info.
        /// </summary>
        /// <param name="tableName">
        /// The table name.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<ColumnInfo> GetTableInfo(string tableName)
        {
            var query = "pragma table_info(\"" + tableName + "\")";
            return this.Query<ColumnInfo>(query);
        }

        /// <summary>
        /// The migrate table.
        /// </summary>
        /// <param name="map">
        /// The map.
        /// </param>
        void MigrateTable(TableMapping map)
        {
            var existingCols = this.GetTableInfo(map.TableName);

            var toBeAdded = new List<TableMapping.Column>();

            foreach (var p in map.Columns)
            {
                var found = false;
                foreach (var c in existingCols)
                {
                    found = string.Compare(p.Name, c.Name, StringComparison.OrdinalIgnoreCase) == 0;
                    if (found)
                    {
                        break;
                    }
                }

                if (!found)
                {
                    toBeAdded.Add(p);
                }
            }

            foreach (var p in toBeAdded)
            {
                var addCol = "alter table \"" + map.TableName + "\" add column "
                             + Orm.SqlDecl(p, this.StoreDateTimeAsTicks);
                this.Execute(addCol);
            }
        }

        /// <summary>
        /// Creates a new SQLiteCommand. Can be overridden to provide a sub-class.
        /// </summary>
        /// <seealso cref="SQLiteCommand.OnInstanceCreated"/>
        /// <returns>
        /// The <see cref="SQLiteCommand"/>.
        /// </returns>
        protected virtual SQLiteCommand NewCommand()
        {
            return new SQLiteCommand(this);
        }

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
        public SQLiteCommand CreateCommand(string cmdText, params object[] ps)
        {
            if (!this._open)
            {
                throw SQLiteException.New(SQLite3.Result.Error, "Cannot create commands from unopened database");
            }

            var cmd = this.NewCommand();
            cmd.CommandText = cmdText;
            if (ps != null)
            {
                foreach (var o in ps)
                {
                    cmd.Bind(o);
                }
            }
            return cmd;
        }

        /// <summary>
        /// Creates a SQLiteCommand given the command text (SQL) with arguments. Place a '?'
        /// in the command text for each of the arguments and then executes that command.
        /// Use this method instead of Query when you don't expect rows back. Such cases include
        /// INSERTs, UPDATEs, and DELETEs.
        /// You can set the Trace or TimeExecution properties of the connection
        /// to profile execution.
        /// </summary>
        /// <param name="query">
        /// The fully escaped SQL.
        /// </param>
        /// <param name="args">
        /// Arguments to substitute for the occurences of '?' in the query.
        /// </param>
        /// <returns>
        /// The number of rows modified in the database as a result of this execution.
        /// </returns>
        public virtual int Execute(string query, params object[] args)
        {
            var cmd = this.CreateCommand(query, args);

            if (this.TimeExecution)
            {
                if (this._sw == null)
                {
                    this._sw = new Stopwatch();
                }

                this._sw.Reset();
                this._sw.Start();
            }

            var r = cmd.ExecuteNonQuery();

            if (this.TimeExecution)
            {
                this._sw.Stop();
                this._elapsedMilliseconds += this._sw.ElapsedMilliseconds;
                Debug.WriteLine(
                    string.Format(
                        "Finished in {0} ms ({1:0.0} s total)",
                        this._sw.ElapsedMilliseconds,
                        this._elapsedMilliseconds / 1000.0));
            }

            return r;
        }

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
        public virtual T ExecuteScalar<T>(string query, params object[] args)
        {
            var cmd = this.CreateCommand(query, args);

            if (this.TimeExecution)
            {
                if (this._sw == null)
                {
                    this._sw = new Stopwatch();
                }

                this._sw.Reset();
                this._sw.Start();
            }

            var r = cmd.ExecuteScalar<T>();

            if (this.TimeExecution)
            {
                this._sw.Stop();
                this._elapsedMilliseconds += this._sw.ElapsedMilliseconds;
                Debug.WriteLine(
                    string.Format(
                        "Finished in {0} ms ({1:0.0} s total)",
                        this._sw.ElapsedMilliseconds,
                        this._elapsedMilliseconds / 1000.0));
            }

            return r;
        }

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
        public List<T> Query<T>(string query, params object[] args) where T : new()
        {
            var cmd = this.CreateCommand(query, args);
            return cmd.ExecuteQuery<T>();
        }

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
        public IEnumerable<T> DeferredQuery<T>(string query, params object[] args) where T : new()
        {
            var cmd = this.CreateCommand(query, args);
            return cmd.ExecuteDeferredQuery<T>();
        }

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
        public List<object> Query(TableMapping map, string query, params object[] args)
        {
            var cmd = this.CreateCommand(query, args);
            return cmd.ExecuteQuery<object>(map);
        }

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
        public IEnumerable<object> DeferredQuery(TableMapping map, string query, params object[] args)
        {
            var cmd = this.CreateCommand(query, args);
            var result = cmd.ExecuteDeferredQuery<object>(map);

            return result;
        }

        /// <summary>
        /// Returns a queryable interface to the table represented by the given type.
        /// </summary>
        /// <returns>
        /// A queryable object that is able to translate Where, OrderBy, and Take
        /// queries into native SQL.
        /// </returns>
        public TableQuery<T> Table<T>() where T : new()
        {
            return new TableQuery<T>(this);
        }

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
        public T Get<T>(object pk) where T : new()
        {
            var map = this.GetMapping(typeof(T));
            return this.Query<T>(map.GetByPrimaryKeySql, pk).First();
        }

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
        public T Get<T>(Expression<Func<T, bool>> predicate) where T : new()
        {
            return this.Table<T>().Where(predicate).First();
        }

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
        public T Find<T>(object pk) where T : new()
        {
            var map = this.GetMapping(typeof(T));
            return this.Query<T>(map.GetByPrimaryKeySql, pk).FirstOrDefault();
        }

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
        public object Find(object pk, TableMapping map)
        {
            return this.Query(map, map.GetByPrimaryKeySql, pk).FirstOrDefault();
        }

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
        public T Find<T>(Expression<Func<T, bool>> predicate) where T : new()
        {
            return this.Table<T>().Where(predicate).FirstOrDefault();
        }

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
        public T FindWithQuery<T>(string query, params object[] args) where T : new()
        {
            return this.Query<T>(query, args).FirstOrDefault();
        }

        /// <summary>
        /// Whether <see cref="BeginTransaction"/> has been called and the database is waiting for a <see cref="Commit"/>.
        /// </summary>
        public bool IsInTransaction => this._transactionDepth > 0;

        /// <summary>
        /// Begins a new transaction. Call <see cref="Commit"/> to end the transaction.
        /// </summary>
        /// <example cref="System.InvalidOperationException">Throws if a transaction has already begun.</example>
        public virtual void BeginTransaction()
        {
            // The BEGIN command only works if the transaction stack is empty,
            // or in other words if there are no pending transactions.
            // If the transaction stack is not empty when the BEGIN command is invoked,
            // then the command fails with an error.
            // Rather than crash with an error, we will just ignore calls to BeginTransaction
            // that would result in an error.
            if (Interlocked.CompareExchange(ref this._transactionDepth, 1, 0) == 0)
            {
                try
                {
                    this.Execute("begin transaction");
                }
                catch (Exception ex)
                {
                    var sqlExp = ex as SQLiteException;
                    if (sqlExp != null)
                    {
                        // It is recommended that applications respond to the errors listed below
                        // by explicitly issuing a ROLLBACK command.
                        // TODO: This rollback failsafe should be localized to all throw sites.
                        switch (sqlExp.Result)
                        {
                            case SQLite3.Result.IOError:
                            case SQLite3.Result.Full:
                            case SQLite3.Result.Busy:
                            case SQLite3.Result.NoMem:
                            case SQLite3.Result.Interrupt:
                                this.RollbackTo(null, true);
                                break;
                        }
                    }
                    else
                    {
                        // Call decrement and not VolatileWrite in case we've already
                        // created a transaction point in SaveTransactionPoint since the catch.
                        Interlocked.Decrement(ref this._transactionDepth);
                    }

                    throw;
                }
            }
            else
            {
                // Calling BeginTransaction on an already open transaction is invalid
                throw new InvalidOperationException("Cannot begin a transaction while already in a transaction.");
            }
        }

        /// <summary>
        /// Creates a savepoint in the database at the current point in the transaction timeline.
        /// Begins a new transaction if one is not in progress.
        ///
        /// Call <see cref="RollbackTo"/> to undo transactions since the returned savepoint.
        /// Call <see cref="Release"/> to commit transactions after the savepoint returned here.
        /// Call <see cref="Commit"/> to end the transaction, committing all changes.
        /// </summary>
        /// <returns>A string naming the savepoint.</returns>
        public string SaveTransactionPoint()
        {
            int depth = Interlocked.Increment(ref this._transactionDepth) - 1;
            string retVal = "S" + this._rand.Next(short.MaxValue) + "D" + depth;

            try
            {
                this.Execute("savepoint " + retVal);
            }
            catch (Exception ex)
            {
                var sqlExp = ex as SQLiteException;
                if (sqlExp != null)
                {
                    // It is recommended that applications respond to the errors listed below
                    // by explicitly issuing a ROLLBACK command.
                    // TODO: This rollback failsafe should be localized to all throw sites.
                    switch (sqlExp.Result)
                    {
                        case SQLite3.Result.IOError:
                        case SQLite3.Result.Full:
                        case SQLite3.Result.Busy:
                        case SQLite3.Result.NoMem:
                        case SQLite3.Result.Interrupt:
                            this.RollbackTo(null, true);
                            break;
                    }
                }
                else
                {
                    Interlocked.Decrement(ref this._transactionDepth);
                }

                throw;
            }

            return retVal;
        }

        /// <summary>
        /// Rolls back the transaction that was begun by <see cref="BeginTransaction"/> or <see cref="SaveTransactionPoint"/>.
        /// </summary>
        public void Rollback()
        {
            this.RollbackTo(null, false);
        }

        /// <summary>
        /// Rolls back the savepoint created by <see cref="BeginTransaction"/> or SaveTransactionPoint.
        /// </summary>
        /// <param name="savepoint">
        /// The name of the savepoint to roll back to, as returned by <see cref="SaveTransactionPoint"/>.  If savepoint is null or empty, this method is equivalent to a call to <see cref="Rollback"/>
        /// </param>
        public void RollbackTo(string savepoint)
        {
            this.RollbackTo(savepoint, false);
        }

        /// <summary>
        /// Rolls back the transaction that was begun by <see cref="BeginTransaction"/>.
        /// </summary>
        /// <param name="savepoint">
        /// The savepoint.
        /// </param>
        /// <param name="noThrow">
        /// true to avoid throwing exceptions, false otherwise
        /// </param>
        void RollbackTo(string savepoint, bool noThrow)
        {
            // Rolling back without a TO clause rolls backs all transactions
            // and leaves the transaction stack empty.
            try
            {
                if (string.IsNullOrEmpty(savepoint))
                {
                    if (Interlocked.Exchange(ref this._transactionDepth, 0) > 0)
                    {
                        this.Execute("rollback");
                    }
                }
                else
                {
                    this.DoSavePointExecute(savepoint, "rollback to ");
                }
            }
            catch (SQLiteException)
            {
                if (!noThrow)
                {
                    throw;
                }
            }

            // No need to rollback if there are no transactions open.
        }

        /// <summary>
        /// Releases a savepoint returned from <see cref="SaveTransactionPoint"/>.  Releasing a savepoint
        ///    makes changes since that savepoint permanent if the savepoint began the transaction,
        ///    or otherwise the changes are permanent pending a call to <see cref="Commit"/>.
        ///
        /// The RELEASE command is like a COMMIT for a SAVEPOINT.
        /// </summary>
        /// <param name="savepoint">
        /// The name of the savepoint to release.  The string should be the result of a call to <see cref="SaveTransactionPoint"/>
        /// </param>
        public void Release(string savepoint)
        {
            this.DoSavePointExecute(savepoint, "release ");
        }

        /// <summary>
        /// The do save point execute.
        /// </summary>
        /// <param name="savepoint">
        /// The savepoint.
        /// </param>
        /// <param name="cmd">
        /// The cmd.
        /// </param>
        protected virtual void DoSavePointExecute(string savepoint, string cmd)
        {
            // Validate the savepoint
            int firstLen = savepoint.IndexOf('D');
            if (firstLen >= 2 && savepoint.Length > firstLen + 1)
            {
                int depth;
                if (int.TryParse(savepoint.Substring(firstLen + 1), out depth))
                {
                    // TODO: Mild race here, but inescapable without locking almost everywhere.
                    if (0 <= depth && depth < this._transactionDepth)
                    {
#if NETFX_CORE || USE_SQLITEPCL_RAW
                        Volatile.Write(ref _transactionDepth, depth);
#elif SILVERLIGHT
                        this._transactionDepth = depth;
#else
                        Thread.VolatileWrite (ref _transactionDepth, depth);
#endif
                        this.Execute(cmd + savepoint);
                        return;
                    }
                }
            }

            throw new ArgumentException(
                "savePoint is not valid, and should be the result of a call to SaveTransactionPoint.",
                "savePoint");
        }

        /// <summary>
        /// Commits the transaction that was begun by <see cref="BeginTransaction"/>.
        /// </summary>
        public virtual void Commit()
        {
            if (Interlocked.Exchange(ref this._transactionDepth, 0) != 0)
            {
                this.Execute("commit");
            }

            // Do nothing on a commit with no open transaction
        }

        /// <summary>
        /// The run in transaction.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        public virtual void RunInTransaction(Action action)
        {
            try
            {
                var savePoint = this.SaveTransactionPoint();
                action();
                this.Release(savePoint);
            }
            catch (Exception)
            {
                this.Rollback();
                throw;
            }
        }

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
        public int InsertAll(IEnumerable objects, bool runInTransaction = true)
        {
            var c = 0;
            if (runInTransaction)
            {
                this.RunInTransaction(
                    () =>
                        {
                            foreach (var r in objects)
                            {
                                c += this.Insert(r);
                            }
                        });
            }
            else
            {
                foreach (var r in objects)
                {
                    c += this.Insert(r);
                }
            }

            return c;
        }

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
        public int InsertAll(IEnumerable objects, string extra, bool runInTransaction = true)
        {
            var c = 0;
            if (runInTransaction)
            {
                this.RunInTransaction(
                    () =>
                        {
                            foreach (var r in objects)
                            {
                                c += this.Insert(r, extra);
                            }
                        });
            }
            else
            {
                foreach (var r in objects)
                {
                    c += this.Insert(r);
                }
            }

            return c;
        }

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
        public int InsertAll(IEnumerable objects, Type objType, bool runInTransaction = true)
        {
            var c = 0;
            if (runInTransaction)
            {
                this.RunInTransaction(
                    () =>
                        {
                            foreach (var r in objects)
                            {
                                c += this.Insert(r, objType);
                            }
                        });
            }
            else
            {
                foreach (var r in objects)
                {
                    c += this.Insert(r, objType);
                }
            }

            return c;
        }

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
        public int Insert(object obj)
        {
            if (obj == null)
            {
                return 0;
            }

            return this.Insert(obj, string.Empty, obj.GetType());
        }

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
        public int InsertOrReplace(object obj)
        {
            if (obj == null)
            {
                return 0;
            }

            return this.Insert(obj, "OR REPLACE", obj.GetType());
        }

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
        public int Insert(object obj, Type objType)
        {
            return this.Insert(obj, string.Empty, objType);
        }

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
        public int InsertOrReplace(object obj, Type objType)
        {
            return this.Insert(obj, "OR REPLACE", objType);
        }

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
        public virtual int Insert(object obj, string extra)
        {
            if (obj == null)
            {
                return 0;
            }

            return this.Insert(obj, extra, obj.GetType());
        }

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
        public int Insert(object obj, string extra, Type objType)
        {
            if (obj == null || objType == null)
            {
                return 0;
            }

            var map = this.GetMapping(objType);

#if USE_NEW_REFLECTION_API
            if (map.PK != null && map.PK.IsAutoGuid)
            {
                // no GetProperty so search our way up the inheritance chain till we find it
                PropertyInfo prop;
                while (objType != null)
                {
                    var info = objType.GetTypeInfo();
                    prop = info.GetDeclaredProperty(map.PK.PropertyName);
                    if (prop != null)
                    {
                        if (prop.GetValue(obj, null).Equals(Guid.Empty))
                        {
                            prop.SetValue(obj, Guid.NewGuid(), null);
                        }

                        break;
                    }

                    objType = info.BaseType;
                }
            }

#else
            if (map.PK != null && map.PK.IsAutoGuid) {
                var prop = objType.GetProperty(map.PK.PropertyName);
                if (prop != null) {
                    if (prop.GetValue(obj, null).Equals(Guid.Empty)) {
                        prop.SetValue(obj, Guid.NewGuid(), null);
                    }
                }
            }
#endif

            var replacing = string.Compare(extra, "OR REPLACE", StringComparison.OrdinalIgnoreCase) == 0;

            var cols = replacing ? map.InsertOrReplaceColumns : map.InsertColumns;
            var vals = new object[cols.Length];
            for (var i = 0; i < vals.Length; i++)
            {
                vals[i] = cols[i].GetValue(obj);
            }

            var insertCmd = map.GetInsertCommand(this, extra);
            int count;

            lock (insertCmd)
            {
                // We lock here to protect the prepared statement returned via GetInsertCommand.
                // A SQLite prepared statement can be bound for only one operation at a time.
                try
                {
                    count = insertCmd.ExecuteNonQuery(vals);
                }
                catch (SQLiteException ex)
                {
                    if (SQLite3.ExtendedErrCode(this.Handle) == SQLite3.ExtendedResult.ConstraintNotNull)
                    {
                        throw NotNullConstraintViolationException.New(ex.Result, ex.Message, map, obj);
                    }

                    throw;
                }

                if (map.HasAutoIncPK)
                {
                    var id = SQLite3.LastInsertRowid(this.Handle);
                    map.SetAutoIncPK(obj, id);
                }
            }

            if (count > 0)
            {
                this.OnTableChanged(map, NotifyTableChangedAction.Insert);
            }

            return count;
        }

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
        public int Update(object obj)
        {
            if (obj == null)
            {
                return 0;
            }

            return this.Update(obj, obj.GetType());
        }

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
        public virtual int Update(object obj, Type objType)
        {
            int rowsAffected = 0;
            if (obj == null || objType == null)
            {
                return 0;
            }

            var map = this.GetMapping(objType);

            var pk = map.PK;

            if (pk == null)
            {
                throw new NotSupportedException("Cannot update " + map.TableName + ": it has no PK");
            }

            var cols = from p in map.Columns where p != pk select p;
            var vals = from c in cols select c.GetValue(obj);
            var ps = new List<object>(vals);
            ps.Add(pk.GetValue(obj));
            var q = string.Format(
                "update \"{0}\" set {1} where {2} = ? ",
                map.TableName,
                string.Join(",", (from c in cols select "\"" + c.Name + "\" = ? ").ToArray()),
                pk.Name);

            try
            {
                rowsAffected = this.Execute(q, ps.ToArray());
            }
            catch (SQLiteException ex)
            {
                if (ex.Result == SQLite3.Result.Constraint
                    && SQLite3.ExtendedErrCode(this.Handle) == SQLite3.ExtendedResult.ConstraintNotNull)
                {
                    throw NotNullConstraintViolationException.New(ex, map, obj);
                }

                throw ex;
            }

            if (rowsAffected > 0)
            {
                this.OnTableChanged(map, NotifyTableChangedAction.Update);
            }

            return rowsAffected;
        }

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
        public int UpdateAll(IEnumerable objects, bool runInTransaction = true)
        {
            var c = 0;
            if (runInTransaction)
            {
                this.RunInTransaction(
                    () =>
                        {
                            foreach (var r in objects)
                            {
                                c += this.Update(r);
                            }
                        });
            }
            else
            {
                foreach (var r in objects)
                {
                    c += this.Update(r);
                }
            }

            return c;
        }

        /// <summary>
        /// Deletes the given object from the database using its primary key.
        /// </summary>
        /// <param name="objectToDelete">
        /// The object to delete. It must have a primary key designated using the PrimaryKeyAttribute.
        /// </param>
        /// <returns>
        /// The number of rows deleted.
        /// </returns>
        public int Delete(object objectToDelete)
        {
            var map = this.GetMapping(objectToDelete.GetType());
            var pk = map.PK;
            if (pk == null)
            {
                throw new NotSupportedException("Cannot delete " + map.TableName + ": it has no PK");
            }

            var q = string.Format("delete from \"{0}\" where \"{1}\" = ?", map.TableName, pk.Name);
            var count = this.Execute(q, pk.GetValue(objectToDelete));
            if (count > 0)
            {
                this.OnTableChanged(map, NotifyTableChangedAction.Delete);
            }

            return count;
        }

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
        public virtual int Delete<T>(object primaryKey)
        {
            var map = this.GetMapping(typeof(T));
            var pk = map.PK;
            if (pk == null)
            {
                throw new NotSupportedException("Cannot delete " + map.TableName + ": it has no PK");
            }

            var q = string.Format("delete from \"{0}\" where \"{1}\" = ?", map.TableName, pk.Name);
            var count = this.Execute(q, primaryKey);
            if (count > 0)
            {
                this.OnTableChanged(map, NotifyTableChangedAction.Delete);
            }

            return count;
        }

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
        public virtual int DeleteAll<T>()
        {
            var map = this.GetMapping(typeof(T));
            var query = string.Format("delete from \"{0}\"", map.TableName);
            var count = this.Execute(query);
            if (count > 0)
            {
                this.OnTableChanged(map, NotifyTableChangedAction.Delete);
            }

            return count;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="SQLiteConnection"/> class.
        /// </summary>
        ~SQLiteConnection()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The close.
        /// </summary>
        public void Close()
        {
            SQLiteException lastDisposeException = null;
            do
            {
                try
                {
                    this.Dispose(true);
                    lastDisposeException = null;
                }
                catch (SQLiteException disposeException)
                {
                    lastDisposeException = disposeException;
                }
            }
            while (lastDisposeException != null);
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        /// <exception cref="SQLiteException">
        /// </exception>
        protected virtual void Dispose(bool disposing)
        {
            if (this._open && this.Handle != NullHandle)
            {
                if (disposing)
                {
                    if (this._mappings != null)
                    {
                        foreach (var sqlInsertCommand in this._mappings.Values)
                        {
                            sqlInsertCommand.Dispose();
                        }
                    }

                    var r = SQLite3.Close(this.Handle);
                    if (r != SQLite3.Result.OK)
                    {
                        string errorMessage = SQLite3.GetErrmsg(this.Handle);
                        Debug.WriteLine($"Exception was thrown :{errorMessage}");

                        throw SQLiteException.New(r, errorMessage);
                    }

                    this.Handle = NullHandle;
                    this._open = false;
                }
                else
                {
                    SQLite3.Close2(this.Handle);
                }
            }
        }

        /// <summary>
        /// The on table changed.
        /// </summary>
        /// <param name="table">
        /// The table.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        void OnTableChanged(TableMapping table, NotifyTableChangedAction action)
        {
            var ev = this.TableChanged;
            if (ev != null)
            {
                ev(this, new NotifyTableChangedEventArgs(table, action));
            }
        }

        /// <summary>
        /// The table changed.
        /// </summary>
        public event EventHandler<NotifyTableChangedEventArgs> TableChanged;
    }

    /// <summary>
    /// The notify table changed event args.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class NotifyTableChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyTableChangedEventArgs"/> class.
        /// </summary>
        /// <param name="table">
        /// The table.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        public NotifyTableChangedEventArgs(TableMapping table, NotifyTableChangedAction action)
        {
            this.Table = table;
            this.Action = action;
        }

        /// <summary>
        /// Gets the action.
        /// </summary>
        public NotifyTableChangedAction Action { get; private set; }

        /// <summary>
        /// Gets the table.
        /// </summary>
        public TableMapping Table { get; private set; }
    }

    /// <summary>
    /// The notify table changed action.
    /// </summary>
    public enum NotifyTableChangedAction
    {
        /// <summary>
        /// The insert.
        /// </summary>
        Insert,

        /// <summary>
        /// The update.
        /// </summary>
        Update,

        /// <summary>
        /// The delete.
        /// </summary>
        Delete,
    }

    /// <summary>
    /// Represents a parsed connection string.
    /// </summary>
    [ExcludeFromCodeCoverage]
    class SQLiteConnectionString
    {
        /// <summary>
        /// Gets the connection string.
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// Gets the database path.
        /// </summary>
        public string DatabasePath { get; private set; }

        /// <summary>
        /// Gets a value indicating whether store date time as ticks.
        /// </summary>
        public bool StoreDateTimeAsTicks { get; private set; }

#if NETFX_CORE
        static readonly string MetroStyleDataPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteConnectionString"/> class.
        /// </summary>
        /// <param name="databasePath">
        /// The database path.
        /// </param>
        /// <param name="storeDateTimeAsTicks">
        /// The store date time as ticks.
        /// </param>
        public SQLiteConnectionString(string databasePath, bool storeDateTimeAsTicks)
        {
            this.ConnectionString = databasePath;
            this.StoreDateTimeAsTicks = storeDateTimeAsTicks;

#if NETFX_CORE
            DatabasePath = System.IO.Path.Combine(MetroStyleDataPath, databasePath);
#else
            this.DatabasePath = databasePath;
#endif
        }
    }

    /// <summary>
    /// The table attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [ExcludeFromCodeCoverage]
    public class TableAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableAttribute"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public TableAttribute(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// The column attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    [ExcludeFromCodeCoverage]
    public class ColumnAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnAttribute"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public ColumnAttribute(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// The primary key attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    [ExcludeFromCodeCoverage]
    public class PrimaryKeyAttribute : Attribute
    {
    }

    /// <summary>
    /// The auto increment attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    [ExcludeFromCodeCoverage]
    public class AutoIncrementAttribute : Attribute
    {
    }

    /// <summary>
    /// The indexed attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    [ExcludeFromCodeCoverage]
    public class IndexedAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexedAttribute"/> class.
        /// </summary>
        public IndexedAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexedAttribute"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="order">
        /// The order.
        /// </param>
        public IndexedAttribute(string name, int order)
        {
            this.Name = name;
            this.Order = order;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether unique.
        /// </summary>
        public virtual bool Unique { get; set; }
    }

    /// <summary>
    /// The ignore attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    [ExcludeFromCodeCoverage]
    public class IgnoreAttribute : Attribute
    {
    }

    /// <summary>
    /// The unique attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    [ExcludeFromCodeCoverage]
    public class UniqueAttribute : IndexedAttribute
    {
        /// <summary>
        /// Gets or sets a value indicating whether unique.
        /// </summary>
        public override bool Unique
        {
            get
            {
                return true;
            }

            set
            {
                /* throw?  */
            }
        }
    }

    /// <summary>
    /// The max length attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    [ExcludeFromCodeCoverage]
    public class MaxLengthAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaxLengthAttribute"/> class.
        /// </summary>
        /// <param name="length">
        /// The length.
        /// </param>
        public MaxLengthAttribute(int length)
        {
            this.Value = length;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public int Value { get; private set; }
    }

    /// <summary>
    /// The collation attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    [ExcludeFromCodeCoverage]
    public class CollationAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollationAttribute"/> class.
        /// </summary>
        /// <param name="collation">
        /// The collation.
        /// </param>
        public CollationAttribute(string collation)
        {
            this.Value = collation;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public string Value { get; private set; }
    }

    /// <summary>
    /// The not null attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    [ExcludeFromCodeCoverage]
    public class NotNullAttribute : Attribute
    {
    }

    /// <summary>
    /// The table mapping.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class TableMapping
    {
        /// <summary>
        /// The _auto pk.
        /// </summary>
        readonly Column _autoPk;

        /// <summary>
        /// The _insert columns.
        /// </summary>
        Column[] _insertColumns;

        /// <summary>
        /// The _insert command map.
        /// </summary>
        ConcurrentStringDictionary _insertCommandMap;

        /// <summary>
        /// The _insert or replace columns.
        /// </summary>
        Column[] _insertOrReplaceColumns;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableMapping"/> class.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="createFlags">
        /// The create flags.
        /// </param>
        public TableMapping(Type type, CreateFlags createFlags = CreateFlags.None)
        {
            this.MappedType = type;

#if USE_NEW_REFLECTION_API
            var tableAttr =
                (TableAttribute)
                CustomAttributeExtensions.GetCustomAttribute(type.GetTypeInfo(), typeof(TableAttribute), true);
#else
			var tableAttr = (TableAttribute)type.GetCustomAttributes (typeof (TableAttribute), true).FirstOrDefault ();
#endif

            this.TableName = tableAttr != null ? tableAttr.Name : this.MappedType.Name;

#if !USE_NEW_REFLECTION_API
			var props = MappedType.GetProperties (BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
#else
            var props = from p in this.MappedType.GetRuntimeProperties()
                        where
                            (p.GetMethod != null && p.GetMethod.IsPublic)
                             || (p.SetMethod != null && p.SetMethod.IsPublic)
                             || (p.GetMethod != null && p.GetMethod.IsStatic)
                             || (p.SetMethod != null && p.SetMethod.IsStatic)
                        select p;
#endif
            var cols = new List<Column>();
            foreach (var p in props)
            {
#if !USE_NEW_REFLECTION_API
				var ignore = p.GetCustomAttributes (typeof(IgnoreAttribute), true).Length > 0;
#else
                var ignore = p.GetCustomAttributes(typeof(IgnoreAttribute), true).Count() > 0;
#endif
                if (p.CanWrite && !ignore)
                {
                    cols.Add(new Column(p, createFlags));
                }
            }

            this.Columns = cols.ToArray();
            foreach (var c in this.Columns)
            {
                if (c.IsAutoInc && c.IsPK)
                {
                    this._autoPk = c;
                }

                if (c.IsPK)
                {
                    this.PK = c;
                }
            }

            this.HasAutoIncPK = this._autoPk != null;

            if (this.PK != null)
            {
                this.GetByPrimaryKeySql = string.Format(
                    "select * from \"{0}\" where \"{1}\" = ?",
                    this.TableName,
                    this.PK.Name);
            }
            else
            {
                // People should not be calling Get/Find without a PK
                this.GetByPrimaryKeySql = string.Format("select * from \"{0}\" limit 1", this.TableName);
            }

            this._insertCommandMap = new ConcurrentStringDictionary();
        }

        /// <summary>
        /// Gets the columns.
        /// </summary>
        public Column[] Columns { get; private set; }

        /// <summary>
        /// Gets the get by primary key sql.
        /// </summary>
        public string GetByPrimaryKeySql { get; private set; }

        /// <summary>
        /// Gets a value indicating whether has auto inc pk.
        /// </summary>
        public bool HasAutoIncPK { get; private set; }

        /// <summary>
        /// Gets the insert columns.
        /// </summary>
        public Column[] InsertColumns
        {
            get
            {
                if (this._insertColumns == null)
                {
                    this._insertColumns = this.Columns.Where(c => !c.IsAutoInc).ToArray();
                }

                return this._insertColumns;
            }
        }

        /// <summary>
        /// Gets the insert or replace columns.
        /// </summary>
        public Column[] InsertOrReplaceColumns
        {
            get
            {
                if (this._insertOrReplaceColumns == null)
                {
                    this._insertOrReplaceColumns = this.Columns.ToArray();
                }

                return this._insertOrReplaceColumns;
            }
        }

        /// <summary>
        /// Gets the mapped type.
        /// </summary>
        public Type MappedType { get; private set; }

        /// <summary>
        /// Gets the pk.
        /// </summary>
        public Column PK { get; private set; }

        /// <summary>
        /// Gets the table name.
        /// </summary>
        public string TableName { get; private set; }

        /// <summary>
        /// The find column.
        /// </summary>
        /// <param name="columnName">
        /// The column name.
        /// </param>
        /// <returns>
        /// The <see cref="Column"/>.
        /// </returns>
        public Column FindColumn(string columnName)
        {
            var exact = this.Columns.FirstOrDefault(c => c.Name == columnName);
            return exact;
        }

        /// <summary>
        /// The find column with property name.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <returns>
        /// The <see cref="Column"/>.
        /// </returns>
        public Column FindColumnWithPropertyName(string propertyName)
        {
            var exact = this.Columns.FirstOrDefault(c => c.PropertyName == propertyName);
            return exact;
        }

        /// <summary>
        /// The get insert command.
        /// </summary>
        /// <param name="conn">
        /// The conn.
        /// </param>
        /// <param name="extra">
        /// The extra.
        /// </param>
        /// <returns>
        /// The <see cref="PreparedSqlLiteInsertCommand"/>.
        /// </returns>
        public PreparedSqlLiteInsertCommand GetInsertCommand(SQLiteConnection conn, string extra)
        {
            object prepCmdO;

            if (!this._insertCommandMap.TryGetValue(extra, out prepCmdO))
            {
                var prepCmd = this.CreateInsertCommand(conn, extra);
                prepCmdO = prepCmd;
                if (!this._insertCommandMap.TryAdd(extra, prepCmd))
                {
                    // Concurrent add attempt beat us.
                    prepCmd.Dispose();
                    this._insertCommandMap.TryGetValue(extra, out prepCmdO);
                }
            }

            return (PreparedSqlLiteInsertCommand)prepCmdO;
        }

        /// <summary>
        /// The set auto inc pk.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        public void SetAutoIncPK(object obj, long id)
        {
            if (this._autoPk != null)
            {
                this._autoPk.SetValue(obj, Convert.ChangeType(id, this._autoPk.ColumnType, null));
            }
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        protected internal void Dispose()
        {
            foreach (var pair in this._insertCommandMap)
            {
                ((PreparedSqlLiteInsertCommand)pair.Value).Dispose();
            }

            this._insertCommandMap = null;
        }

        /// <summary>
        /// The create insert command.
        /// </summary>
        /// <param name="conn">
        /// The conn.
        /// </param>
        /// <param name="extra">
        /// The extra.
        /// </param>
        /// <returns>
        /// The <see cref="PreparedSqlLiteInsertCommand"/>.
        /// </returns>
        PreparedSqlLiteInsertCommand CreateInsertCommand(SQLiteConnection conn, string extra)
        {
            var cols = this.InsertColumns;
            string insertSql;
            if (!cols.Any() && this.Columns.Count() == 1 && this.Columns[0].IsAutoInc)
            {
                insertSql = string.Format("insert {1} into \"{0}\" default values", this.TableName, extra);
            }
            else
            {
                var replacing = string.Compare(extra, "OR REPLACE", StringComparison.OrdinalIgnoreCase) == 0;

                if (replacing)
                {
                    cols = this.InsertOrReplaceColumns;
                }

                insertSql = string.Format(
                    "insert {3} into \"{0}\"({1}) values ({2})",
                    this.TableName,
                    string.Join(",", (from c in cols select "\"" + c.Name + "\"").ToArray()),
                    string.Join(",", (from c in cols select "?").ToArray()),
                    extra);
            }

            var insertCommand = new PreparedSqlLiteInsertCommand(conn);
            insertCommand.CommandText = insertSql;
            return insertCommand;
        }

        /// <summary>
        /// The column.
        /// </summary>
        public class Column
        {
            /// <summary>
            /// The _prop.
            /// </summary>
            readonly PropertyInfo _prop;

            /// <summary>
            /// Initializes a new instance of the <see cref="Column"/> class.
            /// </summary>
            /// <param name="prop">
            /// The prop.
            /// </param>
            /// <param name="createFlags">
            /// The create flags.
            /// </param>
            public Column(PropertyInfo prop, CreateFlags createFlags = CreateFlags.None)
            {
                var colAttr = (ColumnAttribute)prop.GetCustomAttributes(typeof(ColumnAttribute), true).FirstOrDefault();

                this._prop = prop;
                this.Name = colAttr == null ? prop.Name : colAttr.Name;

                // If this type is Nullable<T> then Nullable.GetUnderlyingType returns the T, otherwise it returns null, so get the actual type instead
                this.ColumnType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                this.Collation = Orm.Collation(prop);

                this.IsPK = Orm.IsPK(prop)
                            || (((createFlags & CreateFlags.ImplicitPK) == CreateFlags.ImplicitPK)
                                && string.Compare(prop.Name, Orm.ImplicitPkName, StringComparison.OrdinalIgnoreCase)
                                == 0);

                var isAuto = Orm.IsAutoInc(prop)
                             || (this.IsPK && ((createFlags & CreateFlags.AutoIncPK) == CreateFlags.AutoIncPK));
                this.IsAutoGuid = isAuto && this.ColumnType == typeof(Guid);
                this.IsAutoInc = isAuto && !this.IsAutoGuid;

                this.Indices = Orm.GetIndices(prop);
                if (!this.Indices.Any() && !this.IsPK
                    && ((createFlags & CreateFlags.ImplicitIndex) == CreateFlags.ImplicitIndex)
                    && this.Name.EndsWith(Orm.ImplicitIndexSuffix, StringComparison.OrdinalIgnoreCase))
                {
                    this.Indices = new[] { new IndexedAttribute() };
                }

                this.IsNullable = !(this.IsPK || Orm.IsMarkedNotNull(prop));
                this.MaxStringLength = Orm.MaxStringLength(prop);
            }

            /// <summary>
            /// Gets the collation.
            /// </summary>
            public string Collation { get; private set; }

            /// <summary>
            /// Gets the column type.
            /// </summary>
            public Type ColumnType { get; private set; }

            /// <summary>
            /// Gets or sets the indices.
            /// </summary>
            public IEnumerable<IndexedAttribute> Indices { get; set; }

            /// <summary>
            /// Gets a value indicating whether is auto guid.
            /// </summary>
            public bool IsAutoGuid { get; private set; }

            /// <summary>
            /// Gets a value indicating whether is auto inc.
            /// </summary>
            public bool IsAutoInc { get; private set; }

            /// <summary>
            /// Gets a value indicating whether is nullable.
            /// </summary>
            public bool IsNullable { get; private set; }

            /// <summary>
            /// Gets a value indicating whether is pk.
            /// </summary>
            public bool IsPK { get; private set; }

            /// <summary>
            /// Gets the max string length.
            /// </summary>
            public int? MaxStringLength { get; private set; }

            /// <summary>
            /// Gets the name.
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            /// Gets the property name.
            /// </summary>
            public string PropertyName
            {
                get
                {
                    return this._prop.Name;
                }
            }

            /// <summary>
            /// The get value.
            /// </summary>
            /// <param name="obj">
            /// The obj.
            /// </param>
            /// <returns>
            /// The <see cref="object"/>.
            /// </returns>
            public object GetValue(object obj)
            {
                return this._prop.GetValue(obj, null);
            }

            /// <summary>
            /// The set value.
            /// </summary>
            /// <param name="obj">
            /// The obj.
            /// </param>
            /// <param name="val">
            /// The val.
            /// </param>
            public void SetValue(object obj, object val)
            {
                this._prop.SetValue(obj, val, null);
            }
        }
    }

    /// <summary>
    /// The orm.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class Orm
    {
        /// <summary>
        /// The default max string length.
        /// </summary>
        public const int DefaultMaxStringLength = 140;

        /// <summary>
        /// The implicit index suffix.
        /// </summary>
        public const string ImplicitIndexSuffix = "Id";

        /// <summary>
        /// The implicit pk name.
        /// </summary>
        public const string ImplicitPkName = "Id";

        /// <summary>
        /// The collation.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Collation(MemberInfo p)
        {
            var attrs = p.GetCustomAttributes(typeof(CollationAttribute), true);
#if !USE_NEW_REFLECTION_API
			if (attrs.Length > 0) {
				return ((CollationAttribute)attrs [0]).Value;
#else
            if (attrs.Count() > 0)
            {
                return ((CollationAttribute)attrs.First()).Value;
#endif
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// The get indices.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public static IEnumerable<IndexedAttribute> GetIndices(MemberInfo p)
        {
            var attrs = p.GetCustomAttributes(typeof(IndexedAttribute), true);
            return attrs.Cast<IndexedAttribute>();
        }

        /// <summary>
        /// The is auto inc.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsAutoInc(MemberInfo p)
        {
            var attrs = p.GetCustomAttributes(typeof(AutoIncrementAttribute), true);
            return attrs.Any();
        }

        /// <summary>
        /// The is marked not null.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsMarkedNotNull(MemberInfo p)
        {
            var attrs = p.GetCustomAttributes(typeof(NotNullAttribute), true);
            return attrs.Any();
        }

        /// <summary>
        /// The is pk.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsPK(MemberInfo p)
        {
            var attrs = p.GetCustomAttributes(typeof(PrimaryKeyAttribute), true);
            return attrs.Any();
        }

        /// <summary>
        /// The max string length.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <returns>
        /// The <see cref="Nullable{T}"/> with <c>T</c> being <see cref="int"/>
        /// </returns>
        public static int? MaxStringLength(PropertyInfo p)
        {
            var attrs = p.GetCustomAttributes(typeof(MaxLengthAttribute), true);
            return ((MaxLengthAttribute)attrs.FirstOrDefault())?.Value;
        }

        /// <summary>
        /// The sql decl.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <param name="storeDateTimeAsTicks">
        /// The store date time as ticks.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string SqlDecl(TableMapping.Column p, bool storeDateTimeAsTicks)
        {
            string decl = "\"" + p.Name + "\" " + SqlType(p, storeDateTimeAsTicks) + " ";

            if (p.IsPK)
            {
                decl += "primary key ";
            }

            if (p.IsAutoInc)
            {
                decl += "autoincrement ";
            }

            if (!p.IsNullable)
            {
                decl += "not null ";
            }

            if (!string.IsNullOrEmpty(p.Collation))
            {
                decl += "collate " + p.Collation + " ";
            }

            return decl;
        }

        /// <summary>
        /// The sql type.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <param name="storeDateTimeAsTicks">
        /// The store date time as ticks.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public static string SqlType(TableMapping.Column p, bool storeDateTimeAsTicks)
        {
            var clrType = p.ColumnType;
            if (clrType == typeof(bool) || clrType == typeof(byte) || clrType == typeof(UInt16)
                || clrType == typeof(SByte) || clrType == typeof(Int16) || clrType == typeof(Int32)
                || clrType == typeof(UInt32) || clrType == typeof(Int64))
            {
                return "integer";
            }

            if (clrType == typeof(Single) || clrType == typeof(double) || clrType == typeof(decimal))
            {
                return "float";
            }

            if (clrType == typeof(string))
            {
                int? len = p.MaxStringLength;

                if (len.HasValue)
                {
                    return "varchar(" + len.Value + ")";
                }

                return "varchar";
            }

            if (clrType == typeof(TimeSpan))
            {
                return "bigint";
            }

            if (clrType == typeof(DateTime))
            {
                return storeDateTimeAsTicks ? "bigint" : "datetime";
            }

            if (clrType == typeof(DateTimeOffset))
            {
                return "bigint";
            }

            if (clrType.GetTypeInfo().IsEnum)
            {
                return "integer";
            }

            if (clrType == typeof(byte[]))
            {
                return "blob";
            }

            if (clrType == typeof(Guid))
            {
                return "varchar(36)";
            }

            throw new NotSupportedException("Don't know about " + clrType);
        }
    }

    /// <summary>
    /// The sq lite command.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class SQLiteCommand
    {
        private static bool? logResults = Logger?.LogSettings?.LogFlags.HasFlag(LogFlag.LogResults);

        /// <summary>
        /// Gets logging interface
        /// </summary>
        public static ILogger Logger => SimpleIoc.Default.GetInstance<ILogger>();

        /// <summary>
        /// The negative pointer.
        /// </summary>
        internal static IntPtr NegativePointer = new IntPtr(-1);

        /// <summary>
        /// The _bindings.
        /// </summary>
        private readonly List<Binding> _bindings;

        /// <summary>
        /// The _conn.
        /// </summary>
        readonly SQLiteConnection _conn;

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteCommand"/> class.
        /// </summary>
        /// <param name="conn">
        /// The conn.
        /// </param>
        internal SQLiteCommand(SQLiteConnection conn)
        {
            this._conn = conn;
            this._bindings = new List<Binding>();
            this.CommandText = string.Empty;
        }

        /// <summary>
        /// Gets or sets the command text.
        /// </summary>
        public string CommandText { get; set; }

        /// <summary>
        /// The bind.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="val">
        /// The val.
        /// </param>
        public void Bind(string name, object val)
        {
            this._bindings.Add(new Binding { Name = name, Value = val });
        }

        /// <summary>
        /// The bind.
        /// </summary>
        /// <param name="val">
        /// The val.
        /// </param>
        public void Bind(object val)
        {
            this.Bind(null, val);
        }

        /// <summary>
        /// Clears the bindings
        /// </summary>
        public void ClearBindings()
        {
            this._bindings.Clear();
        }

        /// <summary>
        /// The execute deferred query.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<T> ExecuteDeferredQuery<T>()
        {
            return this.ExecuteDeferredQuery<T>(this._conn.GetMapping(typeof(T)));
        }

        /// <summary>
        /// The execute deferred query.
        /// </summary>
        /// <param name="map">
        /// The map.
        /// </param>
        /// <typeparam name="T">type of object
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public virtual IEnumerable<T> ExecuteDeferredQuery<T>(TableMapping map)
        {
            Logger.LogDebug("Executing Query: " + this, LogFlag.LogStatements);
            IntPtr stmt = IntPtr.Zero;
            try
            {
                stmt = this.Prepare();
                var cols = new TableMapping.Column[SQLite3.ColumnCount(stmt)];

                for (int i = 0; i < cols.Length; i++)
                {
                    var name = SQLite3.ColumnName16(stmt, i);
                    cols[i] = map.FindColumn(name);
                }

                var logBuilder = new StringBuilder();
                while (SQLite3.Step(stmt) == SQLite3.Result.Row)
                {
                    var obj = Activator.CreateInstance(map.MappedType);
                    for (int i = 0; i < cols.Length; i++)
                    {
                        if (cols[i] == null)
                        {
                            continue;
                        }

                        var colType = SQLite3.ColumnType(stmt, i);
                        var val = this.ReadCol(stmt, i, colType, cols[i].ColumnType);
                        cols[i].SetValue(obj, val);
                    }

                    this.OnInstanceCreated(obj);
                    yield return (T)obj;
                }
            }
            finally
            {
                if (stmt != IntPtr.Zero)
                {
                    SQLite3.Finalize(stmt);
                }
            }
        }

        /// <summary>
        /// The execute non query.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        /// <exception cref="SQLiteException">
        /// </exception>
        /// <exception cref="NotNullConstraintViolationException">
        /// </exception>
        public int ExecuteNonQuery()
        {
            Logger.LogDebug("Executing Non Query: " + this, LogFlag.LogStatements);
            var r = SQLite3.Result.OK;

            IntPtr stmt = IntPtr.Zero;

            try
            {
                stmt = this.Prepare();
                r = SQLite3.Step(stmt);
            }
            finally
            {
                if (stmt != IntPtr.Zero)
                {
                    this.Finalize(stmt);
                }
            }

            if (r == SQLite3.Result.OK || r == SQLite3.Result.Done)
            {
                int rowsAffected = SQLite3.Changes(this._conn.Handle);
                return rowsAffected;
            }

            if (r == SQLite3.Result.Error)
            {
                string msg = SQLite3.GetErrmsg(this._conn.Handle);
                throw SQLiteException.New(r, msg);
            }

            if (r == SQLite3.Result.Constraint)
            {
                if (SQLite3.ExtendedErrCode(this._conn.Handle) == SQLite3.ExtendedResult.ConstraintNotNull)
                {
                    throw NotNullConstraintViolationException.New(r, SQLite3.GetErrmsg(this._conn.Handle));
                }
            }

            throw SQLiteException.New(r, r.ToString());
        }

        /// <summary>
        /// The execute query.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<T> ExecuteQuery<T>()
        {
            return this.ExecuteDeferredQuery<T>(this._conn.GetMapping(typeof(T))).ToList();
        }

        /// <summary>
        /// The execute query.
        /// </summary>
        /// <param name="map">
        /// The map.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<T> ExecuteQuery<T>(TableMapping map)
        {
            return this.ExecuteDeferredQuery<T>(map).ToList();
        }

        /// <summary>
        /// The execute scalar.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        /// <exception cref="SQLiteException">
        /// </exception>
        public T ExecuteScalar<T>()
        {
            Logger.LogDebug("Executing Scalar: " + this, LogFlag.LogStatements);
            T val = default(T);

            IntPtr stmt = IntPtr.Zero;

            try
            {
                stmt = this.Prepare();
                var r = SQLite3.Step(stmt);
                if (r == SQLite3.Result.Row)
                {
                    var colType = SQLite3.ColumnType(stmt, 0);
                    val = (T)this.ReadCol(stmt, 0, colType, typeof(T));
                }
                else if (r == SQLite3.Result.Done)
                {
                }
                else
                {
                    throw SQLiteException.New(r, SQLite3.GetErrmsg(this._conn.Handle));
                }
            }
            finally
            {
                if (stmt != IntPtr.Zero)
                {
                    this.Finalize(stmt);
                }
            }

            return val;
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            var parts = new string[1 + this._bindings.Count];
            parts[0] = this.CommandText;
            var i = 1;
            foreach (var b in this._bindings)
            {
                parts[i] = $"  {i - 1}: {b.Value}";
                i++;
            }

            return string.Join(",", parts);
        }

        /// <summary>
        /// The bind parameter.
        /// </summary>
        /// <param name="stmt">
        /// The stmt.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="storeDateTimeAsTicks">
        /// The store date time as ticks.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// </exception>
        internal static void BindParameter(IntPtr stmt, int index, object value, bool storeDateTimeAsTicks)
        {
            if (value == null)
            {
                SQLite3.BindNull(stmt, index);
            }
            else
            {
                if (value is Int32)
                {
                    SQLite3.BindInt(stmt, index, (int)value);
                }
                else if (value is String)
                {
                    SQLite3.BindText(stmt, index, (string)value, -1, NegativePointer);
                }
                else if (value is Byte || value is UInt16 || value is SByte || value is Int16)
                {
                    SQLite3.BindInt(stmt, index, Convert.ToInt32(value));
                }
                else if (value is Boolean)
                {
                    SQLite3.BindInt(stmt, index, (bool)value ? 1 : 0);
                }
                else if (value is UInt32 || value is Int64)
                {
                    SQLite3.BindInt64(stmt, index, Convert.ToInt64(value));
                }
                else if (value is Single || value is Double || value is Decimal)
                {
                    SQLite3.BindDouble(stmt, index, Convert.ToDouble(value));
                }
                else if (value is TimeSpan)
                {
                    SQLite3.BindInt64(stmt, index, ((TimeSpan)value).Ticks);
                }
                else if (value is DateTime)
                {
                    if (storeDateTimeAsTicks)
                    {
                        SQLite3.BindInt64(stmt, index, ((DateTime)value).Ticks);
                    }
                    else
                    {
                        SQLite3.BindText(
                            stmt,
                            index,
                            ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss"),
                            -1,
                            NegativePointer);
                    }
                }
                else if (value is DateTimeOffset)
                {
                    SQLite3.BindInt64(stmt, index, ((DateTimeOffset)value).UtcTicks);
#if !USE_NEW_REFLECTION_API
				} else if (value.GetType().IsEnum) {
#else
                }
                else if (value.GetType().GetTypeInfo().IsEnum)
                {
#endif
                    SQLite3.BindInt(stmt, index, Convert.ToInt32(value));
                }
                else if (value is byte[])
                {
                    SQLite3.BindBlob(stmt, index, (byte[])value, ((byte[])value).Length, NegativePointer);
                }
                else if (value is Guid)
                {
                    SQLite3.BindText(stmt, index, ((Guid)value).ToString(), 72, NegativePointer);
                }
                else
                {
                    throw new NotSupportedException("Cannot store type: " + value.GetType());
                }
            }
        }

        /// <summary>
        /// Invoked every time an instance is loaded from the database.
        /// </summary>
        /// <param name="obj">
        /// The newly created object.
        /// </param>
        /// <remarks>
        /// This can be overridden in combination with the <see cref="SQLiteConnection.NewCommand"/>
        /// method to hook into the life-cycle of objects.
        ///
        /// Type safety is not possible because MonoTouch does not support virtual generic methods.
        /// </remarks>
        protected virtual void OnInstanceCreated(object obj)
        {
            // Can be overridden.
            if (logResults.HasValue && logResults.Value)
            {
                Logger?.LogDebug($"Data Row of type {obj.GetType()} Created: {JsonConvert.SerializeObject(obj)}", LogFlag.LogResults);
            }
        }

        /// <summary>
        /// The bind all.
        /// </summary>
        /// <param name="stmt">
        /// The stmt.
        /// </param>
        void BindAll(IntPtr stmt)
        {
            int nextIdx = 1;
            foreach (var b in this._bindings)
            {
                if (b.Name != null)
                {
                    b.Index = SQLite3.BindParameterIndex(stmt, b.Name);
                }
                else if (b.Index == 0)
                {
                    b.Index = nextIdx++;
                }

                BindParameter(stmt, b.Index, b.Value, this._conn.StoreDateTimeAsTicks);
            }
        }

        /// <summary>
        /// The finalize.
        /// </summary>
        /// <param name="stmt">
        /// The stmt.
        /// </param>
        void Finalize(IntPtr stmt)
        {
            SQLite3.Finalize(stmt);
        }

        /// <summary>
        /// The prepare.
        /// </summary>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        IntPtr Prepare()
        {
            var stmt = SQLite3.Prepare2(this._conn.Handle, this.CommandText);
            this.BindAll(stmt);
            return stmt;
        }

        /// <summary>
        /// The read col.
        /// </summary>
        /// <param name="stmt">
        /// The stmt.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="clrType">
        /// The clr type.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        object ReadCol(IntPtr stmt, int index, SQLite3.ColType type, Type clrType)
        {
            if (type == SQLite3.ColType.Null)
            {
                return null;
            }
            else
            {
                if (clrType == typeof(String))
                {
                    return SQLite3.ColumnString(stmt, index);
                }
                else if (clrType == typeof(Int32))
                {
                    return (int)SQLite3.ColumnInt(stmt, index);
                }
                else if (clrType == typeof(Boolean))
                {
                    return SQLite3.ColumnInt(stmt, index) == 1;
                }
                else if (clrType == typeof(double))
                {
                    return SQLite3.ColumnDouble(stmt, index);
                }
                else if (clrType == typeof(float))
                {
                    return (float)SQLite3.ColumnDouble(stmt, index);
                }
                else if (clrType == typeof(TimeSpan))
                {
                    return new TimeSpan(SQLite3.ColumnInt64(stmt, index));
                }
                else if (clrType == typeof(DateTime))
                {
                    if (this._conn.StoreDateTimeAsTicks)
                    {
                        return new DateTime(SQLite3.ColumnInt64(stmt, index));
                    }
                    else
                    {
                        var text = SQLite3.ColumnString(stmt, index);
                        return DateTime.Parse(text);
                    }
                }
                else if (clrType == typeof(DateTimeOffset))
                {
                    return new DateTimeOffset(SQLite3.ColumnInt64(stmt, index), TimeSpan.Zero);
                }
                else if (clrType.GetTypeInfo().IsEnum)
                {
                    return SQLite3.ColumnInt(stmt, index);
                }
                else if (clrType == typeof(Int64))
                {
                    return SQLite3.ColumnInt64(stmt, index);
                }
                else if (clrType == typeof(UInt32))
                {
                    return (uint)SQLite3.ColumnInt64(stmt, index);
                }
                else if (clrType == typeof(decimal))
                {
                    return (decimal)SQLite3.ColumnDouble(stmt, index);
                }
                else if (clrType == typeof(Byte))
                {
                    return (byte)SQLite3.ColumnInt(stmt, index);
                }
                else if (clrType == typeof(UInt16))
                {
                    return (ushort)SQLite3.ColumnInt(stmt, index);
                }
                else if (clrType == typeof(Int16))
                {
                    return (short)SQLite3.ColumnInt(stmt, index);
                }
                else if (clrType == typeof(sbyte))
                {
                    return (sbyte)SQLite3.ColumnInt(stmt, index);
                }
                else if (clrType == typeof(byte[]))
                {
                    return SQLite3.ColumnByteArray(stmt, index);
                }
                else if (clrType == typeof(Guid))
                {
                    var text = SQLite3.ColumnString(stmt, index);
                    return new Guid(text);
                }
                else
                {
                    throw new NotSupportedException("Don't know how to read " + clrType);
                }
            }
        }

        /// <summary>
        /// The binding.
        /// </summary>
        class Binding
        {
            /// <summary>
            /// Gets or sets the index.
            /// </summary>
            public int Index { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            public object Value { get; set; }
        }
    }

    /// <summary>
    /// Since the insert never changed, we only need to prepare once.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class PreparedSqlLiteInsertCommand : IDisposable
    {
        /// <summary>
        /// The null statement.
        /// </summary>
        internal static readonly IntPtr NullStatement = default(Sqlite3Statement);
        /// <summary>
        /// Gets logging interface
        /// </summary>
        public ILogger Logger => SimpleIoc.Default.GetInstance<ILogger>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PreparedSqlLiteInsertCommand"/> class.
        /// </summary>
        /// <param name="conn">
        /// The conn.
        /// </param>
        internal PreparedSqlLiteInsertCommand(SQLiteConnection conn)
        {
            this.Connection = conn;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="PreparedSqlLiteInsertCommand"/> class.
        /// </summary>
        ~PreparedSqlLiteInsertCommand()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets or sets the command text.
        /// </summary>
        public string CommandText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether initialized.
        /// </summary>
        public bool Initialized { get; set; }

        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        protected SQLiteConnection Connection { get; set; }

        /// <summary>
        /// Gets or sets the statement.
        /// </summary>
        protected IntPtr Statement { get; set; }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The execute non query.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        /// <exception cref="SQLiteException">
        /// </exception>
        /// <exception cref="NotNullConstraintViolationException">
        /// </exception>
        public int ExecuteNonQuery(object[] source)
        {
            this.Logger.LogDebug("Executing Non Query: " + this, LogFlag.LogStatements);
            var r = SQLite3.Result.OK;

            try
            {
                if (!this.Initialized)
                {
                    this.Statement = this.Prepare();
                    this.Initialized = true;
                }

                // bind the values.
                if (source != null)
                {
                    for (int i = 0; i < source.Length; i++)
                    {
                        SQLiteCommand.BindParameter(this.Statement, i + 1, source[i], this.Connection.StoreDateTimeAsTicks);
                    }
                }

                r = SQLite3.Step(this.Statement);

                if (r == SQLite3.Result.Done)
                {
                    int rowsAffected = SQLite3.Changes(this.Connection.Handle);
                    return rowsAffected;
                }
                else if (r == SQLite3.Result.Error)
                {
                    string msg = SQLite3.GetErrmsg(this.Connection.Handle);
                    throw SQLiteException.New(r, msg);
                }
                else if (r == SQLite3.Result.Constraint
                         && SQLite3.ExtendedErrCode(this.Connection.Handle) == SQLite3.ExtendedResult.ConstraintNotNull)
                {
                    throw NotNullConstraintViolationException.New(r, SQLite3.GetErrmsg(this.Connection.Handle));
                }
                else
                {
                    throw SQLiteException.New(r, r.ToString());
                }
            }
            finally
            {
                if (this.Statement != IntPtr.Zero)
                {
                    SQLite3.Finalize(this.Statement);
                }
            }
        }

        /// <summary>
        /// The prepare.
        /// </summary>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        protected virtual IntPtr Prepare()
        {
            var stmt = SQLite3.Prepare2(this.Connection.Handle, this.CommandText);
            return stmt;
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (this.Statement != NullStatement)
            {
                try
                {
                    SQLite3.Finalize(this.Statement);
                }
                finally
                {
                    this.Statement = NullStatement;
                    this.Connection = null;
                }
            }
        }
    }

    /// <summary>
    /// The base table query.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract class BaseTableQuery
    {
        /// <summary>
        /// The ordering.
        /// </summary>
        protected class Ordering
        {
            /// <summary>
            /// Gets or sets a value indicating whether ascending.
            /// </summary>
            public bool Ascending { get; set; }

            /// <summary>
            /// Gets or sets the column name.
            /// </summary>
            public string ColumnName { get; set; }
        }
    }

    /// <summary>
    /// The table query.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    [ExcludeFromCodeCoverage]
    public class TableQuery<T> : BaseTableQuery, IEnumerable<T>
    {
        /// <summary>
        /// The _deferred.
        /// </summary>
        bool _deferred;

        /// <summary>
        /// The _join inner.
        /// </summary>
        BaseTableQuery _joinInner;

        /// <summary>
        /// The _join inner key selector.
        /// </summary>
        Expression _joinInnerKeySelector;

        /// <summary>
        /// The _join outer.
        /// </summary>
        BaseTableQuery _joinOuter;

        /// <summary>
        /// The _join outer key selector.
        /// </summary>
        Expression _joinOuterKeySelector;

        /// <summary>
        /// The _join selector.
        /// </summary>
        Expression _joinSelector;

        /// <summary>
        /// The _limit.
        /// </summary>
        int? _limit;

        /// <summary>
        /// The _offset.
        /// </summary>
        int? _offset;

        /// <summary>
        /// The _order bys.
        /// </summary>
        List<Ordering> _orderBys;

        /// <summary>
        /// The _selector.
        /// </summary>
        Expression _selector;

        /// <summary>
        /// The _where.
        /// </summary>
        Expression _where;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableQuery{T}"/> class.
        /// </summary>
        /// <param name="conn">
        /// The conn.
        /// </param>
        public TableQuery(SQLiteConnection conn)
        {
            this.Connection = conn;
            this.Table = this.Connection.GetMapping(typeof(T));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableQuery{T}"/> class.
        /// </summary>
        /// <param name="conn">
        /// The conn.
        /// </param>
        /// <param name="table">
        /// The table.
        /// </param>
        TableQuery(SQLiteConnection conn, TableMapping table)
        {
            this.Connection = conn;
            this.Table = table;
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        public SQLiteConnection Connection { get; private set; }

        /// <summary>
        /// Gets the table.
        /// </summary>
        public TableMapping Table { get; private set; }

        /// <summary>
        /// The clone.
        /// </summary>
        /// <typeparam name="U">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TableQuery"/>.
        /// </returns>
        public TableQuery<U> Clone<U>()
        {
            var q = new TableQuery<U>(this.Connection, this.Table);
            q._where = this._where;
            q._deferred = this._deferred;
            if (this._orderBys != null)
            {
                q._orderBys = new List<Ordering>(this._orderBys);
            }

            q._limit = this._limit;
            q._offset = this._offset;
            q._joinInner = this._joinInner;
            q._joinInnerKeySelector = this._joinInnerKeySelector;
            q._joinOuter = this._joinOuter;
            q._joinOuterKeySelector = this._joinOuterKeySelector;
            q._joinSelector = this._joinSelector;
            q._selector = this._selector;
            return q;
        }

        /// <summary>
        /// The count.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int Count()
        {
            return this.GenerateCommand("count(*)").ExecuteScalar<int>();
        }

        /// <summary>
        /// The count.
        /// </summary>
        /// <param name="predExpr">
        /// The pred expr.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int Count(Expression<Func<T, bool>> predExpr)
        {
            return this.Where(predExpr).Count();
        }

        /// <summary>
        /// The deferred.
        /// </summary>
        /// <returns>
        /// The <see cref="TableQuery"/>.
        /// </returns>
        public TableQuery<T> Deferred()
        {
            var q = this.Clone<T>();
            q._deferred = true;
            return q;
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="predExpr">
        /// The pred expr.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public int Delete(Expression<Func<T, bool>> predExpr)
        {
            if (predExpr.NodeType == ExpressionType.Lambda)
            {
                var lambda = (LambdaExpression)predExpr;
                var pred = lambda.Body;
                var args = new List<object>();
                var w = this.CompileExpr(pred, args);
                var cmdText = "delete from \"" + this.Table.TableName + "\"";
                cmdText += " where " + w.CommandText;
                var command = this.Connection.CreateCommand(cmdText, args.ToArray());

                int result = command.ExecuteNonQuery();
                return result;
            }
            else
            {
                throw new NotSupportedException("Must be a predicate");
            }
        }

        /// <summary>
        /// The element at.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T ElementAt(int index)
        {
            return this.Skip(index).Take(1).First();
        }

        /// <summary>
        /// The first.
        /// </summary>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T First()
        {
            var query = this.Take(1);
            return query.ToList<T>().First();
        }

        /// <summary>
        /// The first or default.
        /// </summary>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T FirstOrDefault()
        {
            var query = this.Take(1);
            return query.ToList<T>().FirstOrDefault();
        }

        /// <summary>
        /// The get enumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            if (!this._deferred)
            {
                return this.GenerateCommand("*").ExecuteQuery<T>().GetEnumerator();
            }

            return this.GenerateCommand("*").ExecuteDeferredQuery<T>().GetEnumerator();
        }

        /// <summary>
        /// The join.
        /// </summary>
        /// <param name="inner">
        /// The inner.
        /// </param>
        /// <param name="outerKeySelector">
        /// The outer key selector.
        /// </param>
        /// <param name="innerKeySelector">
        /// The inner key selector.
        /// </param>
        /// <param name="resultSelector">
        /// The result selector.
        /// </param>
        /// <typeparam name="TInner">
        /// </typeparam>
        /// <typeparam name="TKey">
        /// </typeparam>
        /// <typeparam name="TResult">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TableQuery"/>.
        /// </returns>
        public TableQuery<TResult> Join<TInner, TKey, TResult>(
            TableQuery<TInner> inner,
            Expression<Func<T, TKey>> outerKeySelector,
            Expression<Func<TInner, TKey>> innerKeySelector,
            Expression<Func<T, TInner, TResult>> resultSelector)
        {
            var q = new TableQuery<TResult>(this.Connection, this.Connection.GetMapping(typeof(TResult)))
            {
                _joinOuter = this,
                _joinOuterKeySelector = outerKeySelector,
                _joinInner = inner,
                _joinInnerKeySelector = innerKeySelector,
                _joinSelector = resultSelector,
            };
            return q;
        }

        /// <summary>
        /// The order by.
        /// </summary>
        /// <param name="orderExpr">
        /// The order expr.
        /// </param>
        /// <typeparam name="U">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TableQuery"/>.
        /// </returns>
        public TableQuery<T> OrderBy<U>(Expression<Func<T, U>> orderExpr)
        {
            return this.AddOrderBy<U>(orderExpr, true);
        }

        /// <summary>
        /// The order by descending.
        /// </summary>
        /// <param name="orderExpr">
        /// The order expr.
        /// </param>
        /// <typeparam name="U">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TableQuery"/>.
        /// </returns>
        public TableQuery<T> OrderByDescending<U>(Expression<Func<T, U>> orderExpr)
        {
            return this.AddOrderBy<U>(orderExpr, false);
        }

        /// <summary>
        /// The select.
        /// </summary>
        /// <param name="selector">
        /// The selector.
        /// </param>
        /// <typeparam name="TResult">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TableQuery"/>.
        /// </returns>
        public TableQuery<TResult> Select<TResult>(Expression<Func<T, TResult>> selector)
        {
            var q = this.Clone<TResult>();
            q._selector = selector;
            return q;
        }

        /// <summary>
        /// The skip.
        /// </summary>
        /// <param name="n">
        /// The n.
        /// </param>
        /// <returns>
        /// The <see cref="TableQuery{T}"/>.
        /// </returns>
        public TableQuery<T> Skip(int n)
        {
            var q = this.Clone<T>();
            q._offset = n;
            return q;
        }

        /// <summary>
        /// The take.
        /// </summary>
        /// <param name="n">
        /// The n.
        /// </param>
        /// <returns>
        /// The <see cref="TableQuery"/>.
        /// </returns>
        public TableQuery<T> Take(int n)
        {
            var q = this.Clone<T>();
            q._limit = n;
            return q;
        }

        /// <summary>
        /// The then by.
        /// </summary>
        /// <param name="orderExpr">
        /// The order expr.
        /// </param>
        /// <typeparam name="U">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TableQuery"/>.
        /// </returns>
        public TableQuery<T> ThenBy<U>(Expression<Func<T, U>> orderExpr)
        {
            return this.AddOrderBy<U>(orderExpr, true);
        }

        /// <summary>
        /// The then by descending.
        /// </summary>
        /// <param name="orderExpr">
        /// The order expr.
        /// </param>
        /// <typeparam name="U">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TableQuery"/>.
        /// </returns>
        public TableQuery<T> ThenByDescending<U>(Expression<Func<T, U>> orderExpr)
        {
            return this.AddOrderBy<U>(orderExpr, false);
        }

        /// <summary>
        /// The where.
        /// </summary>
        /// <param name="predExpr">
        /// The pred expr.
        /// </param>
        /// <returns>
        /// The <see cref="TableQuery"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public TableQuery<T> Where(Expression<Func<T, bool>> predExpr)
        {
            if (predExpr.NodeType == ExpressionType.Lambda)
            {
                var lambda = (LambdaExpression)predExpr;
                var pred = lambda.Body;
                var q = this.Clone<T>();
                q.AddWhere(pred);
                return q;
            }
            else
            {
                throw new NotSupportedException("Must be a predicate");
            }
        }

        /// <summary>
        /// The get enumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// The convert to.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="t">
        /// The t.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        static object ConvertTo(object obj, Type t)
        {
            Type nut = Nullable.GetUnderlyingType(t);

            if (nut != null)
            {
                if (obj == null)
                {
                    return null;
                }

                return Convert.ChangeType(obj, nut);
            }
            else
            {
                return Convert.ChangeType(obj, t);
            }
        }

        private static CompileResult WorkSpecialMagicForEnumerables(IList<object> queryArgs, object queryValue)
        {
            if (queryArgs == null)
            {
                throw new ArgumentNullException(nameof(queryArgs));
            }

            if (queryValue != null && queryValue is IEnumerable && !(queryValue is string)
                && !(queryValue is IEnumerable<byte>))
            {
                var sb = new StringBuilder();
                sb.Append("(");
                var head = string.Empty;
                foreach (var a in (IEnumerable)queryValue)
                {
                    queryArgs.Add(a);
                    sb.Append(head);
                    sb.Append("?");
                    head = ",";
                }

                sb.Append(")");
                return new CompileResult { CommandText = sb.ToString(), Value = queryValue };
            }
            else
            {
                queryArgs.Add(queryValue);
                return new CompileResult { CommandText = "?", Value = queryValue };
            }
        }

        private static CompileResult CompileExprConstant(ConstantExpression expr, IList<object> queryArgs)
        {
            if (expr == null)
            {
                throw new ArgumentNullException(nameof(expr));
            }

            if (queryArgs == null)
            {
                throw new ArgumentNullException(nameof(queryArgs));
            }

            queryArgs.Add(expr.Value);
            return new CompileResult { CommandText = "?", Value = expr.Value };
        }

        /// <summary>
        /// The add order by.
        /// </summary>
        /// <param name="orderExpr">
        /// The order expr.
        /// </param>
        /// <param name="asc">
        /// The asc.
        /// </param>
        /// <typeparam name="U">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TableQuery"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        private TableQuery<T> AddOrderBy<U>(Expression<Func<T, U>> orderExpr, bool asc)
        {
            if (orderExpr.NodeType == ExpressionType.Lambda)
            {
                var lambda = (LambdaExpression)orderExpr;

                MemberExpression mem = null;

                var unary = lambda.Body as UnaryExpression;
                if (unary != null && unary.NodeType == ExpressionType.Convert)
                {
                    mem = unary.Operand as MemberExpression;
                }
                else
                {
                    mem = lambda.Body as MemberExpression;
                }

                if (mem != null && (mem.Expression.NodeType == ExpressionType.Parameter))
                {
                    var q = this.Clone<T>();
                    if (q._orderBys == null)
                    {
                        q._orderBys = new List<Ordering>();
                    }

                    q._orderBys.Add(
                        new Ordering
                        {
                            ColumnName = this.Table.FindColumnWithPropertyName(mem.Member.Name).Name,
                            Ascending = asc
                        });
                    return q;
                }
                else
                {
                    throw new NotSupportedException("Order By does not support: " + orderExpr);
                }
            }
            else
            {
                throw new NotSupportedException("Must be a predicate");
            }
        }

        /// <summary>
        /// The add where.
        /// </summary>
        /// <param name="pred">
        /// The pred.
        /// </param>
        private void AddWhere(Expression pred)
        {
            if (this._where == null)
            {
                this._where = pred;
            }
            else
            {
                this._where = Expression.AndAlso(this._where, pred);
            }
        }

        /// <summary>
        /// The compile expr.
        /// </summary>
        /// <param name="expr">
        /// The expr.
        /// </param>
        /// <param name="queryArgs">
        /// The query args.
        /// </param>
        /// <returns>
        /// The <see cref="TableQuery"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        private CompileResult CompileExpr(Expression expr, IList<object> queryArgs)
        {
            if (expr == null)
            {
                throw new NotSupportedException("Expression is NULL");
            }
            else if (expr is BinaryExpression)
            {
                return this.CompileExprBinary(expr as BinaryExpression, queryArgs);
            }
            else if (expr.NodeType == ExpressionType.Not)
            {
                return this.CompileExprUnary(expr as UnaryExpression, queryArgs);
            }
            else if (expr.NodeType == ExpressionType.Call)
            {
                return this.CompileExprCall(expr as MethodCallExpression, queryArgs);
            }
            else if (expr.NodeType == ExpressionType.Constant)
            {
                return CompileExprConstant(expr as ConstantExpression, queryArgs);
            }
            else if (expr.NodeType == ExpressionType.Convert)
            {
                return this.CompileExprConvert(expr as UnaryExpression, queryArgs);
            }
            else if (expr.NodeType == ExpressionType.MemberAccess)
            {
                return this.CompilerExprMemberAccess(expr as MemberExpression, queryArgs);
            }

            throw new NotSupportedException("Cannot compile: " + expr.NodeType.ToString());
        }

        private CompileResult CompileExprBinary(BinaryExpression expr, IList<object> queryArgs)
        {
            if (expr == null)
            {
                throw new ArgumentNullException(nameof(expr));
            }

            // VB turns 'x=="foo"' into 'CompareString(x,"foo",true/false)==0', so we need to unwrap it
            // http://blogs.msdn.com/b/vbteam/archive/2007/09/18/vb-expression-trees-string-comparisons.aspx
            if (expr.Left.NodeType == ExpressionType.Call)
            {
                var call = (MethodCallExpression)expr.Left;
                if (call.Method.DeclaringType.FullName == "Microsoft.VisualBasic.CompilerServices.Operators"
                    && call.Method.Name == "CompareString")
                {
                    expr = Expression.MakeBinary(expr.NodeType, call.Arguments[0], call.Arguments[1]);
                }
            }

            var leftr = this.CompileExpr(expr.Left, queryArgs);
            var rightr = this.CompileExpr(expr.Right, queryArgs);

            // If either side is a parameter and is null, then handle the other side specially (for "is null"/"is not null")
            string text;
            if (leftr.CommandText == "?" && leftr.Value == null)
            {
                text = this.CompileNullBinaryExpression(expr, rightr);
            }
            else if (rightr.CommandText == "?" && rightr.Value == null)
            {
                text = this.CompileNullBinaryExpression(expr, leftr);
            }
            else
            {
                text = $"({leftr.CommandText} {this.GetSqlName(expr)} {rightr.CommandText})";
            }

            return new CompileResult { CommandText = text };
        }

        private CompileResult CompileExprUnary(UnaryExpression expr, IList<object> queryArgs)
        {
            if (expr == null)
            {
                throw new ArgumentNullException(nameof(expr));
            }

            var operandExpr = expr.Operand;
            var result = this.CompileExpr(operandExpr, queryArgs);
            var resultValue = result.Value;
            if (resultValue is bool)
            {
                resultValue = !((bool)resultValue);
            }

            return new CompileResult { CommandText = "NOT(" + result.CommandText + ")", Value = resultValue };
        }

        private CompileResult CompileExprCall(MethodCallExpression expr, IList<object> queryArgs)
        {
            if (expr == null)
            {
                throw new ArgumentNullException(nameof(expr));
            }

            var args = new CompileResult[expr.Arguments.Count];
            var obj = expr.Object != null ? this.CompileExpr(expr.Object, queryArgs) : null;

            for (var i = 0; i < args.Length; i++)
            {
                args[i] = this.CompileExpr(expr.Arguments[i], queryArgs);
            }

            var sqlCall = string.Empty;

            if (expr.Method.Name == "Like" && args.Length == 2)
            {
                sqlCall = "(" + args[0].CommandText + " like " + args[1].CommandText + ")";
            }
            else if (expr.Method.Name == "Contains" && args.Length == 2)
            {
                sqlCall = "(" + args[1].CommandText + " in " + args[0].CommandText + ")";
            }
            else if (expr.Method.Name == "Contains" && args.Length == 1)
            {
                if (expr.Object != null && expr.Object.Type == typeof(string))
                {
                    sqlCall = "(" + obj.CommandText + " like ('%' || " + args[0].CommandText + " || '%'))";
                }
                else
                {
                    sqlCall = "(" + args[0].CommandText + " in " + obj.CommandText + ")";
                }
            }
            else if (expr.Method.Name == "StartsWith" && args.Length == 1)
            {
                sqlCall = "(" + obj.CommandText + " like (" + args[0].CommandText + " || '%'))";
            }
            else if (expr.Method.Name == "EndsWith" && args.Length == 1)
            {
                sqlCall = "(" + obj.CommandText + " like ('%' || " + args[0].CommandText + "))";
            }
            else if (expr.Method.Name == "Equals" && args.Length == 1)
            {
                sqlCall = "(" + obj.CommandText + " = (" + args[0].CommandText + "))";
            }
            else if (expr.Method.Name == "ToLower")
            {
                sqlCall = "(lower(" + obj.CommandText + "))";
            }
            else if (expr.Method.Name == "ToUpper")
            {
                sqlCall = "(upper(" + obj.CommandText + "))";
            }
            else
            {
                sqlCall = expr.Method.Name.ToLower() + "(" + string.Join(",", args.Select(a => a.CommandText).ToArray()) + ")";
            }

            return new CompileResult { CommandText = sqlCall };
        }

        private CompileResult CompileExprConvert(UnaryExpression expr, IList<object> queryArgs)
        {
            if (expr == null)
            {
                throw new ArgumentNullException(nameof(expr));
            }

            var expressionType = expr.Type;
            var result = this.CompileExpr(expr.Operand, queryArgs);
            return new CompileResult
            {
                CommandText = result.CommandText,
                Value = result.Value != null ? ConvertTo(result.Value, expressionType) : null
            };
        }

        private CompileResult CompilerExprMemberAccess(MemberExpression expr, IList<object> queryArgs)
        {
            if (expr == null)
            {
                throw new ArgumentNullException(nameof(expr));
            }

            if (expr.Expression != null && expr.Expression.NodeType == ExpressionType.Parameter)
            {
                // This is a column of our table, output just the column name
                // Need to translate it if that column name is mapped
                var columnName = this.Table.FindColumnWithPropertyName(expr.Member.Name).Name;
                return new CompileResult { CommandText = "\"" + columnName + "\"" };
            }
            else
            {
                return this.CompilerExprMemberAccess(expr, queryArgs, expr);
            }
        }

        private CompileResult CompilerExprMemberAccess(Expression expr, IList<object> queryArgs, MemberExpression mem)
        {
            if (expr == null)
            {
                throw new ArgumentNullException(nameof(expr));
            }

            if (queryArgs == null)
            {
                throw new ArgumentNullException(nameof(queryArgs));
            }

            if (mem == null)
            {
                throw new ArgumentNullException(nameof(mem));
            }

            object obj = null;
            if (mem.Expression != null)
            {
                var compilerResult = this.CompileExpr(mem.Expression, queryArgs);
                if (compilerResult.Value == null)
                {
                    throw new NotSupportedException("Member access failed to compile expression");
                }

                if (compilerResult.CommandText == "?")
                {
                    queryArgs.RemoveAt(queryArgs.Count - 1);
                }

                obj = compilerResult.Value;
            }

            // Get the member value
            object val = null;

#if !USE_NEW_REFLECTION_API
					if (mem.Member.MemberType == MemberTypes.Property) {
#else
            if (mem.Member is PropertyInfo)
            {
#endif
                var m = (PropertyInfo)mem.Member;
                val = m.GetValue(obj, null);
#if !USE_NEW_REFLECTION_API
					} else if (mem.Member.MemberType == MemberTypes.Field) {
#else
            }
            else if (mem.Member is FieldInfo)
            {
#endif
#if SILVERLIGHT
                val = Expression.Lambda(expr).Compile().DynamicInvoke();
#else
                        var m = (FieldInfo)mem.Member;
                        val = m.GetValue(obj);
#endif
            }
            else
            {
#if !USE_NEW_REFLECTION_API
						throw new NotSupportedException ("MemberExpr: " + mem.Member.MemberType);
#else
                throw new NotSupportedException("MemberExpr: " + mem.Member.DeclaringType);
#endif
            }

            return WorkSpecialMagicForEnumerables(queryArgs, val);
        }

        /// <summary>
        /// Compiles a BinaryExpression where one of the parameters is null.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="parameter">
        /// The non-null parameter
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string CompileNullBinaryExpression(BinaryExpression expression, CompileResult parameter)
        {
            if (expression.NodeType == ExpressionType.Equal)
            {
                return "(" + parameter.CommandText + " is ?)";
            }
            else if (expression.NodeType == ExpressionType.NotEqual)
            {
                return "(" + parameter.CommandText + " is not ?)";
            }

            throw new NotSupportedException("Cannot compile Null-BinaryExpression with type " + expression.NodeType);
        }

        /// <summary>
        /// The generate command.
        /// </summary>
        /// <param name="selectionList">
        /// The selection list.
        /// </param>
        /// <returns>
        /// The <see cref="SQLiteCommand"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        private SQLiteCommand GenerateCommand(string selectionList)
        {
            if (this._joinInner != null && this._joinOuter != null)
            {
                throw new NotSupportedException("Joins are not supported.");
            }
            else
            {
                var cmdText = "select " + selectionList + " from \"" + this.Table.TableName + "\"";
                var args = new List<object>();
                if (this._where != null)
                {
                    var w = this.CompileExpr(this._where, args);
                    cmdText += " where " + w.CommandText;
                }

                if ((this._orderBys != null) && (this._orderBys.Count > 0))
                {
                    var t = string.Join(
                        ", ",
                        this._orderBys.Select(o => "\"" + o.ColumnName + "\"" + (o.Ascending ? string.Empty : " desc")).ToArray());
                    cmdText += " order by " + t;
                }

                if (this._limit.HasValue)
                {
                    cmdText += " limit " + this._limit.Value;
                }

                if (this._offset.HasValue)
                {
                    if (!this._limit.HasValue)
                    {
                        cmdText += " limit -1 ";
                    }

                    cmdText += " offset " + this._offset.Value;
                }

                return this.Connection.CreateCommand(cmdText, args.ToArray());
            }
        }

        /// <summary>
        /// The get sql name.
        /// </summary>
        /// <param name="expr">
        /// The expr.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        string GetSqlName(Expression expr)
        {
            var n = expr.NodeType;
            switch (n)
            {
                case ExpressionType.GreaterThan:
                    return ">";

                case ExpressionType.GreaterThanOrEqual:
                    return ">=";

                case ExpressionType.LessThan:
                    return "<";

                case ExpressionType.LessThanOrEqual:
                    return "<=";

                case ExpressionType.And:
                    return "&";

                case ExpressionType.AndAlso:
                    return "and";

                case ExpressionType.Or:
                    return "|";

                case ExpressionType.OrElse:
                    return "or";

                case ExpressionType.Equal:
                    return "=";

                case ExpressionType.NotEqual:
                    return "!=";

                default:
                    throw new NotSupportedException("Cannot get SQL for: " + n);
            }
        }

        /// <summary>
        /// The compile result.
        /// </summary>
        class CompileResult
        {
            /// <summary>
            /// Gets or sets the command text.
            /// </summary>
            public string CommandText { get; set; }

            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            public object Value { get; set; }
        }
    }

    /// <summary>
    /// The sq lite 3.
    /// </summary>
    //[ExcludeFromCodeCoverage]
    public static class SQLite3
    {
        /// <summary>
        /// The result.
        /// </summary>
        public enum Result : int
        {
            /// <summary>
            /// The ok.
            /// </summary>
            OK = 0,

            /// <summary>
            /// The error.
            /// </summary>
            Error = 1,

            /// <summary>
            /// The internal.
            /// </summary>
            Internal = 2,

            /// <summary>
            /// The perm.
            /// </summary>
            Perm = 3,

            /// <summary>
            /// The abort.
            /// </summary>
            Abort = 4,

            /// <summary>
            /// The busy.
            /// </summary>
            Busy = 5,

            /// <summary>
            /// The locked.
            /// </summary>
            Locked = 6,

            /// <summary>
            /// The no mem.
            /// </summary>
            NoMem = 7,

            /// <summary>
            /// The read only.
            /// </summary>
            ReadOnly = 8,

            /// <summary>
            /// The interrupt.
            /// </summary>
            Interrupt = 9,

            /// <summary>
            /// The io error.
            /// </summary>
            IOError = 10,

            /// <summary>
            /// The corrupt.
            /// </summary>
            Corrupt = 11,

            /// <summary>
            /// The not found.
            /// </summary>
            NotFound = 12,

            /// <summary>
            /// The full.
            /// </summary>
            Full = 13,

            /// <summary>
            /// The cannot open.
            /// </summary>
            CannotOpen = 14,

            /// <summary>
            /// The lock err.
            /// </summary>
            LockErr = 15,

            /// <summary>
            /// The empty.
            /// </summary>
            Empty = 16,

            /// <summary>
            /// The schema chngd.
            /// </summary>
            SchemaChngd = 17,

            /// <summary>
            /// The too big.
            /// </summary>
            TooBig = 18,

            /// <summary>
            /// The constraint.
            /// </summary>
            Constraint = 19,

            /// <summary>
            /// The mismatch.
            /// </summary>
            Mismatch = 20,

            /// <summary>
            /// The misuse.
            /// </summary>
            Misuse = 21,

            /// <summary>
            /// The not implemented lfs.
            /// </summary>
            NotImplementedLFS = 22,

            /// <summary>
            /// The access denied.
            /// </summary>
            AccessDenied = 23,

            /// <summary>
            /// The format.
            /// </summary>
            Format = 24,

            /// <summary>
            /// The range.
            /// </summary>
            Range = 25,

            /// <summary>
            /// The non db file.
            /// </summary>
            NonDBFile = 26,

            /// <summary>
            /// The notice.
            /// </summary>
            Notice = 27,

            /// <summary>
            /// The warning.
            /// </summary>
            Warning = 28,

            /// <summary>
            /// The row.
            /// </summary>
            Row = 100,

            /// <summary>
            /// The done.
            /// </summary>
            Done = 101
        }

        /// <summary>
        /// The extended result.
        /// </summary>
        public enum ExtendedResult : int
        {
            /// <summary>
            /// The io error read.
            /// </summary>
            IOErrorRead = Result.IOError | (1 << 8),

            /// <summary>
            /// The io error short read.
            /// </summary>
            IOErrorShortRead = Result.IOError | (2 << 8),

            /// <summary>
            /// The io error write.
            /// </summary>
            IOErrorWrite = Result.IOError | (3 << 8),

            /// <summary>
            /// The io error fsync.
            /// </summary>
            IOErrorFsync = Result.IOError | (4 << 8),

            /// <summary>
            /// The io error dir f sync.
            /// </summary>
            IOErrorDirFSync = Result.IOError | (5 << 8),

            /// <summary>
            /// The io error truncate.
            /// </summary>
            IOErrorTruncate = Result.IOError | (6 << 8),

            /// <summary>
            /// The io error f stat.
            /// </summary>
            IOErrorFStat = Result.IOError | (7 << 8),

            /// <summary>
            /// The io error unlock.
            /// </summary>
            IOErrorUnlock = Result.IOError | (8 << 8),

            /// <summary>
            /// The io error rdlock.
            /// </summary>
            IOErrorRdlock = Result.IOError | (9 << 8),

            /// <summary>
            /// The io error delete.
            /// </summary>
            IOErrorDelete = Result.IOError | (10 << 8),

            /// <summary>
            /// The io error blocked.
            /// </summary>
            IOErrorBlocked = Result.IOError | (11 << 8),

            /// <summary>
            /// The io error no mem.
            /// </summary>
            IOErrorNoMem = Result.IOError | (12 << 8),

            /// <summary>
            /// The io error access.
            /// </summary>
            IOErrorAccess = Result.IOError | (13 << 8),

            /// <summary>
            /// The io error check reserved lock.
            /// </summary>
            IOErrorCheckReservedLock = Result.IOError | (14 << 8),

            /// <summary>
            /// The io error lock.
            /// </summary>
            IOErrorLock = Result.IOError | (15 << 8),

            /// <summary>
            /// The io error close.
            /// </summary>
            IOErrorClose = Result.IOError | (16 << 8),

            /// <summary>
            /// The io error dir close.
            /// </summary>
            IOErrorDirClose = Result.IOError | (17 << 8),

            /// <summary>
            /// The io error shm open.
            /// </summary>
            IOErrorSHMOpen = Result.IOError | (18 << 8),

            /// <summary>
            /// The io error shm size.
            /// </summary>
            IOErrorSHMSize = Result.IOError | (19 << 8),

            /// <summary>
            /// The io error shm lock.
            /// </summary>
            IOErrorSHMLock = Result.IOError | (20 << 8),

            /// <summary>
            /// The io error shm map.
            /// </summary>
            IOErrorSHMMap = Result.IOError | (21 << 8),

            /// <summary>
            /// The io error seek.
            /// </summary>
            IOErrorSeek = Result.IOError | (22 << 8),

            /// <summary>
            /// The io error delete no ent.
            /// </summary>
            IOErrorDeleteNoEnt = Result.IOError | (23 << 8),

            /// <summary>
            /// The io error m map.
            /// </summary>
            IOErrorMMap = Result.IOError | (24 << 8),

            /// <summary>
            /// The locked sharedcache.
            /// </summary>
            LockedSharedcache = Result.Locked | (1 << 8),

            /// <summary>
            /// The busy recovery.
            /// </summary>
            BusyRecovery = Result.Busy | (1 << 8),

            /// <summary>
            /// The cannott open no temp dir.
            /// </summary>
            CannottOpenNoTempDir = Result.CannotOpen | (1 << 8),

            /// <summary>
            /// The cannot open is dir.
            /// </summary>
            CannotOpenIsDir = Result.CannotOpen | (2 << 8),

            /// <summary>
            /// The cannot open full path.
            /// </summary>
            CannotOpenFullPath = Result.CannotOpen | (3 << 8),

            /// <summary>
            /// The corrupt v tab.
            /// </summary>
            CorruptVTab = Result.Corrupt | (1 << 8),

            /// <summary>
            /// The readonly recovery.
            /// </summary>
            ReadonlyRecovery = Result.ReadOnly | (1 << 8),

            /// <summary>
            /// The readonly cannot lock.
            /// </summary>
            ReadonlyCannotLock = Result.ReadOnly | (2 << 8),

            /// <summary>
            /// The readonly rollback.
            /// </summary>
            ReadonlyRollback = Result.ReadOnly | (3 << 8),

            /// <summary>
            /// The abort rollback.
            /// </summary>
            AbortRollback = Result.Abort | (2 << 8),

            /// <summary>
            /// The constraint check.
            /// </summary>
            ConstraintCheck = Result.Constraint | (1 << 8),

            /// <summary>
            /// The constraint commit hook.
            /// </summary>
            ConstraintCommitHook = Result.Constraint | (2 << 8),

            /// <summary>
            /// The constraint foreign key.
            /// </summary>
            ConstraintForeignKey = Result.Constraint | (3 << 8),

            /// <summary>
            /// The constraint function.
            /// </summary>
            ConstraintFunction = Result.Constraint | (4 << 8),

            /// <summary>
            /// The constraint not null.
            /// </summary>
            ConstraintNotNull = Result.Constraint | (5 << 8),

            /// <summary>
            /// The constraint primary key.
            /// </summary>
            ConstraintPrimaryKey = Result.Constraint | (6 << 8),

            /// <summary>
            /// The constraint trigger.
            /// </summary>
            ConstraintTrigger = Result.Constraint | (7 << 8),

            /// <summary>
            /// The constraint unique.
            /// </summary>
            ConstraintUnique = Result.Constraint | (8 << 8),

            /// <summary>
            /// The constraint v tab.
            /// </summary>
            ConstraintVTab = Result.Constraint | (9 << 8),

            /// <summary>
            /// The notice recover wal.
            /// </summary>
            NoticeRecoverWAL = Result.Notice | (1 << 8),

            /// <summary>
            /// The notice recover rollback.
            /// </summary>
            NoticeRecoverRollback = Result.Notice | (2 << 8)
        }

        /// <summary>
        /// The config option.
        /// </summary>
        public enum ConfigOption : int
        {
            /// <summary>
            /// The single thread.
            /// </summary>
            SingleThread = 1,

            /// <summary>
            /// The multi thread.
            /// </summary>
            MultiThread = 2,

            /// <summary>
            /// The serialized.
            /// </summary>
            Serialized = 3
        }

        /// <summary>
        /// The library path.
        /// </summary>
        const string LibraryPath = SQLibPath.LibraryPath;

#if !USE_CSHARP_SQLITE && !USE_WP8_NATIVE_SQLITE && !USE_SQLITEPCL_RAW

        /// <summary>
        /// The threadsafe.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_threadsafe", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Threadsafe();

        /// <summary>
        /// The open.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <param name="db">
        /// The db.
        /// </param>
        /// <returns>
        /// The <see cref="Result"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_open", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result Open([MarshalAs(UnmanagedType.LPStr)] string filename, out IntPtr db);

        /// <summary>
        /// The open.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <param name="db">
        /// The db.
        /// </param>
        /// <param name="flags">
        /// The flags.
        /// </param>
        /// <param name="zvfs">
        /// The zvfs.
        /// </param>
        /// <returns>
        /// The <see cref="Result"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_open_v2", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result Open(
            [MarshalAs(UnmanagedType.LPStr)] string filename,
            out IntPtr db,
            int flags,
            IntPtr zvfs);

        /// <summary>
        /// The open.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <param name="db">
        /// The db.
        /// </param>
        /// <param name="flags">
        /// The flags.
        /// </param>
        /// <param name="zvfs">
        /// The zvfs.
        /// </param>
        /// <returns>
        /// The <see cref="Result"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_open_v2", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result Open(byte[] filename, out IntPtr db, int flags, IntPtr zvfs);

        /// <summary>
        /// The open 16.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <param name="db">
        /// The db.
        /// </param>
        /// <returns>
        /// The <see cref="Result"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_open16", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result Open16([MarshalAs(UnmanagedType.LPWStr)] string filename, out IntPtr db);

        /// <summary>
        /// The enable load extension.
        /// </summary>
        /// <param name="db">
        /// The db.
        /// </param>
        /// <param name="onoff">
        /// The onoff.
        /// </param>
        /// <returns>
        /// The <see cref="Result"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_enable_load_extension",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern Result EnableLoadExtension(IntPtr db, int onoff);

        /// <summary>
        /// The close.
        /// </summary>
        /// <param name="db">
        /// The db.
        /// </param>
        /// <returns>
        /// The <see cref="Result"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_close", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result Close(IntPtr db);

        /// <summary>
        /// The close 2.
        /// </summary>
        /// <param name="db">
        /// The db.
        /// </param>
        /// <returns>
        /// The <see cref="Result"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_close_v2", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result Close2(IntPtr db);

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <returns>
        /// The <see cref="Result"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_initialize", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result Initialize();

        /// <summary>
        /// The shutdown.
        /// </summary>
        /// <returns>
        /// The <see cref="Result"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_shutdown", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result Shutdown();

        /// <summary>
        /// The config.
        /// </summary>
        /// <param name="option">
        /// The option.
        /// </param>
        /// <returns>
        /// The <see cref="Result"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_config", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result Config(ConfigOption option);

        /// <summary>
        /// The set directory.
        /// </summary>
        /// <param name="directoryType">
        /// The directory type.
        /// </param>
        /// <param name="directoryPath">
        /// The directory path.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_win32_set_directory", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Unicode)]
        public static extern int SetDirectory(uint directoryType, string directoryPath);

        /// <summary>
        /// The busy timeout.
        /// </summary>
        /// <param name="db">
        /// The db.
        /// </param>
        /// <param name="milliseconds">
        /// The milliseconds.
        /// </param>
        /// <returns>
        /// The <see cref="Result"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_busy_timeout", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result BusyTimeout(IntPtr db, int milliseconds);

        /// <summary>
        /// The changes.
        /// </summary>
        /// <param name="db">
        /// The db.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_changes", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Changes(IntPtr db);

        /// <summary>
        /// The prepare 2.
        /// </summary>
        /// <param name="db">
        /// The db.
        /// </param>
        /// <param name="sql">
        /// The sql.
        /// </param>
        /// <param name="numBytes">
        /// The num bytes.
        /// </param>
        /// <param name="stmt">
        /// The stmt.
        /// </param>
        /// <param name="pzTail">
        /// The pz tail.
        /// </param>
        /// <returns>
        /// The <see cref="Result"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_prepare_v2", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result Prepare2(
            IntPtr db,
            [MarshalAs(UnmanagedType.LPStr)] string sql,
            int numBytes,
            out IntPtr stmt,
            IntPtr pzTail);

#if NETFX_CORE
        [DllImport(LibraryPath, EntryPoint = "sqlite3_prepare_v2", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result Prepare2(IntPtr db, byte[] queryBytes, int numBytes, out IntPtr stmt, IntPtr pzTail);
#endif

        /// <summary>
        /// The prepare 2.
        /// </summary>
        /// <param name="db">
        /// The db.
        /// </param>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        /// <exception cref="SQLiteException">
        /// </exception>
        public static IntPtr Prepare2(IntPtr db, string query)
        {
            IntPtr stmt;
#if NETFX_CORE
            byte[] queryBytes = System.Text.UTF8Encoding.UTF8.GetBytes(query);
            var r = Prepare2(db, queryBytes, queryBytes.Length, out stmt, IntPtr.Zero);
#else
            var r = Prepare2(db, query, System.Text.Encoding.UTF8.GetByteCount(query), out stmt, IntPtr.Zero);
#endif

#if TESTS
            Close2(db);
#endif

            if (r != Result.OK)
            {
                throw SQLiteException.New(r, GetErrmsg(db));
            }

            return stmt;
        }

        /// <summary>
        /// The step.
        /// </summary>
        /// <param name="stmt">
        /// The stmt.
        /// </param>
        /// <returns>
        /// The <see cref="Result"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_step", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result Step(IntPtr stmt);

        /// <summary>
        /// The reset.
        /// </summary>
        /// <param name="stmt">
        /// The stmt.
        /// </param>
        /// <returns>
        /// The <see cref="Result"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_reset", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result Reset(IntPtr stmt);

        /// <summary>
        /// The finalize.
        /// </summary>
        /// <param name="stmt">
        /// The stmt.
        /// </param>
        /// <returns>
        /// The <see cref="Result"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_finalize", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result Finalize(IntPtr stmt);

        /// <summary>
        /// The last insert rowid.
        /// </summary>
        /// <param name="db">
        /// The db.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_last_insert_rowid", CallingConvention = CallingConvention.Cdecl)]
        public static extern long LastInsertRowid(IntPtr db);

        /// <summary>
        /// The errmsg.
        /// </summary>
        /// <param name="db">
        /// The db.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_errmsg16", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Errmsg(IntPtr db);

        /// <summary>
        /// The get errmsg.
        /// </summary>
        /// <param name="db">
        /// The db.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetErrmsg(IntPtr db)
        {
            return Marshal.PtrToStringUni(Errmsg(db));
        }

        /// <summary>
        /// The bind parameter index.
        /// </summary>
        /// <param name="stmt">
        /// The stmt.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_bind_parameter_index", CallingConvention = CallingConvention.Cdecl
            )]
        public static extern int BindParameterIndex(IntPtr stmt, [MarshalAs(UnmanagedType.LPStr)] string name);

        /// <summary>
        /// The bind null.
        /// </summary>
        /// <param name="stmt">
        /// The stmt.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_bind_null", CallingConvention = CallingConvention.Cdecl)]
        public static extern int BindNull(IntPtr stmt, int index);

        /// <summary>
        /// The bind int.
        /// </summary>
        /// <param name="stmt">
        /// The stmt.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="val">
        /// The val.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_bind_int", CallingConvention = CallingConvention.Cdecl)]
        public static extern int BindInt(IntPtr stmt, int index, int val);

        /// <summary>
        /// The bind int 64.
        /// </summary>
        /// <param name="stmt">
        /// The stmt.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="val">
        /// The val.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_bind_int64", CallingConvention = CallingConvention.Cdecl)]
        public static extern int BindInt64(IntPtr stmt, int index, long val);

        /// <summary>
        /// The bind double.
        /// </summary>
        /// <param name="stmt">
        /// The stmt.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="val">
        /// The val.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_bind_double", CallingConvention = CallingConvention.Cdecl)]
        public static extern int BindDouble(IntPtr stmt, int index, double val);

        /// <summary>
        /// The bind text.
        /// </summary>
        /// <param name="stmt">
        /// The stmt.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="val">
        /// The val.
        /// </param>
        /// <param name="n">
        /// The n.
        /// </param>
        /// <param name="free">
        /// The free.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_bind_text16", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Unicode)]
        public static extern int BindText(
            IntPtr stmt,
            int index,
            [MarshalAs(UnmanagedType.LPWStr)] string val,
            int n,
            IntPtr free);

        /// <summary>
        /// The bind blob.
        /// </summary>
        /// <param name="stmt">
        /// The stmt.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="val">
        /// The val.
        /// </param>
        /// <param name="n">
        /// The n.
        /// </param>
        /// <param name="free">
        /// The free.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_bind_blob", CallingConvention = CallingConvention.Cdecl)]
        public static extern int BindBlob(IntPtr stmt, int index, byte[] val, int n, IntPtr free);

        /// <summary>
        /// The column count.
        /// </summary>
        /// <param name="stmt">
        /// The stmt.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_column_count", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ColumnCount(IntPtr stmt);

        /// <summary>
        /// The column name.
        /// </summary>
        /// <param name="stmt">
        /// The stmt.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_column_name", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ColumnName(IntPtr stmt, int index);

        /// <summary>
        /// The column name 16 internal.
        /// </summary>
        /// <param name="stmt">
        /// The stmt.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_column_name16", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr ColumnName16Internal(IntPtr stmt, int index);

        /// <summary>
        /// The column name 16.
        /// </summary>
        /// <param name="stmt">
        /// The stmt.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ColumnName16(IntPtr stmt, int index)
        {
            return Marshal.PtrToStringUni(ColumnName16Internal(stmt, index));
        }

        /// <summary>
        /// The column type.
        /// </summary>
        /// <param name="stmt">
        /// The stmt.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="ColType"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_column_type", CallingConvention = CallingConvention.Cdecl)]
        public static extern ColType ColumnType(IntPtr stmt, int index);

        /// <summary>
        /// The column int.
        /// </summary>
        /// <param name="stmt">
        /// The stmt.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_column_int", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ColumnInt(IntPtr stmt, int index);

        /// <summary>
        /// The column int 64.
        /// </summary>
        /// <param name="stmt">
        /// The stmt.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_column_int64", CallingConvention = CallingConvention.Cdecl)]
        public static extern long ColumnInt64(IntPtr stmt, int index);

        /// <summary>
        /// The column double.
        /// </summary>
        /// <param name="stmt">
        /// The stmt.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_column_double", CallingConvention = CallingConvention.Cdecl)]
        public static extern double ColumnDouble(IntPtr stmt, int index);

        /// <summary>
        /// The column text.
        /// </summary>
        /// <param name="stmt">
        /// The stmt.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_column_text", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ColumnText(IntPtr stmt, int index);

        /// <summary>
        /// The column text 16.
        /// </summary>
        /// <param name="stmt">
        /// The stmt.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_column_text16", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ColumnText16(IntPtr stmt, int index);

        /// <summary>
        /// The column blob.
        /// </summary>
        /// <param name="stmt">
        /// The stmt.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_column_blob", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ColumnBlob(IntPtr stmt, int index);

        /// <summary>
        /// The column bytes.
        /// </summary>
        /// <param name="stmt">
        /// The stmt.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_column_bytes", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ColumnBytes(IntPtr stmt, int index);

        /// <summary>
        /// The column string.
        /// </summary>
        /// <param name="stmt">
        /// The stmt.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ColumnString(IntPtr stmt, int index)
        {
            return Marshal.PtrToStringUni(ColumnText16(stmt, index));
        }

        /// <summary>
        /// The column byte array.
        /// </summary>
        /// <param name="stmt">
        /// The stmt.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="T:byte[]"/>.
        /// </returns>
        public static byte[] ColumnByteArray(IntPtr stmt, int index)
        {
            int length = ColumnBytes(stmt, index);
            var result = new byte[length];
            if (length > 0)
            {
                Marshal.Copy(ColumnBlob(stmt, index), result, 0, length);
            }

            return result;
        }

        /// <summary>
        /// The extended err code.
        /// </summary>
        /// <param name="db">
        /// The db.
        /// </param>
        /// <returns>
        /// The <see cref="ExtendedResult"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_extended_errcode", CallingConvention = CallingConvention.Cdecl)]
        public static extern ExtendedResult ExtendedErrCode(IntPtr db);

        /// <summary>
        /// The lib version number.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport(LibraryPath, EntryPoint = "sqlite3_libversion_number", CallingConvention = CallingConvention.Cdecl)]
        public static extern int LibVersionNumber();
#else
        public static Result Open(string filename, out Sqlite3DatabaseHandle db)
		{
			return (Result) Sqlite3.sqlite3_open(filename, out db);
		}

		public static Result Open(string filename, out Sqlite3DatabaseHandle db, int flags, IntPtr zVfs)
		{
#if USE_WP8_NATIVE_SQLITE
			return (Result)Sqlite3.sqlite3_open_v2(filename, out db, flags, string.Empty);
#else
			return (Result)Sqlite3.sqlite3_open_v2(filename, out db, flags, null);
#endif
		}

		public static Result Close(Sqlite3DatabaseHandle db)
		{
			return (Result)Sqlite3.sqlite3_close(db);
		}

		public static Result Close2(Sqlite3DatabaseHandle db)
		{
			return (Result)Sqlite3.sqlite3_close_v2(db);
		}

		public static Result BusyTimeout(Sqlite3DatabaseHandle db, int milliseconds)
		{
			return (Result)Sqlite3.sqlite3_busy_timeout(db, milliseconds);
		}

		public static int Changes(Sqlite3DatabaseHandle db)
		{
			return Sqlite3.sqlite3_changes(db);
		}

		public static Sqlite3Statement Prepare2(Sqlite3DatabaseHandle db, string query)
		{
			Sqlite3Statement stmt = default(Sqlite3Statement);
#if USE_WP8_NATIVE_SQLITE || USE_SQLITEPCL_RAW
			var r = Sqlite3.sqlite3_prepare_v2(db, query, out stmt);
#else
			stmt = new Sqlite3Statement();
			var r = Sqlite3.sqlite3_prepare_v2(db, query, -1, ref stmt, 0);
#endif
			if (r != 0)
			{
				throw SQLiteException.New((Result)r, GetErrmsg(db));
			}
			return stmt;
		}

		public static Result Step(Sqlite3Statement stmt)
		{
			return (Result)Sqlite3.sqlite3_step(stmt);
		}

		public static Result Reset(Sqlite3Statement stmt)
		{
			return (Result)Sqlite3.sqlite3_reset(stmt);
		}

		public static Result Finalize(Sqlite3Statement stmt)
		{
			return (Result)Sqlite3.sqlite3_finalize(stmt);
		}

		public static long LastInsertRowid(Sqlite3DatabaseHandle db)
		{
			return Sqlite3.sqlite3_last_insert_rowid(db);
		}

		public static string GetErrmsg(Sqlite3DatabaseHandle db)
		{
			return Sqlite3.sqlite3_errmsg(db);
		}

		public static int BindParameterIndex(Sqlite3Statement stmt, string name)
		{
			return Sqlite3.sqlite3_bind_parameter_index(stmt, name);
		}

		public static int BindNull(Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_bind_null(stmt, index);
		}

		public static int BindInt(Sqlite3Statement stmt, int index, int val)
		{
			return Sqlite3.sqlite3_bind_int(stmt, index, val);
		}

		public static int BindInt64(Sqlite3Statement stmt, int index, long val)
		{
			return Sqlite3.sqlite3_bind_int64(stmt, index, val);
		}

		public static int BindDouble(Sqlite3Statement stmt, int index, double val)
		{
			return Sqlite3.sqlite3_bind_double(stmt, index, val);
		}

		public static int BindText(Sqlite3Statement stmt, int index, string val, int n, IntPtr free)
		{
#if USE_WP8_NATIVE_SQLITE
			return Sqlite3.sqlite3_bind_text(stmt, index, val, n);
#elif USE_SQLITEPCL_RAW
			return Sqlite3.sqlite3_bind_text(stmt, index, val);
#else
			return Sqlite3.sqlite3_bind_text(stmt, index, val, n, null);
#endif
		}

		public static int BindBlob(Sqlite3Statement stmt, int index, byte[] val, int n, IntPtr free)
		{
#if USE_WP8_NATIVE_SQLITE
			return Sqlite3.sqlite3_bind_blob(stmt, index, val, n);
#elif USE_SQLITEPCL_RAW
			return Sqlite3.sqlite3_bind_blob(stmt, index, val);
#else
			return Sqlite3.sqlite3_bind_blob(stmt, index, val, n, null);
#endif
		}

		public static int ColumnCount(Sqlite3Statement stmt)
		{
			return Sqlite3.sqlite3_column_count(stmt);
		}

		public static string ColumnName(Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_name(stmt, index);
		}

		public static string ColumnName16(Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_name(stmt, index);
		}

		public static ColType ColumnType(Sqlite3Statement stmt, int index)
		{
			return (ColType)Sqlite3.sqlite3_column_type(stmt, index);
		}

		public static int ColumnInt(Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_int(stmt, index);
		}

		public static long ColumnInt64(Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_int64(stmt, index);
		}

		public static double ColumnDouble(Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_double(stmt, index);
		}

		public static string ColumnText(Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_text(stmt, index);
		}

		public static string ColumnText16(Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_text(stmt, index);
		}

		public static byte[] ColumnBlob(Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_blob(stmt, index);
		}

		public static int ColumnBytes(Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_bytes(stmt, index);
		}

		public static string ColumnString(Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_text(stmt, index);
		}

		public static byte[] ColumnByteArray(Sqlite3Statement stmt, int index)
		{
			return ColumnBlob(stmt, index);
		}

#if !USE_SQLITEPCL_RAW
		public static Result EnableLoadExtension(Sqlite3DatabaseHandle db, int onoff)
		{
			return (Result)Sqlite3.sqlite3_enable_load_extension(db, onoff);
		}
#endif

		public static ExtendedResult ExtendedErrCode(Sqlite3DatabaseHandle db)
		{
			return (ExtendedResult)Sqlite3.sqlite3_extended_errcode(db);
		}
#endif

        /// <summary>
        /// The col type.
        /// </summary>
        public enum ColType : int
        {
            /// <summary>
            /// The integer.
            /// </summary>
            Integer = 1,

            /// <summary>
            /// The float.
            /// </summary>
            Float = 2,

            /// <summary>
            /// The text.
            /// </summary>
            Text = 3,

            /// <summary>
            /// The blob.
            /// </summary>
            Blob = 4,

            /// <summary>
            /// The null.
            /// </summary>
            Null = 5
        }
    }
}

#if NO_CONCURRENT
namespace SQLite.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ListEx
    {
        public static bool TryAdd<TKey, TValue> (this IDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            try {
                dict.Add (key, value);
                return true;
            }
            catch (ArgumentException) {
                return false;
            }
        }
    }
}
#endif
