// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SQLiteORMExt.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   Extends the SQLiteORM by providing generic query features
// </summary>
// <author>
//    Rashan Anushka
// </author>
// --------------------------------------------------------------------------------------------------------------------

namespace SQLite
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Extended version of the Column info
    /// </summary>
    public class ColumnInfoExt
    {
        /// <summary>
        /// Gets the type of the column color.
        /// </summary>
        /// <value>
        /// The type of the column color.
        /// </value>
        [JsonIgnore]
        public Type ColumnClrType
        {
            get
            {
                switch (this.ColumnMainType)
                {
                    case SQLite3.ColType.Integer:
                        return typeof(long);

                    case SQLite3.ColType.Float:
                        return typeof(double);

                    case SQLite3.ColType.Text:
                        return typeof(string);

                    case SQLite3.ColType.Blob:
                        return typeof(byte[]);

                    case SQLite3.ColType.Null:
                        return typeof(object);

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Gets or sets the index of the column.
        /// </summary>
        /// <value>
        /// The index of the column.
        /// </value>
        public int ColumnIndex { get; set; }

        /// <summary>
        /// Gets or sets the type of the column main.
        /// </summary>
        /// <value>
        /// The type of the column main.
        /// </value>
        public SQLite3.ColType ColumnMainType { get; set; }

        /// <summary>
        /// Gets or sets the type of the column.
        /// </summary>
        /// <value>
        /// The type of the column.
        /// </value>
        [Column("type")]
        public string ColumnType { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.Name;
        }
    }

    /// <summary>
    /// Dictionary representation of a result row
    /// </summary>
    /// <seealso cref="System.Collections.Generic.Dictionary{TKey,TValue}" /> with <c>TKey</c> being <see cref="int"/> and <c>TValue</c> is <see cref="object"/>
    public partial class ResultRow : Dictionary<int, object>
    {
    }

    /// <summary>
    /// Collection f result rowsd
    /// </summary>
    /// <seealso cref="ResultRow" />
    public partial class ResultCollection : List<ResultRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResultCollection"/> class.
        /// </summary>
        /// <param name="columns">
        /// The columns.
        /// </param>
        public ResultCollection(IEnumerable<ColumnInfoExt> columns)
        {
            this.Columns = new List<ColumnInfoExt>(columns);
        }

        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>
        /// The columns.
        /// </value>
        public List<ColumnInfoExt> Columns { get; private set; }
    }

    /// <summary>
    /// Extending the SQLite command
    /// </summary>
    public partial class SQLiteCommand
    {
        /// <summary>
        /// Binds the specified index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="val">
        /// The value.
        /// </param>
        public void Bind(int index, object val)
        {
            this._bindings.Add(new Binding { Name = null, Value = val, Index = index });
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="maxRows">
        /// The maximum rows.
        /// </param>
        /// <returns>
        /// The <see cref="ResultCollection"/>.
        /// </returns>
        public virtual ResultCollection ExecuteQuery(int maxRows)
        {
            ResultCollection results;
            IntPtr stmt = IntPtr.Zero;

            try
            {
                stmt = this.Prepare();
                var cols = new ColumnInfoExt[SQLite3.ColumnCount(stmt)];

                for (var i = 0; i < cols.Length; i++)
                {
                    cols[i] = new ColumnInfoExt { Name = SQLite3.ColumnName16(stmt, i), ColumnIndex = i };
                }

                results = new ResultCollection(cols);

                var rows = 0;
                while (SQLite3.Step(stmt) == SQLite3.Result.Row)
                {
                    var obj = new ResultRow();
                    for (var i = 0; i < cols.Length; i++)
                    {
                        if (cols[i] == null)
                        {
                            continue;
                        }

                        var colType = SQLite3.ColumnType(stmt, i);
                        cols[i].ColumnMainType = colType;

                        var val = this.ReadCol(stmt, i, colType, cols[i].ColumnClrType);
                        obj[cols[i].ColumnIndex] = val;
                    }

                    results.Add(obj);

                    if (maxRows != 0 && ++rows == maxRows)
                    {
                        break;
                    }
                }
            }
            finally
            {
                if (stmt != IntPtr.Zero)
                {
                    SQLite3.Finalize(stmt);
                }
            }

            return results;
        }

        /// <summary>
        /// Prepares the command.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        public IntPtr PrepareCommand(string command)
        {
            if (!string.IsNullOrEmpty(command))
            {
                this.CommandText = command;
            }

            var stmt = SQLite3.Prepare2(this._conn.Handle, this.CommandText);

            // BindAll(stmt);
            return stmt;
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public virtual void Reset()
        {
            this._bindings?.Clear();
        }
    }

    /// <summary>
    /// Extending the SQLite connection
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public partial class SQLiteConnection
    {
        /// <summary>
        /// Gets the last row identifier.
        /// </summary>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        public long GetLastRowId() => SQLite3.LastInsertRowid(this.Handle);

        /// <summary>
        /// Gets the table information extended.
        /// </summary>
        /// <param name="tableName">
        /// Name of the table.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<ColumnInfoExt> GetTableInfoExtended(string tableName)
        {
            var query = "pragma table_info(\"" + tableName + "\")";
            return this.Query<ColumnInfoExt>(query);
        }
    }
}
