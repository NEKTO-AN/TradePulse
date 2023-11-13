namespace Domain.PumpDumpSnapshot
{
    public interface IPumpDumpSnapshotRepository
    {
      Task AddAsync(PumpDumpSnapshot entity, CancellationToken cancellationToken = default);
    }
}