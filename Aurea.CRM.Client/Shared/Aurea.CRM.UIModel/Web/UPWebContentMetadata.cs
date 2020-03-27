// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPWebContentMetadata.cs" company="Aurea Software Gmbh">
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
//   The Dashboard Page Model Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Web
{
    using Aurea.CRM.Core.Configuration;

    /// <summary>
    /// UPWebContentMetadata
    /// </summary>
    public class UPWebContentMetadata
    {
        protected bool allowsXMLExport;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPWebContentMetadata"/> class.
        /// </summary>
        /// <param name="theDelegate">The delegate.</param>
        public UPWebContentMetadata(UPWebContentMetadataDelegate theDelegate)
        {
            this.TheDelegate = theDelegate;
        }

        /// <summary>
        /// Gets the type of the report.
        /// </summary>
        /// <value>
        /// The type of the report.
        /// </value>
        public virtual string ReportType { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [allows XML export].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allows XML export]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool AllowsXMLExport => false;

        /// <summary>
        /// Gets a value indicating whether [allows full screen].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allows full screen]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool AllowsFullScreen => false;

        /// <summary>
        /// Gets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public UPWebContentMetadataDelegate TheDelegate { get; private set; }

        /// <summary>
        /// Updates the metadata with view reference.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public virtual void UpdateMetadataWithViewReference(ViewReference viewReference)
        {
        }

        /// <summary>
        /// Webs the type of the content meta data from report.
        /// </summary>
        /// <param name="reportType">Type of the report.</param>
        /// <param name="theDelegate">The delegate.</param>
        /// <returns></returns>
        public static UPWebContentMetadata WebContentMetaDataFromReportType(string reportType, UPWebContentMetadataDelegate theDelegate)
        {
            switch (reportType)
            {
                case "CoreReport":
                    return new UPWebContentMetadataServerReport(theDelegate);

                case "ClientReport":
                    return new UPWebContentMetadataClientReport(theDelegate);

                case "Url":
                case "Html":
                    return new UPWebContentMetadataUrl(theDelegate);

                case "QlikView":
                    return new UPWebContentQlikViewUrl(theDelegate);

                case "Portfolio":
                    return new UPWebContentPortfolio(theDelegate);

                default:
                    return new UPWebContentMetadataStaticHtml(theDelegate);
            }
        }

        /// <summary>
        /// Webs the content meta data from view reference.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="theDelegate">The delegate.</param>
        /// <returns></returns>
        public static UPWebContentMetadata WebContentMetaDataFromViewReference(ViewReference viewReference, UPWebContentMetadataDelegate theDelegate)
        {
            UPWebContentMetadata contentMetaData = WebContentMetaDataFromReportType(viewReference.ContextValueForKey("ReportType"), theDelegate);
            contentMetaData.UpdateMetadataWithViewReference(viewReference);
            return contentMetaData;
        }
    }
}
