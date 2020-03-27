// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConfigurationUnitStore.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Jakub Majewski
// </author>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Configuration
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Platform;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Configuration storage interface
    /// </summary>
    public interface IConfigurationUnitStore
    {
        /// <summary>
        /// Gets the base directory path.
        /// </summary>
        /// <value>
        /// The base directory path.
        /// </value>
        string BaseDirectoryPath { get; }

        /// <summary>
        /// Gets the database filename.
        /// </summary>
        /// <value>
        /// The database filename.
        /// </value>
        string DatabaseFilename { get; }

        /// <summary>
        /// Gets the database instance.
        /// </summary>
        /// <value>
        /// The database instance.
        /// </value>
        ConfigDatabase DatabaseInstance { get; }

        /// <summary>
        /// Gets the Platform.
        /// </summary>
        /// <value>
        /// The Platform.
        /// </value>
        IPlatformService Platform { get; }

        /// <summary>
        /// Gets the unit count.
        /// </summary>
        /// <value>
        /// The unit count.
        /// </value>
        int UnitCount { get; }

        /// <summary>
        /// Retrieves all the data set names sorted.
        /// </summary>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        List<string> AllDataSetNamesSorted();

        /// <summary>
        /// Retrieves all the file resources.
        /// </summary>
        /// <returns>List of Image configurations</returns>
        List<ConfigUnit> AllFileResources();

        /// <summary>
        /// Retrieves all the file resources per configuration identifier.
        /// </summary>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        List<ConfigUnit> AllFileResourcesPerConfigId();

        /// <summary>
        /// Retrieves all the filter names.
        /// </summary>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        List<string> AllFilterNames();

        /// <summary>
        /// Retrieves analysis an analysis object according to its name
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// Analysis configurations
        /// </returns>
        UPConfigAnalysis AnalysisByName(string unitName);

        /// <summary>
        /// Retrieves analysis an analysis category object according to its name
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// Analysis category configurations
        /// </returns>
        UPConfigAnalysisCategory AnalysisCategoryByName(string unitName);

        /// <summary>
        /// Basics the index of the text by.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string BasicTextByIndex(int index);

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
        string BasicTextByIndexDefaultText(int index, string defaultText);

        /// <summary>
        /// Builds the catalog attribute dictionaries.
        /// </summary>
        void BuildCatalogAttributeDictionaries();

        /// <summary>
        /// Buttons the name of the by.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// Button configurations
        /// </returns>
        UPConfigButton ButtonByName(string unitName);

        /// <summary>
        /// Catalogs the attributes by filter.
        /// </summary>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <returns>
        /// Catalog attributes configurations
        /// </returns>
        UPConfigCatalogAttributes CatalogAttributesByFilter(UPConfigFilter filter);

        /// <summary>
        /// Catalogs the name of the attributes by filter.
        /// </summary>
        /// <param name="filterName">
        /// Name of the filter.
        /// </param>
        /// <returns>
        /// Catalog attributes configurations
        /// </returns>
        UPConfigCatalogAttributes CatalogAttributesByFilterName(string filterName);

        /// <summary>
        /// Catalogs the attributes for fixed catalog.
        /// </summary>
        /// <param name="catNr">
        /// The cat nr.
        /// </param>
        /// <returns>
        /// Catalog attributes configurations
        /// </returns>
        UPConfigCatalogAttributes CatalogAttributesForFixedCatalog(int catNr);

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
        UPConfigCatalogAttributes CatalogAttributesForInfoAreaIdFieldId(string infoAreaId, int fieldId);

        /// <summary>
        /// Catalogs the attributes for variable catalog.
        /// </summary>
        /// <param name="catNr">
        /// The cat nr.
        /// </param>
        /// <returns>
        /// Catalog attributes configurations
        /// </returns>
        UPConfigCatalogAttributes CatalogAttributesForVariableCatalog(int catNr);

        /// <summary>
        /// Configurations the value.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string ConfigValue(string unitName);

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
        string ConfigValueDefaultValue(string unitName, string defaultValue);

        /// <summary>
        /// Configurations the value is set.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool ConfigValueIsSet(string unitName);

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
        bool ConfigValueIsSetDefaultValue(string unitName, bool defaultValue);

        /// <summary>
        /// Returns the DatSet configurations with given  the name.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigDataSet"/>.
        /// </returns>
        UPConfigDataSet DataSetByName(string unitName);

        /// <summary>
        /// Defaults the menu for information area identifier.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <returns>
        /// Menu configurations
        /// </returns>
        Menu DefaultMenuForInfoAreaId(string infoAreaId);

        /// <summary>
        /// Deletes the database.
        /// </summary>
        /// <param name="recreate">
        /// if set to <c>true</c> [recreate].
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool DeleteDatabase(bool recreate);

        /// <summary>
        /// Expands the name of the by.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// Expand configurations
        /// </returns>
        UPConfigExpand ExpandByName(string unitName);

        /// <summary>
        /// Fields the name of the control by.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// The <see cref="FieldControl"/>.
        /// </returns>
        FieldControl FieldControlByName(string unitName);

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
        FieldControl FieldControlByNameFromGroup(string controlName, string groupName);

        /// <summary>
        /// The file name for resource name.
        /// </summary>
        /// <param name="resourceName">
        /// The resource name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string FileNameForResourceName(string resourceName);

        /// <summary>
        /// Filters the name of the by.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// Filter configurations
        /// </returns>
        UPConfigFilter FilterByName(string unitName);

        /// <summary>
        /// Forms the name of the by.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// form configurations
        /// </returns>
        Form FormByName(string unitName);

        /// <summary>
        /// Headers the name of the by.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// Header configurations
        /// </returns>
        UPConfigHeader HeaderByName(string unitName);

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
        UPConfigHeader HeaderByNameFromGroup(string headerName, string groupName);

        /// <summary>
        /// Informations the area configuration by identifier.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <returns>
        /// The <see cref="InfoArea"/>.
        /// </returns>
        InfoArea InfoAreaConfigById(string infoAreaId);

        /// <summary>
        /// Menus the name of the by.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// The <see cref="Menu"/>.
        /// </returns>
        Menu MenuByName(string unitName);

        /// <summary>
        /// Queries the name of the by.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// Query configurations
        /// </returns>
        UPConfigQuery QueryByName(string unitName);

        /// <summary>
        /// Quicks the name of the search by.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// Quick search configurations
        /// </returns>
        QuickSearch QuickSearchByName(string unitName);

        /// <summary>
        /// Renames the database to default.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool RenameDatabaseToDefault();

        /// <summary>
        /// Resets this instance.
        /// </summary>
        void Reset();

        /// <summary>
        /// Resources the name of the by.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// Resource configurations
        /// </returns>
        UPConfigResource ResourceByName(string unitName);

        /// <summary>
        /// Searches the name of the and list by.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// The <see cref="SearchAndList"/>.
        /// </returns>
        SearchAndList SearchAndListByName(string unitName);

        /// <summary>
        /// Stores the type of the with.
        /// </summary>
        /// <param name="unitType">
        /// Type of the unit.
        /// </param>
        /// <returns>
        /// The <see cref="UnitTypeStore"/>.
        /// </returns>
        IUnitTypeStore StoreWithType(string unitType);

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
        int SyncElementsOfUnitTypeEmptyTable(JArray elements, string unitTypeName, bool emptyTable);

        /// <summary>
        /// Tables the name of the caption by.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// Table caption configuratons
        /// </returns>
        UPConfigTableCaption TableCaptionByName(string unitName);

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
        string TextByGroupIndexDefaultText(string textGroupName, int index, string defaultText);

        /// <summary>
        /// Texts from field control function identifier.
        /// </summary>
        /// <param name="fieldControlFunctionIdentifier">
        /// The field control function identifier.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string TextFromFieldControlFunctionIdentifier(string fieldControlFunctionIdentifier);

        /// <summary>
        /// Textgroups the name of the by.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// The <see cref="TextGroup"/>.
        /// </returns>
        TextGroup TextgroupByName(string unitName);

        /// <summary>
        /// Timelines the name of the by.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// Timeline configurations
        /// </returns>
        ConfigTimeline TimelineByName(string unitName);

        /// <summary>
        /// TreeViews the name of the by.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// Tree view configurations
        /// </returns>
        UPConfigTreeView TreeViewByName(string unitName);

        /// <summary>
        /// Updates the DDL.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        int UpdateDDL();

        /// <summary>
        /// Updates the web configuration values.
        /// </summary>
        /// <param name="webConfigValueDictionary">
        /// The web configuration value dictionary.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool UpdateWebConfigValues(Dictionary<string, object> webConfigValueDictionary);

        /// <summary>
        /// Webs the name of the configuration layout by.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// The <see cref="WebConfigLayout"/>.
        /// </returns>
        WebConfigLayout WebConfigLayoutByName(string unitName);

        /// <summary>
        /// Webs the name of the configuration value by.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// The <see cref="WebConfigValue"/>.
        /// </returns>
        WebConfigValue WebConfigValueByName(string unitName);
    }
}