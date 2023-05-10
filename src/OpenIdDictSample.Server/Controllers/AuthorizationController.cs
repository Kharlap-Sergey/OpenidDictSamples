using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using OpenIddict.Abstractions;

namespace OpenIdDictSample.Server.Controllers
{
    public class AuthorizationController : Controller
    {
        [HttpPost("~/token")]
        public async ValueTask<IActionResult> Exchange()
        {
            var request = HttpContext.GetOpenIddictServerRequest() ??
            throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            if (request.IsClientCredentialsGrantType())
            {
                var clientId = request.ClientId;
                var identity = new ClaimsIdentity(authenticationType: TokenValidationParameters.DefaultAuthenticationType);
                // Override the user claims present in the principal in case they
                // changed since the authorization code/refresh token was issued.
                identity.SetClaim(Claims.Subject, clientId);
                identity.SetScopes(request.GetScopes());
                var principal = new ClaimsPrincipal(identity);
                // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
                return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            throw new NotImplementedException("The specified grant type is not implemented.");
        }
    }
}
