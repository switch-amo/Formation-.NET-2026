using PAS.Domain.Funds;

namespace PAS.Domain.Repositories;

public interface IFundRepository {
    Task<IReadOnlyList<Fund>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Fund?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Fund fund, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}