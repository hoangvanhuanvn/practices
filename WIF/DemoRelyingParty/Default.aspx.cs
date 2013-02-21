using System;
using System.Linq;
using System.Threading;
using System.Web.UI;
using Microsoft.IdentityModel.Claims;

namespace DemoRelyingParty
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            txtName.Text = User.Identity.Name;
            var identity = ((IClaimsPrincipal)Thread.CurrentPrincipal).Identities[0];
            var dateOfBirth = identity.Claims.FirstOrDefault(x => x.ClaimType == ClaimTypes.DateOfBirth);
            if (dateOfBirth != null)
            {
                lblDate.Text = dateOfBirth.Value;
            }
        }
    }
}