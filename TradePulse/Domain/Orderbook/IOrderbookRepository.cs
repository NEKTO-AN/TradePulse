namespace Domain.Orderbook
{
    public interface IOrderbookRepository
	{
		Task AddAsync(Orderbook entity);
	}
}

