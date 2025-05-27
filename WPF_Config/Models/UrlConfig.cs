using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Config.Models
{
    public class UrlConfig
    {
       
         public int Id { get; set; }
            public string UserAccessToken { get; set; }
            public string Version { get; set; }
            public string PhoneNumberId { get; set; }
            public string ApiUrl { get; set; } = "https://graph.facebook.com";
            public DateTime? LastUpdate { get; set; } = DateTime.Now;
    }

}

