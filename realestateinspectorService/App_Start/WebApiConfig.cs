using System.Data.Entity.Migrations;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Config;
using Microsoft.WindowsAzure.Mobile.Service.Security.Providers;
using realestateinspectorService.Migrations;

namespace realestateinspectorService
{
    public static class WebApiConfig
    {
        public static void Register()
        {

            // Use this class to set configuration options for your mobile service
            ConfigOptions options = new ConfigOptions();
            options.LoginProviders.Remove(typeof(AzureActiveDirectoryLoginProvider));
            options.LoginProviders.Add(typeof(AzureActiveDirectoryExtendedLoginProvider));

            var builder = new ConfigBuilder(options);//, ConfigureDependencies);
            // Use this class to set WebAPI configuration options
            HttpConfiguration config = ServiceConfig.Initialize(builder);
            

            // To display errors in the browser during development, uncomment the following
            // line. Comment it out again when you deploy your service for production use.
            // config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            // This tells the local mobile service project to run as if it is being hosted in Azure, 
            // including honoring the AuthorizeLevel settings. Without this setting, all HTTP requests to 
            // localhost are permitted without authentication despite the AuthorizeLevel setting.
            config.SetIsHosted(true);

           

            // Create a custom route mapping the resource type into the URI.     
            var resourcesRoute = config.Routes.CreateRoute(
                 routeTemplate: "api/sharedaccesssignature/{type}/{id}",
                 defaults: new { controller = "sharedaccesssignature", id = RouteParameter.Optional },
                 constraints: null);

            // Insert the ResourcesController route at the top of the collection to avoid conflicting with predefined routes. 
            config.Routes.Insert(0, "SAS", resourcesRoute);

            config.Routes.MapHttpRoute(
               name: "DefaultApi",
               routeTemplate: "api/{controller}/{action}/{id}",
               defaults: new { action = RouteParameter.Optional, id = RouteParameter.Optional }
           );
            SignalRExtensionConfig.Initialize();

            //Database.SetInitializer(new realestateinspectorInitializer());

            // Migration
            var migrator = new DbMigrator(new Configuration());
            migrator.Update();
        }

        //private static void ConfigureDependencies(HttpConfiguration configuration, ContainerBuilder builder)
        //{
        //    // Configure DI here

        //    // Register our extensions
        //    builder.RegisterType<MyOwinAppBuilderExtension>().As<IOwinAppBuilderExtension>();
        //}

    }


    //public class MyOwinAppBuilderExtension : IOwinAppBuilderExtension
    //{
    //    public void Configure(Owin.IAppBuilder appBuilder)
    //    {
    //        appBuilder.UseMyCustomMiddleware(new CustomAuthenticationOptions()); //"test","test"));
    //     //   appBuilder.UseMyCustomMiddleware2(new CustomAuthenticationOptions());
    //    }
    //}

    //public static class MyOwinExtensions
    //{
    //    public static IAppBuilder UseMyCustomMiddleware(this IAppBuilder appBuilder, CustomAuthenticationOptions options)
    //    {
    //        return appBuilder.Use(typeof (CustomMiddleware));// typeof(MyAuthenticationMiddleware), appBuilder, options);
    //    }

    //    public static IAppBuilder UseMyCustomMiddleware2(this IAppBuilder appBuilder, CustomAuthenticationOptions options)
    //    {
    //        return appBuilder.Use(typeof (MyAuthenticationMiddleware), appBuilder, options);
    //    }
    //}

    //internal static class OwinConstants
    //{
    //    internal const string DefaultAuthenticationType = "Dummy";
    //}

    //public class CustomAuthenticationOptions : AuthenticationOptions
    //{
    //    public CustomAuthenticationOptions() //string userName, string userId)
    //        : base(OwinConstants.DefaultAuthenticationType)
    //    {
    //        Description.Caption = OwinConstants.DefaultAuthenticationType;
    //        AuthenticationMode = AuthenticationMode.Active;
    //        //CallbackPath = new PathString("/signin-dummy");
    //        //UserName = userName;
    //        //UserId = userId;
    //    }


    //    //    public string UserName { get; set; }

    //    //    public string UserId { get; set; }

    //    //    public string SignInAsAuthenticationType { get; set; }

    //    //    public ISecureDataFormat<AuthenticationProperties> StateDataFormat { get; set; }
    //}

    //public class CustomMiddleware : OwinMiddleware
    //{
    //    public CustomMiddleware(OwinMiddleware next) : base(next)
    //    {
    //    }

    //    public async override Task Invoke(IOwinContext context)
    //    {
    //        //var identity = context.Authentication.User.Identities.FirstOrDefault(x => x.Name == "Inspector");
    //        //if (identity == null)
    //        //{
    //        //    identity = new ClaimsIdentity("Inspector");
    //        //    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "InspectorX"));
    //        //    identity.AddClaim(new Claim(ClaimTypes.Name, "InspectorName"));
    //        //    context.Authentication.User.AddIdentity(identity);

    //        //}
    //        if (context.Authentication.User.Identity.AuthenticationType == "Federation")
    //        {
    //            Debug.WriteLine("Here we go");
    //        }
    //            Debug.WriteLine(context != null);
    //        await Next.Invoke(context);

    //        var grant = context.Authentication.AuthenticationResponseGrant;
    //        if (grant != null)
    //        {
    //            Debug.WriteLine("Ok here we go!");
    //        }
    //    }
    //}

    //public class MyAuthenticationMiddleware : AuthenticationMiddleware<CustomAuthenticationOptions>
    //{
    //    public MyAuthenticationMiddleware(OwinMiddleware next, IAppBuilder app, CustomAuthenticationOptions options)
    //        : base(next, options)
    //    {
    //        //if (string.IsNullOrEmpty(Options.SignInAsAuthenticationType))
    //        //{
    //        //    options.SignInAsAuthenticationType = app.GetDefaultSignInAsAuthenticationType();
    //        //}
    //        //if (options.StateDataFormat == null)
    //        //{
    //        //    var dataProtector = app.CreateDataProtector(typeof(MyAuthenticationMiddleware).FullName,
    //        //        options.AuthenticationType);

    //        //    options.StateDataFormat = new PropertiesDataFormat(dataProtector);
    //        //}
    //    }

    //    // Called for each request, to create a handler for each request.
    //    protected override AuthenticationHandler<CustomAuthenticationOptions> CreateHandler()
    //    {
    //        return new CustomAuthenticationHandler();
    //    }
    //}


    //public class CustomAuthenticationHandler : AuthenticationHandler<CustomAuthenticationOptions>
    //{

    //    protected override async Task ApplyResponseGrantAsync()
    //    {
    //        await base.ApplyResponseGrantAsync();

    //       // this.Context.Authentication.AuthenticationResponseGrant.Identity.AddClaim(new Claim("Inspector", "true"));

    //    }

    //    protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
    //    {
    //        return null;
    //        //// ASP.Net Identity requires the NameIdentitifer field to be set or it won't  
    //        //// accept the external login (AuthenticationManagerExtensions.GetExternalLoginInfo)
    //        //var identity = new ClaimsIdentity("Inspector");
    //        //identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "InspectorX", null, Options.AuthenticationType));
    //        //identity.AddClaim(new Claim(ClaimTypes.Name, "InspectorName"));


    //        //return new AuthenticationTicket(identity, new AuthenticationProperties());
    //    }

    //    //protected override Task ApplyResponseChallengeAsync()
    //    //{
    //    //    if (Response.StatusCode == 401)
    //    //    {
    //    //        var challenge = Helper.LookupChallenge(Options.AuthenticationType, Options.AuthenticationMode);

    //    //        // Only react to 401 if there is an authentication challenge for the authentication 
    //    //        // type of this handler.
    //    //        if (challenge != null)
    //    //        {
    //    //            var state = challenge.Properties;

    //    //            if (string.IsNullOrEmpty(state.RedirectUri))
    //    //            {
    //    //                state.RedirectUri = Request.Uri.ToString();
    //    //            }

    //    //            var stateString = Options.StateDataFormat.Protect(state);

    //    //            Response.Redirect(WebUtilities.AddQueryString(Options.CallbackPath.Value, "state", stateString));
    //    //        }
    //    //    }

    //    //    return Task.FromResult<object>(null);
    //    //}

    //    //public override async Task<bool> InvokeAsync()
    //    //{
    //    //    //// This is always invoked on each request. For passive middleware, only do anything if this is
    //    //    //// for our callback path when the user is redirected back from the authentication provider.
    //    //    //if (Options.CallbackPath.HasValue && Options.CallbackPath == Request.Path)
    //    //    //{
    //    //        var ticket = await AuthenticateAsync();

    //    //        if (ticket != null)
    //    //        {
    //    //            Context.Authentication.SignIn(ticket.Properties, ticket.Identity);

    //    //   //         Response.Redirect(ticket.Properties.RedirectUri);

    //    //            // Prevent further processing by the owin pipeline.
    //    //            return true;
    //    //        }
    //    ////    }
    //    ////    // Let the rest of the pipeline run.
    //    //   return false;
    //    //}
    //}





    //public class realestateinspectorInitializer : ClearDatabaseSchemaIfModelChanges<realestateinspectorContext>
    //{
    //    protected override void Seed(realestateinspectorContext context)
    //    {
    //        List<TodoItem> todoItems = new List<TodoItem>
    //        {
    //            new TodoItem { Id = Guid.NewGuid().ToString(), Text = "First item", Complete = false },
    //            new TodoItem { Id = Guid.NewGuid().ToString(), Text = "Second item", Complete = false },
    //        };

    //        foreach (TodoItem todoItem in todoItems)
    //        {
    //            context.Set<TodoItem>().Add(todoItem);
    //        }

    //        base.Seed(context);
    //    }

}


