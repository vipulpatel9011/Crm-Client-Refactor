// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DashboardOrganizerModelController.cs" company="Aurea Software Gmbh">
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
//   The Dashboard Organizer Model Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Organizer
{
    using System.Collections.Generic;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// Dashboard Organizer Model Controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Organizer.UPOrganizerModelController" />
    public class UPDashboardOrganizerModelController : UPOrganizerModelController
    {
        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get; private set; }

        /// <summary>
        /// Gets the name of the form.
        /// </summary>
        /// <value>
        /// The name of the form.
        /// </value>
        public string FormName { get; private set; }

        /// <summary>
        /// Gets the name of the header.
        /// </summary>
        /// <value>
        /// The name of the header.
        /// </value>
        public string HeaderName { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPDashboardOrganizerModelController"/> class.
        /// </summary>
        /// <param name="_viewReference">The view reference.</param>
        /// <param name="options">The options.</param>
        public UPDashboardOrganizerModelController(ViewReference _viewReference, UPOrganizerInitOptions options)
            : base(_viewReference, options)
        {
        }

        /// <summary>
        /// Builds the pages from view reference.
        /// </summary>
        public override void BuildPagesFromViewReference()
        {
            var dashboardOrganizer = new UPMOrganizer(StringIdentifier.IdentifierWithStringId("Dashboard"));
            dashboardOrganizer.ExpandFound = true;
            TopLevelElement = dashboardOrganizer;
            var configStore = ConfigurationUnitStore.DefaultStore;
            FormName = ViewReference.ContextValueForKey("ConfigName");
            HeaderName = ViewReference.ContextValueForKey("HeaderName");
            RecordIdentification = ViewReference.ContextValueForKey("RecordId");

            var form = (Form)null;
            if (!string.IsNullOrWhiteSpace(FormName))
            {
                form = configStore.FormByName(FormName);
            }

            var header = (UPConfigHeader)null;
            if (!string.IsNullOrWhiteSpace(HeaderName))
            {
                header = configStore.HeaderByName(HeaderName);
            }

            if (header != null)
            {
                dashboardOrganizer.SubtitleText = header.Label;
                AddActionButtonsFromHeaderRecordIdentification(header, null);
                foreach (UPMElement element in OrganizerHeaderActionItems)
                {
                    if (element is UPMOrganizerAction)
                    {
                        var action = (UPMOrganizerAction)element;
                        if (action.ViewReference.Arguments.ContainsKey("RecordId") ||
                            action.ViewReference.Arguments.ContainsKey("LinkRecordId"))
                        {
                            LogActionWarning(action.LabelText);
                        }
                    }
                    else if (element is UPMOrganizerActionGroup)
                    {
                        var actionGroup = (UPMOrganizerActionGroup)element;
                        foreach (UPMOrganizerAction action in actionGroup.Children)
                        {
                            if (action.ViewReference.Arguments.ContainsKey("RecordId") ||
                                action.ViewReference.Arguments.ContainsKey("LinkRecordId"))
                            {
                                LogActionWarning(action.LabelText);
                            }
                        }
                    }
                }
            }
            else
            {
                var formTab = form?.TabAtIndex(0);
                if (!string.IsNullOrWhiteSpace(formTab?.Label))
                {
                    dashboardOrganizer.SubtitleText = formTab.Label;
                }
            }

            if (form == null)
            {
                return;
            }

            BuildFormTabs(dashboardOrganizer, form);
            BuildHeaderSubViews(dashboardOrganizer, header);
        }

        /// <summary>
        /// Builds Tabs for Forms
        /// </summary>
        /// <param name="dashboardOrganizer">
        /// UPMOrganizer
        /// </param>
        /// <param name="form">
        /// Form
        /// </param>
        private void BuildFormTabs(UPMOrganizer dashboardOrganizer, Form form)
        {
            var count = form.NumberOfTabs;
            for (int i = 0; i < count; i++)
            {
                var parameterDictionary = new Dictionary<string, object> { { "ConfigTabNr", i.ToString() } };
                var pageViewReference = ViewReference.ViewReferenceWith(parameterDictionary);
                pageViewReference = pageViewReference.ViewReferenceWith(RecordIdentification);
                var pageModelController = PageForViewReference(pageViewReference);
                if (pageModelController != null)
                {
                    FormTab tab = form.TabAtIndex(i);
                    if (i == 0 && tab.AttributeForKey("Color") != null)
                    {
                        Organizer.OrganizerColor = AureaColor.ColorWithString(tab.AttributeForKey("Color"));
                    }

                    pageModelController.Page.Parent = dashboardOrganizer;
                    pageModelController.Page.LabelText = tab.Label;
                    AddPageModelController(pageModelController);
                    dashboardOrganizer.AddPage(pageModelController.Page);
                }
            }
        }

        /// <summary>
        /// Builds SubViews for Headers
        /// </summary>
        /// <param name="dashboardOrganizer">
        /// UPMOrganizer
        /// </param>
        /// <param name="header">
        /// UPConfigHeader
        /// </param>
        private void BuildHeaderSubViews(UPMOrganizer dashboardOrganizer, UPConfigHeader header)
        {
            var count = header?.NumberOfSubViews ?? 0;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    var subViewDef = header?.SubViewAtIndex(i);
                    var subViewViewReference = subViewDef?.ViewReference;
                    subViewViewReference = subViewViewReference?.ViewReferenceWith(RecordIdentification);
                    if (subViewViewReference == null)
                    {
                        continue;
                    }

                    var pageModelController = PageForViewReference(subViewViewReference);
                    if (pageModelController != null)
                    {
                        pageModelController.Page.Invalid = true;
                        pageModelController.Page.LabelText = subViewDef.Label;
                        pageModelController.Page.Parent = dashboardOrganizer;
                        AddPageModelController(pageModelController);
                        dashboardOrganizer.AddPage(pageModelController.Page);
                    }
                }
            }
        }

        /// <summary>
        /// Logs Warnings for actions with RecordId and LinkRecordId
        /// </summary>
        /// <param name="label">
        /// Action Label Text
        /// </param>
        private static void LogActionWarning(string label)
        {
            SimpleIoc.Default.GetInstance<ILogger>().LogWarn($"Possible problem by call of action {label}, parameter 'RecordId' and 'LinkRecordId' are not supported.");
        }
    }
}
