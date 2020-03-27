// <copyright file="AnalysisAggregationType.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//   Serdar Tepeyurt
// </author>

namespace Aurea.CRM.Core.Analysis.Model
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Extensions;
    using Value.AnalysisProcessing;

    /// <summary>
    /// Implementation of analysis aggregation type
    /// </summary>
    public class AnalysisAggregationType
    {
        private static List<object> allTypes;
        private static AnalysisAggregationType countType;
        private static AnalysisAggregationType staticType;
        private static AnalysisAggregationType sumType;

        private static Dictionary<string, object> typeDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisAggregationType"/> class.
        /// </summary>
        /// <param name="type">Agggregation type</param>
        public AnalysisAggregationType(string type)
            : this(type, type)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisAggregationType"/> class.
        /// </summary>
        /// <param name="type">Aggregation type</param>
        /// <param name="label">Label</param>
        public AnalysisAggregationType(string type, string label)
        {
            this.Label = label;
            this.Type = type;
            switch (this.Type)
            {
                case "min":
                    this.Min = true;
                    break;

                case "max":
                    this.Max = true;
                    break;

                case "avg":
                    this.Avg = true;
                    break;

                case "static":
                    this.StaticAggregator = true;
                    break;

                default:
                    this.Sum = true;
                    break;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this is average
        /// </summary>
        public bool Avg { get; private set; }

        /// <summary>
        /// Gets label
        /// </summary>
        public string Label { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this is maximum
        /// </summary>
        public bool Max { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this is minimum
        /// </summary>
        public bool Min { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this is static aggregator
        /// </summary>
        public bool StaticAggregator { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this is sum
        /// </summary>
        public bool Sum { get; private set; }

        /// <summary>
        /// Gets type
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// All types
        /// </summary>
        /// <returns>Returns all types</returns>
        public static List<object> All()
        {
            if (allTypes != null)
            {
                return allTypes;
            }

            TypeDictionary();
            return allTypes;
        }

        /// <summary>
        /// Gets count aggregator
        /// </summary>
        /// <returns>Returns count aggregator</returns>
        public static AnalysisAggregationType GetCount()
        {
            return countType ?? (countType = new AnalysisAggregationType("count"));
        }

        /// <summary>
        /// Gets static aggregator
        /// </summary>
        /// <returns>Returns static aggregator</returns>
        public static AnalysisAggregationType GetStaticAggregator()
        {
            return staticType ?? (staticType = new AnalysisAggregationType("static"));
        }

        /// <summary>
        /// Gets sum aggregator
        /// </summary>
        /// <returns>Returns sum aggregator</returns>
        public static AnalysisAggregationType GetSum()
        {
            return sumType ?? (sumType = new AnalysisAggregationType("sum"));
        }

        /// <summary>
        /// Tries to get a named aggregator
        /// </summary>
        /// <param name="typeName">Type name</param>
        /// <returns>Returns requested aggregator if exists</returns>
        public static AnalysisAggregationType WithType(string typeName)
        {
            return TypeDictionary().ValueOrDefault(typeName) as AnalysisAggregationType;
        }

        /// <summary>
        /// Creates value aggregator
        /// </summary>
        /// <returns>Returns value aggregator</returns>
        public AnalysisProcessingValueAggregator CreateValueAggregator()
        {
            if (this.Sum)
            {
                return new AnalysisProcessingSumValueAggregator();
            }
            else if (this.Avg)
            {
                return new AnalysisProcessingAvgValueAggregator();
            }
            else if (this.Max)
            {
                return new AnalysisProcessingMaxValueAggregator();
            }
            else if (this.Min)
            {
                return new AnalysisProcessingMinValueAggregator();
            }
            else if (this.StaticAggregator)
            {
                return new AnalysisProcessingStaticValueAggregator();
            }
            else
            {
                return new AnalysisProcessingValueAggregator();
            }
        }

        /// <inheritdoc/>
        public override string ToString() => this.Type;

        private static Dictionary<string, object> TypeDictionary()
        {
            if (typeDictionary == null)
            {
                AnalysisAggregationType aggregationType;
                var dict = new Dictionary<string, object>();
                aggregationType = GetCount();
                dict.SetObjectForKey(aggregationType, aggregationType.Type);
                var arr = new List<object>();
                aggregationType = GetSum();
                dict.SetObjectForKey(aggregationType, aggregationType.Type);
                arr.Add(aggregationType);
                aggregationType = new AnalysisAggregationType("max");
                dict.SetObjectForKey(aggregationType, aggregationType.Type);
                arr.Add(aggregationType);
                aggregationType = new AnalysisAggregationType("min");
                dict.SetObjectForKey(aggregationType, aggregationType.Type);
                arr.Add(aggregationType);
                aggregationType = new AnalysisAggregationType("avg");
                dict.SetObjectForKey(aggregationType, aggregationType.Type);
                arr.Add(aggregationType);
                aggregationType = GetStaticAggregator();
                dict.SetObjectForKey(aggregationType, "static");
                arr.Add(aggregationType);
                typeDictionary = dict;
                allTypes = arr;
            }

            return typeDictionary;
        }
    }
}
