using System;
using System.Globalization;
using System.Web.Configuration;
using System.Web.Mvc;
using GdNet.Accounts.UI.Controllers.Common;
using Microsoft.IdentityModel.Protocols.WSFederation;
using Microsoft.IdentityModel.SecurityTokenService;
using Microsoft.IdentityModel.Web;

namespace GdNet.Accounts.UI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            string action = Request.QueryString[WSFederationConstants.Parameters.Action];

            try
            {
                if (action == WSFederationConstants.Actions.SignIn)
                {
                    // Process signin request.
                    var requestMessage = (SignInRequestMessage)WSFederationMessage.CreateFromUri(Request.Url);
                    if (User != null && User.Identity.IsAuthenticated)
                    {
                        var issuerName = WebConfigurationManager.AppSettings[Constants.IssuerName];
                        var signingCertificateName = WebConfigurationManager.AppSettings[Common.Constants.SigningCertificateName];
                        var encryptingCertificateName = WebConfigurationManager.AppSettings["EncryptingCertificateName"];

                        SecurityTokenService sts = new CustomSecurityTokenService(CustomSecurityTokenServiceConfiguration.GetCurrent(issuerName, signingCertificateName), encryptingCertificateName);
                        var responseMessage = FederatedPassiveSecurityTokenServiceOperations.ProcessSignInRequest(requestMessage, User, sts);

                        FederatedPassiveSecurityTokenServiceOperations.ProcessSignInResponse(responseMessage, System.Web.HttpContext.Current.Response);
                    }
                    else
                    {
                        throw new UnauthorizedAccessException();
                    }
                }
                else if (action == WSFederationConstants.Actions.SignOut)
                {
                    // Process signout request.
                    var requestMessage = (SignOutRequestMessage)WSFederationMessage.CreateFromUri(Request.Url);
                    FederatedPassiveSecurityTokenServiceOperations.ProcessSignOutRequest(requestMessage, User, requestMessage.Reply, System.Web.HttpContext.Current.Response);
                }
                else
                {
                    throw new InvalidOperationException(
                        String.Format(CultureInfo.InvariantCulture,
                                       "The action '{0}' (Request.QueryString['{1}']) is unexpected. Expected actions are: '{2}' or '{3}'.",
                                       String.IsNullOrEmpty(action) ? "<EMPTY>" : action,
                                       WSFederationConstants.Parameters.Action,
                                       WSFederationConstants.Actions.SignIn,
                                       WSFederationConstants.Actions.SignOut));
                }
            }
            catch (Exception exception)
            {
                throw new Exception("An unexpected error occurred when processing the request. See inner exception for details.", exception);
            }

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
