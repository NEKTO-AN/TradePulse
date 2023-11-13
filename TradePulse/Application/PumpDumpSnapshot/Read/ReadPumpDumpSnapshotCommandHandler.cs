using Domain.PumpDumpSnapshot;
using MediatR;

namespace Application.PumpDumpSnapshot.Read
{
    public class ReadPumpDumpSnapshotCommandHandler : IRequestHandler<ReadPumpDumpSnapshotCommand, ReadPumpDumpSnapshotResponse>
    {
        private readonly IPumpDumpSnapshotRepository _pumpDumpSnapshotRepository;

        public ReadPumpDumpSnapshotCommandHandler(IPumpDumpSnapshotRepository pumpDumpSnapshotRepository)
        {
            _pumpDumpSnapshotRepository = pumpDumpSnapshotRepository;
        }
        public async Task<ReadPumpDumpSnapshotResponse> Handle(ReadPumpDumpSnapshotCommand request, CancellationToken cancellationToken)
        {
            List<Domain.PumpDumpSnapshot.PumpDumpSnapshot> response = await _pumpDumpSnapshotRepository.GetAsync(request.Symbol, request.FromTs, request.Count, cancellationToken);

            return new ReadPumpDumpSnapshotResponse(response);
        }
    }
}