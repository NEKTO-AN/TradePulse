using Domain.Orderbook;
using MediatR;

namespace Application.Orderbook.Read
{
    public class ReadOrderbookCommandHandler : IRequestHandler<ReadOrderbookCommand, ReadOrderbookResponse>
	{
        private readonly IOrderbookRepository _orderbookRepository;

        public ReadOrderbookCommandHandler(IOrderbookRepository orderbookRepository)
		{
            _orderbookRepository = orderbookRepository;
        }

        public async Task<ReadOrderbookResponse> Handle(ReadOrderbookCommand request, CancellationToken cancellationToken)
        {
            List<Domain.Orderbook.Orderbook> orderbooks = await _orderbookRepository.GetAsync(request.Symbol, request.From, request.Count);

            return new(orderbooks);
        }
    }
}

