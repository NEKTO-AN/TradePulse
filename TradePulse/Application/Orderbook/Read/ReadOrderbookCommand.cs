using MediatR;

namespace Application.Orderbook.Read
{
    public record ReadOrderbookCommand(string Symbol, long From, int Count) : IRequest<ReadOrderbookResponse>;
}

