using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer
{
    public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        public CustomClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager
            , RoleManager<IdentityRole> roleManager
            , IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor)
        { }

        public async override Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
        {
            var principal = await base.CreateAsync(user);

            var identity = principal.Identity as ClaimsIdentity;

            if (identity != null)
            {
                if (!string.IsNullOrWhiteSpace(user.FirstName))
                {
                    identity.AddClaims(new[] { new Claim("firstname", user.FirstName) });
                }

                if (!string.IsNullOrWhiteSpace(user.LastName))
                {
                    identity.AddClaims(new[] { new Claim("lastname", user.LastName) });
                }
            }

            

            return principal;
        }
    }
}
