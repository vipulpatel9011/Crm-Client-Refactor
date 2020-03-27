// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InsightBoardGroupModelController.cs" company="Aurea Software Gmbh">
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
//   The Insightboard Group Model Controller
// </summary>
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
    using Aurea.CRM.Core.Session;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.Services.ModelControllers.Organizer;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Groups;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// The Insightboard Group Model Controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Group.UPFieldControlBasedGroupModelController" />
    /// <seealso cref="Aurea.CRM.Services.Delegates.IGroupModelControllerDelegate" />
    public class UPInsightBoardGroupModelController : UPFieldControlBasedGroupModelController, IGroupModelControllerDelegate
    {
        private int pending;
        private int layout;
        private bool horizontalStyle;
        private bool center;
        private int maxVisibleRow;
        private ViewReference testViewReference;

        private List<UPInsightBoardItemGroupModelController> itemControllerArray;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPInsightBoardGroupModelController"/> class.
        /// </summary>
        /// <param name="formItem">The form item.</param>
        /// <param name="identifier">The identifier.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPInsightBoardGroupModelController(FormItem formItem, IIdentifier identifier, IGroupModelControllerDelegate theDelegate)
            : base(formItem, theDelegate)
        {
            this.itemControllerArray = new List<UPInsightBoardItemGroupModelController>();
            this.ExplicitTabIdentifier = identifier;
            this.ExplicitLabel = formItem.Label;
            this.testViewReference = formItem.ViewReference;
            string contextMenuName = formItem.ViewReference.ContextValueForKey("MenuName");
            string recordIdentification = formItem.ViewReference.ContextValueForKey("RecordId");
            recordIdentification = UPCRMDataStore.DefaultStore.ReplaceRecordIdentification(recordIdentification);
            this.ConfigInsightBoard(contextMenuName, recordIdentification);

            UPMInsightBoardGroup insightBoardGroup = new UPMInsightBoardGroup(identifier);
            insightBoardGroup.LabelText = formItem.Label;

            if (formItem.Options != null)
            {
                string heightValue = formItem.Options.ContainsKey("height") ? formItem.Options["height"] as string : null;
                if (!string.IsNullOrEmpty(heightValue))
                {
                    insightBoardGroup.Height = Convert.ToInt32(heightValue);
                }

                string maxVisibleItems = formItem.Options.ContainsKey("MaxVisibleItems") ? formItem.Options["MaxVisibleItems"] as string : null;
                if (!string.IsNullOrEmpty(maxVisibleItems))
                {
                    insightBoardGroup.MaxVisibleItems = Convert.ToInt32(maxVisibleItems);
                }

                string vertical = formItem.Options.ContainsKey("Vertical") ? formItem.Options["Vertical"] as string : null;
                if (!string.IsNullOrEmpty(vertical))
                {
                    var isIntValue = int.TryParse(vertical, out int intValue);
                    insightBoardGroup.SingleRow = isIntValue ? !Convert.ToBoolean(intValue) : !Convert.ToBoolean(vertical);
                    if (!insightBoardGroup.SingleRow)
                    {
                        insightBoardGroup.Center = true;
                    }
                }

                string centerString = formItem.Options.ContainsKey("Center") ? formItem.Options["Center"] as string : null;
                if (!string.IsNullOrEmpty(centerString))
                {
                    insightBoardGroup.Center = Convert.ToBoolean(centerString);
                }

                string layoutValue = formItem.Options.ContainsKey("layout") ? formItem.Options["layout"] as string : null;
                if (!string.IsNullOrEmpty(layoutValue))
                {
                    insightBoardGroup.Layout = (UPMInsightBoardGroupLayout)Convert.ToInt32(layoutValue);
                }
            }

            this.ControllerState = this.itemControllerArray.Count > 0 ? GroupModelControllerState.Pending : GroupModelControllerState.Empty;
            this.Group = insightBoardGroup;
            this.AddDependingKeysFromViewReference(formItem.ViewReference);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPInsightBoardGroupModelController"/> class.
        /// </summary>
        /// <param name="fieldControl">The field control.</param>
        /// <param name="tabIndex">Index of the tab.</param>
        /// <param name="theDelegate">The delegate.</param>
        /// <param name="horizontalStyle">if set to <c>true</c> [horizontal style].</param>
        /// <param name="center">if set to <c>true</c> [center].</param>
        public UPInsightBoardGroupModelController(FieldControl fieldControl, int tabIndex, IGroupModelControllerDelegate theDelegate,
            bool horizontalStyle = true, bool center = false)
            : base(fieldControl, tabIndex, theDelegate)
        {
            this.horizontalStyle = horizontalStyle;
            this.center = center;
            this.itemControllerArray = new List<UPInsightBoardItemGroupModelController>();
            var typeParts = this.TabConfig.Type.Split('_');
            if (typeParts.Length >= 2)
            {
                string contextMenuName = typeParts[1];
                this.ConfigInsightBoard(contextMenuName, null);
                if (typeParts.Length >= 3)
                {
                    this.maxVisibleRow = Convert.ToInt32(typeParts[2]);
                }
            }
            else
            {
                SimpleIoc.Default.GetInstance<ILogger>().LogWarn("InsightBoard: no context menu configurated");
            }
        }

        private void ConfigInsightBoard(string contextMenuName, string recordIdentification)
        {
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            Menu configMenu = configStore.MenuByName(contextMenuName);
            var skipMenusList = this.FormItem?.Options.ValueOrDefault("SkipMenus");
            List<string> skipMenus = skipMenusList as List<string>
                ?? (skipMenusList as List<object>)?.Cast<string>().ToList();

            int count = 0;
            bool useAllKindOfItems = Convert.ToInt32(this.FormItem?.Options.ValueOrDefault("useAllKindOfItems")) != 0;
            for (int i = 0; i < configMenu?.NumberOfSubMenus; i++)
            {
                Menu submenu = configMenu.SubMenuAtIndex(i);
                if (submenu == null
                    || skipMenus?.Contains(submenu.UnitName) == true
                    || submenu?.ViewReference?.ViewName == "DebugView")
                {
                    continue;
                }

                if (submenu.ViewReference?.ViewName == "InsightBoardItem")
                {
                    this.itemControllerArray.Add(new UPInsightBoardItemGroupModelController(submenu, this, count++, this.testViewReference, recordIdentification));
                }
                else
                {
                    if (useAllKindOfItems)
                    {
                        this.itemControllerArray.Add(new UPInsightBoardItemGroupModelController(submenu, this, count++, this.testViewReference, null));
                    }
                    else
                    {
                        SimpleIoc.Default.GetInstance<ILogger>().LogWarn($"ViewName '{submenu.ViewReference?.ViewName}' not as insightBoard submenu supported");
                    }
                }
            }

            this.pending = this.itemControllerArray.Count;
        }

        /// <summary>
        /// Applies the result row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public override UPMGroup ApplyResultRow(UPCRMResultRow row)
        {
            string recordIdentification = row.RootRecordIdentification;
            UPMInsightBoardGroup insightBoardGroup = new UPMInsightBoardGroup(this.TabIdentifierForRecordIdentification(recordIdentification))
            {
                LabelText = this.TabLabel,
                Layout = UPMInsightBoardGroupLayout.Layout2,
                Height = 0,
                SingleRow = this.horizontalStyle,
                Center = !this.horizontalStyle && this.center,
                MaxVisibleItems = this.maxVisibleRow
            };

            this.pending = this.itemControllerArray.Count;
            this.ControllerState = this.itemControllerArray.Count > 0 ? GroupModelControllerState.Pending : GroupModelControllerState.Empty;
            this.Group = insightBoardGroup;
            foreach (UPInsightBoardItemGroupModelController itemController in this.itemControllerArray)
            {
                itemController.ApplyRecordIdentification(recordIdentification);
            }

            insightBoardGroup.Actions.AddRange(this.BuildAdditionalActionsForRecordIdentification(recordIdentification));
            return insightBoardGroup;
        }

        /// <summary>
        /// Groups the model controller finished.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void GroupModelControllerFinished(UPGroupModelController sender)
        {
            UPInsightBoardItemGroupModelController itemController = (UPInsightBoardItemGroupModelController)sender;
            if (itemController.ControllerState == GroupModelControllerState.Finished)
            {
                UPMInsightBoardItem currentItem = (UPMInsightBoardItem)itemController.Group.Children[0];
                if (this.Group.Children.Count == 0)
                {
                    this.Group.AddChild(currentItem);
                }
                else
                {
                    bool inserted = false;
                    for (int i = 0; i < this.Group.Children.Count; i++)
                    {
                        UPMInsightBoardItem item = (UPMInsightBoardItem)this.Group.Children[i];
                        if (item.SortIndex == currentItem.SortIndex)
                        {
                            inserted = true;
                            break;
                        }
                        else if (item.SortIndex > currentItem.SortIndex)
                        {
                            this.Group.InsertChildAtIndex(currentItem, i);
                            inserted = true;
                            break;
                        }
                    }

                    if (!inserted)
                    {
                        this.Group.AddChild(currentItem);
                    }
                }

                this.pending--;
            }

            if (this.pending == 0)
            {
                bool waitTillFinishLoading = false; // ((NSNumber)NSUserDefaults.StandardUserDefaults().ObjectForKey("Dashboard.WaitTillFinishLoad")).BoolValue;
                var dashboard = this.Delegate as DashboardPageModelController;

                if (waitTillFinishLoading && dashboard != null)
                {
                    var viewController = dashboard.ModelControllerDelegate;
                    // viewController.PerformSelectorOnMainThreadWithObjectWaitUntilDone(@selector(hideSyncIndicator), null, false);
                }

                this.ControllerState = GroupModelControllerState.Finished;
                this.Delegate.GroupModelControllerFinished(this);
            }
        }

        /// <summary>
        /// Performs the organizer action.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="viewReference">The view reference.</param>
        public void PerformOrganizerAction(object sender, ViewReference viewReference)
        {
            string menuName = viewReference.Name;
            if (menuName != null && menuName.StartsWith("Menu:"))
            {
                // UPGoogleAnalytics.TrackMenuSource(menuName, "Dashboard");
            }

            this.Delegate.PerformOrganizerAction(sender, viewReference);
        }

        /// <summary>
        /// Performs the organizer action.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="onlineData">if set to <c>true</c> [online data].</param>
        public void PerformOrganizerAction(object sender, ViewReference viewReference, bool onlineData)
        {
            this.Delegate.PerformOrganizerAction(sender, viewReference, onlineData);
        }

        /// <summary>
        /// Groups the model controller value changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="value">The value.</param>
        public void GroupModelControllerValueChanged(object sender, object value)
        {
            this.Delegate.GroupModelControllerValueChanged(sender, value);
        }

        /// <summary>
        /// The transition to content model controller.
        /// </summary>
        /// <param name="modelController">The model controller.</param>
        public void TransitionToContentModelController(UPOrganizerModelController modelController)
        {
            this.Delegate.TransitionToContentModelController(modelController);
        }

        public void ExchangeContentViewController(UPOrganizerModelController modelController)
        {
            this.Delegate.ExchangeContentViewController(modelController);
        }

        /// <summary>
        /// Forces the redraw.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void ForceRedraw(object sender)
        {
        }

        /// <summary>
        /// Applies the context.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns></returns>
        public override UPMGroup ApplyContext(Dictionary<string, object> dictionary)
        {
            this.pending = this.itemControllerArray.Count;
            bool waitTillFinishLoading = false;// ((NSNumber)NSUserDefaults.StandardUserDefaults().ObjectForKey("Dashboard.WaitTillFinishLoad")).BoolValue;

            var dashboard = this.Delegate as DashboardPageModelController;
            if (waitTillFinishLoading && dashboard != null && ServerSession.CurrentSession.ConnectedToServer
                && !ServerSession.CurrentSession.UserChoiceOffline && this.pending > 0)
            {
                object viewController = dashboard.ModelControllerDelegate;
                // viewController.PerformSelectorOnMainThreadWithObjectWaitUntilDone(@selector(showSyncIndicator), null, false);
            }

            foreach (UPInsightBoardItemGroupModelController itemController in this.itemControllerArray)
            {
                itemController.ApplyContext(dictionary);
            }

            this.ControllerState = this.pending > 0 ? GroupModelControllerState.Pending : GroupModelControllerState.Finished;
            return base.ApplyContext(dictionary);
        }

        /// <summary>
        /// Adds the depending key.
        /// </summary>
        /// <param name="dependingKey">The depending key.</param>
        public override void AddDependingKey(string dependingKey)
        {
            foreach (UPInsightBoardItemGroupModelController itemController in this.itemControllerArray)
            {
                itemController.AddDependingKey(dependingKey);
            }
        }

        /// <summary>
        /// Affecteds the by key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public override bool AffectedByKey(string key)
        {
            return this.itemControllerArray.Any(itemController => itemController.AffectedByKey(key));
        }

        /// <summary>
        /// Groups the model controller value for key.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="key">The key.</param>
        /// <returns>
        /// The <see cref="string" />.
        /// </returns>
        public string GroupModelControllerValueForKey(object sender, string key)
        {
            return string.Empty;
        }
    }
}
