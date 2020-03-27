// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubQuery.cs" company="Aurea Software Gmbh">
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
//   Implements database sub query
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.DAL
{
    /// <summary>
    /// Implements database sub query
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.DAL.Query" />
    public class SubQuery : Query
    {
        /// <summary>
        /// The parent.
        /// </summary>
        private readonly QueryTreeItem parent;

        /// <summary>
        /// The parent replace index.
        /// </summary>
        private int parentReplaceIndex;

        /// <summary>
        /// The record id pos.
        /// </summary>
        private int recordIdPos;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubQuery"/> class.
        /// </summary>
        /// <param name="root">
        /// The root.
        /// </param>
        /// <param name="parent">
        /// The parent.
        /// </param>
        public SubQuery(QueryTreeItem root, QueryTreeItem parent)
            : base(root, true)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Executes the specified parent result.
        /// </summary>
        /// <param name="parentResult">
        /// The parent result.
        /// </param>
        /// <returns>
        /// The <see cref="GenericRecordSet"/>.
        /// </returns>
        public GenericRecordSet Execute(GenericRecordSet parentResult)
        {
            int i, count;

            count = parentResult.GetRowCount();

            this.SetLinkRecord(this.parent.InfoAreaId, "#parameterposition#", this.RootTreeItem.LinkId);

            var context = new StatementCreationContext(this);
            if (!string.IsNullOrEmpty(context.ErrorText))
            {
                // _crmDatabase.Trace(_crmDatabase, context.GetErrorText());
                return null;
            }

            var statementString = this.CreateStatement(context, false);
            var parameters = context.ParameterValues;
            var parameterCount = parameters.Count;

            var parameterPos = 0;
            for (i = 0; i < parameterCount; i++)
            {
                if (parameters[i] != "#parameterposition#")
                {
                    continue;
                }

                parameterPos = i;
                break;
            }

            var query = new DatabaseQuery(this.crmDatabase);
            var ret = query.Prepare(statementString);
            if (!ret)
            {
                return null;
            }

            i = 0;
            while (i < count)
            {
                var currentRow = parentResult.GetRow(i);
                var currentRecordId = currentRow.GetColumn(this.recordIdPos);
                if (string.IsNullOrEmpty(currentRecordId))
                {
                }
                else
                {
                    query.Reset();
                    parameters[parameterPos] = currentRecordId;
                    ret = query.Execute(parameters) == 0;
                    if (ret)
                    {
                        return null;
                    }

                    var frs = new DatabaseRecordSet(query);

                    // TODO: Complete the implementation
                }

                i++;
            }

            return null;
        }

        /// <summary>
        /// Sets the sub query information.
        /// </summary>
        /// <param name="recordIdPosition">
        /// The _record identifier position.
        /// </param>
        /// <param name="replaceIndex">
        /// Index of the _replace.
        /// </param>
        public void SetSubQueryInformation(int recordIdPosition, int replaceIndex)
        {
            this.recordIdPos = recordIdPosition;
            this.parentReplaceIndex = replaceIndex;
        }
    }
}
