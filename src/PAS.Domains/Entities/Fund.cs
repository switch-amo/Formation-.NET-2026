using PAS.Domain.Entities.Enums;

namespace PAS.Domain.Entities {
    public sealed class Fund {
        public required string Name { get; set; }
        public required string Isin { get; set; }
        public required string Currency { get; set; }
        public required FundStatusEnum Status { get; set; }
    }
}
