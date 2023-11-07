namespace Domain.Orderbook
{
    public interface IOrderbookRepository
	{
		Task AddAsync(Orderbook entity);
		Task<List<Orderbook>> GetAsync(string symbol, long fromTs, int count);
	}
}

