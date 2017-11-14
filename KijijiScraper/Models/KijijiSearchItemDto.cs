using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KijijiScraper.Models
{
    public class KijijiSearchItemDto
    {
        [Required]
        [Url]
        public string SearchUrl { get; set; }

        [Required]
        //[Url]    // Doesn't work with localhost
        public string WebhookUrl { get; set; }
    }
}
