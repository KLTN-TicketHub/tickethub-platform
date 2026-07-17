namespace Catalog.Application.Features.EventClicks.Requests
{
    public class GetClickTrendRequest
    {
        public string Range { get; set; } = "30d";
    }
}
