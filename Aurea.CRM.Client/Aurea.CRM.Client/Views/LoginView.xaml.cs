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
namespace Aurea.CRM.Client.UI.Views
{
    //using Aurea.CRM.Client.UI.Common;
    //using Aurea.CRM.Client.UI.ViewModel;
 //   using Microsoft.Identity.Client;
    using System;
    using Xamarin.Forms;
    using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
    using Xamarin.Forms.Xaml;

    /// <inheritdoc />
    /// <summary>
    /// Login view code behind
    /// </summary>
    /// <seealso cref="T:Xamarin.Forms.ContentPage" />
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginView : ContentPage
    {
        private LoginViewModel model;
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginView"/> class.
        /// </summary>
        public LoginView()
        {
            this.InitializeComponent();
            this.model = App.Locator.LoginVm;
            this.BindingContext = this.model;
            this.model.UpdateServerList();
            if (this.model.InsightBoard != null)
            {
                this.InsightBoard.Children.Add(this.model.InsightBoard);
            }

            this.UserNameEntry.Completed += (object sender, EventArgs e) => { this.PasswordEntry.Focus(); };
            this.PasswordEntry.Completed += (object sender, EventArgs e) =>
            {
                this.LoginButton.Command.Execute(null);
            };


            //AzureConstants.PublicClientApp = PublicClientApplicationBuilder.Create(AzureConstants.ApplicationID)
            //    .WithTenantId(AzureConstants.TenantId)
            //    .WithIosKeychainSecurityGroup(AzureConstants.IOSKeyChainGroup)
            //    .WithRedirectUri(AzureConstants.ReturnUri)
            //    .Build();
        }

        protected override async void OnAppearing()
        {
            try
            {
                if (Device.RuntimePlatform == Device.iOS)
                {
                    App.Locator.MainPageVm.SafeAreaInsets = On<Xamarin.Forms.PlatformConfiguration.iOS>().SafeAreaInsets();
                }
                App.Locator.MainPageVm.IsLoginPopupButtonHidden = false;
                //this.CheckAzureADLogin();
                //OpenAzurePage();
            }
            catch (Exception)
            {

            }
        }

        private void ComboBox_OnUnFocused(object sender, FocusEventArgs e)
        {
            this.model.IsSelectingServer = false;
            //CheckAzureADLogin();
        }

        private void CheckAzureADLogin()
        {
            Device.StartTimer(new TimeSpan(0, 0, 1), () =>
            {
                if (this.model.SelectedServer != null)
                {
                    if (this.model.SelectedServer.AzureTenantId != null)
                    {
                        this.OpenAzurePage();
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            });            
        }

        private async void OpenAzurePage()
        {
            //var user = await DependencyService.Get<IAuthenticator>().Authenticate(AzureConstants.TenantUrl, AzureConstants.GraphResourceUri, AzureConstants.ApplicationID, AzureConstants.ReturnUri);
            //AzureConstants.AuthenticationResult = user;
            //if (user?.UserInfo != null)
            //{

            //}

            //var res = await new AzureConstants().SignInAsync();
            //var test = res;
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
    }
}
