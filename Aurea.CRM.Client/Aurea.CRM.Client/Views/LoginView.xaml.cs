// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginView.xaml.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Ioan Armenean (Nelutu)
// </author>
// <summary>
//   Login view code behind
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.ObjectModel;
using Aurea.CRM.Client.UI.Resources;
using Aurea.CRM.Core.Session;
using Aurea.CRM.Services.ModelControllers.Organizer;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using Xamarin.Forms.Xaml;

namespace Aurea.CRM.Client.UI.Views
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Aurea.CRM.Client.UI.Common;
    using Core.Configuration;
    using Core.Logging;
    using Core.Messages;
    using Core.OperationHandling;
    using Core.Platform;
    using Core.Services;
    using Core.Session;
    using Xamarin.Forms;

    /// <inheritdoc />
    /// <summary>
    /// Login view code behind
    /// </summary>
    /// <seealso cref="T:Xamarin.Forms.ContentPage" />
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginView : ContentPage
    {
        private const string DefaultServerDefaultValue =
                    @"crmpad://configureserver?identification=update&name=update&url=https://my.update.com/login&networkUsername=&networkPassword=&defaultErrorReportingEmailAddress=&userAgent=&authenticationType=revolution";

        private static readonly string TextSelectServer = "Select server";
        
        private string userName;
        private string password;
        private bool isSettingsPopupVisible;
        private RemoteServer selectedServer;
        private bool offlineMode;
        private bool isPageBusy;
        private bool isSelectingServer;
        private GridLength loginControlsWidth;
        private bool isLoginVisible;
        private bool isViewVisible;
        private bool isLoading;

        /// <summary>
        /// Gets or sets a value indicating whether [enable offline mode].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable offline mode]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableOfflineMode
        {
            get { return this.offlineMode; }

            set { this.offlineMode  = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether dropdown list is shown.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [dropdown list is shown]; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelectingServer
        {
            get { return this.isSelectingServer; }

            set { this.isSelectingServer = value; }
        }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName
        {
            get { return this.userName; }
            set { this.userName = value; }
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password
        {
            get { return this.password; }
            set { this.password = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the login controls are visible.
        /// </summary>
        public bool IsLoginVisible
        {
            get { return this.isLoginVisible; }
            set { this.isLoginVisible = value; }
        }

        /// <summary>
        /// Gets  a value indicating whether the insight board is visible or not.
        /// </summary>
        public bool IsInsightBoardVisible
        {
            get => !this.IsSelectingServer;
        }

        /// <summary>
        /// Gets the width of the login controls
        /// </summary>
        public GridLength LoginControlsWidth
        {
            get => this.loginControlsWidth;
        }

        /// <summary>
        /// Gets or sets the default server url
        /// </summary>
        public string DefaultServerFromConfig { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the default server is the one to be used
        /// </summary>
        public bool ForceDefaultServer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the default server is to be shown just once
        /// </summary>
        public bool RegisterFirst { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is page busy.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is page busy; otherwise, <c>false</c>.
        /// </value>
        public bool IsPageBusy
        {
            get
            {
                return this.isPageBusy;
            }
            set
            {
                this.isPageBusy = value;
                this.SignInCommand.RaiseCanExecuteChanged();
                this.OnPropertyChanged(nameof(this.IsFormEnabled));
                
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is page busy.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is page busy; otherwise, <c>false</c>.
        /// </value>
        public bool IsLoading
        {
            get
            {
                return this.isLoading;
            }

            set
            {
                this.isLoading = value;
            }
        }

        /// <summary>
        /// Gets the servers.
        /// </summary>
        /// <value>
        /// The servers.
        /// </value>
        public ObservableCollection<RemoteServer> Servers { get; } = new ObservableCollection<RemoteServer>();

        /// <summary>
        /// Gets or sets the selected server.
        /// </summary>
        /// <value>
        /// The selected server.
        /// </value>
        public RemoteServer SelectedServer
        {
            get
            {
                return this.selectedServer;
            }

            set
            {
                if (this.selectedServer != value)
                {
                    this.UserName = string.Empty;
                    this.Password = string.Empty;
                }

                this.selectedServer = value;
                this.SignInCommand.RaiseCanExecuteChanged();

                this.IsSelectingServer = false;
                this.OnPropertyChanged(nameof(this.SelectedServerString));
            }
        }

        /// <summary>
        /// Gets the selected server identification or placeholder if no server selected.
        /// </summary>
        /// <value>
        /// The selected server identification.
        /// </value>
        public string SelectedServerString => this.SelectedServer?.ServerIdentification ?? TextSelectServer;

        /// <summary>
        /// Gets or sets a value indicating whether settings popup is visible .
        /// </summary>
        /// <value>
        /// true if settings popup visible.
        /// </value>
        public bool IsSettingsPopupVisible
        {
            get
            {
                return this.isSettingsPopupVisible;
            }

            set
            {
                this.isSettingsPopupVisible = value;
                this.OnPropertyChanged(nameof(this.IsFormEnabled));
            }
        }

        /// <summary>
        /// Gets a value indicating whether form is enabled.
        /// </summary>
        public bool IsFormEnabled => !this.IsSettingsPopupVisible && !this.IsPageBusy;

        /// <summary>
        /// Gets the login command.
        /// </summary>
        /// <value>
        /// The login command.
        /// </value>
        public Command SignInCommand { get; }

        ///// <summary>
        ///// Gets the InsightBoard group
        ///// </summary>
        //public GUIInsightBoardGroup InsightBoard { get; }

        /// <summary>
        /// Gets the show dropdown command.
        /// </summary>
        /// <value>
        /// The show dropdown command.
        /// </value>
        public Command ShowDropDownCommand =>
            new Command(() => this.IsSelectingServer = !this.IsSelectingServer);

        /// <summary>
        /// Gets the application name.
        /// </summary>
        /// <value>
        /// The application name.
        /// </value>
        public string ApplicationName => "Aurea CRM Client";

        ///// <summary>
        ///// Gets the application version.
        ///// </summary>
        ///// <value>
        ///// The application version.
        ///// </value>
        //public string ApplicationVersion =>
        //    SimpleIoc.Default.GetInstance<IDeviceInfoService>().GetApplicationVersion();

        /// <summary>
        /// Gets the password placeholder text.
        /// </summary>
        //public string TextPlaceholderPassword => AppResources.Login_2;
        public string TextPlaceholderPassword => AppResources.Login_2;

        /// <summary>
        /// Gets the username placeholder text.
        /// </summary>
        public string TextPlaceholderUsername => AppResources.Login_1;

        /// <summary>
        /// Gets the sign in button text.
        /// </summary>
        public string TextSignIn => AppResources.Login_32;

        /// <summary>
        /// Gets a value indicating whether or not the view is visible
        /// </summary>
        public bool IsViewVisible
        {
            get => this.isViewVisible;
            set { this.isViewVisible = value; }
        }

        /// <summary>
        /// Server session did perform login.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="passwordChanged">
        /// The password change result.
        /// </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task ServerSessionDidPerformLogin(ServerSession session, PasswordChangeResult passwordChanged)
        {
            //this.logProvider?.LogInfo("LoginViewModel - login performed");

            //Messenger.Default.Send(PageBusyMessage.EndWorkMessage);
            try
            {
                SimpleIoc.Default.GetInstance<MainPageViewModel>().LoginPerformed();
            }
            catch (InvalidOperationException exception)
            {
                //this.logProvider?.LogError(exception);
                var errorMessageTitle = LocalizedString.Localize(
                    LocalizationKeys.TextGroupLogin,
                    LocalizationKeys.KeyLoginErrorTitleGeneral);
                var errorMessage = LocalizedString.Localize(
                    LocalizationKeys.TextGroupLogin,
                    LocalizationKeys.KeyLoginErrorMessageFullSyncRequired);
                //await this.dialogService.ShowMessage(errorMessage, errorMessageTitle);
                throw;
            }

            if (!string.IsNullOrWhiteSpace(session.UserName))
            {
                this.UserName = session.UserName;
            }

            await ServerManager.DefaultManager.SaveLastServerToFile(session.CrmServer.Name, this.UserName);
            this.IsLoading = false;
        }

        /// <summary>
        /// Gets called when login performed in login page
        /// </summary>
        public void LoginPerformed()
        {
            UPMultipleOrganizerManager.CurrentOrganizerManager.TheDelegate = this;

            // Initialize the framework
            UPMultipleOrganizerManager.CurrentOrganizerManager.ApplicationModelController.Build();
            UPMultipleOrganizerManager.CurrentOrganizerManager.OrganizerBarDelegate = this;
            this.AppOrganizerModelController = UPMultipleOrganizerManager.CurrentOrganizerManager.ApplicationModelController;
            this.AppOrganizerModelController.StartOrganizerModelController.ModelControllerDelegate = App.Locator.DashboardVm;
            // Publish the login event
            //this.MessengerInstance.Send(LoginEventMessage.LoginMessage);
            this.OnShowHomePageCommand();
        }

        /// <summary>
        /// Servers the session did fail login with error password changed.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="passwordChanged">
        /// The password changed result.
        /// </param>
        public void ServerSessionDidFailLogin(ServerSession session, Exception error, PasswordChangeResult passwordChanged)
        {
            this.logProvider?.LogError(error);

            Messenger.Default.Send(PageBusyMessage.EndWorkMessage);
            string errMessage = null;
            string errorMessageTitle = null;
            var offlineError = true;
            if (error != null)
            {
                switch (error.Message)
                {
                    case "LocalDatabaseLanguageMissing":
                    case "LocalDatabaseMissing":
                        errMessage = LocalizedString.Localize(
                            LocalizationKeys.TextGroupLogin,
                            LocalizationKeys.KeyLoginErrorMessageNoOfflineDataAvailable);
                        errorMessageTitle = LocalizedString.Localize(
                            LocalizationKeys.TextGroupLogin,
                            LocalizationKeys.KeyLoginErrorTitleLoginFailed);
                        break;

                    case "LocalFullSyncRequired":
                        errorMessageTitle = LocalizedString.Localize(
                            LocalizationKeys.TextGroupLogin,
                            LocalizationKeys.KeyLoginErrorTitleMissingInternetConnection);
                        errMessage = LocalizedString.Localize(
                            LocalizationKeys.TextGroupLogin,
                            LocalizationKeys.KeyLoginErrorMessageFullSyncRequired);
                        break;

                    case "LicensingFailed":
                        errMessage = LocalizedString.Localize(
                            LocalizationKeys.TextGroupLogin,
                            LocalizationKeys.KeyLoginErrorMissingLicense);
                        break;

                    case "NointernetConnection":
                        errMessage = LocalizedString.Localize(
                            LocalizationKeys.TextGroupLogin,
                            LocalizationKeys.KeyLoginErrorMessageMissingInternetConnection);
                        break;

                    case "OfflineModeRejected":
                        errMessage = LocalizedString.Localize(
                            LocalizationKeys.TextGroupLogin,
                            LocalizationKeys.KeyLoginErrorMessageOfflineModeRejected);
                        break;

                    case "OnlineLoginFailed":
                    case "LocalPasswordIncorrect":
                        errMessage = LocalizedString.Localize(
                            LocalizationKeys.TextGroupLogin,
                            LocalizationKeys.KeyLoginErrorMessageCheckLoginData);
                        errorMessageTitle = LocalizedString.Localize(
                            LocalizationKeys.TextGroupLogin,
                            LocalizationKeys.KeyLoginErrorTitleLoginFailed);
                        break;

                    case "ServerStarting":
                        errMessage = LocalizedString.Localize(
                            LocalizationKeys.TextGroupLogin,
                            LocalizationKeys.KeyLoginErrorMessageServerApplicationCurrentlyStarting);
                        errorMessageTitle = LocalizedString.Localize(
                            LocalizationKeys.TextGroupLogin,
                            LocalizationKeys.KeyLoginErrorTitleLoginNotPossible);
                        break;

                    case "SyncFailed":
                        errMessage = LocalizedString.TextErrorMessageErrorOccuredDuringSync;
                        errorMessageTitle = LocalizedString.TextErrorTitleSyncFailed;
                        break;

                    case "PasswordExpired":
                        errMessage = LocalizedString.TextPasswordExpired;
                        errorMessageTitle = LocalizedString.TextChangePassword;
                        break;
                }
            }
            else
            {
                offlineError = false;
                errMessage = LocalizedString.Localize(
                    LocalizationKeys.TextGroupLogin,
                    LocalizationKeys.KeyLoginErrorMessageGeneral);
            }

            if (offlineError && errorMessageTitle == null)
            {
                errorMessageTitle = LocalizedString.Localize(
                    LocalizationKeys.TextGroupLogin,
                    LocalizationKeys.KeyLoginErrorTitleMissingInternetConnection);
            }
            else if (errorMessageTitle == null)
            {
                errorMessageTitle = LocalizedString.Localize(
                    LocalizationKeys.TextGroupLogin,
                    LocalizationKeys.KeyLoginErrorTitleGeneral);
            }

            if (this.MessengerInstance != null)
            {
                var message = new ToastrMessage
                {
                    MessageText = errorMessageTitle,
                    DetailedMessage = errMessage
                };

                this.MessengerInstance.Send(message);
            }
            this.IsLoading = false;
        }

        /// <summary>
        /// Server session requires newer client version.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="passwordChanged">
        /// The password change result.
        /// </param>
        public async void ServerSessionRequiresNewerClientVersion(ServerSession session, PasswordChangeResult passwordChanged)
        {
            this.logProvider?.LogInfo("LoginViewModel - server session requires new version");

            Messenger.Default.Send(PageBusyMessage.EndWorkMessage);

            if (this.dialogService != null)
            {
                await this.dialogService.ShowMessage(
                    LocalizedString.Localize(
                        LocalizationKeys.TextGroupLogin,
                        LocalizationKeys.KeyLoginInvalidClientVersionMessage),
                    string.Empty);
            }
        }

        /// <summary>
        /// Servers the operation manager requires language for session.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="availableServerLanguages">
        /// The available server languages.
        /// </param>
        /// <param name="sessionAttributes">
        /// The session attributes.
        /// </param>
        public void ServerOperationManagerRequiresLanguage(
            ServerSession session,
            List<ServerLanguage> availableServerLanguages,
            Dictionary<string, object> sessionAttributes)
        {
            this.logProvider?.LogInfo("LoginViewModel - requires language");

            Messenger.Default.Send(PageBusyMessage.EndWorkMessage);
            this.navigationService.NavigateTo(nameof(SyncLanguageSelectorView), false, true);
        }

        /// <summary>
        /// Servers the session warning show warn message text.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        public async void ServerSessionWarningShowWarnMessageText(ServerSession session, string text)
        {
            this.logProvider?.LogInfo($"LoginViewModel - session warning {text}.");

            Messenger.Default.Send(PageBusyMessage.EndWorkMessage);

            if (this.dialogService != null)
            {
                await this.dialogService.ShowMessage(text, string.Empty);
            }
        }

        /// <summary>
        /// Updates list of server, puts "Select server" placeholder as a first item.
        /// </summary>
        public async void UpdateServerList()
        {
            var servers = await ServerService.GetServerListAsync();
            this.Servers.Clear();
            foreach (var server in servers)
            {
                this.Servers.Add(server);
            }

            this.RaisePropertyChanged(() => this.Servers);
            this.ResetStateOfInputBoxes();
        }

        /// <summary>
        /// Resets the state of input boxes.
        /// </summary>
        public void ResetStateOfInputBoxes()
        {
            this.SelectedServer =
                this.Servers.FirstOrDefault(a => a.Name == ServerManager.DefaultManager.LastServer?.Item1) ??
                this.Servers.FirstOrDefault();
            this.UserName = $"{ServerManager.DefaultManager.LastServer?.Item2}";
            this.Password = string.Empty;
        }

        /// <summary>
        /// Adds the default server if available
        /// </summary>
        /// <returns>Task representing this method</returns>
        public async Task ManageDefaultServer()
        {
            var platformSerice = SimpleIoc.Default.GetInstance<IPlatformService>();

            var iniConfig = new IniConfig(await platformSerice.StorageProvider.GetConfigdataAsync());

            this.DefaultServerFromConfig = iniConfig.GetValue("DefaultServer", "com.update.CRMpad.defaultServer");
            this.ForceDefaultServer = iniConfig.GetValue("DefaultServer", "com.update.CRMpad.defaultServer.force").ToUpper().In("TRUE", "1");
            this.RegisterFirst = iniConfig.GetValue("DefaultServer", "com.update.CRMpad.defaultServer.registerFirst").ToUpper().In("TRUE", "1");

            if (ServerManager.DefaultManager.IsServerListLoaded != null)
            {
                await ServerManager.DefaultManager.IsServerListLoaded;
            }

            if (this.RegisterFirst)
            {
                this.AddDefaultServer();
            }
        }

        /// <summary>
        /// Tried to auto-Login auser
        /// </summary>
        /// <param name="initialUserName">Username who should be logged in</param>
        public async void TryAutoLogin(string initialUserName)
        {
            this.logProvider?.LogInfo("LoginViewModel - trying auto login.");

            var manager = ServerManager.DefaultManager;
            var server = manager.ServerByName(this.SelectedServer?.Name);

            var account = await ServerAccount.AccountForServer(server, initialUserName, this.Password, null, false);
            var languageKey = account?.LastUsedLanguageKey;

            if (account?.AutoLoginPassword != null && !string.IsNullOrEmpty(languageKey))
            {
                var session = await ServerSession.Create(server, initialUserName, string.Empty, null, this, this.EnableOfflineMode);
                session.PerformLogin();
            }
        }

        /// <inheritdoc />
        public override void OnAppearing(Type newPageType)
        {
            base.OnAppearing(newPageType);
            Messenger.Default.Register<SettingsPopupMessage>(this, m => { this.IsSettingsPopupVisible = m.IsOpen; });
            Messenger.Default.Register<ToggleOfflineModeMessage>(this, m => { this.EnableOfflineMode = m.Enabled; });
            Messenger.Default.Register<PageBusyMessage>(this, m =>
            {
                if (m.IsBusy.HasValue)
                {
                    this.IsPageBusy = m.IsBusy.Value;
                }
            });
            this.navigationService.UpdateTitle("");
            this.SetControlsWidth();
            this.IsViewVisible = true;
        }

        /// <inheritdoc />
        public override void OnDisappearing(Type oldType)
        {
            base.OnDisappearing(oldType);
            Messenger.Default.Unregister(this);
            this.IsViewVisible = false;
        }

        /// <inheritdoc />
        public override void OnResize()
        {
            base.OnResize();
            this.SetControlsWidth();
        }

        /// <summary>
        /// Set controls width according screen size
        /// </summary>
        public void AdjustControlsAccordingScreen()
        {
            this.IsLoginVisible = false;
            var maximumControlsWidth = this.themeService.GetStyle<double>("XfLoginControlsMaximumWidth");
            var screenWidth = this.deviceInfoService.GetApplicationScreenWidth();
            this.loginControlsWidth = screenWidth > (int)(maximumControlsWidth * 1.1)
                ? new GridLength(maximumControlsWidth, GridUnitType.Absolute)
                : new GridLength(100, GridUnitType.Star);
            this.RaisePropertyChanged(nameof(this.LoginControlsWidth));
            this.IsLoginVisible = true;
        }

        /// <summary>
        /// Starts the session command.
        /// </summary>
        private async void OnSignInCommandAsync()
        {
            this.logProvider?.LogInfo("LoginViewModel - starting session.");
            this.deviceInfoService?.SetAutomaticScreenLock(false);
            this.IsLoading = true;

            // Register a session
            var manager = ServerManager.DefaultManager;
            RemoteServer server = null;
            if (this.SelectedServer != null)
            {
                var currentlySelectedServer = this.SelectedServer;
                if (this.ForceDefaultServer)
                {
                    var defaultServer = this.AddDefaultServer();
                    if (!defaultServer.IsEquivalent(currentlySelectedServer))
                    {
                        await this.dialogService.ShowMessage(
                            LocalizedString.Localize(
                                LocalizationKeys.TextGroupLogin,
                                LocalizationKeys.KeyLoginSelectDefaultServerMessage),
                            LocalizedString.Localize(
                                LocalizationKeys.TextGroupLogin,
                                LocalizationKeys.KeyLoginSelectDefaultServerTitle),
                            LocalizedString.Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicOK),
                            null);
                        return;
                    }
                }

                server = manager.ServerByName(this.SelectedServer?.Name);
            }
            else if (!this.RegisterFirst)
            {
                if (this.ForceDefaultServer)
                {
                    await this.dialogService.ShowMessage(
                        LocalizedString.Localize(
                            LocalizationKeys.TextGroupLogin,
                            LocalizationKeys.KeyLoginSelectDefaultServerMessage),
                        LocalizedString.Localize(
                            LocalizationKeys.TextGroupLogin,
                            LocalizationKeys.KeyLoginSelectDefaultServerTitle),
                        LocalizedString.Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicOK),
                        null);
                }

                this.AddDefaultServer();
                return;
            }

            if (server == null)
            {
                return;
            }

            Messenger.Default.Send(PageBusyMessage.StartWorkMessage);

            var mainPageViewModel = App.Locator.MainPageVm;
            CultureInfo currentClientCulture;
            if (mainPageViewModel != null)
            {
                currentClientCulture = SimpleIoc.Default.GetInstance<MainPageViewModel>().CurrentCulture;
            }
            else
            {
                currentClientCulture = null;
            }

            var session = await ServerSession.Create(server, this.UserName, this.Password, null, this, this.EnableOfflineMode, currentClientCulture);

            session.PerformLogin();
        }

        private RemoteServer AddDefaultServer()
        {
            var defultServerUrl = this.DefaultServerFromConfig;
            if (string.IsNullOrWhiteSpace(defultServerUrl))
            {
                defultServerUrl = DefaultServerDefaultValue;
            }

            var uri = new Uri(defultServerUrl);
            this.logProvider.LogInfo($"Adding Server URL {uri}.");
            var remoteServer = RemoteServer.CreateRemoteServerFromUriData(uri);

            var sameServer =
                ServerManager.DefaultManager.AvailableServers.FirstOrDefault(s => s.IsEquivalent(remoteServer));
            if (sameServer == null)
            {
                ServerManager.DefaultManager.RemoveByName(remoteServer.Name);

                ServerManager.DefaultManager.Add(remoteServer);
                Task.Run(async () =>
                    await ServerManager.DefaultManager.SaveLastServerToFile(remoteServer.Name, string.Empty));

                this.UpdateServerList();

                this.logProvider.LogInfo($"Server {remoteServer.Name} added.");
            }
            else
            {
                return sameServer;
            }

            return remoteServer;
        }

        private void SetControlsWidth()
        {
            try
            {
                Device.BeginInvokeOnMainThread(() => { this.AdjustControlsAccordingScreen(); });
            }
            catch (Exception ex)
            {
                this.logProvider?.LogError(ex);
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginView"/> class.
        /// </summary>
        public LoginView()
        {
            try
            {
                this.InitializeComponent();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        protected override async void OnAppearing()
        {
            try
            {
                //if (Device.RuntimePlatform == Device.iOS)
                //{
                //    App.Locator.MainPageVm.SafeAreaInsets = On<Xamarin.Forms.PlatformConfiguration.iOS>().SafeAreaInsets();
                //}
                //App.Locator.MainPageVm.IsLoginPopupButtonHidden = false;
                //this.CheckAzureADLogin();
                //OpenAzurePage();
            }
            catch (Exception)
            {

            }
        }

        private void ComboBox_OnUnFocused(object sender, FocusEventArgs e)
        {
            //this.model.IsSelectingServer = false;
            //CheckAzureADLogin();
        }

        /// <summary>
        /// Prevent default back button
        /// </summary>
        /// <returns></returns>
        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            return true;
        }

        private void PasswordEntry_OnCompleted(object sender, EventArgs e)
        {
            this.LoginButton.Command.Execute(null);
        }

        private void UserNameEntry_OnCompleted(object sender, EventArgs e)
        {
            this.PasswordEntry.Focus();
        }
    }
}
