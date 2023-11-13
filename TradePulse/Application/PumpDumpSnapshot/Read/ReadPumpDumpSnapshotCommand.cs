using MediatR;

namespace Application.PumpDumpSnapshot.Read
{
    public record ReadPumpDumpSnapshotCommand(string Symbol, long FromTs, int Count) : IRequest<ReadPumpDumpSnapshotResponse>;
}