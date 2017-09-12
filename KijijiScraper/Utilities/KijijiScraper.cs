using AngleSharp;
using KijijiScraper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KijijiScraper.Utilities
{
    public class KijijiScraper
    {
        public async Task<List<KijijiAd>> GetAds(string address, DateTime newerThan)
        {
            var config = Configuration.Default.WithDefaultLoader();

            var document = await BrowsingContext.New(config).OpenAsync(address);

            var filteredAds = document.QuerySelectorAll("item")
                .Select(item =>
                {
                    var postedAt = DateTime.Parse(item.QuerySelector("pubDate").TextContent).ToUniversalTime();
                    var adUrl = item.QuerySelector("guid").TextContent;
                    var title = item.QuerySelector("title").TextContent;
                    return new KijijiAd
                    {
                        AdUrl = adUrl,
                        Title = title,
                        PostedAt = postedAt
                    };
                })
                .Where(item => item.PostedAt > newerThan)
                .ToList();

            return filteredAds;
        }
    }
}
