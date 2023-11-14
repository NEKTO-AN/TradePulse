using Domain.Orderbook;

namespace Application.Services.DataConsumerService
{
    public record AddOrderbookItemQuery(OrderbookDataMessage OrderbookDataMessage);
    public record FindAnomalyQuery(string Symbol);
}