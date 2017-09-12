using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Linq;
using KijijiScraper.Models;
using System.Threading.Tasks;

namespace KijijiScraper
{
    public static class ScrapeTimer
    {
        [FunctionName("ScrapeTimer")]
        public static async Task Run(
            [TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, 
            [Table("SavedSearches")] IQueryable<KijijiSearchItem> searchItemsTable,
            [Queue("SearchItems")] IAsyncCollector<KijijiSearchItem> searchQueue,
            TraceWriter log)
        {
            var searches = searchItemsTable.ToList();
            await Task.WhenAll(searches.Select(x => searchQueue.AddAsync(x)));
        }
    }
}
