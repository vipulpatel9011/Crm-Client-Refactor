// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigAnalysis.cs" company="Aurea Software Gmbh">
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
//   Analysis field flags
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Aurea.CRM.Core.Extensions;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Analysis field flags
    /// </summary>
    [Flags]
    public enum ConfigAnalysisFieldFlag
    {
        /// <summary>
        /// The category.
        /// </summary>
        Category = 1,

        /// <summary>
        /// The result column.
        /// </summary>
        ResultColumn = 2,

        /// <summary>
        /// The default category.
        /// </summary>
        DefaultCategory = 4,

        /// <summary>
        /// The date filter.
        /// </summary>
        DateFilter = 16,

        /// <summary>
        /// The currency.
        /// </summary>
        Currency = 32,

        /// <summary>
        /// The dependent on currency.
        /// </summary>
        DependentOnCurrency = 64,

        /// <summary>
        /// The filter.
        /// </summary>
        Filter = 128,

        /// <summary>
        /// The dependent on weight.
        /// </summary>
        DependentOnWeight = 256,

        /// <summary>
        /// The weight.
        /// </summary>
        Weight = 512,

        /// <summary>
        /// The do not sort.
        /// </summary>
        DoNotSort = 1024,

        /// <summary>
        /// The show all.
        /// </summary>
        ShowAll = 2048,

        /// <summary>
        /// The must select.
        /// </summary>
        MustSelect = 4096,

        /// <summary>
        /// The read all.
        /// </summary>
        ReadAll = 8192,

        /// <summary>
        /// The alternate currency.
        /// </summary>
        AlternateCurrency = 16384,

        /// <summary>
        /// The no other.
        /// </summary>
        NoOther = 32768,

        /// <summary>
        /// The cat not mandatory.
        /// </summary>
        CatNotMandatory = 65536,

        /// <summary>
        /// The x category.
        /// </summary>
        XCategory = 131072,
    }

    /// <summary>
    /// Configurations related to Analysis
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Configuration.ConfigUnit" />
    public class UPConfigAnalysis : ConfigUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigAnalysis"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        public UPConfigAnalysis(List<object> definition)
        {
            this.QueryName = definition[1] as string;
            var tableDefs = (definition[2] as JArray)?.ToObject<List<object>>();
            if (tableDefs != null && tableDefs.Any())
            {
                var tables = new List<UPConfigAnalysisTable>(tableDefs.Count);
                foreach (JArray tableDef in tableDefs)
                {
                    var table = new UPConfigAnalysisTable(tableDef?.ToObject<List<object>>(), this);
                    tables.Add(table);
                }

                this.Tables = tables;
            }

            var valueDefs = (definition[4] as JArray)?.ToObject<List<object>>();
            if (valueDefs != null && valueDefs.Count > 0)
            {
                var values = new List<UPConfigAnalysisValue>(valueDefs.Count);
                foreach (JArray valueDef in valueDefs)
                {
                    var value = new UPConfigAnalysisValue(valueDef?.ToObject<List<object>>(), this);
                    values.Add(value);
                }

                this.Values = values;
            }

            var resultColumnDefs = (definition[3] as JArray)?.ToObject<List<object>>();
            if (resultColumnDefs != null && resultColumnDefs.Count > 0)
            {
                var resultColumns = new List<UPConfigAnalysisResultColumn>(resultColumnDefs.Count);
                foreach (JArray resultColumnDef in resultColumnDefs)
                {
                    var resultColumn = new UPConfigAnalysisResultColumn(resultColumnDef?.ToObject<List<object>>(), this);
                    resultColumns.Add(resultColumn);
                }

                this.ResultColumns = resultColumns;
            }

            if (definition.Count < 8)
            {
                this.MaxBars = 0;
                this.Options = 0;
                this.Flags = 0;
            }
            else
            {
                this.MaxBars = JObjectExtensions.ToInt(definition[5]);
                this.Options = JObjectExtensions.ToInt(definition[6]);
                this.Flags = JObjectExtensions.ToInt(definition[7]);
            }
        }

        /// <summary>
        /// Gets the flags.
        /// </summary>
        /// <value>
        /// The flags.
        /// </value>
        public int Flags { get; private set; }

        /// <summary>
        /// Gets the maximum bars.
        /// </summary>
        /// <value>
        /// The maximum bars.
        /// </value>
        public int MaxBars { get; private set; }

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        public int Options { get; private set; }

        /// <summary>
        /// Gets the name of the query.
        /// </summary>
        /// <value>
        /// The name of the query.
        /// </value>
        public string QueryName { get; private set; }

        /// <summary>
        /// Gets the result columns.
        /// </summary>
        /// <value>
        /// The result columns.
        /// </value>
        public List<UPConfigAnalysisResultColumn> ResultColumns { get; private set; }

        /// <summary>
        /// Gets the tables.
        /// </summary>
        /// <value>
        /// The tables.
        /// </value>
        public List<UPConfigAnalysisTable> Tables { get; private set; }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        public List<UPConfigAnalysisValue> Values { get; private set; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return
                $"name={this.UnitName}, queryName={this.QueryName}, tables={this.Tables}, values={this.Values}, resultColumns={this.ResultColumns}";
        }
    }

    /// <summary>
    /// Analysis field configurations
    /// </summary>
    public class UPConfigAnalysisField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigAnalysisField"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        /// <param name="analysisTable">
        /// The analysis table.
        /// </param>
        public UPConfigAnalysisField(List<object> definition, UPConfigAnalysisTable analysisTable)
        {
            if (definition == null || definition.Count < 8)
            {
                return;
            }

            this.AnalysisTable = analysisTable;
            this.FieldId = JObjectExtensions.ToInt(definition[0]);
            this.Flags = JObjectExtensions.ToInt(definition[1]);
            this.DefaultValue = definition[2] as string;
            this.DefaultEnd = definition[3] as string;
            this.CategoryName = definition[4] as string;
            this.ListColNr = JObjectExtensions.ToInt(definition[5]);
            this.ListWidth = JObjectExtensions.ToInt(definition[6]);
            this.Options = definition[7] as string;
            this.Slices = JObjectExtensions.ToInt(definition[8]);
        }

        /// <summary>
        /// Gets the analysis table.
        /// </summary>
        /// <value>
        /// The analysis table.
        /// </value>
        public UPConfigAnalysisTable AnalysisTable { get; private set; }

        /// <summary>
        /// Gets the name of the category.
        /// </summary>
        /// <value>
        /// The name of the category.
        /// </value>
        public string CategoryName { get; private set; }

        /// <summary>
        /// Gets the default end.
        /// </summary>
        /// <value>
        /// The default end.
        /// </value>
        public string DefaultEnd { get; private set; }

        /// <summary>
        /// Gets the default value.
        /// </summary>
        /// <value>
        /// The default value.
        /// </value>
        public string DefaultValue { get; private set; }

        /// <summary>
        /// Gets the field identifier.
        /// </summary>
        /// <value>
        /// The field identifier.
        /// </value>
        public int FieldId { get; private set; }

        /// <summary>
        /// Gets the flags.
        /// </summary>
        /// <value>
        /// The flags.
        /// </value>
        public int Flags { get; private set; }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Key => $"{this.AnalysisTable?.Key}.{this.FieldId}";

        /// <summary>
        /// Gets the list col nr.
        /// </summary>
        /// <value>
        /// The list col nr.
        /// </value>
        public int ListColNr { get; private set; }

        /// <summary>
        /// Gets the width of the list.
        /// </summary>
        /// <value>
        /// The width of the list.
        /// </value>
        public int ListWidth { get; private set; }

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        public string Options { get; private set; }

        /// <summary>
        /// Gets the slices.
        /// </summary>
        /// <value>
        /// The slices.
        /// </value>
        public int Slices { get; private set; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var fieldString = $"id={this.FieldId}";
            if (!string.IsNullOrEmpty(this.CategoryName))
            {
                fieldString += $", cat={this.CategoryName}";
            }

            return fieldString;
        }
    }

    /// <summary>
    /// Analysis table configurations
    /// </summary>
    public class UPConfigAnalysisTable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigAnalysisTable"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        /// <param name="analysis">
        /// The analysis.
        /// </param>
        public UPConfigAnalysisTable(List<object> definition, UPConfigAnalysis analysis)
        {
            if (definition == null)
            {
                return;
            }

            this.InfoAreaId = definition[0] as string;
            this.Occurrence = JObjectExtensions.ToInt(definition[1]);
            this.TableNumber = JObjectExtensions.ToInt(definition[2]);
            var fieldDefs = (definition[3] as JArray)?.ToObject<List<object>>();
            if (fieldDefs != null && fieldDefs.Count > 0)
            {
                var fields = new List<UPConfigAnalysisField>(fieldDefs.Count);
                foreach (JArray fieldDef in fieldDefs)
                {
                    var field = new UPConfigAnalysisField(fieldDef?.ToObject<List<object>>(), this);
                    fields.Add(field);
                }

                this.Fields = fields;
            }
        }

        /// <summary>
        /// Gets the fields.
        /// </summary>
        /// <value>
        /// The fields.
        /// </value>
        public List<UPConfigAnalysisField> Fields { get; private set; }

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; private set; }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Key => KeyWithInfoAreaIdOccurrence(this.InfoAreaId, this.Occurrence);

        /// <summary>
        /// Gets the occurrence.
        /// </summary>
        /// <value>
        /// The occurrence.
        /// </value>
        public int Occurrence { get; private set; }

        /// <summary>
        /// Gets the table number.
        /// </summary>
        /// <value>
        /// The table number.
        /// </value>
        public int TableNumber { get; private set; }

        /// <summary>
        /// Keys the with information area identifier occurrence.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="occurrence">
        /// The occurrence.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string KeyWithInfoAreaIdOccurrence(string infoAreaId, int occurrence)
        {
            return $"{infoAreaId}#{occurrence}";
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"infoAreaId({this.TableNumber})={this.InfoAreaId}, occ={this.Occurrence}, fields={this.Fields}";
        }
    }

    /// <summary>
    /// Analysis value configurations
    /// </summary>
    public class UPConfigAnalysisValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigAnalysisValue"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        /// <param name="analysis">
        /// The analysis.
        /// </param>
        public UPConfigAnalysisValue(List<object> definition, UPConfigAnalysis analysis)
        {
            this.ValueNumber = JObjectExtensions.ToInt(definition[0]);
            this.Name = definition[1] as string;
            this.FixedType = definition[2] as string;
            this.Label = definition[3] as string;
            this.Parameter = definition[4] as string;
            this.OptionString = definition[5] as string;
            this.Options = this.StringFromOptionString(this.OptionString);
        }

        /// <summary>
        /// Gets the type of the fixed.
        /// </summary>
        /// <value>
        /// The type of the fixed.
        /// </value>
        public string FixedType { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is category.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is category; otherwise, <c>false</c>.
        /// </value>
        public bool IsCategory => this.OptionIsSet("IsCategory");

        /// <summary>
        /// Gets a value indicating whether this instance is default category.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is default category; otherwise, <c>false</c>.
        /// </value>
        public bool IsDefaultCategory => this.OptionIsSet("DefaultCategory");

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Key => this.Name;

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        public Dictionary<string, object> Options { get; private set; }

        /// <summary>
        /// Gets the option string.
        /// </summary>
        /// <value>
        /// The option string.
        /// </value>
        public string OptionString { get; private set; }

        /// <summary>
        /// Gets the parameter.
        /// </summary>
        /// <value>
        /// The parameter.
        /// </value>
        public string Parameter { get; private set; }

        /// <summary>
        /// Gets the value number.
        /// </summary>
        /// <value>
        /// The value number.
        /// </value>
        public int ValueNumber { get; private set; }

        /// <summary>
        /// Options the is set.
        /// </summary>
        /// <param name="optionName">
        /// Name of the option.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool OptionIsSet(string optionName)
        {
            var o = this.Options?.ValueOrDefault(optionName);
            if (o == null)
            {
                return false;
            }

            if (o is int)
            {
                return JObjectExtensions.ToInt(o) != 0;
            }

            var p = o as string[];
            if (p != null && p.Length == 0)
            {
                return true;
            }

            if (p == null)
            {
                return false;
            }

            var p1 = p[0];
            return p1 == "true" || JObjectExtensions.ToInt(p1) != 0;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"value={this.Name}, param={this.Parameter}, options={this.OptionString}";
        }

        /// <summary>
        /// Strings from option string.
        /// </summary>
        /// <param name="optionString">
        /// The option string.
        /// </param>
        /// <returns>
        /// option lookup
        /// </returns>
        private Dictionary<string, object> StringFromOptionString(string optionString)
        {
            if (string.IsNullOrEmpty(optionString))
            {
                return null;
            }

            if (optionString.Length > 1 && optionString.StartsWith("{"))
            {
                return optionString.JsonDictionaryFromString();
            }

            var optionComponents = optionString.Split(';');
            var dict = new Dictionary<string, object>(optionComponents.Length);
            foreach (var component in optionComponents)
            {
                var parameterStart = component.IndexOf("(", StringComparison.Ordinal);
                if (parameterStart == -1)
                {
                    dict.SetObjectForKey(1, component);
                    continue;
                }

                var parameterName = component.Substring(parameterStart);
                var parameterEnd = component.IndexOf(")", ++parameterStart, StringComparison.Ordinal);
                if (parameterEnd == -1)
                {
                    continue;
                }

                var parameters = component.Substring(parameterStart, parameterEnd - parameterStart).Split(',');

                dict.SetObjectForKey(parameters, parameterName);
            }

            return dict;
        }
    }

    /// <summary>
    /// Analysis results column
    /// </summary>
    public class UPConfigAnalysisResultColumn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigAnalysisResultColumn"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        /// <param name="analysis">
        /// The analysis.
        /// </param>
        public UPConfigAnalysisResultColumn(List<object> definition, UPConfigAnalysis analysis)
        {
            this.FieldTableId = JObjectExtensions.ToInt(definition[0]);
            this.FieldId = JObjectExtensions.ToInt(definition[1]);
            this.AggregationType = definition[2] as string;
            this.CategoryName = definition[3] as string;
            this.ValueName = definition[4] as string;
        }

        /// <summary>
        /// Gets the type of the aggregation.
        /// </summary>
        /// <value>
        /// The type of the aggregation.
        /// </value>
        public string AggregationType { get; private set; }

        /// <summary>
        /// Gets the name of the category.
        /// </summary>
        /// <value>
        /// The name of the category.
        /// </value>
        public string CategoryName { get; private set; }

        /// <summary>
        /// Gets the field identifier.
        /// </summary>
        /// <value>
        /// The field identifier.
        /// </value>
        public int FieldId { get; private set; }

        /// <summary>
        /// Gets the field table identifier.
        /// </summary>
        /// <value>
        /// The field table identifier.
        /// </value>
        public int FieldTableId { get; private set; }

        /// <summary>
        /// Gets the name of the value.
        /// </summary>
        /// <value>
        /// The name of the value.
        /// </value>
        public string ValueName { get; private set; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return !string.IsNullOrEmpty(this.ValueName)
                       ? $"value={this.ValueName}, type={this.AggregationType}"
                       : $"field={this.FieldTableId}/{this.FieldId}, type={this.AggregationType}";
        }
    }
}
