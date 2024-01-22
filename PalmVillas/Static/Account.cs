using Ardalis.GuardClauses;
using PalmVillas.DbServices;
using PalmVillas.Domain;
using System.Security.Claims;

namespace PalmVillas.Static
{
    public static class UserFunctions
    {
        internal static User GetUserByUserName(IAccountDbService accountDbService, ClaimsPrincipal User)
        {
            Guard.Against.Null(User);
            if (!User.Identity.IsAuthenticated) throw new Exception("Not authorised");
            ClaimsIdentity identity = (ClaimsIdentity)User.Identity;
            Guard.Against.Null(identity);
            Claim emailClaim = identity.FindFirst(ClaimTypes.Email);
            return accountDbService.GetUserByUserName(emailClaim.Value);
        }
    }
}
