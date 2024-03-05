using ClubWorldWeb.Domains.Models.Config;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ClubWorldWeb.Domains.Common
{
    public class Signupmodel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("emailid")]
        public string EmailId { get; set; }

        [JsonPropertyName("mobileno")]
        public string MobileNo { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

      
    }
   
}
