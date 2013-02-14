using System;
using System.Text;
using System.Web;

namespace RESTfullDemo.Services
{
    public class TestRestService : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("HttpMethod: {0}", context.Request.HttpMethod);
            sb.AppendLine();
            sb.AppendFormat("QueryString: {0}", context.Request.QueryString);

            switch (context.Request.HttpMethod.ToHttpMethod())
            {
                case HttpMethod.Get:
                case HttpMethod.Post:
                case HttpMethod.Put:
                case HttpMethod.Delete:
                    context.Response.Write(sb.ToString());
                    break;

                case HttpMethod.Unknown:
                    throw new NotImplementedException();
            }
        }
    }
}