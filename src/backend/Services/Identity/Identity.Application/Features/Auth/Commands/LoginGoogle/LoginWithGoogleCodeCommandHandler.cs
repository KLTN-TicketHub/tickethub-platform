using Identity.Application.Common.DTOs.Auth;
using Identity.Application.Common.Interfaces.IExternalServices.ITokenServices;
using Identity.Common.Options;
using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.Auth.Commands.LoginGoogle
{
    public class LoginWithGoogleCodeCommandHandler : IRequestHandler<LoginWithGoogleCodeCommand, AuthDto>
    {
        private readonly GoogleAuthSettings _googleSettings;
        private readonly HttpClient _httpClient;
        private readonly UserManager<User> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUnitOfWork _unitOfWork;

        public LoginWithGoogleCodeCommandHandler(
            IOptions<GoogleAuthSettings> googleSettings,
            HttpClient httpClient,
            UserManager<User> userManager,
            IJwtTokenService jwtTokenService,
            IUnitOfWork unitOfWork)
        {
            _googleSettings = googleSettings.Value;
            _httpClient = httpClient;
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _unitOfWork = unitOfWork;
        }

        public Task<AuthDto> Handle(LoginWithGoogleCodeCommand request, CancellationToken cancellationToken)
        {
            return LoginWithGoogleAsync(request.Request.Code, cancellationToken);
        }

        private async Task<AuthDto> LoginWithGoogleAsync(string code, CancellationToken cancellationToken = default)
        {
            // 1. Exchange code for access token
            // 2. Use access token to get user info from Google
            // 3. Check if user exists in the database, if not create a new user
            // 4. Generate JWT token and refresh token for the user
            // 5. Return AuthDto with tokens
            throw new NotImplementedException();
        }
    }
}
