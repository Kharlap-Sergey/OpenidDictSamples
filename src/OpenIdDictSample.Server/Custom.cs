using Microsoft.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore.Models;

namespace OpenIdDictSample.Server
{
    public class OAuthEntityFrameworkCoreApplication :
        OpenIddictEntityFrameworkCoreApplication<
            int,
            OAuthEntityFrameworkCoreAuthorization,
            OAuthEntityFrameworkCoreToken
            >
    {
    }

    public class OAuthEntityFrameworkCoreAuthorization
            : OpenIddictEntityFrameworkCoreAuthorization<
                int,
                OAuthEntityFrameworkCoreApplication,
                OAuthEntityFrameworkCoreToken
                >
    {
    }

    public class OAuthEntityFrameworkCoreScope : OpenIddictEntityFrameworkCoreScope<int>
    {
    }

    public class OAuthEntityFrameworkCoreToken
            : OpenIddictEntityFrameworkCoreToken<
                int,
                OAuthEntityFrameworkCoreApplication,
                OAuthEntityFrameworkCoreAuthorization
                >
    {
    }

    public static class OAuthEntityFrameworkCoreBuilder
    {
        public static OpenIddictEntityFrameworkCoreBuilder ReplaceWithCustomOAuthEntities(
            this OpenIddictEntityFrameworkCoreBuilder builder
            )
        {
            builder.ReplaceDefaultEntities<OAuthEntityFrameworkCoreApplication,
                                     OAuthEntityFrameworkCoreAuthorization,
                                     OAuthEntityFrameworkCoreScope,
                                     OAuthEntityFrameworkCoreToken, int>();
            return builder;
        }
    }

    public static class DcOAuthEntityFrameworkCoreExtensions
    {

        public static DbContextOptionsBuilder UseCustomOAuth(this DbContextOptionsBuilder builder)
            => builder.UseOpenIddict<OAuthEntityFrameworkCoreApplication,
                                     OAuthEntityFrameworkCoreAuthorization,
                                     OAuthEntityFrameworkCoreScope,
                                     OAuthEntityFrameworkCoreToken, int>();
    }
}
