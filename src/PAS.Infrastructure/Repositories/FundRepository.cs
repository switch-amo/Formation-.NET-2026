using Microsoft.EntityFrameworkCore;
using PAS.Domain.Funds;
using PAS.Domain.Repositories;

namespace PAS.Asset.Infrastructure.Persistence.Repositories;

public sealed class FundRepository : IFundRepository {
    private readonly AssetDbContext _dbContext;

    public FundRepository(AssetDbContext dbContext) => _dbContext = dbContext;

    // Read path: no change tracking. Owned Navs are loaded automatically.
    public async Task<IReadOnlyList<Fund>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbContext.Funds
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    // Tracking left ON: PutFundNav needs EF to detect the changes it makes.
    public async Task<Fund?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbContext.Funds
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

    public async Task AddAsync(Fund fund, CancellationToken cancellationToken = default)
        => await _dbContext.Funds.AddAsync(fund, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _dbContext.SaveChangesAsync(cancellationToken);
}