using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Newtonsoft.Json.Linq;

namespace RealEstateInspector.Core.ViewModels
{
    public class CustomMobileServiceSyncHandler : MobileServiceSyncHandler
    {
        public async override Task<JObject> ExecuteTableOperationAsync(IMobileServiceTableOperation operation)
        {
            try
            {
                return await base.ExecuteTableOperationAsync(operation);
            }
            catch (MobileServiceConflictException cex)
            {
                Debug.WriteLine(cex.Message);
                throw;
            }
            catch (MobileServicePreconditionFailedException pex)
            {
                Debug.WriteLine(pex.Message);
                throw;
            }
            catch (MobileServicePushFailedException pfex)
            {
                Debug.WriteLine(pfex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public override Task OnPushCompleteAsync(MobileServicePushCompletionResult result)
        {
            foreach (var error in result.Errors)
            {
                if (error.Status == HttpStatusCode.Conflict)
                {
                    error.CancelAndUpdateItemAsync(error.Result);
                    error.Handled = true;
                }
            }
            return base.OnPushCompleteAsync(result);
        }
    }
}