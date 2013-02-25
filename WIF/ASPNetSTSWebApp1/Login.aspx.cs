using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace ASPNetSTSWebApp1
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Note: Add code to validate user name, password. This code is for illustrative purpose only.
            // Do not use it in production environment.
            if (!string.IsNullOrEmpty(txtUserName.Text))
            {
                if (Request.QueryString["ReturnUrl"] != null)
                {
                    FormsAuthentication.RedirectFromLoginPage(txtUserName.Text, false);
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(txtUserName.Text, false);
                    Response.Redirect("default.aspx");
                }
            }
            else if (!IsPostBack)
            {
                txtUserName.Text = "Adam Carter";
            }
        }
    }
}