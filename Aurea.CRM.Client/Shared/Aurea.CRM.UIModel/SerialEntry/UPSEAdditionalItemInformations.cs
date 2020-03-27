// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEAdditionalItemInformations.cs" company="Aurea Software Gmbh">
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
//   UPSEAdditionalItemInformations
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM;

    /// <summary>
    /// UPSEAdditionalItemInformations
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.SerialEntry.UPSESingleAdditionalItemInformationDelegate" />
    public class UPSEAdditionalItemInformations : UPSESingleAdditionalItemInformationDelegate
    {
        private int currentLoadIndex;
        private bool loadStarted;
        private UPRequestOption requestOption;

        /// <summary>
        /// Gets the additional item array.
        /// </summary>
        /// <value>
        /// The additional item array.
        /// </value>
        public List<UPSESingleAdditionalItemInformation> AdditionalItemArray { get; private set; }

        /// <summary>
        /// Gets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public UPSEAdditionalItemInformationsDelegate TheDelegate { get; private set; }

        /// <summary>
        /// Creates the specified serial entry.
        /// </summary>
        /// <param name="serialEntry">The serial entry.</param>
        /// <param name="configNames">The configuration names.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="filterParameters">The filter parameters.</param>
        /// <param name="theDelegate">The delegate.</param>
        /// <returns></returns>
        public static UPSEAdditionalItemInformations Create(UPSerialEntry serialEntry, List<string> configNames, string columnName,
            Dictionary<string, object> filterParameters, UPSEAdditionalItemInformationsDelegate theDelegate)
        {
            try
            {
                return new UPSEAdditionalItemInformations(serialEntry, configNames, columnName, filterParameters, theDelegate);
            }
            catch
            {
                return null;
            }
        }

        private UPSEAdditionalItemInformations(UPSerialEntry serialEntry, List<string> configNames, string columnName,
            Dictionary<string, object> filterParameters, UPSEAdditionalItemInformationsDelegate theDelegate)
        {
            List<UPSESingleAdditionalItemInformation> additionalItems = null;
            foreach (string configName in configNames)
            {
                var item = UPSESingleAdditionalItemInformation.Create(serialEntry, configName, columnName, filterParameters, this);
                if (item != null)
                {
                    if (additionalItems == null)
                    {
                        additionalItems = new List<UPSESingleAdditionalItemInformation> { item };
                    }
                    else
                    {
                        additionalItems.Add(item);
                    }
                }
            }

            if (additionalItems == null)
            {
                throw new Exception("Additional Items is null");
            }

            this.AdditionalItemArray = additionalItems;
            this.TheDelegate = theDelegate;
        }

        /// <summary>
        /// Loads the next.
        /// </summary>
        void LoadNext()
        {
            if (this.currentLoadIndex < this.AdditionalItemArray.Count)
            {
                UPSESingleAdditionalItemInformation item = this.AdditionalItemArray[this.currentLoadIndex++];
                item.LoadWithRequestOption(this.requestOption);
            }
            else
            {
                this.TheDelegate?.AdditionalItemsInformationDidFinishWithResult(this, null);
            }
        }

        /// <summary>
        /// Loads the with request option.
        /// </summary>
        /// <param name="requestOption">The request option.</param>
        /// <returns></returns>
        public bool LoadWithRequestOption(UPRequestOption requestOption)
        {
            if (this.loadStarted)
            {
                return false;
            }

            this.loadStarted = true;
            this.currentLoadIndex = 0;
            this.requestOption = requestOption;
            this.LoadNext();
            return true;
        }

        /// <summary>
        /// Additionals the source fields with key column.
        /// </summary>
        /// <param name="keyColumn">The key column.</param>
        /// <param name="startIndex">The start index.</param>
        /// <returns></returns>
        public List<UPSEColumn> AdditionalSourceFieldsWithKeyColumn(UPSESourceColumn keyColumn, int startIndex)
        {
            List<UPSEColumn> fieldArray = null;
            foreach (UPSESingleAdditionalItemInformation addInfo in this.AdditionalItemArray)
            {
                List<UPSESourceAdditionalColumn> fields = addInfo.AdditionalSourceFieldsWithKeyColumn(keyColumn, startIndex);
                if (fields.Count > 0)
                {
                    if (fieldArray == null)
                    {
                        fieldArray = new List<UPSEColumn>(fields);
                    }
                    else
                    {
                        fieldArray.AddRange(fields);
                    }

                    startIndex += fields.Count;
                }
            }

            return fieldArray;
        }

        /// <summary>
        /// Additionals the item information did finish with result.
        /// </summary>
        /// <param name="addItem">The add item.</param>
        /// <param name="result">The result.</param>
        public void AdditionalItemInformationDidFinishWithResult(UPSESingleAdditionalItemInformation addItem, object result)
        {
            this.LoadNext();
        }

        /// <summary>
        /// Additionals the item information did fail with error.
        /// </summary>
        /// <param name="addItem">The add item.</param>
        /// <param name="error">The error.</param>
        public void AdditionalItemInformationDidFailWithError(UPSESingleAdditionalItemInformation addItem, Exception error)
        {
            this.TheDelegate.AdditionalItemsInformationDidFailWithError(this, error);
        }
    }
}
