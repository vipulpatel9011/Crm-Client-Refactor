// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPWebContentMetadataClientReport.cs" company="Aurea Software Gmbh">
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
//   UPWebContentMetadataClientReport
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.Core.Platform;
    using Aurea.CRM.Core.Session;
    using Aurea.CRM.Core.Utilities;

    /// <summary>
    /// UPWebContentMetadataClientReport
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.Web.UPWebContentMetadataReport" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.UPCRMLinkReaderDelegate" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.ISearchOperationHandler" />
    public class UPWebContentMetadataClientReport : UPWebContentMetadataReport, UPCRMLinkReaderDelegate, ISearchOperationHandler
    {
        private List<UPCRMResult> resultsForClientReports;
        private int nextClientReport;
        private Dictionary<string, object> filterParameters;
        private string recordIdentification;
        private int linkId;
        private Dictionary<string, string> parentLinkDictionary;
        private UPContainerMetaInfo crmQuery;
        private UPCRMLinkReader linkReader;
        private bool blockStart;
        private List<string> signatureImageTagNames;
        private List<string> signaturePathNames;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPWebContentMetadataClientReport"/> class.
        /// </summary>
        /// <param name="theDelegate">The delegate.</param>
        public UPWebContentMetadataClientReport(UPWebContentMetadataDelegate theDelegate)
            : base(theDelegate)
        {
        }

        /// <summary>
        /// Gets the signature count.
        /// </summary>
        /// <value>
        /// The signature count.
        /// </value>
        public int SignatureCount => this.signatureImageTagNames.Count;

        /// <summary>
        /// Gets the signing configuration.
        /// </summary>
        /// <value>
        /// The signing configuration.
        /// </value>
        public Dictionary<string, object> SigningConfig { get; private set; }

        /// <summary>
        /// Gets the signature image identifier.
        /// </summary>
        /// <value>
        /// The signature image identifier.
        /// </value>
        public string SignatureImageId { get; private set; }

        /// <summary>
        /// Gets the signature title.
        /// </summary>
        /// <value>
        /// The signature title.
        /// </value>
        public string SignatureTitle { get; private set; }

        /// <summary>
        /// Gets the name of the PDF file.
        /// </summary>
        /// <value>
        /// The name of the PDF file.
        /// </value>
        public string PdfFileName { get; private set; }

        /// <summary>
        /// Gets the PDF file name date format.
        /// </summary>
        /// <value>
        /// The PDF file name date format.
        /// </value>
        public string PdfFileNameDateFormat { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [with signature].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [with signature]; otherwise, <c>false</c>.
        /// </value>
        public bool WithSignature { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [upload report].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [upload report]; otherwise, <c>false</c>.
        /// </value>
        public bool UploadReport { get; private set; }

        /// <summary>
        /// Gets the client reports.
        /// </summary>
        /// <value>
        /// The client reports.
        /// </value>
        public List<UPWebContentClientReport> ClientReports { get; private set; }

        /// <summary>
        /// Gets the name of the root XML.
        /// </summary>
        /// <value>
        /// The name of the root XML.
        /// </value>
        public string RootXmlName { get; private set; }

        /// <summary>
        /// Gets the name of the XSL.
        /// </summary>
        /// <value>
        /// The name of the XSL.
        /// </value>
        public string XslName { get; private set; }

        /// <summary>
        /// Gets the request option.
        /// </summary>
        /// <value>
        /// The request option.
        /// </value>
        public UPRequestOption RequestOption { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [request option dependent on root].
        /// </summary>
        /// <value>
        /// <c>true</c> if [request option dependent on root]; otherwise, <c>false</c>.
        /// </value>
        public bool RequestOptionDependentOnRoot { get; private set; }

        /// <summary>
        /// Gets the type of the report.
        /// </summary>
        /// <value>
        /// The type of the report.
        /// </value>
        public override string ReportType => "ClientReport";

        /// <summary>
        /// Gets a value indicating whether [allows XML export].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allows XML export]; otherwise, <c>false</c>.
        /// </value>
        public override bool AllowsXMLExport => ServerSession.CurrentSession.ValueIsSet("Action.Allow.ExportXML");

        /// <summary>
        /// Updates the metadata with view reference.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public override void UpdateMetadataWithViewReference(ViewReference viewReference)
        {
            base.UpdateMetadataWithViewReference(viewReference);
            string jsonString = viewReference.ContextValueForKey("SigningConfig");
            if (!string.IsNullOrEmpty(jsonString))
            {
                Dictionary<string, object> defaultConfigDict = "{\"sign\": true, \"upload\": true, \"emptySignatureName\": \"Button:PleaseSign\", \"signatureImageTagName\": \"SignatureImage\", \"signatureImageId\": \"img-signature\", \"signedReportFileName\": \"SampleReport.pdf\", \"signedReportFileNameDateFormat\": \"yyyy-MM-dd\"}".JsonDictionaryFromString();
                Dictionary<string, object> tempDict = new Dictionary<string, object>(defaultConfigDict);

                foreach (var entry in jsonString.JsonDictionaryFromString())
                {
                    tempDict[entry.Key] = entry.Value;
                }

                this.SigningConfig = new Dictionary<string, object>(tempDict);
            }
            else
            {
                this.SigningConfig = null;
            }

            if (this.SigningConfig != null)
            {
                this.PdfFileName = this.SigningConfig.ValueOrDefault("signedReportFileName") as string;
                this.PdfFileNameDateFormat = this.SigningConfig.ValueOrDefault("signedReportFileNameDateFormat") as string;
                this.SignatureImageId = this.SigningConfig.ValueOrDefault("signatureImageId") as string;
                this.SignatureTitle = this.SigningConfig.ValueOrDefault("signatureTitle") as string;
                this.signatureImageTagNames = ((string)this.SigningConfig["signatureImageTagName"]).Split(';').ToList();
                //this.WithSignature = this.SigningConfig.BoolValueForKeyTheDefault("sign", true);
                //this.UploadReport = this.SigningConfig.BoolValueForKeyTheDefault("upload", true);

                this.signaturePathNames = new List<string>();
                IFileStorage fileStore = ServerSession.CurrentSession.FileStore;
                var emptySignatureNames = ((string)this.SigningConfig["emptySignatureName"]).Split(';');
                for (int i = 0; i < emptySignatureNames.Length; i++)
                {
                    string emptySignatureName = emptySignatureNames[i];
                    string languageEmptySignatureName = $"{emptySignatureName}_{ServerSession.CurrentSession.LanguageKey}";
                    //string languageEmptySignaturePathName = fileStore.ImagePathForName(languageEmptySignatureName);
                    //string emptySignaturePathName = languageEmptySignaturePathName ?? fileStore.ImagePathForName(emptySignatureName);
                    //string signatureFileName = NSString.StringWithFormat("signature-%lu.%@", i, emptySignaturePathName.PathExtension());
                    //string signaturePathName = fileStore.BaseDirectoryPath().StringByAppendingPathComponent(signatureFileName);
                    //if (!string.IsNullOrEmpty(emptySignaturePathName))
                    //{
                    //    NSFileManager.DefaultManager().RemoveItemAtPathError(signaturePathName, null);
                    //    NSFileManager.DefaultManager().CopyItemAtPathToPathError(emptySignaturePathName, signaturePathName, null);
                    //}

                    //signaturePathNames.AddObject(signaturePathName);
                }
            }

            string configParentLink = viewReference.ContextValueForKey("ConfigParentLink");
            UPWebContentClientReport clientReport = new UPWebContentClientReport(viewReference.ContextValueForKey("ConfigName"), viewReference.ContextValueForKey("RootName"), configParentLink);
            string additionalConfigNames = viewReference.ContextValueForKey("AdditionalConfigNames");
            string additionalRootNames = viewReference.ContextValueForKey("AdditionalRootNames");
            if (!string.IsNullOrEmpty(additionalConfigNames))
            {
                string additionalConfigParentLinkString = viewReference.ContextValueForKey("AdditionalConfigParentLinks");
                string[] additionalConfigParentLinks = null;
                var configNameParts = additionalConfigNames.Split(';');
                if (!string.IsNullOrEmpty(additionalConfigParentLinkString))
                {
                    additionalConfigParentLinks = additionalConfigParentLinkString.Split(';');
                }

                string[] rootNameParts = null;
                if (!string.IsNullOrEmpty(additionalRootNames))
                {
                    rootNameParts = additionalRootNames.Split(';');
                }

                int count = configNameParts.Length;
                List<UPWebContentClientReport> clientReportArray = new List<UPWebContentClientReport>(count + 1);
                clientReportArray.Add(clientReport);
                for (int i = 0; i < count; i++)
                {
                    string parentLinkConfig = null;
                    if (additionalConfigParentLinks?.Length > i)
                    {
                        parentLinkConfig = additionalConfigParentLinks[i];
                    }

                    clientReport = new UPWebContentClientReport(configNameParts[i], rootNameParts?.Length > i ? rootNameParts[i] : null, parentLinkConfig);
                    clientReportArray.Add(clientReport);
                }

                this.ClientReports = clientReportArray;
            }
            else
            {
                this.ClientReports = new List<UPWebContentClientReport> { clientReport };
            }

            this.RootXmlName = viewReference.ContextValueForKey("XmlRootElementName");
            this.XslName = viewReference.ContextValueForKey("Xsl");
            this.RequestOption = UPCRMDataStore.RequestOptionFromString(viewReference.ContextValueForKey("RequestOption"), UPRequestOption.FastestAvailable);
            this.RequestOptionDependentOnRoot = viewReference.ContextValueIsSet("RequestOptionDependentOnRoot");
            if (this.RequestOptionDependentOnRoot && this.TheDelegate.IsOnline)
            {
                this.RequestOption = UPRequestOption.Online;
            }
        }

        /// <summary>
        /// Clears the signature.
        /// </summary>
        public void ClearSignature()
        {
#if PORTING
            UIImage image = this.CreateImage(CGSizeMake(300, 200));     // CRM-5007
            ArrayList images = new ArrayList();
            for (uint i = 0; i < signaturePathNames.Count; i++)
            {
                images.Add(image);
            }

            this.WriteImagesToSignaturePathName(images);
#endif
        }

#if PORTING       // CRM-5007
        void WriteImagesToSignaturePathName(ArrayList images)
        {
            for (uint i = 0; i < images.Count && i < signaturePathNames.Count; i++)
            {
                this.WriteImageToPath(images[i], signaturePathNames[i]);
            }
        }

        void WriteImageToPath(UIImage image, string path)
        {
            NSFileManager.DefaultManager().RemoveItemAtPathError(path, null);
            NSData data = path.PathExtension().LowercaseString().IsEqualToString("png") ? UIImagePNGRepresentation(image) : UIImageJPEGRepresentation(image, 1.0);
            data.WriteToFileAtomically(path, true);
        }

        UIImage CreateImage(CGSize size)
        {
            UIGraphicsBeginImageContext(size);
            CGContextRef context = UIGraphicsGetCurrentContext();
            CGContextSetRGBFillColor(context, 1, 1, 1, 1);
            CGContextFillRect(context, CGRectMake(0, 0, size.Width, size.Height));
            UIImage image = UIGraphicsGetImageFromCurrentImageContext();
            UIGraphicsEndImageContext();
            return image;
        }
#endif

        /// <summary>
        /// Builds the XML.
        /// </summary>
        public void BuildXml()
        {
            this.TheDelegate.WebContentMetaDataFinishedWithXmlString(this, this.CalcClientReportFromCachedResult());
        }

        /// <summary>
        /// Calculates the client report from cached result.
        /// </summary>
        /// <returns></returns>
        public string CalcClientReportFromCachedResult()
        {
            int count = this.ClientReports.Count;
            string xslString = string.Empty;
            TimeSpan timeInterval = DateExtensions.TimeIntervalSinceReferenceDate(DateTime.UtcNow);
            if (!string.IsNullOrEmpty(this.XslName))
            {
                //string xslFileName = ServerSession.CurrentSession.FileStore.ImagePathForName(this.XslName);
                //xslFileName = xslFileName.StringByAddingPercentEscapesUsingEncoding(NSUTF8StringEncoding);
                //xslString = $"<?xml-stylesheet type=\"text/xsl\" href=\"{xslFileName}?time={timeInterval}\"?>";
            }

            UPXmlMemoryWriter writer = new UPXmlMemoryWriter();
            if (!string.IsNullOrEmpty(this.RootXmlName))
            {
                writer.WriteElementStart(this.RootXmlName);
            }
            else
            {
                writer.WriteElementStart("Results");
            }

            for (int i = 0; i < count; i++)
            {
                object result = this.resultsForClientReports[i];
                if (result != null)
                {
                    UPWebContentClientReport clientReport = this.ClientReports[i];
                    ((UPCRMResult)result).SerializeRootElementName(writer, clientReport.RootNodeName);
                }
            }

            //for (int j = 0; j < signatureImageTagNames.Count && j < signaturePathNames.Count; j++)
            //{
            //    writer.WriteElementValue(signatureImageTagNames[j], $"{signaturePathNames[j]}?time={timeInterval}");
            //}

            writer.WriteLocaleSettingsElement();
            writer.WriteElementEnd();
            return $"<?xml version=\"1.0\" encoding=\"UTF-8\"?>{xslString}{writer.XmlContentStringAmpDecoded()}";
        }

        private bool ComputeNextReport()
        {
            if (this.ClientReports.Count <= this.nextClientReport)
            {
                this.BuildXml();
                this.blockStart = false;
                return true;
            }

            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            UPWebContentClientReport report = this.ClientReports[this.nextClientReport];
            SearchAndList searchAndList = configStore.SearchAndListByName(report.ConfigName);
            FieldControl tmpFieldControl = searchAndList == null
                ? configStore.FieldControlByNameFromGroup("List", report.ConfigName)
                : configStore.FieldControlByNameFromGroup("List", searchAndList.FieldGroupName);

            if (tmpFieldControl == null)
            {
                this.nextClientReport++;
                return this.ComputeNextReport();
            }

            this.crmQuery = new UPContainerMetaInfo(tmpFieldControl);
            if (!string.IsNullOrEmpty(this.recordIdentification))
            {
                int _linkId = this.linkId;
                if (report.ExplicitLinkId)
                {
                    _linkId = report.LinkId;
                }

                if (string.IsNullOrEmpty(report.ParentLinkConfig))
                {
                    this.crmQuery.SetLinkRecordIdentification(this.recordIdentification, _linkId);
                }
                else if (report.ParentLinkConfig != "nolink")
                {
                    string linkRecordIdentification = this.parentLinkDictionary.ValueOrDefault(report.ParentLinkConfig);
                    if (linkRecordIdentification == null)
                    {
                        this.linkReader = new UPCRMLinkReader(this.recordIdentification, report.ParentLinkConfig, this);
                        this.linkReader.Start();
                        return true;
                    }

                    if (!string.IsNullOrEmpty(linkRecordIdentification))
                    {
                        this.crmQuery.SetLinkRecordIdentification(linkRecordIdentification, _linkId);
                    }
                }
            }

            if (!string.IsNullOrEmpty(searchAndList?.FilterName))
            {
                UPConfigFilter filter = configStore.FilterByName(searchAndList.FilterName);
                filter = filter?.FilterByApplyingValueDictionaryDefaults(this.filterParameters, true);
                if (filter != null)
                {
                    this.crmQuery.ApplyFilter(filter);
                }
            }

            this.crmQuery.Find(this.RequestOption, this);
            return true;
        }

        /// <summary>
        /// Starts the with record identification link identifier parameters.
        /// </summary>
        /// <param name="_recordIdentification">The record identification.</param>
        /// <param name="_linkId">The link identifier.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public bool StartWithRecordIdentificationLinkIdParameters(string _recordIdentification, int _linkId, Dictionary<string, object> parameters)
        {
            if (this.blockStart)
            {
                return false;
            }

            this.blockStart = true;
            this.nextClientReport = 0;
            this.recordIdentification = _recordIdentification;
            this.resultsForClientReports = new List<UPCRMResult>();
            this.filterParameters = parameters;
            int count = this.ClientReports.Count;
            for (int i = 0; i < count; i++)
            {
                this.resultsForClientReports.Add(null);
            }

            this.linkId = _linkId;
            return this.ComputeNextReport();
        }

        /// <summary>
        /// Links the reader did finish with result.
        /// </summary>
        /// <param name="_linkReader">The link reader.</param>
        /// <param name="result">The result.</param>
        public void LinkReaderDidFinishWithResult(UPCRMLinkReader _linkReader, object result)
        {
            string _recordIdentification;
            this.linkReader = null;
            _recordIdentification = _linkReader.DestinationRecordIdentification ?? string.Empty;

            if (this.parentLinkDictionary == null)
            {
                this.parentLinkDictionary = new Dictionary<string, string> { { _linkReader.ParentLinkString, _recordIdentification } };
            }
            else
            {
                this.parentLinkDictionary[_linkReader.ParentLinkString] = _recordIdentification;
            }

            this.ComputeNextReport();
        }

        /// <summary>
        /// Links the reader did finish with error.
        /// </summary>
        /// <param name="linkReader">The link reader.</param>
        /// <param name="error">The error.</param>
        public void LinkReaderDidFinishWithError(UPCRMLinkReader linkReader, Exception error)
        {
            this.crmQuery = null;
            this.TheDelegate.WebContentMetaDataFailedWithError(this, error);
            return;
        }

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.crmQuery = null;
            this.resultsForClientReports = null;
            this.TheDelegate.WebContentMetaDataFailedWithError(this, error);
            return;
        }

        /// <summary>
        /// Searches the operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="result">The result.</param>
        public void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            if (result == null)
            {
                result = UPCRMResult.EmptyClientResult();
            }

            this.resultsForClientReports[this.nextClientReport++] = result;
            this.crmQuery = null;
            this.ComputeNextReport();
        }

        /// <summary>
        /// Searches the operation did finish with results.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="results">The results.</param>
        public void SearchOperationDidFinishWithResults(Operation operation, List<UPCRMResult> results)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches the operation did finish with count.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="count">The count.</param>
        public void SearchOperationDidFinishWithCount(Operation operation, int count)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches the operation did finish with counts.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="counts">The counts.</param>
        public void SearchOperationDidFinishWithCounts(Operation operation, List<int?> counts)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }
    }
}
