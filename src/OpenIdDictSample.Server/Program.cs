using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenIdDictSample.Server;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<DbContext>(
    options =>
    {
        //botain connection string from configuration
        //in my case it is laying in User Secrets 
        var authConnectionString = builder.Configuration.GetConnectionString("OAuthServer");

        //in case we don't provided connection string somehow, we are gonna use inmemory database.
        if(authConnectionString != null)
        {
            options.UseSqlServer(authConnectionString);
        }
        else
        {
            options.UseInMemoryDatabase(nameof(DbContext));
        }

        options.UseCustomOAuth();
    }
    );

builder.Services.AddOpenIddict()
    .AddCore(
    _ =>
    {
        _.UseEntityFrameworkCore()
            .ReplaceWithCustomOAuthEntities()
            .UseDbContext<DbContext>();
    }
    )
    .AddServer(
    _ =>
    {
        _.AllowClientCredentialsFlow();
        _.AllowRefreshTokenFlow();

        _.SetTokenEndpointUris("token");
        _.SetRevocationEndpointUris("token/revoke");
        _.SetIntrospectionEndpointUris("token/introspect");

        //don't use this in porduction
        if (builder.Environment.IsDevelopment())
        {
            _.AddDevelopmentEncryptionCertificate()
            .AddDevelopmentSigningCertificate();
        }
        else
        {
            //register real certificate here
            //like
            //should be cert in PKCE format
            var cerdata = builder.Configuration["OpenIddict:Certificate"];
            var cer = new X509Certificate2(
                Convert.FromBase64String(cerdata),
                password: (string)null,
            //it is important to use X509KeyStorageFlags.EphemeralKeySet to avoid 
            //Internal.Cryptography.CryptoThrowHelper+WindowsCryptographicException: The system cannot find the file specified.
                keyStorageFlags: X509KeyStorageFlags.EphemeralKeySet
                );
            _.AddSigningCertificate(cer)
             .AddEncryptionCertificate(cer);
        }

        //Refresh Tokens Rotation controlling
        //_.DisableRollingRefreshTokens();

        //disable access token payload encryption
        _.DisableAccessTokenEncryption();

        _.UseAspNetCore().EnableTokenEndpointPassthrough();

        //_.AddEventHandler(new Open)
    }
    );

builder.Services.AddHostedService<ClientSeeder>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();
//app.UseAuthorization();

app.MapControllers();

app.Run();
