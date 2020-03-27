// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogSettings.cs" company="Aurea Software Gmbh">
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
//   Implementation of logger settings interface
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Logging
{
    using System;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.Session;
    using GalaSoft.MvvmLight.Messaging;
    using Messages;
    using Newtonsoft.Json;

    /// <summary>
    /// Implementation of logger settings interface
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Logging.ILogSettings" />
    public class LogSettings : ILogSettings
    {
        /// <summary>
        /// The storage provider.
        /// </summary>
        private readonly ISettingsLoader settingsLoader;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogSettings"/> class.
        /// </summary>
        /// <param name="settingsLoader">The storage provider</param>
        public LogSettings(ISettingsLoader settingsLoader)
        {
            this.settingsLoader = settingsLoader;
            this.LogLevel = LogLevel.Error;

            this.LogConfig = true;
            this.LogNetwork = true;
            this.LogQuestionnaire = true;
            this.LogRequests = true;
            this.LogResults = true;
           // this.LogSerialEntry = true;
            this.LogStatements = true;
         //   this.LogUpSync = true;

            this.Init();
        }

        /// <inheritdoc />
        public string LogClass { get; protected set; }

        /// <inheritdoc />
        public bool LogConfig { get; protected set; }

        /// <inheritdoc />
        [JsonIgnore]
        public LogFlag LogFlags { get; private set; }

        /// <inheritdoc />
        public LogLevel LogLevel { get; set; }

        /// <inheritdoc />
        public string LogMethod { get; protected set; }

        /// <inheritdoc />
        public bool LogNetwork { get; protected set; }

        /// <inheritdoc />
        public bool LogQuestionnaire { get; protected set; }

        /// <inheritdoc/>
        [JsonIgnore]
        public string LogReportEmailAddress { get; protected set; }

        /// <inheritdoc />
        public bool LogRequests { get; protected set; }

        /// <inheritdoc />
        public bool LogResults { get; protected set; }

        /// <inheritdoc />
        public bool LogSerialEntry { get; protected set; }

        /// <inheritdoc />
        public bool LogStatements { get; protected set; }

        /// <inheritdoc />
        public bool LogUpSync { get; protected set; }

        /// <inheritdoc/>
        public bool OverrideServerSettings { get; protected set; }

        /// <summary>
        /// The as boolean setting.
        /// </summary>
        /// <param name="settingStr">
        /// The setting str.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool AsBooleanSetting(string settingStr)
        {
            bool.TryParse(settingStr?.ToLower(), out var result);
            return result;
        }

        /// <inheritdoc/>
        public void UpdateSettingsFromSession(IConfigurationUnitStore configStore)
        {
            this.LogReportEmailAddress = configStore?.ConfigValue("LogReport.EmailAddress") ?? string.Empty;
            if (configStore == null || this.OverrideServerSettings)
            {
                return;
            }

            this.LogClass = configStore.ConfigValue("Log.Class");
            this.LogMethod = configStore.ConfigValue("Log.LogMethod");
            this.LogConfig = this.AsBooleanSetting(configStore.ConfigValue("Log.Config"));
            this.LogNetwork = this.AsBooleanSetting(configStore.ConfigValue("Log.Network"));
            this.LogQuestionnaire = this.AsBooleanSetting(configStore.ConfigValue("Log.Questionnaire"));
            this.LogRequests = this.AsBooleanSetting(configStore.ConfigValue("Log.Requests"));
            this.LogResults = this.AsBooleanSetting(configStore.ConfigValue("Log.Result"));
            this.LogSerialEntry = this.AsBooleanSetting(configStore.ConfigValue("Log.SerialEntry"));
            this.LogStatements = this.AsBooleanSetting(configStore.ConfigValue("Log.Statements"));
            this.LogUpSync = this.AsBooleanSetting(configStore.ConfigValue("Log.UpSync"));
            this.LogFlags = this.GetLogFlags();
            var logLevel = "4"; //configStore.ConfigValue("Log.Level");
            this.LogLevel = LogLevel.Debug;
            if (!string.IsNullOrWhiteSpace(logLevel))
            {
                var val = 1;
                if (int.TryParse(logLevel, out val) && Enum.IsDefined(typeof(LogLevel), val))
                {
                    this.LogLevel = (LogLevel)val;
                }
            }
        }

        /// <summary>
        /// The configure log level from storage.
        /// </summary>
        protected virtual void ConfigureLogLevelFromStorage()
        {
            this.LogLevel = LogLevel.Error;
            if (this.OverrideServerSettings && Enum.TryParse(this.settingsLoader.GetStringFromStorage(nameof(this.LogLevel)), out LogLevel storedLogLevel))
            {
                this.LogLevel = storedLogLevel;
            }
        }

        /// <summary>
        /// The init.
        /// </summary>
        private void Init()
        {
            Messenger.Default.Register<LoginEventMessage>(
                this,
                m =>
                    {
                        if (m.IsLogedIn)
                        {
                            this.UpdateSettingsFromSession(ServerSession.CurrentSession?.ConfigUnitStore);
                        }
                    });
            this.LoadSettings();
        }

        /// <summary>
        /// The load settings.
        /// </summary>
        private void LoadSettings()
        {
            try
            {
                if (this.settingsLoader == null)
                {
                    this.ResetSettings();
                    return;
                }

                this.OverrideServerSettings = this.settingsLoader.GetBoolFromStorage(nameof(this.OverrideServerSettings));
                if (this.OverrideServerSettings)
                {
                    this.LogClass = this.settingsLoader.GetStringFromStorage(nameof(this.LogClass));
                    this.LogMethod = this.settingsLoader.GetStringFromStorage(nameof(this.LogMethod));

                    this.LogConfig = this.settingsLoader.GetBoolFromStorage(nameof(this.LogConfig));
                    this.LogNetwork = this.settingsLoader.GetBoolFromStorage(nameof(this.LogNetwork));
                    this.LogQuestionnaire = this.settingsLoader.GetBoolFromStorage(nameof(this.LogQuestionnaire));
                    this.LogRequests = this.settingsLoader.GetBoolFromStorage(nameof(this.LogRequests));
                    this.LogResults = this.settingsLoader.GetBoolFromStorage(nameof(this.LogResults));
                    this.LogSerialEntry = this.settingsLoader.GetBoolFromStorage(nameof(this.LogSerialEntry));
                    this.LogStatements = this.settingsLoader.GetBoolFromStorage(nameof(this.LogStatements));
                    this.LogUpSync = this.settingsLoader.GetBoolFromStorage(nameof(this.LogUpSync));
                    this.LogFlags = this.GetLogFlags();
                    this.ConfigureLogLevelFromStorage();
                }
                else
                {
                    this.UpdateSettingsFromSession(ServerSession.CurrentSession?.ConfigUnitStore);
                }
            }
            catch (Exception)
            {
                this.ResetSettings();
            }
        }

        /// <summary>
        /// The reset settings.
        /// </summary>
        private void ResetSettings()
        {
            this.LogUpSync = false;
            this.OverrideServerSettings = false;
            this.LogClass = string.Empty;
            this.LogConfig = false;
            this.LogFlags = LogFlag.LogNone;
            this.LogMethod = string.Empty;
            this.LogQuestionnaire = false;
            this.LogNetwork = true;
            this.LogResults = false;
            this.LogSerialEntry = false;
            this.LogStatements = false;
            this.LogRequests = false;
            this.LogLevel = LogLevel.Error;
        }

        /// <summary>
        /// The get log flags.
        /// </summary>
        /// <returns>
        /// The <see cref="LogFlag"/>.
        /// </returns>
        private LogFlag GetLogFlags()
        {
            var result = LogFlag.LogNone;
            if (this.LogConfig)
            {
                result |= LogFlag.LogConfig;
            }

            if (this.LogNetwork)
            {
                result |= LogFlag.LogNetwork;
            }

            if (this.LogQuestionnaire)
            {
                result |= LogFlag.LogQuestionnaire;
            }

            if (this.LogRequests)
            {
                result |= LogFlag.LogRequests;
            }

            if (this.LogResults)
            {
                result |= LogFlag.LogResults;
            }

            if (this.LogSerialEntry)
            {
                result |= LogFlag.LogSerialEntry;
            }

            if (this.LogStatements)
            {
                result |= LogFlag.LogStatements;
            }

            if (this.LogUpSync)
            {
                result |= LogFlag.LogUpSync;
            }

            return result;
        }
    }
}
