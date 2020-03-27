// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorTexts.cs" company="Aurea Software Gmbh">
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
//   Error texts details
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Utilities
{
    using System;
    using System.Collections.Generic;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.Session;

    /// <summary>
    /// Error texts details
    /// </summary>
    public class UPErrorTexts
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPErrorTexts"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public UPErrorTexts(string name)
            : this(name, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPErrorTexts"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="parent">
        /// The parent.
        /// </param>
        public UPErrorTexts(string name, UPErrorTexts parent)
        {
            this.Name = name;
            var errorTextTokenArray = new List<string>();
            var errorTextDictionary = new Dictionary<string, string>();
            var configStore = ConfigurationUnitStore.DefaultStore;
            var configNames = name.Split(';');
            foreach (var configName in configNames)
            {
                var control = configStore.FieldControlByName(configName);
                if (control == null)
                {
                    control = configStore.FieldControlByNameFromGroup("List", configName);
                }

                if (control != null)
                {
                    foreach (UPConfigFieldControlField field in control.Fields)
                    {
                        if (!string.IsNullOrEmpty(field.Function) &&
#if PORTING
                            field.Label != null &&
#endif
                            errorTextDictionary[field.Function] == null)
                        {
#if PORTING
                            errorTextDictionary[field.Function] = field.Label;
                            errorTextTokenArray.Add(field.Function);
#endif
                        }
                    }
                }
            }

            if (parent != null)
            {
                foreach (var func in parent.ErrorTextTokenArray)
                {
                    if (errorTextDictionary[func] == null)
                    {
                        errorTextTokenArray.Add(func);
                        errorTextDictionary[func] = parent.ErrorTextDictionaryForToken[func];
                    }
                }
            }

            this.ErrorTextDictionaryForToken = errorTextDictionary;
            this.ErrorTextTokenArray = errorTextTokenArray;
        }

        /// <summary>
        /// Gets the error text dictionary for token.
        /// </summary>
        /// <value>
        /// The error text dictionary for token.
        /// </value>
        public Dictionary<string, string> ErrorTextDictionaryForToken { get; private set; }

        /// <summary>
        /// Gets the error text token array.
        /// </summary>
        /// <value>
        /// The error text token array.
        /// </value>
        public List<string> ErrorTextTokenArray { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Defaults the error texts.
        /// </summary>
        /// <returns>
        /// The <see cref="UPErrorTexts"/>.
        /// </returns>
        public static UPErrorTexts DefaultErrorTexts()
        {
            return ServerSession.CurrentSession.CustomDefaultErrorTexts();
        }

        /// <summary>
        /// Errors the name of the text with.
        /// </summary>
        /// <param name="errorTextName">
        /// Name of the error text.
        /// </param>
        /// <returns>
        /// The <see cref="UPErrorTexts"/>.
        /// </returns>
        public static UPErrorTexts ErrorTextWithName(string errorTextName)
        {
            return ServerSession.CurrentSession.CustomErrorTextsWithName(errorTextName);
        }

        /// <summary>
        /// Translates the error.
        /// </summary>
        /// <param name="errorText">
        /// The error text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string TranslateError(string errorText)
        {
            foreach (var key in this.ErrorTextTokenArray)
            {
                var parts = key.Split(';');
                if (parts.Length < 2)
                {
                    if (errorText.IndexOf(key, StringComparison.Ordinal) > 0)
                    {
                        return this.ErrorTextDictionaryForToken[key];
                    }
                }
                else
                {
                    var found = true;
                    foreach (var part in parts)
                    {
                        if (errorText.IndexOf(part, StringComparison.Ordinal) < 0)
                        {
                            found = false;
                            break;
                        }
                    }

                    if (found)
                    {
                        return this.ErrorTextDictionaryForToken[key];
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Translates the error description.
        /// </summary>
        /// <param name="errorText">
        /// The error text.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string TranslateErrorDescription(string errorText, string description)
        {
            return string.IsNullOrEmpty(description)
                       ? this.TranslateError(errorText)
                       : this.TranslateError(
                           string.IsNullOrWhiteSpace(errorText) ? description : $"{errorText}{description}");
        }
    }
}
