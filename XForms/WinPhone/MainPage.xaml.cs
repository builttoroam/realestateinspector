using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using RealEstateInspector.Core.ViewModels;
using RealEstateInspector.Shared.Client;

namespace RealEstateInspector.XForms.WinPhone
{
    public partial class MainPage : global::Xamarin.Forms.Platform.WinPhone.FormsApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            SupportedOrientations = SupportedPageOrientation.PortraitOrLandscape;

            global::Xamarin.Forms.Forms.Init();
            RealEstateInspector.XForms.MainPage.AuthenticateRequested += Authenticate;
            LoadApplication(new RealEstateInspector.XForms.App());
        }

        public async void Authenticate(object sender, EventArgs e)
        {
            var page = sender as RealEstateInspector.XForms.MainPage;
            var token = await AuthenticationHelper.Authenticate();
            Debug.WriteLine(token);

            await (page.BindingContext as MainViewModel).DataService.Initialize(token);
            
            await (page.BindingContext as MainViewModel).LoadPropertyData();
        }
    }

    public class AuthenticationResult
    {
        private static DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToLocalTime();

        public string ClientId { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("resource")]
        public string Resource { get; set; }

            [JsonProperty("expires_on")]
        public long ExpiresOnSeconds { get; set; }

        public DateTime ExpiresOn
        {
            get { return epoch.AddSeconds(ExpiresOnSeconds); }
        }

        private long refreshTokenExpiresInSeconds;
        [JsonProperty("refresh_token_expires_in")]
        public long RefreshTokenExpiresInSeconds
        {
            get { return refreshTokenExpiresInSeconds; }
            set
            {
                refreshTokenExpiresInSeconds = value;
                RefreshTokensExpiresOn = DateTime.Now.AddSeconds(refreshTokenExpiresInSeconds);
            }
        }

        public DateTime RefreshTokensExpiresOn { get;private set; }
}

    public class AuthorizationParameters
    {
        
    }

    public class AuthenticationContext
    {
        public static readonly List<AuthenticationResult> Tokens = new List<AuthenticationResult>();   

        public string Authority { get; set; }
        public string Resource { get; set; }
        public string ClientId { get; set; }
        public Uri RedirectUri { get; set; }

        public AuthenticationContext(string authority)
        {
            Authority = authority;
        }

        public async Task<AuthenticationResult> AcquireTokenByRefreshTokenAsync(string refreshToken, string clientId)
        {
            var http = new HttpClient();


            var tokenUrl = string.Format("{0}/oauth2/token", Authority);
            var formData = new Dictionary<string, string>
            {
                {"grant_type","refresh_token"},
                {"client_id",ClientId},
                {"refresh_token",refreshToken},
                {"resource",Resource}
            };

            var content = new FormUrlEncodedContent(formData);
            var data = await http.PostAsync(new Uri(tokenUrl), content);
            var result = JsonConvert.DeserializeObject<AuthenticationResult>(await data.Content.ReadAsStringAsync());
            result.ClientId = ClientId;
            Tokens.Add(result);
            return result;
        }

        public async Task<AuthenticationResult> AcquireTokenSilentAsync(
            string resource,
            string clientId)
        {
            var result = (from t in Tokens
                where t.ClientId == clientId &&
                      t.Resource == resource &&
                      t.ExpiresOn > DateTime.Now
                select t).FirstOrDefault();
            return result;
        }

    

        public async Task<AuthenticationResult> AcquireTokenAsync(
            string resource,
            string clientId,
            Uri redirectUri,
            AuthorizationParameters parameters)
        {
            Resource = resource;
            ClientId = clientId;
            RedirectUri = redirectUri;
            var code = await Authenticate();

            var http = new HttpClient();
            

            var tokenUrl = string.Format("{0}/oauth2/token",Authority);
            var formData = new Dictionary<string, string>
            {
                {"grant_type","authorization_code"},
                {"client_id",ClientId},
                {"code",code},
                {"resource",Resource},
                {"redirect_uri",RedirectUri.OriginalString}
            };

            var content = new FormUrlEncodedContent(formData);
            var data = await http.PostAsync(new Uri(tokenUrl), content);
            var result = JsonConvert.DeserializeObject<AuthenticationResult>(await data.Content.ReadAsStringAsync());
            result.ClientId = ClientId;
            Tokens.Add(result);
            return result;
        }

        private ManualResetEvent authenticateWaiter = new ManualResetEvent(false);
        private string AccessToken { get; set; }
        public async Task<string> Authenticate()
        {
            authenticateWaiter.Reset();
            var authUrlTemplate =
               "{0}/oauth2/authorize?response_type=code&client_id={1}&redirect_uri={2}";
            var authUrl = string.Format(authUrlTemplate,
                Authority,
                ClientId,
                Uri.EscapeDataString(RedirectUri.OriginalString)
                );

            var page = (Application.Current.RootVisual as Frame).Content as Page;
            var firstChild = page.Content;
            if (!(firstChild is Grid))
            {
                page.Content = null;
                var gd = new Grid();
                gd.Children.Add(firstChild);
                page.Content = gd;
                firstChild = gd;
            }

            var mainGrid = firstChild as Grid;
            var browser = new WebBrowser
            {
                IsScriptEnabled = true
            };
            browser.Navigating += BrowserNavigating;
            Grid.SetRowSpan(browser, (mainGrid.RowDefinitions != null && mainGrid.RowDefinitions.Count > 0) ? mainGrid.RowDefinitions.Count : 1);
            Grid.SetColumnSpan(browser, (mainGrid.ColumnDefinitions != null && mainGrid.ColumnDefinitions.Count > 0) ? mainGrid.ColumnDefinitions.Count : 1);
            mainGrid.Children.Add(browser);
            browser.Navigate(new Uri(authUrl));

            await Task.Run(() => authenticateWaiter.WaitOne());
            return AccessToken;
        }

        private void BrowserNavigating(object sender, NavigatingEventArgs e)
        {
            if (e.Uri.OriginalString.ToLower().StartsWith(RedirectUri.OriginalString.ToLower()))
            {
                try
                {
                    var query = e.Uri.OriginalString.Substring(e.Uri.OriginalString.IndexOf('?') + 1);
                    var code = (from pair in query.Split('&')
                                let bits = pair.Split('=')
                                where bits.Length == 2
                                      && bits[0] == "code"
                                select bits[1]).FirstOrDefault();
                    AccessToken = code;
                }
                catch (Exception ex)
                {
                    ex.Log();
                    AccessToken = null;
                }
                finally
                {
                    authenticateWaiter.Set();
                    var browser = sender as WebBrowser;
                    browser.Navigating -= BrowserNavigating;
                    (browser.Parent as Grid).Children.Remove(browser);
                }
            }
        }
    }
}
