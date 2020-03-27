// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseRecordSet.cs" company="Aurea Software Gmbh">
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
//   Defines the generic record set
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.DAL
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the generic record set
    /// </summary>
    public abstract class GenericRecordSet
    {
        // : DatabaseQuery
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericRecordSet"/> class.
        /// </summary>
        /// <param name="db">
        /// The database.
        /// </param>
        protected GenericRecordSet(IDatabase db)
        {
            // : base(db)
        }

        /// <summary>
        /// Gets the column count.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public abstract int GetColumnCount();

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        /// <param name="colNr">
        /// The col nr.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public abstract string GetColumnName(int colNr);

        /// <summary>
        /// Gets the type of the column.
        /// </summary>
        /// <param name="colNr">
        /// The col nr.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public abstract int GetColumnType(int colNr);

        /// <summary>
        /// Gets the row.
        /// </summary>
        /// <param name="rowNum">
        /// The row number.
        /// </param>
        /// <returns>
        /// The <see cref="DatabaseRow"/>.
        /// </returns>
        public abstract DatabaseRow GetRow(int rowNum);

        /// <summary>
        /// Gets the row count.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public abstract int GetRowCount();

        /// <summary>
        /// Gets the skip rows.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public abstract int GetSkipRows();
    }

    /// <summary>
    /// Database record set implementation
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.DAL.GenericRecordSet" />
    public class DatabaseRecordSet : GenericRecordSet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseRecordSet"/> class.
        /// </summary>
        /// <param name="db">
        /// The database.
        /// </param>
        public DatabaseRecordSet(IDatabase db)
            : base(db)
        {
            this.Query = new DatabaseQuery(db);
            this.HasQueryOwnership = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseRecordSet"/> class.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        public DatabaseRecordSet(DatabaseQuery query)
            : base(query?.Database)
        {
            this.Query = query;
            this.HasQueryOwnership = false;
            this.BuildResult(0);
        }

        /// <summary>
        /// Gets a value indicating whether this instance has query ownership.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has query ownership; otherwise, <c>false</c>.
        /// </value>
        public bool HasQueryOwnership { get; }

        /// <summary>
        /// Gets the query.
        /// </summary>
        /// <value>
        /// The query.
        /// </value>
        public DatabaseQuery Query { get; }

        /// <summary>
        /// Gets the row count.
        /// </summary>
        /// <value>
        /// The row count.
        /// </value>
        public int RowCount => this.Rows?.Count ?? 0;

        /// <summary>
        /// Gets the rows.
        /// </summary>
        /// <value>
        /// The rows.
        /// </value>
        public List<DatabaseRow> Rows { get; private set; }

        /// <summary>
        /// Gets the row skip.
        /// </summary>
        /// <value>
        /// The row skip.
        /// </value>
        public int RowSkip { get; private set; }

        /// <summary>
        /// Gets the <see cref="DatabaseRow"/> with the specified row number.
        /// </summary>
        /// <value>
        /// The <see cref="DatabaseRow"/>.
        /// </value>
        /// <param name="rowNum">
        /// The row number.
        /// </param>
        /// <returns>
        /// The <see cref="DatabaseRow"/>.
        /// </returns>
        public DatabaseRow this[int rowNum] => rowNum >= this.RowCount ? new DatabaseRow() : this.Rows[rowNum];

        /// <summary>
        /// Adds the row.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        public virtual void AddRow(DatabaseRow row)
        {
            if (this.Rows == null)
            {
                this.Rows = new List<DatabaseRow>();
            }

            this.Rows.Add(row);
        }

        /// <summary>
        /// Builds the result.
        /// </summary>
        /// <param name="maxRows">
        /// The maximum rows.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int BuildResult(int maxRows)
        {
#if PORTING
            int i, rowLen, rowCount;
            int colCount;
            int[] cellLens;
            int curLen;
            string rowBuffer;
            string cellVal;
          
            Query.MoveNext();
            rowCount = 0;
            colCount = Query.GetColumnCount();

            while (!Query.GetEOF())
            {
                cellLens = new int[colCount];
                rowLen = colCount;
                for (i = 0; i < colCount; i++)
                {
                    cellLens[i] = Query.GetColumnContentLength(i);
                    rowLen += cellLens[i];
                }

                rowBuffer = new char[rowLen];
                rowLen = 0;
                for (i = 0; i < colCount; i++)
                {
                    cellVal = Query.GetColumnContent(i);
                    curLen = cellLens[i];
                    if (cellVal != null)
                    {
                        memcpy(rowBuffer + rowLen, cellVal, curLen /*-1*/);
                    }

                    cellLens[i] = rowLen; // len -> offset
                    rowLen += curLen;
                    rowBuffer[rowLen++] = 0;
                }

                AddRow(new DatabaseRow(rowBuffer, colCount, cellLens));

                rowCount++;
                if (maxRows > 0 && rowCount == maxRows)
                {
                    break;
                }

                Query.MoveNext();
            }
#endif
            return 0;
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int Execute()
        {
            return this.Execute(0);
        }

        /// <summary>
        /// Executes the specified maximum rows.
        /// </summary>
        /// <param name="maxRows">
        /// The maximum rows.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public virtual int Execute(int maxRows)
        {
            if (this.Query == null || !this.Query.ColumnInfoLoaded)
            {
                return 1;
            }

            var results = this.Query.ExecuteQuery(maxRows);

            foreach (var result in results)
            {
                this.AddRow(new DatabaseRow(result));
            }

            return 0;
        }

        /// <summary>
        /// Executes the specified text statement.
        /// </summary>
        /// <param name="txtStatement">
        /// The text statement.
        /// </param>
        /// <param name="maxRows">
        /// The maximum rows.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int Execute(string txtStatement, int maxRows = 0)
        {
            var ret = this.Query?.Prepare(txtStatement) ?? false;
            return ret ? this.Execute(maxRows) : 1;
        }

        /// <summary>
        /// Executes the specified text statement.
        /// </summary>
        /// <param name="txtStatement">
        /// The text statement.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="maxRows">
        /// The maximum rows.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int Execute(string txtStatement, string[] parameters, int maxRows)
        {
            var ret = this.Query?.Prepare(txtStatement) ?? false;
            if (!ret)
            {
                return 1;
            }

            if (parameters?.Length > 0)
            {
                foreach (var parameter in parameters)
                {
                    this.Query.Bind(parameter);
                }
            }

            return this.Execute(maxRows);
        }

        /// <summary>
        /// Gets the column count.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int GetColumnCount() => this.Query.GetColumnCount();

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        /// <param name="colNr">
        /// The col nr.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string GetColumnName(int colNr) => this.Query.GetColumnName(colNr);

        /// <summary>
        /// Gets the column nr.
        /// </summary>
        /// <param name="columnName">
        /// Name of the column.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetColumnNr(string columnName) => this.Query.GetColumnNr(columnName);

        /// <summary>
        /// Gets the type of the column.
        /// </summary>
        /// <param name="colNr">
        /// The col nr.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int GetColumnType(int colNr) => (int)this.Query.GetColumnType(colNr);

        /// <summary>
        /// Gets the row.
        /// </summary>
        /// <param name="rowNum">
        /// The row number.
        /// </param>
        /// <returns>
        /// The <see cref="DatabaseRow"/>.
        /// </returns>
        public override DatabaseRow GetRow(int rowNum) => rowNum >= this.RowCount ? null : this.Rows[rowNum];

        /// <summary>
        /// Gets the row count.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int GetRowCount() => this.RowCount;

        /// <summary>
        /// Gets the skip rows.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int GetSkipRows() => this.RowSkip;

        /// <summary>
        /// Takes the row ownership.
        /// </summary>
        /// <param name="rowNum">
        /// The row number.
        /// </param>
        /// <returns>
        /// The <see cref="DatabaseRow"/>.
        /// </returns>
        public DatabaseRow TakeRowOwnership(int rowNum)
        {
            if (rowNum >= this.RowCount)
            {
                return null;
            }

            var row = this.Rows[rowNum];
            this.Rows[rowNum] = null;
            return row;
        }
    }
}
