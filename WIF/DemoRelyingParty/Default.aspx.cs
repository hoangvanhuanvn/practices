using System;
using System.Web.UI;
using GdNet.Claims;
using Microsoft.IdentityModel.Claims;

namespace DemoRelyingParty
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            txtName.Text = User.Identity.Name;

            var identity = GetClaimsIdentity();
            lblDate.Text = identity.BuildHtmlValue();
        }

        private IClaimsIdentity GetClaimsIdentity()
        {
            return User.Identity as IClaimsIdentity;
        }
    }
}