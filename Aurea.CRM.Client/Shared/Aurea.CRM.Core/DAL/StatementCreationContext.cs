// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatementCreationContext.cs" company="Aurea Software Gmbh">
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
//   Context information related to statement creation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.DAL
{
    using System.Collections.Generic;

    /// <summary>
    /// Context information related to statement creation
    /// </summary>
    public class StatementCreationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatementCreationContext"/> class.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        public StatementCreationContext(Query query)
        {
            this.Query = query;
        }

        /// <summary>
        /// Gets the collation.
        /// </summary>
        /// <value>
        /// The collation.
        /// </value>
        public string Collation => this.Query.CollationName;

        /// <summary>
        /// Gets or sets the error text.
        /// </summary>
        /// <value>
        /// The error text.
        /// </value>
        public string ErrorText { get; set; }

        /// <summary>
        /// Gets the parameter values.
        /// </summary>
        /// <value>
        /// The parameter values.
        /// </value>
        public List<string> ParameterValues { get; private set; }

        /// <summary>
        /// Gets the query.
        /// </summary>
        /// <value>
        /// The query.
        /// </value>
        public Query Query { get; }

        /// <summary>
        /// Gets or sets the record template.
        /// </summary>
        /// <value>
        /// The record template.
        /// </value>
        public RecordTemplate RecordTemplate { get; set; }

        /// <summary>
        /// Adds the parameter value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void AddParameterValue(string value)
        {
            if (this.ParameterValues == null)
            {
                this.ParameterValues = new List<string>();
            }

            this.ParameterValues.Add(value);
        }
    }
}
