using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddAuthentication(
    defaultScheme: JwtBearerDefaults.AuthenticationScheme
    )
    .AddJwtBearer(
    //JwtBearerDefaults.AuthenticationScheme is default value, and you don't need to specify it.
    authenticationScheme: JwtBearerDefaults.AuthenticationScheme,
    _ =>
    {
        //validation property
        //checks for whom this token should be issued for
        _.Audience = "test_resource";
        //validation property
        //by whom this token should be issued 
        _.Authority = "https://localhost:7153";
    }
    );

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
