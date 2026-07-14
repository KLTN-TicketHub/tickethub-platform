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
    public class JwtTokenService : IJwtTokenService, IDisposable
    {
        private readonly RSA _rsa;
        private readonly RsaSecurityKey _signingKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly double _tokenExpirationMinutes;

        public JwtTokenService(IOptions<AppSettings> appSettings)
        {
            AppSettings settings = appSettings.Value;
            JwtConfig jwtConfig = settings.JwtConfig
                ?? throw new InvalidOperationException("JWT config is not configured.");

            string privateKeyRelativePath = jwtConfig.PrivateKeyPath
                ?? throw new InvalidOperationException("JWT private key path is not configured.");
            _issuer = jwtConfig.ValidIssuer
                ?? throw new InvalidOperationException("JWT issuer is not configured.");
            _audience = jwtConfig.ValidAudience
                ?? throw new InvalidOperationException("JWT audience is not configured.");
            _tokenExpirationMinutes = Convert.ToDouble(jwtConfig.TokenExpirationMinutes);

            string privateKeyPath = Path.Combine(AppContext.BaseDirectory, privateKeyRelativePath);

            _rsa = RSA.Create();
            _rsa.ImportFromPem(File.ReadAllText(privateKeyPath));

            _signingKey = new RsaSecurityKey(_rsa)
            {
                KeyId = jwtConfig.KeyId ?? Guid.NewGuid().ToString()
            };
        }

        public string GenerateJwtToken(User user, IList<string> roles)
        {
            JwtSecurityTokenHandler jwtTokenHandler = new JwtSecurityTokenHandler();

            List<Claim> claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Aud, _audience),
                new Claim(JwtRegisteredClaimNames.Iss, _issuer),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.FullName),
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_tokenExpirationMinutes),
                SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.RsaSha256)
            };

            SecurityToken token = jwtTokenHandler.CreateToken(securityTokenDescriptor);
            string jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }

        public void Dispose()
        {
            _rsa?.Dispose();
        }
    }
}