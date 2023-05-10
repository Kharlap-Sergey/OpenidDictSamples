using Microsoft.EntityFrameworkCore;
using OpenIdDictSample.Server;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<DbContext>(
    options =>
    {
        options.UseInMemoryDatabase(nameof(DbContext));
        options.UseOpenIddict();
    }
    );

builder.Services.AddOpenIddict()
    .AddCore(
    _ =>
    {
        _.UseEntityFrameworkCore()
            .UseDbContext<DbContext>();
        ////.ReplaceWithDcOAuthEntities()
        //    .UseDbContext<DbContext>();
    }
    )
    .AddServer(
    _ =>
    {
        _.AllowClientCredentialsFlow();
        //.AllowPasswordFlow()
        //.AllowRefreshTokenFlow();

        _.SetTokenEndpointUris("token");
        //       .SetIntrospectionEndpointUris(serverConfiguration.IntrospectEndpoint)
        //       .SetRevocationEndpointUris(serverConfiguration.RevokeEndpoint);

        _.AddDevelopmentEncryptionCertificate()
         .AddDevelopmentSigningCertificate();

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
