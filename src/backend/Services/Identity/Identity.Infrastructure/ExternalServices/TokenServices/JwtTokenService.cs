using Identity.Application.Common.Interfaces.IExternalServices.ITokenServices;
using Identity.Common.Options;
using Identity.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Identity.Infrastructure.ExternalServices.TokenServices
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly AppSettings _appSettings;
        public JwtTokenService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        public string GenerateJwtToken(User user, IList<string> roles)
        {
            JwtSecurityTokenHandler jwtTokenHandler = new JwtSecurityTokenHandler();
            JwtConfig jwtConfig = _appSettings.JwtConfig
                ?? throw new InvalidOperationException("JWT config is not configured.");

            string privateKeyRelativePath = jwtConfig.PrivateKeyPath
                ?? throw new InvalidOperationException("JWT private key path is not configured.");
            string issuer = jwtConfig.ValidIssuer
                ?? throw new InvalidOperationException("JWT issuer is not configured.");
            string audience = jwtConfig.ValidAudience
                ?? throw new InvalidOperationException("JWT audience is not configured.");

            string privateKeyPath = Path.Combine(AppContext.BaseDirectory, privateKeyRelativePath);

            RSA rsa = RSA.Create();
            rsa.ImportFromPem(File.ReadAllText(privateKeyPath));

            List<Claim> claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Aud, audience),
                new Claim(JwtRegisteredClaimNames.Iss, issuer),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.FullName),
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var rsaKey = new RsaSecurityKey(rsa)
            {
                KeyId = jwtConfig.KeyId ?? Guid.NewGuid().ToString()
            };

            SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_appSettings.JwtConfig.TokenExpirationMinutes)),
                SigningCredentials = new SigningCredentials(
                    rsaKey,
                    SecurityAlgorithms.RsaSha256)
            };

            SecurityToken token = jwtTokenHandler.CreateToken(securityTokenDescriptor);
            string jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }
    }
}
