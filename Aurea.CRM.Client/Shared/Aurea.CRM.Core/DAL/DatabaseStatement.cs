// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseStatement.cs" company="Aurea Software Gmbh">
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
//   Implements database statement functionality
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.DAL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SQLite;

    /// <summary>
    /// Implements database statement functionality
    /// </summary>
    /// <seealso cref="SQLite.SQLiteCommand" />
    public class DatabaseStatement : SQLiteCommand
    {
        private string statement = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseStatement"/> class.
        /// </summary>
        /// <param name="database">
        /// The database.
        /// </param>
        public DatabaseStatement(IDatabase database)
            : base(database.GetConnection())
        {
            this.Database = database;
        }

        /// <summary>
        /// Gets a value indicating whether the current statement has column info loaded
        /// </summary>
        public bool ColumnInfoLoaded { get; private set; }

        /// <summary>
        /// Gets or sets the column names.
        /// </summary>
        /// <value>
        /// The column names.
        /// </value>
        public List<string> ColumnNames { get; protected set; }

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>
        /// The columns.
        /// </value>
        public ColumnInfoExt[] Columns { get; protected set; }

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <value>
        /// The database.
        /// </value>
        public IDatabase Database { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="DatabaseStatement"/> is executed.
        /// </summary>
        /// <value>
        /// <c>true</c> if executed; otherwise, <c>false</c>.
        /// </value>
        public bool Executed { get; private set; }

        /// <summary>
        /// Executes the specified parameters.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int Execute(params object[] parameters)
        {
            if (parameters.Length != 0)
            {
                // Clearing bindings list before adding new bindings
                this.ClearBindings();

                foreach (var parameter in parameters)
                {
                    this.Bind(parameter);
                }
            }

            try
            {
                this.ExecuteNonQuery();
                this.Executed = true;
                return 0;
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex);
                return 1;
            }
        }

        /// <summary>
        /// Executes the specified statement.
        /// </summary>
        /// <param name="statement">The statement.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>0 if success, else error nuumber</returns>
        public int Execute(string statement, params object[] parameters)
        {
            IntPtr stmt = IntPtr.Zero;

            try
            {
                stmt = this.PrepareCommand(statement);

                this.LoadColumnInfo();

                if (parameters.Length != 0)
                {
                    foreach (var parameter in parameters)
                    {
                        this.Bind(parameter);
                    }
                }

                this.ExecuteNonQuery();
                
                return 0;
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex);
                return 1;
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
        /// Gets the column count.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetColumnCount()
        {
            IntPtr statement = IntPtr.Zero;

            try
            {
                statement = this.GetPreparedStatement();
                return SQLite3.ColumnCount(this.GetPreparedStatement());
            }
            finally
            {
                if (statement != IntPtr.Zero)
                {
                    SQLite3.Finalize(statement);
                }
            }
        }

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        /// <param name="colNr">
        /// The col nr.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetColumnName(int colNr)
        {
            IntPtr statement = IntPtr.Zero;

            try
            {
                statement = this.GetPreparedStatement();
                return SQLite3.ColumnName16(this.GetPreparedStatement(), colNr);
            }
            finally
            {
                if (statement != IntPtr.Zero)
                {
                    SQLite3.Finalize(statement);
                }
            }
        }

        /// <summary>
        /// Gets the column nr.
        /// </summary>
        /// <param name="columnName">
        /// Name of the column.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetColumnNr(string columnName) => this.ColumnNames?.IndexOf(columnName) ?? -1;

        /// <summary>
        /// Gets the type of the column.
        /// </summary>
        /// <param name="colNr">
        /// The col nr.
        /// </param>
        /// <returns>
        /// The <see cref="ColType"/>.
        /// </returns>
        public SQLite3.ColType GetColumnType(int colNr)
        {
            IntPtr statement = IntPtr.Zero;

            try
            {
                statement = this.GetPreparedStatement();
                return SQLite3.ColumnType(statement, colNr);
            }
            finally
            {
                if (statement != IntPtr.Zero)
                {
                    SQLite3.Finalize(statement);
                }
            }
        }

        /// <summary>
        /// Loads column info
        /// </summary>
        /// <param name="statement">
        /// The statement.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Prepare(string statement)
        {
            this.statement = statement;
            this.ColumnInfoLoaded = false;
            try
            {
                this.LoadColumnInfo();
                this.ColumnInfoLoaded = true;
                return true;
            }
            catch (Exception ex)
            {
                Logger?.LogError($"Error executing statement: {this.statement}");
                Logger?.LogError(ex);
                return false;
            }
        }

        /// <summary>
        /// Gets the prepared statement.
        /// </summary>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        protected IntPtr GetPreparedStatement()
        {
            return this.PrepareCommand(this.statement);
        }

        /// <summary>
        /// Loads the column information.
        /// </summary>
        private void LoadColumnInfo()
        {
            var stmt = this.GetPreparedStatement();

            try
            {
                var cols = new ColumnInfoExt[SQLite3.ColumnCount(stmt)];

                for (var i = 0; i < cols.Length; i++)
                {
                    cols[i] = new ColumnInfoExt { Name = SQLite3.ColumnName16(stmt, i) };
                }

                this.Columns = cols;
                if (this.Columns != null)
                {
                    this.ColumnNames = this.Columns.Select(c => c.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex);
            }
            finally
            {
                SQLite3.Finalize(stmt);
            }
        }
    }
}
