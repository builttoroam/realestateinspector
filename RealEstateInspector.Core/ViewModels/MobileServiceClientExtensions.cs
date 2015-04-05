using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuiltToRoam.Mobile;
using Microsoft.WindowsAzure.MobileServices;

namespace RealEstateInspector.Core.ViewModels
{
    public static class MobileServiceClientExtensions
    {
        public async static Task PullLatestAsync<TTable>(this IMobileServiceClient client, CancellationToken token) where TTable : BaseEntityData
        {
            // Get the most recent
            var mostRecent = await client.GetLatestAsync<TTable>();

            // Convert the most recent into a query (assuming there is one)
            if (mostRecent != null)
            {
                var maxTimestamp = mostRecent.UpdatedAt.AddMilliseconds(1);
                var q = client.GetSyncTable<TTable>()
                    .CreateQuery()
                    .Where(x => (x.Id != mostRecent.Id || x.UpdatedAt > maxTimestamp));
                // Do a (filtered) pull from the remote tabl
                await client.GetSyncTable<TTable>().PullAsync(typeof(TTable).Name, q, token);
            }
            else
            {
                await client.GetSyncTable<TTable>().PullAsync(typeof(TTable).Name, client.GetSyncTable<TTable>().CreateQuery(), token);
            }
        }

        public async static Task<TTable> GetLatestAsync<TTable>(this IMobileServiceClient client) where TTable : BaseEntityData
        {
            return (await client.GetSyncTable<TTable>()
                .OrderByDescending(x => x.UpdatedAt)
                .Take(1)
                .ToListAsync()).SingleOrDefault();
        }
    }
}