// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewReference.cs" company="Aurea Software Gmbh">
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
//   Implements the view reference class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Aurea.CRM.Core.Extensions;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Implements the view reference class
    /// </summary>
    public class ViewReference
    {
        /// <summary>
        /// The back view reference
        /// </summary>
        private static ViewReference backViewReference = null;

        /// <summary>
        /// The definition
        /// </summary>
        private List<object> def;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewReference"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        public ViewReference(List<object> definition, string name)
        {
            this.ViewName = (string)definition[1];
            var parameterDefs = (definition[2] as JArray)?.ToObject<List<object>>();
            if (parameterDefs != null)
            {
                this.Arguments = new Dictionary<string, ReferenceArgument>();

                // var arg = new ReferenceArgument(parameters);
                foreach (var paramDef in parameterDefs)
                {
                    var parameters = (paramDef as JArray)?.ToObject<List<string>>();
                    var arg = new ReferenceArgument(parameters);
                    if (!string.IsNullOrWhiteSpace(arg.Name))
                    {
                        this.Arguments[arg.Name] = arg;
                    }
                }
            }

            this.Name = name;
            this.def = definition;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewReference"/> class.
        /// </summary>
        /// <param name="viewReference">
        /// The view reference.
        /// </param>
        /// <param name="currentContext">
        /// The current context.
        /// </param>
        /// <param name="addValues">
        /// The add values.
        /// </param>
        /// <param name="yieldAddValues">
        /// if set to <c>true</c> [yield add values].
        /// </param>
        public ViewReference(
            ViewReference viewReference,
            Dictionary<string, string> currentContext,
            Dictionary<string, object> addValues,
            bool yieldAddValues)
        {
            this.ViewName = viewReference.ViewName;
            this.Name = viewReference.Name;
            if (currentContext != null)
            {
                this.Arguments = new Dictionary<string, ReferenceArgument>();
                if (viewReference.Arguments != null)
                {
                    foreach (var arg in viewReference.Arguments.Values)
                    {
                        var currentContextValue = currentContext.ValueOrDefault(arg.Value);
                        if (currentContextValue != null)
                        {
                            this.Arguments[arg.Name] = new ReferenceArgument(arg, currentContextValue);
                        }
                        else
                        {
                            this.Arguments[arg.Name] = arg;
                        }
                    }
                }
            }
            else
            {
                this.Arguments = new Dictionary<string, ReferenceArgument>(viewReference.Arguments);
            }

            if (addValues != null)
            {
                foreach (string key in addValues.Keys)
                {
                    ReferenceArgument arg = this.Arguments.ValueOrDefault(key);
                    if (!yieldAddValues || string.IsNullOrWhiteSpace(arg?.Value))
                    {
                        this.Arguments[key] = new ReferenceArgument(key, addValues[key] as string);
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewReference"/> class.
        /// </summary>
        /// <param name="viewReference">
        /// The view reference.
        /// </param>
        /// <param name="currentContext">
        /// The current context.
        /// </param>
        public ViewReference(ViewReference viewReference, Dictionary<string, string> currentContext)
            : this(viewReference, currentContext, null, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewReference"/> class.
        /// </summary>
        /// <param name="dictionary">
        /// The _dictionary.
        /// </param>
        /// <param name="viewName">
        /// Name of the _view.
        /// </param>
        public ViewReference(Dictionary<string, object> dictionary, string viewName)
        {
            this.ViewName = viewName;
            this.Arguments = new Dictionary<string, ReferenceArgument>();
            if (dictionary == null)
            {
                return;
            }

            foreach (var key in dictionary?.Keys)
            {
                this.Arguments[key] = new ReferenceArgument(key, (string) dictionary.ValueOrDefault(key));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewReference"/> class.
        /// </summary>
        /// <param name="viewReference">
        /// The view reference.
        /// </param>
        /// <param name="valueDictionary">
        /// The value dictionary.
        /// </param>
        /// <param name="nilWithOpenValues">
        /// if set to <c>true</c> [nil with open values].
        /// </param>
        public ViewReference(ViewReference viewReference, Dictionary<string, object> valueDictionary, bool nilWithOpenValues)
        {
            this.ViewName = viewReference.ViewName;
            this.Name = viewReference.Name;
            this.Arguments = new Dictionary<string, ReferenceArgument>();
            foreach (var key in viewReference.Arguments.Keys)
            {
                var arg = viewReference.Arguments.ValueOrDefault(key);
                var replaceValue = valueDictionary.ValueOrDefault(arg.Value) as string;
                if (!string.IsNullOrWhiteSpace(replaceValue))
                {
                    this.Arguments[key] = new ReferenceArgument(key, replaceValue);
                }
                else if (nilWithOpenValues && arg.Value.StartsWith("$"))
                {
                    return;
                }
                else
                {
                    this.Arguments[key] = arg;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewReference"/> class.
        /// </summary>
        /// <param name="viewReference">
        /// The view reference.
        /// </param>
        /// <param name="oldValue">
        /// The old value.
        /// </param>
        /// <param name="newValue">
        /// The new value.
        /// </param>
        /// <param name="searchKey">
        /// The search key.
        /// </param>
        public ViewReference(ViewReference viewReference, string oldValue, string newValue, string searchKey)
        {
            this.ViewName = viewReference.ViewName;
            this.Name = viewReference.Name;
            this.Arguments = new Dictionary<string, ReferenceArgument>();
            var replaceByValue = string.IsNullOrWhiteSpace(searchKey);
            var found = false;

            foreach (var key in viewReference.Arguments.Keys)
            {
                var arg = viewReference.Arguments[key];
                if (replaceByValue && arg.Value == oldValue)
                {
                    this.Arguments[key] = new ReferenceArgument(key, newValue);
                }
                else if (!replaceByValue && arg.Name == searchKey)
                {
                    this.Arguments[key] = new ReferenceArgument(searchKey, newValue);
                    found = true;
                }
                else
                {
                    this.Arguments[key] = arg;
                }
            }

            if (!found && !string.IsNullOrWhiteSpace(searchKey))
            {
                this.Arguments[searchKey] = new ReferenceArgument(searchKey, newValue);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewReference"/> class.
        /// </summary>
        /// <param name="serialization">
        /// The serialization.
        /// </param>
        /// <param name="viewName">
        /// Name of the view.
        /// </param>
        public ViewReference(string serialization, string viewName)
            : this(serialization.JsonDictionaryFromString(), viewName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewReference"/> class.
        /// </summary>
        /// <param name="serialization">
        /// The serialization.
        /// </param>
        public ViewReference(string serialization)
        {
            var jObject = JsonConvert.DeserializeObject<JObject>(serialization);
            var parameters = jObject?.ParseObject<Dictionary<string, object>>();
            if (parameters == null)
            {
                return;
            }

            this.ViewName = (string)parameters["viewName"];
            if (string.IsNullOrWhiteSpace(this.ViewName))
            {
                return;
            }

            this.Arguments = new Dictionary<string, ReferenceArgument>();
            foreach (var key in parameters.Keys)
            {
                this.Arguments[key] = new ReferenceArgument(key, (string)parameters[key]);
            }
        }

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        /// <value>
        /// The arguments.
        /// </value>
        public Dictionary<string, ReferenceArgument> Arguments { get; }

        /// <summary>
        /// Gets or sets the current record values.
        /// </summary>
        /// <value>
        /// The current record values.
        /// </value>
        public Dictionary<string, object> CurrentRecordValues { get; set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets the name of the view.
        /// </summary>
        /// <value>
        /// The name of the view.
        /// </value>
        public string ViewName { get; }

        /// <summary>
        /// Backs the view reference.
        /// </summary>
        /// <returns>
        /// The <see cref="ViewReference"/>.
        /// </returns>
        public static ViewReference BackViewReference()
        {
            return backViewReference
                   ?? (backViewReference = new ViewReference(new Dictionary<string, object> { { "back", "Action" } }, "OrganizerAction"));
        }

        /// <summary>
        /// Sets the value for key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void SetValueForKey(string key, string value)
        {
            this.Arguments[key] = new ReferenceArgument(key, value);
        }

        /// <summary>
        /// Contexts the value for key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ContextValueForKey(string key)
        {
            var arg = this.Arguments?.ValueOrDefault(key);
            return arg?.Value;
        }

        /// <summary>
        /// Contexts the value is set.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ContextValueIsSet(string key)
        {
            var value = this.ContextValueForKey(key);
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            return value != "0" && value.ToLower() != "false";
        }

        /// <summary>
        /// Contexts the with.
        /// </summary>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public Dictionary<string, string> ContextWith(string recordIdentification)
        {
            return new Dictionary<string, string>
                       {
                           { "uid", recordIdentification },
                           { "recordId", recordIdentification }
                       };
        }

        /// <summary>
        /// Contexts the with.
        /// </summary>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <param name="linkRecordIdentification">
        /// The link record identification.
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public Dictionary<string, string> ContextWith(string recordIdentification, string linkRecordIdentification)
        {
            return new Dictionary<string, string>
                       {
                           { "uid", recordIdentification },
                           { "recordId", recordIdentification },
                           { "linkRecordId", linkRecordIdentification },
                           { "link", linkRecordIdentification }
                       };
        }

        /// <summary>
        /// Determines whether [has back to previous follow up action].
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HasBackToPreviousFollowUpAction()
        {
            return this.ContextValueForKey("defaultFinishAction") == ".backToPrevious";
        }

        /// <summary>
        /// Determines whether [has open parameters] [the specified context values].
        /// </summary>
        /// <param name="contextValues">
        /// The context values.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HasOpenParameters(Dictionary<string, string> contextValues)
        {
            if (this.Arguments?.Values == null)
            {
                return false;
            }

            return this.Arguments.Values.Any(arg => arg.Value.StartsWith("$") && contextValues[arg.Value] == null);
        }

        /// <summary>
        /// Jsons the dictionary for key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public Dictionary<string, object> JsonDictionaryForKey(string key)
        {
            var str = this.ContextValueForKey(key);
            return str?.JsonDictionaryFromString();
        }

        /// <summary>
        /// Parameters the dictionary.
        /// </summary>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public Dictionary<string, object> ParameterDictionary()
        {
            return this.ParameterDictionaryByAppendingDictionary(null);
        }

        /// <summary>
        /// Parameters the dictionary by appending dictionary.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public Dictionary<string, object> ParameterDictionaryByAppendingDictionary(Dictionary<string, object> dictionary)
        {
            var dict = new Dictionary<string, object>(this.Arguments.Count);
            foreach (var arg in this.Arguments.Values)
            {
                if (arg.Value != null)
                {
                    dict[arg.Name] = arg.Value;
                }
            }

            if (dictionary == null || dictionary.Count <= 0)
            {
                return dict;
            }

            foreach (var key in dictionary.Keys)
            {
                dict[key] = dictionary[key];
            }

            return dict;
        }

        /// <summary>
        /// Parameterses the dictionary.
        /// </summary>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public Dictionary<string, object> ParamsDictionary()
        {
            var parameterDictionary = new Dictionary<string, object>();
            for (var i = 1; i <= 9; i++)
            {
                var parameterKey = $"Param{i}";
                var parameterValue = this.ContextValueForKey(parameterKey);
                if (string.IsNullOrWhiteSpace(parameterValue))
                {
                    continue;
                }

                parameterDictionary[parameterKey] = parameterValue;
                var parts = parameterValue.Split(';');
                if (parts.Length < 2)
                {
                    continue;
                }

                for (var j = 1; j <= parts.Length; j++)
                {
                    parameterDictionary[$"Param.{i}.{j}"] = parts[j - 1];
                }
            }

            if (this.CurrentRecordValues != null)
            {
                parameterDictionary =
                    parameterDictionary.Concat(this.CurrentRecordValues.ToDictionary(p => p.Key, p => p.Value))
                        .Distinct()
                        .ToDictionary(k => k.Key, k => k.Value);
            }

            return parameterDictionary.Count > 0 ? parameterDictionary : null;
        }

        /// <summary>
        /// Serializeds this instance.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string Serialized()
        {
            var parameterDictionary = new Dictionary<string, string>();
            foreach (var arg in this.Arguments.Values)
            {
                if (!string.IsNullOrWhiteSpace(arg.Value))
                {
                    parameterDictionary[arg.Name] = arg.Value;
                }
            }

            parameterDictionary["viewName"] = this.ViewName;
            return JsonConvert.SerializeObject(parameterDictionary);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendFormat($"View={this.ViewName} ({this.Name})");
            foreach (var arg in this.Arguments.Values)
            {
                builder.AppendFormat($",{Environment.NewLine}  {arg.Name}={arg.Value}");
            }

            return builder.ToString();
        }

        /// <summary>
        /// Views the reference with.
        /// </summary>
        /// <param name="currentContext">
        /// The current context.
        /// </param>
        /// <returns>
        /// The <see cref="ViewReference"/>.
        /// </returns>
        public ViewReference ViewReferenceWith(Dictionary<string, string> currentContext)
        {
            return new ViewReference(this, currentContext);
        }

        /// <summary>
        /// Views the reference with.
        /// </summary>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <returns>
        /// The <see cref="ViewReference"/>.
        /// </returns>
        public ViewReference ViewReferenceWith(string recordIdentification)
        {
            return this.ViewReferenceWith(recordIdentification, string.Empty);
        }

        /// <summary>
        /// Views the reference with.
        /// </summary>
        /// <param name="currentContext">
        /// The current context.
        /// </param>
        /// <param name="addValues">
        /// The add values.
        /// </param>
        /// <returns>
        /// The <see cref="ViewReference"/>.
        /// </returns>
        public ViewReference ViewReferenceWith(
            Dictionary<string, string> currentContext,
            Dictionary<string, object> addValues)
        {
            return new ViewReference(this, currentContext, addValues, true);
        }

        /// <summary>
        /// Views the reference with.
        /// </summary>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <param name="linkRecordIdentification">
        /// The link record identification.
        /// </param>
        /// <returns>
        /// The <see cref="ViewReference"/>.
        /// </returns>
        public ViewReference ViewReferenceWith(string recordIdentification, string linkRecordIdentification)
        {
            if (recordIdentification == null)
            {
                recordIdentification = string.Empty;
            }

            if (linkRecordIdentification == null)
            {
                linkRecordIdentification = string.Empty;
            }

            return new ViewReference(this, this.ContextWith(recordIdentification, linkRecordIdentification));
        }

        /// <summary>
        /// Views the reference with.
        /// </summary>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <param name="addValues">
        /// The add values.
        /// </param>
        /// <returns>
        /// The <see cref="ViewReference"/>.
        /// </returns>
        public ViewReference ViewReferenceWith(string recordIdentification, Dictionary<string, object> addValues)
        {
            return new ViewReference(this, this.ContextWith(recordIdentification), addValues, true);
        }

        /// <summary>
        /// Views the reference with.
        /// </summary>
        /// <param name="addValues">
        /// The add values.
        /// </param>
        /// <returns>
        /// The <see cref="ViewReference"/>.
        /// </returns>
        public ViewReference ViewReferenceWith(Dictionary<string, object> addValues)
        {
            return new ViewReference(this, null, addValues, true);
        }

        /// <summary>
        /// Views the reference with.
        /// </summary>
        /// <param name="currentContext">
        /// The current context.
        /// </param>
        /// <param name="addValues">
        /// The add values.
        /// </param>
        /// <param name="yieldAddValues">
        /// if set to <c>true</c> [yield add values].
        /// </param>
        /// <returns>
        /// The <see cref="ViewReference"/>.
        /// </returns>
        public ViewReference ViewReferenceWith(
            Dictionary<string, string> currentContext,
            Dictionary<string, object> addValues,
            bool yieldAddValues)
        {
            return new ViewReference(this, currentContext, addValues, yieldAddValues);
        }

        /// <summary>
        /// Views the reference with back to previous follow up action.
        /// </summary>
        /// <returns>
        /// The <see cref="ViewReference"/>.
        /// </returns>
        public ViewReference ViewReferenceWithBackToPreviousFollowUpAction()
        {
            return new ViewReference(this, ".backToPrevious", "defaultFinishAction", null);
        }
    }
}
