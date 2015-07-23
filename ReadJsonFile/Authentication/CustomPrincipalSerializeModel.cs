using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReadJsonFile.Authentication
{
    public class CustomPrincipalSerializeModel
    {
        public int UserId { get; set; }
        public string[] roles { get; set; }
        public string Email { get; set; }
  
    }
}