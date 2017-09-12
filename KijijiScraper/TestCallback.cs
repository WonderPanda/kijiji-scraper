using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Collections.Generic;

namespace KijijiScraper
{
    public static class TestCallback
    {
        [FunctionName("TestCallback")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# callback test received kijiji ads");
            
            // Get request body
            List<object> data = await req.Content.ReadAsAsync<List<object>>();

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
