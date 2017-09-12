using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using KijijiScraper.Models;
using System;

namespace KijijiScraper
{
    [StorageAccount("AzureWebJobsStorage")]
    public static class AddSavedSearch
    {
        [FunctionName("AddSavedSearch")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req,
            [Table("SavedSearches")] IAsyncCollector<KijijiSearchItem> savedSearchTable,
            TraceWriter log)
        {
            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();
            var searchUrl = data?.searchUrl?.ToString();
            var webhookUrl = data?.webhookUrl?.ToString();

            if (searchUrl == null || webhookUrl == null)
                return req.CreateResponse(HttpStatusCode.BadRequest, "searchUrl and webhookUrl are required");

            var savedSearch = new KijijiSearchItem
            {
                PartitionKey = "SavedSearches",
                RowKey = WebUtility.UrlEncode(searchUrl),
                SearchUrl = searchUrl,
                WebhookUrl = webhookUrl,
                NewerThan = DateTime.UtcNow - TimeSpan.FromHours(5)
            };

            await savedSearchTable.AddAsync(savedSearch);

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
