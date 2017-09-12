using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using KijijiScraper.Models;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using System.Net.Http;
using System.Linq;

namespace KijijiScraper
{
    public static class SearchRunner
    {
        private static Lazy<HttpClient> _httpClient = new Lazy<HttpClient>(() => new HttpClient());

        [FunctionName("SearchRunner")]
        public static async Task Run(
            [QueueTrigger("SearchItems")]KijijiSearchItem searchItem,
            [Table("SavedSearches")]CloudTable savedSearchesTable,
            TraceWriter log)
        {
            var scraper = new Utilities.KijijiScraper();

            var ads = await scraper.GetAds(searchItem.SearchUrl, searchItem.NewerThan);

            if (!ads.Any()) return;

            var newestDate = ads.Max(x => x.PostedAt);

            await _httpClient.Value.PostAsJsonAsync(searchItem.WebhookUrl, ads);

            searchItem.NewerThan = newestDate;
            var replaceOp = TableOperation.Replace(searchItem);
            await savedSearchesTable.ExecuteAsync(replaceOp);
        }
    }
}
