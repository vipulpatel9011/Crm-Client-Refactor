// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPWebContentMetadataReport.cs" company="Aurea Software Gmbh">
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
//   UPWebContentMetadataReport
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Web
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// UPWebContentMetadataReport
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.Web.UPWebContentMetadata" />
    public class UPWebContentMetadataReport : UPWebContentMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPWebContentMetadataReport"/> class.
        /// </summary>
        /// <param name="theDelegate">The delegate.</param>
        public UPWebContentMetadataReport(UPWebContentMetadataDelegate theDelegate)
            : base(theDelegate)
        {
        }

        /// <summary>
        /// Gets the report parameters.
        /// </summary>
        /// <value>
        /// The report parameters.
        /// </value>
        public Dictionary<string, object> ReportParameters { get; private set; }

        /// <summary>
        /// Reports the parameters as string array.
        /// </summary>
        /// <returns></returns>
        public List<object> ReportParametersAsStringArray()
        {
            return null;
            //ArrayList parameterArray = NSMutableArray.TheNew();
            //foreach (var entry in this.ReportParameters)
            //{
            //    parameterArray.Add(new ArrayList { entry.Key, entry.Value });
            //}

            //return parameterArray;
        }

        /// <summary>
        /// Updates the metadata with view reference.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public override void UpdateMetadataWithViewReference(ViewReference viewReference)
        {
            base.UpdateMetadataWithViewReference(viewReference);
            string reportParameterString = viewReference.ContextValueForKey("Parameters");
            if (!string.IsNullOrEmpty(reportParameterString))
            {
                this.ReportParameters = reportParameterString.JsonDictionaryFromString();
            }
        }
    }
}
