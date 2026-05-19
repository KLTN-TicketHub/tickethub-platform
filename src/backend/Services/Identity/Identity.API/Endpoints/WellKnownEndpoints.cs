using Identity.Common.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Identity.API.Endpoints;

public static class WellKnownEndpoints
{
    public static WebApplication MapWellKnownEndpoints(this WebApplication app)
    {
        var config = app.Services.GetRequiredService<IConfiguration>();
        var jwtCfg = config.GetSection("AppSettings:JwtConfig").Get<JwtConfig>()!;

        app.MapGet("/.well-known/jwks.json", () =>
        {
            string publicKeyPath = Path.Combine(AppContext.BaseDirectory, jwtCfg.PublicKeyPath!);
            string publicPem = File.ReadAllText(publicKeyPath);
            using RSA rsa = RSA.Create();
            rsa.ImportFromPem(publicPem);
            var p = rsa.ExportParameters(false);
            var jwk = new JsonWebKey
            {
                Kty = "RSA",
                Kid = jwtCfg.KeyId!,
                Alg = "RS256",
                Use = "sig",
                N = Base64UrlEncoder.Encode(p.Modulus),
                E = Base64UrlEncoder.Encode(p.Exponent)
            };
            return Results.Json(new { keys = new[] { jwk } });
        });

        app.MapGet("/.well-known/openid-configuration", () =>
        {
            var issuer = jwtCfg.ValidIssuer ?? string.Empty;
            var jwksUri = issuer.TrimEnd('/') + "/.well-known/jwks.json";
            return Results.Json(new { issuer, jwks_uri = jwksUri });
        });

        return app;
    }
}
