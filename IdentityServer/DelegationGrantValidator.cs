using IdentityServer4.Models;
using IdentityServer4.Validation;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer
{
    /*
     * https://github.com/IdentityServer/IdentityServer4/issues/775
     * https://github.com/IdentityServer/IdentityServer4/blob/dev/test/IdentityServer.IntegrationTests/Clients/Setup/DynamicParameterExtensionGrantValidator.cs
     * 
     */
    public class DelegationGrantValidator : IExtensionGrantValidator
    {
        private readonly ITokenValidator _validator;

        public DelegationGrantValidator(ITokenValidator validator)
        {
            _validator = validator;
        }

        public string GrantType => "delegation";

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var userToken = context.Request.Raw.Get("token");

            if (string.IsNullOrEmpty(userToken))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                return;
            }

            var result = await _validator.ValidateAccessTokenAsync(userToken);
            if (result.IsError)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                return;
            }

            var sub = context.Request.Raw.Get("sub");

            if (!string.IsNullOrEmpty(sub))
            {
                context.Result = new GrantValidationResult(sub, "delegation");
            }
            else
            {
                context.Result = new GrantValidationResult();
            }

            return;
        }
    }
}
