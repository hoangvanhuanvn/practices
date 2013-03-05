using System.Web;
using Microsoft.IdentityModel.Protocols.WSFederation.Metadata;

namespace GdNet.Accounts.UI.FederationMetadata._2007_06
{
    public class Federationmetadata : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            var metadata = Common.GetFederationMetadata();
            var serializer = new MetadataSerializer();
            serializer.WriteMetadata(context.Response.OutputStream, metadata);
            context.Response.ContentType = "text/xml";
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