using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using RealEstateInspector.Core.ViewModels;
using RealEstateInspector.Shared.Entities;
#if !SILVERLIGHT
using Microsoft.IdentityModel.Clients.ActiveDirectory;
#else
using RealEstateInspector.XForms.WinPhone;
#endif
using RealEstateInspector.Core;

namespace RealEstateInspector.Shared.Client
{


    public static class AuthenticationHelper
    {
#if SILVERLIGHT
        public static IEnumerable<AuthenticationResult> Tokens(this AuthenticationContext context)
        {
            return AuthenticationContext.Tokens;
#else
        public static IEnumerable<TokenCacheItem> Tokens(this AuthenticationContext context)
        {
            return context.TokenCache.ReadItems();
#endif
        }


        public static async Task<string> Authenticate(
#if DROID
       Android.App.Activity caller
#elif DESKTOP
IntPtr caller
#elif __IOS__
UIKit.UIViewController caller
#endif
)
        {
            try
            {

                var authContext = new AuthenticationContext(Configuration.Current.ADAuthority);

                var tokens = authContext.Tokens();
                var existing = (from t in tokens
                                where t.ClientId == Configuration.Current.ADNativeClientApplicationClientId &&
                                      t.Resource == Configuration.Current.MobileServiceAppIdUri
                                select t).FirstOrDefault();
                if (existing != null)
                {
                    try
                    {
                        var res = await authContext.AcquireTokenSilentAsync(
                            Configuration.Current.MobileServiceAppIdUri,
                            Configuration.Current.ADNativeClientApplicationClientId);
                        if (res != null && !string.IsNullOrWhiteSpace(res.AccessToken))
                        {
                            return res.AccessToken;
                        }
                    }
                    catch (Exception saex)
                    {
                        Debug.WriteLine(saex);
                    }

                    try
                    {
                        var res = await
                            authContext.AcquireTokenByRefreshTokenAsync(existing.RefreshToken,
                                Configuration.Current.ADNativeClientApplicationClientId);
                        if (res != null && !string.IsNullOrWhiteSpace(res.AccessToken))
                        {
                            return res.AccessToken;
                        }
                    }
                    catch (Exception saex)
                    {
                        Debug.WriteLine(saex);
                    }

                }





                var authResult =
                    await
                        authContext.AcquireTokenAsync(Configuration.Current.MobileServiceAppIdUri,
                        Configuration.Current.ADNativeClientApplicationClientId,
                        new Uri(Configuration.Current.ADRedirectUri),
#if WINDOWS_PHONE_APP || SILVERLIGHT 
                        new AuthorizationParameters()
#elif DROID || __IOS__
 new AuthorizationParameters(null)
#elif DESKTOP
 new AuthorizationParameters(PromptBehavior.Auto, caller)
#else
 new AuthorizationParameters(PromptBehavior.Auto, false)
#endif
);
                Debug.WriteLine(authResult != null);
                MobileServiceHttpHandler.RefreshToken = authResult.RefreshToken;
                return authResult.AccessToken;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
