// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupModelController.cs" company="Aurea Software Gmbh">
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
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Group
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.Services.ModelControllers.Settings;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Contexts;
    using Aurea.CRM.UIModel.Groups;
    using GalaSoft.MvvmLight.Messaging;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// Constants
    /// </summary>
    public partial class Constants
    {
        /// <summary>
        /// The generic formcontrol action form
        /// </summary>
        public const string GenericFormcontrolActionform = "Generic:1011";

        /// <summary>
        /// The generic formcontrol trigger execution
        /// </summary>
        public const string GenericFormcontrolTriggerExecution = "Generic:1012";

        /// <summary>
        /// The generic formcontrol analysis
        /// </summary>
        public const string GenericFormcontrolAnalysis = "Generic:1020";

        /// <summary>
        /// The generic formcontrol query
        /// </summary>
        public const string GenericFormcontrolQuery = "Query";
    }

    /// <summary>
    /// States of group model controller
    /// </summary>
    public enum GroupModelControllerState
    {
        /// <summary>
        /// Finished
        /// </summary>
        Finished = 1,

        /// <summary>
        /// Empty
        /// </summary>
        Empty = 2,

        /// <summary>
        /// Error
        /// </summary>
        Error = 3,

        /// <summary>
        /// Pending
        /// </summary>
        Pending = 4
    }

    /// <summary>
    /// Group model controller
    /// </summary>
    public class UPGroupModelController
    {
        private Dictionary<string, string> _properties;
        private UPCRMResultRow _applyResultRow;
        private Dictionary<string, object> _applyDictionary;
        private List<string> _dependingKeyArray;
        private bool _changedControllerState;

        /// <summary>
        /// The group
        /// </summary>
        protected UPMGroup group;

        /// <summary>
        /// The controller state
        /// </summary>
        protected GroupModelControllerState controllerState;

        /// <summary>
        /// The alternate group model controller
        /// </summary>
        protected UPGroupModelController alternateGroupModelController;

        /// <summary>
        /// The alternate group model controller active
        /// </summary>
        protected bool AlternateGroupModelControllerActive;

        /// <summary>
        /// The action identifier to view reference dictionary
        /// </summary>
        protected Dictionary<string, ViewReference> ActionIdentifierToViewReferenceDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPGroupModelController"/> class.
        /// </summary>
        /// <param name="formItem">The form item.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPGroupModelController(FormItem formItem, IGroupModelControllerDelegate theDelegate)
        {
            this.Delegate = theDelegate;
            this.FormItem = formItem;
            this.controllerState = GroupModelControllerState.Empty;
            this._changedControllerState = true;
            this.RequestOption = UPRequestOption.FastestAvailable;
            this.RootTabIndex = -1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPGroupModelController"/> class.
        /// </summary>
        /// <param name="theDelegate">The delegate.</param>
        public UPGroupModelController(IGroupModelControllerDelegate theDelegate)
            : this(null, theDelegate)
        {
        }

        /// <summary>
        /// Gets a value indicating whether [delegate cleared].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [delegate cleared]; otherwise, <c>false</c>.
        /// </value>
        public bool DelegateCleared { get; private set; }

        /// <summary>
        /// Gets the root group model controller.
        /// </summary>
        /// <value>
        /// The root group model controller.
        /// </value>
        public virtual UPGroupModelController RootGroupModelController
            => this.ParentGroupModelController == null ? this : this.ParentGroupModelController.RootGroupModelController;

        /// <summary>
        /// Gets a value indicating whether [online only].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [online only]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool OnlineOnly => false;

        /// <summary>
        /// Gets the name of the value.
        /// </summary>
        /// <value>
        /// The name of the value.
        /// </value>
        public virtual string ValueName => this.FormItem?.ValueName;

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public virtual string Value => null;

        /// <summary>
        /// Gets or sets a value indicating whether [changed controller state].
        /// </summary>
        /// <value>
        /// <c>true</c> if [changed controller state]; otherwise, <c>false</c>.
        /// </value>
        protected virtual bool ChangedControllerState
        {
            get
            {
                if (this.AlternateGroupModelControllerActive)
                {
                    return this.alternateGroupModelController.ChangedControllerState;
                }

                if (this._changedControllerState)
                {
                    this._changedControllerState = false;
                    return true;
                }

                return false;
            }

            set
            {
                if (this.AlternateGroupModelControllerActive)
                {
                    this.alternateGroupModelController.ChangedControllerState = value;
                }

                if (this._changedControllerState)
                {
                    this._changedControllerState = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the state of the controller.
        /// </summary>
        /// <value>
        /// The state of the controller.
        /// </value>
        public virtual GroupModelControllerState ControllerState
        {
            get
            {
                return this.AlternateGroupModelControllerActive
                    ? this.alternateGroupModelController.controllerState
                    : this.controllerState;
            }

            set
            {
                this.controllerState = value;
                this.ChangedControllerState = true;
            }
        }

        /// <summary>
        /// Gets or sets the explicit label.
        /// </summary>
        /// <value>
        /// The explicit label.
        /// </value>
        public string ExplicitLabel { get; set; }

        /// <summary>
        /// Gets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public IGroupModelControllerDelegate Delegate { get; private set; }

        /// <summary>
        /// Gets or sets the request option.
        /// </summary>
        /// <value>
        /// The request option.
        /// </value>
        public virtual UPRequestOption RequestOption { get; set; }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public virtual Exception Error { get; protected set; }

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        /// <value>
        /// The group.
        /// </value>
        public virtual UPMGroup Group
        {
            get { return this.AlternateGroupModelControllerActive ? this.alternateGroupModelController.Group : this.group; }

            set { this.group = value; }
        }

        /// <summary>
        /// Gets or sets the explicit tab identifier.
        /// </summary>
        /// <value>
        /// The explicit tab identifier.
        /// </value>
        public IIdentifier ExplicitTabIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the alternate group model controller.
        /// </summary>
        /// <value>
        /// The alternate group model controller.
        /// </value>
        public UPGroupModelController AlternateGroupModelController
        {
            get
            {
                return this.alternateGroupModelController;
            }

            set
            {
                this.alternateGroupModelController = value;
                this.alternateGroupModelController.ParentGroupModelController = this;
            }
        }

        /// <summary>
        /// Gets or sets the parent group model controller.
        /// </summary>
        /// <value>
        /// The parent group model controller.
        /// </value>
        public UPGroupModelController ParentGroupModelController { get; set; }

        /// <summary>
        /// Gets the form item.
        /// </summary>
        /// <value>
        /// The form item.
        /// </value>
        public FormItem FormItem { get; protected set; }

        /// <summary>
        /// Gets or sets the index of the root tab.
        /// </summary>
        /// <value>
        /// The index of the root tab.
        /// </value>
        public virtual int RootTabIndex { get; set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="UPGroupModelController"/> is finished.
        /// </summary>
        /// <value>
        ///   <c>true</c> if finished; otherwise, <c>false</c>.
        /// </value>
        public virtual bool Finished => this.controllerState != GroupModelControllerState.Pending;

        /// <summary>
        /// Gets the current data.
        /// </summary>
        /// <value>
        /// The current data.
        /// </value>
        public virtual Dictionary<string, object> CurrentData => null;

        /// <summary>
        /// Sets the property value for key.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="key">The key.</param>
        public virtual void SetPropertyValueForKey(string value, string key)
        {
            if (this._properties == null)
            {
                this._properties = new Dictionary<string, string>
                {
                    { key, value }
                };
            }
            else
            {
                this._properties.SetObjectForKey(value, key);
            }
        }

        /// <summary>
        /// Clears the delegate.
        /// </summary>
        public virtual void ClearDelegate()
        {
            this.Delegate = null;
            this.DelegateCleared = true;
        }

        /// <summary>
        /// Properties the value for key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public virtual string PropertyValueForKey(string key)
        {
            return this._properties?.ValueOrDefault(key);
        }

        /// <summary>
        /// Detailses the group model controller.
        /// </summary>
        /// <param name="fieldControl">The field control.</param>
        /// <param name="tabIndex">Index of the tab.</param>
        /// <param name="theDelegate">The delegate.</param>
        /// <returns></returns>
        public static UPGroupModelController DetailsGroupModelController(FieldControl fieldControl, int tabIndex, IGroupModelControllerDelegate theDelegate)
        {
            return UPFieldControlBasedGroupModelController.DetailsGroupModelControllerForFieldControl(fieldControl, tabIndex, theDelegate);
        }

        /// <summary>
        /// Edits the group model controller for control.
        /// </summary>
        /// <param name="fieldControl">The field control.</param>
        /// <param name="tabIndex">Index of the tab.</param>
        /// <param name="editPageContext">The edit page context.</param>
        /// <param name="theDelegate">The delegate.</param>
        /// <returns></returns>
        public static UPGroupModelController EditGroupModelControllerForControl(FieldControl fieldControl, int tabIndex, UPEditPageContext editPageContext, IGroupModelControllerDelegate theDelegate)
        {
            return UPFieldControlBasedEditGroupModelController.EditGroupModelControllerFor(fieldControl, tabIndex, editPageContext, theDelegate);
        }

        /// <summary>
        /// Groups the model controller.
        /// </summary>
        /// <param name="formItem">The form item.</param>
        /// <param name="identifier">The identifier.</param>
        /// <param name="theDelegate">The delegate.</param>
        /// <returns></returns>
        public static UPGroupModelController GroupModelController(FormItem formItem, IIdentifier identifier, IGroupModelControllerDelegate theDelegate)
        {
            if (formItem?.ViewReference == null)
            {
                return null;
            }

            var viewName = formItem.ViewReference.ViewName;
            try
            {
                switch (viewName)
                {
                    case "RecordList":
                        return new UPListResultGroupModelController(formItem, identifier, theDelegate);

                    case "Map":
                        return new UPMultiMapGroupModelController(formItem, identifier, theDelegate);

                    case "WebContent":
                        return new UPWebContentGroupModelController(formItem, identifier, theDelegate);

                    case "InsightBoard":
                        return new UPInsightBoardGroupModelController(formItem, identifier, theDelegate);

                    case "DatePicker":
                        return new UPCalendarGroupModelController(formItem, identifier, theDelegate);

                    case "Details":
                        return new UPCalendarGroupModelController(formItem, identifier, theDelegate);

                    case "FormControl":
                        var controlType = formItem.ViewReference.ContextValueForKey("ControlType");
                        if (controlType == Constants.GenericFormcontrolActionform)
                        {
                            return new UPActionFormGroupModelController(formItem, identifier, theDelegate);
                        }

                        if (controlType == Constants.GenericFormcontrolTriggerExecution)
                        {
                            //return new UPActionGroupModelController(formItem, identifier, theDelegate);
                        }

                        if (controlType == Constants.GenericFormcontrolAnalysis)
                        {
                            //return new UPAnalysisListResultGroupModelController(formItem, identifier, theDelegate);
                        }

                        if (controlType == Constants.GenericFormcontrolQuery)
                        {
                            //return new UPQueryListResultGroupModelController(formItem, identifier, theDelegate);
                        }

                        break;
                }
            }
            catch (Exception exception)
            {
                SimpleIoc.Default.GetInstance<ILogger>().LogError($"Uanble to load the controller {viewName}. Exception Details: {exception.Message} & Stacktrace: {exception.StackTrace}");
                Messenger.Default.Send(new Core.Messages.ToastrMessage
                {
                    MessageText = $"Uanble to load the controller {viewName}",
                    DetailedMessage = $"{exception.InnerException?.Message}"
                });
            }

            if (!string.IsNullOrEmpty(formItem.Label) && !string.IsNullOrEmpty(formItem.ViewReference.ContextValueForKey("Func")))
            {
                return new UPStaticTextGroupModelController(formItem, identifier, theDelegate);
            }

            if (ConfigurationUnitStore.DefaultStore.ConfigValueIsSetDefaultValue("Form.ShowUnknownControls", false))
            {
                return new UnknownControlGroupModelController(formItem, identifier, theDelegate);
            }

            return null;
        }

        /// <summary>
        /// Settingses the group model controller.
        /// </summary>
        /// <param name="layout">The layout.</param>
        /// <param name="tabIndex">Index of the tab.</param>
        /// <returns></returns>
        public static UPGroupModelController SettingsGroupModelController(WebConfigLayout layout, int tabIndex)
        {
            return new SettingsViewGroupModelController(layout, tabIndex);
        }

        /// <summary>
        /// Settingses the edit group model controller.
        /// </summary>
        /// <param name="layout">The layout.</param>
        /// <param name="tabIndex">Index of the tab.</param>
        /// <returns></returns>
        public static UPGroupModelController SettingsEditGroupModelController(WebConfigLayout layout, int tabIndex)
        {
            return new SettingsEditViewGroupModelController(layout, tabIndex);
        }

        /// <summary>
        /// Adds the depending keys from view reference.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public virtual void AddDependingKeysFromViewReference(ViewReference viewReference)
        {
            foreach (var arg in viewReference.Arguments.Values)
            {
                if (arg.Value.StartsWith("$"))
                {
                    this.AddDependingKey(arg.Value);
                }
            }
        }

        /// <summary>
        /// Gets the tab label.
        /// </summary>
        /// <value>
        /// The tab label.
        /// </value>
        public virtual string TabLabel => this.ExplicitLabel;

        /// <summary>
        /// Handles the empty group.
        /// </summary>
        /// <returns></returns>
        public virtual bool HandleEmptyGroup()
        {
            this.controllerState = GroupModelControllerState.Empty;
            this.Group = null;
            if (this.alternateGroupModelController != null)
            {
                this.AlternateGroupModelControllerActive = true;
                if (this._applyResultRow != null)
                {
                    this.alternateGroupModelController.ApplyResultRow(this._applyResultRow);
                }
                else
                {
                    this.alternateGroupModelController.ApplyContext(this._applyDictionary);
                }

                return this.alternateGroupModelController.controllerState != GroupModelControllerState.Pending;
            }

            return true;
        }

        /// <summary>
        /// Clears the empty group.
        /// </summary>
        public virtual void ClearEmptyGroup()
        {
            this.AlternateGroupModelControllerActive = false;
        }

        /// <summary>
        /// Applies the result row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public virtual UPMGroup ApplyResultRow(UPCRMResultRow row)
        {
            this._applyResultRow = row;
            this._applyDictionary = null;
            return null;
        }

        /// <summary>
        /// Applies the context.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns></returns>
        public virtual UPMGroup ApplyContext(Dictionary<string, object> dictionary)
        {
            this._applyResultRow = null;
            this._applyDictionary = dictionary;
            return null;
        }

        /// <summary>
        /// Settingses the group.
        /// </summary>
        /// <returns></returns>
        public virtual UPMGroup SettingsGroup() => null;

        /// <summary>
        /// Adds the view reference for action key.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="key">The key.</param>
        public virtual void AddViewReferenceForActionKey(ViewReference viewReference, string key)
        {
            if (this.ActionIdentifierToViewReferenceDictionary == null)
            {
                this.ActionIdentifierToViewReferenceDictionary = new Dictionary<string, ViewReference>();
            }

            this.ActionIdentifierToViewReferenceDictionary.SetObjectForKey(viewReference, key);
        }

        /// <summary>
        /// Views the reference for action key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public virtual ViewReference ViewReferenceForActionKey(string key)
        {
            return this.ActionIdentifierToViewReferenceDictionary.ValueOrDefault(key);
        }

        /// <summary>
        /// Adds the depending key.
        /// </summary>
        /// <param name="dependingKey">The depending key.</param>
        public virtual void AddDependingKey(string dependingKey)
        {
            if (this._dependingKeyArray == null)
            {
                this._dependingKeyArray = new List<string> { dependingKey };
            }
            else
            {
                this._dependingKeyArray.Add(dependingKey);
            }
        }

        /// <summary>
        /// Affecteds the by key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public virtual bool AffectedByKey(string key)
        {
            return this._dependingKeyArray?.Any(dk => dk.Equals(key)) ?? false;
        }

        /// <summary>
        /// Applies the current values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public virtual UPMGroup ApplyCurrentValues(Dictionary<string, object> values)
        {
            return this.Group;
        }

        /// <summary>
        /// Finds the quick actions for row.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        public virtual void FindQuickActionsForRow(UPMListRow resultRow)
        {
            this.alternateGroupModelController?.FindQuickActionsForRow(resultRow);
        }

        /// <summary>
        /// Datas the name of from value.
        /// </summary>
        /// <param name="valueName">Name of the value.</param>
        /// <returns></returns>
        public virtual Dictionary<string, object> DataFromValueName(string valueName)
        {
            return this.Delegate?.FindAndInvokeMethod("DataFromValueName", new object[] { valueName }) as Dictionary<string, object>;
        }
    }
}
