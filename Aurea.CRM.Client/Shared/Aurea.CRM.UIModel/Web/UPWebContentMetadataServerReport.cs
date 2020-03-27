// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPWebContentMetadataServerReport.cs" company="Aurea Software Gmbh">
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
//   The UPWebContentMetadataServerReport class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Web
{
    using System;
    using Aurea.CRM.Core.Configuration;

    /// <summary>
    /// UPWebContentMetadataServerReport
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.Web.UPWebContentMetadataReport" />
    public class UPWebContentMetadataServerReport : UPWebContentMetadataReport
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPWebContentMetadataServerReport" /> class.
        /// </summary>
        /// <param name="theDelegate">The delegate.</param>
        public UPWebContentMetadataServerReport(UPWebContentMetadataDelegate theDelegate)
            : base(theDelegate)
        {
        }

        /// <summary>
        /// Gets the name of the report.
        /// </summary>
        /// <value>
        /// The name of the report.
        /// </value>
        public string ReportName { get; private set; }

        /// <summary>
        /// Gets the report private.
        /// </summary>
        /// <value>
        /// The report private.
        /// </value>
        public string ReportPrivate { get; private set; }

        /// <summary>
        /// Updates the metadata with view reference.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public override void UpdateMetadataWithViewReference(ViewReference viewReference)
        {
            base.UpdateMetadataWithViewReference(viewReference);
            this.ReportName = viewReference.ContextValueForKey("Report");
            this.ReportPrivate = viewReference.ContextValueForKey("ReportPrivate");
        }

        /// <summary>
        /// Gets the type of the report.
        /// </summary>
        /// <value>
        /// The type of the report.
        /// </value>
        public override string ReportType => "CoreReport";

        /// <summary>
        /// Calculates the service URL with record identification.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <returns></returns>
        public Uri CalcServiceURLWithRecordIdentification(string recordIdentification)
        {
            // TODO: Need to implement CalcServiceURLWithRecordIdentification
            return null;

#if PORTING
            //Uri webServiceUrl = ServerSession.CurrentSession.CrmServer.MobileWebserviceUrl; //.WebserviceURL;
            //string urlParameterString = NSString.StringWithFormat("%@?Service=Report&ReportType=%@&ReportName=%@", webServiceUrl.AbsoluteString(), this.ReportType, this.ReportName.StringByAddingPercentEscapesUsingEncoding(NSUTF8StringEncoding));
            //if (!string.IsNullOrEmpty(this.ReportPrivate))
            //{
            //    urlParameterString = urlParameterString.StringByAppendingFormat("&ReportPrivate=%@", this.ReportPrivate);
            //}

            //if (!string.IsNullOrEmpty(recordIdentification))
            //{
            //    urlParameterString = urlParameterString.StringByAppendingFormat("&RecordIdentification=%@", recordIdentification);
            //}

            //if (this.ReportParameters.Count > 0)
            //{
            //    urlParameterString = urlParameterString.StringByAppendingFormat("&Parameters=%@", NSString.StringFromObject(this.ReportParametersAsStringArray()).StringByAddingPercentEscapesUsingEncoding(NSUTF8StringEncoding));
            //}

            //return urlParameterString;
#endif
        }
    }
}
