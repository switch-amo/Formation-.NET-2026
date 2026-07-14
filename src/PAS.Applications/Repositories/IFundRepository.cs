using PAS.Domain.Entities;

namespace PAS.Application.Repositories;

public interface IFundRepository {
    Task<IReadOnlyCollection<Fund>> GetAllAsync(CancellationToken cancellationToken);
}