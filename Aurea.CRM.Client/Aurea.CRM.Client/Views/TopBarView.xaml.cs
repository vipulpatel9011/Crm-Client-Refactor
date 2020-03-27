// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TopBarView.xaml.cs" company="Aurea Software Gmbh">
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
//   Code behind for the top bar view
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Client.UI.Views
{
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

    /// <inheritdoc />
    /// <summary>
    /// Code behind for the top bar view
    /// </summary>
    /// <seealso cref="T:Xamarin.Forms.ContentPage" />
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TopBarView : ContentView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TopBarView"/> class.
        /// </summary>
        public TopBarView()
        {
            this.InitializeComponent();
            //this.BindingContext = App.Locator.MainPageVm;
            //grdTopBar.Padding = App.Locator.MainPageVm.SafeAreaInsets;
            //grdTopBar.On<Xamarin.Forms.PlatformConfiguration.iOS>().
        }
    }
}
