using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KijijiScraper.Models
{
    public class KijijiSearchItem : TableEntity
    {
        public string SearchUrl { get; set; }
        public string WebhookUrl { get; set; }
        public DateTime NewerThan { get; set; }
    }
}
