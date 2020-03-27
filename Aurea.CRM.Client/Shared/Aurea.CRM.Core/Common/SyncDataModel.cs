// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncDataModel.cs" company="Aurea Software Gmbh">
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
//   The synchronization data model
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Common
{
    using System.Collections.Generic;

    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.DAL;
    using Aurea.CRM.Core.Extensions;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// The synchronization data model
    /// </summary>
    public class UPSyncDataModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSyncDataModel"/> class.
        /// </summary>
        /// <param name="dataStore">
        /// The data store.
        /// </param>
        public UPSyncDataModel(ICRMDataStore dataStore)
        {
            this.DataStore = dataStore;
        }

        /// <summary>
        /// Gets the data store.
        /// </summary>
        /// <value>
        /// The data store.
        /// </value>
        public ICRMDataStore DataStore { get; private set; }

        /// <summary>
        /// Synchronizes the with data model definition.
        /// </summary>
        /// <param name="dataModelDef">
        /// The data model definition.
        /// </param>
        /// <returns>
        /// 1 if error, else 0
        /// </returns>
        public int SyncWithDataModelDefinition(Dictionary<string, object> dataModelDef)
        {
            var tables = dataModelDef?.ValueOrDefault("tables") as JArray;
            if (tables == null)
            {
                return 0;
            }

            var dataModel = new DataModel(this.DataStore.DatabaseInstance);
            foreach (var tableDefArray in tables)
            {
                this.AddTable(dataModel, tableDefArray as JArray);
            }

            return this.DataStore.StoreDataModel(dataModel);
        }

        /// <summary>
        /// Adds the field to table information field definition.
        /// </summary>
        /// <param name="tableInfo">
        /// The table information.
        /// </param>
        /// <param name="fieldDefArray">
        /// The field definition array.
        /// </param>
        private void AddField(TableInfo tableInfo, List<object> fieldDefArray)
        {
            if (fieldDefArray == null)
            {
                return;
            }

            var rightsValue = 0;
            var formatValue = 0;
            if (fieldDefArray.Count > 8)
            {
                rightsValue = int.Parse(fieldDefArray[8].ToString());
            }

            if (fieldDefArray.Count > 9)
            {
                formatValue = int.Parse(fieldDefArray[9].ToString());
            }

            int arrayFieldCount;
            int[] arrayFieldIndices;
            if (fieldDefArray.Count > 10)
            {
                var arrayFieldIndicesArray = (fieldDefArray[10] as JArray)?.ToObject<List<object>>();
                arrayFieldCount = arrayFieldIndicesArray?.Count ?? 0;
                if (arrayFieldCount > 0)
                {
                    arrayFieldIndices = new int[arrayFieldCount];
                    for (var i = 0; i < arrayFieldCount; i++)
                    {
                        arrayFieldIndices[i] = JObjectExtensions.ToInt(arrayFieldIndicesArray[i]);
                    }
                }
                else
                {
                    arrayFieldIndices = null;
                }
            }
            else
            {
                arrayFieldCount = 0;
                arrayFieldIndices = null;
            }

            var fieldInfo = new FieldInfo(
                tableInfo.InfoAreaId,
                JObjectExtensions.ToInt(fieldDefArray[0]),
                (string)fieldDefArray[1],
                (string)fieldDefArray[1],
                (char)fieldDefArray[2].ToInt(),
                JObjectExtensions.ToInt(fieldDefArray[5]),
                JObjectExtensions.ToInt(fieldDefArray[3]),
                JObjectExtensions.ToInt(fieldDefArray[4]),
                JObjectExtensions.ToInt(fieldDefArray[6]),
                (string)fieldDefArray[7],
                rightsValue,
                formatValue,
                arrayFieldCount,
                arrayFieldIndices);

            tableInfo.AddFieldInfoWithOwnership(fieldInfo);
        }

        /// <summary>
        /// Adds the link to table information link definition.
        /// </summary>
        /// <param name="tableInfo">
        /// The table information.
        /// </param>
        /// <param name="linkDefArray">
        /// The link definition array.
        /// </param>
        private void AddLink(TableInfo tableInfo, JArray linkDefArray)
        {
            var sourceFieldId = -1;
            var destFieldId = -1;
            int linkFlag;
            if (linkDefArray.Count > 4)
            {
                sourceFieldId = JObjectExtensions.ToInt(linkDefArray[4]);
                destFieldId = JObjectExtensions.ToInt(linkDefArray[5]);
            }
            else
            {
                sourceFieldId = -1;
                destFieldId = -1;
            }

            if (linkDefArray.Count > 7)
            {
                linkFlag = JObjectExtensions.ToInt(linkDefArray[7]);
            }
            else
            {
                linkFlag = 0;
            }

            var arr = linkDefArray.Count > 6 ? linkDefArray[6] as JArray : null;

            if (arr == null)
            {
                return;
            }

            var linkFieldCount = arr.Count;
            var sourceFieldIds = new int[linkFieldCount];
            var destinationFieldIds = new int[linkFieldCount];
            var sourceValuesBuffer = new string[linkFieldCount];
            var destinationValuesBuffer = new string[linkFieldCount];
            for (var i = 0; i < arr.Count; i++)
            {
                var fieldArray = arr[i] as JArray;
                sourceFieldIds[i] = JObjectExtensions.ToInt(fieldArray?[0]);
                destinationFieldIds[i] = JObjectExtensions.ToInt(fieldArray?[1]);
                if (fieldArray.Count > 2)
                {
                    var sourceValue = fieldArray[2];
                    var destValue = fieldArray[3];

                    sourceValuesBuffer[i] = (string)sourceValue;
                    destinationValuesBuffer[i] = (string)destValue;
                }
                else
                {
                    sourceValuesBuffer[i] = null;
                    destinationValuesBuffer[i] = null;
                }
            }

            var linkInfo = new LinkInfo(
                tableInfo.InfoAreaId,
                (string)linkDefArray[0],
                JObjectExtensions.ToInt(linkDefArray[1]),
                JObjectExtensions.ToInt(linkDefArray[2]),
                LinkInfo.ToLinkType((string)linkDefArray[3]),
                sourceFieldId,
                destFieldId,
                linkFieldCount,
                sourceFieldIds,
                destinationFieldIds,
                linkFlag,
                sourceValuesBuffer,
                destinationValuesBuffer);

            tableInfo.AddLinkInfoWithOwnership(linkInfo);
        }

        /// <summary>
        /// Adds the table.
        /// </summary>
        /// <param name="dataModel">
        /// The data model.
        /// </param>
        /// <param name="tableDefArray">
        /// The table definition array.
        /// </param>
        private void AddTable(DataModel dataModel, JArray tableDefArray)
        {
            if (tableDefArray == null)
            {
                return;
            }

            var rootInfoAreaid = (string)tableDefArray[1];
            var tableInfo = new TableInfo(
                (string)tableDefArray[0],
                rootInfoAreaid,
                dataModel.GetRootPhysicalInfoAreaId(rootInfoAreaid),
                (string)tableDefArray[3],
                0);

            var fieldDefs = tableDefArray[4] as JArray;
            var linkDefs = tableDefArray[5] as JArray;

            if (fieldDefs != null)
            {
                foreach (var fieldDefArray in fieldDefs)
                {
                    this.AddField(tableInfo, (fieldDefArray as JArray)?.ToObject<List<object>>());
                }
            }

            if (linkDefs != null)
            {
                foreach (var linkDefArray in linkDefs)
                {
                    this.AddLink(tableInfo, linkDefArray as JArray);
                }
            }

            dataModel.AddTableInfoWithOwnership(tableInfo);
        }
    }
}
