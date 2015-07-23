using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace ReadJsonFile.Module
{
    public class CustomHttpModule :IHttpModule
    {
        private Stopwatch sw;
        public void Dispose()
        {
            
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(this.OnBeginRequest);
            context.EndRequest += new EventHandler(this.OnEndRequest);
        }

        private void OnBeginRequest(object sender, EventArgs e)
        {
            sw = Stopwatch.StartNew();
        }

        private void OnEndRequest(object sender, EventArgs e)
        {
            sw.Stop();
            //HttpApplication app = (HttpApplication)sender;
            //app.Response.Write("<div>" + sw.Elapsed + "<div>");
        }


    }
}