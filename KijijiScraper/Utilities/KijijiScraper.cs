using AngleSharp;
using KijijiScraper.Models;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KijijiScraper.Utilities
{
    public class KijijiScraper
    {
        private readonly TelemetryClient _telemetry;

        public KijijiScraper(TelemetryClient telemetryClient)
        {
            _telemetry = telemetryClient;
        }

        public async Task<List<KijijiAd>> GetAds(string operationId, string address, DateTime newerThan)
        {
            var startTime = DateTime.UtcNow;
            var timer = Stopwatch.StartNew();
            bool success = true;

            try
            {
                _telemetry.Context.Operation.Id = operationId;

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
            catch (Exception e)
            {
                success = false;
                throw e;
            }
            finally
            {
                timer.Stop();
                var dependencyTelemetry = new DependencyTelemetry("Http", address, "Kijiji", "Scrape Ads", startTime, timer.Elapsed, null, success);
                dependencyTelemetry.Context.Operation.Id = operationId;
                _telemetry.TrackDependency(dependencyTelemetry);
            }
        }
    }
}
