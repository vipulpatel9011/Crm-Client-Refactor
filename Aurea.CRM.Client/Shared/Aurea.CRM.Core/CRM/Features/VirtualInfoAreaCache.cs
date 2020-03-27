// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VirtualInfoAreaCache.cs" company="Aurea Software Gmbh">
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
//   Virtual info area cache for given info area
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.Features
{
    using System.Collections.Generic;

    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Virtual info area cache
    /// </summary>
    public class UPVirtualInfoAreaCache
    {
        /// <summary>
        /// The cache for information area
        /// </summary>
        private Dictionary<string, UPVirtualInfoAreaCacheForInfoArea> cacheForInfoArea;

        /// <summary>
        /// Applies the result.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        public void ApplyResult(UPCRMResult result)
        {
            if (!result.IsServerResult)
            {
                return;
            }

            var resultRowCount = result.RowCount;
            if (resultRowCount == 0)
            {
                return;
            }

            var dataStore = UPCRMDataStore.DefaultStore;
            var resultInfoAreaCount = result.MetaInfo.NumberOfResultInfoAreaMetaInfos();
            var mapping = new int[resultInfoAreaCount];
            var cacheForInfoAreaMap = new UPVirtualInfoAreaCacheForInfoArea[resultInfoAreaCount];
            var count = 0;

            for (var i = 0; i < resultInfoAreaCount; i++)
            {
                var infoAreaMetaInfo = result.MetaInfo.ResultInfoAreaMetaInfoAtIndex(i);
                var tableInfo = dataStore.TableInfoForInfoArea(infoAreaMetaInfo.InfoAreaId);
                if (tableInfo == null || !tableInfo.HasVirtualInfoAreas)
                {
                    continue;
                }

                mapping[count] = i;
                cacheForInfoAreaMap[count] = this.CacheForInfoAreaId(tableInfo.InfoAreaId);
                ++count;
            }

            if (count == 0)
            {
                return;
            }

            for (var i = 0; i < resultRowCount; i++)
            {
                var row = result.ResultRowAtIndex(i) as UPCRMResultRow;
                if (row == null)
                {
                    continue;
                }

                for (var j = 0; j < count; j++)
                {
                    cacheForInfoAreaMap[j].AddRecordIdInfoAreaId(
                        row.RecordIdAtIndex(mapping[j]),
                        row.VirtualInfoAreaIdAtIndex(mapping[j]));
                }
            }
        }

        /// <summary>
        /// Caches for information area identifier.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <returns>
        /// The <see cref="UPVirtualInfoAreaCacheForInfoArea"/>.
        /// </returns>
        public UPVirtualInfoAreaCacheForInfoArea CacheForInfoAreaId(string infoAreaId)
        {
            var dataStore = UPCRMDataStore.DefaultStore;
            infoAreaId = dataStore.RootInfoAreaIdForInfoAreaId(infoAreaId);
            var tableInfo = dataStore.TableInfoForInfoArea(infoAreaId);
            if (!tableInfo.HasVirtualInfoAreas)
            {
                return null;
            }

            var infoAreaCache = this.cacheForInfoArea.ValueOrDefault(infoAreaId);
            if (infoAreaCache != null)
            {
                return infoAreaCache;
            }

            infoAreaCache = new UPVirtualInfoAreaCacheForInfoArea(infoAreaId);
            if (this.cacheForInfoArea != null)
            {
                this.cacheForInfoArea.SetObjectForKey(infoAreaCache, infoAreaId);
            }
            else
            {
                this.cacheForInfoArea = new Dictionary<string, UPVirtualInfoAreaCacheForInfoArea>
                                            {
                                                { infoAreaId, infoAreaCache }
                                            };
            }

            return infoAreaCache;
        }

        /// <summary>
        /// Virtuals the information area identifier for record identification.
        /// </summary>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string VirtualInfoAreaIdForRecordIdentification(string recordIdentification)
        {
            var infoAreaId = recordIdentification.InfoAreaId();
            var infoAreaCache = this.cacheForInfoArea.ValueOrDefault(infoAreaId);
            return infoAreaCache != null
                       ? infoAreaCache.VirtualInfoAreaIdForRecordId(recordIdentification.RecordId())
                       : infoAreaId;
        }
    }
}
