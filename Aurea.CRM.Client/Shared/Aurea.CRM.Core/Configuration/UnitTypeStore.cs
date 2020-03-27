// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitTypeStore.cs" company="Aurea Software Gmbh">
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
//   UnitType definition
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.DAL;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    //using Microsoft.Practices.ServiceLocation;
    using GalaSoft.MvvmLight.Ioc;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// UnitType definition
    /// </summary>
    public class UnitTypeStore : IUnitTypeStore
    {
        /// <summary>
        /// The factory.
        /// </summary>
        private readonly Func<object, object> factory;

        /// <summary>
        /// The pre load.
        /// </summary>
        private bool preLoad;

        /// <summary>
        /// The unit def store.
        /// </summary>
        private Dictionary<string, string> unitDefStore;

        /// <summary>
        /// The unit store.
        /// </summary>
        private Dictionary<string, ConfigUnit> unitStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitTypeStore"/> class.
        /// </summary>
        /// <param name="unitType">
        /// Type of the unit.
        /// </param>
        /// <param name="tableName">
        /// Name of the table.
        /// </param>
        /// <param name="shouldPreLoad">
        /// if set to <c>true</c> [should pre load].
        /// </param>
        /// <param name="creator">
        /// The creator.
        /// </param>
        /// <param name="configStore">
        /// The configuration store.
        /// </param>
        public UnitTypeStore(
            string unitType,
            string tableName,
            bool shouldPreLoad,
            Func<object, object> creator,
            ConfigurationUnitStore configStore)
        {
            this.UnitType = unitType;
            this.TableName = tableName;
            this.preLoad = shouldPreLoad;
            this.factory = creator;
            this.unitDefStore = null;
            this.unitStore = null;
            this.ConfigStore = configStore;
        }

        /// <summary>
        /// Gets the configuration store.
        /// </summary>
        /// <value>
        /// The configuration store.
        /// </value>
        public ConfigurationUnitStore ConfigStore { get; private set; }

        /// <summary>
        /// Gets logging interface
        /// </summary>
        public ILogger Logger => SimpleIoc.Default.GetInstance<ILogger>();

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <value>
        /// The database.
        /// </value>
        public ConfigDatabase Database => this.ConfigStore.DatabaseInstance;

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <value>
        /// The name of the table.
        /// </value>
        public string TableName { get; }

        /// <summary>
        /// Gets the type of the unit.
        /// </summary>
        /// <value>
        /// The type of the unit.
        /// </value>
        public string UnitType { get; }

        /// <summary>
        /// Alls the unit names.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<string> AllUnitNames()
        {
            lock (this)
            {
                if (this.preLoad && this.unitDefStore != null)
                {
                    return this.unitDefStore?.Keys.ToList();
                }

                if (!this.LoadTable())
                {
                    return null;
                }

                this.preLoad = true;

                return this.unitDefStore?.Keys.ToList();
            }
        }

        /// <summary>
        /// Alls the unit names sorted.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<string> AllUnitNamesSorted()
        {
            var sortedList = this.AllUnitNames();
            sortedList?.Sort();

            return sortedList;
        }

        /// <summary>
        /// Alls the units.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<ConfigUnit> AllUnits()
        {
            lock (this)
            {
                if (!this.preLoad || this.unitDefStore == null)
                {
                    if (!this.LoadTable() || this.unitDefStore == null)
                    {
                        return null;
                    }

                    this.preLoad = true;
                }

                return this.unitDefStore.Keys.Select(this.UnitWithName).ToList();
            }
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Load()
        {
            this.Reset();
            return !this.preLoad || this.LoadTable();
        }

        /// <summary>
        /// Loads the table.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool LoadTable()
        {
            if (this.Database.ExistsTable(this.TableName))
            {
                var tableRecordSet = new DatabaseRecordSet(this.Database);
                var statementString = $"SELECT unitName, unitDef FROM {this.TableName}";
                var ret = tableRecordSet.Query.Prepare(statementString);
                if (ret)
                {
                    ret = tableRecordSet.Execute() == 0;
                }

                if (ret)
                {
                    var unitCount = tableRecordSet.GetRowCount();
                    if (unitCount > 0)
                    {
                        this.unitDefStore = new Dictionary<string, string>();
                        for (var i = 0; i < unitCount; i++)
                        {
                            var row = tableRecordSet.GetRow(i);
                            var theObject = row.GetColumn(1);
                            var key = row.GetColumn(0);
                            if (key != null)
                            {
                                this.unitDefStore.SetObjectForKey(theObject, key);
                            }
                        }
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Loads the unit.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object LoadUnit(string unitName)
        {
            object result = null;

            var recordSet = new DatabaseRecordSet(this.Database);
            var statementString = $"SELECT unitDef FROM {this.TableName} WHERE unitName = ?";
            var ret = recordSet.Query.Prepare(statementString);
            if (!ret)
            {
                return null;
            }

            recordSet.Query.Bind(1, unitName);
            recordSet.Execute();

            if (recordSet.GetRowCount() == 0)
            {
                return null;
            }

            var row = recordSet.GetRow(0);
            var def = row.GetColumn(0);
            result = JsonConvert.DeserializeObject<List<object>>(def as string);

            return result;
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            this.unitDefStore = null;
            this.unitStore = null;
        }

        /// <summary>
        /// Synchronizes the elements.
        /// </summary>
        /// <param name="elements">
        /// The elements.
        /// </param>
        /// <param name="emptyTable">
        /// if set to <c>true</c> [empty table].
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int SyncElements(JArray elements, bool emptyTable)
        {
            var ret = 0;
            var tableName = this.TableName;

            if (emptyTable)
            {
                ret = this.Database.EmptyTable(tableName);

                this.Database.BeginTransaction();

                foreach (var element in elements)
                {
                    var innerArray = element as JArray;
                    if (innerArray == null || !innerArray.HasValues || string.IsNullOrEmpty(innerArray[0].ToString()))
                    {
                        continue;
                    }

                    var json = this.ToJSON(element);
                    ret = this.Database.WriteUnitToTable(tableName, innerArray[0].ToString(), json);
                    if (ret == 1)
                    {
                        break;
                    }
                }

                this.Database.Commit();
            }
            else
            {
                this.Database.BeginTransaction();

                foreach (var element in elements)
                {
                    var unitName = element[0].ToString();
                    var definition = this.ToJSON(element);

                    var existsStatement = this.Database.CreateExistsUnitStatement(tableName, unitName);

                    if (existsStatement.ExecuteNonQuery() > 0)
                    {
                        var updateStatement = this.Database.CreateUpdateUnitStatement(tableName, unitName, definition);

                        ret = updateStatement.ExecuteNonQuery();
                    }
                    else
                    {
                        var insertStatement = this.Database.CreateInsertUnitStatement(tableName, unitName, definition);
                        ret = insertStatement.ExecuteNonQuery();
                    }
                }

                if (ret < 0)
                {
                    this.Database.Rollback();
                }
                else
                {
                    this.Database.Commit();
                }
            }

            // return ret;
            return 0;
        }

        /// <summary>
        /// To the json.
        /// </summary>
        /// <param name="theObject">
        /// The object.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ToJSON(object theObject)
        {
            return JsonConvert.SerializeObject(theObject);
        }

        /// <summary>
        /// Units the name of the with.
        /// </summary>
        /// <param name="unitName">
        /// Name of the unit.
        /// </param>
        /// <returns>
        /// The <see cref="ConfigUnit"/>.
        /// </returns>
        public virtual ConfigUnit UnitWithName(string unitName)
        {
            lock (this)
            {
                ConfigUnit unit = null;
                if (this.unitStore != null)
                {
                    unit = this.unitStore.ValueOrDefault(unitName);
                    if (unit != null)
                    {
                        this.Logger.LogDebug($"{this.UnitType}: {unitName}", LogFlag.LogConfig);
                        return unit;
                    }
                }

                object unitDef = null;
                if (this.preLoad || this.unitDefStore != null)
                {
                    if (this.unitDefStore == null)
                    {
                        this.Load();
                    }

                    if (this.unitDefStore != null)
                    {
                        object jsonDef = this.unitDefStore.ValueOrDefault(unitName);
                        if (jsonDef == null)
                        {
                            // if (UPLogSettings.LogConfig() && unitName.RangeOfString(CONFIGUNIT_V1POSTFIX).Location == NSNotFound)
                            // {
                            // DDLogConfig("%@: %@ not found -> value empty", _unitType, unitName);
                            // }
                            this.Logger.LogDebug($"{this.UnitType}: {unitName} not found -> empty value", LogFlag.LogConfig);
                            return null;
                        }

                        unitDef = JsonConvert.DeserializeObject<List<object>>(jsonDef as string);
                    }
                }
                else
                {
                    unitDef = this.LoadUnit(unitName);
                }

                if (unitDef == null)
                {
                    // if (UPLogSettings.LogConfig() && unitName.RangeOfString(CONFIGUNIT_V1POSTFIX).Location == NSNotFound)
                    // {
                    // DDLogConfig("%@: %@ not found -> value empty", _unitType, unitName);
                    // }
                    this.Logger.LogDebug($"{this.UnitType}: {unitName} not found -> empty value", LogFlag.LogConfig);
                    return null;
                }

                unit = (ConfigUnit)this.factory(unitDef);
                if (this.unitStore == null)
                {
                    this.unitStore = new Dictionary<string, ConfigUnit>();
                }

                this.unitStore[unitName] = unit;

                // if (UPLogSettings.LogConfig())
                // {
                // DDLogConfig("%@: %@", _unitType, unitName);
                // }
                this.Logger.LogDebug($"{this.UnitType}: {unitName} not found -> empty value", LogFlag.LogConfig);
                return unit;
            }
        }
    }
}
