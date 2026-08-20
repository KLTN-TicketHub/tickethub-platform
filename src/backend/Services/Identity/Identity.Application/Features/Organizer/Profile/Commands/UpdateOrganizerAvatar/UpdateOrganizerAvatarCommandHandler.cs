using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Events.Organizer;
using BuildingBlocks.Domain.Exceptions;
using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SixLabors.ImageSharp;

namespace Identity.Application.Features.Organizer.Profile.Commands.UpdateOrganizerAvatar
{
    public class UpdateOrganizerAvatarCommandHandler : IRequestHandler<UpdateOrganizerAvatarCommand, string>
    {
        private static readonly string[] AllowedContentTypes = { "image/jpeg", "image/jpg", "image/png", "image/webp" };

        private readonly UserManager<User> _userManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFileService _fileService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateOrganizerAvatarCommandHandler(
            UserManager<User> userManager,
            ICurrentUserService currentUserService,
            IFileService fileService,
            IEventPublisher eventPublisher,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _currentUserService = currentUserService;
            _fileService = fileService;
            _eventPublisher = eventPublisher;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(UpdateOrganizerAvatarCommand command, CancellationToken cancellationToken)
        {
            return await UpdateOrganizerAvatarAsync(command.File, cancellationToken);
        }

        private async Task<string> UpdateOrganizerAvatarAsync(IFormFile file, CancellationToken cancellationToken = default)
        {
            CheckAvatarFile(file);
            await CheckAvatarDimensionsAsync(file, cancellationToken);

            Guid organizerId = _currentUserService.UserId
                ?? throw new BuildingBlocks.Domain.Exceptions.UnauthorizedAccessException("Bạn cần đăng nhập để thực hiện chức năng này.");

            User user = await _userManager.FindByIdAsync(organizerId.ToString())
                ?? throw new NotFoundException($"Không tìm thấy tài khoản với ID {organizerId}.");

            string relativeUrl = await _fileService.SaveFileAsync(file, "organizers/avatars", cancellationToken);

            user.ImageUrl = relativeUrl;
            user.SetUpdated(organizerId);

            IdentityResult updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                throw new BusinessRuleException(
                    $"Cập nhật ảnh đại diện thất bại: {string.Join(", ", updateResult.Errors.Select(e => e.Description))}");

            await _eventPublisher.PublishAsync(new OrganizerAvatarUpdatedEvent
            {
                Id = organizerId,
                ImageUrl = relativeUrl
            }, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _fileService.GetAbsoluteUrl(relativeUrl);
        }

        private static void CheckAvatarFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new BusinessRuleException("Ảnh đại diện không được để trống.");

            if (!AllowedContentTypes.Contains(file.ContentType))
                throw new BusinessRuleException("Ảnh đại diện phải có định dạng JPG, PNG hoặc WebP.");
        }

        private static async Task CheckAvatarDimensionsAsync(IFormFile file, CancellationToken cancellationToken)
        {
            using Stream stream = file.OpenReadStream();
            using Image image = await Image.LoadAsync(stream, cancellationToken);

            if (image.Width != 275 || image.Height != 275)
                throw new BusinessRuleException(
                    $"Ảnh đại diện phải có kích thước 275×275 px. Kích thước hiện tại: {image.Width}×{image.Height} px.");
        }
    }
}
