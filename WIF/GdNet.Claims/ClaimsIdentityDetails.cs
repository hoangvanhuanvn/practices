using Microsoft.IdentityModel.Claims;

namespace GdNet.Claims
{
    public class ClaimsIdentityDetails
    {
        private readonly IClaimsIdentity _identity;

        public ClaimsIdentityDetails(IClaimsIdentity identity)
        {
            _identity = identity;
        }

        public string Email
        {
            get { return _identity.GetClaimValue(ClaimTypes.Email); }
        }

        public string Name
        {
            get { return _identity.GetClaimValue(ClaimTypes.Name); }
        }

        public string MobilePhone
        {
            get { return _identity.GetClaimValue(ClaimTypes.MobilePhone); }
        }

        public string Webpage
        {
            get { return _identity.GetClaimValue(ClaimTypes.Webpage); }
        }
    }
}
