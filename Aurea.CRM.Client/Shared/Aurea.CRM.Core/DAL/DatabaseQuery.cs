// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseQuery.cs" company="Aurea Software Gmbh">
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
//   Implements a data base query which returns results
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.DAL
{
    /// <summary>
    /// Implements a data base query which returns results
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.DAL.DatabaseStatement" />
    public class DatabaseQuery : DatabaseStatement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseQuery"/> class.
        /// </summary>
        /// <param name="database">
        /// The database.
        /// </param>
        public DatabaseQuery(IDatabase database)
            : base(database)
        {
            // : base (PlatformPath(databaseName, recreate), SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create)
#if PORTING
            Types = 0;
            ColumnNames = 0;
            Eof = true;
#endif
        }

        /// <summary>
        /// Existses the row.
        /// </summary>
        /// <param name="id">
        /// The identifier.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public virtual bool ExistsRow(string id)
        {
            this.ClearBindings();
            this.Bind(id);
            var result = this.ExecuteScalar<string>();

            return result != null;
        }
    }
}
