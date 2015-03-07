using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.OData;
using System.Web.ModelBinding;
using Microsoft.AspNet.Identity;
//using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Microsoft.WindowsAzure.Mobile.Service.Tables;
using RealEstateInspector;
using RealEstateInspector.Shared.Entities;
using realestateinspectorService.DataObjects;
using realestateinspectorService.Models;

namespace realestateinspectorService.Controllers
{
    public class PropertyTypeController : RealEstateBaseTableController<PropertyType> { }

    public class RealEstatePropertyController : RealEstateBaseTableController<RealEstateProperty> { }

    public class InspectionController : RealEstateBaseTableController<Inspection> { }

    [AuthorizeLevel(AuthorizationLevel.User)]
//    [AuthorizeInspector]
    public class RealEstateBaseTableController<TEntity> : TableController<TEntity>
        where TEntity : class, ITableData
    {
        protected async override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            var context = new realestateinspectorContext();
            DomainManager = new EntityDomainManager<TEntity>(context, Request, Services);

        }


        // GET tables/TEntity
        public async Task<IQueryable<TEntity>> GetAll()
        {
            return Query();
            //var user = User as ServiceUser;
            //var aadCreds = (await user.GetIdentitiesAsync()).OfType<AzureActiveDirectoryCredentials>().FirstOrDefault();
            //Debug.WriteLine(aadCreds.AccessToken);


            //var token = this.ActionContext.Request.Headers.GetValues(Constants.RefreshTokenHeaderKey)
            //    .FirstOrDefault();

            //var auth = new AuthenticationContext(Constants.ADAuthority, false);
            //var newToken = await auth.AcquireTokenByRefreshTokenAsync(token,
            //    Constants.ADNativeClientApplicationClientId, "https://graph.windows.net");



            //var client = RetrieveActiveDirectoryClient(newToken.AccessToken);
            //var grps = await client.Groups.ExecuteAsync();
            //var moreGroups = grps.CurrentPage;

            //var claimsIdentity = user.Identities.First();
            //var claimType = "IsInspector";
            ////var claims = Request.GetOwinContext().Authentication.User .Claims;
            //while (moreGroups != null)
            //{
            //    foreach (var grp in grps.CurrentPage)
            //    {
            //        if (grp.DisplayName == "Inspectors")
            //        {
            //            if ((await client.IsMemberOfAsync(grp.ObjectId, aadCreds.ObjectId)) ?? false)
            //            {
            //                if (claimsIdentity.HasClaim((c) => c.Type == claimType))
            //                {
            //                    claimsIdentity.RemoveClaim(claimsIdentity.FindFirst(claimType));
            //                }
            //                claimsIdentity.AddClaim(new Claim(claimType, "true"));
            //                 var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
            //                 authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalBearer);
            //                 authenticationManager.SignIn(user.Identities.ToArray());
            //            }
            //            return Query();
            //        }




            //        Debug.WriteLine(grp != null);
            //    }
            //    if (grps.MorePagesAvailable)
            //    {
            //        grps = await grps.GetNextPageAsync();
            //        moreGroups = grps.CurrentPage;
            //    }
            //}

            //return null;
        }

        // GET tables/TEntity/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<TEntity> Get(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/TEntity/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<TEntity> Patch(string id, Delta<TEntity> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/TEntity
        public async Task<IHttpActionResult> Post(TEntity item)
        {
            TEntity current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/TEntity/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task Delete(string id)
        {
            return DeleteAsync(id);
        }
    }



    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AuthorizeInspector : AuthorizationFilterAttribute
    {
        //public static ActiveDirectoryClient RetrieveActiveDirectoryClient(string token)
        //{
        //    var baseServiceUri = new Uri(Microsoft.Azure.ActiveDirectory.GraphClient.Constants.ResourceId);
        //    var activeDirectoryClient =
        //        new ActiveDirectoryClient(new Uri(baseServiceUri, Configuration.Current.ADTenant),
        //            async () => token);
        //    return activeDirectoryClient;
        //}

        public override async Task OnAuthorizationAsync(HttpActionContext actionContext,
            CancellationToken cancellationToken)
        {
            await base.OnAuthorizationAsync(actionContext, cancellationToken);

            var controller = actionContext.ControllerContext.Controller as ApiController;
            if (controller == null)
            {
                return;
            }
            var user = controller.User as ServiceUser;

            //var user = User as ServiceUser;
            var aadCreds = (await user.GetIdentitiesAsync()).OfType<AzureActiveDirectoryCredentials>().FirstOrDefault();
            Debug.WriteLine(aadCreds.AccessToken);

            var cookie = HttpContext.Current.Request.Cookies["IsInspector"];
            var accessTokenCookie = HttpContext.Current.Request.Cookies["IsInspectorAccessToken"];
            var isInspector = cookie != null ? cookie.Value : null;
            var access_token = accessTokenCookie != null ? accessTokenCookie.Value : null;
            if (isInspector != null && access_token == aadCreds.AccessToken)
            {
                if (!(bool.Parse(isInspector)))
                {
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden);
                }
                return;
            }



            var token = actionContext.Request.Headers.GetValues(SharedConstants.RefreshTokenHeaderKey)
                .FirstOrDefault();

            var auth = new AuthenticationContext(Configuration.Current.ADAuthority, false);
            var newToken = await auth.AcquireTokenByRefreshTokenAsync(token,
                Configuration.Current.ADNativeClientApplicationClientId, ServiceConstants.AADGraphAPIRoot);



            //var client = RetrieveActiveDirectoryClient(newToken.AccessToken);
            //var grps = await client.Groups.ExecuteAsync();
            //var moreGroups = grps.CurrentPage;

            try
            {
                //while (moreGroups != null)
                //{
                //    foreach (var grp in grps.CurrentPage)
                //    {
                //        if (grp.DisplayName == ServiceConstants.InspectorsADGroupName)
                //        {
                //            if ((await client.IsMemberOfAsync(grp.ObjectId, aadCreds.ObjectId)) ?? false)
                //            {
                //                HttpContext.Current.Response.Cookies.Add(new HttpCookie(ServiceConstants.IsInspectorCookie, true.ToString()));

                //                return;
                //            }
                //        }
                //    }
                //    if (grps.MorePagesAvailable)
                //    {
                //        grps = await grps.GetNextPageAsync();
                //        moreGroups = grps.CurrentPage;
                //    }
                //    else
                //    {
                //        grps = null;
                //        moreGroups = null;
                //    }
                //}
                HttpContext.Current.Response.Cookies.Add(new HttpCookie("IsInspector", false.ToString()));
            }
            finally
            {
                HttpContext.Current.Response.Cookies.Add(new HttpCookie("IsInspectorAccessToken", aadCreds.AccessToken));

            }
        }
    }


    public class AzureADGraphClaimsAuthenticationManager : ClaimsAuthenticationManager
    {
        public override ClaimsPrincipal Authenticate(string resourceName, ClaimsPrincipal incomingPrincipal)
        {
            return base.Authenticate(resourceName, incomingPrincipal);
            //if (incomingPrincipal == null || !incomingPrincipal.Identity.IsAuthenticated)
            //    return incomingPrincipal;

            //// Ideally this should be the code below so the connection is resolved from a DI container, but for simplicity of the demo I'll leave it as a new statement
            ////var graphConnection = DependencyResolver.Current.GetService<IAzureADGraphConnection>();
            //var graphConnection = new AzureADGraphConnection(
            //    ConfigurationManager.AppSettings["AzureADTenant"],
            //    ConfigurationManager.AppSettings["ida:ClientId"],
            //    ConfigurationManager.AppSettings["ida:Password"]);

            //var roles = graphConnection.GetRolesForUser(incomingPrincipal);
            //foreach (var r in roles)
            //    ((ClaimsIdentity)incomingPrincipal.Identity).AddClaim(
            //        new Claim(ClaimTypes.Role, r, ClaimValueTypes.String, "GRAPH"));
            //return incomingPrincipal;
        }
    }


}