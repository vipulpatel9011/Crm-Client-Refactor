// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPResultRowProvider.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The PResultRowProviderDelegate interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel
{
    using System.Collections.Generic;

    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// The PResultRowProviderDelegate interface.
    /// </summary>
    public interface IResultRowProviderDelegate
    {
        /// <summary>
        /// The result row provider did create row from data row.
        /// </summary>
        /// <param name="resultRowProvider">
        /// The result row provider.
        /// </param>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <param name="dataRow">
        /// The data row.
        /// </param>
        void ResultRowProviderDidCreateRowFromDataRow(
            UPResultRowProvider resultRowProvider,
            UPMResultRow row,
            UPCRMResultRow dataRow);
    }

    /// <summary>
    /// The up result row provider.
    /// </summary>
    public class UPResultRowProvider : UPMResultRowProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPResultRowProvider"/> class.
        /// </summary>
        /// <param name="theDelegate">
        /// The the delegate.
        /// </param>
        public UPResultRowProvider(IResultRowProviderDelegate theDelegate)
        {
            this.TheDelegate = theDelegate;
        }

        /// <summary>
        /// Gets the the delegate.
        /// </summary>
        public IResultRowProviderDelegate TheDelegate { get; private set; }

        /// <summary>
        /// The number of result rows.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public virtual int NumberOfResultRows()
        {
            return 0;
        }

        /// <summary>
        /// The row at index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="UPMResultRow"/>.
        /// </returns>
        public virtual UPMResultRow RowAtIndex(int index)
        {
            return null;
        }

        /// <summary>
        /// The row from result row.
        /// </summary>
        /// <param name="crmRow">
        /// The crm row.
        /// </param>
        /// <returns>
        /// The <see cref="UPMResultRow"/>.
        /// </returns>
        public virtual UPMResultRow RowFromResultRow(UPCRMResultRow crmRow)
        {
            RecordIdentifier identifier = new RecordIdentifier(crmRow.RecordIdentificationAtIndex(0));
            UPMResultRow resultRow = new UPMResultRow(identifier) { Invalid = true, DataValid = true };

            this.TheDelegate?.ResultRowProviderDidCreateRowFromDataRow(this, resultRow, crmRow);
            return resultRow;
        }
    }

    /// <summary>
    /// The up result row provider for crm result.
    /// </summary>
    public class UPResultRowProviderForCRMResult : UPResultRowProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPResultRowProviderForCRMResult"/> class.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <param name="theDelegate">
        /// The the delegate.
        /// </param>
        public UPResultRowProviderForCRMResult(UPCRMResult result, IResultRowProviderDelegate theDelegate)
            : base(theDelegate)
        {
            this.Result = result;
        }

        /// <summary>
        /// Gets the result.
        /// </summary>
        protected UPCRMResult Result { get; }

        /// <summary>
        /// The number of result rows.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int NumberOfResultRows()
        {
            return this.Result.RowCount;
        }

        /// <summary>
        /// The row at index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="UPMResultRow"/>.
        /// </returns>
        public override UPMResultRow RowAtIndex(int index)
        {
            return this.RowFromResultRow((UPCRMResultRow)this.Result.ResultRowAtIndex(index));
        }
    }

    /// <summary>
    /// The up result row from crm result rows.
    /// </summary>
    public class UPResultRowFromCRMResultRows : UPResultRowProvider
    {
        /// <summary>
        /// The crm result rows.
        /// </summary>
        protected List<UPCRMResultRow> crmResultRows;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPResultRowFromCRMResultRows"/> class.
        /// </summary>
        /// <param name="theDelegate">
        /// The the delegate.
        /// </param>
        public UPResultRowFromCRMResultRows(IResultRowProviderDelegate theDelegate)
            : base(theDelegate)
        {
            this.crmResultRows = new List<UPCRMResultRow>();
        }

        /// <summary>
        /// The add row.
        /// </summary>
        /// <param name="crmResultRow">
        /// The crm result row.
        /// </param>
        public void AddRow(UPCRMResultRow crmResultRow)
        {
            this.crmResultRows.Add(crmResultRow);
        }

        /// <summary>
        /// The number of result rows.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int NumberOfResultRows()
        {
            return this.crmResultRows.Count;
        }

        /// <summary>
        /// The row at index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="UPMResultRow"/>.
        /// </returns>
        public override UPMResultRow RowAtIndex(int index)
        {
            return this.RowFromResultRow(this.crmResultRows[index]);
        }
    }
}
