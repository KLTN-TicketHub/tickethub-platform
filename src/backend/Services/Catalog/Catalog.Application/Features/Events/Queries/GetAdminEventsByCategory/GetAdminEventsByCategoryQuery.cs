using Catalog.Application.Common.DTOs.Reports;
using MediatR;

namespace Catalog.Application.Features.Events.Queries.GetAdminEventsByCategory
{
    public record GetAdminEventsByCategoryQuery(DateTime From, DateTime To) : IRequest<List<AdminEventByCategoryDto>>;
}
