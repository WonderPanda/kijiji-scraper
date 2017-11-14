using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using KijijiScraper.Models;
using System;
using KijijiScraper.Common;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using KijijiScraper.Utilities;

namespace KijijiScraper
{
    [StorageAccount("AzureWebJobsStorage")]
    public static class AddSavedSearch
    {
        [FunctionName("AddSavedSearch")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req,
            [Table(Tables.SavedSearchesTable)] IAsyncCollector<KijijiSearchItem> savedSearchTable,
            TraceWriter log)
        {
            var searchItem = await req.Content.ReadAsAsync<KijijiSearchItemDto>() ?? new KijijiSearchItemDto();
            List<ValidationResult> results;
            var isValid = Validation.ValidateModel(searchItem, out results);

            if (!isValid) return req.CreateResponse(HttpStatusCode.BadRequest, results);
            
            var savedSearch = new KijijiSearchItem
            {
                PartitionKey = "SavedSearches",
                RowKey = WebUtility.UrlEncode(searchItem.SearchUrl),
                SearchUrl = searchItem.SearchUrl,
                WebhookUrl = searchItem.WebhookUrl,
                NewerThan = DateTime.UtcNow - TimeSpan.FromDays(2.0)
            };

            await savedSearchTable.AddAsync(savedSearch);

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
