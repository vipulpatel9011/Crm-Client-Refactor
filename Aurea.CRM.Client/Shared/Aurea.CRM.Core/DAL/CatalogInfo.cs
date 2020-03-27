// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CatalogInfo.cs" company="Aurea Software Gmbh">
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
//   Catalog Info data access implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.DAL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Aurea.CRM.Core.CRM;

    /// <summary>
    /// Catalog Info data access implementation
    /// </summary>
    public class CatalogInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogInfo"/> class.
        /// </summary>
        /// <param name="database">
        /// The database.
        /// </param>
        /// <param name="catalogNr">
        /// The catalog nr.
        /// </param>
        public CatalogInfo(CRMDatabase database, int catalogNr)
        {
            this.CatalogNr = catalogNr;
            this.Database = database;
        }

        /// <summary>
        /// Gets the catalog nr.
        /// </summary>
        /// <value>
        /// The catalog nr.
        /// </value>
        public int CatalogNr { get; }

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <value>
        /// The database.
        /// </value>
        public CRMDatabase Database { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is dependent.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is dependent; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsDependent => false;

        /// <summary>
        /// Gets a value indicating whether this instance is fixed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is fixed; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsFixed => false;

        /// <summary>
        /// Gets a value indicating whether this instance is variable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is variable; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsVariable => !this.IsFixed;

        /// <summary>
        /// Gets or sets the parent catalog nr.
        /// </summary>
        /// <value>
        /// The parent catalog nr.
        /// </value>
        public int ParentCatalogNr { get; protected set; }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        public CatalogValueSet Values { get; private set; }

        /// <summary>
        /// Creates the value set.
        /// </summary>
        /// <param name="parentCode">
        /// The parent code.
        /// </param>
        /// <returns>
        /// The <see cref="CatalogValueSet"/>.
        /// </returns>
        public virtual CatalogValueSet CreateValueSet(int parentCode)
        {
            // bool variable, dep;
            // SYS_CHAR tableName[MAX_CATALOG_TABLENAME];
            var catalogValueRecordSet = new DatabaseRecordSet(this.Database);

            var memorybuffer = new StringBuilder();
            memorybuffer.Append("SELECT code, text, sortinfo, access");

            if (this.IsVariable)
            {
                memorybuffer.Append(", extkey, tenant");
            }

            if (this.IsDependent)
            {
                memorybuffer.Append(", parentcode");
            }

            memorybuffer.Append(" FROM ");
            memorybuffer.Append(this.GetDatabaseTableName());

            var parameterCount = 0;
            if (this.IsDependent && parentCode >= 0)
            {
                memorybuffer.Append($" WHERE parentcode = {parentCode}");
                parameterCount = 1;
            }

            var returnString = memorybuffer.ToString();

            int ret;
            CatalogValueSet values = null;
            if (parameterCount > 0)
            {
                ret = catalogValueRecordSet.Execute(returnString, new[] { $"{parentCode}" }, 0);
            }
            else
            {
                ret = catalogValueRecordSet.Query.Prepare(returnString) ? 0 : 1;
                if (ret == 0)
                {
                    ret = catalogValueRecordSet.Execute();
                }
            }

            if (ret == 0)
            {
                values = new CatalogValueSet { SortFunction = this.GetSortFunction };

                int i, catalogValueCount = catalogValueRecordSet.RowCount;
                DatabaseRow row;
                for (i = 0; i < catalogValueCount; i++)
                {
                    row = catalogValueRecordSet.GetRow(i);
                    if (this.IsDependent)
                    {
                        values.AddDependentCatalogValue(
                            row.GetColumnInt(0),
                            row.GetColumnInt(6),
                            row.GetColumn(1),
                            row.GetColumnInt(5),
                            row.GetColumn(4),
                            row.GetColumnInt(2),
                            row.GetColumnInt(3));
                    }
                    else if (this.IsVariable)
                    {
                        values.AddVariableCatalogValue(
                            row.GetColumnInt(0),
                            row.GetColumn(1),
                            row.GetColumnInt(5),
                            row.GetColumn(4),
                            row.GetColumnInt(2),
                            row.GetColumnInt(3));
                    }
                    else
                    {
                        values.AddFixedCatalogValue(
                            row.GetColumnInt(0),
                            row.GetColumn(1),
                            row.GetColumnInt(2),
                            row.GetColumnInt(3));
                    }
                }
            }

            if (values == null)
            {
                values = new CatalogValueSet();
            }

            values.Sort();
            return values;
        }

        /// <summary>
        /// Gets the catalog text.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public virtual string GetCatalogText(int code)
        {
            if (this.Values == null)
            {
                this.GetValueSet();
            }

            return this.Values?.GetCatalogText(code);
        }

        /// <summary>
        /// Gets the catalog value.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="CatalogValue"/>.
        /// </returns>
        public virtual CatalogValue GetCatalogValue(int code)
        {
            return this.GetValueSet()?.GetCatalogValue(code);
        }

        /// <summary>
        /// Gets the name of the code column.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public virtual string GetCodeColumnName() => "code";

        /// <summary>
        /// Gets the create table statement.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public virtual string GetCreateTableStatement()
        {
            var memorybuffer = new StringBuilder();
            memorybuffer.Append("CREATE TABLE ");
            memorybuffer.Append(this.GetDatabaseTableName());
            memorybuffer.Append("(code INTEGER, text TEXT COLLATE NOCASE");

            if (this.IsVariable)
            {
                memorybuffer.Append(", extkey TEXT, tenant INTEGER");
            }

            if (this.IsDependent)
            {
                memorybuffer.Append(", parentcode INTEGER");
            }

            memorybuffer.Append(", sortinfo INTEGER, access INTEGER");
            memorybuffer.Append(")");

            return memorybuffer.ToString();
        }

        /// <summary>
        /// Gets the name of the database table.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public virtual string GetDatabaseTableName() => null;

        /// <summary>
        /// Gets the drop table statement.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public virtual string GetDropTableStatement()
        {
            // SYS_CHAR catalogTableName[MAX_CATALOG_TABLENAME];
            return $"DROP TABLE {this.GetDatabaseTableName()}";
        }

        /// <summary>
        /// Gets the external key.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public virtual string GetExternalKey(int code)
        {
            return null;
        }

        /// <summary>
        /// Gets the sort function.
        /// </summary>
        /// <param name="c1">
        /// The c1.
        /// </param>
        /// <param name="c2">
        /// The c2.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public virtual int GetSortFunction(CatalogValue c1, CatalogValue c2) => 0;

        /// <summary>
        /// Gets the name of the sort information column.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public virtual string GetSortInfoColumnName() => "sortinfo";

        /// <summary>
        /// Gets the name of the text column.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public virtual string GetTextColumnName() => "text";

        /// <summary>
        /// Gets the value set.
        /// </summary>
        /// <returns>
        /// The <see cref="CatalogValueSet"/>.
        /// </returns>
        public virtual CatalogValueSet GetValueSet()
        {
            if (this.Values != null)
            {
                return this.Values;
            }

            this.Values = this.CreateValueSet(-1);
            return this.Values;
        }

        /// <summary>
        /// Resets the value set.
        /// </summary>
        public virtual void ResetValueSet()
        {
            this.Values = null;
        }

        /// <summary>
        /// Sets the catalog value set with ownership.
        /// </summary>
        /// <param name="valueSet">
        /// The value set.
        /// </param>
        public virtual void SetCatalogValueSetWithOwnership(CatalogValueSet valueSet)
        {
            this.Values = valueSet;
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public virtual int Update()
        {
            int parameterCount;

            var tableName = this.GetDatabaseTableName();
            this.Database.EmptyTable(tableName);

            var memorybuffer = new StringBuilder();
            memorybuffer.Append("INSERT INTO ");
            memorybuffer.Append(tableName);
            memorybuffer.Append(" (code, text, sortinfo, access");
            if (this.IsVariable)
            {
                memorybuffer.Append(", tenant, extKey");
            }

            if (this.IsDependent)
            {
                memorybuffer.Append(", parentcode");
            }

            memorybuffer.Append(") VALUES (?,?,?,?");
            parameterCount = 4;
            if (this.IsVariable)
            {
                parameterCount += 2;
                memorybuffer.Append(",?,?");
            }

            if (this.IsDependent)
            {
                memorybuffer.Append(",?");
                parameterCount++;
            }

            memorybuffer.Append(")");

            var txtStatement = memorybuffer.ToString();

            try
            {
                this.Database.BeginTransaction();

                var statement = this.Database.CreateCommand(txtStatement);

                int i;
                for (i = 0; i < this.Values.Count; i++)
                {
                    statement.Reset();
                    string[] parameters;
                    this.Values.GetCatalogValueFromIndex(i).FillParameters(parameterCount, out parameters);
                    if (parameters == null)
                    {
                        continue;
                    }

                    foreach (var parameter in parameters)
                    {
                        statement.Bind(parameter);
                    }

                    statement.ExecuteNonQuery();
                }

                this.Database.Commit();
            }
            catch (Exception)
            {
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// Adds the with ownership.
        /// </summary>
        /// <param name="catalogValue">
        /// The catalog value.
        /// </param>
        /// <returns>
        /// The <see cref="CatalogValue"/>.
        /// </returns>
        protected virtual CatalogValue AddWithOwnership(CatalogValue catalogValue)
        {
            if (this.Values == null)
            {
                this.Values = new CatalogValueSet();
            }

            return this.Values.AddWithOwnership(catalogValue);
        }
    }

    /// <summary>
    /// Variable catalog info data access
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.DAL.CatalogInfo" />
    public class VariableCatalogInfo : CatalogInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VariableCatalogInfo"/> class.
        /// </summary>
        /// <param name="database">
        /// The database.
        /// </param>
        /// <param name="catalogNr">
        /// The catalog nr.
        /// </param>
        public VariableCatalogInfo(CRMDatabase database, int catalogNr)
            : base(database, catalogNr)
        {
        }

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="tenant">
        /// The tenant.
        /// </param>
        /// <param name="extKey">
        /// The ext key.
        /// </param>
        /// <param name="langinfo">
        /// The langinfo.
        /// </param>
        /// <param name="access">
        /// The access.
        /// </param>
        /// <returns>
        /// The <see cref="VariableCatalogValue"/>.
        /// </returns>
        public VariableCatalogValue AddValue(
            int code,
            string value,
            int tenant,
            string extKey,
            int langinfo,
            int access)
        {
            var val = new VariableCatalogValue(code, value, tenant, extKey, langinfo, access, true);
            this.AddWithOwnership(val);
            return val;
        }

        /// <summary>
        /// Gets the name of the database table.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string GetDatabaseTableName() => $"VARCAT_{this.CatalogNr}";

        /// <summary>
        /// Gets the external key.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string GetExternalKey(int code)
        {
            var value = this.Values?.GetCatalogValue(code) as VariableCatalogValue;
            return value?.ExtKey;
        }

        /// <summary>
        /// Gets the sort function.
        /// </summary>
        /// <param name="b1">
        /// The b1.
        /// </param>
        /// <param name="b2">
        /// The b2.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int GetSortFunction(CatalogValue b1, CatalogValue b2)
        {
            if (b1 == null && b2 != null)
            {
                return -1;
            }

            if (b1 != null && b2 == null)
            {
                return 1;
            }

            if (b1 == null)
            {
                return 0;
            }

            if (b1.SortInfo > 0)
            {
                if (b2.SortInfo <= 0)
                {
                    return -1;
                }

                var c = b1.SortInfo - b2.SortInfo;
                return c == 0 ? string.CompareOrdinal(b1.Text, b2.Text) : c;
            }

            return b2.SortInfo > 0 ? 1 : string.CompareOrdinal(b1.Text, b2.Text);
        }
    }

    /// <summary>
    /// Dependent catalog data access
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.DAL.VariableCatalogInfo" />
    public class DependentCatalogInfo : VariableCatalogInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependentCatalogInfo"/> class.
        /// </summary>
        /// <param name="database">
        /// The database.
        /// </param>
        /// <param name="catalogNr">
        /// The catalog nr.
        /// </param>
        /// <param name="parentCatalogNr">
        /// The parent catalog nr.
        /// </param>
        public DependentCatalogInfo(CRMDatabase database, int catalogNr, int parentCatalogNr)
            : base(database, catalogNr)
        {
            this.ParentCatalogNr = parentCatalogNr;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is dependent.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is dependent; otherwise, <c>false</c>.
        /// </value>
        public override bool IsDependent => true;

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <param name="parentCode">
        /// The parent code.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="tenant">
        /// The tenant.
        /// </param>
        /// <param name="extKey">
        /// The ext key.
        /// </param>
        /// <param name="langinfo">
        /// The langinfo.
        /// </param>
        /// <param name="access">
        /// The access.
        /// </param>
        /// <returns>
        /// The <see cref="DependentCatalogValue"/>.
        /// </returns>
        public DependentCatalogValue AddValue(
            int code,
            int parentCode,
            string value,
            int tenant,
            string extKey,
            int langinfo,
            int access)
        {
            var val = new DependentCatalogValue(code, parentCode, value, tenant, extKey, langinfo, access, true);
            this.AddWithOwnership(val);
            return val;
        }

        /// <summary>
        /// Creates the dependent catalog value set.
        /// </summary>
        /// <param name="parentValue">
        /// The parent value.
        /// </param>
        /// <returns>
        /// The <see cref="CatalogValueSet"/>.
        /// </returns>
        public CatalogValueSet CreateDependentCatalogValueSet(int parentValue)
        {
            return this.CreateValueSet(parentValue);
        }

        /// <summary>
        /// Gets the name of the database table.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string GetDatabaseTableName() => $"DEPCAT_{this.CatalogNr}";
    }

    /// <summary>
    /// Fixed catalog data access
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.DAL.CatalogInfo" />
    public class FixedCatalogInfo : CatalogInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FixedCatalogInfo"/> class.
        /// </summary>
        /// <param name="database">
        /// The database.
        /// </param>
        /// <param name="catalogNr">
        /// The catalog nr.
        /// </param>
        public FixedCatalogInfo(CRMDatabase database, int catalogNr)
            : base(database, catalogNr)
        {
        }

        /// <summary>
        /// Gets a value indicating whether this instance is fixed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is fixed; otherwise, <c>false</c>.
        /// </value>
        public override bool IsFixed => true;

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="langinfo">
        /// The langinfo.
        /// </param>
        /// <param name="access">
        /// The access.
        /// </param>
        /// <returns>
        /// The <see cref="FixedCatalogValue"/>.
        /// </returns>
        public FixedCatalogValue AddValue(int code, string value, int langinfo, int access)
        {
            var val = new FixedCatalogValue(code, value, langinfo, access, true);

            this.AddWithOwnership(val);
            return val;
        }

        /// <summary>
        /// Gets the name of the database table.
        /// </summary>
        /// <param name="tableName">
        /// Name of the table.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string GetDatabaseTableName() => $"FIXCAT_{this.CatalogNr}";

        /// <summary>
        /// Gets the sort function.
        /// </summary>
        /// <param name="c1">
        /// The c1.
        /// </param>
        /// <param name="c2">
        /// The c2.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int GetSortFunction(CatalogValue c1, CatalogValue c2)
        {
            return this.Database?.FixedCatSortBySortInfoAndCode ?? false
                       ? FixCatComparerSortInfoAndCode(c1, c2)
                       : FixCatComparer(c1, c2);
        }

        /// <summary>
        /// Fix catalog Comparer.
        /// </summary>
        /// <param name="b1">
        /// The b1.
        /// </param>
        /// <param name="b2">
        /// The b2.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int FixCatComparer(CatalogValue b1, CatalogValue b2)
        {
            return string.CompareOrdinal(b1?.Text, b2?.Text);
        }

        /// <summary>
        /// Fix catalog comparer considering sort information and code.
        /// </summary>
        /// <param name="b1">
        /// The b1.
        /// </param>
        /// <param name="b2">
        /// The b2.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int FixCatComparerSortInfoAndCode(CatalogValue b1, CatalogValue b2)
        {
            var sortInfo1 = b1.SortInfo;
            var sortInfo2 = b2.SortInfo;

            if (sortInfo1 > 0)
            {
                if (sortInfo2 == 0)
                {
                    return 1;
                }
                else if (sortInfo1 != sortInfo2)
                {
                    return sortInfo1 - sortInfo2;
                }
            }
            else if (sortInfo2 > 0)
            {
                return -1;
            }

            return b1.Code - b2.Code;
        }
    }

    /// <summary>
    /// Generic catalog value
    /// </summary>
    public class CatalogValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogValue"/> class.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="sortinfo">
        /// The sortinfo.
        /// </param>
        /// <param name="access">
        /// The access.
        /// </param>
        /// <param name="copy">
        /// if set to <c>true</c> [copy].
        /// </param>
        public CatalogValue(int code, string text, int sortinfo, int access, bool copy)
        {
            this.Code = code;

            if (copy)
            {
                this.Copy = true;
                this.Text = text;
            }
            else
            {
                this.Copy = false;
                this.Text = text;
            }

            this.Access = access;
            this.SortInfo = sortinfo;
        }

        /// <summary>
        /// Gets the access.
        /// </summary>
        /// <value>
        /// The access.
        /// </value>
        public int Access { get; }

        /// <summary>
        /// Gets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public int Code { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="CatalogValue"/> is copy.
        /// </summary>
        /// <value>
        /// <c>true</c> if copy; otherwise, <c>false</c>.
        /// </value>
        public bool Copy { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is dependent value.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is dependent value; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsDependentValue => false;

        /// <summary>
        /// Gets a value indicating whether this instance is fixed value.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is fixed value; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsFixedValue => false;

        /// <summary>
        /// Gets the sort information.
        /// </summary>
        /// <value>
        /// The sort information.
        /// </value>
        public int SortInfo { get; }

        /// <summary>
        /// Gets the tenant no.
        /// </summary>
        /// <value>
        /// The tenant no.
        /// </value>
        public virtual int TenantNo => 0;

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text { get; }

        /// <summary>
        /// Creates the copy.
        /// </summary>
        /// <returns>
        /// The <see cref="CatalogValue"/>.
        /// </returns>
        public virtual CatalogValue CreateCopy()
        {
            return null;
        }

        /// <summary>
        /// Fills the parameters.
        /// </summary>
        /// <param name="parameterCount">
        /// The parameter count.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        public virtual void FillParameters(int parameterCount, out string[] parameters)
        {
            parameters = new string[parameterCount];
            if (parameterCount < 2)
            {
                return;
            }

            parameters[0] = $"{this.Code}";
            parameters[1] = this.Text;

            if (parameterCount < 4)
            {
                return;
            }

            parameters[2] = $"{this.SortInfo}";
            parameters[3] = $"{this.Access}";
        }

        /// <summary>
        /// Gets the access for tenants.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="tenantNos">
        /// The tenant nos.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public virtual int GetAccessForTenants(int count, int[] tenantNos)
        {
            return 0;
        }

        /// <summary>
        /// Gets the tenant no.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public virtual int GetTenantNo()
        {
            return 0;
        }
    }

    /// <summary>
    /// Fixed catalog value
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.DAL.CatalogValue" />
    public class FixedCatalogValue : CatalogValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FixedCatalogValue"/> class.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="sortinfo">
        /// The sortinfo.
        /// </param>
        /// <param name="access">
        /// The access.
        /// </param>
        /// <param name="copy">
        /// if set to <c>true</c> [copy].
        /// </param>
        public FixedCatalogValue(int code, string text, int sortinfo, int access, bool copy)
            : base(code, text, sortinfo, access, copy)
        {
        }

        /// <summary>
        /// Gets a value indicating whether this instance is fixed value.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is fixed value; otherwise, <c>false</c>.
        /// </value>
        public override bool IsFixedValue => true;

        /// <summary>
        /// Creates the copy.
        /// </summary>
        /// <returns>
        /// The <see cref="FixedCatalogValue"/>.
        /// </returns>
        public override CatalogValue CreateCopy()
        {
            return new FixedCatalogValue(this.Code, this.Text, this.SortInfo, this.Access, true);
        }
    }

    /// <summary>
    /// Variable catalog value
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.DAL.CatalogValue" />
    public class VariableCatalogValue : CatalogValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VariableCatalogValue"/> class.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="tenantNo">
        /// The tenant no.
        /// </param>
        /// <param name="extKey">
        /// The ext key.
        /// </param>
        /// <param name="sortinfo">
        /// The sortinfo.
        /// </param>
        /// <param name="access">
        /// The access.
        /// </param>
        /// <param name="copy">
        /// if set to <c>true</c> [copy].
        /// </param>
        public VariableCatalogValue(
            int code,
            string text,
            int tenantNo,
            string extKey,
            int sortinfo,
            int access,
            bool copy)
            : base(code, text, sortinfo, access, copy)
        {
            this.ExtKey = copy ? $"{extKey}" : extKey;
            this.TenantNo = tenantNo;
        }

        /// <summary>
        /// Gets the ext key.
        /// </summary>
        /// <value>
        /// The ext key.
        /// </value>
        public string ExtKey { get; }

        /// <summary>
        /// Gets the tenant no.
        /// </summary>
        /// <value>
        /// The tenant no.
        /// </value>
        public override int TenantNo { get; }

        /// <summary>
        /// Creates the copy.
        /// </summary>
        /// <returns>
        /// The <see cref="VariableCatalogValue"/>.
        /// </returns>
        public override CatalogValue CreateCopy()
        {
            return new VariableCatalogValue(
                this.Code,
                this.Text,
                this.TenantNo,
                this.ExtKey,
                this.SortInfo,
                this.Access,
                true);
        }

        /// <summary>
        /// Fills the parameters.
        /// </summary>
        /// <param name="parameterCount">
        /// The parameter count.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        public override void FillParameters(int parameterCount, out string[] parameters)
        {
            base.FillParameters(parameterCount, out parameters);
            if (parameterCount > 4)
            {
                parameters[4] = $"{this.TenantNo}";
            }

            if (parameterCount > 5)
            {
                parameters[5] = this.ExtKey;
            }
        }

        /// <summary>
        /// Gets the access for tenants.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="tenantNos">
        /// The tenant nos.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int GetAccessForTenants(int count, int[] tenantNos)
        {
            var noAccess = 0;

            if (this.TenantNo == 0 || count == 0)
            {
                return noAccess;
            }

            noAccess = 1;

            for (var i = 0; noAccess > 0 && i < count; i++)
            {
                if (tenantNos[i] == this.TenantNo)
                {
                    noAccess = 0;
                }
            }

            return noAccess;
        }

        /// <summary>
        /// Gets the tenant no.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int GetTenantNo()
        {
            return this.TenantNo;
        }
    }

    /// <summary>
    /// Deependent catalog value
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.DAL.VariableCatalogValue" />
    public class DependentCatalogValue : VariableCatalogValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependentCatalogValue"/> class.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <param name="parentCode">
        /// The parent code.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="tenantNo">
        /// The tenant no.
        /// </param>
        /// <param name="extKey">
        /// The ext key.
        /// </param>
        /// <param name="sortinfo">
        /// The sortinfo.
        /// </param>
        /// <param name="access">
        /// The access.
        /// </param>
        /// <param name="copy">
        /// if set to <c>true</c> [copy].
        /// </param>
        public DependentCatalogValue(
            int code,
            int parentCode,
            string text,
            int tenantNo,
            string extKey,
            int sortinfo,
            int access,
            bool copy)
            : base(code, text, tenantNo, extKey, sortinfo, access, copy)
        {
            this.ParentCode = parentCode;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is dependent value.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is dependent value; otherwise, <c>false</c>.
        /// </value>
        public override bool IsDependentValue => true;

        /// <summary>
        /// Gets the parent code.
        /// </summary>
        /// <value>
        /// The parent code.
        /// </value>
        public int ParentCode { get; }

        /// <summary>
        /// Fills the parameters.
        /// </summary>
        /// <param name="parameterCount">
        /// The parameter count.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        public override void FillParameters(int parameterCount, out string[] parameters)
        {
            base.FillParameters(parameterCount, out parameters);

            if (parameterCount > 6)
            {
                parameters[6] = $"{this.ParentCode}";
            }
        }
    }

    /// <summary>
    /// Catalog value set
    /// </summary>
    public class CatalogValueSet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogValueSet"/> class.
        /// </summary>
        public CatalogValueSet()
        {
            this.Unsorted = false;
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count => this.Values?.Count ?? 0;

        /// <summary>
        /// Gets the sorted values.
        /// </summary>
        /// <value>
        /// The sorted values.
        /// </value>
        public List<CatalogValue> SortedValues { get; private set; }

        /// <summary>
        /// Gets or sets the sort function.
        /// </summary>
        /// <value>
        /// The sort function.
        /// </value>
        public Func<CatalogValue, CatalogValue, int> SortFunction { get; set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="CatalogValueSet"/> is unsorted.
        /// </summary>
        /// <value>
        /// <c>true</c> if unsorted; otherwise, <c>false</c>.
        /// </value>
        public bool Unsorted { get; private set; }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        public List<CatalogValue> Values { get; private set; }

        /// <summary>
        /// Adds the specified catalog value.
        /// </summary>
        /// <param name="catalogValue">
        /// The catalog value.
        /// </param>
        /// <returns>
        /// The <see cref="CatalogValue"/>.
        /// </returns>
        public CatalogValue Add(CatalogValue catalogValue)
        {
            return this.AddWithOwnership(catalogValue.CreateCopy());
        }

        /// <summary>
        /// Adds the dependent catalog value.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <param name="parentCode">
        /// The parent code.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="tenant">
        /// The tenant.
        /// </param>
        /// <param name="extKey">
        /// The ext key.
        /// </param>
        /// <param name="langinfo">
        /// The langinfo.
        /// </param>
        /// <param name="access">
        /// The access.
        /// </param>
        /// <returns>
        /// The <see cref="DependentCatalogValue"/>.
        /// </returns>
        public DependentCatalogValue AddDependentCatalogValue(
            int code,
            int parentCode,
            string value,
            int tenant,
            string extKey,
            int langinfo,
            int access)
        {
            var val = new DependentCatalogValue(code, parentCode, value, tenant, extKey, langinfo, access, true);

            this.AddWithOwnership(val);
            return val;
        }

        /// <summary>
        /// Adds the fixed catalog value.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="langinfo">
        /// The langinfo.
        /// </param>
        /// <param name="access">
        /// The access.
        /// </param>
        /// <returns>
        /// The <see cref="FixedCatalogValue"/>.
        /// </returns>
        public FixedCatalogValue AddFixedCatalogValue(int code, string value, int langinfo, int access)
        {
            var val = new FixedCatalogValue(code, value, langinfo, access, true);

            this.AddWithOwnership(val);
            return val;
        }

        /// <summary>
        /// Adds the variable catalog value.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="tenant">
        /// The tenant.
        /// </param>
        /// <param name="extKey">
        /// The ext key.
        /// </param>
        /// <param name="langinfo">
        /// The langinfo.
        /// </param>
        /// <param name="access">
        /// The access.
        /// </param>
        /// <returns>
        /// The <see cref="VariableCatalogValue"/>.
        /// </returns>
        public VariableCatalogValue AddVariableCatalogValue(
            int code,
            string value,
            int tenant,
            string extKey,
            int langinfo,
            int access)
        {
            var val = new VariableCatalogValue(code, value, tenant, extKey, langinfo, access, true);
            this.AddWithOwnership(val);
            return val;
        }

        /// <summary>
        /// Adds the with ownership.
        /// </summary>
        /// <param name="catalogValue">
        /// The catalog value.
        /// </param>
        /// <returns>
        /// The <see cref="CatalogValue"/>.
        /// </returns>
        public CatalogValue AddWithOwnership(CatalogValue catalogValue)
        {
            if (this.Values == null)
            {
                this.Values = new List<CatalogValue>();
            }

            this.Values.Add(catalogValue);
            this.Unsorted = true;
            return catalogValue;
        }

        /// <summary>
        /// Creates the copy.
        /// </summary>
        /// <returns>
        /// The <see cref="CatalogValueSet"/>.
        /// </returns>
        public CatalogValueSet CreateCopy()
        {
            var copy = new CatalogValueSet();
            if (this.Count <= 0)
            {
                return copy;
            }

            for (var i = 0; i < this.Count; i++)
            {
                copy.Add(this.Values[i].CreateCopy());
            }

            return copy;
        }

        /// <summary>
        /// Gets the catalog text.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetCatalogText(int code)
        {
            var value = this.GetCatalogValue(code);
            return value?.Text;
        }

        /// <summary>
        /// Gets the catalog value.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="CatalogValue"/>.
        /// </returns>
        public CatalogValue GetCatalogValue(int code)
        {
            return this.Values?.FirstOrDefault(v => v.Code == code);
        }

        /// <summary>
        /// Gets the index of the catalog value from.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="CatalogValue"/>.
        /// </returns>
        public CatalogValue GetCatalogValueFromIndex(int index)
        {
            return this.Values != null && index < this.Count ? this.Values[index] : null;
        }

        /// <summary>
        /// Gets the index of the sorted catalog value from.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="CatalogValue"/>.
        /// </returns>
        public CatalogValue GetSortedCatalogValueFromIndex(int index)
        {
            return index < this.Count
                       ? (this.SortedValues != null ? this.SortedValues[index] : this.Values[index])
                       : null;
        }

        /// <summary>
        /// Sorts this instance.
        /// </summary>
        public void Sort()
        {
            this.Values?.Sort((a, b) => a.Code - b.Code);

            if (this.SortFunction != null)
            {
                if (this.SortedValues != null)
                {
                    this.SortedValues = null;
                }

                if (this.Values != null)
                {
                    this.SortedValues = this.Values.ToList();
                    this.SortedValues.Sort((a, b) => this.SortFunction(a, b));
                }
            }

            this.Unsorted = false;
        }
    }
}
