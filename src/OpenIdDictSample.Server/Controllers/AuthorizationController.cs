using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using OpenIddict.Abstractions;
using Microsoft.AspNetCore.Authentication;

namespace OpenIdDictSample.Server.Controllers
{

    public class AuthorizationController : Controller
    {
        private readonly IOpenIddictScopeManager _scopeManager;

        public AuthorizationController(IOpenIddictScopeManager scopeManager)
        {
            _scopeManager = scopeManager;
        }

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

                //populate audience (aud) claims based on requested scopes
                //ensure that you declared Resources while scope creating!
                principal.SetResources(await _scopeManager.ListResourcesAsync(principal.GetScopes()).ToListAsync());

                // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
                return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }
            if (request.IsRefreshTokenGrantType())
            {
                var claimsPrincipal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;

                return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            throw new NotImplementedException("The specified grant type is not implemented.");
        }
    }
}
