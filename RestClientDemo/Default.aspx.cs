using System;
using RestSharp;

namespace RestClientDemo
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var client = new RestClient("http://localhost:50728");
            var request = new RestRequest("/test?id=1", Method.GET);

            client.Authenticator = new HttpBasicAuthenticator("huan", "huan");
            client.Authenticator.Authenticate(client, request);

            var response = client.Execute(request);
            LG.Text = response.Content;

            var request2 = new RestRequest("/test?id=1", Method.POST);
            var response2 = client.Execute(request2);
            LP.Text = response2.Content;

            var request3 = new RestRequest("/test?id=1", Method.PUT);
            var response3 = client.Execute(request3);
            LPU.Text = response3.Content;

            var request4 = new RestRequest("/test?id=1", Method.DELETE);
            var response4 = client.Execute(request4);
            LD.Text = response4.Content;
        }
    }
}