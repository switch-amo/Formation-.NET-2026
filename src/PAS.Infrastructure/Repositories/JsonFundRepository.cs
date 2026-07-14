using System.Text.Json;
using PAS.Application.Repositories;
using PAS.Domain.Entities;
using PAS.Domain.Entities.Enums;

namespace PAS.Infrastructure.Repositories;

public sealed class JsonFundRepository : IFundRepository {
    private readonly string _filePath;

    public JsonFundRepository() {
        _filePath = Path.Combine(AppContext.BaseDirectory, "Data", "funds.json");
    }

    public async Task<IReadOnlyCollection<Fund>> GetAllAsync(CancellationToken cancellationToken) {
        if (!File.Exists(_filePath)) {
            return [];
        }

        var json = await File.ReadAllTextAsync(
            _filePath,
            cancellationToken);

        var funds = JsonSerializer.Deserialize<List<FundJsonModel>>(json);

        if (funds is null) {
            return [];
        }

        return funds.Select(f => new Fund(f.Id, f.Name, f.Isin, f.Currency, Enum.Parse<FundStatusEnum>(f.Status))).ToList();
    }


    private sealed class FundJsonModel {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Isin { get; set; } = string.Empty;

        public string Currency { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
    }
}