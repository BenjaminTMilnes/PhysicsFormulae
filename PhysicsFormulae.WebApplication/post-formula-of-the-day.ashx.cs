using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhysicsFormulae.WebApplication
{
     public class post_formula_of_the_day : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("Posted formula of the day to social media.");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}