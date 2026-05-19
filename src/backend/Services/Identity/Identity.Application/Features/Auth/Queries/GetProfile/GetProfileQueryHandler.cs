using AutoMapper;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Exceptions;
using Identity.Application.Common.DTOs.Auth;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Features.Auth.Queries.GetProfile
{
    public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, ProfileDto>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IFileService _fileService;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public GetProfileQueryHandler(
            ICurrentUserService currentUserService,
            IFileService fileService,
            UserManager<User> userManager,
            IMapper mapper)
        {
            _currentUserService = currentUserService;
            _fileService = fileService;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<ProfileDto> Handle(GetProfileQuery request, CancellationToken cancellationToken)
        {
            return await GetProfileAsync(cancellationToken);
        }

        private async Task<ProfileDto> GetProfileAsync(CancellationToken cancellationToken = default)
        {
            Guid userId = _currentUserService.UserId
                ?? throw new ValidatorException("Người dùng chưa được xác thực");

            User user = await _userManager.FindByIdAsync(userId.ToString())
                ?? throw new NotFoundException("Không tìm thấy người dùng");

            IList<string> databaseRoles = await _userManager.GetRolesAsync(user);
            IList<string> requestRoles = _currentUserService.Roles
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (requestRoles.Count > 0 && !HasSameRoles(requestRoles.ToList(), databaseRoles.ToList()))
                throw new BuildingBlocks.Domain.Exceptions.UnauthorizedAccessException("Bạn không có quyền truy cập hồ sơ này");

            user.ImageUrl = GetProfileImageUrl(user.ImageUrl);

            ProfileDto result = _mapper.Map<ProfileDto>(user);
            result.Roles = databaseRoles.ToList();

            return result;
        }

        private string? GetProfileImageUrl(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return null;

            if (imageUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                imageUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return imageUrl;
            }

            return _fileService.GetAbsoluteUrl(imageUrl);
        }

        private static bool HasSameRoles(IReadOnlyCollection<string> currentRoles, IReadOnlyCollection<string> databaseRoles)
        {
            HashSet<string> currentSet = new(currentRoles, StringComparer.OrdinalIgnoreCase);
            HashSet<string> databaseSet = new(databaseRoles, StringComparer.OrdinalIgnoreCase);

            return currentSet.SetEquals(databaseSet);
        }
    }
}