// <copyright file="UPWebContentPortfolio.cs" company="Aurea Software Gmbh">
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
//   UPWebContentPortfolio
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel.Web
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.Session;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// UPWebContentPortfolio
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.Web.UPWebContentMetadataStaticHtml" />
    public class UPWebContentPortfolio : UPWebContentMetadataStaticHtml//, UPCalculatePortfolioRequestDelegate
    {
        private float offsetLEFT;
        private float offsetTOP;
        private float offsetRIGHT;
        private float offsetBOTTOM;
        private float svgGridWidth;
        private float svgGridHeight;
        private float radiusPoint;
        private float radiusCorner;
        private float spacing;
        private float sectorPadding;
        private float valueX;
        private float valueY;
        private float spacingAxisText;
        private string valueText;
        private UPPortfolio portfolio;
        private bool loaded;

        //UPCalculatePortfolioServerOperation operation;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPWebContentPortfolio"/> class.
        /// </summary>
        /// <param name="theDelegate">The delegate.</param>
        public UPWebContentPortfolio(UPWebContentMetadataDelegate theDelegate)
            : base(theDelegate)
        {
            this.SetDefaults();
        }

        /// <summary>
        /// Sets the defaults.
        /// </summary>
        public void SetDefaults()
        {
            this.radiusCorner = 0.0f;
            this.radiusPoint = 15.0f;
            this.spacing = 4.0f;
            this.sectorPadding = 4.0f;
            this.valueText = string.Empty;
            this.valueX = 0.0f;
            this.valueY = 0.0f;
            this.spacingAxisText = 30;
            this.loaded = false;
        }

        /// <summary>
        /// Calculates the height of the grid size for width.
        /// </summary>
        /// <param name="elementWidth">Width of the element.</param>
        /// <param name="elementHeight">Height of the element.</param>
        public void CalculateGridSizeForWidthHeight(float elementWidth, float elementHeight)
        {
            this.offsetTOP = this.radiusPoint * 1.2f;
            this.offsetRIGHT = this.radiusPoint * 1.2f;
            this.offsetBOTTOM = this.radiusPoint * 1.0f;
            this.offsetLEFT = this.radiusPoint * 1.2f + 10;
            this.svgGridWidth = elementWidth - this.offsetLEFT - this.offsetRIGHT;
            this.svgGridHeight = elementHeight - this.offsetTOP - this.offsetBOTTOM - this.spacingAxisText;
        }

        /// <summary>
        /// Gets the type of the report.
        /// </summary>
        /// <value>
        /// The type of the report.
        /// </value>
        public override string ReportType => "Portfolio";

        /// <summary>
        /// Gets a value indicating whether [allows full screen].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allows full screen]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowsFullScreen => true;

        /// <summary>
        /// Gets logging interface
        /// </summary>
        public ILogger Logger => SimpleIoc.Default.GetInstance<ILogger>();

        /// <summary>
        /// Updates the metadata with view reference.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public override void UpdateMetadataWithViewReference(ViewReference viewReference)
        {
            base.UpdateMetadataWithViewReference(viewReference);
            if (!this.loaded)
            {
                ServiceInfo serviceInfo = ServerSession.CurrentSession.ServiceInfoForServiceName("Portfolio");
                string survey = viewReference.ContextValueForKey("RecordId");
                if (serviceInfo != null && serviceInfo.IsAtLeastVersion("1.0"))
                {
                    // this.operation = new UPCalculatePortfolioServerOperation(survey, this);
                    // ServerSession.CurrentSession.ExecuteRequest(this.operation);
                }
                else
                {
                    this.Logger.LogError("Server does not support calculate portfolio operation.");
                }

                this.loaded = true;
            }
        }

        //void CalculatePortfolioDidFinishWithResult(UPCalculatePortfolioServerOperation sender, Dictionary<string, object> result)
        //{
        //    UPPortfolio portfolioResult = new UPPortfolio(result);
        //    this.portfolio = portfolioResult;
        //    string svgString = this.GetSVGStringFromResult(result);
        //    this.TheDelegate.WebContentMetaDataFinishedWithXmlString(this, svgString);
        //}

        string GetSVGStringFromResult(object result)
        {
            throw new NotImplementedException(); // Port this function when needed
        }

        Dictionary<string, object> CalculateAxis()
        {
            throw new NotImplementedException(); // Port this function when needed
        }

        object CreateTextElementForParentElementTextTooltipColor(object parentElement, object text, string tooltip,
            string color)
        {
            throw new NotImplementedException(); // Port this function when needed
        }

        void DrawAxisIntoXGroupYGroup(object groupX, object groupY)
        {
            throw new NotImplementedException(); // Port this function when needed
        }

        List<object> CalculcateSectors()
        {
            throw new NotImplementedException();
        }

        void DrawSectorsIntoDefGroupSectionGroupSectionTextGroup(object defs, object groupSections, object groupSectionsText)
        {
            throw new NotImplementedException();
        }

        Dictionary<string, object> CalculateResultPositionForXY(float x, float y)
        {
            throw new NotImplementedException();
        }

        void DrawResultPointIntoGroup(object groupResultPoint)
        {
            throw new NotImplementedException();
        }

        //void CalculatePortfolioDidFailWithError(UPCalculatePortfolioServerOperation sender, Exception error)
        //{
        //    //DDLogError("Could not load portfolio data.");
        //    this.TheDelegate.WebContentMetaDataFailedWithError(this, error);
        //}

        List<object> CalculateLegend()
        {
            throw new NotImplementedException();
        }

        void DrawLegendIntoElement(object parentElement)
        {
            throw new NotImplementedException();
        }

        void DrawResultBoxIntoElement(object resultBox)
        {
            throw new NotImplementedException();
        }
    }
}
