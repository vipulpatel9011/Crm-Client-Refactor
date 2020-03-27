// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationUnitStore.cs" company="Aurea Software Gmbh">
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
//   Configuration unit store related constants
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.Platform;
    using Aurea.CRM.Core.Session;

    //using Microsoft.Practices.ServiceLocation;
    using Newtonsoft.Json.Linq;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Ioc;
    using GalaSoft.MvvmLight.Views;

    /// <summary>
    /// Configuration unit store related constants
    /// </summary>
    public partial class Constants
    {
        /// <summary>
        /// The configuni t_ v 1 postfix.
        /// </summary>
        public const string CONFIGUNITV1POSTFIX = "[V1]";

        /// <summary>
        /// The preloa d_ fieldcontrols.
        /// </summary>
        public const bool PRELOADFIELDCONTROLS = false;

        /// <summary>
        /// The preloa d_ menus.
        /// </summary>
        public const bool PRELOADMENUS = true;

        /// <summary>
        /// The preloa d_ webconfiglayouts.
        /// </summary>
        public const bool PRELOADWEBCONFIGLAYOUTS = false;

        /// <summary>
        /// The preloa d_ webconfigvalues.
        /// </summary>
        public const bool PRELOADWEBCONFIGVALUES = true;
    }

    /// <summary>
    /// Implements the configuration unit store
    /// </summary>
    public class ConfigurationUnitStore : IConfigurationUnitStore
    {
        // static NSDictionary legacyWebConfigDict;

        private bool catAttributeDictionariesBuilt;
        private Dictionary<int, UPConfigCatalogAttributes> fixedCatalogValueAttributes;
        private Dictionary<string, IUnitTypeStore> unitStoreDictionary;
        private Dictionary<int, UPConfigCatalogAttributes> variableCatalogValueAttributes;

        /// <summary>
        /// The default database name
        /// </summary>
        public const string DefaultDatabaseName = "configData";

        /// <summary>
        /// The environment key.
        /// </summary>
        protected string environmentKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationUnitStore"/> class.
        /// </summary>
        /// <param name="baseDirectoryPath">
        /// The base directory path.
        /// </param>
        public ConfigurationUnitStore(string baseDirectoryPath)
            : this(baseDirectoryPath, DefaultDatabaseName, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationUnitStore"/> class.
        /// </summary>
        /// <param name="baseDirectoryPath">
        /// The base directory path.
        /// </param>
        /// <param name="fileName">
        /// Name of the file.
        /// </param>
        /// <param name="recreate">
        /// if set to <c>true</c> [recreate].
        /// </param>
        public ConfigurationUnitStore(string baseDirectoryPath, string fileName, bool recreate)
        {
         // this.Platform = SimpleIoc.Default.GetInstance<IPlatformService>();

            this.Platform = SimpleIoc.Default.GetInstance<IPlatformService>();
            this.BaseDirectoryPath = baseDirectoryPath;
            this.unitStoreDictionary = new Dictionary<string, IUnitTypeStore>();
            this.DatabaseFilename = Path.Combine(baseDirectoryPath, fileName);

            this.DatabaseInstance = ConfigDatabase.Create(this.DatabaseFilename);

         // Changed WebConfigLayout, Query and Analysis preload to true. For an unknown reason, they don't work correctly after initialization.
            this.AddUnitStore("Menu", "menus", true, def => new Menu(def));
            this.AddUnitStore("FieldControl", "fieldcontrols", true, def => new FieldControl(def));
            this.AddUnitStore("WebConfigLayout", "webconfiglayouts", true, def => new WebConfigLayout(def));
            this.AddUnitStore("WebConfigValue", "webconfigvalues", true, def => new WebConfigValue(def));
            this.AddUnitStore("Form", "forms", true, def => new Form(def));
            this.AddUnitStore("QuickSearch", "quicksearch", true, def => new QuickSearch(def));
            this.AddUnitStore("Query", "queries", true, def => new UPConfigQuery(def));
            this.AddUnitStore("Analysis", "analyses", true, def => new UPConfigAnalysis(def));
            this.AddUnitStore("AnalysisCategory", "analysiscategories", false, def => new UPConfigAnalysisCategory(def));
            this.AddUnitStore("Details", "details", true, def => new UPConfigExpand(def));
            this.AddUnitStore("Search", "searches", true, def => new SearchAndList(def));
            this.AddUnitStore("Button", "buttons", true, def => new UPConfigButton(def));
            this.AddUnitStore("Header", "header", true, def => new UPConfigHeader(def));
            this.AddUnitStore("Image", "images", true, def => new UPConfigResource(def));
            this.AddUnitStore("Filter", "filter", true, def => new UPConfigFilter(def));
            this.AddUnitStore("InfoAreas", "infoareas", true, def => new InfoArea(def));
            this.AddUnitStore("Textgroups", "textgroups", true, def => new TextGroup(def));
            this.AddUnitStore("DataSets", "datasets", true, def => new UPConfigDataSet(def));
            this.AddUnitStore("TableCaptions", "tablecaptions", true, def => new UPConfigTableCaption(def));
            this.AddUnitStore("Timeline", "timeline", true, def => new ConfigTimeline(def));
            this.AddUnitStore("TreeView", "treeview", true, def => new UPConfigTreeView(def));

            if (recreate && this.UpdateDDL() != 0)
            {
                return;
            }

            this.Reset();
        }

        /// <summary>
        /// Gets the default store.
        /// </summary>
        /// <value>
        /// The default store.
        /// </value>
        public static IConfigurationUnitStore DefaultStore => ServerSession.CurrentSession?.ConfigUnitStore;

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public static ILogger Logger => SimpleIoc.Default.GetInstance<ILogger>();

        /// <summary>
        /// Gets the base directory path.
        /// </summary>
        /// <value>
        /// The base directory path.
        /// </value>
        public string BaseDirectoryPath { get; }

        /// <summary>
        /// Gets the database filename.
        /// </summary>
        /// <value>
        /// The database filename.
        /// </value>
        public string DatabaseFilename { get; }

        /// <summary>
        /// Gets the database instance.
        /// </summary>
        /// <value>
        /// The database instance.
        /// </value>
        public ConfigDatabase DatabaseInstance { get; private set; }

        /// <summary>
        /// Gets the Platform.
        /// </summary>
        /// <value>
        /// The Platform.
        /// </value>
        public IPlatformService Platform { get; }

        /// <summary>
        /// Gets the unit count.
        /// </summary>
        /// <value>
        /// The unit count.
        /// </value>
        public int UnitCount => this.unitStoreDictionary?.Count ?? 0;

        /// <summary>
        /// Alls the data set names sorted.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<string> AllDataSetNamesSorted()
        {
            return this.StoreWithType("DataSets").AllUnitNamesSorted();
        }

        /// <summary>
        /// Alls the file resources.
        /// </summary>
        /// <returns>List of Image configurations</returns>
        public List<ConfigUnit> AllFileResources()
        {
            return this.StoreWithType("Image").AllUnits();
        }

        /// <summary>
        /// Alls the file resources per configuration identifier.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<ConfigUnit> AllFileResourcesPerConfigId()
        {
            var allResources = this.AllFileResources();
            var retArray = new List<ConfigUnit>();
            var maxZipId = -1;
            foreach (UPConfigResource tempRes in allResources.Cast<UPConfigResource>())
            {
                if (tempRes?.UnitName == null || !tempRes.UnitName.StartsWith("ZIP:"))
                {
                    continue;
                }

                retArray.Add(tempRes);
                if (tempRes.ConfigId > maxZipId)
                {
                    maxZipId = tempRes.ConfigId;
                }
            }

            if (maxZipId == -1)
            {
                var allFileResources = new List<ConfigUnit>();

                // Skiping the font icons from the all filere sources, because they are not physical files
                foreach (var fileRes in allResources.Cast<UPConfigResource>().Where(x => !x.FileName.IsFontIcon()))
                {
                    allFileResources.Add(fileRes);
                }

                return allFileResources;
            }

            foreach (UPConfigResource tempRes in allResources.Cast<UPConfigResource>())
            {
                if (tempRes.ConfigId > maxZipId)
                {
                    retArray.Add(tempRes);
                }
            }

            return retArray;
        }

        /// <summary>
        /// Alls the filter names.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<string> AllFilterNames()
        {
            return this.StoreWithType("Filter").AllUnitNames();
        }

        /// <summary>
        /// Analysises the name of the by.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// Analysis configurations
        /// </returns>
        public UPConfigAnalysis AnalysisByName(string unitName)
        {
            return (UPConfigAnalysis)this.StoreWithType("Analysis").UnitWithName(unitName);
        }

        /// <summary>
        /// Analysises the name of the category by.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// Analysis category configurations
        /// </returns>
        public UPConfigAnalysisCategory AnalysisCategoryByName(string unitName)
        {
            return (UPConfigAnalysisCategory)this.StoreWithType("AnalysisCategory").UnitWithName(unitName);
        }

        /// <summary>
        /// Basics the index of the text by.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string BasicTextByIndex(int index)
        {
            return this.BasicTextByIndexDefaultText(index, null);
        }

        /// <summary>
        /// Basics the text by index default text.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="defaultText">
        /// The default text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string BasicTextByIndexDefaultText(int index, string defaultText)
        {
            return this.TextByGroupIndexDefaultText("basic", index, defaultText);
        }

        /// <summary>
        /// Builds the catalog attribute dictionaries.
        /// </summary>
        public void BuildCatalogAttributeDictionaries()
        {
            var theFixed = new Dictionary<int, UPConfigCatalogAttributes>();
            var variable = new Dictionary<int, UPConfigCatalogAttributes>();
            foreach (var filterName in this.AllFilterNames())
            {
                var range = (filterName ?? string.Empty).IndexOf("CATALOG", StringComparison.Ordinal);
                if (range < 0)
                {
                    continue;
                }

                var catalogAttributes = this.CatalogAttributesByFilterName(filterName);
                if (catalogAttributes == null)
                {
                    continue;
                }

                var dict = catalogAttributes.FixedCatalog ? theFixed : variable;
                dict.SetObjectForKey(catalogAttributes, catalogAttributes.CatalogNumber);
            }

            this.fixedCatalogValueAttributes = theFixed;
            this.variableCatalogValueAttributes = variable;
            this.catAttributeDictionariesBuilt = true;
        }

        /* Buttons */

        /// <summary>
        /// Buttons the name of the by.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// Button configurations
        /// </returns>
        public UPConfigButton ButtonByName(string unitName)
        {
            return (UPConfigButton)this.StoreWithType("Button").UnitWithName(unitName);
        }

        /// <summary>
        /// Catalogs the attributes by filter.
        /// </summary>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <returns>
        /// Catalog attributes configurations
        /// </returns>
        public UPConfigCatalogAttributes CatalogAttributesByFilter(UPConfigFilter filter)
        {
            return filter == null ? null : new UPConfigCatalogAttributes(filter);
        }

        /// <summary>
        /// Catalogs the name of the attributes by filter.
        /// </summary>
        /// <param name="filterName">
        /// Name of the filter.
        /// </param>
        /// <returns>
        /// Catalog attributes configurations
        /// </returns>
        public UPConfigCatalogAttributes CatalogAttributesByFilterName(string filterName)
        {
            var filter = this.FilterByName(filterName);
            return this.CatalogAttributesByFilter(filter);
        }

        /// <summary>
        /// Catalogs the attributes for fixed catalog.
        /// </summary>
        /// <param name="catNr">
        /// The cat nr.
        /// </param>
        /// <returns>
        /// Catalog attributes configurations
        /// </returns>
        public UPConfigCatalogAttributes CatalogAttributesForFixedCatalog(int catNr)
        {
            if (!this.catAttributeDictionariesBuilt)
            {
                this.BuildCatalogAttributeDictionaries();
            }

            return this.fixedCatalogValueAttributes.ValueOrDefault(catNr);
        }

        /// <summary>
        /// Catalogs the attributes for information area identifier field identifier.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <returns>
        /// Catalog attributes configurations
        /// </returns>
        public UPConfigCatalogAttributes CatalogAttributesForInfoAreaIdFieldId(string infoAreaId, int fieldId)
        {
            var fieldInfo = UPCRMDataStore.DefaultStore.FieldInfoForInfoAreaFieldId(infoAreaId, fieldId);
            if (fieldInfo.FieldType == "K")
            {
                return this.CatalogAttributesForVariableCatalog(fieldInfo.CatNo);
            }

            return fieldInfo.FieldType == "X" ? this.CatalogAttributesForFixedCatalog(fieldInfo.CatNo) : null;
        }

        /// <summary>
        /// Catalogs the attributes for variable catalog.
        /// </summary>
        /// <param name="catNr">
        /// The cat nr.
        /// </param>
        /// <returns>
        /// Catalog attributes configurations
        /// </returns>
        public UPConfigCatalogAttributes CatalogAttributesForVariableCatalog(int catNr)
        {
            if (!this.catAttributeDictionariesBuilt)
            {
                this.BuildCatalogAttributeDictionaries();
            }

            return this.variableCatalogValueAttributes.ValueOrDefault(catNr);
        }

        /// <summary>
        /// Configurations the value.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ConfigValue(string unitName)
        {
            var value = this.WebConfigValueByName(unitName);
            return value == null ? string.Empty : value.Value;
        }

        /// <summary>
        /// Configurations the value default value.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ConfigValueDefaultValue(string unitName, string defaultValue)
        {
            var value = this.WebConfigValueByName(unitName);
            return string.IsNullOrWhiteSpace(value?.Value) ? defaultValue : value.Value;
        }

        /// <summary>
        /// Configurations the value is set.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ConfigValueIsSet(string unitName)
        {
            return this.ConfigValueIsSetDefaultValue(unitName, false);
        }

        /// <summary>
        /// Configurations the value is set default value.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <param name="defaultValue">
        /// if set to <c>true</c> [default value].
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ConfigValueIsSetDefaultValue(string unitName, bool defaultValue)
        {
            var value = this.WebConfigValueByName(unitName);
            if (value?.Value == null)
            {
                return defaultValue;
            }

            if (defaultValue)
            {
                // Eigentlich verkehrt herum. Bei default yes sollte im Falle von nil als auch bei "????" YES zurÃ¼ckgegeben werden.
                // Wird so seit 11.2011 verwendet, daher keine Anpassung!
                // default YES --> value must be true
                return value.Value == "1" || value.Value.Equals("true", StringComparison.OrdinalIgnoreCase);
            }

            // default NO --> value must be false
            return !(value.Value == "0" || value.Value.Equals("false", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Returns the DatSet configurations with given  the name.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigDataSet"/>.
        /// </returns>
        public UPConfigDataSet DataSetByName(string unitName)
        {
            return this.StoreWithType("DataSets").UnitWithName(unitName) as UPConfigDataSet;
        }

        /// <summary>
        /// Defaults the menu for information area identifier.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <returns>
        /// Menu configurations
        /// </returns>
        public Menu DefaultMenuForInfoAreaId(string infoAreaId)
        {
            var configInfoArea = this.InfoAreaConfigById(infoAreaId);
            Menu configMenu = null;
            if (!string.IsNullOrEmpty(configInfoArea?.DefaultAction))
            {
                configMenu = this.MenuByName(configInfoArea.DefaultAction);
            }

            return configMenu ?? this.MenuByName("SHOWRECORD");
        }

        /// <summary>
        /// Deletes the database.
        /// </summary>
        /// <param name="recreate">
        /// if set to <c>true</c> [recreate].
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool DeleteDatabase(bool recreate)
        {
            var fileManager = this.Platform.StorageProvider;
            if (!fileManager.FileExists(this.DatabaseFilename))
            {
                return false;
            }

            this.DatabaseInstance.Close();

            //this.DatabaseInstance?.Dispose();

            Exception ex;
            fileManager.TryDelete(this.DatabaseFilename, out ex);

            if (recreate)
            {
                this.DatabaseInstance = ConfigDatabase.Create(this.DatabaseFilename);
            }

            return true;
        }

        /* Details */

        /// <summary>
        /// Expands the name of the by.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// Expand configurations
        /// </returns>
        public UPConfigExpand ExpandByName(string unitName)
        {
            return string.IsNullOrEmpty(unitName)
                       ? null
                       : (UPConfigExpand)this.StoreWithType("Details").UnitWithName(unitName);
        }

        /// <summary>
        /// Fields the name of the control by.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// The <see cref="FieldControl"/>.
        /// </returns>
        public FieldControl FieldControlByName(string unitName)
        {
            return (FieldControl)this.StoreWithType("FieldControl").UnitWithName(unitName);
        }

        /// <summary>
        /// Fields the control by name from group.
        /// </summary>
        /// <param name="controlName">
        /// Name of the control.
        /// </param>
        /// <param name="groupName">
        /// Name of the group.
        /// </param>
        /// <returns>
        /// The <see cref="FieldControl"/>.
        /// </returns>
        public FieldControl FieldControlByNameFromGroup(string controlName, string groupName)
        {
            return this.FieldControlByName($"{groupName}.{controlName}");
        }

        /// <summary>
        /// The file name for resource name.
        /// </summary>
        /// <param name="resourceName">
        /// The resource name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string FileNameForResourceName(string resourceName)
        {
            return ServerSession.CurrentSession.FileStore.ImagePathForName(resourceName);
        }

        /// <summary>
        /// Filters the name of the by.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// Filter configurations
        /// </returns>
        public UPConfigFilter FilterByName(string unitName)
        {
            return (UPConfigFilter)this.StoreWithType("Filter").UnitWithName(unitName);
        }

        /* Form */

        /// <summary>
        /// Forms the name of the by.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// form configurations
        /// </returns>
        public Form FormByName(string unitName)
        {
            return (Form)this.StoreWithType("Form").UnitWithName(unitName);
        }

        /* Header */

        /// <summary>
        /// Headers the name of the by.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// Header configurations
        /// </returns>
        public UPConfigHeader HeaderByName(string unitName)
        {
            return (UPConfigHeader)this.StoreWithType("Header").UnitWithName(unitName);
        }

        /// <summary>
        /// Headers the by name from group.
        /// </summary>
        /// <param name="headerName">
        /// Name of the header.
        /// </param>
        /// <param name="groupName">
        /// Name of the group.
        /// </param>
        /// <returns>
        /// Header configurations
        /// </returns>
        public UPConfigHeader HeaderByNameFromGroup(string headerName, string groupName)
        {
            return this.HeaderByName($"{groupName}.{headerName}");
        }

        /// <summary>
        /// Informations the area configuration by identifier.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <returns>
        /// The <see cref="InfoArea"/>.
        /// </returns>
        public InfoArea InfoAreaConfigById(string infoAreaId)
        {
            return (InfoArea)this.StoreWithType("InfoAreas").UnitWithName(infoAreaId);
        }

        /// <summary>
        /// Menus the name of the by.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// The <see cref="Menu"/>.
        /// </returns>
        public Menu MenuByName(string unitName)
        {
            return (Menu)this.StoreWithType("Menu").UnitWithName(unitName);
        }

        /// <summary>
        /// Queries the name of the by.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// Query configurations
        /// </returns>
        public UPConfigQuery QueryByName(string unitName)
        {
            return (UPConfigQuery)this.StoreWithType("Query").UnitWithName(unitName);
        }

        /// <summary>
        /// Quicks the name of the search by.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// Quick search configurations
        /// </returns>
        public QuickSearch QuickSearchByName(string unitName)
        {
            return (QuickSearch)this.StoreWithType("QuickSearch").UnitWithName(unitName);
        }

        /// <summary>
        /// Renames the database to default.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool RenameDatabaseToDefault()
        {
            this.DatabaseInstance = null;

            var fileManager = this.Platform.StorageProvider;
            var defaultDatabaseFilename = Path.Combine(this.BaseDirectoryPath, DefaultDatabaseName);
            if (defaultDatabaseFilename.Equals(this.DatabaseFilename))
            {
                return true;
            }

            Exception error;
            return fileManager.TryMove(this.DatabaseFilename, defaultDatabaseFilename, out error);
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            this.catAttributeDictionariesBuilt = false;
            this.fixedCatalogValueAttributes = null;
            this.variableCatalogValueAttributes = null;

            foreach (var store in this.unitStoreDictionary.Values)
            {
                store.Reset();
            }
        }

        /* Image */

        /// <summary>
        /// Resources the name of the by.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// Resource configurations
        /// </returns>
        public UPConfigResource ResourceByName(string unitName)
        {
            if (string.IsNullOrWhiteSpace(unitName))
            {
                return null;
            }

            return (UPConfigResource)this.StoreWithType("Image").UnitWithName(unitName);
        }

        /// <summary>
        /// Searches the name of the and list by.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// The <see cref="SearchAndList"/>.
        /// </returns>
        public SearchAndList SearchAndListByName(string unitName)
        {
            return (SearchAndList)this.StoreWithType("Search").UnitWithName(unitName);
        }

        /// <summary>
        /// Stores the type of the with.
        /// </summary>
        /// <param name="unitType">
        /// Type of the unit.
        /// </param>
        /// <returns>
        /// The <see cref="UnitTypeStore"/>.
        /// </returns>
        public IUnitTypeStore StoreWithType(string unitType)
        {
            return this.unitStoreDictionary.ValueOrDefault(unitType);
        }

        /// <summary>
        /// Synchronizes the elements of unit type empty table.
        /// </summary>
        /// <param name="elements">
        /// The elements.
        /// </param>
        /// <param name="unitTypeName">
        /// Name of the unit type.
        /// </param>
        /// <param name="emptyTable">
        /// if set to <c>true</c> [empty table].
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int SyncElementsOfUnitTypeEmptyTable(JArray elements, string unitTypeName, bool emptyTable)
        {
            var store = this.StoreWithType(unitTypeName);
            return store?.SyncElements(elements, emptyTable) ?? 0;
        }

        /// <summary>
        /// Tables the name of the caption by.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// Table caption configuratons
        /// </returns>
        public UPConfigTableCaption TableCaptionByName(string unitName)
        {
            return (UPConfigTableCaption)this.StoreWithType("TableCaptions").UnitWithName(unitName);
        }

        /// <summary>
        /// Texts the by group index default text.
        /// </summary>
        /// <param name="textGroupName">
        /// Name of the text group.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="defaultText">
        /// The default text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string TextByGroupIndexDefaultText(string textGroupName, int index, string defaultText)
        {
            var textgroup = this.TextgroupByName(textGroupName);
            return textgroup != null ? textgroup.TextAtIndexDefaultText(index, defaultText) : defaultText;
        }

        /// <summary>
        /// Texts from field control function identifier.
        /// </summary>
        /// <param name="fieldControlFunctionIdentifier">
        /// The field control function identifier.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string TextFromFieldControlFunctionIdentifier(string fieldControlFunctionIdentifier)
        {
            var parts = fieldControlFunctionIdentifier.Split('.');
            if (parts.Length < 2)
            {
                return null;
            }

            var fieldControl = this.FieldControlByNameFromGroup("List", parts[0]);
            return fieldControl.LabelTextForFunctionName(parts[1]);
        }

        /// <summary>
        /// Textgroups the name of the by.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// The <see cref="TextGroup"/>.
        /// </returns>
        public TextGroup TextgroupByName(string unitName)
        {
            return (TextGroup)this.StoreWithType("Textgroups").UnitWithName(unitName);
        }

        /// <summary>
        /// Timelines the name of the by.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// Timeline configurations
        /// </returns>
        public ConfigTimeline TimelineByName(string unitName)
        {
            return (ConfigTimeline)this.StoreWithType("Timeline").UnitWithName(unitName);
        }

        /// <summary>
        /// TreeViews the name of the by.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// Tree view configurations
        /// </returns>
        public UPConfigTreeView TreeViewByName(string unitName)
        {
            return (UPConfigTreeView)this.StoreWithType("TreeView").UnitWithName(unitName);
        }

        /// <summary>
        /// Updates the DDL.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int UpdateDDL()
        {
            var database = this.DatabaseInstance;
            database.EnsureDDL();

            foreach (var store in this.unitStoreDictionary.Values)
            {
                if (!database.EnsureTableDDL(store.TableName))
                {
                    return 1;
                }
            }

            return 0;
        }

        /// <summary>
        /// Updates the web configuration values.
        /// </summary>
        /// <param name="webConfigValueDictionary">
        /// The web configuration value dictionary.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool UpdateWebConfigValues(Dictionary<string, object> webConfigValueDictionary)
        {
            var webConfigParameters = new List<List<object>>(webConfigValueDictionary.Count);
            foreach (var webConfigValueName in webConfigValueDictionary.Keys)
            {
                webConfigParameters.Add(
                    new List<object> { webConfigValueName, webConfigValueDictionary[webConfigValueName], 0 });
            }

            var unitTypeStore = this.StoreWithType("WebConfigValue");
            var ret = 0;

            // ret = unitTypeStore.SyncElements(webConfigParameters as JArray, false);
            unitTypeStore.Reset();

            ServerSession.CurrentSession.LoadApplicationSettings();
            return ret >= 0;
        }

        /// <summary>
        /// Webs the name of the configuration layout by.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// The <see cref="WebConfigLayout"/>.
        /// </returns>
        public WebConfigLayout WebConfigLayoutByName(string unitName)
        {
            return (WebConfigLayout)this.StoreWithType("WebConfigLayout").UnitWithName(unitName);
        }

        /// <summary>
        /// Webs the name of the configuration value by.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// The <see cref="WebConfigValue"/>.
        /// </returns>
        public WebConfigValue WebConfigValueByName(string unitName)
        {
            var store = this.StoreWithType("WebConfigValue");
            var val = store?.UnitWithName(unitName);
            return val as WebConfigValue;
        }

        /// <summary>
        /// Adds the unit store.
        /// </summary>
        /// <typeparam name="TDef">
        /// The type of the definition.
        /// </typeparam>
        /// <param name="unitType">
        /// Type of the unit.
        /// </param>
        /// <param name="tableName">
        /// Name of the table.
        /// </param>
        /// <param name="preLoad">
        /// if set to <c>true</c> [pre load].
        /// </param>
        /// <param name="factory">
        /// The factory.
        /// </param>
        private void AddUnitStore<TDef>(string unitType, string tableName, bool preLoad, Func<TDef, ConfigUnit> factory)
            where TDef : class
        {
            Func<object, object> creator = b => factory(b as TDef);

            this.unitStoreDictionary[unitType] = new UnitTypeStore(unitType, tableName, preLoad, creator, this);
        }

        /// <summary>
        /// Adds the unit store.
        /// </summary>
        /// <param name="unitType">
        /// Type of the unit.
        /// </param>
        /// <param name="tableName">
        /// Name of the table.
        /// </param>
        /// <param name="preLoad">
        /// if set to <c>true</c> [pre load].
        /// </param>
        /// <param name="factory">
        /// The factory.
        /// </param>
        private void AddUnitStore(
            string unitType,
            string tableName,
            bool preLoad,
            Func<List<object>, ConfigUnit> factory)
        {
            this.AddUnitStore<List<object>>(unitType, tableName, preLoad, factory);
        }
    }
}
