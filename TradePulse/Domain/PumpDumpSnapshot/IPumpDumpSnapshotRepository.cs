namespace Domain.PumpDumpSnapshot
{
    public interface IPumpDumpSnapshotRepository
    {
      Task AddAsync(PumpDumpSnapshot entity, CancellationToken cancellationToken = default);
      Task<List<PumpDumpSnapshot>> GetAsync(string symbol, long fromTs, int count, CancellationToken cancellationToken = default);
    }
}