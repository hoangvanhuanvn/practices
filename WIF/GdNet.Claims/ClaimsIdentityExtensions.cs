using System.Linq;
using Microsoft.IdentityModel.Claims;

namespace GdNet.Claims
{
    public static class ClaimsIdentityExtensions
    {
        /// <param name="identity">The identity</param>
        /// <param name="claimType">should be a value of <see cref="ClaimTypes"/></param>
        public static Claim GetClaim(this IClaimsIdentity identity, string claimType)
        {
            return identity.Claims.FirstOrDefault(claim => claim.ClaimType == claimType);
        }

        /// <param name="identity">The identity</param>
        /// <param name="claimType">should be a value of <see cref="ClaimTypes"/></param>
        public static string GetClaimValue(this IClaimsIdentity identity, string claimType)
        {
            var claim = identity.GetClaim(claimType);
            return (claim == null) ? null : claim.Value;
        }

        public static string BuildHtmlValue(this IClaimsIdentity identity)
        {
            return string.Join("<br />", identity.Claims.Select(claim => claim.ToString()));
        }
    }
}
