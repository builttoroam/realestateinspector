using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace realestateinspectorService.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.Anonymous)]
    public class SharedAccessSignatureController :  ResourcesControllerBase
    {
 
        public async Task<string> Get(string id)
        {
            var sas = string.Empty;

            if (!string.IsNullOrEmpty(id))
            {
                // Try to get the Azure storage account token from app settings.  
                string storageAccountConnectionString;

                if (Services.Settings.TryGetValue("ResourceBrokerStorageConnectionString", out storageAccountConnectionString) )
                {
                    // Set the URI for the Blob Storage service.
                    var account = CloudStorageAccount.Parse(storageAccountConnectionString);
                    // Create the BLOB service client.
                    var blobClient = new CloudBlobClient(account.BlobStorageUri,account.Credentials);

                    // Create a container, if it doesn't already exist.
                    var container = blobClient.GetContainerReference(id);
                    await container.CreateIfNotExistsAsync();

                    // Create a shared access permission policy. 
                    var containerPermissions = new BlobContainerPermissions();

                    // Enable anonymous read access to BLOBs.
                    containerPermissions.PublicAccess = BlobContainerPublicAccessType.Blob;
                    container.SetPermissions(containerPermissions);

                    // Define a policy that gives write access to the container for 1h
                    var sasPolicy = new SharedAccessBlobPolicy()
                    {
                        SharedAccessStartTime = DateTime.UtcNow,
                        SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(59).AddSeconds(59),
                        Permissions = SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Read
                    };
                    var ub = new UriBuilder(container.Uri.OriginalString)
                    {
                        Query = container.GetSharedAccessSignature(sasPolicy).TrimStart('?')
                    };
                    sas =  ub.Uri.OriginalString;
                }
            }

            return sas;
        }
    }

}
