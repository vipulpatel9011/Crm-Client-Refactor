// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICrmDataSource.cs" company="Aurea Software Gmbh">
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
//   Request option types
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.CRM
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// CRM data source interface
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.ICrmDataSourceMetaInfo" />
    public interface ICrmDataSource : ICrmDataSourceMetaInfo
    {
        /// <summary>
        /// Gets a value indicating whether is server result.
        /// </summary>
        bool IsServerResult { get; }

        /// <summary>
        /// Gets rows count.
        /// </summary>
        int RowCount { get; }

        /// <summary>
        /// Results the index of the row at.
        /// </summary>
        /// <param name="rowIndex">
        /// Index of the row.
        /// </param>
        /// <returns>
        /// The <see cref="ICrmDataSourceRow"/>.
        /// </returns>
        ICrmDataSourceRow ResultRowAtIndex(int rowIndex);
    }

    /// <summary>
    /// Request option types
    /// </summary>
    public enum UPRequestOption
    {
        /// <summary>
        /// The offline.
        /// </summary>
        Offline = 0, // read offline

        /// <summary>
        /// The online.
        /// </summary>
        Online, // read online

        /// <summary>
        /// The best available.
        /// </summary>
        BestAvailable, // read online first, then offline

        /// <summary>
        /// The fastest available.
        /// </summary>
        FastestAvailable, // read offline first, then online

        /// <summary>
        /// The default.
        /// </summary>
        Default // use the preferred way
    }

    /// <summary>
    /// CRM data source field interface
    /// </summary>
    public interface ICrmDataSourceField
    {
        /// <summary>
        /// Gets the field identifier.
        /// </summary>
        /// <value>
        /// The field identifier.
        /// </value>
        int FieldId { get; }

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        string InfoAreaId { get; }

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        string Label { get; }

        /// <summary>
        /// Gets the position in result.
        /// </summary>
        /// <value>
        /// The position in result.
        /// </value>
        int PositionInResult { get; }

        /// <summary>
        /// Subs the field indices.
        /// </summary>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        List<object> SubFieldIndices();

        /// <summary>
        /// Values from raw value.
        /// </summary>
        /// <param name="rawValue">
        /// The raw value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string ValueFromRawValue(string rawValue);
    }

    /// <summary>
    /// CRM data source table
    /// </summary>
    public interface ICrmDataSourceTable
    {
        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        string InfoAreaId { get; }

        /// <summary>
        /// Gets the number of sub tables.
        /// </summary>
        /// <value>
        /// The number of sub tables.
        /// </value>
        int NumberOfSubTables { get; }

        /// <summary>
        /// Fields at index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="ICrmDataSourceField"/>.
        /// </returns>
        ICrmDataSourceField FieldAtIndex(int index);

        /// <summary>
        /// Informations the area position in result.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        int InfoAreaPositionInResult();

        /// <summary>
        /// Numbers the of fields.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        int NumberOfFields();

        /// <summary>
        /// Subs the index of the table at.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="ICrmDataSourceTable"/>.
        /// </returns>
        ICrmDataSourceTable SubTableAtIndex(int index);
    }

    /// <summary>
    /// CRM data source row interface
    /// </summary>
    public interface ICrmDataSourceRow
    {
        /// <summary>
        /// Gets the root record identifier.
        /// </summary>
        /// <value>
        /// The root record identifier.
        /// </value>
        string RootRecordId { get; }

        /// <summary>
        /// Gets the root record identification.
        /// </summary>
        string RootRecordIdentification { get; }

        /// <summary>
        /// Raws the index of the value at.
        /// </summary>
        /// <param name="positionInResult">
        /// The position in result.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string RawValueAtIndex(int positionInResult);

        /// <summary>
        /// Records the index of the identification at.
        /// </summary>
        /// <param name="infoAreaPositionInResult">
        /// The information area position in result.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string RecordIdentificationAtIndex(int infoAreaPositionInResult);

        /// <summary>
        /// Values at index.
        /// </summary>
        /// <param name="positionInResult">
        /// The position in result.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string ValueAtIndex(int positionInResult);
    }

    /// <summary>
    /// CRM data source meta info interface
    /// </summary>
    public interface ICrmDataSourceMetaInfo
    {
        /// <summary>
        /// Gets the number of result tables.
        /// </summary>
        /// <value>
        /// The number of result tables.
        /// </value>
        int NumberOfResultTables { get; }

        /// <summary>
        /// Fields at index.
        /// </summary>
        /// <param name="fieldIndex">
        /// Index of the field.
        /// </param>
        /// <returns>
        /// The <see cref="ICrmDataSourceField"/>.
        /// </returns>
        ICrmDataSourceField FieldAtIndex(int fieldIndex);

        /// <summary>
        /// Results the index of the table at.
        /// </summary>
        /// <param name="tableIndex">
        /// Index of the table.
        /// </param>
        /// <returns>
        /// The <see cref="ICrmDataSourceTable"/>.
        /// </returns>
        ICrmDataSourceTable ResultTableAtIndex(int tableIndex);
    }
}
