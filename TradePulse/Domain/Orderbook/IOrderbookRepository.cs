namespace Domain.Orderbook
{
    public interface IOrderbookRepository
	{
		Task AddAsync(Orderbook entity, CancellationToken cancellationToken = default);
		Task<List<Orderbook>> GetAsync(string symbol, long fromTs, int count, CancellationToken cancellationToken = default);
	}
}

