using System.Security.Claims;
using System.Security.Principal;

namespace Palm.Static
{
    public static class IdentityExtensions
    {       

        public static string GetEmail(this IIdentity identity)
        {
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity?.FindFirst(ClaimTypes.Email);

            return claim?.Value ?? string.Empty;
        }
    }
}
