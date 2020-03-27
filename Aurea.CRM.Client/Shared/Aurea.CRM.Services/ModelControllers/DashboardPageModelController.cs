// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DashboardPageModelController.cs" company="Aurea Software Gmbh">
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
namespace Aurea.CRM.Services.ModelControllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Services.ModelControllers.Group;
    using Aurea.CRM.Services.ModelControllers.Organizer;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Contexts;
    using Aurea.CRM.UIModel.Fields.Edit;
    using Aurea.CRM.UIModel.Identifiers;
    using Aurea.CRM.UIModel.Pages;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// Dashboard Page Model Controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.GroupBasedPageModelController" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.UPCopyFieldsDelegate" />
    public class DashboardPageModelController : GroupBasedPageModelController, UPCopyFieldsDelegate
    {
        private FormTab formtab;
        private Dictionary<string, UPGroupModelController> namedGroupController;
        private Dictionary<string, object> copyFieldDictionary;
        private int initializationState;
        private UPCopyFields copyFields;
        private int smartbookMenuPosition;

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardPageModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public DashboardPageModelController(ViewReference viewReference)
            : base(viewReference)
        {
            this.smartbookMenuPosition = -1;
            UPAppProcessContext currentContext = UPAppProcessContext.CurrentContext;
            int navControllerId = this.ParentOrganizerModelController?.NavControllerId ?? 0;

            string addDashboardFormName = currentContext.ContextValueForKey("firstDashboardName", navControllerId) as string;
            string addDashboardConfigTabNr = currentContext.ContextValueForKey("firstDashboardConfigTabNr", navControllerId) as string;
            string formName = viewReference.ContextValueForKey("ConfigName");
            string tabNr = viewReference.ContextValueForKey("ConfigTabNr");

            if (addDashboardFormName == null)
            {
                currentContext.SetContextValueForKey(formName, "firstDashboardName", navControllerId);
                currentContext.SetContextValueForKey(tabNr, "firstDashboardConfigTabNr", navControllerId);
                this.smartbookMenuPosition = Convert.ToInt32(ConfigurationUnitStore.DefaultStore.ConfigValueDefaultValue("StartPage.LegacyMenuPosition", "0"));
            }
            else if (addDashboardFormName == formName && (tabNr == null || tabNr == addDashboardConfigTabNr))
            {
                this.smartbookMenuPosition = Convert.ToInt32(ConfigurationUnitStore.DefaultStore.ConfigValueDefaultValue("StartPage.LegacyMenuPosition", "0"));
            }

            this.BuildPage();
        }

        /// <summary>
        /// Gets logging interface
        /// </summary>
        public ILogger Logger => SimpleIoc.Default.GetInstance<ILogger>();

        /// <summary>
        /// Gets the configuration tab nr.
        /// </summary>
        /// <value>
        /// The configuration tab nr.
        /// </value>
        public int ConfigTabNr { get; private set; }

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get; private set; }

        /// <summary>
        /// Instantiates the page.
        /// </summary>
        /// <returns></returns>
        public override Page InstantiatePage()
        {
            return this.Page != null
                ? new MDashboardPage(this.Page.Identifier)
                : new MDashboardPage(StringIdentifier.IdentifierWithStringId("Form"));
        }

        /// <summary>
        /// Resets the with reason.
        /// </summary>
        /// <param name="reason">The reason.</param>
        public override void ResetWithReason(ModelControllerResetReason reason)
        {
            bool isStartNavController = this.ParentOrganizerModelController.NavControllerId == UPMultipleOrganizerManager.CurrentOrganizerManager.StartOrganizerNavControllerId;
            if (reason != ModelControllerResetReason.MultiOrganizerSwitch || !isStartNavController)
            {
                base.ResetWithReason(reason);
                this.BuildPage();
            }
        }

        /// <summary>
        /// Builds the page.
        /// </summary>
        public override void BuildPage()
        {
            this.ConfigName = this.ViewReference.ContextValueForKey("ConfigName");
            this.ConfigTabNr = Convert.ToInt32(this.ViewReference.ContextValueForKey("ConfigTabNr"));
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            Form form = configStore.FormByName(this.ConfigName);
            this.formtab = null;
            MDashboardPage _page = (MDashboardPage)this.InstantiatePage();

            if (this.ConfigTabNr < form.NumberOfTabs)
            {
                if (this.ConfigTabNr < 0)
                {
                    this.ConfigTabNr = 0;
                }

                this.formtab = form.TabAtIndex(this.ConfigTabNr);
            }
            else
            {
                this.Logger.LogError($"Configured ConfigTabNr {this.ConfigTabNr} is larger than the number of available tabs {form.NumberOfTabs}");

                // DDLogError("Configured ConfigTabNr (%ld) is larger than the number of available tabs (%lu)", (long)this.ConfigTabNr, (unsigned long)form.NumberOfTabs());
                return;
            }

            _page.LabelText = !string.IsNullOrWhiteSpace(this.formtab.Label) ? this.formtab.Label : "*** FormTab Label Missing ***";

            _page.Invalid = true;
            this.TopLevelElement = _page;
            string copyRecordIdentification = this.ViewReference.ContextValueForKey("RecordId");
            string copyFieldGroupName = this.ViewReference.ContextValueForKey("CopySourceFieldGroupName");
            this.copyFields = null;

            if (copyRecordIdentification.IsRecordIdentification() && !string.IsNullOrWhiteSpace(copyFieldGroupName))
            {
                FieldControl fieldControl = configStore.FieldControlByNameFromGroup("List", copyFieldGroupName);
                if (fieldControl != null)
                {
                    this.copyFields = new UPCopyFields(fieldControl);
                }
            }

            if (this.copyFields != null)
            {
                this.copyFields.CopyFieldValuesForRecordIdentification(copyRecordIdentification, false, this);
            }
            else
            {
                this.ContinueWithCopyFields(null);
            }
        }

        private void ContinueWithCopyFields(Dictionary<string, object> _copyFieldDictionary)
        {
            var itemId = 0;
            Dictionary<string, object> fixedCopyFields = null;
            var fixedCopyFieldString = this.ViewReference.ContextValueForKey("FixedCopyFieldValues");
            if (!string.IsNullOrWhiteSpace(fixedCopyFieldString))
            {
                fixedCopyFields = fixedCopyFieldString.JsonDictionaryFromString();
            }

            if (_copyFieldDictionary != null)
            {
                if (fixedCopyFields != null)
                {
                    Dictionary<string, object> dict = new Dictionary<string, object>(fixedCopyFields);
                    foreach (var item in _copyFieldDictionary)
                    {
                        dict.Add(item.Key, item.Value);
                    }

                    this.copyFieldDictionary = dict;
                }
                else
                {
                    this.copyFieldDictionary = _copyFieldDictionary;
                }
            }
            else
            {
                this.copyFieldDictionary = fixedCopyFields;
            }

            if (this.copyFieldDictionary?.Count > 0)
            {
                this.AddValueWithValueName(this.copyFieldDictionary, "_copyFields");
            }

            this.ClearGroupControllers();
            var formRows = this.GetFormRows();

            var index = 0;
            foreach (var formRow in formRows)
            {
                this.ProcessFormRow(formRow, ref index, ref itemId);
            }
        }

        private IEnumerable<FormRow> GetFormRows()
        {
            var formRows = this.formtab.Rows;
            if (this.smartbookMenuPosition < 0)
            {
                return formRows;
            }

            var configStore = ConfigurationUnitStore.DefaultStore;
            var menu = configStore.FormByName("StartPage.LegacyMenuTemplate");
            var formRowFromSmartbookMenu = menu.TabAtIndex(0).RowAtIndex(0);

            if (formRowFromSmartbookMenu == null)
            {
                return formRows;
            }

            var adjustedFormRowArray = new List<FormRow>(formRows.Count + 1);
            if (formRows.Count < this.smartbookMenuPosition)
            {
                this.smartbookMenuPosition = formRows.Count;
            }

            var start = 0;
            int count;
            if (this.smartbookMenuPosition > 0)
            {
                count = this.smartbookMenuPosition;
                adjustedFormRowArray.AddRange(formRows.GetRange(start, count));
                start += this.smartbookMenuPosition;
                count = formRows.Count - this.smartbookMenuPosition;
            }
            else
            {
                count = formRows.Count;
            }

            adjustedFormRowArray.Add(formRowFromSmartbookMenu);
            if (count > 0)
            {
                adjustedFormRowArray.AddRange(formRows.GetRange(start, count));
            }

            formRows = adjustedFormRowArray;

            return formRows;
        }

        private void ProcessFormRow(FormRow formRow, ref int index, ref int itemId)
        {
            var items = formRow.Items;
            var count = items.Count;
            if (count <= 0)
            {
                return;
            }

            var formItem = items[0];
            var basisIdentifierAsString = $"Group_{itemId++}";
            var groupIdentifier = StringIdentifier.IdentifierWithStringId($"{basisIdentifierAsString}_0");
            var groupModelController = UPGroupModelController.GroupModelController(formItem, groupIdentifier, this);
            if (groupModelController == null)
            {
                return;
            }

            groupModelController.ExplicitLabel = formItem.Label;
            groupModelController.RootTabIndex = index;
            if (count > 1)
            {
                var currentModelController = groupModelController;
                for (var indexFormRow = 1; indexFormRow  < count; indexFormRow ++)
                {
                    var alternateItem = items[indexFormRow ];
                    var alternateModelController = UPGroupModelController.GroupModelController(alternateItem, StringIdentifier.IdentifierWithStringId($"{0}_{indexFormRow }"), this);
                    if (alternateModelController != null)
                    {
                        alternateModelController.RootTabIndex = index;
                        alternateModelController.ExplicitLabel = alternateItem.Label;
                        currentModelController.AlternateGroupModelController = alternateModelController;
                        currentModelController = alternateModelController;
                    }
                }
            }

            this.AddGroupController(groupModelController);
            index++;
            if (!string.IsNullOrWhiteSpace(formItem.ValueName))
            {
                if (this.namedGroupController != null)
                {
                    this.namedGroupController[formItem.ValueName] = groupModelController;
                }
                else
                {
                    this.namedGroupController = new Dictionary<string, UPGroupModelController>
                    {
                        [formItem.ValueName] = groupModelController
                    };
                }
            }
        }

        /// <summary>
        /// Datas the name of from value.
        /// </summary>
        /// <param name="valueName">Name of the value.</param>
        /// <returns></returns>
        public Dictionary<string, object> DataFromValueName(string valueName)
        {
            UPGroupModelController groupModelController = this.namedGroupController[valueName];
            return groupModelController.CurrentData;
        }

        /// <summary>
        /// Backgrounds the refresh in Start organizer.
        /// </summary>
        /// <returns></returns>
        public override bool BackgroundRefreshInStartOrganizer()
        {
            return true;
        }

        /// <summary>
        /// Updateds the element for page.
        /// </summary>
        /// <param name="oldDetailPage">The old detail page.</param>
        /// <returns></returns>
        public override UPMElement UpdatedElementForPage(Page oldDetailPage)
        {
            MDashboardPage detailPage = (MDashboardPage)this.InstantiatePage();

            lock (this)
            {
                //this.InformAboutDidChangeTopLevelElement(detailPage, detailPage, null, UPChangeHints.ChangeHintsWithHint(Constants.GroupPageChangeHint));
                //Thread.Sleep(2000);
                //List<IIdentifier> changeIdentifier = new List<IIdentifier>();
                foreach (UPGroupModelController groupModelController in this.GroupModelControllerArray)
                {
                    //if (!(groupModelController is UPListResultGroupModelController))
                    //{
                        groupModelController.ApplyContext(this.ValueDictionary);
                    //}
                }

                foreach (UPGroupModelController groupModelController in this.GroupModelControllerArray)
                {
                    if (groupModelController.Group != null)
                    {
                        groupModelController.Group.ConfiguredPostionOfGroup = groupModelController.RootTabIndex;
                        detailPage.AddGroup(groupModelController.Group);
                        //if (groupModelController.ControllerState == GroupModelControllerState.Finished)
                        //{
                        //    changeIdentifier.Add(groupModelController.Group.Identifier);
                        //}
                    }
                }

                detailPage.Invalid = false;
                //this.InformAboutDidChangeTopLevelElement(detailPage, detailPage, changeIdentifier, UPChangeHints.ChangeHintsWithHint(Constants.GroupAddedToPageHint));
            }

            return detailPage;
        }

        /// <summary>
        /// Users the did change field.
        /// </summary>
        /// <param name="field">The field.</param>
        public void UserDidChangeField(UPMEditField field)
        {
            field.EditFieldDelegate?.FieldChangedValue(field);
        }

        /// <summary>
        /// Copies the fields did finish with values.
        /// </summary>
        /// <param name="_copyFields">The copy fields.</param>
        /// <param name="dictionary">The dictionary.</param>
        public void CopyFieldsDidFinishWithValues(UPCopyFields _copyFields, Dictionary<string, object> dictionary)
        {
            this.copyFields = null;
            this.ContinueWithCopyFields(dictionary);
        }

        /// <summary>
        /// Copies the fields did fail with error.
        /// </summary>
        /// <param name="_copyFields">The copy fields.</param>
        /// <param name="error">The error.</param>
        public void CopyFieldsDidFailWithError(UPCopyFields _copyFields, Exception error)
        {
            this.copyFields = null;
            this.ReportError(error, false);
        }
    }
}
