using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using RealEstateInspector.Shared.Entities;

namespace RealEstateInspector.Core.ViewModels
{
    public class MobileServiceHttpHandler : DelegatingHandler
    {
        public static string RefreshToken { get; set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(RefreshToken))
            {
                request.Headers.Add(SharedConstants.RefreshTokenHeaderKey, RefreshToken);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}