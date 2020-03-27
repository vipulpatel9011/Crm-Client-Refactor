// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigTimelineCriteria.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Max Menezes
// </author>
// <summary>
//   Config Timeline Criteria
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.DataModel;

    /// <summary>
    /// Time line criteria configurations
    /// </summary>
    public class ConfigTimelineCriteria
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigTimelineCriteria"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        public ConfigTimelineCriteria(List<object> definition)
        {
            this.FieldId = Convert.ToInt32(definition[0]);
            this.CompareOperator = UPCRMField.ConditionOperatorFromString(definition[1] as string);
            this.CompareValue = definition[2] as string;
            this.CompareValueTo = definition[3] as string;
            this.Setting1 = definition[4] as string;
            this.Setting2 = definition[5] as string;
        }

        /// <summary>
        /// Gets the compare operator.
        /// </summary>
        /// <value>
        /// The compare operator.
        /// </value>
        public UPConditionOperator CompareOperator { get; private set; }

        /// <summary>
        /// Gets the compare value.
        /// </summary>
        /// <value>
        /// The compare value.
        /// </value>
        public string CompareValue { get; private set; }

        /// <summary>
        /// Gets the compare value to.
        /// </summary>
        /// <value>
        /// The compare value to.
        /// </value>
        public string CompareValueTo { get; private set; }

        /// <summary>
        /// Gets the field identifier.
        /// </summary>
        /// <value>
        /// The field identifier.
        /// </value>
        public int FieldId { get; private set; }

        /// <summary>
        /// Gets the setting1.
        /// </summary>
        /// <value>
        /// The setting1.
        /// </value>
        public string Setting1 { get; private set; }

        /// <summary>
        /// Gets the setting2.
        /// </summary>
        /// <value>
        /// The setting2.
        /// </value>
        public string Setting2 { get; private set; }
    }
}
