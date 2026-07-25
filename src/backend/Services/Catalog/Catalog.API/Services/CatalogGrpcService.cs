using Catalog.API.Protos;
using Catalog.Application.Features.Grpc.Queries.GetCategoryTrend;
using Catalog.Application.Features.Grpc.Queries.GetCheckoutData;
using Catalog.Application.Features.Grpc.Queries.GetEventInsight;
using Catalog.Application.Features.Grpc.Queries.GetOrganizerPortfolio;
using Catalog.Application.Features.Grpc.Queries.ValidateSeatLock;
using Catalog.Application.Features.Grpc.Queries.ValidateTicketTypes;
using Grpc.Core;
using MediatR;

namespace Catalog.API.Services
{
    public class CatalogGrpcService : CatalogGrpc.CatalogGrpcBase
    {
        private readonly IMediator _mediator;

        public CatalogGrpcService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task<GetCheckoutDataResponse> GetCheckoutData(
            GetCheckoutDataRequest request,
            ServerCallContext context)
        {
            try
            {
                if (!Guid.TryParse(request.EventId, out var eventId))
                    return new GetCheckoutDataResponse { IsSuccess = false, Message = "EventId không đúng định dạng Guid." };

                if (!Guid.TryParse(request.ShowtimeId, out var showtimeId))
                    return new GetCheckoutDataResponse { IsSuccess = false, Message = "ShowtimeId không đúng định dạng Guid." };

                List<GetCheckoutDataItem> items = new List<GetCheckoutDataItem>();
                foreach (var item in request.Items)
                {
                    if (!Guid.TryParse(item.TicketTypeId, out var ticketTypeId))
                        return new GetCheckoutDataResponse { IsSuccess = false, Message = $"TicketTypeId '{item.TicketTypeId}' không đúng định dạng Guid." };

                    Guid? seatId = null;
                    if (!string.IsNullOrEmpty(item.SeatId))
                    {
                        if (!Guid.TryParse(item.SeatId, out var parsedSeatId))
                            return new GetCheckoutDataResponse { IsSuccess = false, Message = $"SeatId '{item.SeatId}' không đúng định dạng Guid." };
                        seatId = parsedSeatId;
                    }

                    items.Add(new GetCheckoutDataItem
                    {
                        SeatId = seatId,
                        TicketTypeId = ticketTypeId,
                        Quantity = item.Quantity
                    });
                }

                var query = new GetCheckoutDataQuery(eventId, showtimeId, items);
                var result = await _mediator.Send(query, context.CancellationToken);

                if (!result.IsSuccess)
                {
                    return new GetCheckoutDataResponse { IsSuccess = false, Message = result.Message };
                }

                GetCheckoutDataResponse response = new GetCheckoutDataResponse
                {
                    IsSuccess = true,
                    Message = result.Message
                };

                foreach (var t in result.TicketItems)
                {
                    response.TicketItems.Add(new ValidatedCheckoutTicketItem
                    {
                        SeatId = t.SeatId?.ToString() ?? "",
                        TicketTypeId = t.TicketTypeId.ToString(),
                        TicketTypeName = t.TicketTypeName,
                        Price = (double)t.Price,
                        SeatName = t.SeatName ?? "",
                        RowName = t.RowName ?? "",
                        Quantity = t.Quantity
                    });
                }

                return response;
            }
            catch (Exception ex)
            {
                return new GetCheckoutDataResponse
                {
                    IsSuccess = false,
                    Message = $"Lỗi xử lý hệ thống: {ex.Message}"
                };
            }
        }

        public override async Task<ValidateSeatLockResponse> ValidateSeatLock(
            ValidateSeatLockRequest request,
            ServerCallContext context)
        {
            try
            {
                if (!Guid.TryParse(request.ShowtimeId, out var showtimeId))
                    return new ValidateSeatLockResponse { IsSuccess = false, Message = "ShowtimeId không đúng định dạng Guid." };

                var seatIds = new List<Guid>();
                foreach (var seatIdStr in request.SeatIds)
                {
                    if (!Guid.TryParse(seatIdStr, out var seatId))
                        return new ValidateSeatLockResponse { IsSuccess = false, Message = $"SeatId '{seatIdStr}' không đúng định dạng Guid." };
                    seatIds.Add(seatId);
                }

                var query = new ValidateSeatLockQuery(showtimeId, seatIds);
                var result = await _mediator.Send(query, context.CancellationToken);

                return new ValidateSeatLockResponse
                {
                    IsSuccess = result.IsSuccess,
                    Message = result.Message
                };
            }
            catch (Exception ex)
            {
                return new ValidateSeatLockResponse
                {
                    IsSuccess = false,
                    Message = $"Lỗi xử lý hệ thống: {ex.Message}"
                };
            }
        }

        public override async Task<ValidateTicketTypesResponse> ValidateTicketTypes(
            ValidateTicketTypesRequest request,
            ServerCallContext context)
        {
            try
            {
                if (!Guid.TryParse(request.ShowtimeId, out var showtimeId))
                    return new ValidateTicketTypesResponse { IsSuccess = false, Message = "ShowtimeId không đúng định dạng Guid." };

                var ticketItems = new List<(Guid TicketTypeId, int Quantity)>();
                foreach (var item in request.TicketItems)
                {
                    if (!Guid.TryParse(item.TicketTypeId, out var ticketTypeId))
                        return new ValidateTicketTypesResponse { IsSuccess = false, Message = $"TicketTypeId '{item.TicketTypeId}' không đúng định dạng Guid." };
                    ticketItems.Add((ticketTypeId, item.Quantity));
                }

                var query = new ValidateTicketTypesQuery(showtimeId, ticketItems);
                var result = await _mediator.Send(query, context.CancellationToken);

                return new ValidateTicketTypesResponse
                {
                    IsSuccess = result.IsSuccess,
                    Message = result.Message
                };
            }
            catch (Exception ex)
            {
                return new ValidateTicketTypesResponse
                {
                    IsSuccess = false,
                    Message = $"Lỗi xử lý hệ thống: {ex.Message}"
                };
            }
        }

        public override async Task<GetCategoryTrendResponse> GetCategoryTrend(
            GetCategoryTrendRequest request,
            ServerCallContext context)
        {
            try
            {
                if (!DateOnly.TryParse(request.From, out var from))
                    return new GetCategoryTrendResponse { IsSuccess = false, Message = "From không đúng định dạng ngày." };

                if (!DateOnly.TryParse(request.To, out var to))
                    return new GetCategoryTrendResponse { IsSuccess = false, Message = "To không đúng định dạng ngày." };

                var query = new GetCategoryTrendQuery(from, to);
                var result = await _mediator.Send(query, context.CancellationToken);

                if (!result.IsSuccess)
                    return new GetCategoryTrendResponse { IsSuccess = false, Message = result.Message };

                var response = new GetCategoryTrendResponse { IsSuccess = true, Message = result.Message };

                foreach (var c in result.Categories)
                {
                    response.Categories.Add(new CategoryTrendItem
                    {
                        CategoryId = c.CategoryId.ToString(),
                        CategoryName = c.CategoryName,
                        ViewCount = c.ViewCount,
                        PurchaseIntentCount = c.PurchaseIntentCount,
                        ActiveEventCount = c.ActiveEventCount,
                        ViewGrowthPercent = c.ViewGrowthPercent,
                        AvgOverallRating = c.AvgOverallRating,
                        RatingSampleSize = c.RatingSampleSize
                    });
                }

                return response;
            }
            catch (Exception ex)
            {
                return new GetCategoryTrendResponse
                {
                    IsSuccess = false,
                    Message = $"Lỗi xử lý hệ thống: {ex.Message}"
                };
            }
        }

        public override async Task<GetOrganizerPortfolioResponse> GetOrganizerPortfolio(
            GetOrganizerPortfolioRequest request,
            ServerCallContext context)
        {
            try
            {
                if (!Guid.TryParse(request.OrganizerId, out var organizerId))
                    return new GetOrganizerPortfolioResponse { IsSuccess = false, Message = "OrganizerId không đúng định dạng Guid." };

                if (!DateOnly.TryParse(request.From, out var from))
                    return new GetOrganizerPortfolioResponse { IsSuccess = false, Message = "From không đúng định dạng ngày." };

                if (!DateOnly.TryParse(request.To, out var to))
                    return new GetOrganizerPortfolioResponse { IsSuccess = false, Message = "To không đúng định dạng ngày." };

                var query = new GetOrganizerPortfolioQuery(organizerId, from, to);
                var result = await _mediator.Send(query, context.CancellationToken);

                if (!result.IsSuccess)
                    return new GetOrganizerPortfolioResponse { IsSuccess = false, Message = result.Message };

                var response = new GetOrganizerPortfolioResponse { IsSuccess = true, Message = result.Message };

                foreach (var c in result.CategoryDistribution)
                {
                    response.CategoryDistribution.Add(new OrganizerPortfolioCategoryItem
                    {
                        CategoryId = c.CategoryId.ToString(),
                        CategoryName = c.CategoryName,
                        EventCount = c.EventCount
                    });
                }

                foreach (var e in result.Events)
                {
                    response.Events.Add(new OrganizerPortfolioEventItem
                    {
                        EventId = e.EventId.ToString(),
                        EventTitle = e.EventTitle,
                        ViewCount = e.ViewCount,
                        PurchaseIntentCount = e.PurchaseIntentCount,
                        ConversionRate = e.ConversionRate,
                        OverallRatingAvg = e.OverallRatingAvg,
                        RatingSampleSize = e.RatingSampleSize,
                        LowestRatingDimension = e.LowestRatingDimension
                    });
                }

                return response;
            }
            catch (Exception ex)
            {
                return new GetOrganizerPortfolioResponse
                {
                    IsSuccess = false,
                    Message = $"Lỗi xử lý hệ thống: {ex.Message}"
                };
            }
        }

        public override async Task<GetEventInsightResponse> GetEventInsight(
            GetEventInsightRequest request,
            ServerCallContext context)
        {
            try
            {
                if (!Guid.TryParse(request.EventId, out var eventId))
                    return new GetEventInsightResponse { IsSuccess = false, Message = "EventId không đúng định dạng Guid." };

                if (!Guid.TryParse(request.OrganizerId, out var organizerId))
                    return new GetEventInsightResponse { IsSuccess = false, Message = "OrganizerId không đúng định dạng Guid." };

                if (!DateOnly.TryParse(request.From, out var from))
                    return new GetEventInsightResponse { IsSuccess = false, Message = "From không đúng định dạng ngày." };

                if (!DateOnly.TryParse(request.To, out var to))
                    return new GetEventInsightResponse { IsSuccess = false, Message = "To không đúng định dạng ngày." };

                var query = new GetEventInsightQuery(eventId, organizerId, from, to);
                var result = await _mediator.Send(query, context.CancellationToken);

                if (!result.IsSuccess)
                    return new GetEventInsightResponse { IsSuccess = false, Message = result.Message };

                var response = new GetEventInsightResponse
                {
                    IsSuccess = true,
                    Message = result.Message,
                    EventTitle = result.EventTitle,
                    ViewCount = result.ViewCount,
                    PurchaseIntentCount = result.PurchaseIntentCount,
                    ConversionRate = result.ConversionRate,
                    SoundAvg = result.SoundAvg,
                    VisualAvg = result.VisualAvg,
                    OrganizationAvg = result.OrganizationAvg,
                    FacilityAvg = result.FacilityAvg,
                    ServiceAvg = result.ServiceAvg,
                    PerformanceAvg = result.PerformanceAvg,
                    OverallAvg = result.OverallAvg,
                    RatingSampleSize = result.RatingSampleSize
                };

                response.RecentComments.AddRange(result.RecentComments);

                return response;
            }
            catch (Exception ex)
            {
                return new GetEventInsightResponse
                {
                    IsSuccess = false,
                    Message = $"Lỗi xử lý hệ thống: {ex.Message}"
                };
            }
        }
    }
}
