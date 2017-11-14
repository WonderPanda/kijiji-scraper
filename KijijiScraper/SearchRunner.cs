using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using KijijiScraper.Models;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using System.Net.Http;
using System.Linq;
using KijijiScraper.Common;
using Microsoft.Extensions.Logging;

namespace KijijiScraper
{
    public static class SearchRunner
    {
        private static Lazy<HttpClient> _httpClient = new Lazy<HttpClient>(() => new HttpClient());

        [FunctionName("SearchRunner")]
        public static async Task Run(
            [QueueTrigger(Queues.SearchItemQueue)]KijijiSearchItem searchItem,
            [Table(Tables.SavedSearchesTable)]CloudTable savedSearchesTable,
            ExecutionContext context,
            ILogger logger)
        {
            var scraper = new Utilities.KijijiScraper(Services.TelemetryClient.Value);

            var ads = await scraper.GetAds(context.InvocationId.ToString(), searchItem.SearchUrl, searchItem.NewerThan);

            if (!ads.Any()) return;

            var newestDate = ads.Max(x => x.PostedAt);

            await _httpClient.Value.PostAsJsonAsync(searchItem.WebhookUrl, ads);

            searchItem.NewerThan = newestDate;
            var replaceOp = TableOperation.Replace(searchItem);
            await savedSearchesTable.ExecuteAsync(replaceOp);
        }
    }
}
