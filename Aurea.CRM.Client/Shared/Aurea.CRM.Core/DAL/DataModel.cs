// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataModel.cs" company="Aurea Software Gmbh">
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
//   Implements the data access data model
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.DAL
{
    using System.Collections.Generic;
    using System.Linq;

    using Aurea.CRM.Core.CRM;

    /// <summary>
    /// Implements the data access data model
    /// </summary>
    public class DataModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataModel"/> class.
        /// </summary>
        /// <param name="crmDatabase">
        /// The CRM database.
        /// </param>
        public DataModel(CRMDatabase crmDatabase)
        {
            this.Unsorted = false;
            this.TableInfos = null;
            this.Database = crmDatabase;
        }

        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        /// <value>
        /// The database.
        /// </value>
        public CRMDatabase Database { get; set; }

        /// <summary>
        /// Gets the fixcat count.
        /// </summary>
        /// <value>
        /// The fixcat count.
        /// </value>
        public int FixcatCount => this.FixCats?.Count ?? 0;

        /// <summary>
        /// Gets or sets the fix cats.
        /// </summary>
        /// <value>
        /// The fix cats.
        /// </value>
        public List<FixedCatalogInfo> FixCats { get; set; }

        /// <summary>
        /// Gets the table count.
        /// </summary>
        /// <value>
        /// The table count.
        /// </value>
        public int TableCount => this.TableInfos?.Count ?? 0;

        /// <summary>
        /// Gets or sets the table infos.
        /// </summary>
        /// <value>
        /// The table infos.
        /// </value>
        public List<TableInfo> TableInfos { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="DataModel"/> is unsorted.
        /// </summary>
        /// <value>
        /// <c>true</c> if unsorted; otherwise, <c>false</c>.
        /// </value>
        public bool Unsorted { get; set; }

        /// <summary>
        /// Gets the varcat count.
        /// </summary>
        /// <value>
        /// The varcat count.
        /// </value>
        public int VarcatCount => this.VarCats?.Count ?? 0;

        /// <summary>
        /// Gets or sets the variable cats.
        /// </summary>
        /// <value>
        /// The variable cats.
        /// </value>
        public List<VariableCatalogInfo> VarCats { get; set; }

        /// <summary>
        /// Adds the catalog with ownership.
        /// </summary>
        /// <param name="cataloginfo">
        /// The cataloginfo.
        /// </param>
        /// <returns>
        /// The <see cref="FixedCatalogInfo"/>.
        /// </returns>
        public FixedCatalogInfo AddCatalogWithOwnership(FixedCatalogInfo cataloginfo)
        {
            if (this.FixCats == null)
            {
                this.FixCats = new List<FixedCatalogInfo>();
            }

            this.FixCats.Add(cataloginfo);
            this.Unsorted = true;
            return cataloginfo;
        }

        /// <summary>
        /// Adds the fixed catalog.
        /// </summary>
        /// <param name="catalogNr">
        /// The catalog nr.
        /// </param>
        /// <returns>
        /// The <see cref="FixedCatalogInfo"/>.
        /// </returns>
        public FixedCatalogInfo AddFixedCatalog(int catalogNr)
        {
            return this.AddCatalogWithOwnership(new FixedCatalogInfo(this.Database, catalogNr));
        }

        /// <summary>
        /// Adds the table information.
        /// </summary>
        /// <param name="src">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="TableInfo"/>.
        /// </returns>
        public TableInfo AddTableInfo(TableInfo src)
        {
            return src == null ? null : this.AddTableInfoWithOwnership(src); ;//.CreateCopy()); - TBD : Check if this helps fix memory leals
        }

        /// <summary>
        /// Adds the table information.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="rootInfoAreaId">
        /// The root information area identifier.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="haslookup">
        /// The haslookup.
        /// </param>
        /// <returns>
        /// The <see cref="TableInfo"/>.
        /// </returns>
        public TableInfo AddTableInfo(string infoAreaId, string rootInfoAreaId, string name, int haslookup)
        {
            return
                this.AddTableInfoWithOwnership(
                    new TableInfo(
                        infoAreaId,
                        rootInfoAreaId,
                        this.GetRootPhysicalInfoAreaId(rootInfoAreaId),
                        name,
                        haslookup));
        }

        /// <summary>
        /// Adds the table information with ownership.
        /// </summary>
        /// <param name="tableInfo">
        /// The table information.
        /// </param>
        /// <returns>
        /// The <see cref="TableInfo"/>.
        /// </returns>
        public TableInfo AddTableInfoWithOwnership(TableInfo tableInfo)
        {
            this.Unsorted = true;
            if (this.TableInfos == null)
            {
                this.TableInfos = new List<TableInfo>();
            }

            this.TableInfos.Add(tableInfo);
            return tableInfo;
        }

        /// <summary>
        /// Adds the variable catalog with ownership.
        /// </summary>
        /// <param name="cataloginfo">
        /// The cataloginfo.
        /// </param>
        /// <returns>
        /// The <see cref="VariableCatalogInfo"/>.
        /// </returns>
        public VariableCatalogInfo AddVarCatalogWithOwnership(VariableCatalogInfo cataloginfo)
        {
            if (this.VarCats == null)
            {
                this.VarCats = new List<VariableCatalogInfo>();
            }

            this.VarCats.Add(cataloginfo);
            return cataloginfo;
        }

        /// <summary>
        /// Adds the variable catalog.
        /// </summary>
        /// <param name="catalogNr">
        /// The catalog nr.
        /// </param>
        /// <returns>
        /// The <see cref="VariableCatalogInfo"/>.
        /// </returns>
        public VariableCatalogInfo AddVariableCatalog(int catalogNr)
        {
            return this.AddVarCatalogWithOwnership(new VariableCatalogInfo(this.Database, catalogNr));
        }

        /// <summary>
        /// Adds the variable catalog.
        /// </summary>
        /// <param name="catalogNr">
        /// The catalog nr.
        /// </param>
        /// <param name="parentCatalogNr">
        /// The parent catalog nr.
        /// </param>
        /// <returns>
        /// The <see cref="VariableCatalogInfo"/>.
        /// </returns>
        public VariableCatalogInfo AddVariableCatalog(int catalogNr, int parentCatalogNr)
        {
            return this.AddVarCatalogWithOwnership(new DependentCatalogInfo(this.Database, catalogNr, parentCatalogNr));
        }

        /// <summary>
        /// Adds the virtual link with ownership.
        /// </summary>
        /// <param name="virtualLinkInfo">
        /// The virtual link information.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool AddVirtualLinkWithOwnership(VirtualLinkInfo virtualLinkInfo)
        {
            if (virtualLinkInfo == null || !virtualLinkInfo.IsValid)
            {
                return false;
            }

            var tableInfo = this.InternalGetTableInfo(virtualLinkInfo.InfoAreaId);
            return tableInfo.AddVirtualLinkWithOwnership(virtualLinkInfo);
        }

        /// <summary>
        /// Ensures the DDL.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int EnsureDDL()
        {
            var ret = 0;
            for (var i = 0; i < this.TableCount; i++)
            {
                var tableinfo = this.TableInfos[i];
                if (this.Database.ExistsTable(tableinfo.DatabaseTableName))
                {
                    var metainfo = this.Database.CreateTableMetaInfo(tableinfo.DatabaseTableName);
                    var statements = tableinfo.CreateAlterTableStatements(metainfo);
                    if (statements != null)
                    {
                        ret = 0;
                        for (var j = 0; ret == 0 && j < statements.Count; j++)
                        {
                            ret = this.Database.Execute(statements[j]);
                        }
                    }
                }
                else
                {
                    var txtStatement = tableinfo.CreateCreateTableStatement();
                    if (txtStatement != null)
                    {
                        var parts = txtStatement.Split(';');
                        foreach (var stmt in parts)
                        {
                            ret = this.Database.Execute(stmt);

                            if (ret != 0)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            for (var i = 0; ret == 0 && i < this.FixcatCount; i++)
            {
                if (this.Database.ExistsTable(this.FixCats[i].GetDatabaseTableName()))
                {
                    var txtStatement = this.FixCats[i].GetDropTableStatement();
                    ret = this.Database.Execute(txtStatement);
                }

                if (ret == 0)
                {
                    var txtStatement = this.FixCats[i].GetCreateTableStatement();
                    ret = this.Database.Execute(txtStatement);
                }
            }

            for (var i = 0; ret == 0 && i < this.VarcatCount; i++)
            {
                if (this.Database.ExistsTable(this.VarCats[i].GetDatabaseTableName()))
                {
                    var txtStatement = this.VarCats[i].GetDropTableStatement();
                    ret = this.Database.Execute(txtStatement);
                }

                if (ret == 0)
                {
                    var txtStatement = this.VarCats[i].GetCreateTableStatement();
                    ret = this.Database.Execute(txtStatement);
                }
            }

            return ret;
        }

        /// <summary>
        /// Gets the field information.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <returns>
        /// The <see cref="FieldInfo"/>.
        /// </returns>
        public FieldInfo GetFieldInfo(string infoAreaId, int fieldId)
        {
            var tableInfo = this.InternalGetTableInfo(infoAreaId);
            return tableInfo?.GetFieldInfo(fieldId);
        }

        /// <summary>
        /// Gets the fix cat.
        /// </summary>
        /// <param name="catalogNr">
        /// The catalog nr.
        /// </param>
        /// <returns>
        /// The <see cref="FixedCatalogInfo"/>.
        /// </returns>
        public FixedCatalogInfo GetFixCat(int catalogNr)
        {
            return this.FixCats?.FirstOrDefault(v => v.CatalogNr == catalogNr);
        }

        /// <summary>
        /// Gets the root physical information area identifier.
        /// </summary>
        /// <param name="rootInfoAreaId">
        /// The root information area identifier.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetRootPhysicalInfoAreaId(string rootInfoAreaId)
        {
            if (!string.IsNullOrEmpty(rootInfoAreaId) && this.Database.IsUpdateCrm && Equals(rootInfoAreaId, "KP"))
            {
                return "CP";
            }

            return rootInfoAreaId;
        }

        /// <summary>
        /// Gets the table information.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="TableInfo"/>.
        /// </returns>
        public TableInfo GetTableInfo(int index)
        {
            return index < this.TableCount ? this.TableInfos.ElementAt(index) : null;
        }

        /// <summary>
        /// Gets the variable cat.
        /// </summary>
        /// <param name="catalogNr">
        /// The catalog nr.
        /// </param>
        /// <returns>
        /// The <see cref="VariableCatalogInfo"/>.
        /// </returns>
        public VariableCatalogInfo GetVarCat(int catalogNr)
        {
            return this.VarCats?.FirstOrDefault(v => v.CatalogNr == catalogNr);
        }

        /// <summary>
        /// Determines whether [has lookup records] [the specified information area identifier].
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HasLookupRecords(string infoAreaId)
        {
            TableInfo tableInfo = this.InternalGetTableInfo(infoAreaId);
            return tableInfo == null || tableInfo.HasLookup;
        }

        /// <summary>
        /// Internals the get table information.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <returns>
        /// The <see cref="TableInfo"/>.
        /// </returns>
        public TableInfo InternalGetTableInfo(string infoAreaId)
        {
            if (string.IsNullOrEmpty(infoAreaId))
            {
                return null;
            }

            return this.TableInfos?.Find(i => Equals(i.InfoAreaId, infoAreaId));
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int Load()
        {
            var tableRecordSet = new DatabaseRecordSet(this.Database);
            tableRecordSet.Query.Prepare("SELECT infoareaid, rootinfoareaid, name, haslookup FROM tableinfo");

            TableInfo tableinfo;
            DatabaseRow row;
            var ret = tableRecordSet.Execute();
            var tableInfoCount = tableRecordSet.GetRowCount();

            for (var i = 0; i < tableInfoCount; i++)
            {
                row = tableRecordSet.GetRow(i);
                tableinfo = this.AddTableInfo(
                    row.GetColumn(0),
                    row.GetColumn(1),
                    row.GetColumn(2),
                    row.GetColumnInt(3, 0));
                tableinfo.Load(this.Database);
            }

            for (var i = 0; i < tableInfoCount; i++)
            {
                tableinfo = this.TableInfos[i];
                if (!string.IsNullOrEmpty(tableinfo.RootInfoAreaId))
                {
                    var parent = this.Database.GetTableInfoByInfoArea(tableinfo.RootInfoAreaId);
                    parent?.AddSpecialInfoArea(tableinfo);
                }
            }

            var catalogRecordSet = new DatabaseRecordSet(this.Database);
            catalogRecordSet.Query.Prepare("SELECT catnr FROM fixcatinfo");
            catalogRecordSet.Execute();

            var catalogInfoCount = catalogRecordSet.GetRowCount();
            for (var i = 0; i < catalogInfoCount; i++)
            {
                row = catalogRecordSet.GetRow(i);
                this.AddFixedCatalog(row.GetColumnInt(0));
            }

            catalogRecordSet = new DatabaseRecordSet(this.Database);
            catalogRecordSet.Query.Prepare("SELECT catnr, parentcatnr FROM varcatinfo");
            if (ret == 0)
            {
                ret = catalogRecordSet.Execute();
            }

            catalogInfoCount = catalogRecordSet.GetRowCount();
            for (var i = 0; i < catalogInfoCount; i++)
            {
                row = catalogRecordSet.GetRow(i);
                if (row.IsNull(1))
                {
                    this.AddVariableCatalog(row.GetColumnInt(0));
                }
                else
                {
                    this.AddVariableCatalog(row.GetColumnInt(0), row.GetColumnInt(1));
                }
            }

            this.Sort(true);

            return ret;
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int Save()
        {
            this.Database.BeginTransaction();

            this.Database.EmptyTable("fieldinfo");
            this.Database.EmptyTable("tableinfo");
            this.Database.EmptyTable("fixcatinfo");
            this.Database.EmptyTable("varcatinfo");
            this.Database.EmptyTable("linkinfo");
            this.Database.EmptyTable("linkfieldinfo");

            var tableInsertStatement = this.Database.CreateInsertTableInfoStatement();
            var fieldInsertStatement = this.Database.CreateInsertFieldInfoStatement();
            var linkInsertStatement = this.Database.CreateInsertLinkInfoStatement();
            var linkFieldInsertStatement = this.Database.CreateInsertLinkFieldInfoStatement();

            if (tableInsertStatement != null && fieldInsertStatement != null && linkInsertStatement != null)
            {
                for (var i = 0; i < this.TableCount; i++)
                {
                    this.Database.WriteTableInfo(
                        this.TableInfos.ElementAt(i),
                        tableInsertStatement,
                        fieldInsertStatement,
                        linkInsertStatement,
                        linkFieldInsertStatement);
                }
            }

            if (this.FixcatCount > 0)
            {
                var catStatement = this.Database.CreateInsertFixCatStatement();
                if (catStatement != null)
                {
                    for (var i = 0; i < this.FixcatCount; i++)
                    {
                        this.Database.WriteFixCatInfo(catStatement, this.FixCats.ElementAt(i));
                    }
                }
            }

            if (this.VarcatCount > 0)
            {
                var catStatement = this.Database.CreateInsertVarCatStatement();
                if (catStatement != null)
                {
                    for (var i = 0; i < this.VarcatCount; i++)
                    {
                        this.Database.WriteVarCatInfo(catStatement, this.VarCats.ElementAt(i));
                    }
                }
            }

            this.Database.Commit();
            return 0;
        }

        /// <summary>
        /// Sets the catalog information from data model.
        /// </summary>
        public void SetCatalogInfoFromDataModel()
        {
            for (var i = 0; i < this.TableCount; i++)
            {
                var tableInfo = this.TableInfos[i];
                var fieldInfoCount = tableInfo.FieldCount;

                for (var j = 0; j < fieldInfoCount; j++)
                {
                    var fieldInfo = tableInfo.GetFieldInfoByIndex(j);
                    switch (fieldInfo.FieldType)
                    {
                        case 'K':
                            if (this.GetVarCat(fieldInfo.Cat) == null)
                            {
                                var ucat = fieldInfo.UCat;
                                if (ucat >= 0)
                                {
                                    var parentFieldInfo = tableInfo.GetFieldInfo(ucat);
                                    if (parentFieldInfo != null)
                                    {
                                        this.AddVariableCatalog(fieldInfo.Cat, parentFieldInfo.Cat);
                                    }
                                }
                                else
                                {
                                    this.AddVariableCatalog(fieldInfo.Cat);
                                }
                            }

                            break;
                        case 'X':
                            if (this.GetFixCat(fieldInfo.Cat) == null)
                            {
                                this.AddFixedCatalog(fieldInfo.Cat);
                            }

                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the has lookup records.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool SetHasLookupRecords(string infoAreaId)
        {
            var tableInfo = this.InternalGetTableInfo(infoAreaId);
            if (tableInfo != null)
            {
                // .SetHasLookupRecords(1);
                tableInfo.HasLookup = true;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sorts the specified sort children.
        /// </summary>
        /// <param name="sortChildren">
        /// if set to <c>true</c> [sort children].
        /// </param>
        public void Sort(bool sortChildren)
        {
            if (!this.Unsorted || this.TableCount == 0)
            {
                return;
            }

            if (this.TableInfos != null && this.TableCount > 0)
            {
                this.TableInfos.Sort((t1, t2) => string.CompareOrdinal(t1.InfoAreaId, t2.InfoAreaId));
            }

            if (this.FixCats != null && this.FixcatCount > 0)
            {
                this.FixCats.Sort((c1, c2) => c1.CatalogNr - c2.CatalogNr);
            }

            if (this.VarCats != null && this.VarcatCount > 0)
            {
                this.VarCats.Sort((c1, c2) => c1.CatalogNr - c2.CatalogNr);
            }

            this.Unsorted = false;

            if (!sortChildren || this.TableInfos == null)
            {
                return;
            }

            foreach (var tableInfo in this.TableInfos)
            {
                tableInfo.Sort();
            }
        }
    }
}
