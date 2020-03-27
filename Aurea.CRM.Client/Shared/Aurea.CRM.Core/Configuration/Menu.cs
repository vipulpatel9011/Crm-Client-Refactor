// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Menu.cs" company="Aurea Software Gmbh">
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
// <summary>
//   defines constants related to Menu
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Configuration
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// defines constants related to Menu
    /// </summary>
    public partial class Constants
    {
        /// <summary>
        /// The main menu name.
        /// </summary>
        public const string MainMenuName = "SMARTBOOK";
    }

    /// <summary>
    /// defines the menu configurations
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Configuration.ConfigUnit" />
    public class Menu : ConfigUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Menu"/> class.
        /// </summary>
        /// <param name="defArray">
        /// The definition array.
        /// </param>
        public Menu(List<object> defArray)
        {
            if (defArray == null || defArray.Count < 5)
            {
                return;
            }

            var name = (string)defArray[0];
            this.SetLabelWithText(name, (string)defArray[1]);
            var viewReferenceDef = (defArray[2] as JArray)?.ToObject<List<object>>();
            this.ViewReference = viewReferenceDef != null ? new ViewReference(viewReferenceDef, $"Menu:{name}") : null;

            this.Items = (defArray[3] as JArray)?.ToObject<List<object>>();

            this.ImageName = defArray[4] as string;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Menu"/> class.
        /// </summary>
        protected Menu()
        {
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Gets the name of the image.
        /// </summary>
        /// <value>
        /// The name of the image.
        /// </value>
        public string ImageName { get; private set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public List<object> Items { get; protected set; }

        /// <summary>
        /// Gets the view reference.
        /// </summary>
        /// <value>
        /// The view reference.
        /// </value>
        public ViewReference ViewReference { get; private set; }

        /// <summary>
        /// Mains the menu.
        /// </summary>
        /// <returns>
        /// The <see cref="Menu"/>.
        /// </returns>
        public static Menu MainMenu()
        {
            var mainMenu = ConfigurationUnitStore.DefaultStore?.MenuByName(Constants.MainMenuName);
            if (mainMenu != null && mainMenu.Items.Count > 0)
            {
                return mainMenu;
            }

            var syncDef = new List<string> { "SYNCHRONIZATION", "Synchronization", null, null, "Menu:Synchronization" };
            var logoutDef = new List<string> { "LOGOUT", "Logout", null, null, "Menu:Logout" };
            var mainMenuDef = new List<object>
                                  {
                                      "SMARTBOOK",
                                      "CRMpad",
                                      null,
                                      new List<object> { syncDef, logoutDef },
                                      null
                                  };
            return new ConfigMenuDefault(mainMenuDef);
        }

        /// <summary>
        /// Adds the sub menu.
        /// </summary>
        /// <param name="menuitem">
        /// The menuitem.
        /// </param>
        public void AddSubMenu(Menu menuitem)
        {
            if (this.Items == null)
            {
                this.Items = new List<object>();
            }

            this.Items.Add(menuitem);
        }

        /// <summary>
        /// Names the index of for sub menu at.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public virtual string NameForSubMenuAtIndex(int index)
        {
            if (this.Items != null && index >= 0 && index < this.Items.Count)
            {
                return (string)this.Items[index];
            }

            return null;
        }

        /// <summary>
        /// Numbers the of sub menus.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int NumberOfSubMenus => this.Items?.Count ?? 0;

        /// <summary>
        /// Sets the label with text.
        /// </summary>
        /// <param name="label">
        /// The label.
        /// </param>
        /// <param name="displayText">
        /// The display text.
        /// </param>
        public void SetLabelWithText(string label, string displayText)
        {
            this.UnitName = label;
            this.DisplayName = displayText;
        }

        /// <summary>
        /// Subs the index of the menu at.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="Menu"/>.
        /// </returns>
        public virtual Menu SubMenuAtIndex(int index)
        {
            var subItemName = this.NameForSubMenuAtIndex(index);
            return ConfigurationUnitStore.DefaultStore.MenuByName(subItemName);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"ConfigMenu={this.DisplayName}{Environment.NewLine}  viewReference={this.ViewReference}";
        }
    }

    /// <summary>
    /// Defines the configuration menu configurations
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Configuration.Menu" />
    public class ConfigMenuDefault : Menu
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigMenuDefault"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        public ConfigMenuDefault(object definition)
        {
            List<object> subDefinitions = definition as List<object>;
            if (subDefinitions != null)
            {
                var newItems = new List<object>(subDefinitions.Count);
                for (var i = 0; i < subDefinitions.Count; i++)
                {
                    var subItemDef = subDefinitions[i];
                    var subItem = new ConfigMenuDefault(subItemDef);
                    newItems.Add(subItem);
                }
                if (this.Items != null)
                {
                    this.Items = newItems;
                }
            }
        }

        /// <summary>
        /// Names the index of for sub menu at.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string NameForSubMenuAtIndex(int index)
        {
            return this.SubMenuAtIndex(index).UnitName;
        }

        /// <summary>
        /// Subs the index of the menu at.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="ConfigUnit"/>.
        /// </returns>
        public override Menu SubMenuAtIndex(int index)
        {
            return this.Items[index] as Menu;
        }
    }
}
