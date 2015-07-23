using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReadJsonFile.Models
{
    [JsonObject]
    public class Comment
    {
        [JsonProperty("Text")]
        public string Text { get; set; }
         [JsonProperty("UserName")]
        public string UserName { get; set; }
    }
}