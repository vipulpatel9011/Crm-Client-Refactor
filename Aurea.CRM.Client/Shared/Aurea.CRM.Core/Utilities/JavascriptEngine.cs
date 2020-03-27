// <copyright file="JavascriptEngine.cs" company="Aurea Software Gmbh">
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

namespace Aurea.CRM.Core.Utilities
{
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Configuration;
    using Jint;
    using Jint.Native;
    //using Microsoft.Practices.ServiceLocation;
    using GalaSoft.MvvmLight.Ioc;
    using Platform;

    /// <summary>
    /// Javascript engine class implementation
    /// </summary>
    public class JavascriptEngine
    {
        private static JavascriptEngine javascriptEngine;
        private Engine jCtx;

        /// <summary>
        /// Initializes a new instance of the <see cref="JavascriptEngine"/> class.
        /// </summary>
        public JavascriptEngine()
        {
            this.jCtx = new Engine();
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            var globalJavascripts = configStore?.ConfigValue("System.JavascriptGlobals");
            if (globalJavascripts?.Length > 0)
            {
                var scriptFiles = globalJavascripts.Split(',');
                foreach (string scriptName in scriptFiles)
                {
                    string fileName = configStore.FileNameForResourceName(scriptName);
                    Task.Run(async () =>
                    {
                        var content = await SimpleIoc.Default.GetInstance<IPlatformService>().StorageProvider.FileContents(fileName);
                        var script = Encoding.UTF8.GetString(content, 0, content.Length);
                        this.jCtx.Execute(script);
                    });
                }
            }
        }

        /// <summary>
        /// Gets current engine
        /// </summary>
        public static JavascriptEngine Current => javascriptEngine ?? (javascriptEngine = new JavascriptEngine());

        /// <summary>
        /// Array for value
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>List of objects</returns>
        public static List<object> ArrayForValue(JsValue value)
        {
            return value.IsArray() ? value.TryCast<List<object>>() : null;
        }

        /// <summary>
        /// Double for object
        /// </summary>
        /// <param name="obj">Object</param>
        /// <returns>Double value</returns>
        public static double DoubleForObject(object obj)
        {
            if (obj is JsValue)
            {
                return DoubleForValue((JsValue)obj);
            }

            return obj as double? ?? 0;
        }

        /// <summary>
        /// Double for value
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Double value</returns>
        public static double DoubleForValue(JsValue value)
        {
            return value.IsNumber() ? value.AsNumber() : 0;
        }

        /// <summary>
        /// Checks if object
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>True if object</returns>
        public static bool IsObject(JsValue value)
        {
            return value.IsObject();
        }

        /// <summary>
        /// Reset javascript engine
        /// </summary>
        public static void ResetJavascriptEngine()
        {
            javascriptEngine = null;
        }

        /// <summary>
        /// String for value
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>String value</returns>
        public static string StringForValue(JsValue value)
        {
            return value.AsString();
        }

        /// <summary>
        /// Context
        /// </summary>
        /// <returns>Returns context</returns>
        public Engine Context()
        {
            return this.jCtx;
        }

        /// <summary>
        /// Function for script
        /// </summary>
        /// <param name="script">Script</param>
        /// <returns>Function handler</returns>
        public JsValue FunctionForScript(string script)
        {
            return this.FunctionForScriptWithParameters(script, "v");
        }

        /// <summary>
        /// Function for script with parameters
        /// </summary>
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Javascript object</returns>
        public JsValue FunctionForScriptWithParameters(string script, string parameters)
        {
            string f = $"var func = function({parameters}) {{return {script}}};";
            this.jCtx.Execute(f);
            var ret = this.jCtx.GetValue("func");
            return ret;
        }

        /// <summary>
        /// Value for double
        /// </summary>
        /// <param name="num">Num</param>
        /// <returns>Value</returns>
        public JsValue ValueForDouble(double num)
        {
            return this.jCtx.GetValue(num);
        }

        /// <summary>
        /// Value for string
        /// </summary>
        /// <param name="str">String</param>
        /// <returns>Value</returns>
        public JsValue ValueForString(string str)
        {
            return this.jCtx.GetValue(str);
        }
    }
}
