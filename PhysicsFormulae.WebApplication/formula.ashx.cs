using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PhysicsFormulae.WebApplication
{
    public class formula : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";

            var urlReference = context.Request.QueryString["r"];

            if (urlReference == "")
            {
                return;
            }

            var json = File.ReadAllText(context.Server.MapPath("~/formulae.json"));

            dynamic jo = JValue.Parse(json);

            var formulae = (IEnumerable<dynamic>)jo.Formulae;

            if (!formulae.Any(f => f.URLReference == urlReference))
            {
                return;
            }

            var formula = formulae.First(f => f.URLReference == urlReference);

            var title = formula.Title.ToString();
            var interpretation = formula.Interpretation.ToString();
            var previewImageURL = "http://www.physicsformulae.com/images/" + formula.URLReference.ToString() + ".png";
            var canonicalURL = "http://www.physicsformulae.com/#/formula/" + formula.URLReference.ToString();

            var template = File.ReadAllText(context.Server.MapPath("~/metadata-template.html"));

            template = template.Replace("{{title}}", title);
            template = template.Replace("{{description}}", interpretation);
            template = template.Replace("{{previewImageURL}}", previewImageURL);
            template = template.Replace("{{canonicalURL}}", canonicalURL);

            context.Response.Write(template);
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