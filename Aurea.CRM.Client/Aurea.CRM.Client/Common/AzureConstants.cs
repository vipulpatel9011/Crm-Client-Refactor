//using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurea.CRM.Client.UI.Common
{
    public class AzureConstants
    {
        public static string ApplicationID = "92492922-7c80-4a47-bc47-dd3ba368fa6f";
        public static string TenantUrl = "https://login.microsoftonline.com/tfp/0404b13e-5f04-4ec2-8e12-1a2bb55b3b20/";
        public static string TenantId = "0404b13e-5f04-4ec2-8e12-1a2bb55b3b20";
        public static string ReturnUri = "https://crmtest.gbo.com";
        public static string GraphResourceUri = "https://graph.microsoft.com";
        public static string IOSKeyChainGroup = "com.microsoft.adalcache";
        public static string[] Scopes = { "user.read" };
        public static string AppReturnUrl = "msal" + ApplicationID + "://auth";
        //public static AuthenticationResult AuthenticationResult = null;

        //private const string ClientId = "92492922-7c80-4a47-bc47-dd3ba368fa6f";

        //public static IPublicClientApplication PublicClientApp;

        //public async Task<AuthenticationResult> SignInAsync()
        //{
        //    AuthenticationResult newContext = null;
        //    try
        //    {
        //        // acquire token silent
        //        newContext = await AcquireToken().ConfigureAwait(false);
        //    }
        //    catch (MsalUiRequiredException ex)
        //    {
        //        // acquire token interactive
        //        newContext = await SignInInteractively().ConfigureAwait(false);
        //    }
        //    return newContext;
        //}

        //private async Task<AuthenticationResult> SignInInteractively()
        //{
        //    IEnumerable<IAccount> accounts = await PublicClientApp.GetAccountsAsync();

        //   // AuthenticationResult authResult = await PublicClientApp.AcquireTokenInteractive(Scopes)
        //        //.WithAccount(GetAccountByPolicy(accounts, AuthorizationConstants.SignUpSignInPolicy))
        //        //.WithParentActivityOrWindow(ParentActivityOrWindow)
        //       // .ExecuteAsync();

        //    //var newContext = UpdateUserInfo(authResult);
        //    //return newContext;
        //    return null;
        //}
        //private async Task<AuthenticationResult> AcquireToken()
        //{
        //    try
        //    {
        //        IEnumerable<IAccount> accounts = await PublicClientApp.GetAccountsAsync();
        //     //   AuthenticationResult authResult = await PublicClientApp.AcquireTokenSilent(Scopes, accounts.ToList().FirstOrDefault())
        //           //.WithB2CAuthority(AuthorizationConstants.AuthoritySignInSignUp)
        //          // .ExecuteAsync();

        //        //var newContext = UpdateUserInfo(authResult);
        //        //return newContext;
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}

    }
}
