using Microsoft.ApplicationInsights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KijijiScraper.Common
{
    public class Services
    {
        public static Lazy<TelemetryClient> TelemetryClient = new Lazy<TelemetryClient>(() =>
        {
            var key = Environment.GetEnvironmentVariable(Config.InstrumentationKey);
            return new TelemetryClient() { InstrumentationKey = key };
        });
    }
}
