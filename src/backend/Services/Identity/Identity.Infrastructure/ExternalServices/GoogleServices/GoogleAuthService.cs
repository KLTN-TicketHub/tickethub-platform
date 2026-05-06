using Identity.Application.Common.DTOs.Auth;
using Identity.Application.Common.Interfaces.IExternalServices.IGoogleServices;
using Identity.Common.Options;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace Identity.Infrastructure.ExternalServices.GoogleServices
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly GoogleAuthSettings _googleAuthSettings;
        private readonly IHttpClientFactory _httpClientFactory;

        public GoogleAuthService(IOptions<GoogleAuthSettings> googleAuthSettings, IHttpClientFactory httpClientFactory)
        {
            _googleAuthSettings = googleAuthSettings.Value;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<GoogleExchangeResult> ExchangeCodeAsync(string code, string redirectUri, CancellationToken cancellationToken = default)
        {
            HttpClient client = _httpClientFactory.CreateClient();

            using HttpResponseMessage resp = await client.PostAsync(
                "https://oauth2.googleapis.com/token",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["code"] = code,
                    ["client_id"] = _googleAuthSettings.ClientId,
                    ["client_secret"] = _googleAuthSettings.ClientSecret,
                    ["redirect_uri"] = redirectUri,
                    ["grant_type"] = "authorization_code"
                }), cancellationToken);

            if (!resp.IsSuccessStatusCode)
            {
                string errorContent = await resp.Content.ReadAsStringAsync(cancellationToken);
                throw new Exception($"Failed to exchange code with Google. Status Code: {resp.StatusCode}, Response: {errorContent}");
            }

            GoogleTokenResponse? token = await resp.Content.ReadFromJsonAsync<GoogleTokenResponse>(cancellationToken: cancellationToken);

            if (token == null)
                throw new Exception("Google token response empty");

            if (!string.IsNullOrWhiteSpace(token.Error))
                throw new Exception($"Google token error: {token.Error} {token.ErrorDescription}");

            ValidationSettings validationSettings = new ValidationSettings
            {
                Audience = new[] { _googleAuthSettings.ClientId }
            };

            Payload payload = await ValidateAsync(token.IdToken, validationSettings);

            GoogleTokenPayloadDto userDto = new GoogleTokenPayloadDto
            {
                Subject = payload.Subject ?? string.Empty,
                Email = payload.Email,
                EmailVerified = payload.EmailVerified,
                Name = payload.Name,
                Picture = payload.Picture
            };

            return new GoogleExchangeResult
            {
                AccessToken = token.AccessToken,
                IdToken = token.IdToken,
                RefreshToken = token.RefreshToken!,
                ExpiresIn = token.ExpiresIn,
                TokenPayload = userDto
            };
        }

        private class GoogleTokenResponse
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; } = string.Empty;

            [JsonPropertyName("expires_in")]
            public int ExpiresIn { get; set; }

            [JsonPropertyName("scope")]
            public string? Scope { get; set; }

            [JsonPropertyName("token_type")]
            public string? TokenType { get; set; }

            [JsonPropertyName("id_token")]
            public string IdToken { get; set; } = string.Empty;

            [JsonPropertyName("refresh_token")]
            public string? RefreshToken { get; set; }

            [JsonPropertyName("error")]
            public string? Error { get; set; }

            [JsonPropertyName("error_description")]
            public string? ErrorDescription { get; set; }
        }
    }
}
