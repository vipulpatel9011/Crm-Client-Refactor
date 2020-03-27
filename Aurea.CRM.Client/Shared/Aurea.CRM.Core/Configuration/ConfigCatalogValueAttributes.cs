// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigCatalogValueAttributes.cs" company="Aurea Software Gmbh">
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
//   Catalog value attributes configurations
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Configuration
{
    using System;
    using System.Collections.Generic;

    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Catalog value attributes configurations
    /// </summary>
    public class UPConfigCatalogValueAttributes
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigCatalogValueAttributes"/> class.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <param name="colorKey">
        /// The color key.
        /// </param>
        /// <param name="imageName">
        /// Name of the image.
        /// </param>
        public UPConfigCatalogValueAttributes(int code, string colorKey, string imageName)
        {
            this.Code = code;
            this.ColorKey = colorKey;
            this.ImageName = imageName;
            this.RawValue = $"{this.Code}";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigCatalogValueAttributes"/> class.
        /// </summary>
        /// <param name="rawValue">
        /// The raw value.
        /// </param>
        /// <param name="colorKey">
        /// The color key.
        /// </param>
        /// <param name="imageName">
        /// Name of the image.
        /// </param>
        public UPConfigCatalogValueAttributes(string rawValue, string colorKey, string imageName)
        {
            this.Code = int.Parse(rawValue);
            this.ColorKey = colorKey;
            this.ImageName = imageName;
            this.RawValue = rawValue;
        }

        /// <summary>
        /// Gets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public int Code { get; private set; }

        /// <summary>
        /// Gets the color key.
        /// </summary>
        /// <value>
        /// The color key.
        /// </value>
        public string ColorKey { get; private set; }

        /// <summary>
        /// Gets the name of the image.
        /// </summary>
        /// <value>
        /// The name of the image.
        /// </value>
        public string ImageName { get; private set; }

        /// <summary>
        /// Gets the raw value.
        /// </summary>
        /// <value>
        /// The raw value.
        /// </value>
        public string RawValue { get; private set; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"value={this.RawValue}: color={this.ColorKey}, image={this.ImageName}";
        }
    }

    /// <summary>
    /// Catalog attributes
    /// </summary>
    public class UPConfigCatalogAttributes
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigCatalogAttributes"/> class.
        /// </summary>
        /// <param name="filter">
        /// The filter.
        /// </param>
        public UPConfigCatalogAttributes(UPConfigFilter filter)
        {
            var rootTable = filter?.RootTable;
            var arr = rootTable?.QueryConditions(string.Empty, false);
            if (arr == null || arr.Count != 1)
            {
                return;
            }

            var codeQueryCondition = arr[0];
            this.CrmField = UPCRMField.FieldWithFieldIdInfoAreaId(codeQueryCondition.FieldId, rootTable.InfoAreaId);
            this.FieldInfo = UPCRMDataStore.DefaultStore.FieldInfoForField(this.CrmField);
            if (this.FieldInfo?.FieldType == "X")
            {
                this.FixedCatalog = true;
                this.CatalogNumber = this.FieldInfo.CatNo;
            }
            else if (this.FieldInfo?.FieldType == "K")
            {
                this.FixedCatalog = false;
                this.CatalogNumber = this.FieldInfo.CatNo;
            }
            else
            {
                this.FixedCatalog = false;
                this.CatalogNumber = -1;
            }

            arr = rootTable.QueryConditions("Image", false);
            var imageQueryCondition = arr != null && arr.Count > 0 ? arr[0] : null;

            arr = rootTable.QueryConditions("Color", false);
            var colorKeyQueryCondition = arr != null && arr.Count > 0 ? arr[0] : null;

            var catCodes = codeQueryCondition?.FieldValues;
            var imageNames = imageQueryCondition?.FieldValues;
            var colorKeys = colorKeyQueryCondition?.FieldValues;

            var catCodesTemp = catCodes != null ? new List<object>(catCodes) : new List<object>();
            catCodesTemp.Remove(string.Empty);
            catCodes = catCodesTemp;

            int count = catCodes.Count, colorKeysCount = colorKeys?.Count ?? 0, imageNamesCount = imageNames?.Count ?? 0;

            var dict = new Dictionary<int, UPConfigCatalogValueAttributes>(count);
            var rawDict = new Dictionary<string, UPConfigCatalogValueAttributes>(count);
            var orderedValues = new List<UPConfigCatalogValueAttributes>(count);
            for (var i = 0; i < count; i++)
            {
                var image = i < imageNamesCount ? imageNames[i] as string : string.Empty;
                var color = i < colorKeysCount ? colorKeys[i] as string : string.Empty;
                if (image.StartsWith("#"))
                {
                    image = image.Substring(1);
                }

                if (color.StartsWith("#"))
                {
                    color = color.Substring(1);
                }

                var valueAttr = this.CatalogNumber >= 0
                                    ? new UPConfigCatalogValueAttributes(
                                          JObjectExtensions.ToInt(catCodes[i]),
                                          color,
                                          image)
                                    : new UPConfigCatalogValueAttributes(
                                          JObjectExtensions.ToInt(catCodes[i]),
                                          color,
                                          image);

                dict.SetObjectForKey(valueAttr, valueAttr.Code);
                rawDict.SetObjectForKey(valueAttr, valueAttr.RawValue);
                orderedValues.Add(valueAttr);
            }

            this.ValuesByCode = dict;
            this.ValuesByRawValue = rawDict;
            this.ValueArray = orderedValues;
        }

        /// <summary>
        /// Gets the catalog number.
        /// </summary>
        /// <value>
        /// The catalog number.
        /// </value>
        public int CatalogNumber { get; private set; }

        /// <summary>
        /// Gets the CRM field.
        /// </summary>
        /// <value>
        /// The CRM field.
        /// </value>
        public UPCRMField CrmField { get; private set; }

        /// <summary>
        /// Gets the field information.
        /// </summary>
        /// <value>
        /// The field information.
        /// </value>
        public UPCRMFieldInfo FieldInfo { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [fixed catalog].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [fixed catalog]; otherwise, <c>false</c>.
        /// </value>
        public bool FixedCatalog { get; private set; }

        /// <summary>
        /// Gets the value array.
        /// </summary>
        /// <value>
        /// The value array.
        /// </value>
        public List<UPConfigCatalogValueAttributes> ValueArray { get; private set; }

        /// <summary>
        /// Gets the values by code.
        /// </summary>
        /// <value>
        /// The values by code.
        /// </value>
        public Dictionary<int, UPConfigCatalogValueAttributes> ValuesByCode { get; private set; }

        /// <summary>
        /// Gets the values by raw value.
        /// </summary>
        /// <value>
        /// The values by raw value.
        /// </value>
        public Dictionary<string, UPConfigCatalogValueAttributes> ValuesByRawValue { get; private set; }

        /// <summary>
        /// Colors the key for raw value.
        /// </summary>
        /// <param name="rawValue">
        /// The raw value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ColorKeyForRawValue(string rawValue)
        {
            var attr = this.ValuesByRawValue?.ValueOrDefault(rawValue);
            return attr?.ColorKey;
        }

        /// <summary>
        /// Images the name for raw value.
        /// </summary>
        /// <param name="rawValue">
        /// The raw value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ImageNameForRawValue(string rawValue)
        {
            var attr = this.ValuesByRawValue?.ValueOrDefault(rawValue);
            return attr?.ImageName;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return
                $"catNo={this.CatalogNumber},fix={this.FixedCatalog}, values={Environment.NewLine}{this.ValuesByRawValue}";
        }
    }
}
