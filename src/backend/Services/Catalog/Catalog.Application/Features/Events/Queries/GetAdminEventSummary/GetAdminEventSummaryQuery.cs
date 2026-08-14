using Catalog.Application.Common.DTOs.Reports;
using MediatR;

namespace Catalog.Application.Features.Events.Queries.GetAdminEventSummary
{
    public record GetAdminEventSummaryQuery(DateTime From, DateTime To) : IRequest<AdminEventSummaryDto>;
}
