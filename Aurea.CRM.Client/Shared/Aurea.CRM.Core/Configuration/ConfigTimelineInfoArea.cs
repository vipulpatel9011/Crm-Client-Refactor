// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigTimelineInfoArea.cs" company="Aurea Software Gmbh">
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
//   Config Timeline InfoArea
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
    /// Time line info area configurations
    /// </summary>
    public class ConfigTimelineInfoArea
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigTimelineInfoArea"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        public ConfigTimelineInfoArea(List<object> definition)
        {
            this.InfoAreaId = definition[0] as string;
            this.LinkId = Convert.ToInt32(definition[1]);
            this.ConfigName = definition[2] as string;
            var dateFieldIndex = Convert.ToInt32(definition[3]);
            this.ColorString = definition[4] as string;
            this.Color2String = definition[5] as string;
            this.FilterName = definition[8] as string;
            this.Text = definition[9] as string;

            var colorCriteriaDefs = (definition[10] as JArray)?.ToObject<List<object>>();
            if (colorCriteriaDefs != null && colorCriteriaDefs.Any())
            {
                var colorCriteriaArray = new List<ConfigTimelineCriteria>(colorCriteriaDefs.Count);
                foreach (JArray colorCriteriaDef in colorCriteriaDefs)
                {
                    if (colorCriteriaDef == null)
                    {
                        continue;
                    }

                    var criteria = new ConfigTimelineCriteria(colorCriteriaDef.ToObject<List<object>>());
                    colorCriteriaArray.Add(criteria);
                }

                this.ColorCriteria = colorCriteriaArray;
            }

            var configStore = ConfigurationUnitStore.DefaultStore;
            var searchAndList = configStore.SearchAndListByName(this.ConfigName);
            var fieldControl = (searchAndList != null
                ? configStore.FieldControlByNameFromGroup("List", searchAndList.FieldGroupName)
                : null) ?? configStore.FieldControlByNameFromGroup("List", this.ConfigName);

            if (fieldControl == null)
            {
                return;
            }

            var fieldsWithName = fieldControl.FunctionNames();
            this.DateField = fieldsWithName.ValueOrDefault("Date");
            this.TimeField = fieldsWithName.ValueOrDefault("Time");
            this.EndDateField = fieldsWithName.ValueOrDefault("EndDate");
            this.EndTimeField = fieldsWithName.ValueOrDefault("EndTime");

            if (dateFieldIndex == 0 && this.DateField != null)
            {
                dateFieldIndex = this.DateField.FieldId;
            }
        }

        /// <summary>
        /// Gets the color2 string.
        /// </summary>
        /// <value>
        /// The color2 string.
        /// </value>
        public string Color2String { get; private set; }

        /// <summary>
        /// Gets the color criteria.
        /// </summary>
        /// <value>
        /// The color criteria.
        /// </value>
        public List<ConfigTimelineCriteria> ColorCriteria { get; private set; }

        /// <summary>
        /// Gets the color string.
        /// </summary>
        /// <value>
        /// The color string.
        /// </value>
        public string ColorString { get; private set; }

        /// <summary>
        /// Gets the name of the configuration.
        /// </summary>
        /// <value>
        /// The name of the configuration.
        /// </value>
        public string ConfigName { get; private set; }

        /// <summary>
        /// Gets the date field.
        /// </summary>
        /// <value>
        /// The date field.
        /// </value>
        public UPConfigFieldControlField DateField { get; private set; }

        /// <summary>
        /// Gets the end date field.
        /// </summary>
        /// <value>
        /// The end date field.
        /// </value>
        public UPConfigFieldControlField EndDateField { get; private set; }

        /// <summary>
        /// Gets the end time field.
        /// </summary>
        /// <value>
        /// The end time field.
        /// </value>
        public UPConfigFieldControlField EndTimeField { get; private set; }

        /// <summary>
        /// Gets the name of the filter.
        /// </summary>
        /// <value>
        /// The name of the filter.
        /// </value>
        public string FilterName { get; private set; }

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; private set; }

        /// <summary>
        /// Gets the link identifier.
        /// </summary>
        /// <value>
        /// The link identifier.
        /// </value>
        public int LinkId { get; private set; }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text { get; private set; }

        /// <summary>
        /// Gets the time field.
        /// </summary>
        /// <value>
        /// The time field.
        /// </value>
        public UPConfigFieldControlField TimeField { get; private set; }
    }
}
