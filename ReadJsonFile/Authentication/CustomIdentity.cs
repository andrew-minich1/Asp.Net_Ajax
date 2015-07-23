using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace ReadJsonFile.Authentication
{
    public class CustomIdentity: IIdentity
    {
        public CustomIdentity(string name)
        {
            this.Name = name;
        }
        public string Name { get; private set; }

        public string AuthenticationType
        {
            get { return "CustomAuthentication"; }
        }

        public bool IsAuthenticated
        {
            get { return !string.IsNullOrWhiteSpace(Name); }
        }
    }
}